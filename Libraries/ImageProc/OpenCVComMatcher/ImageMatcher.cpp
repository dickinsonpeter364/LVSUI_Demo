// ImageMatcher.cpp : Implementation of CImageMatcher

#include "pch.h"
#include "ImageMatcher.h"
#include <atlstr.h> 
#include <sstream>
#include <msxml6.h>
#pragma comment(lib, "msxml6.lib")

// Tesseract & ZXing includes
#include <tesseract/baseapi.h>
#include "ZXing/ReadBarcode.h"
#include "ZXing/DecodeHints.h"
#include "ZXing/Result.h"
#include "ZXing/ImageView.h"

// Poppler includes
#include <poppler/cpp/poppler-document.h>
#include <poppler/cpp/poppler-image.h>
#include <poppler/cpp/poppler-page-renderer.h>
#include <poppler/cpp/poppler-page.h>


// =========================================================
//  Internal Helpers
// =========================================================

HRESULT CImageMatcher::ExtractImagesFromPdf(const std::string& pdfFilePath,
    std::vector<cv::Mat>& images) {
    images.clear();
    try {
        // Use unique_ptr to ensure doc is deleted
        std::unique_ptr<poppler::document> doc(
            poppler::document::load_from_file(pdfFilePath));
        if (!doc) {
            return E_FAIL;
        }

        int numPages = doc->pages();
        images.reserve(numPages);

        poppler::page_renderer renderer;
        renderer.set_render_hint(poppler::page_renderer::antialiasing, true);
        renderer.set_render_hint(poppler::page_renderer::text_antialiasing, true);

        for (int i = 0; i < numPages; ++i) {
            cv::Mat mat;
            std::unique_ptr<poppler::page> p(doc->create_page(i));
            if (p) {
                // Render page at 300 DPI
                poppler::image img = renderer.render_page(p.get(), 300, 300);
                if (img.is_valid()) {
                    // Convert poppler image to cv::Mat
                    // We wrap the poppler data in a temporary cv::Mat header,
                    // then perform a single copy (clone or cvtColor) to create the
                    // persistent Mat.
                    if (img.format() == poppler::image::format_rgb24) {
                        cv::Mat wrapped(img.height(), img.width(), CV_8UC3,
                            (void*)img.const_data(), img.bytes_per_row());
                        cv::cvtColor(wrapped, mat, cv::COLOR_RGB2BGR);
                    }
                    else if (img.format() == poppler::image::format_argb32) {
                        cv::Mat wrapped(img.height(), img.width(), CV_8UC4,
                            (void*)img.const_data(), img.bytes_per_row());
                        cv::cvtColor(wrapped, mat, cv::COLOR_BGRA2BGR);
                    }
                    else if (img.format() == poppler::image::format_bgr24) {
                        cv::Mat wrapped(img.height(), img.width(), CV_8UC3,
                            (void*)img.const_data(), img.bytes_per_row());
                        mat = wrapped.clone();
                    }
                }
            }
            // Push back (even if empty to preserve index if needed, or if failed)
            images.push_back(std::move(mat));
        }

        return S_OK;
    }
    catch (...) {
        return E_FAIL;
    }
}

int CImageMatcher::GetHistogramTrough(const cv::UMat& graySrc)
{
    if (graySrc.empty()) return -1;

    int histSize = 256;

    // Prepare arguments for UMat calcHist
    std::vector<cv::UMat> images = { graySrc };
    std::vector<int> channels = { 0 };
    std::vector<int> histSizes = { histSize };
    std::vector<float> ranges = { 0, 256 };

    cv::Mat hist;
    cv::calcHist(images, channels, cv::noArray(), hist, histSizes, ranges);

    // Smooth Histogram to remove noise
    cv::GaussianBlur(hist, hist, cv::Size(1, 9), 0);

	/*
    auto hImage = DrawHistogram(graySrc);
    std::stringstream ss;
    ss << "C:\\tmp\\histogram_" << (_imageNumber) << ".png";
	cv::imwrite(ss.str(), hImage);
    */
    float* h = (float*)hist.data;

    // 1. Find First Peak
    int firstPeak = 0;
    // Handle case where peak is at 0 (falling immediately)
    if (h[1] < h[0]) {
        firstPeak = 0;
    }
    else {
        // Find local maximum
        for (int i = 1; i < histSize - 1; i++) {
            if (h[i] > h[i - 1] && h[i] >= h[i + 1]) {
                firstPeak = i;
                break;
            }
        }
    }
     
    // 2. Find First Trough after First Peak
    int trough = -1;
    // Look for local minimum2`1
    for (int i = firstPeak + 1; i < histSize - 1; i++) {
        if (h[i] < h[i - 1] && h[i] < h[i + 1] -2) {
            trough = i;
            break;
        }
    }

    return trough;
}
cv::Mat CImageMatcher::DrawHistogram(const cv::UMat& graySrc)
{
    if (graySrc.empty()) return cv::Mat();

    int histSize = 256;
    std::vector<cv::UMat> images = { graySrc };
    std::vector<int> channels = { 0 };
    std::vector<int> histSizes = { histSize };
    std::vector<float> ranges = { 0, 256 };

    cv::Mat hist;
    cv::calcHist(images, channels, cv::noArray(), hist, histSizes, ranges);

    int hist_w = 512, hist_h = 400;
    int bin_w = cvRound((double)hist_w / histSize);
    cv::Mat histImage(hist_h, hist_w, CV_8UC3, cv::Scalar(0, 0, 0));

    // Normalize histogram to fit in the image
    cv::normalize(hist, hist, 0, histImage.rows, cv::NORM_MINMAX, -1, cv::Mat());

    for (int i = 1; i < histSize; i++)
    {
        cv::line(histImage,
            cv::Point(bin_w * (i - 1), hist_h - cvRound(hist.at<float>(i - 1))),
            cv::Point(bin_w * (i), hist_h - cvRound(hist.at<float>(i))),
            cv::Scalar(255, 255, 255), 2, 8, 0);
    }

    return histImage;
}

