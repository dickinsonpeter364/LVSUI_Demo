#include "pch.h"
#include <tesseract/baseapi.h>
#include <opencv2/opencv.hpp>
#include <opencv2/objdetect/barcode.hpp>
#include <algorithm>
#include <opencv2/imgproc.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/dnn.hpp>
#include <string>
#include <fstream>
#include <numeric>
#include <algorithm>
#include <filesystem>
#include <strstream>
#include "ImageProcessor.h"


using namespace cv;
double ImageProcessor::m_dMaxOverlap = 0;
double ImageProcessor::m_dScore = 0.5;
double ImageProcessor::m_dToleranceAngle = 0;
int ImageProcessor::m_iMinReduceArea = 256;
int ImageProcessor::m_iMaxPos = 70;
double ImageProcessor::m_dTolerance2 = 60;
double ImageProcessor::m_dTolerance3 = -110;
double ImageProcessor::m_dTolerance4 = -100;
bool ImageProcessor::m_bStopLayer1 = false;
bool ImageProcessor::m_bToleranceRange = true;

cv::Mat ImageProcessor::m_current_pattern;
cv::Mat ImageProcessor::m_matSrc;
double ImageProcessor::m_dSrcScale=1;
double ImageProcessor::m_dDstScale=1;

s_TemplData ImageProcessor::m_TemplData;
bool ImageProcessor::m_bDebugMode=false;
double ImageProcessor::m_dTolerance1=40;

std::vector<s_SingleTargetMatch> ImageProcessor::m_vecSingleTargetData;

bool compareScoreBig2Small(const s_MatchParameter& lhs, const s_MatchParameter& rhs) { return  lhs.dMatchScore > rhs.dMatchScore; }
bool comparePtWithAngle(const std::pair<Point2f, double> lhs, const std::pair<Point2f, double> rhs) { return lhs.second < rhs.second; }
bool compareMatchResultByPos(const s_SingleTargetMatch& lhs, const s_SingleTargetMatch& rhs)
{
    double dTol = 2;
    if (fabs(lhs.ptCenter.y - rhs.ptCenter.y) <= dTol)
        return lhs.ptCenter.x < rhs.ptCenter.x;
    else
        return lhs.ptCenter.y < rhs.ptCenter.y;

};
bool compareMatchResultByScore(const s_SingleTargetMatch& lhs, const s_SingleTargetMatch& rhs) { return lhs.dMatchScore > rhs.dMatchScore; }
bool compareMatchResultByPosX(const s_SingleTargetMatch& lhs, const s_SingleTargetMatch& rhs) { return lhs.ptCenter.x < rhs.ptCenter.x; }

inline int _mm_hsum_epi32(__m128i V)      // V3 V2 V1 V0
{
    __m128i T = _mm_add_epi32(V, _mm_srli_si128(V, 8));  // V3+V1   V2+V0  V1  V0  
    T = _mm_add_epi32(T, _mm_srli_si128(T, 4));    // V3+V1+V2+V0  V2+V0+V1 V1+V0 V0 
    return _mm_cvtsi128_si32(T);
}

inline int IM_Conv_SIMD(unsigned char* pCharKernel, unsigned char* pCharConv, int iLength)
{
    const int iBlockSize = 16, Block = iLength / iBlockSize;
    __m128i SumV = _mm_setzero_si128();
    __m128i Zero = _mm_setzero_si128();
    for (int Y = 0; Y < Block * iBlockSize; Y += iBlockSize)
    {
        __m128i SrcK = _mm_loadu_si128((__m128i*)(pCharKernel + Y));
        __m128i SrcC = _mm_loadu_si128((__m128i*)(pCharConv + Y));
        __m128i SrcK_L = _mm_unpacklo_epi8(SrcK, Zero);
        __m128i SrcK_H = _mm_unpackhi_epi8(SrcK, Zero);
        __m128i SrcC_L = _mm_unpacklo_epi8(SrcC, Zero);
        __m128i SrcC_H = _mm_unpackhi_epi8(SrcC, Zero);
        __m128i SumT = _mm_add_epi32(_mm_madd_epi16(SrcK_L, SrcC_L), _mm_madd_epi16(SrcK_H, SrcC_H));
        SumV = _mm_add_epi32(SumV, SumT);
    }
    int Sum = _mm_hsum_epi32(SumV);
    for (int Y = Block * iBlockSize; Y < iLength; Y++)
    {
        Sum += pCharKernel[Y] * pCharConv[Y];
    }
    return Sum;
}

class WatershedSegmenter {
private:
    cv::Mat markers;
public:
    void setMarkers(cv::Mat& markerImage)
    {
        markerImage.convertTo(markers, CV_32S);
    }

    cv::Mat process(cv::Mat& image)
    {
        cv::watershed(image, markers);
        markers.convertTo(markers, CV_8U);
        return markers;
    }
};
void ImageProcessor::watershed( Mat& image)
{
    cv::Mat binary;// = cv::imread(argv[2], 0);
    cv::cvtColor(image, binary, COLOR_BGR2GRAY);
    cv::threshold(binary, binary, 100, 255, THRESH_BINARY);

    imwrite("originalimage.png", image);
    imwrite("originalbinary.png", binary);

    // Eliminate noise and smaller objects
    cv::Mat fg;
    cv::erode(binary, fg, cv::Mat(), cv::Point(-1, -1), 2);
    imwrite("fg.png", fg);

    // Identify image pixels without objects
    cv::Mat bg;
    cv::dilate(binary, bg, cv::Mat(), cv::Point(-1, -1), 3);
    cv::threshold(bg, bg, 1, 128, cv::THRESH_BINARY_INV);
    imwrite("bg.png", bg);

    // Create markers image
    cv::Mat markers(binary.size(), CV_8U, cv::Scalar(0));
    markers = fg + bg;
    imwrite("markers.png", markers);

    // Create watershed segmentation object
    WatershedSegmenter segmenter;
    segmenter.setMarkers(markers);

    cv::Mat result = segmenter.process(image);
    result.convertTo(result, CV_8U);
    imwrite("final_result.png", result);

}

void ImageProcessor::get_masked_image(const UMat& image, UMat& maskedImage, UMat& foreground, int fileNum, bool with_cropped)
{
    if (with_cropped)
        foreground = crop_image_from_background(image, 72);
    else
        foreground = image.clone();
    double minVal, maxVal;
    minMaxLoc(image, &minVal, &maxVal);
    UMat thr;
    threshold(foreground, thr, (maxVal - minVal) / 4.5, 255, THRESH_BINARY);
    maskedImage = thr.clone();
}

UMat ImageProcessor::crop_image_from_background(const UMat& image, int thresholdValue)
{
    CV_Assert(!image.empty());
    UMat gray;
    if (image.channels() == 3)
        cvtColor(image, gray, COLOR_BGR2GRAY);
    else if (image.channels() == 4)
        cvtColor(image, gray, COLOR_BGRA2GRAY);
    else
        gray = image;

    UMat mask;
    if (thresholdValue == 0)
        threshold(gray, mask, 0, 255, THRESH_BINARY | THRESH_OTSU);
    else
        threshold(gray, mask, thresholdValue, 255, THRESH_BINARY);

    std::vector<std::vector<Point>> contours;
    cv::findContours(mask, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);
    imwrite("mask.png", mask);

    if (contours.empty())
        return gray;

    UMat copy = image.clone();
    drawContours(copy, contours, -1, Scalar(255, 255, 255), 20);
    imwrite("contours.png", copy);

    auto cnt = contours[0];
    for (const auto& c : contours) {
        if (contourArea(c) > contourArea(cnt))
            cnt = c;
    }
    Rect foreRect = boundingRect(cnt);
    UMat foreground = extract_rect(image, foreRect);
    imwrite("foreground.png", foreground);
    return foreground;
}

UMat ImageProcessor::extract_rect(const UMat& image, const Rect& rect)
{
    const Rect valid_rect = Rect(0, 0, image.cols, image.rows);
    if (valid_rect.width <= 0 || valid_rect.height <= 0)
        return {};
    return image(rect).clone();
}

void ImageProcessor::apply_adaptive_histogram_eq(const Mat& before, Mat& after)
{
    Ptr<CLAHE> ptr = createCLAHE();
    ptr->setClipLimit(4);
    ptr->apply(before, after);
}

Mat ImageProcessor::extract_dark_text_mask(const Mat& sourceImage)
{
    Mat grayImage;
    if (sourceImage.channels() == 3)
        cvtColor(sourceImage, grayImage, COLOR_BGR2GRAY);
    else if (sourceImage.channels() == 4)
        cvtColor(sourceImage, grayImage, COLOR_BGRA2GRAY);
    else
        grayImage = sourceImage.clone();

    Mat binaryMask;
    int blockSize = 15;
    double C = 7.0;
    adaptiveThreshold(grayImage, binaryMask, 255, ADAPTIVE_THRESH_GAUSSIAN_C, THRESH_BINARY_INV, blockSize, C);

    Mat kernel = getStructuringElement(MORPH_RECT, Size(3, 3));
    Mat cleanedMask;
    morphologyEx(binaryMask, cleanedMask, MORPH_OPEN, kernel);
    return cleanedMask;
}

