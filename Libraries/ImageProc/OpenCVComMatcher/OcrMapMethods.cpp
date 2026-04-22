// OcrMapMethods.cpp
// Implements CreateAbsoluteMap (id=11) and ComputeContentRect (id=12)
// for CImageMatcher.

#include "pch.h"
#include "ImageMatcher.h"
#include <atlstr.h>
#include <sstream>
#include <limits>
#include <cctype>

// ---------------------------------------------------------------------------
// Internal helpers
// ---------------------------------------------------------------------------

static std::string BstrToUtf8(BSTR bstr)
{
    if (!bstr) return "";
    int len = WideCharToMultiByte(CP_UTF8, 0, bstr, -1, nullptr, 0, nullptr, nullptr);
    if (len <= 0) return "";
    std::string result(len - 1, '\0');
    WideCharToMultiByte(CP_UTF8, 0, bstr, -1, &result[0], len, nullptr, nullptr);
    return result;
}

static std::string JsonEscapeOcr(const std::string& s)
{
    std::string out;
    for (unsigned char c : s) {
        switch (c) {
        case '"':  out += "\\\""; break;
        case '\\': out += "\\\\"; break;
        case '\n': out += "\\n";  break;
        case '\r': out += "\\r";  break;
        case '\t': out += "\\t";  break;
        default:   out += static_cast<char>(c); break;
        }
    }
    return out;
}

static std::string AbsoluteMapToJson(const ocr::OCRAnalysis::AbsoluteMapResult& r)
{
    std::ostringstream j;
    j << "{";
    j << "\"success\":" << (r.success ? "true" : "false") << ",";
    j << "\"errorMessage\":\"" << JsonEscapeOcr(r.errorMessage) << "\",";
    j << "\"imageWidth\":" << r.imageWidth << ",";
    j << "\"imageHeight\":" << r.imageHeight << ",";
    j << "\"cwRotations\":" << r.cwRotations << ",";
    j << "\"elements\":[";
    for (size_t i = 0; i < r.elements.size(); ++i) {
        const auto& e = r.elements[i];
        if (i > 0) j << ",";
        j << "{";
        j << "\"type\":\"" << (e.type == ocr::OCRAnalysis::AbsoluteElement::TEXT ? "TEXT" : "IMAGE") << "\",";
        j << "\"text\":\"" << JsonEscapeOcr(e.text) << "\",";
        j << "\"x\":" << e.x << ",";
        j << "\"y\":" << e.y << ",";
        j << "\"width\":" << e.width << ",";
        j << "\"height\":" << e.height << ",";
        j << "\"fontName\":\"" << JsonEscapeOcr(e.fontName) << "\",";
        j << "\"fontSize\":" << e.fontSize << ",";
        j << "\"isBold\":" << (e.isBold ? "true" : "false") << ",";
        j << "\"isItalic\":" << (e.isItalic ? "true" : "false");
        j << "}";
    }
    j << "]}";
    return j.str();
}

static bool StartsWithCI(const std::string& s, const std::string& prefix)
{
    if (s.size() < prefix.size()) return false;
    for (size_t i = 0; i < prefix.size(); ++i)
        if (std::tolower(static_cast<unsigned char>(s[i])) !=
            std::tolower(static_cast<unsigned char>(prefix[i])))
            return false;
    return true;
}

// ---------------------------------------------------------------------------
// 11. CreateAbsoluteMap
// ---------------------------------------------------------------------------
STDMETHODIMP CImageMatcher::CreateAbsoluteMap(
    SAFEARRAY* imgData,
    LONG width, LONG height, LONG channels,
    BSTR l1PdfPath,
    BSTR l2PdfPath,
    DOUBLE dpi,
    VARIANT_BOOL markImage,
    BSTR imageFilePath,
    BSTR* jsonResult,
    VARIANT_BOOL* success)
{
    if (!jsonResult || !success) return E_POINTER;
    *success = VARIANT_FALSE;
    *jsonResult = nullptr;

    try {
        // Reconstruct cv::Mat from SAFEARRAY
        cv::Mat image;
        HRESULT hr = SafeArrayToMat(imgData, width, height, channels, image);
        if (FAILED(hr) || image.empty()) {
            std::string err = "{\"success\":false,"
                              "\"errorMessage\":\"Failed to decode image SAFEARRAY\","
                              "\"elements\":[]}";
            *jsonResult = SysAllocStringByteLen(err.c_str(), static_cast<UINT>(err.size()));
            return S_OK;
        }

        std::string l1Path  = BstrToUtf8(l1PdfPath);
        std::string l2Path  = BstrToUtf8(l2PdfPath);
        std::string imgPath = BstrToUtf8(imageFilePath);
        bool doMark = (markImage == VARIANT_TRUE);

        // Extract PDF elements from L1 PDF
        ocr::OCRAnalysis analyzer;
        analyzer.initialize();
        auto elements = analyzer.extractPDFElements(l1Path);
        if (!elements.success) {
            std::string err = "{\"success\":false,\"errorMessage\":\"" +
                              JsonEscapeOcr("extractPDFElements failed: " + elements.errorMessage) +
                              "\",\"elements\":[]}";
            *jsonResult = SysAllocStringByteLen(err.c_str(), static_cast<UINT>(err.size()));
            return S_OK;
        }

        auto result = analyzer.createAbsoluteMap(
            elements, image, imgPath, doMark, l1Path, dpi, l2Path);

        std::string json = AbsoluteMapToJson(result);
        *jsonResult = SysAllocStringByteLen(json.c_str(), static_cast<UINT>(json.size()));
        *success = result.success ? VARIANT_TRUE : VARIANT_FALSE;
        return S_OK;
    }
    catch (const std::exception& ex) {
        std::string err = "{\"success\":false,\"errorMessage\":\"" +
                          JsonEscapeOcr(ex.what()) + "\",\"elements\":[]}";
        *jsonResult = SysAllocStringByteLen(err.c_str(), static_cast<UINT>(err.size()));
        return S_OK;
    }
    catch (...) {
        return E_FAIL;
    }
}