std::string CImageMatcher::RemoveWhitespace(const std::string& input)
{
    std::string output;
    output.reserve(input.length());
    for (char c : input) {
        // Keep only alphanumeric characters (removes whitespace, punctuation, symbols)
        if (std::isalnum(static_cast<unsigned char>(c))) {
            output += c;
        }
    }
    return output;
}

std::string CImageMatcher::ApplySubstitutions(const std::string& input)
{
    std::string result = input;
    for (const auto& sub : m_substitutionList) {
        if (sub.first.empty()) continue;
        size_t pos = 0;
        while ((pos = result.find(sub.first, pos)) != std::string::npos) {
            result.replace(pos, sub.first.length(), sub.second);
            pos += sub.second.length();
        }
    }
    return result;
}
void CImageMatcher::Log(const char* fmt, ...)
{
    FILE* fp = nullptr;
    // Open for appending
    if (fopen_s(&fp, "C:\\tmp\\ImageMatcher.log", "a+") == 0 && fp)
    {
        // Add Timestamp
        SYSTEMTIME st;
        GetLocalTime(&st);
        fprintf(fp, "[%04d-%02d-%02d %02d:%02d:%02d.%03d] ",
            st.wYear, st.wMonth, st.wDay,
            st.wHour, st.wMinute, st.wSecond, st.wMilliseconds);

        // Format message
        va_list args;
        va_start(args, fmt);
        vfprintf(fp, fmt, args);
        va_end(args);

        fprintf(fp, "\n");
        fclose(fp);
    }
}

cv::Mat CImageMatcher::ClipSourceToLargestObject(cv::Mat& src, int trough)
{
    if (src.empty()) return src;

    // Convert to grayscale
    cv::Mat gray;
    if (src.channels() >= 3) {
        cv::cvtColor(src, gray, cv::COLOR_BGR2GRAY);
    }
    else {
        gray = src;
    }

    // Threshold to mask out black background (pixel values < 65 considered background)
    cv::Mat mask;
    cv::threshold(gray, mask, trough, 255, cv::THRESH_BINARY);

    std::vector<std::vector<cv::Point>> contours;
    cv::findContours(mask, contours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_SIMPLE);

    if (contours.empty()) return src; // Return full image if no contours found

    // Find the largest contour by area
    double maxArea = -1.0;
    cv::Rect maxRect = cv::Rect(0, 0, src.cols, src.rows);

    for (const auto& cnt : contours) {
        double area = cv::contourArea(cnt);
        if (area > maxArea) {
            maxArea = area;
            maxRect = cv::boundingRect(cnt);
        }
    }

    // Safely clip (intersect with image bounds)
    maxRect = maxRect & cv::Rect(0, 0, src.cols, src.rows);

    return src(maxRect).clone();
}

// Helper: Convert cv::Mat to COM SafeArray
HRESULT CImageMatcher::MatToSafeArray(const cv::Mat& mat, SAFEARRAY** ppSA)
{
    try {
        if (mat.empty()) return E_FAIL;

        // Ensure continuous memory layout for memcpy
        cv::Mat contiguous;
        if (mat.isContinuous()) contiguous = mat;
        else contiguous = mat.clone();

        size_t totalBytes = contiguous.total() * contiguous.elemSize();

        // Create SafeArray of bytes (VT_UI1)
        SAFEARRAYBOUND bound;
        bound.cElements = (ULONG)totalBytes;
        bound.lLbound = 0;
        *ppSA = SafeArrayCreate(VT_UI1, 1, &bound);

        if (!*ppSA) return E_OUTOFMEMORY;

        // Copy data
        void* pDest = nullptr;
        HRESULT hr = SafeArrayAccessData(*ppSA, &pDest);
        if (FAILED(hr)) {
            SafeArrayDestroy(*ppSA);
            *ppSA = nullptr;
            return hr;
        }

        if (pDest) {
            memcpy(pDest, contiguous.data, totalBytes);
        }

        SafeArrayUnaccessData(*ppSA);
        return S_OK;
    }
    catch (...) {
        return E_FAIL;
    }
}

// Helper: Get text content of a named child node
std::string GetChildText(IXMLDOMNode* pParent, const wchar_t* childName) {
    if (!pParent) return "";

    CComPtr<IXMLDOMNodeList> pList;
    pParent->get_childNodes(&pList);
    long length = 0;
    pList->get_length(&length);

    for (long i = 0; i < length; ++i) {
        CComPtr<IXMLDOMNode> pNode;
        pList->get_item(i, &pNode);

        CComBSTR nodeName;
        pNode->get_nodeName(&nodeName);

        if (wcscmp(nodeName, childName) == 0) {
            CComBSTR text;
            pNode->get_text(&text);
            return std::string(CW2A(text));
        }
    }
    return "";
}

