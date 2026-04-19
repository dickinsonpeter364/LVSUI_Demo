#pragma once

#include <opencv2/highgui.hpp>
#include <opencv2/dnn.hpp>
#include <utility>
#include <vector>
#define VISION_TOLERANCE 0.0000001
#define D2R (CV_PI / 180.0)
#define R2D (180.0 / CV_PI)
#define MATCH_CANDIDATE_NUM 5

struct s_SingleTargetMatch
{
	cv::Point2d ptLT, ptRT, ptRB, ptLB, ptCenter;
	double dMatchedAngle;
	double dMatchScore;
};

struct s_TemplData
{
	std::vector<cv::Mat> vecPyramid;
	std::vector<cv::Scalar> vecTemplMean;
	std::vector<double> vecTemplNorm;
	std::vector<double> vecInvArea;
	std::vector<bool> vecResultEqual1;
	std::vector<s_SingleTargetMatch> m_vecSingleTargetData;

	bool bIsPatternLearned;
	int iBorderColor;
	void clear()
	{
		std::vector<double>().swap(vecTemplNorm);
		std::vector<double>().swap(vecInvArea);
		std::vector<cv::Scalar>().swap(vecTemplMean);
		std::vector<bool>().swap(vecResultEqual1);
	}
	void resize(int iSize)
	{
		vecTemplMean.resize(iSize);
		vecTemplNorm.resize(iSize, 0);
		vecInvArea.resize(iSize, 1);
		vecResultEqual1.resize(iSize, false);
	}
	s_TemplData() : iBorderColor(0)
	{
		bIsPatternLearned = false;
	}
};
struct s_MatchParameter
{
	cv::Point2d pt;
	double dMatchScore;
	double dMatchAngle;
	//Mat matRotatedSrc;
	cv::Rect rectRoi;
	double dAngleStart;
	double dAngleEnd;
	cv::RotatedRect rectR;
	cv::Rect rectBounding;
	bool bDelete;

	double vecResult[3][3];//for subpixel
	int iMaxScoreIndex;//for subpixel
	bool bPosOnBorder;
	cv::Point2d ptSubPixel;
	double dNewAngle;

	s_MatchParameter(cv::Point2f ptMinMax, double dScore, double dAngle)//, Mat matRotatedSrc = Mat ())
	{
		pt = ptMinMax;
		dMatchScore = dScore;
		dMatchAngle = dAngle;

		bDelete = false;
		dNewAngle = 0.0;

		bPosOnBorder = false;
	}
	s_MatchParameter()
	{
		double dMatchScore = 0;
		double dMatchAngle = 0;
	}
	~s_MatchParameter()
	{

	}
};
struct s_BlockMax
{
	struct Block
	{
		cv::Rect rect;
		double dMax;
		cv::Point ptMaxLoc;
		Block()
		{
		}
		Block(cv::Rect rect_, double dMax_, cv::Point ptMaxLoc_)
		{
			rect = rect_;
			dMax = dMax_;
			ptMaxLoc = ptMaxLoc_;
		}
	};
	s_BlockMax()
	{
	}

	cv::Mat matSrc;
	std::vector<Block> vecBlock;