// ---------------------------------------------------------------------------
// 12. ComputeContentRect
// ---------------------------------------------------------------------------
STDMETHODIMP CImageMatcher::ComputeContentRect(
    BSTR pdfPath,
    DOUBLE* minX,
    DOUBLE* minY,
    DOUBLE* maxX,
    DOUBLE* maxY,
    VARIANT_BOOL* success)
{
    if (!minX || !minY || !maxX || !maxY || !success) return E_POINTER;
    *success = VARIANT_FALSE;
    *minX = *minY = *maxX = *maxY = 0.0;

    try {
        std::string path = BstrToUtf8(pdfPath);

        ocr::OCRAnalysis analyzer;
        analyzer.initialize();
        auto elements = analyzer.extractPDFElements(path);
        if (!elements.success) return S_OK;

        // Determine L1/L2 from filename stem
        std::string stem = path;
        auto sep = stem.find_last_of("/\\");
        if (sep != std::string::npos) stem = stem.substr(sep + 1);
        auto dot = stem.rfind('.');
        if (dot != std::string::npos) stem = stem.substr(0, dot);

        bool isL1 = StartsWithCI(stem, "l1");
        bool isL2 = StartsWithCI(stem, "l2");
        if (!isL1 && !isL2) return S_OK;

        if (isL1) {
            if (!elements.rectangles.empty()) {
                double rMinX = std::numeric_limits<double>::max();
                double rMinY = std::numeric_limits<double>::max();
                double rMaxX = std::numeric_limits<double>::lowest();
                double rMaxY = std::numeric_limits<double>::lowest();
                for (const auto& r : elements.rectangles) {
                    rMinX = std::min(rMinX, r.x);
                    rMinY = std::min(rMinY, r.y);
                    rMaxX = std::max(rMaxX, r.x + r.width);
                    rMaxY = std::max(rMaxY, r.y + r.height);
                }
                *minX = rMinX; *minY = rMinY; *maxX = rMaxX; *maxY = rMaxY;
                *success = VARIANT_TRUE;
                return S_OK;
            }
            if (!elements.images.empty()) {
                double iMinX = std::numeric_limits<double>::max();
                double iMinY = std::numeric_limits<double>::max();
                double iMaxX = std::numeric_limits<double>::lowest();
                double iMaxY = std::numeric_limits<double>::lowest();
                for (const auto& img : elements.images) {
                    iMinX = std::min(iMinX, img.x);
                    iMinY = std::min(iMinY, img.y);
                    iMaxX = std::max(iMaxX, img.x + img.displayWidth);
                    iMaxY = std::max(iMaxY, img.y + img.displayHeight);
                }
                *minX = iMinX; *minY = iMinY; *maxX = iMaxX; *maxY = iMaxY;
                *success = VARIANT_TRUE;
                return S_OK;
            }
        } else {
            // L2: crop-marks bounding box
            if (elements.linesBoundingBoxWidth > 0 &&
                elements.linesBoundingBoxHeight > 0) {
                *minX = elements.linesBoundingBoxX;
                *minY = elements.linesBoundingBoxY;
                *maxX = *minX + elements.linesBoundingBoxWidth;
                *maxY = *minY + elements.linesBoundingBoxHeight;
                *success = VARIANT_TRUE;
                return S_OK;
            }
        }
        return S_OK;
    }
    catch (...) {
        return E_FAIL;
    }
}