// Helper: Parse XML String into C++ Objects
LabelDef CImageMatcher::ParseLabelXml(BSTR xmlStr) {
    LabelDef label;

    CComPtr<IXMLDOMDocument2> pDoc;
    HRESULT hr = pDoc.CoCreateInstance(__uuidof(DOMDocument60));
    if (FAILED(hr)) return label;

    pDoc->put_async(VARIANT_FALSE);
    pDoc->put_validateOnParse(VARIANT_FALSE);
    pDoc->put_resolveExternals(VARIANT_FALSE);

    VARIANT_BOOL isSuccessful = VARIANT_FALSE;
    pDoc->loadXML(xmlStr, &isSuccessful);

    if (isSuccessful == VARIANT_FALSE) return label;

    CComPtr<IXMLDOMElement> pRoot;
    pDoc->get_documentElement(&pRoot);
    if (!pRoot) return label;

    label.name = GetChildText(pRoot, L"Name");

    CComPtr<IXMLDOMNode> pElementsNode;
    CComPtr<IXMLDOMNodeList> pChildren;
    pRoot->get_childNodes(&pChildren);
    long len = 0;
    pChildren->get_length(&len);

    for (long i = 0; i < len; i++) {
        CComPtr<IXMLDOMNode> node;
        pChildren->get_item(i, &node);
        CComBSTR name;
        node->get_nodeName(&name);
        if (wcscmp(name, L"Elements") == 0) {
            pElementsNode = node;
            break;
        }
    }

    if (pElementsNode) {
        CComPtr<IXMLDOMNodeList> pElemList;
        pElementsNode->get_childNodes(&pElemList);
        long elemCount = 0;
        pElemList->get_length(&elemCount);

        for (long i = 0; i < elemCount; ++i) {
            CComPtr<IXMLDOMNode> pNode;
            pElemList->get_item(i, &pNode);

            DOMNodeType nodeType;
            pNode->get_nodeType(&nodeType);
            if (nodeType != NODE_ELEMENT) continue;

            long x = std::stol(GetChildText(pNode, L"SearchX"));
            long y = std::stol(GetChildText(pNode, L"SearchY"));
            long w = std::stol(GetChildText(pNode, L"SearchWidth"));
            long h = std::stol(GetChildText(pNode, L"SearchHeight"));

            std::string xsiType;
            CComPtr<IXMLDOMNamedNodeMap> pAttrs;
            pNode->get_attributes(&pAttrs);
            if (pAttrs) {
                long attrCount = 0;
                pAttrs->get_length(&attrCount);
                for (long k = 0; k < attrCount; ++k) {
                    CComPtr<IXMLDOMNode> pAttr;
                    pAttrs->get_item(k, &pAttr);
                    CComBSTR attrName;
                    pAttr->get_nodeName(&attrName);
                    if (wcscmp(attrName, L"xsi:type") == 0 || wcscmp(attrName, L"type") == 0) {
                        CComBSTR bstrType;
                        pAttr->get_text(&bstrType);
                        xsiType = std::string(CW2A(bstrType));
                        break;
                    }
                }
            }

            std::string tmplFile = GetChildText(pNode, L"TemplateFileName");
            std::string txtSearch = GetChildText(pNode, L"SearchText");

            if (xsiType == "TemplateSearchDefinition" || (!tmplFile.empty() && xsiType.empty())) {
                auto t = std::make_shared<TemplateDef>();
                t->x = x; t->y = y; t->w = w; t->h = h;
                t->filename = tmplFile;
                label.elements.push_back(t);
            }
            else if (xsiType == "TextSearchDefinition" || (!txtSearch.empty() && xsiType.empty())) {
                auto t = std::make_shared<TextDef>();
                t->x = x; t->y = y; t->w = w; t->h = h;
                t->searchText = txtSearch;
                label.elements.push_back(t);
            }
            else if (xsiType == "DataMatrixDefinition") {
                auto d = std::make_shared<DataMatrixDef>();
                d->x = x; d->y = y; d->w = w; d->h = h;
                // Parse SearchText (or similar field) for DataMatrix expectation
                // Assuming XML reuses SearchText for the expected value
                d->symbolData = txtSearch;
                label.elements.push_back(d);
            }
        }
    }
    return label;
}


// Helper: Convert COM SafeArray to cv::Mat
HRESULT CImageMatcher::SafeArrayToMat(SAFEARRAY* pSA, int width, int height, int channels, cv::Mat& outMat)
{
    if (!pSA) return E_POINTER;
    if (SafeArrayGetDim(pSA) != 1) return E_INVALIDARG;

    unsigned char* pData = nullptr;
    HRESULT hr = SafeArrayAccessData(pSA, (void**)&pData);
    if (FAILED(hr)) return hr;

    try {
        int type = (channels == 1) ? CV_8UC1 : (channels == 3) ? CV_8UC3 : CV_8UC4;
        cv::Mat wrapped(height, width, type, pData);
        outMat = wrapped.clone(); // Deep copy
    }
    catch (const cv::Exception&) {
        SafeArrayUnaccessData(pSA);
        return E_FAIL;
    }

    SafeArrayUnaccessData(pSA);
    return S_OK;
}

// Private helper containing the core UMat/ROI logic
// CHANGED: img input is now UMat reference
HRESULT CImageMatcher::MatchInternal(
    cv::UMat& img,
    cv::Mat& templ,
    cv::Mat& mask,
    cv::Rect searchRect,
    LONG* matchX,
    LONG* matchY,
    DOUBLE* confidence,
    VARIANT_BOOL* found)
{
    if (img.empty() || templ.empty()) return E_INVALIDARG;
    if (templ.cols > img.cols || templ.rows > img.rows) return E_INVALIDARG;

    cv::UMat imgROI;
    cv::Rect imgBounds(0, 0, img.cols, img.rows);
    searchRect = searchRect & imgBounds;

    if (searchRect.area() <= 0 || searchRect.width < templ.cols || searchRect.height < templ.rows) {
        return E_INVALIDARG;
    }

    // Create ROI on the UMat (no data copy, just header manipulation or GPU sub-buffer)
    imgROI = img(searchRect);

    // Type check (UMat::type() works same as Mat::type())
    if (img.type() != templ.type()) return E_INVALIDARG;

    try {
        cv::UMat templU, resultU;

        // Upload template to GPU
        templ.copyTo(templU);

        if (mask.empty()) {
            cv::matchTemplate(imgROI, templU, resultU, cv::TM_CCOEFF_NORMED);
        }
        else {
            cv::bitwise_not(mask, mask);
            cv::UMat maskU;
            // Upload mask to GPU
            mask.copyTo(maskU);
            cv::matchTemplate(imgROI, templU, resultU, cv::TM_CCORR_NORMED, maskU);
        }

        double minVal, maxVal;
        cv::Point minLoc, maxLoc;
        cv::minMaxLoc(resultU, &minVal, &maxVal, &minLoc, &maxLoc);

        *matchX = maxLoc.x + searchRect.x;
        *matchY = maxLoc.y + searchRect.y;
        *confidence = maxVal;

        if (maxVal >= 0.8) {
            *found = VARIANT_TRUE;
        }
        else {
            *found = VARIANT_FALSE;
        }
    }
    catch (const cv::Exception&) {
        return E_FAIL;
    }

    return S_OK;
}

