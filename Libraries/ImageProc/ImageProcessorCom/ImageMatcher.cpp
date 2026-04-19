// ImageMatcher.cpp : Implementation of CImageMatcher

#include "pch.h"
#include "ImageMatcher.h"

// Helper: Convert COM SafeArray to cv::Mat
HRESULT CImageMatcher::SafeArrayToMat(SAFEARRAY* pSA, int width, int height, int channels, cv::Mat& outMat)
{
    if (!pSA) return E_POINTER;

    // Check array dimensions
    if (SafeArrayGetDim(pSA) != 1) return E_INVALIDARG; // Expecting 1D linear buffer

    // Access raw data
    unsigned char* pData = nullptr;
    HRESULT hr = SafeArrayAccessData(pSA, (void**)&pData);
    if (FAILED(hr)) return hr;

    try {
        int type = (channels == 1) ? CV_8UC1 : (channels == 3) ? CV_8UC3 : CV_8UC4;

        // Create Mat header. Note: This Mat uses the existing SafeArray memory.
        // We must clone it if we want it to persist after UnaccessData, 
        // but for immediate processing, this is efficient.
        cv::Mat wrapped(height, width, type, pData);

        // Clone to output to ensure data safety after we unlock the SafeArray
        outMat = wrapped.clone();
    }
    catch (const cv::Exception& e) {
        SafeArrayUnaccessData(pSA);
        return E_FAIL;
    }

    SafeArrayUnaccessData(pSA);
    return S_OK;
}

STDMETHODIMP CImageMatcher::MatchTemplate(
    SAFEARRAY* sourceImgData,
    LONG srcWidth,
    LONG srcHeight,
    LONG srcChannels,
    SAFEARRAY* templateImgData,
    LONG tmplWidth,
    LONG tmplHeight,
    LONG tmplChannels,
    LONG* matchX,
    LONG* matchY,
    DOUBLE* confidence,
    VARIANT_BOOL* found)
{
    // 1. Validate pointers
    if (!matchX || !matchY || !confidence || !found) return E_POINTER;

    // Initialize outputs
    *matchX = 0;
    *matchY = 0;
    *confidence = 0.0;
    *found = VARIANT_FALSE;

    // 2. Convert Inputs to cv::Mat
    cv::Mat img, templ;

    HRESULT hr = SafeArrayToMat(sourceImgData, srcWidth, srcHeight, srcChannels, img);
    if (FAILED(hr)) return hr;

    hr = SafeArrayToMat(templateImgData, tmplWidth, tmplHeight, tmplChannels, templ);
    if (FAILED(hr)) return hr;

    // 3. Validation Logic
    if (img.empty() || templ.empty()) return E_INVALIDARG;
    if (templ.cols > img.cols || templ.rows > img.rows) return E_INVALIDARG; // Template larger than image

    // 4. Ensure formats match (convert to Grayscale for matching usually works best, or keep color)
    // For this example, we ensure they are the same type.
    if (img.type() != templ.type()) {
        // Simple fallback: convert both to BGR or Gray if they differ
        // But for this COM method, we expect the caller to handle formats.
        return E_INVALIDARG;
    }

    try {
        // 5. Perform Template Matching
        cv::Mat result;
        // TM_CCOEFF_NORMED is standard. 1.0 = perfect match, -1.0 = mismatch.
        cv::matchTemplate(img, templ, result, cv::TM_CCOEFF_NORMED);

        // 6. Find best match
        double minVal, maxVal;
        cv::Point minLoc, maxLoc;
        cv::minMaxLoc(result, &minVal, &maxVal, &minLoc, &maxLoc);

        // 7. Return results
        *matchX = maxLoc.x;
        *matchY = maxLoc.y;
        *confidence = maxVal;

        // Threshold check (e.g., 0.8)
        if (maxVal >= 0.8) {
            *found = VARIANT_TRUE;
        }
        else {
            *found = VARIANT_FALSE;
        }
    }
    catch (const cv::Exception& e) {
        // OutputDebugStringA(e.what()); // Optional logging
        return E_FAIL;
    }

    return S_OK;
}