cv::Mat ImageProcessor::segment_by_background_intensity(const cv::Mat& grayImage, int k) {
    if (grayImage.channels() != 1) {
        std::cerr << "Error: segment_by_background_intensity requires a grayscale image." << std::endl;
        return grayImage;
    }

    cv::Mat samples(grayImage.rows * grayImage.cols, 1, CV_32F);
    grayImage.reshape(1, grayImage.rows * grayImage.cols).convertTo(samples, CV_32F);

    cv::Mat labels;
    cv::Mat centers;
    cv::TermCriteria criteria(cv::TermCriteria::EPS + cv::TermCriteria::COUNT, 10, 1.0);

    cv::kmeans(samples, k, labels, criteria, 10, cv::KMEANS_PP_CENTERS, centers);

    cv::Mat segmentedImage(grayImage.size(), CV_8U);
    for (int r = 0; r < grayImage.rows; ++r) {
        for (int c = 0; c < grayImage.cols; ++c) {
            int cluster_idx = labels.at<int>(r * grayImage.cols + c, 0);
            float center_intensity = centers.at<float>(cluster_idx, 0);
            segmentedImage.at<uchar>(r, c) = cv::saturate_cast<uchar>(center_intensity);
        }
    }
    return segmentedImage;
}

cv::Mat detectTextEAST(const cv::Mat& sourceImage, const std::string& modelPath,
    float confThreshold = 0.5, float nmsThreshold = 0.4) {

    // --- 1. PREPARE INPUT AND LOAD MODEL ---

    // Create a copy to draw on
    cv::Mat resultImage = sourceImage.clone();

    // EAST expects a 3-channel BGR image.
    if (resultImage.channels() == 1) {
        cv::cvtColor(resultImage, resultImage, cv::COLOR_GRAY2BGR);
    }
    else if (resultImage.channels() == 4) {
        cv::cvtColor(resultImage, resultImage, cv::COLOR_BGRA2BGR);
    }

    // Load the network
    cv::dnn::Net net = cv::dnn::readNet(modelPath);

    // Define the input size for the EAST model (must be a multiple of 32)
    int inputWidth = 320;
    int inputHeight = 320;

    // Get original image size and calculate scaling ratios
    int origWidth = resultImage.cols;
    int origHeight = resultImage.rows;
    float rW = (float)origWidth / (float)inputWidth;
    float rH = (float)origHeight / (float)inputHeight;

    // Create a blob from the image.
    // Mean subtraction values (123.68, 116.78, 103.94) are standard for EAST.
    cv::Mat blob;
    cv::dnn::blobFromImage(resultImage, blob, 1.0, cv::Size(inputWidth, inputHeight),
        cv::Scalar(123.68, 116.78, 103.94), true, false);

    // --- 2. RUN THE MODEL ---
    net.setInput(blob);

    // EAST has two output layers: scores and geometry
    std::vector<std::string> outputLayersNames = { "feature_fusion/Conv_7/Sigmoid", "feature_fusion/concat_3" };
    std::vector<cv::Mat> outputs;
    net.forward(outputs, outputLayersNames);

    cv::Mat scores = outputs[0];
    cv::Mat geometry = outputs[1];

    // --- 3. DECODE THE OUTPUT ---
    // This part is complex as we must interpret the raw model output

    std::vector<cv::RotatedRect> boxes;
    std::vector<float> confidences;

    int numRows = scores.size[2]; // Feature map height (e.g., 80)
    int numCols = scores.size[3]; // Feature map width (e.g., 80)

    for (int y = 0; y < numRows; ++y) {
        const float* scoresData = scores.ptr<float>(0, 0, y);
        const float* geomData0 = geometry.ptr<float>(0, 0, y);
        const float* geomData1 = geometry.ptr<float>(0, 1, y);
        const float* geomData2 = geometry.ptr<float>(0, 2, y);
        const float* geomData3 = geometry.ptr<float>(0, 3, y);
        const float* geomData4 = geometry.ptr<float>(0, 4, y);

        for (int x = 0; x < numCols; ++x) {
            float score = scoresData[x];
            if (score < confThreshold) {
                continue;
            }

            // The feature map is 1/4 the size of the input blob
            float offsetX = x * 4.0f;
            float offsetY = y * 4.0f;

            // Get angle and geometry
            float angle = geomData4[x];
            float cosA = std::cos(angle);
            float sinA = std::sin(angle);

            float h = geomData0[x] + geomData2[x]; // top + bottom
            float w = geomData1[x] + geomData3[x]; // left + right

            // Calculate the center point of the rotated box
            cv::Point2f center(offsetX + (cosA * geomData1[x]) + (sinA * geomData2[x]),
                offsetY - (sinA * geomData1[x]) + (cosA * geomData2[x]));

            // EAST outputs angle in radians, RotatedRect expects degrees
            boxes.push_back(cv::RotatedRect(center, cv::Size2f(w, h), -angle * 180.0f / CV_PI));
            confidences.push_back(score);
        }
    }

    // --- 4. APPLY NON-MAXIMUM SUPPRESSION (NMS) ---
    std::vector<int> indices;
    cv::dnn::NMSBoxes(boxes, confidences, confThreshold, nmsThreshold, indices);

    // --- 5. DRAW THE FINAL BOXES ---
    for (int idx : indices) {
        cv::RotatedRect box = boxes[idx];
        cv::Point2f vertices[4];
        box.points(vertices);

        for (int j = 0; j < 4; ++j) {
            // Scale the vertices back to the *original* image size
            vertices[j].x *= rW;
            vertices[j].y *= rH;
        }

        // Draw the 4 lines of the rotated rectangle in red
        for (int j = 0; j < 4; ++j) {
            cv::line(resultImage, vertices[j], vertices[(j + 1) % 4], cv::Scalar(0, 0, 255), 2);
        }
    }

    return resultImage;
}

std::vector<cv::Mat> ImageProcessor::extractBackgroundAreas(const cv::Mat& grayImage, int k) {
    if (grayImage.channels() != 1) {
        std::cerr << "Error: extractBackgroundAreas requires a grayscale image." << std::endl;
        return {};
    }

    cv::Mat samples(grayImage.rows * grayImage.cols, 1, CV_32F);
    grayImage.reshape(1, grayImage.rows * grayImage.cols).convertTo(samples, CV_32F);

    cv::Mat labels;
    cv::Mat centers;
    cv::TermCriteria criteria(cv::TermCriteria::EPS + cv::TermCriteria::COUNT, 10, 1.0);

    cv::kmeans(samples, k, labels, criteria, 10, cv::KMEANS_PP_CENTERS, centers);

    std::vector<cv::Mat> backgroundMasks(k, cv::Mat::zeros(grayImage.size(), CV_8U));
    for (int r = 0; r < grayImage.rows; ++r) {
        for (int c = 0; c < grayImage.cols; ++c) {
            int cluster_idx = labels.at<int>(r * grayImage.cols + c, 0);
            backgroundMasks[cluster_idx].at<uchar>(r, c) = 255;
        }
    }
    return backgroundMasks;
}

void ImageProcessor::SetCurrentPattern(const Mat& pattern)
{
    m_current_pattern = pattern.clone();
}

void ImageProcessor::SetSourceImage(const Mat& src)
{
    m_matSrc = src.clone();
}

Point2f ImageProcessor::ptRotatePt2f(Point2f ptInput, Point2f ptOrg, double dAngle)
{
    double dWidth = ptOrg.x * 2;
    double dHeight = ptOrg.y * 2;
    double dY1 = dHeight - ptInput.y, dY2 = dHeight - ptOrg.y;

    double dX = (ptInput.x - ptOrg.x) * cos(dAngle) - (dY1 - ptOrg.y) * sin(dAngle) + ptOrg.x;
    double dY = (ptInput.x - ptOrg.x) * sin(dAngle) + (dY1 - ptOrg.y) * cos(dAngle) + dY2;

    dY = -dY + dHeight;
    return Point2f((float)dX, (float)dY);
}