ImageDef CImageMatcher::ParseImageXml(BSTR xmlStr) {
    ImageDef def;

    CComPtr<IXMLDOMDocument2> pDoc;
    HRESULT hr = pDoc.CoCreateInstance(__uuidof(DOMDocument60));
    if (FAILED(hr)) return def;

    VARIANT_BOOL isSuccessful = VARIANT_FALSE;
    pDoc->loadXML(xmlStr, &isSuccessful);
    if (isSuccessful == VARIANT_FALSE) return def;

    CComPtr<IXMLDOMElement> pRoot;
    pDoc->get_documentElement(&pRoot);
    if (!pRoot) return def;

    // Assuming root is <ImageDefinition>
    try {
        std::string sX = GetChildText(pRoot, L"ClipX");
        std::string sY = GetChildText(pRoot, L"ClipY");
        std::string sW = GetChildText(pRoot, L"ClipWidth");
        std::string sH = GetChildText(pRoot, L"ClipHeight");
        std::string sRot = GetChildText(pRoot, L"RotationAngle");

        if (!sX.empty()) def.clipX = std::stol(sX);
        if (!sY.empty()) def.clipY = std::stol(sY);
        if (!sW.empty()) def.clipWidth = std::stol(sW);
        if (!sH.empty()) def.clipHeight = std::stol(sH);
        if (!sRot.empty()) def.rotationAngle = std::stol(sRot);
    }
    catch (...) {
        // Fallback or leave as defaults
    }
    return def;
}

// --------------------------------------------------------
// 1. SetSourceImage
// --------------------------------------------------------
STDMETHODIMP CImageMatcher::SetSourceImage(
    SAFEARRAY* imgData,
    LONG width,
    LONG height,
    LONG channels,
    BSTR xmlImageDef)
{
    cv::Mat tempMat;
    HRESULT hr = SafeArrayToMat(imgData, width, height, channels, tempMat);
    //cv::imwrite("C:\\tmp\\source_image.bmp", tempMat);
    if (FAILED(hr)) return hr;

    try {
        // Parse XML
        //ImageDef def = ParseImageXml(xmlImageDef);

        // 1. Clip the ORIGINAL Image
        cv::Rect imgBounds(0, 0, tempMat.cols, tempMat.rows);
		cv::UMat uTempMat;
		tempMat.copyTo(uTempMat);
		auto trough = GetHistogramTrough(uTempMat);
        cv::Mat clippedMat = ClipSourceToLargestObject(tempMat, trough);
        
        std::stringstream tt;
        tt << "C:\\tmp\\clipped_" << _imageNumber << ".bmp";
		cv::imwrite(tt.str(), clippedMat);
        
        cv::UMat uClipped, uRotated;
        clippedMat.copyTo(uClipped);
        int angle = 90;
        // 2. Rotate the CLIPPED Image (on GPU)
        //int angle = def.rotationAngle % 360;
        if (angle < 0) angle += 360;

        if (angle == 0) {
            uRotated = uClipped;
        }
        else if (angle == 90) {
            cv::rotate(uClipped, uRotated, cv::ROTATE_90_CLOCKWISE);
        }
        else if (angle == 180) {
            cv::rotate(uClipped, uRotated, cv::ROTATE_180);
        }
        else if (angle == 270) {
            cv::rotate(uClipped, uRotated, cv::ROTATE_90_COUNTERCLOCKWISE);
        }
        else {
            return E_INVALIDARG;
        }
        
        std::stringstream uu;
        uu << "C:\\tmp\\rotated_" << _imageNumber << ".bmp";
        cv::imwrite(uu.str(), uRotated);
        
        // Debug: Threshold logic (on GPU)
        cv::UMat uGray, uBaseline;        // GaussianBlur to remove noise if needed, or simply for smoothing before threshold
        //cv::GaussianBlur(rotatedMat, baseline, cv::Size(5, 5), 0, 0, cv::BORDER_DEFAULT);
        cv::bilateralFilter(uRotated, uBaseline, 7, 23, 23);
        if (uRotated.channels() >= 3) 
            cv::cvtColor(uRotated, uGray, cv::COLOR_BGR2GRAY);
        else
            uGray = uRotated;

        int thresh = GetHistogramTrough(uGray);

        cv::threshold(uGray, uRotated, thresh-20, 255, cv::THRESH_BINARY);

        // Upload to UMat
        std::stringstream ss;
        cv::Mat resultMat = uRotated.getMat(cv::ACCESS_READ);

        // Write debug file
        
        ss << "C:\\tmp\\" << _imageNumber << ".bmp";
        _imageNumber++;
        cv::imwrite(ss.str(), resultMat);
        
        uRotated.copyTo(m_CurrentSourceImageU);

        // Initialize m_MarkedSource as BGR for colored annotations
       // Initialize m_MarkedSource as BGR for colored annotations
        if (resultMat.channels() == 1) {
            cv::cvtColor(resultMat, m_MarkedSource, cv::COLOR_GRAY2BGR);
        }
        else {
            resultMat.copyTo(m_MarkedSource);
        }
    }
    catch (const cv::Exception&) {
        return E_FAIL;
    }

    return S_OK;
}

// --------------------------------------------------------
// 2. MatchTemplate (Stored Source, Byte-Array Template)
// --------------------------------------------------------
STDMETHODIMP CImageMatcher::MatchTemplate(
    SAFEARRAY* templateImgData,
    LONG tmplWidth,
    LONG tmplHeight,
    LONG tmplChannels,
    SAFEARRAY* maskImgData,
    LONG maskWidth,
    LONG maskHeight,
    LONG maskChannels,
    LONG searchX,
    LONG searchY,
    LONG searchWidth,
    LONG searchHeight,
    LONG* matchX,
    LONG* matchY,
    DOUBLE* confidence,
    VARIANT_BOOL* found)
{
    if (!matchX || !matchY || !confidence || !found) return E_POINTER;
    *matchX = 0; *matchY = 0; *confidence = 0.0; *found = VARIANT_FALSE;

    if (m_CurrentSourceImageU.empty()) return E_PENDING;

    cv::Mat templ, mask;
    HRESULT hr = SafeArrayToMat(templateImgData, tmplWidth, tmplHeight, tmplChannels, templ);
    if (FAILED(hr)) return hr;

    if (maskImgData != nullptr) {
        hr = SafeArrayToMat(maskImgData, maskWidth, maskHeight, maskChannels, mask);
        if (FAILED(hr)) return hr;
        if (!mask.empty() && (mask.rows != templ.rows || mask.cols != templ.cols)) return E_INVALIDARG;
    }

    cv::Rect searchRect(searchX, searchY, searchWidth, searchHeight);
    return MatchInternal(m_CurrentSourceImageU, templ, mask, searchRect, matchX, matchY, confidence, found);
}

