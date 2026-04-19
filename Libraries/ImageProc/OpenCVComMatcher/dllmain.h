// dllmain.h : Declaration of module class.

class COpenCVComMatcherModule : public ATL::CAtlDllModuleT< COpenCVComMatcherModule >
{
public :
	DECLARE_LIBID(LIBID_OpenCVComMatcherLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_OPENCVCOMMATCHER, "{a106c1ff-c789-480d-89db-6e1441244f06}")
};

extern class COpenCVComMatcherModule _AtlModule;