void ImageProcessor::ccoeff_denominator(cv::Mat& matSrc, s_TemplData* pTemplData, cv::Mat& matResult, int iLayer)
{
    if (pTemplData->vecResultEqual1[iLayer])
    {
        matResult = Scalar::all(1);
        return;
    }
    double* q0 = 0, * q1 = 0, * q2 = 0, * q3 = 0;

    Mat sum, sqsum;
    integral(matSrc, sum, sqsum, CV_64F);

    q0 = (double*)sqsum.data;
    q1 = q0 + pTemplData->vecPyramid[iLayer].cols;
    q2 = (double*)(sqsum.data + pTemplData->vecPyramid[iLayer].rows * sqsum.step);
    q3 = q2 + pTemplData->vecPyramid[iLayer].cols;

    double* p0 = (double*)sum.data;
    double* p1 = p0 + pTemplData->vecPyramid[iLayer].cols;
    double* p2 = (double*)(sum.data + pTemplData->vecPyramid[iLayer].rows * sum.step);
    double* p3 = p2 + pTemplData->vecPyramid[iLayer].cols;

    int sumstep = sum.data ? (int)(sum.step / sizeof(double)) : 0;
    int sqstep = sqsum.data ? (int)(sqsum.step / sizeof(double)) : 0;

    //
    double dTemplMean0 = pTemplData->vecTemplMean[iLayer][0];
    double dTemplNorm = pTemplData->vecTemplNorm[iLayer];
    double dInvArea = pTemplData->vecInvArea[iLayer];
    //

    int i, j;
    for (i = 0; i < matResult.rows; i++)
    {
        float* rrow = matResult.ptr<float>(i);
        int idx = i * sumstep;
        int idx2 = i * sqstep;

        for (j = 0; j < matResult.cols; j += 1, idx += 1, idx2 += 1)
        {
            double num = rrow[j], t;
            double wndMean2 = 0, wndSum2 = 0;

            t = p0[idx] - p1[idx] - p2[idx] + p3[idx];
            wndMean2 += t * t;
            num -= t * dTemplMean0;
            wndMean2 *= dInvArea;


            t = q0[idx2] - q1[idx2] - q2[idx2] + q3[idx2];
            wndSum2 += t;


            //t = std::sqrt (MAX (wndSum2 - wndMean2, 0)) * dTemplNorm;

            double diff2 = MAX(wndSum2 - wndMean2, 0);
            if (diff2 <= std::min(0.5, 10 * FLT_EPSILON * wndSum2))
                t = 0; // avoid rounding errors
            else
                t = std::sqrt(diff2) * dTemplNorm;

            if (fabs(num) < t)
                num /= t;
            else if (fabs(num) < t * 1.125)
                num = num > 0 ? 1 : -1;
            else
                num = 0;

            rrow[j] = (float)num;
        }
    }
}
void ImageProcessor::MatchTemplate(Mat& matSrc, s_TemplData* pTemplData, Mat& matResult, int iLayer, bool bUseSIMD)
{
    if (bUseSIMD)
    {
        matResult.create(matSrc.rows - pTemplData->vecPyramid[iLayer].rows + 1,
            matSrc.cols - pTemplData->vecPyramid[iLayer].cols + 1, CV_32FC1);
        matResult.setTo(0);
        cv::Mat& matTemplate = pTemplData->vecPyramid[iLayer];

        int  t_r_end = matTemplate.rows, t_r = 0;
        for (int r = 0; r < matResult.rows; r++)
        {
            float* r_matResult = matResult.ptr<float>(r);
            uchar* r_source = matSrc.ptr<uchar>(r);
            uchar* r_template, * r_sub_source;
            for (int c = 0; c < matResult.cols; ++c, ++r_matResult, ++r_source)
            {
                r_template = matTemplate.ptr<uchar>();
                r_sub_source = r_source;
                for (t_r = 0; t_r < t_r_end; ++t_r, r_sub_source += matSrc.cols, r_template += matTemplate.cols)
                {
                    *r_matResult = *r_matResult + IM_Conv_SIMD(r_template, r_sub_source, matTemplate.cols);
                }
            }
        }
    }
    else
        matchTemplate(matSrc, pTemplData->vecPyramid[iLayer], matResult, TM_CCORR);

    /*Mat diff;
    absdiff(matResult, matResult, diff);
    double dMaxValue;
    minMaxLoc(diff, 0, &dMaxValue, 0,0);*/
    ccoeff_denominator(matSrc, pTemplData, matResult, iLayer);
}
Size ImageProcessor::GetBestRotationSize(Size sizeSrc, Size sizeDst, double dRAngle)
{
    double dRAngle_radian = dRAngle * D2R;
    Point ptLT(0, 0), ptLB(0, sizeSrc.height - 1), ptRB(sizeSrc.width - 1, sizeSrc.height - 1), ptRT(sizeSrc.width - 1, 0);
    Point2f ptCenter((sizeSrc.width - 1) / 2.0f, (sizeSrc.height - 1) / 2.0f);
    Point2f ptLT_R = ptRotatePt2f(Point2f(ptLT), ptCenter, dRAngle_radian);
    Point2f ptLB_R = ptRotatePt2f(Point2f(ptLB), ptCenter, dRAngle_radian);
    Point2f ptRB_R = ptRotatePt2f(Point2f(ptRB), ptCenter, dRAngle_radian);
    Point2f ptRT_R = ptRotatePt2f(Point2f(ptRT), ptCenter, dRAngle_radian);

    float fTopY = max(max(ptLT_R.y, ptLB_R.y), max(ptRB_R.y, ptRT_R.y));
    float fBottomY = min(min(ptLT_R.y, ptLB_R.y), min(ptRB_R.y, ptRT_R.y));
    float fRightX = max(max(ptLT_R.x, ptLB_R.x), max(ptRB_R.x, ptRT_R.x));
    float fLeftX = min(min(ptLT_R.x, ptLB_R.x), min(ptRB_R.x, ptRT_R.x));

    if (dRAngle > 360)
        dRAngle -= 360;
    else if (dRAngle < 0)
        dRAngle += 360;

    if (fabs(fabs(dRAngle) - 90) < VISION_TOLERANCE || fabs(fabs(dRAngle) - 270) < VISION_TOLERANCE)
    {
        return Size(sizeSrc.height, sizeSrc.width);
    }
    else if (fabs(dRAngle) < VISION_TOLERANCE || fabs(fabs(dRAngle) - 180) < VISION_TOLERANCE)
    {
        return sizeSrc;
    }

    double dAngle = dRAngle;

    if (dAngle > 0 && dAngle < 90)
    {
        ;
    }
    else if (dAngle > 90 && dAngle < 180)
    {
        dAngle -= 90;
    }
    else if (dAngle > 180 && dAngle < 270)
    {
        dAngle -= 180;
    }
    else if (dAngle > 270 && dAngle < 360)
    {
        dAngle -= 270;
    }

    float fH1 = sizeDst.width * sin(dAngle * D2R) * cos(dAngle * D2R);
    float fH2 = sizeDst.height * sin(dAngle * D2R) * cos(dAngle * D2R);

    int iHalfHeight = (int)ceil(fTopY - ptCenter.y - fH1);
    int iHalfWidth = (int)ceil(fRightX - ptCenter.x - fH2);

    Size sizeRet(iHalfWidth * 2, iHalfHeight * 2);

    bool bWrongSize = (sizeDst.width < sizeRet.width && sizeDst.height > sizeRet.height)
        || (sizeDst.width > sizeRet.width && sizeDst.height < sizeRet.height
            || sizeDst.area() > sizeRet.area());
    if (bWrongSize)
        sizeRet = Size(int(fRightX - fLeftX + 0.5), int(fTopY - fBottomY + 0.5));

    return sizeRet;
}

void ImageProcessor::SortPtWithCenter(std::vector<Point2f>& vecSort)
{
    int iSize = (int)vecSort.size();
    Point2f ptCenter;
    for (int i = 0; i < iSize; i++)
        ptCenter += vecSort[i];
    ptCenter /= iSize;

    Point2f vecX(1, 0);

    std::vector<std::pair<Point2f, double>> vecPtAngle(iSize);
    for (int i = 0; i < iSize; i++)
    {
        vecPtAngle[i].first = vecSort[i];//pt
        Point2f vec1(vecSort[i].x - ptCenter.x, vecSort[i].y - ptCenter.y);
        float fNormVec1 = vec1.x * vec1.x + vec1.y * vec1.y;
        float fDot = vec1.x;

        if (vec1.y < 0)//若點在中心的上方
        {
            vecPtAngle[i].second = acos(fDot / fNormVec1) * R2D;
        }
        else if (vec1.y > 0)//下方
        {
            vecPtAngle[i].second = 360 - acos(fDot / fNormVec1) * R2D;
        }
        else
        {
            if (vec1.x - ptCenter.x > 0)
                vecPtAngle[i].second = 0;
            else
                vecPtAngle[i].second = 180;
        }

    }
    sort(vecPtAngle.begin(), vecPtAngle.end(), comparePtWithAngle);
    for (int i = 0; i < iSize; i++)
        vecSort[i] = vecPtAngle[i].first;
}
void ImageProcessor::FilterWithRotatedRect(std::vector<s_MatchParameter>* vec, int iMethod, double dMaxOverLap)
{
    int iMatchSize = (int)vec->size();
    RotatedRect rect1, rect2;
    for (int i = 0; i < iMatchSize - 1; i++)
    {
        if (vec->at(i).bDelete)
            continue;
        for (int j = i + 1; j < iMatchSize; j++)
        {
            if (vec->at(j).bDelete)
                continue;
            rect1 = vec->at(i).rectR;
            rect2 = vec->at(j).rectR;
            std::vector<Point2f> vecInterSec;
            int iInterSecType = rotatedRectangleIntersection(rect1, rect2, vecInterSec);
            if (iInterSecType == INTERSECT_NONE)
                continue;
            else if (iInterSecType == INTERSECT_FULL) //
            {
                int iDeleteIndex;
                if (iMethod == TM_SQDIFF)
                    iDeleteIndex = (vec->at(i).dMatchScore <= vec->at(j).dMatchScore) ? j : i;
                else
                    iDeleteIndex = (vec->at(i).dMatchScore >= vec->at(j).dMatchScore) ? j : i;
                vec->at(iDeleteIndex).bDelete = true;
            }
            else
            {
                if (vecInterSec.size() < 3)
                    continue;
                else
                {
                    int iDeleteIndex;
                    SortPtWithCenter(vecInterSec);
                    double dArea = contourArea(vecInterSec);
                    double dRatio = dArea / rect1.size.area();
                    if (dRatio > dMaxOverLap)
                    {
                        if (iMethod == TM_SQDIFF)
                            iDeleteIndex = (vec->at(i).dMatchScore <= vec->at(j).dMatchScore) ? j : i;
                        else
                            iDeleteIndex = (vec->at(i).dMatchScore >= vec->at(j).dMatchScore) ? j : i;
                        vec->at(iDeleteIndex).bDelete = true;
                    }
                }
            }
        }
    }
    std::vector<s_MatchParameter>::iterator it;
    for (it = vec->begin(); it != vec->end();)
    {
        if ((*it).bDelete)
            it = vec->erase(it);
        else
            ++it;
    }
}
Point ImageProcessor::GetNextMaxLoc(Mat& matResult, Point ptMaxLoc, Size sizeTemplate, double& dMaxValue, double dMaxOverlap)
{
    int iStartX = ptMaxLoc.x - sizeTemplate.width * (1 - dMaxOverlap);
    int iStartY = ptMaxLoc.y - sizeTemplate.height * (1 - dMaxOverlap);
    rectangle(matResult, Rect(iStartX, iStartY, 2 * sizeTemplate.width * (1 - dMaxOverlap), 2 * sizeTemplate.height * (1 - dMaxOverlap)), Scalar(-1), cv::FILLED);
    Point ptNewMaxLoc;
    minMaxLoc(matResult, 0, &dMaxValue, 0, &ptNewMaxLoc);
    return ptNewMaxLoc;
}