// --------------------------------------------------------
// 3. MatchTemplateFromFile (Stored Source, File-Path Template)
// --------------------------------------------------------
STDMETHODIMP CImageMatcher::MatchTemplateFromFile(
    BSTR templateFilePath,
    BSTR maskFilePath,
    LONG searchX,
    LONG searchY,
    LONG searchWidth,
    LONG searchHeight,
    LONG* matchX,
    LONG* matchY,
    DOUBLE* confidence,
    VARIANT_BOOL* found)
{
    if (!matchX || !matchY || !confidence || !found) return E_POINTER;
    *matchX = 0; *matchY = 0; *confidence = 0.0; *found = VARIANT_FALSE;

    if (m_CurrentSourceImageU.empty()) return E_PENDING;

    std::string tmplPath((LPCSTR)CW2A(templateFilePath));
    std::string maskPath;
    if (maskFilePath != nullptr) maskPath = std::string((LPCSTR)CW2A(maskFilePath));

    try {
        cv::Mat templ = cv::imread(tmplPath, cv::IMREAD_COLOR);
        cv::Mat mask;
        if (!maskPath.empty()) mask = cv::imread(maskPath, cv::IMREAD_GRAYSCALE);

        if (templ.empty()) return E_INVALIDARG;

        cv::Rect searchRect(searchX, searchY, searchWidth, searchHeight);
        return MatchInternal(m_CurrentSourceImageU, templ, mask, searchRect, matchX, matchY, confidence, found);
    }
    catch (...) {
        return E_FAIL;
    }
}

// --------------------------------------------------------
// 4. GetDataMatrices (Stored Source)
// --------------------------------------------------------
std::string JsonEscape(const std::string& input);

// Internal helper method implementation
std::vector<cv::Rect> CImageMatcher::DetectDataMatrices(cv::UMat& img, std::vector<std::string>* outTexts)
{
    std::vector<cv::Rect> rects;
    if (img.empty()) return rects;

    // ZXing requires CPU access to pixels. We must download from UMat to Mat.
    cv::Mat cpuImg = img.getMat(cv::ACCESS_READ);

    cv::Mat gray;
    if (cpuImg.channels() == 3 || cpuImg.channels() == 4) {
        cv::cvtColor(cpuImg, gray, cv::COLOR_BGR2GRAY);
    }
    else {
        gray = cpuImg;
    }

    ZXing::ImageView zxImage(gray.data, gray.cols, gray.rows, ZXing::ImageFormat::Lum);

    // Depending on ZXing version, might be DecodeHints or ReaderOptions
    ZXing::ReaderOptions hints;
    hints.setFormats(ZXing::BarcodeFormat::DataMatrix);
    hints.setTryHarder(true);
    hints.setTryRotate(true);

    auto results = ZXing::ReadBarcodes(zxImage, hints);

    for (const auto& res : results) {
        auto pos = res.position();
        int x = pos.topLeft().x;
        int y = pos.topLeft().y;
        int w = std::abs(pos.bottomRight().x - pos.topLeft().x);
        int h = std::abs(pos.bottomRight().y - pos.topLeft().y);

        rects.push_back(cv::Rect(x, y, w, h));

        if (outTexts) {
            outTexts->push_back(res.text());
        }
    }

    return rects;
}

STDMETHODIMP CImageMatcher::GetDataMatrices(
    BSTR* jsonResult,
    VARIANT_BOOL* found)
{
    if (!jsonResult || !found) return E_POINTER;
    *found = VARIANT_FALSE;
    *jsonResult = nullptr;

    if (m_CurrentSourceImageU.empty()) return E_PENDING;

    try {
        std::vector<std::string> texts;
        std::vector<cv::Rect> rects = DetectDataMatrices(m_CurrentSourceImageU, &texts);

        if (rects.empty()) {
            CComBSTR emptyJson("[]");
            *jsonResult = emptyJson.Detach();
            return S_OK;
        }

        std::stringstream ss;
        ss << "[";
        for (size_t i = 0; i < rects.size(); ++i) {
            const auto& r = rects[i];
            std::string text = (i < texts.size()) ? texts[i] : "";

            ss << "{";
            ss << "\"x\":" << r.x << ",";
            ss << "\"y\":" << r.y << ",";
            ss << "\"w\":" << r.width << ",";
            ss << "\"h\":" << r.height << ",";
            ss << "\"text\":\"" << JsonEscape(text) << "\"";
            ss << "}";

            if (i < rects.size() - 1) {
                ss << ",";
            }
        }
        ss << "]";

        std::string jsonStr = ss.str();
        CComBSTR bstrResult(jsonStr.c_str());
        *jsonResult = bstrResult.Detach();
        *found = VARIANT_TRUE;

    }
    catch (...) {
        return E_FAIL;
    }

    return S_OK;
}

