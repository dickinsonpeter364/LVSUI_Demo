

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0628 */
/* at Tue Jan 19 03:14:07 2038
 */
/* Compiler settings for OpenCVComMatcher.idl:
    Oicf, W1, Zp8, env=Win64 (32b run), target_arch=AMD64 8.01.0628 
    protocol : all , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */



/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif /* __RPCNDR_H_VERSION__ */

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __OpenCVComMatcher_i_h__
#define __OpenCVComMatcher_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

#ifndef DECLSPEC_XFGVIRT
#if defined(_CONTROL_FLOW_GUARD_XFG)
#define DECLSPEC_XFGVIRT(base, func) __declspec(xfg_virtual(base, func))
#else
#define DECLSPEC_XFGVIRT(base, func)
#endif
#endif

/* Forward Declarations */ 

#ifndef __IImageMatcher_FWD_DEFINED__
#define __IImageMatcher_FWD_DEFINED__
typedef interface IImageMatcher IImageMatcher;

#endif 	/* __IImageMatcher_FWD_DEFINED__ */


#ifndef __ImageMatcher_FWD_DEFINED__
#define __ImageMatcher_FWD_DEFINED__

#ifdef __cplusplus
typedef class ImageMatcher ImageMatcher;
#else
typedef struct ImageMatcher ImageMatcher;
#endif /* __cplusplus */

#endif 	/* __ImageMatcher_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IImageMatcher_INTERFACE_DEFINED__
#define __IImageMatcher_INTERFACE_DEFINED__