bool ImageProcessor::SubPixEsimation(std::vector<s_MatchParameter>* vec, double* dNewX, double* dNewY, double* dNewAngle, double dAngleStep, int iMaxScoreIndex)
{
    //Az=S, (A.T)Az=(A.T)s, z = ((A.T)A).inv (A.T)s

    Mat matA(27, 10, CV_64F);
    Mat matZ(10, 1, CV_64F);
    Mat matS(27, 1, CV_64F);

    double dX_maxScore = (*vec)[iMaxScoreIndex].pt.x;
    double dY_maxScore = (*vec)[iMaxScoreIndex].pt.y;
    double dTheata_maxScore = (*vec)[iMaxScoreIndex].dMatchAngle;
    int iRow = 0;
    /*for (int x = -1; x <= 1; x++)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int theta = 0; theta <= 2; theta++)
            {*/
    for (int theta = 0; theta <= 2; theta++)
    {
        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                //xx yy tt xy xt yt x y t 1
                //0  1  2  3  4  5  6 7 8 9
                double dX = dX_maxScore + x;
                double dY = dY_maxScore + y;
                //double dT = (*vec)[theta].dMatchAngle + (theta - 1) * dAngleStep;
                double dT = (dTheata_maxScore + (theta - 1) * dAngleStep) * D2R;
                matA.at<double>(iRow, 0) = dX * dX;
                matA.at<double>(iRow, 1) = dY * dY;
                matA.at<double>(iRow, 2) = dT * dT;
                matA.at<double>(iRow, 3) = dX * dY;
                matA.at<double>(iRow, 4) = dX * dT;
                matA.at<double>(iRow, 5) = dY * dT;
                matA.at<double>(iRow, 6) = dX;
                matA.at<double>(iRow, 7) = dY;
                matA.at<double>(iRow, 8) = dT;
                matA.at<double>(iRow, 9) = 1.0;
                matS.at<double>(iRow, 0) = (*vec)[iMaxScoreIndex + (theta - 1)].vecResult[x + 1][y + 1];
                iRow++;
#ifdef _DEBUG
                /*string str = format ("%.6f, %.6f, %.6f, %.6f, %.6f, %.6f, %.6f, %.6f, %.6f, %.6f", dValueA[0], dValueA[1], dValueA[2], dValueA[3], dValueA[4], dValueA[5], dValueA[6], dValueA[7], dValueA[8], dValueA[9]);
                fileA <<  str << endl;
                str = format ("%.6f", dValueS[iRow]);
                fileS << str << endl;*/
#endif
            }
        }
    }
    //[ x* ] = [ 2k0 k3 k4 ]-1 [ -k6 ]
    //| y* | = | k3 2k1 k5 |   | -k7 |
    //[ t* ] = [ k4 k5 2k2 ]   [ -k8 ]

    //solve (matA, matS, matZ, DECOMP_SVD);
    matZ = (matA.t() * matA).inv() * matA.t() * matS;
    Mat matZ_t;
    transpose(matZ, matZ_t);
    double* dZ = matZ_t.ptr<double>(0);
    Mat matK1 = (Mat_<double>(3, 3) <<
        (2 * dZ[0]), dZ[3], dZ[4],
        dZ[3], (2 * dZ[1]), dZ[5],
        dZ[4], dZ[5], (2 * dZ[2]));
    Mat matK2 = (Mat_<double>(3, 1) << -dZ[6], -dZ[7], -dZ[8]);
    Mat matDelta = matK1.inv() * matK2;

    *dNewX = matDelta.at<double>(0, 0);
    *dNewY = matDelta.at<double>(1, 0);
    *dNewAngle = matDelta.at<double>(2, 0) * R2D;
    return true;
}
bool ImageProcessor::Match()
{
    if (m_matSrc.empty() || m_current_pattern.empty())
        return false;
    if ((m_current_pattern.cols < m_matSrc.cols && m_current_pattern.rows > m_matSrc.rows) || (m_current_pattern.cols > m_matSrc.cols && m_current_pattern.rows < m_matSrc.rows))
        return false;
    if (m_current_pattern.size().area() > m_matSrc.size().area())
        return false;
    if (!m_TemplData.bIsPatternLearned)
        return false;
    double d1 = clock();
    int iTopLayer = GetTopLayer(m_current_pattern, static_cast<int>(sqrt(static_cast<double>(m_iMinReduceArea))));
    std::vector<Mat> vecMatSrcPyr;
    buildPyramid(m_matSrc, vecMatSrcPyr, iTopLayer);
    s_TemplData* pTemplData = &m_TemplData;
    double dAngleStep = atan(2.0 / max(pTemplData->vecPyramid[iTopLayer].cols, pTemplData->vecPyramid[iTopLayer].rows)) * R2D;

    std::vector<double> vecAngles;
    if (m_bToleranceRange)
    {
        if (m_dTolerance1 >= m_dTolerance2 || m_dTolerance3 >= m_dTolerance4)
        {
            return false;
        }
        for (double dAngle = m_dTolerance1; dAngle < m_dTolerance2 + dAngleStep; dAngle += dAngleStep)
            vecAngles.push_back(dAngle);
        for (double dAngle = m_dTolerance3; dAngle < m_dTolerance4 + dAngleStep; dAngle += dAngleStep)
            vecAngles.push_back(dAngle);
    }
    else
    {
        if (m_dToleranceAngle < VISION_TOLERANCE)
            vecAngles.push_back(0.0);
        else
        {
            for (double dAngle = 0; dAngle < m_dToleranceAngle + dAngleStep; dAngle += dAngleStep)
                vecAngles.push_back(dAngle);
            for (double dAngle = -dAngleStep; dAngle > -m_dToleranceAngle - dAngleStep; dAngle -= dAngleStep)
                vecAngles.push_back(dAngle);
        }
    }
    int iTopSrcW = vecMatSrcPyr[iTopLayer].cols, iTopSrcH = vecMatSrcPyr[iTopLayer].rows;
    Point2f ptCenter((iTopSrcW - 1) / 2.0f, (iTopSrcH - 1) / 2.0f);

    int iSize = (int)vecAngles.size();
    //vector<s_MatchParameter> vecMatchParameter (iSize * (m_iMaxPos + MATCH_CANDIDATE_NUM));
    std::vector<s_MatchParameter> vecMatchParameter;
    //Caculate lowest score at every layer
    std::vector<double> vecLayerScore(iTopLayer + 1, m_dScore);
    for (int iLayer = 1; iLayer <= iTopLayer; iLayer++)
        vecLayerScore[iLayer] = vecLayerScore[iLayer - 1] * 0.9;

    Size sizePat = pTemplData->vecPyramid[iTopLayer].size();
    bool bCalMaxByBlock = (vecMatSrcPyr[iTopLayer].size().area() / sizePat.area() > 500) && m_iMaxPos > 10;
    for (int i = 0; i < iSize; i++)
    {
        Mat matRotatedSrc;
    	Mat matR = getRotationMatrix2D(ptCenter, vecAngles[i], 1);
        // ensure proper type/size and values
        if (!cv::checkRange(matR)) { // checks for NaN/Inf/out-of-range
            std::cerr << "Affine matrix contains NaN/Inf\n";
            continue;
        }
        // also validate inputs
        Mat matResult;
        Point ptMaxLoc;
        double dValue, dMaxVal;
        double dRotate = clock();
        Size sizeBest = GetBestRotationSize(vecMatSrcPyr[iTopLayer].size(), pTemplData->vecPyramid[iTopLayer].size(), vecAngles[i]);
        if (!std::isfinite(vecAngles[i]) || sizeBest.width <= 0 || sizeBest.height <= 0) {
            std::cerr << "Invalid rotation input (angle/size)\n";
            continue;
        }

        float fTranslationX = (sizeBest.width - 1) / 2.0f - ptCenter.x;
        float fTranslationY = (sizeBest.height - 1) / 2.0f - ptCenter.y;
        matR.at<double>(0, 2) += fTranslationX;
        matR.at<double>(1, 2) += fTranslationY;
        warpAffine(vecMatSrcPyr[iTopLayer], matRotatedSrc, matR, sizeBest, INTER_LINEAR, BORDER_CONSTANT, Scalar(pTemplData->iBorderColor));

        MatchTemplate(matRotatedSrc, pTemplData, matResult, iTopLayer, false);

        if (bCalMaxByBlock)
        {
            s_BlockMax blockMax(matResult, pTemplData->vecPyramid[iTopLayer].size());
            blockMax.GetMaxValueLoc(dMaxVal, ptMaxLoc);
            if (dMaxVal < vecLayerScore[iTopLayer])
                continue;
            vecMatchParameter.push_back(s_MatchParameter(Point2f(ptMaxLoc.x - fTranslationX, ptMaxLoc.y - fTranslationY), dMaxVal, vecAngles[i]));
            for (int j = 0; j < m_iMaxPos + MATCH_CANDIDATE_NUM - 1; j++)
            {
                ptMaxLoc = GetNextMaxLoc(matResult, ptMaxLoc, pTemplData->vecPyramid[iTopLayer].size(), dValue, m_dMaxOverlap);
                if (dValue < vecLayerScore[iTopLayer])
                    break;
                vecMatchParameter.push_back(s_MatchParameter(Point2f(ptMaxLoc.x - fTranslationX, ptMaxLoc.y - fTranslationY), dValue, vecAngles[i]));
            }
        }
        else
        {
            minMaxLoc(matResult, 0, &dMaxVal, 0, &ptMaxLoc);
            if (dMaxVal < vecLayerScore[iTopLayer])
                continue;
            vecMatchParameter.push_back(s_MatchParameter(Point2f(ptMaxLoc.x - fTranslationX, ptMaxLoc.y - fTranslationY), dMaxVal, vecAngles[i]));
            for (int j = 0; j < m_iMaxPos + MATCH_CANDIDATE_NUM - 1; j++)
            {
                ptMaxLoc = GetNextMaxLoc(matResult, ptMaxLoc, pTemplData->vecPyramid[iTopLayer].size(), dValue, m_dMaxOverlap);
                if (dValue < vecLayerScore[iTopLayer])
                    break;
                vecMatchParameter.push_back(s_MatchParameter(Point2f(ptMaxLoc.x - fTranslationX, ptMaxLoc.y - fTranslationY), dValue, vecAngles[i]));
            }
        }
    }
    sort(vecMatchParameter.begin(), vecMatchParameter.end(), compareScoreBig2Small);


    int iMatchSize = (int)vecMatchParameter.size();
    int iDstW = pTemplData->vecPyramid[iTopLayer].cols, iDstH = pTemplData->vecPyramid[iTopLayer].rows;

    if (m_bDebugMode)
    {
        int iDebugScale = 2;

        Mat matShow, matResize;
        std::string str = format("Toplayer, Candidate:%d", iMatchSize);
		matShow = vecMatSrcPyr[iTopLayer].clone();
        std::vector<Point2f> vec;
        for (int i = 0; i < iMatchSize; i++)
        {
            Point2f ptLT, ptRT, ptRB, ptLB;
            double dRAngle = -vecMatchParameter[i].dMatchAngle * D2R;
            ptLT = ptRotatePt2f(vecMatchParameter[i].pt, ptCenter, dRAngle);
            ptRT = Point2f(ptLT.x + iDstW * (float)cos(dRAngle), ptLT.y - iDstW * (float)sin(dRAngle));
            ptLB = Point2f(ptLT.x + iDstH * (float)sin(dRAngle), ptLT.y + iDstH * (float)cos(dRAngle));
            ptRB = Point2f(ptRT.x + iDstH * (float)sin(dRAngle), ptRT.y + iDstH * (float)cos(dRAngle));
            line(matShow, ptLT * iDebugScale, ptLB * iDebugScale, Scalar(0, 255, 0));
            line(matShow, ptLB * iDebugScale, ptRB * iDebugScale, Scalar(0, 255, 0));
            line(matShow, ptRB * iDebugScale, ptRT * iDebugScale, Scalar(0, 255, 0));
            line(matShow, ptRT * iDebugScale, ptLT * iDebugScale, Scalar(0, 255, 0));
            circle(matShow, ptLT * iDebugScale, 1, Scalar(0, 0, 255));
            vec.push_back(ptLT * iDebugScale);
            vec.push_back(ptRT * iDebugScale);
            vec.push_back(ptLB * iDebugScale);
            vec.push_back(ptRB * iDebugScale);

            std::string strText = format("%d", i);
            putText(matShow, strText, ptLT * iDebugScale, FONT_HERSHEY_PLAIN, 1, Scalar(0, 255, 0));
        }
        Rect rectShow = boundingRect(vec);
		std::string filename = format("c:/img/toplayer_candidate%d.jpg", iMatchSize);
        imwrite(filename, matShow);
        //moveWindow (str, 0, 0);
    }
    bool bSubPixelEstimation = true;
    int iStopLayer = m_bStopLayer1 ? 1 : 0;
    //int iSearchSize = min (m_iMaxPos + MATCH_CANDIDATE_NUM, (int)vecMatchParameter.size ());//可能不需要搜尋到全部 太浪費時間
    std::vector<s_MatchParameter> vecAllResult;
    for (int i = 0; i < (int)vecMatchParameter.size(); i++)
        //for (int i = 0; i < iSearchSize; i++)
    {
        double dRAngle = -vecMatchParameter[i].dMatchAngle * D2R;
        Point2f ptLT = ptRotatePt2f(vecMatchParameter[i].pt, ptCenter, dRAngle);

        double dAngleStep = atan(2.0 / max(iDstW, iDstH)) * R2D;//min改為max
        vecMatchParameter[i].dAngleStart = vecMatchParameter[i].dMatchAngle - dAngleStep;
        vecMatchParameter[i].dAngleEnd = vecMatchParameter[i].dMatchAngle + dAngleStep;

        if (iTopLayer <= iStopLayer)
        {
            vecMatchParameter[i].pt = Point2d(ptLT * ((iTopLayer == 0) ? 1 : 2));
            vecAllResult.push_back(vecMatchParameter[i]);
        }
        else
        {
            for (int iLayer = iTopLayer - 1; iLayer >= iStopLayer; iLayer--)
            {
                dAngleStep = atan(2.0 / max(pTemplData->vecPyramid[iLayer].cols, pTemplData->vecPyramid[iLayer].rows)) * R2D;//min改為max
                std::vector<double> vecAngles;
                double dMatchedAngle = vecMatchParameter[i].dMatchAngle;
                if (m_bToleranceRange)
                {
                    for (int i = -1; i <= 1; i++)
                        vecAngles.push_back(dMatchedAngle + dAngleStep * i);
                }
                else
                {
                    if (m_dToleranceAngle < VISION_TOLERANCE)
                        vecAngles.push_back(0.0);
                    else
                        for (int i = -1; i <= 1; i++)
                            vecAngles.push_back(dMatchedAngle + dAngleStep * i);
                }
                Point2f ptSrcCenter((vecMatSrcPyr[iLayer].cols - 1) / 2.0f, (vecMatSrcPyr[iLayer].rows - 1) / 2.0f);
                iSize = (int)vecAngles.size();
                std::vector<s_MatchParameter> vecNewMatchParameter(iSize);
                int iMaxScoreIndex = 0;
                double dBigValue = -1;
                for (int j = 0; j < iSize; j++)
                {
                    Mat matResult, matRotatedSrc;
                    double dMaxValue = 0;
                    Point ptMaxLoc;
                    GetRotatedROI(vecMatSrcPyr[iLayer], pTemplData->vecPyramid[iLayer].size(), ptLT * 2, vecAngles[j], matRotatedSrc);

                    MatchTemplate(matRotatedSrc, pTemplData, matResult, iLayer, true);
                    minMaxLoc(matResult, 0, &dMaxValue, 0, &ptMaxLoc);
                    vecNewMatchParameter[j] = s_MatchParameter(ptMaxLoc, dMaxValue, vecAngles[j]);

                    if (vecNewMatchParameter[j].dMatchScore > dBigValue)
                    {
                        iMaxScoreIndex = j;
                        dBigValue = vecNewMatchParameter[j].dMatchScore;
                    }
                    if (ptMaxLoc.x == 0 || ptMaxLoc.y == 0 || ptMaxLoc.x == matResult.cols - 1 || ptMaxLoc.y == matResult.rows - 1)
                        vecNewMatchParameter[j].bPosOnBorder = true;
                    if (!vecNewMatchParameter[j].bPosOnBorder)
                    {
                        for (int y = -1; y <= 1; y++)
                            for (int x = -1; x <= 1; x++)
                                vecNewMatchParameter[j].vecResult[x + 1][y + 1] = matResult.at<float>(ptMaxLoc + Point(x, y));
                    }
                }
                if (vecNewMatchParameter[iMaxScoreIndex].dMatchScore < vecLayerScore[iLayer])
                    break;
                if (bSubPixelEstimation
                    && iLayer == 0
                    && (!vecNewMatchParameter[iMaxScoreIndex].bPosOnBorder)
                    && iMaxScoreIndex != 0
                    && iMaxScoreIndex != 2)
                {
                    double dNewX = 0, dNewY = 0, dNewAngle = 0;
                    SubPixEsimation(&vecNewMatchParameter, &dNewX, &dNewY, &dNewAngle, dAngleStep, iMaxScoreIndex);
                    vecNewMatchParameter[iMaxScoreIndex].pt = Point2d(dNewX, dNewY);
                    vecNewMatchParameter[iMaxScoreIndex].dMatchAngle = dNewAngle;
                }

                double dNewMatchAngle = vecNewMatchParameter[iMaxScoreIndex].dMatchAngle;

                Point2f ptPaddingLT = ptRotatePt2f(ptLT * 2, ptSrcCenter, dNewMatchAngle * D2R) - Point2f(3, 3);
                Point2f pt(vecNewMatchParameter[iMaxScoreIndex].pt.x + ptPaddingLT.x, vecNewMatchParameter[iMaxScoreIndex].pt.y + ptPaddingLT.y);
                pt = ptRotatePt2f(pt, ptSrcCenter, -dNewMatchAngle * D2R);

                if (iLayer == iStopLayer)
                {
                    vecNewMatchParameter[iMaxScoreIndex].pt = pt * (iStopLayer == 0 ? 1 : 2);
                    vecAllResult.push_back(vecNewMatchParameter[iMaxScoreIndex]);
                }
                else
                {
                    vecMatchParameter[i].dMatchAngle = dNewMatchAngle;
                    vecMatchParameter[i].dAngleStart = vecMatchParameter[i].dMatchAngle - dAngleStep / 2;
                    vecMatchParameter[i].dAngleEnd = vecMatchParameter[i].dMatchAngle + dAngleStep / 2;
                    ptLT = pt;
                }
            }

        }
    }
    FilterWithScore(&vecAllResult, m_dScore);

    iDstW = pTemplData->vecPyramid[iStopLayer].cols * (iStopLayer == 0 ? 1 : 2);
    iDstH = pTemplData->vecPyramid[iStopLayer].rows * (iStopLayer == 0 ? 1 : 2);

    for (int i = 0; i < (int)vecAllResult.size(); i++)
    {
        Point2f ptLT, ptRT, ptRB, ptLB;
        double dRAngle = -vecAllResult[i].dMatchAngle * D2R;
        ptLT = vecAllResult[i].pt;
        ptRT = Point2f(ptLT.x + iDstW * (float)cos(dRAngle), ptLT.y - iDstW * (float)sin(dRAngle));
        ptLB = Point2f(ptLT.x + iDstH * (float)sin(dRAngle), ptLT.y + iDstH * (float)cos(dRAngle));
        ptRB = Point2f(ptRT.x + iDstH * (float)sin(dRAngle), ptRT.y + iDstH * (float)cos(dRAngle));
        vecAllResult[i].rectR = RotatedRect(ptLT, ptRT, ptRB);
    }
    FilterWithRotatedRect(&vecAllResult, TM_CCOEFF_NORMED, m_dMaxOverlap);
    sort(vecAllResult.begin(), vecAllResult.end(), compareScoreBig2Small);

    m_vecSingleTargetData.clear();
    iMatchSize = (int)vecAllResult.size();
    if (vecAllResult.size() == 0)
        return false;
    int iW = pTemplData->vecPyramid[0].cols, iH = pTemplData->vecPyramid[0].rows;

    for (int i = 0; i < iMatchSize; i++)
    {
        s_SingleTargetMatch sstm;
        double dRAngle = -vecAllResult[i].dMatchAngle * D2R;

        sstm.ptLT = vecAllResult[i].pt;

        sstm.ptRT = Point2d(sstm.ptLT.x + iW * cos(dRAngle), sstm.ptLT.y - iW * sin(dRAngle));
        sstm.ptLB = Point2d(sstm.ptLT.x + iH * sin(dRAngle), sstm.ptLT.y + iH * cos(dRAngle));
        sstm.ptRB = Point2d(sstm.ptRT.x + iH * sin(dRAngle), sstm.ptRT.y + iH * cos(dRAngle));
        sstm.ptCenter = Point2d((sstm.ptLT.x + sstm.ptRT.x + sstm.ptRB.x + sstm.ptLB.x) / 4, (sstm.ptLT.y + sstm.ptRT.y + sstm.ptRB.y + sstm.ptLB.y) / 4);
        sstm.dMatchedAngle = -vecAllResult[i].dMatchAngle;
        sstm.dMatchScore = vecAllResult[i].dMatchScore;

        if (sstm.dMatchedAngle < -180)
            sstm.dMatchedAngle += 360;
        if (sstm.dMatchedAngle > 180)
            sstm.dMatchedAngle -= 360;
        m_vecSingleTargetData.push_back(sstm);

        OutputRoi(sstm);

        if (i + 1 == m_iMaxPos)
            break;
    }
    //sort (m_vecSingleTargetData.begin (), m_vecSingleTargetData.end (), compareMatchResultByPosX);

    return m_vecSingleTargetData.size() > 0;
}