// --------------------------------------------------------
// 5. FillRectMasked (Internal Helper)
// --------------------------------------------------------
void CImageMatcher::FillRectMasked(cv::UMat& img, cv::Rect rect, cv::Scalar color, cv::Mat& mask)
{
    if (img.empty()) return;

    // Intersect provided rect with actual image bounds
    cv::Rect imgBounds(0, 0, img.cols, img.rows);
    cv::Rect validRect = rect & imgBounds;

    if (validRect.area() <= 0) return;

    // Get ROI from the UMat (zero-copy if handled by driver, or GPU view)
    cv::UMat roi = img(validRect);

    if (!mask.empty()) {
        cv::Mat validMask;

        // Clip the mask on CPU first
        if (mask.size() == validRect.size()) {
            validMask = mask;
        }
        else if (mask.size() == rect.size()) {
            cv::Rect maskROI(validRect.x - rect.x, validRect.y - rect.y, validRect.width, validRect.height);
            validMask = mask(maskROI);
        }
        else if (mask.size() == img.size()) {
            validMask = mask(validRect);
        }
        else {
            return;
        }

        // Upload mask to GPU UMat to perform the setTo operation
        cv::UMat maskU;
        validMask.copyTo(maskU);
        roi.setTo(color, maskU);
    }
    else {
        // No mask
        roi.setTo(color);
    }
}

// 6. MatchTemplates (Batch from Files)
STDMETHODIMP CImageMatcher::MatchTemplates(
    SAFEARRAY* templateFilePaths,
    LONG searchX,
    LONG searchY,
    LONG searchWidth,
    LONG searchHeight,
    BSTR* jsonResult,
    VARIANT_BOOL* anyFound)
{
    if (!jsonResult || !anyFound) return E_POINTER;
    *anyFound = VARIANT_FALSE;
    *jsonResult = nullptr;

    // Validate Input
    if (!templateFilePaths) return E_POINTER;
    if (SafeArrayGetDim(templateFilePaths) != 1) return E_INVALIDARG;
    if (m_CurrentSourceImageU.empty()) return E_PENDING;

    // Access BSTRs from SAFEARRAY
    BSTR* pVals = nullptr;
    HRESULT hr = SafeArrayAccessData(templateFilePaths, (void**)&pVals);
    if (FAILED(hr)) return hr;

    long lowerBound, upperBound;
    SafeArrayGetLBound(templateFilePaths, 1, &lowerBound);
    SafeArrayGetUBound(templateFilePaths, 1, &upperBound);

    std::stringstream ss;
    ss << "[";
    bool firstEntry = true;
    cv::Rect searchRect(searchX, searchY, searchWidth, searchHeight);

    try {
        for (long i = lowerBound; i <= upperBound; ++i) {
            std::string filePath((LPCSTR)CW2A(pVals[i]));

            // Load Template
            cv::Mat templ = cv::imread(filePath, cv::IMREAD_COLOR);
            if (templ.empty()) continue; // Skip invalid files

            // Matching inputs
            LONG x = 0, y = 0;
            DOUBLE conf = 0.0;
            VARIANT_BOOL found = VARIANT_FALSE;
            cv::Mat noMask; // Empty mask

            // Execute Match
            MatchInternal(m_CurrentSourceImageU, templ, noMask, searchRect, &x, &y, &conf, &found);

            // If found, append to JSON
            if (found == VARIANT_TRUE) {
                *anyFound = VARIANT_TRUE;

                if (!firstEntry) ss << ",";
                ss << "{";
                ss << "\"index\":" << i << ",";
                ss << "\"path\":\"" << JsonEscape(filePath) << "\",";
                ss << "\"x\":" << x << ",";
                ss << "\"y\":" << y << ",";
                ss << "\"confidence\":" << conf;
                ss << "}";
                firstEntry = false;
            }
        }
    }
    catch (...) {
        SafeArrayUnaccessData(templateFilePaths);
        return E_FAIL;
    }

    ss << "]";
    SafeArrayUnaccessData(templateFilePaths);

    // Return Result
    std::string jsonStr = ss.str();
    CComBSTR bstrResult(jsonStr.c_str());
    *jsonResult = bstrResult.Detach();

    return S_OK;
}
// 7. SetLabelDefinition
STDMETHODIMP CImageMatcher::SetLabelDefinition(BSTR xmlLabelDef)
{
    m_currentLabelDef = ParseLabelXml(xmlLabelDef);
    return S_OK;
}

// 8. PerformMatch
STDMETHODIMP CImageMatcher::PerformMatch(
    SAFEARRAY* keys,
    SAFEARRAY* values,
    VARIANT_BOOL* result)
{
    if (!result) return E_POINTER;
    *result = VARIANT_FALSE;

    if (!keys || !values) return E_INVALIDARG;
    if (SafeArrayGetDim(keys) != 1 || SafeArrayGetDim(values) != 1) return E_INVALIDARG;

    long lbK, ubK, lbV, ubV;
    HRESULT hrK = SafeArrayGetLBound(keys, 1, &lbK);
    SafeArrayGetUBound(keys, 1, &ubK);
    HRESULT hrV = SafeArrayGetLBound(values, 1, &lbV);
    SafeArrayGetUBound(values, 1, &ubV);

    if (FAILED(hrK) || FAILED(hrV)) return E_FAIL;
    if ((ubK - lbK) != (ubV - lbV)) return E_INVALIDARG;

    BSTR* pKeys = nullptr;
    BSTR* pValues = nullptr;
    HRESULT hrAccessK = SafeArrayAccessData(keys, (void**)&pKeys);
    HRESULT hrAccessV = SafeArrayAccessData(values, (void**)&pValues);

    if (FAILED(hrAccessK) || FAILED(hrAccessV)) {
        if (pKeys) SafeArrayUnaccessData(keys);
        if (pValues) SafeArrayUnaccessData(values);
        return E_FAIL;
    }

    try {
        m_substitutionList.clear();
        m_substitutionList.push_back({ "PRZ2", "PR2" });
        std::vector<std::pair<std::string, std::string>> replacements;
        long count = ubK - lbK + 1;
        for (long i = 0; i < count; i++) {
            std::string key = "<" + std::string((LPCSTR)CW2A(pKeys[i])) + ">";
            std::string val = std::string((LPCSTR)CW2A(pValues[i]));
            m_substitutionList.push_back({ key, val });
            replacements.push_back({ key, val });
        }

        LabelDef newLabel;
        newLabel.name = m_currentLabelDef.name;

        for (const auto& elem : m_currentLabelDef.elements) {
            if (auto t = std::dynamic_pointer_cast<TextDef>(elem)) {
                auto newT = std::make_shared<TextDef>(*t);
                newT->searchText = ApplySubstitutions(newT->searchText);
                newLabel.elements.push_back(newT);
            }
            else if (auto tmpl = std::dynamic_pointer_cast<TemplateDef>(elem)) {
                newLabel.elements.push_back(std::make_shared<TemplateDef>(*tmpl));
            }
            else if (auto dm = std::dynamic_pointer_cast<DataMatrixDef>(elem)) {
                auto newDm = std::make_shared<DataMatrixDef>(*dm);
                // Perform substitution on Data Matrix content as well
                newDm->symbolData = ApplySubstitutions(newDm->symbolData);
                newLabel.elements.push_back(newDm);
            }
        }

        bool allChecksPassed = true;
        for (const auto& elem : newLabel.elements) {
            if (auto t = std::dynamic_pointer_cast<TextDef>(elem)) {
                if (!CheckText(*t)) {
                    allChecksPassed = false;
                }
            }
            else if (auto dm = std::dynamic_pointer_cast<DataMatrixDef>(elem)) {
                if (!CheckDataMatrix(*dm)) {
                    allChecksPassed = false;
                }
            }
        }

        *result = allChecksPassed ? VARIANT_TRUE : VARIANT_FALSE;
    }
    catch (...) {
        SafeArrayUnaccessData(keys);
        SafeArrayUnaccessData(values);
        return E_FAIL;
    }

    SafeArrayUnaccessData(keys);
    SafeArrayUnaccessData(values);
    return S_OK;
}