	s_BlockMax(cv::Mat matSrc_, cv::Size sizeTemplate)
	{
		matSrc = matSrc_;
		//?matSrc ????block????????
		int iBlockW = sizeTemplate.width * 2;
		int iBlockH = sizeTemplate.height * 2;

		int iCol = matSrc.cols / iBlockW;
		bool bHResidue = matSrc.cols % iBlockW != 0;

		int iRow = matSrc.rows / iBlockH;
		bool bVResidue = matSrc.rows % iBlockH != 0;

		if (iCol == 0 || iRow == 0)
		{
			vecBlock.clear();
			return;
		}

		vecBlock.resize(iCol * iRow);
		int iCount = 0;
		for (int y = 0; y < iRow; y++)
		{
			for (int x = 0; x < iCol; x++)
			{
				cv::Rect rectBlock(x * iBlockW, y * iBlockH, iBlockW, iBlockH);
				vecBlock[iCount].rect = rectBlock;
				cv::minMaxLoc(matSrc(rectBlock), 0, &vecBlock[iCount].dMax, 0, &vecBlock[iCount].ptMaxLoc);
				vecBlock[iCount].ptMaxLoc += rectBlock.tl();
				iCount++;
			}
		}
		if (bHResidue && bVResidue)
		{
			cv::Rect rectRight(iCol * iBlockW, 0, matSrc.cols - iCol * iBlockW, matSrc.rows);
			Block blockRight;
			blockRight.rect = rectRight;
			minMaxLoc(matSrc(rectRight), 0, &blockRight.dMax, 0, &blockRight.ptMaxLoc);
			blockRight.ptMaxLoc += rectRight.tl();
			vecBlock.push_back(blockRight);

			cv::Rect rectBottom(0, iRow * iBlockH, iCol * iBlockW, matSrc.rows - iRow * iBlockH);
			Block blockBottom;
			blockBottom.rect = rectBottom;
			minMaxLoc(matSrc(rectBottom), 0, &blockBottom.dMax, 0, &blockBottom.ptMaxLoc);
			blockBottom.ptMaxLoc += rectBottom.tl();
			vecBlock.push_back(blockBottom);
		}
		else if (bHResidue)
		{
			cv::Rect rectRight(iCol * iBlockW, 0, matSrc.cols - iCol * iBlockW, matSrc.rows);
			Block blockRight;
			blockRight.rect = rectRight;
			minMaxLoc(matSrc(rectRight), 0, &blockRight.dMax, 0, &blockRight.ptMaxLoc);
			blockRight.ptMaxLoc += rectRight.tl();
			vecBlock.push_back(blockRight);
		}
		else
		{
			cv::Rect rectBottom(0, iRow * iBlockH, matSrc.cols, matSrc.rows - iRow * iBlockH);
			Block blockBottom;
			blockBottom.rect = rectBottom;
			minMaxLoc(matSrc(rectBottom), 0, &blockBottom.dMax, 0, &blockBottom.ptMaxLoc);
			blockBottom.ptMaxLoc += rectBottom.tl();
			vecBlock.push_back(blockBottom);
		}
	}
	void UpdateMax(cv::Rect rectIgnore)
	{
		if (vecBlock.size() == 0)
			return;
		//?????rectIgnore???block
		int iSize = vecBlock.size();
		for (int i = 0; i < iSize; i++)
		{
			cv::Rect rectIntersec = rectIgnore & vecBlock[i].rect;
			//???
			if (rectIntersec.width == 0 && rectIntersec.height == 0)
				continue;
			//?????????????
			cv::minMaxLoc(matSrc(vecBlock[i].rect), 0, &vecBlock[i].dMax, 0, &vecBlock[i].ptMaxLoc);
			vecBlock[i].ptMaxLoc += vecBlock[i].rect.tl();
		}
	}
	void GetMaxValueLoc(double& dMax, cv::Point& ptMaxLoc)
	{
		int iSize = vecBlock.size();
		if (iSize == 0)
		{
			minMaxLoc(matSrc, 0, &dMax, 0, &ptMaxLoc);
			return;
		}
		//?block?????
		int iIndex = 0;
		dMax = vecBlock[0].dMax;
		for (int i = 1; i < iSize; i++)
		{
			if (vecBlock[i].dMax >= dMax)
			{
				iIndex = i;
				dMax = vecBlock[i].dMax;
			}
		}
		ptMaxLoc = vecBlock[iIndex].ptMaxLoc;
	}
};

class ImageProcessor
{
public:
	static double m_dTolerance3;
	static double m_dTolerance4;
	static bool m_bStopLayer1;
	static bool m_bToleranceRange;
	static std::vector<s_SingleTargetMatch> m_vecSingleTargetData;

	static void watershed(cv::Mat& image);
	static void get_masked_image(const cv::UMat& image, cv::UMat& maskedImage, cv::UMat& foreground, int fileNum, bool with_cropped);
	static cv::UMat crop_image_from_background(const cv::UMat& image, int thresholdValue);
	static cv::Mat extract_dark_text_mask(const cv::Mat& sourceImage);
	static cv::Mat segment_by_background_intensity(const cv::Mat& grayImage, int k);
	static std::vector<cv::Mat> extractBackgroundAreas(const cv::Mat& grayImage, int k);
	static void SetCurrentPattern(const cv::Mat& pattern);
	static void SetSourceImage(const cv::Mat& src);
	static cv::Size GetBestRotationSize(cv::Size sizeSrc, cv::Size sizeDst, double dRAngle);
	static void SortPtWithCenter(std::vector<cv::Point2f>& vecSort);
	static void FilterWithRotatedRect(std::vector<s_MatchParameter>* vec, int iMethod, double dMaxOverLap);
	static cv::Point GetNextMaxLoc(cv::Mat& matResult, cv::Point ptMaxLoc, cv::Size sizeTemplate, double& dMaxValue,
	                               double dMaxOverlap);
	static bool SubPixEsimation(std::vector<s_MatchParameter>* vec, double* dNewX, double* dNewY, double* dNewAngle,
	                            double dAngleStep, int iMaxScoreIndex);
	static 	void GetRotatedROI(cv::Mat& matSrc, cv::Size size, cv::Point2f ptLT, double dAngle, cv::Mat& matROI);