void ImageProcessor::OutputRoi(s_SingleTargetMatch sstm)
{
    Rect rect(sstm.ptLT, sstm.ptRB);
    for (int i = 1; i < 50; i++)
    {
        String strName = format("c:/img/roi%d.bmp", i);
        imwrite(strName, m_matSrc(rect));
        break;
    }
}
void ImageProcessor::FilterWithScore(std::vector<s_MatchParameter>* vec, double dScore)
{
    sort(vec->begin(), vec->end(), compareScoreBig2Small);
    int iSize = vec->size(), iIndexDelete = iSize + 1;
    for (int i = 0; i < iSize; i++)
    {
        if ((*vec)[i].dMatchScore < dScore)
        {
            iIndexDelete = i;
            break;
        }
    }
    if (iIndexDelete == iSize + 1)
        return;
    vec->erase(vec->begin() + iIndexDelete, vec->end());
    return;
}
void ImageProcessor::GetRotatedROI (Mat& matSrc, Size size, Point2f ptLT, double dAngle, Mat& matROI)
{
	double dAngle_radian = dAngle * D2R;
	Point2f ptC ((matSrc.cols - 1) / 2.0f, (matSrc.rows - 1) / 2.0f);
	Point2f ptLT_rotate = ptRotatePt2f (ptLT, ptC, dAngle_radian);
	Size sizePadding (size.width + 6, size.height + 6);


	Mat rMat = getRotationMatrix2D (ptC, dAngle, 1);
	rMat.at<double> (0, 2) -= ptLT_rotate.x - 3;
	rMat.at<double> (1, 2) -= ptLT_rotate.y - 3;
	warpAffine (matSrc, matROI, rMat, sizePadding);
}

