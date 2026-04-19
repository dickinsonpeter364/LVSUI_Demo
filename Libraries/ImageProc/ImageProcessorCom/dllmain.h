// dllmain.h : Declaration of module class.
#include "ImageProcessorCom_i.h"
#include "resource.h"

#ifndef LIBID_ImageProcessorComLib
// Replace the GUID below with your actual LIBID from your .idl file if different
#define LIBID_ImageProcessorComLib  __uuidof(ImageProcessorComLib)
#endif

// Forward declare the library interface to resolve the identifier
interface __declspec(uuid("49c083c9-6492-4a4f-aa44-063e75a94206")) ImageProcessorComLib;

class CImageProcessorComModule : public ATL::CAtlDllModuleT< CImageProcessorComModule >
{
public:
	DECLARE_LIBID(LIBID_ImageProcessorComLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_IMAGEPROCESSORCOM, "{49c083c9-6492-4a4f-aa44-063e75a94206}")
};

extern class CImageProcessorComModule _AtlModule;