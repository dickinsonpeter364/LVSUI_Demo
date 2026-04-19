

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0628 */
/* at Tue Jan 19 03:14:07 2038
 */
/* Compiler settings for ImageProcessorCom.idl:
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

#ifndef __ImageProcessorCom_i_h__
#define __ImageProcessorCom_i_h__

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
    
    MIDL_INTERFACE("F88FB73F-CD9F-4D8A-AD18-03F5D1FC213F")
    IImageMatcher : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE MatchTemplate( 
            /* [in] */ SAFEARRAY * sourceImgData,
            /* [in] */ LONG srcWidth,
            /* [in] */ LONG srcHeight,
            /* [in] */ LONG srcChannels,
            /* [in] */ SAFEARRAY * templateImgData,
            /* [in] */ LONG tmplWidth,
            /* [in] */ LONG tmplHeight,
            /* [in] */ LONG tmplChannels,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found) = 0;
        
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
        
        DECLSPEC_XFGVIRT(IImageMatcher, MatchTemplate)
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *MatchTemplate )( 
            IImageMatcher * This,
            /* [in] */ SAFEARRAY * sourceImgData,
            /* [in] */ LONG srcWidth,
            /* [in] */ LONG srcHeight,
            /* [in] */ LONG srcChannels,
            /* [in] */ SAFEARRAY * templateImgData,
            /* [in] */ LONG tmplWidth,
            /* [in] */ LONG tmplHeight,
            /* [in] */ LONG tmplChannels,
            /* [out] */ LONG *matchX,
            /* [out] */ LONG *matchY,
            /* [out] */ DOUBLE *confidence,
            /* [retval][out] */ VARIANT_BOOL *found);
        
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


#define IImageMatcher_MatchTemplate(This,sourceImgData,srcWidth,srcHeight,srcChannels,templateImgData,tmplWidth,tmplHeight,tmplChannels,matchX,matchY,confidence,found)	\
    ( (This)->lpVtbl -> MatchTemplate(This,sourceImgData,srcWidth,srcHeight,srcChannels,templateImgData,tmplWidth,tmplHeight,tmplChannels,matchX,matchY,confidence,found) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IImageMatcher_INTERFACE_DEFINED__ */



#ifndef __ImageProcessorCom_LIBRARY_DEFINED__
#define __ImageProcessorCom_LIBRARY_DEFINED__

/* library ImageProcessorCom */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_ImageProcessorCom;

EXTERN_C const CLSID CLSID_ImageMatcher;

#ifdef __cplusplus

class DECLSPEC_UUID("DFA03317-EC6B-4CF7-9974-68E66E31F904")
ImageMatcher;
#endif
#endif /* __ImageProcessorCom_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree(     unsigned long *, LPSAFEARRAY * ); 

unsigned long             __RPC_USER  LPSAFEARRAY_UserSize64(     unsigned long *, unsigned long            , LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserMarshal64(  unsigned long *, unsigned char *, LPSAFEARRAY * ); 
unsigned char * __RPC_USER  LPSAFEARRAY_UserUnmarshal64(unsigned long *, unsigned char *, LPSAFEARRAY * ); 
void                      __RPC_USER  LPSAFEARRAY_UserFree64(     unsigned long *, LPSAFEARRAY * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