bool ImageProcessor::SetCurrentPattern(const std::string& patternFile)
{
    if (!std::filesystem::exists(patternFile))
        return false;
    Mat pattern = cv::imread(patternFile, IMREAD_GRAYSCALE);
    if (pattern.empty())
        return false;
    m_current_pattern = pattern.clone();
    return LearnPattern();
}

bool ImageProcessor::LearnPattern()
{
    int iTopLayer = GetTopLayer( m_current_pattern, (int)sqrt(static_cast<double>(m_iMinReduceArea)));
    buildPyramid(m_current_pattern, m_TemplData.vecPyramid, iTopLayer);
    s_TemplData* templData = &m_TemplData;
    templData->iBorderColor = mean(m_current_pattern).val[0] < 128 ? 255 : 0;
    int iSize = templData->vecPyramid.size();
    templData->resize(iSize);
    for (int i = 0; i < iSize; i++)
    {
        double invArea = 1. / ((double)templData->vecPyramid[i].rows * templData->vecPyramid[i].cols);
        double templNorm = 0, templSum2 = 0;
        int ch = templData->vecPyramid[i].channels();
        cv::Mat mean_mat, sdv_mat;
        cv::meanStdDev(templData->vecPyramid[i], mean_mat, sdv_mat);
        cv::Scalar templMean(
            mean_mat.at<double>(0, 0),
            mean_mat.cols > 1 ? mean_mat.at<double>(0, 1) : 0,
            /* ... */ 0
        );
        Scalar  templSdv(
            sdv_mat.at<double>(0, 0),
            sdv_mat.cols > 1 ? sdv_mat.at<double>(0, 1) : 0,
            /* ... */ 0

        );

        templNorm = templSdv[0] * templSdv[0] + templSdv[1] * templSdv[1] + templSdv[2] * templSdv[2] + templSdv[3] * templSdv[3];

        if (templNorm < DBL_EPSILON)
        {
            templData->vecResultEqual1[i] = true;
        }
        templSum2 = templNorm + templMean[0] * templMean[0] + templMean[1] * templMean[1] + templMean[2] * templMean[2] + templMean[3] * templMean[3];


        templSum2 /= invArea;
        templNorm = std::sqrt(templNorm);
        templNorm /= std::sqrt(invArea); // care of accuracy here


        templData->vecInvArea[i] = invArea;
        templData->vecTemplMean[i] = templMean;
        templData->vecTemplNorm[i] = templNorm;
    }
    templData->bIsPatternLearned = true;

    return templData->bIsPatternLearned;
}