	static bool Match();
	static void OutputRoi(s_SingleTargetMatch sstm);
	static void FilterWithScore(std::vector<s_MatchParameter>* vec, double dScore);
	static bool SetCurrentPattern(const std::string& patternFile);
	static bool LearnPattern();
	static std::vector<cv::Rect> GetNLargestContourBoxes(int N, const cv::Mat& image);
	static std::vector<cv::Rect> GetLogoCandidates(const cv::Mat& image, double minArea = 10000, double maxArea = 200000);
	static std::vector<cv::Rect> GetWhiteLabelRects(const cv::Mat& image, double minArea = 1000);
	static int GetTopLayer(const cv::Mat& matTempl, int iMinDstLength);
	static cv::Point2f findIntersection(cv::Vec2f line_h, cv::Vec2f line_v);
	static void segmentAndWrite(cv::Mat& gray_image,  const std::string& inputFilename, int k);
	static cv::Mat findAndCropCropMarks(cv::Mat& input_img);
	static cv::Mat removeColorControlStrips(const cv::Mat& input_img);
	static cv::Mat detect_edges(const cv::Mat& grayImage, double threshold1, double threshold2);
	static cv::Mat laplacian_filter(cv::Mat& img);
	static std::vector<cv::Rect2i> detect_text_swt(cv::UMat& img);
	static void apply_adaptive_histogram_eq(const cv::Mat& before, cv::Mat& after);
	static cv::Point2f ptRotatePt2f(cv::Point2f ptInput, cv::Point2f ptOrg, double dAngle);
	static void ccoeff_denominator(cv::Mat& matSrc, s_TemplData* pTemplData, cv::Mat& matResult, int iLayer);
	static void MatchTemplate(cv::Mat& matSrc, s_TemplData* pTemplData, cv::Mat& matResult, int iLayer, bool bUseSIMD);
	// Resize image so its largest dimension becomes targetMax (default 800) and
	// both dimensions are multiples of 32. The image is resized in-place and
	// the function returns a pair{scale_x, scale_y} representing the applied
	// scale factors for width and height respectively.
	static std::pair<double,double> resize_to_max_and_align(cv::Mat& img, int targetMax = 1200);

	// Text detection using DB (e.g., DB18). modelPath should point to the trained DB model (onnx or other supported format).
	// Returns axis-aligned bounding rectangles of detected text regions.
	static std::vector<cv::Rect> text_detect_db18(const cv::Mat& img, const std::string& modelPath,
		float boxThreshold = 0.6f, float binThreshold = 0.3f, int maxCandidates = 1000);
	static std::vector<std::pair<std::string, cv::Rect>> ocrWithTesseract(const cv::Mat& sourceImage, std::string tessDataPath, std::string langs, std::string report = "");
	static void find_text(cv::Mat& image);
	static void fast_nl_means_denoise(cv::Mat& img, cv::Mat& dst);
	static cv::Mat detectDataMatrix(const cv::UMat& sourceImage);

	static double m_dMaxOverlap;
	static double m_dScore;
	static double m_dToleranceAngle;
	static int m_iMinReduceArea;
	static int m_iMaxPos;
	static int m_iMessageCount;


private:
	static cv::UMat extract_rect(const cv::UMat& image, const cv::Rect& rect);

	static cv::Mat m_current_pattern;
	static cv::Mat m_matSrc;
	static double m_dSrcScale;
	static double m_dDstScale;

	static s_TemplData m_TemplData;
	static bool m_bDebugMode;
	static double m_dTolerance1;
	static double m_dTolerance2;
};