// --------------------------------------------------------
// 10. RetrieveNormalisedImage
// --------------------------------------------------------
STDMETHODIMP CImageMatcher::RetrieveNormalisedImage(
    SAFEARRAY** pImgData,
    LONG* width,
    LONG* height,
    LONG* channels)
{
    if (!pImgData || !width || !height || !channels) return E_POINTER;

    try {
        if (m_CurrentSourceImageU.empty()) return E_PENDING;

        cv::Mat mat = m_CurrentSourceImageU.getMat(cv::ACCESS_READ);

        *width = mat.cols;
        *height = mat.rows;
        *channels = mat.channels();

        return MatToSafeArray(mat, pImgData);
    }
    catch (...) {
        return E_FAIL;
    }
}

// Lazy initializer for Tesseract API
tesseract::TessBaseAPI* CImageMatcher::GetTessApi(const char* lang)
{
    // Explicit path for Tesseract data files
    const char* kDataPath = "C:\\tessdata\\tessdata\\";

    // If not initialized, create new
    if (m_pTessApi == nullptr) {
        m_pTessApi = new tesseract::TessBaseAPI();
        if (m_pTessApi->Init(kDataPath, lang)) {
            // Init failed (check if path exists and contains .traineddata)
            delete m_pTessApi;
            m_pTessApi = nullptr;
            return nullptr;
        }
        m_currentTessLang = lang;
    }
    // If initialized but language changed, re-init
    else if (m_currentTessLang != lang) {
        m_pTessApi->End(); // Clear current data
        if (m_pTessApi->Init(kDataPath, lang)) {
            // Re-init failed
            delete m_pTessApi;
            m_pTessApi = nullptr;
            return nullptr;
        }
        m_currentTessLang = lang;
    }

    return m_pTessApi;
}