/* interface IImageMatcher */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IImageMatcher;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("443CE94F-F9EB-4F5F-8FEA-2E3B0492260D")
    IImageMatcher : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetSourceImage( 
            /* [in] */ SAFEARRAY * imgData,
            /* [in] */ LONG width,
            /* [in] */ LONG height,
            /* [in] */ LONG channels,
            /* [in] */ BSTR xmlImageDef) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE MatchTemplate( 
            /* [in] */ SAFEARRAY * templateImgData,
            /* [in] */ LONG tmplWidth,
            /* [in] */ LONG tmplHeight,
            /* [in] */ LONG tmplChannels,
            /* [in] */ SAFEARRAY * maskImgData,
            /* [in] */ LONG maskWidth,
            /* [in] */ LONG maskHeight,
            /* [in] */ LONG maskChannels,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE MatchTemplateFromFile( 
            /* [in] */ BSTR templateFilePath,
            /* [in] */ BSTR maskFilePath,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetDataMatrices( 
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *found) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE MatchTemplates( 
            /* [in] */ SAFEARRAY * templateFilePaths,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *anyFound) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE PerformOCR( 
            /* [in] */ BSTR language,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ BSTR *detectedText,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *success) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE SetLabelDefinition( 
            /* [in] */ BSTR xmlLabelDef) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE PerformMatch( 
            /* [in] */ SAFEARRAY * keys,
            /* [in] */ SAFEARRAY * values,
            /* [retval][out] */ VARIANT_BOOL *result) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetMarkedImage( 
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RetrieveNormalisedImage( 
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE CreateAbsoluteMap( 
            /* [in] */ SAFEARRAY * imgData,
            /* [in] */ LONG width,
            /* [in] */ LONG height,
            /* [in] */ LONG channels,
            /* [in] */ BSTR l1PdfPath,
            /* [in] */ BSTR l2PdfPath,
            /* [in] */ DOUBLE dpi,
            /* [in] */ VARIANT_BOOL markImage,
            /* [in] */ BSTR imageFilePath,
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *success) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE ComputeContentRect( 
            /* [in] */ BSTR pdfPath,
            /* [out] */ DOUBLE *minX,
            /* [out] */ DOUBLE *minY,
            /* [out] */ DOUBLE *maxX,
            /* [out] */ DOUBLE *maxY,
            /* [retval][out] */ VARIANT_BOOL *success) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE RenderPdfPage( 
            /* [in] */ BSTR pdfPath,
            /* [in] */ DOUBLE dpi,
            /* [in] */ LONG pageIndex,
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels,
            /* [retval][out] */ VARIANT_BOOL *success) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IImageMatcherVtbl
    {
        BEGIN_INTERFACE
        
        DECLSPEC_XFGVIRT(IUnknown, QueryInterface)
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IImageMatcher * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        DECLSPEC_XFGVIRT(IUnknown, AddRef)
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IImageMatcher * This);
        
        DECLSPEC_XFGVIRT(IUnknown, Release)
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IImageMatcher * This);
        
        DECLSPEC_XFGVIRT(IDispatch, GetTypeInfoCount)
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IImageMatcher * This,
            /* [out] */ UINT *pctinfo);
        
        DECLSPEC_XFGVIRT(IDispatch, GetTypeInfo)
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IImageMatcher * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        DECLSPEC_XFGVIRT(IDispatch, GetIDsOfNames)
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IImageMatcher * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        DECLSPEC_XFGVIRT(IDispatch, Invoke)
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IImageMatcher * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        DECLSPEC_XFGVIRT(IImageMatcher, SetSourceImage)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetSourceImage )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * imgData,
            /* [in] */ LONG width,
            /* [in] */ LONG height,
            /* [in] */ LONG channels,
            /* [in] */ BSTR xmlImageDef);
        
        DECLSPEC_XFGVIRT(IImageMatcher, MatchTemplate)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *MatchTemplate )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * templateImgData,
            /* [in] */ LONG tmplWidth,
            /* [in] */ LONG tmplHeight,
            /* [in] */ LONG tmplChannels,
            /* [in] */ SAFEARRAY * maskImgData,
            /* [in] */ LONG maskWidth,
            /* [in] */ LONG maskHeight,
            /* [in] */ LONG maskChannels,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found);
        
        DECLSPEC_XFGVIRT(IImageMatcher, MatchTemplateFromFile)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *MatchTemplateFromFile )( 
            IImageMatcher * This,
            /* [in] */ BSTR templateFilePath,
            /* [in] */ BSTR maskFilePath,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found);
        
        DECLSPEC_XFGVIRT(IImageMatcher, GetDataMatrices)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetDataMatrices )( 
            IImageMatcher * This,
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *found);
        
        DECLSPEC_XFGVIRT(IImageMatcher, MatchTemplates)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *MatchTemplates )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * templateFilePaths,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *anyFound);
        
        DECLSPEC_XFGVIRT(IImageMatcher, PerformOCR)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *PerformOCR )( 
            IImageMatcher * This,
            /* [in] */ BSTR language,
            /* [in] */ LONG searchX,
            /* [in] */ LONG searchY,
            /* [in] */ LONG searchWidth,
            /* [in] */ LONG searchHeight,
            /* [out] */ BSTR *detectedText,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *success);
        
        DECLSPEC_XFGVIRT(IImageMatcher, SetLabelDefinition)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *SetLabelDefinition )( 
            IImageMatcher * This,
            /* [in] */ BSTR xmlLabelDef);
        
        DECLSPEC_XFGVIRT(IImageMatcher, PerformMatch)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *PerformMatch )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * keys,
            /* [in] */ SAFEARRAY * values,
            /* [retval][out] */ VARIANT_BOOL *result);
        
        DECLSPEC_XFGVIRT(IImageMatcher, GetMarkedImage)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetMarkedImage )( 
            IImageMatcher * This,
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels);
        
        DECLSPEC_XFGVIRT(IImageMatcher, RetrieveNormalisedImage)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RetrieveNormalisedImage )( 
            IImageMatcher * This,
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels);
        
        DECLSPEC_XFGVIRT(IImageMatcher, CreateAbsoluteMap)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *CreateAbsoluteMap )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * imgData,
            /* [in] */ LONG width,
            /* [in] */ LONG height,
            /* [in] */ LONG channels,
            /* [in] */ BSTR l1PdfPath,
            /* [in] */ BSTR l2PdfPath,
            /* [in] */ DOUBLE dpi,
            /* [in] */ VARIANT_BOOL markImage,
            /* [in] */ BSTR imageFilePath,
            /* [out] */ BSTR *jsonResult,
            /* [retval][out] */ VARIANT_BOOL *success);
        
        DECLSPEC_XFGVIRT(IImageMatcher, ComputeContentRect)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *ComputeContentRect )( 
            IImageMatcher * This,
            /* [in] */ BSTR pdfPath,
            /* [out] */ DOUBLE *minX,
            /* [out] */ DOUBLE *minY,
            /* [out] */ DOUBLE *maxX,
            /* [out] */ DOUBLE *maxY,
            /* [retval][out] */ VARIANT_BOOL *success);
        
        DECLSPEC_XFGVIRT(IImageMatcher, RenderPdfPage)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *RenderPdfPage )( 
            IImageMatcher * This,
            /* [in] */ BSTR pdfPath,
            /* [in] */ DOUBLE dpi,
            /* [in] */ LONG pageIndex,
            /* [out] */ SAFEARRAY * *pImgData,
            /* [out] */ LONG *width,
            /* [out] */ LONG *height,
            /* [out] */ LONG *channels,
            /* [retval][out] */ VARIANT_BOOL *success);
        
        END_INTERFACE
    } IImageMatcherVtbl;

    interface IImageMatcher
    {
        CONST_VTBL struct IImageMatcherVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IImageMatcher_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IImageMatcher_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IImageMatcher_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IImageMatcher_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IImageMatcher_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IImageMatcher_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IImageMatcher_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IImageMatcher_SetSourceImage(This,imgData,width,height,channels,xmlImageDef)	\
    ( (This)->lpVtbl -> SetSourceImage(This,imgData,width,height,channels,xmlImageDef) ) 

#define IImageMatcher_MatchTemplate(This,templateImgData,tmplWidth,tmplHeight,tmplChannels,maskImgData,maskWidth,maskHeight,maskChannels,searchX,searchY,searchWidth,searchHeight,matchX,matchY,confidence,found)	\
    ( (This)->lpVtbl -> MatchTemplate(This,templateImgData,tmplWidth,tmplHeight,tmplChannels,maskImgData,maskWidth,maskHeight,maskChannels,searchX,searchY,searchWidth,searchHeight,matchX,matchY,confidence,found) ) 

#define IImageMatcher_MatchTemplateFromFile(This,templateFilePath,maskFilePath,searchX,searchY,searchWidth,searchHeight,matchX,matchY,confidence,found)	\
    ( (This)->lpVtbl -> MatchTemplateFromFile(This,templateFilePath,maskFilePath,searchX,searchY,searchWidth,searchHeight,matchX,matchY,confidence,found) ) 

#define IImageMatcher_GetDataMatrices(This,jsonResult,found)	\
    ( (This)->lpVtbl -> GetDataMatrices(This,jsonResult,found) ) 

#define IImageMatcher_MatchTemplates(This,templateFilePaths,searchX,searchY,searchWidth,searchHeight,jsonResult,anyFound)	\
    ( (This)->lpVtbl -> MatchTemplates(This,templateFilePaths,searchX,searchY,searchWidth,searchHeight,jsonResult,anyFound) ) 

#define IImageMatcher_PerformOCR(This,language,searchX,searchY,searchWidth,searchHeight,detectedText,confidence,success)	\
    ( (This)->lpVtbl -> PerformOCR(This,language,searchX,searchY,searchWidth,searchHeight,detectedText,confidence,success) ) 

#define IImageMatcher_SetLabelDefinition(This,xmlLabelDef)	\
    ( (This)->lpVtbl -> SetLabelDefinition(This,xmlLabelDef) ) 

#define IImageMatcher_PerformMatch(This,keys,values,result)	\
    ( (This)->lpVtbl -> PerformMatch(This,keys,values,result) ) 

#define IImageMatcher_GetMarkedImage(This,pImgData,width,height,channels)	\
    ( (This)->lpVtbl -> GetMarkedImage(This,pImgData,width,height,channels) ) 

#define IImageMatcher_RetrieveNormalisedImage(This,pImgData,width,height,channels)	\
    ( (This)->lpVtbl -> RetrieveNormalisedImage(This,pImgData,width,height,channels) ) 

#define IImageMatcher_CreateAbsoluteMap(This,imgData,width,height,channels,l1PdfPath,l2PdfPath,dpi,markImage,imageFilePath,jsonResult,success)	\
    ( (This)->lpVtbl -> CreateAbsoluteMap(This,imgData,width,height,channels,l1PdfPath,l2PdfPath,dpi,markImage,imageFilePath,jsonResult,success) ) 

#define IImageMatcher_ComputeContentRect(This,pdfPath,minX,minY,maxX,maxY,success)	\
    ( (This)->lpVtbl -> ComputeContentRect(This,pdfPath,minX,minY,maxX,maxY,success) ) 

#define IImageMatcher_RenderPdfPage(This,pdfPath,dpi,pageIndex,pImgData,width,height,channels,success)	\
    ( (This)->lpVtbl -> RenderPdfPage(This,pdfPath,dpi,pageIndex,pImgData,width,height,channels,success) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IImageMatcher_INTERFACE_DEFINED__ */



#ifndef __OpenCVComMatcherLib_LIBRARY_DEFINED__
#define __OpenCVComMatcherLib_LIBRARY_DEFINED__

/* library OpenCVComMatcherLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_OpenCVComMatcherLib;

EXTERN_C const CLSID CLSID_ImageMatcher;

#ifdef __cplusplus

class DECLSPEC_UUID("456F81F6-6DCC-4DC9-BE23-55A18E67580B")
ImageMatcher;
#endif
#endif /* __OpenCVComMatcherLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

unsigned long             __RPC_USER  BSTR_UserSize64(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal64(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree64(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize64(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal64(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal64(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree64(     unsigned long *, LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