std::vector<cv::Rect> ImageProcessor::GetNLargestContourBoxes(int N, const Mat& image) {
    std::vector<cv::Rect> resultBoxes;

    if (image.empty() || N <= 0) {
        return resultBoxes;
    }

    cv::Mat gray, binary;

    // 1. Pre-process: Convert to grayscale if necessary
    if (image.channels() == 3) {
        cv::cvtColor(image, gray, cv::COLOR_BGR2GRAY);
    }
    else {
        gray = image.clone();
    }

    // 2. Create binary image (Thresholding)
    // Adjust threshold method (Otsu, adaptive, or fixed) based on specific needs.
    // Here we use Otsu's binarization for automatic thresholding.
    cv::threshold(gray, binary, 0, 255, cv::THRESH_BINARY | cv::THRESH_OTSU);

    // 3. Find Contours
    std::vector<std::vector<cv::Point>> contours;
    std::vector<cv::Vec4i> hierarchy;
    // RETR_EXTERNAL retrieves only the extreme outer contours
    cv::findContours(binary, contours, hierarchy, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    // 4. Sort Contours by Area (Descending)
    // Use a lambda to compare contour areas
    std::ranges::sort(contours, [](const std::vector<cv::Point>& a, const std::vector<cv::Point>& b) {
        return cv::contourArea(a) > cv::contourArea(b);
        });

    // 5. Extract Bounding Rects for the top N contours
    int count = std::min((int)contours.size(), N);
    for (int i = 0; i < count; i++) {
        resultBoxes.push_back(cv::boundingRect(contours[i]));
    }

    return resultBoxes;
}

std::vector<cv::Rect> ImageProcessor::GetLogoCandidates(const cv::Mat& image, double minArea, double maxArea) {
    std::vector<cv::Rect> logoRects;
    if (image.empty()) return logoRects;

    cv::Mat gray, edges;

    // 1. Convert to Grayscale & Blur
    if (image.channels() == 3) {
        cv::cvtColor(image, gray, cv::COLOR_BGR2GRAY);
    }
    else {
        gray = image.clone();
    }
    cv::GaussianBlur(gray, gray, cv::Size(25, 25), 0);

    // 2. Canny Edge Detection
    // Canny is often better for logos as it captures internal details better than thresholding
    cv::Canny(gray, edges, 50, 150);

    // 3. Find Contours with Hierarchy
    // RETR_TREE retrieves all contours and reconstructs a full hierarchy of nested contours.
    // Hierarchy: [Next, Previous, First_Child, Parent]
    std::vector<std::vector<cv::Point>> contours;
    std::vector<cv::Vec4i> hierarchy;
    cv::findContours(edges, contours, hierarchy, cv::RETR_TREE, cv::CHAIN_APPROX_SIMPLE);

    // 4. Iterate and Filter
    for (size_t i = 0; i < contours.size(); i++) {
        double area = cv::contourArea(contours[i]);

        // Filter by size
        if (area < minArea || area > maxArea) continue;

        // Get Bounding Box
        cv::Rect rect = cv::boundingRect(contours[i]);

        // Filter by Aspect Ratioll
        // Text is usually very wide (width >> height).
        // Logos are usually "squarish" or circular (aspect ratio ~ 0.5 to 2.0).
        double aspectRatio = (double)rect.width / rect.height;
        if (aspectRatio < 0.5 || aspectRatio > 2.0) continue;

        // Filter by Hierarchy (Optional but effective)
        // Logos often frame something or have internal details. 
        // hierarchy[i][2] != -1 means this contour has a child (something inside it).
        // This helps filter out simple noise specs or solid blocks.
        bool hasChild = hierarchy[i][2] != -1;

        // We can also check solidity (Area / ConvexHullArea) to ensure it's a compact shape
        std::vector<cv::Point> hull;
        cv::convexHull(contours[i], hull);
        double hullArea = cv::contourArea(hull);
        double solidity = area / hullArea;

        // If it has children OR it's a very solid, distinct shape
        if (hasChild || solidity > 0.9) {
            logoRects.push_back(rect);
        }
    }

    return logoRects;
}

std::vector<cv::Rect> ImageProcessor::GetWhiteLabelRects(const cv::Mat& image, double minArea) {
    std::vector<cv::Rect> labelRects;
    if (image.empty()) return labelRects;

    cv::Mat gray, binary;

    // 1. Convert to Grayscale
    if (image.channels() == 3) {
        cv::cvtColor(image, gray, cv::COLOR_BGR2GRAY);
    }
    else {
        gray = image.clone();
    }

    // 2. Adaptive Thresholding
    // Uses local neighborhood to find threshold. Good for uneven lighting.
    // Block Size: 21 (neighborhood size), C: 5 (constant subtracted from mean)
    // THRESH_BINARY ensures white pixels (labels) remain white (255)
    cv::adaptiveThreshold(gray, binary, 255, cv::ADAPTIVE_THRESH_GAUSSIAN_C, cv::THRESH_BINARY, 21, 5);

    // 3. Morphological Operations (Noise Removal)
    // "Open" operation (Erosion followed by Dilation) removes small white noise dots
    cv::Mat kernel = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(3, 3));
    cv::morphologyEx(binary, binary, cv::MORPH_OPEN, kernel);

    // 4. Find Contours
    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(binary, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    // 5. Filter for Label Characteristics
    for (const auto& cnt : contours) {
        double area = cv::contourArea(cnt);
        if (area < minArea) continue;

        // Approximate the contour to a polygon to check for rectangularity
        std::vector<cv::Point> approx;
        double epsilon = 0.04 * cv::arcLength(cnt, true);
        cv::approxPolyDP(cnt, approx, epsilon, true);

        // A label is typically a rectangle (4 vertices)
        // We also check convex hull or aspect ratio here if needed for strictness
        if (approx.size() == 4) {
            labelRects.push_back(cv::boundingRect(cnt));
        }
        // Fallback: If it's not a perfect rectangle but huge, it's likely the label
        // (e.g., if a corner is slightly rounded or occluded)
        else if (area > minArea * 2) {
            labelRects.push_back(cv::boundingRect(cnt));
        }
    }

    return labelRects;
}
int ImageProcessor::GetTopLayer(const Mat& matTempl, int iMinDstLength)
{
    int iTopLayer = 0;
    int iMinReduceArea = iMinDstLength * iMinDstLength;
    int iArea = matTempl.cols * matTempl.rows;
    while (iArea > iMinReduceArea)
    {
        iArea /= 4;
        iTopLayer++;
    }
    return iTopLayer;
}
void ImageProcessor::segmentAndWrite(Mat& gray_image, const std::string& inputFilename, int k) {
    std::vector<cv::Mat> backgroundAreas = extractBackgroundAreas(gray_image, k);
    for (int i = 0; i < (int)backgroundAreas.size(); ++i) {
        std::string baseName = inputFilename;
        size_t dotPos = inputFilename.find_last_of('.');
        if (dotPos != std::string::npos) baseName = inputFilename.substr(0, dotPos);
        std::string outputFilename = baseName + "_segment_" + std::to_string(i) + ".png";
        if (!cv::imwrite(outputFilename, backgroundAreas[i]))
            std::cerr << "Error: Could not write " << outputFilename << std::endl;
    }
}
Point2f ImageProcessor::findIntersection(Vec2f line_h, Vec2f line_v) 
{
    float rho_h = line_h[0], theta_h = line_h[1];
    float rho_v = line_v[0], theta_v = line_v[1];

    // Convert polar coordinates (rho, theta) to standard line form Ax + By = C
    float cos_h = cos(theta_h);
    float sin_h = sin(theta_h);
    float cos_v = cos(theta_v);
    float sin_v = sin(theta_v);

    // Solve for intersection (x, y) using Cramer's rule
    float det = cos_h * sin_v - sin_h * cos_v;

    // Check for parallel lines
    if (abs(det) < 1e-6) {
        return Point2f(-1, -1); // Sentinel value for failure
    }

    float x = (rho_h * sin_v - sin_h * rho_v) / det;
    float y = (cos_h * rho_v - rho_h * cos_v) / det;

    return Point2f(x, y);
}

Mat ImageProcessor::findAndCropCropMarks(Mat& input_img) {
    if (input_img.empty()) {
        return Mat();
    }

    Mat& gray = input_img, edges;
    Canny(gray, edges, 50, 200, 3); // Canny edge detection
	imwrite("c:/1404 labels/edges.png", edges);
    // 2. Line Detection using Standard Hough Transform (returns rho, theta)
    std::vector<Vec2f> lines;
    // Parameters: 1 pixel resolution, 1 degree resolution, 100 vote threshold
    HoughLines(edges, lines, 1, CV_PI / 180, 75);

    if (lines.empty()) {
	    std::cerr << "Warning: No strong lines detected using Hough Transform. Returning original image." << std::endl;
        return input_img.clone();
    }

    // Define tolerance for angle (5 degrees in radians)
    const float DEGREE_TOLERANCE = 5.0f;
    const float RAD_TOLERANCE = DEGREE_TOLERANCE * CV_PI / 180.0f;

    std::vector<Vec2f> horizontal_lines;
    std::vector<Vec2f> vertical_lines;

    // 3. Filter lines into horizontal and vertical groups
    for (const auto& line : lines) {
        float theta = line[1];

        // Horizontal lines: theta near 0 or PI
        if (abs(theta) < RAD_TOLERANCE || abs(theta - CV_PI) < RAD_TOLERANCE) {
            horizontal_lines.push_back(line);
        }
        // Vertical lines: theta near PI/2
        else if (abs(theta - CV_PI / 2.0f) < RAD_TOLERANCE) {
            vertical_lines.push_back(line);
        }
    }

    if (horizontal_lines.size() < 2 || vertical_lines.size() < 2) {
	    std::cerr << "Warning: Not enough required lines (min 2 H, 2 V). Returning original image." << std::endl;
        return input_img.clone();
    }

    // 4. Find the four most extreme lines (Top, Bottom, Left, Right)
    // Horizontal lines are sorted by rho (distance from origin). Smallest rho is typically Top.
    sort(horizontal_lines.begin(), horizontal_lines.end(), [](const Vec2f& a, const Vec2f& b) {
        return a[0] < b[0];
        });

    // Vertical lines are sorted by rho. Smallest rho is typically Left.
    sort(vertical_lines.begin(), vertical_lines.end(), [](const Vec2f& a, const Vec2f& b) {
        return a[0] < b[0];
        });

    // Select the first (min rho) and last (max rho) lines to define the extent of the bounding box.
    const Vec2f line_top = horizontal_lines.front();
    const Vec2f line_bottom = horizontal_lines.back();
    const Vec2f line_left = vertical_lines.front();
    const Vec2f line_right = vertical_lines.back();

    // 5. Calculate the four corner intersection points
    Point2f tl = findIntersection(line_top, line_left);    // Top-Left
    Point2f tr = findIntersection(line_top, line_right);   // Top-Right
    Point2f bl = findIntersection(line_bottom, line_left); // Bottom-Left
    Point2f br = findIntersection(line_bottom, line_right); // Bottom-Right

    if (tl.x < 0 || tr.x < 0 || bl.x < 0 || br.x < 0) {
	    std::cerr << "Error: Failed to calculate valid intersection points. Returning original image." << std::endl;
        return input_img.clone();
    }

    // 6. Define the bounding rectangle for cropping
    int min_x = static_cast<int>(min({ tl.x, tr.x, bl.x, br.x }));
    int max_x = static_cast<int>(max({ tl.x, tr.x, bl.x, br.x }));
    int min_y = static_cast<int>(min({ tl.y, tr.y, bl.y, br.y }));
    int max_y = static_cast<int>(max({ tl.y, tr.y, bl.y, br.y }));

    // Clamp the rectangle to the image boundaries
    min_x = max(0, min_x);
    min_y = max(0, min_y);
    max_x = min(input_img.cols, max_x);
    max_y = min(input_img.rows, max_y);

    int width = max_x - min_x;
    int height = max_y - min_y;

    if (width <= 0 || height <= 0) {
	    std::cerr << "Error: Calculated crop area is invalid (width/height <= 0). Returning original image." << std::endl;
        return input_img.clone();
    }

    Rect crop_rect(min_x, min_y, width, height);

    // 7. Crop the image
    Mat cropped_img = input_img(crop_rect).clone();

    return cropped_img;
}

Mat ImageProcessor::removeColorControlStrips(const Mat& input_img) {
    if (input_img.empty()) {
	    std::cerr << "Error: Input image is empty." << std::endl;
        return Mat();
    }

    // 1. Preprocessing: Convert to grayscale and apply adaptive thresholding
    Mat  binary;

    // Use Otsu's thresholding to get a clean separation between background/strips and main content
    // This is robust if the background is uniform.
    threshold(input_img, binary, 0, 255, THRESH_BINARY_INV | THRESH_OTSU);
    bitwise_not(binary, binary);
    imwrite("c:/1404 labels/thresh.png", binary);
	// invert the image so that content areas are white (255) and background/strips are black (0)

    // 2. Find contours
    std::vector<std::vector<Point>> contours;
    std::vector<Vec4i> hierarchy;
    // RETR_EXTERNAL retrieves only the external contours (outer boundaries of objects)
    findContours(binary, contours, hierarchy, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE);

    if (contours.empty()) {
	    std::cerr << "Warning: No contours found after thresholding. Returning original image." << std::endl;
        return input_img.clone();
    }

    // 3. Find the largest contour, assuming it's the main content area
    double max_area = 0;
    Rect max_rect;
    bool found_contour = false;

    for (const auto& contour : contours) {
        double area = contourArea(contour);
        // Only consider contours that are a reasonable size (e.g., more than 1% of the total image area)
        // This helps filter out noise.
        if (area > (input_img.total() * 0.01)) {
            Rect rect = boundingRect(contour);
            if (area > max_area) {
                max_area = area;
                max_rect = rect;
                found_contour = true;
            }
        }
    }

    if (!found_contour) {
	    std::cerr << "Warning: No sufficiently large contour found for cropping. Returning original image." << std::endl;
        return input_img.clone();
    }

    // 4. Crop the image to the bounding box of the main content area
    Mat cropped_img = input_img(max_rect).clone();

    return cropped_img;
}

cv::Mat ImageProcessor::detect_edges(const cv::Mat& grayImage, double threshold1, double threshold2) {
    if (grayImage.channels() != 1) {
        std::cerr << "Error: detectEdges requires a grayscale image." << std::endl;
        return grayImage;
    }
    cv::Mat blurredImage;
    cv::GaussianBlur(grayImage, blurredImage, cv::Size(3, 3), 0);
    cv::Mat edges;
    cv::Canny(blurredImage, edges, threshold1, threshold2);
    return edges;
}

Mat ImageProcessor::laplacian_filter(Mat& img)
{
    Mat laplacian;
    Laplacian(img, laplacian, CV_16S, 3);
    convertScaleAbs(laplacian, laplacian);
    return laplacian;
}


std::pair<double,double> ImageProcessor::resize_to_max_and_align(cv::Mat& img, int targetMax)
{
    if (img.empty()) return {1.0, 1.0};
    int w = img.cols, h = img.rows;
    int maxDim = std::max(w, h);
    if (maxDim <= targetMax) return {1.0, 1.0};
    double scale = static_cast<double>(targetMax) / static_cast<double>(maxDim);
    int newW = std::max(32, ((int)std::round(w * scale) + 16) / 32 * 32);
    int newH = std::max(32, ((int)std::round(h * scale) + 16) / 32 * 32);
    double scaleX = static_cast<double>(newW) / w;
    double scaleY = static_cast<double>(newH) / h;
    cv::resize(img, img, cv::Size(newW, newH), 0, 0, cv::INTER_AREA);
    return {scaleX, scaleY};
}

std::vector<cv::Rect> ImageProcessor::text_detect_db18(const cv::Mat& img, const std::string& modelPath,
    float boxThreshold, float binThreshold, int maxCandidates)
{
    std::vector<cv::Rect> boxes;
    if (img.empty()) return boxes;
    try {
        cv::dnn::TextDetectionModel_DB model(modelPath);
        model.setBinaryThreshold(binThreshold);
        model.setPolygonThreshold(boxThreshold);
		double meanVals[3] = {122.67891434, 116.66876762, 104.00698793};
		
        cv::Mat inputImg;
        if (img.channels() == 1) cvtColor(img, inputImg, COLOR_GRAY2BGR);
        else if (img.channels() == 4) cvtColor(img, inputImg, COLOR_BGRA2BGR);
        else inputImg = img;

        auto scale = resize_to_max_and_align(inputImg, 4000);
        // set input size if desired; many DB models work with arbitrary sizes
        model.setInputParams(1.0 / 255.0, cv::Size(inputImg.cols, inputImg.rows), true);
        std::vector<cv::RotatedRect> detected;
		imwrite("db_input.png", inputImg);
        model.detectTextRectangles(inputImg, detected);

        // Convert each rotated rect to an axis-aligned, clipped bounding box
        for (const auto &r : detected) {
            Point2f verticesf[4];
            r.points(verticesf);
            std::vector<Point> pts;
            pts.reserve(4);
            for (int i = 0; i < 4; ++i)
                pts.emplace_back(Point(cv::saturate_cast<int>(std::round(verticesf[i].x)),
                                        cv::saturate_cast<int>(std::round(verticesf[i].y))));
            Rect bbox = boundingRect(pts);
            // Clip bbox to image bounds
            bbox.x = std::max(0, bbox.x);
            bbox.y = std::max(0, bbox.y);
            bbox.width = std::min(inputImg.cols - bbox.x, bbox.width);
            bbox.height = std::min(inputImg.rows - bbox.y, bbox.height);
            if (bbox.width > 0 && bbox.height > 0)
                boxes.push_back(bbox);
        }

        // draw detected boxes for visualization
        Mat visImg = inputImg.clone();
        for (const auto& r : detected) {
            Point2f vertices[4];
            r.points(vertices);
            for (int j = 0; j < 4; j++)
                line(visImg, vertices[j], vertices[(j + 1) % 4], Scalar(0, 255, 0), 2);
        }
        imwrite("db_detected.png", visImg);


    }
    catch (const cv::Exception &e) {
        std::cerr << "DB text detector error: " << e.what() << std::endl;
    }
    return boxes;
}


std::vector<std::pair<std::string, cv::Rect>> ImageProcessor::ocrWithTesseract(const cv::Mat& sourceImage, std::string tessDataPath, std::string langs, std::string report) {
    std::vector<std::pair<std::string, cv::Rect>> results;
    std::string report_file;
	if (report.empty())
		report_file = "laf_analysis.txt";
	else
		report_file = report;
    // open report_file for writing
	std::fstream reportStream(report_file, std::ios::out);
    Mat markedImage = sourceImage.clone();
    // 1. Initialize Tesseract API
    tesseract::TessBaseAPI api;
    // Initialize tesseract-ocr with English, without specifying tessdata path
    if (api.Init(tessDataPath.c_str(), langs.c_str())) {
        std::cerr << "Could not initialize tesseract." << std::endl;
        return results;
    }

    // 2. Set Page Segmentation Mode
    // PSM_AUTO allows Tesseract to automatically segment the page.
    api.SetVariable("tessedit_word_separator", "/");
    api.SetPageSegMode(tesseract::PSM_AUTO);

    // 3. Set the image for Tesseract
    // We pass the raw pixel data, dimensions, and channel info directly.
    // Tesseract's SetImage can handle 1-channel (gray) and 3-channel (BGR) images.
    api.SetImage(sourceImage.data, sourceImage.cols, sourceImage.rows,
        sourceImage.channels(), sourceImage.step);
	api.Recognize(0);
    // 4. Get the Result Iterator
    tesseract::ResultIterator* ri = api.GetIterator();
    tesseract::PageIteratorLevel level = tesseract::RIL_WORD;
	reportStream << "LAF analysis report:\n";
    int boxNumber = 1;
    if (ri != 0) {
        do {
            // Get the text
            const char* lineText = ri->GetUTF8Text(level);
            if (lineText == 0) {
                continue;
            }

            // Get the confidence
            float conf = ri->Confidence(level);

            // Get the bounding box
            int x1, y1, x2, y2;
            ri->BoundingBox(level, &x1, &y1, &x2, &y2);
            cv::Rect box(x1, y1, x2 - x1, y2 - y1);

            // Store the result
            results.emplace_back(std::string(lineText), box);

            // Draw a red rectangle on markedImage representing the box
			cv::Scalar colour = Scalar(0, 0, 255);
            if ( std::string(lineText).find('<',0) != std::string::npos)
				colour = Scalar(0, 255, 255); 
            cv::rectangle(markedImage, box, colour, 2);
            reportStream << "Box: " << boxNumber++  << "\t\t\t"  << lineText  << "\n";
            // Clean up the dynamically allocated string
#ifndef _DEBUG
            delete[] lineText;
#endif


        } while (ri->Next(level));

        // Clean up the iterator
        delete ri;
    }
	imwrite("laf_analysis.png", markedImage);
    // 5. Clean up Tesseract API
    api.End();

    return results;
}

void ImageProcessor::find_text(Mat& image)
{
    Mat dst = image.clone();
	blur(dst, dst, Size(5, 5));
    medianBlur(dst, dst, 5);
    GaussianBlur(dst,dst, Size2i( 5,5 ), 0);
    Mat dst2;
    bilateralFilter(dst, dst2, 9, 75, 75);
	// inverted binary threshold
	threshold(dst2, dst2, 60, 255, THRESH_BINARY);

	imwrite("blur.png", dst2);
    text_detect_db18(dst2, "c:/tessdata/DB_TD500_resnet50.onnx");

}
void ImageProcessor::fast_nl_means_denoise( Mat& img, Mat& dst)
{
	//fastNlMeansDenoising(img, dst, 5, 13, 33);
    //medianBlur(img, dst, 25);
    //Canny(img, dst, 7.0, 7.0);
    GaussianBlur(img, dst, Size(3, 3), 0);
    threshold(dst, dst, 55, 255, THRESH_BINARY);
    Mat src = dst.clone();
    imwrite("denoised.png", dst);
}