void CImageMatcher::FinalRelease()
{
    if (m_pTessApi) {
        m_pTessApi->End();
        delete m_pTessApi;
        m_pTessApi = nullptr;
    }
}
bool CImageMatcher::CheckDataMatrix(const DataMatrixDef& def) {
    if (m_CurrentSourceImageU.empty()) return false;
    bool matched = false;
    std::string decodedData = ""; // Store decoded text
    std::string format = "";

    try {
        
            //cv::Mat mat = m_CurrentSourceImageU.getMat(cv::ACCESS_READ);
            
            cv::Rect roi(def.x, def.y, def.w, def.h);
            roi = roi & cv::Rect(0, 0, m_currentSourceImageM.cols, m_currentSourceImageM.rows);
            if (roi.empty()) return false;

            // Extract and Convert to Gray/Mono for ZXing
            cv::Mat crop = m_currentSourceImageM(roi);
            cv::Mat gray;
            if (crop.channels() >= 3) cv::cvtColor(crop, gray, cv::COLOR_BGR2GRAY);
            else gray = crop;

            ZXing::ImageView zxImage(gray.data, gray.cols, gray.rows, ZXing::ImageFormat::Lum);
            ZXing::ReaderOptions hints;
            hints.setFormats(ZXing::BarcodeFormat::DataMatrix);
            hints.setTryHarder(true);
            hints.setTryRotate(true);
            hints.setTryInvert(true);
            hints.setTryDownscale(true);
            hints.setDownscaleFactor(5);
            

            auto result = ZXing::ReadBarcode(zxImage, hints);
            format = ZXing::ToString(result.format());
            Log("CheckDataMatrix: Format %s\ndata %s", format.c_str(), result.text().c_str());
            // Compare result if found
            
            decodedData = result.text(); // Capture text
            // Compare with symbolData (exact match or similar logic to text)
            // Assuming exact match requirement for Data Matrix content
            if (decodedData == def.symbolData) {
                matched = true;
            }
            
        

        // Marking Phase
        if (!m_MarkedSource.empty()) {
            cv::Scalar color = matched ? cv::Scalar(0, 255, 0) : cv::Scalar(0, 0, 255);
            cv::Rect roi(def.x, def.y, def.w, def.h);
            roi = roi & cv::Rect(0, 0, m_MarkedSource.cols, m_MarkedSource.rows);
            if (!roi.empty()) {
                // Draw a thicker rectangle or cross to distinguish? Or just rect.
                cv::rectangle(m_MarkedSource, roi, color, 5);

                // Write decoded data
                if (!decodedData.empty()) {
                    // Position at top right of ROI
                    cv::Point textOrigin(roi.x + roi.width, roi.y);
                    // Add some offset so it doesn't overlap the line immediately or stay offscreen?
                    // "to the top right" suggests referencing that corner.
                    color = cv::Scalar(255, 0, 0);
                    cv::putText(m_MarkedSource, decodedData, textOrigin, cv::FONT_HERSHEY_SIMPLEX, 3.0, color, 2);
                }
            }
        }
        return matched;
    }
    catch (...) {
        return false;
    }
}
bool CImageMatcher::CheckText(const TextDef& def) {
    if (m_CurrentSourceImageU.empty()) return false;

    // Use efficient helper (defaults to "eng" if not specified)
    tesseract::TessBaseAPI* api = GetTessApi("eng");
    if (!api) return false;

    bool matched = false;

    try {
        // Read Phase
        {
            cv::Mat mat = m_CurrentSourceImageU.getMat(cv::ACCESS_READ);
            cv::Rect roi(def.x, def.y, def.w, def.h);
            roi = roi & cv::Rect(0, 0, mat.cols, mat.rows);
            if (roi.empty()) return false;

            cv::Mat crop = mat(roi);

            api->SetPageSegMode(tesseract::PSM_SINGLE_BLOCK);
            api->SetVariable("tessedit_char_whitelist", "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ:./");

            int bytesPerPixel = crop.channels();
            api->SetImage(crop.data, crop.cols, crop.rows, bytesPerPixel, (int)crop.step);

            char* rawText = api->GetUTF8Text();
            std::string extractedText = "";
            if (rawText) {
                extractedText = std::string(rawText);
                delete[] rawText;
            }
            api->SetVariable("tessedit_char_whitelist", "");

            auto normalize = [](const std::string& s) -> std::string {
                std::string res;
                for (char c : s) if (std::isalnum(static_cast<unsigned char>(c))) res += c;
                return res;
                };
            Log("CheckText Begin");
            Log("Pre sub: %s", extractedText.c_str());
            auto substituted = ApplySubstitutions(extractedText);
            Log("Pos sub: %s\n", substituted.c_str());
            extractedText = substituted;
            std::string cleanExtracted = RemoveWhitespace(extractedText);
            std::string cleanSearch = RemoveWhitespace(def.searchText);
            Log("\nDetected:\t%s\nSearching:\t%s\n\n", cleanExtracted.c_str(), cleanSearch.c_str());
            if (cleanSearch.empty()) matched = true;
            else
            {
                if (cleanExtracted.size() > cleanSearch.size())
                {
                    if (cleanExtracted.substr(0, cleanSearch.size()) == cleanSearch)
                        matched = true;
                }
                else
                    matched = (cleanExtracted == cleanSearch);
            }
            Log("CheckText End");
        }

        // Marking Phase (Draw Green/Red Rect on m_MarkedSource)
        if (!m_MarkedSource.empty()) {
            cv::Scalar color = matched ? cv::Scalar(0, 255, 0) : cv::Scalar(0, 0, 255); // Green / Red
            cv::Rect roi(def.x, def.y, def.w, def.h);
            // Safety clip
            roi = roi & cv::Rect(0, 0, m_MarkedSource.cols, m_MarkedSource.rows);
            if (!roi.empty()) {
                cv::rectangle(m_MarkedSource, roi, color, 5);
            }
        }

        return matched;
    }
    catch (...) {
        return false;
    }
}

// Helper to escape JSON string
std::string JsonEscape(const std::string& input) {
    std::ostringstream ss;
    for (char c : input) {
        if (c == '"') ss << "\\\"";
        else if (c == '\\') ss << "\\\\";
        else if (c == '\b') ss << "\\b";
        else if (c == '\f') ss << "\\f";
        else if (c == '\n') ss << "\\n";
        else if (c == '\r') ss << "\\r";
        else if (c == '\t') ss << "\\t";
        else if (c >= 0x00 && c <= 0x1f) {} // Skip control chars
        else ss << c;
    }
    return ss.str();
}

// 6. PerformOCR (Updated to use persistent API)
STDMETHODIMP CImageMatcher::PerformOCR(
    BSTR language,
    LONG searchX,
    LONG searchY,
    LONG searchWidth,
    LONG searchHeight,
    BSTR* detectedText,
    DOUBLE* confidence,
    VARIANT_BOOL* success)
{
    if (!detectedText || !confidence || !success) return E_POINTER;
    *detectedText = nullptr;
    *confidence = 0.0;
    *success = VARIANT_FALSE;

    if (m_CurrentSourceImageU.empty()) return E_PENDING;

    std::string langStr((LPCSTR)CW2A(language));
    tesseract::TessBaseAPI* api = GetTessApi(langStr.c_str());
    if (!api) return E_FAIL;

    try {
        cv::Mat mat = m_CurrentSourceImageU.getMat(cv::ACCESS_READ);
        cv::Rect roi(searchX, searchY, searchWidth, searchHeight);
        roi = roi & cv::Rect(0, 0, mat.cols, mat.rows);
        if (roi.empty()) return E_INVALIDARG;

        cv::Mat crop = mat(roi);

        api->SetPageSegMode(tesseract::PSM_SINGLE_LINE);
        int bytesPerPixel = crop.channels();
        api->SetImage(crop.data, crop.cols, crop.rows, bytesPerPixel, (int)crop.step);

        char* text = api->GetUTF8Text();
        int meanConf = api->MeanTextConf();

        if (text != nullptr) {
            *detectedText = CComBSTR(text).Detach();
            *confidence = (double)meanConf / 100.0;
            *success = VARIANT_TRUE;
            delete[] text;
        }
    }
    catch (...) {
        return E_FAIL;
    }
    return S_OK;
}

// 9. GetMarkedImage
STDMETHODIMP CImageMatcher::GetMarkedImage(
    SAFEARRAY** pImgData,
    LONG* width,
    LONG* height,
    LONG* channels)
{
    if (!pImgData || !width || !height || !channels) return E_POINTER;

    try {
        if (m_MarkedSource.empty()) return E_PENDING;

        *width = m_MarkedSource.cols;
        *height = m_MarkedSource.rows;
        *channels = m_MarkedSource.channels();

        return MatToSafeArray(m_MarkedSource, pImgData);
    }
    catch (...) {
        return E_FAIL;
    }
}