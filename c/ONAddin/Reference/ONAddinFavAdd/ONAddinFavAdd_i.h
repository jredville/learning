

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 7.00.0499 */
/* at Mon Sep 08 17:33:11 2008
 */
/* Compiler settings for .\ONAddinFavAdd.idl:
    Oicf, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext, robust
    error checks: stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __ONAddinFavAdd_i_h__
#define __ONAddinFavAdd_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IOneNoteAddIn_FWD_DEFINED__
#define __IOneNoteAddIn_FWD_DEFINED__
typedef interface IOneNoteAddIn IOneNoteAddIn;
#endif 	/* __IOneNoteAddIn_FWD_DEFINED__ */


#ifndef __AddinMain_FWD_DEFINED__
#define __AddinMain_FWD_DEFINED__

#ifdef __cplusplus
typedef class AddinMain AddinMain;
#else
typedef struct AddinMain AddinMain;
#endif /* __cplusplus */

#endif 	/* __AddinMain_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_ONAddinFavAdd_0000_0000 */
/* [local] */ 

typedef /* [uuid] */  DECLSPEC_UUID("AAE363E2-3D91-4b0c-9021-EFDA0ACBD858") 
enum OneNoteAddIn_Event
    {	evtAddInNavigation	= 0,
	evtAddInHierarchyChange	= 1
    } 	OneNoteAddIn_Event;



extern RPC_IF_HANDLE __MIDL_itf_ONAddinFavAdd_0000_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ONAddinFavAdd_0000_0000_v0_0_s_ifspec;

#ifndef __IOneNoteAddIn_INTERFACE_DEFINED__
#define __IOneNoteAddIn_INTERFACE_DEFINED__

/* interface IOneNoteAddIn */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_IOneNoteAddIn;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("C9590FA7-2132-47fb-9A78-AF0BF19AF4E6")
    IOneNoteAddIn : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE OnClick( 
            /* [in] */ BSTR bstrActivePageID,
            /* [retval][out] */ VARIANT_BOOL *pfEnabled) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OnEvent( 
            /* [in] */ OneNoteAddIn_Event evt,
            /* [in] */ BSTR bstrParameter,
            /* [retval][out] */ VARIANT_BOOL *pfEnabled) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IOneNoteAddInVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IOneNoteAddIn * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ 
            __RPC__deref_out  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IOneNoteAddIn * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IOneNoteAddIn * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IOneNoteAddIn * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IOneNoteAddIn * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IOneNoteAddIn * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IOneNoteAddIn * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *OnClick )( 
            IOneNoteAddIn * This,
            /* [in] */ BSTR bstrActivePageID,
            /* [retval][out] */ VARIANT_BOOL *pfEnabled);
        
        HRESULT ( STDMETHODCALLTYPE *OnEvent )( 
            IOneNoteAddIn * This,
            /* [in] */ OneNoteAddIn_Event evt,
            /* [in] */ BSTR bstrParameter,
            /* [retval][out] */ VARIANT_BOOL *pfEnabled);
        
        END_INTERFACE
    } IOneNoteAddInVtbl;

    interface IOneNoteAddIn
    {
        CONST_VTBL struct IOneNoteAddInVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IOneNoteAddIn_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IOneNoteAddIn_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IOneNoteAddIn_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IOneNoteAddIn_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IOneNoteAddIn_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IOneNoteAddIn_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IOneNoteAddIn_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IOneNoteAddIn_OnClick(This,bstrActivePageID,pfEnabled)	\
    ( (This)->lpVtbl -> OnClick(This,bstrActivePageID,pfEnabled) ) 

#define IOneNoteAddIn_OnEvent(This,evt,bstrParameter,pfEnabled)	\
    ( (This)->lpVtbl -> OnEvent(This,evt,bstrParameter,pfEnabled) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IOneNoteAddIn_INTERFACE_DEFINED__ */



#ifndef __ONAddinFavAddLib_LIBRARY_DEFINED__
#define __ONAddinFavAddLib_LIBRARY_DEFINED__

/* library ONAddinFavAddLib */
/* [helpstring][version][uuid] */ 


EXTERN_C const IID LIBID_ONAddinFavAddLib;

EXTERN_C const CLSID CLSID_AddinMain;

#ifdef __cplusplus

class DECLSPEC_UUID("29DEF9C0-B770-4b5a-9742-3DF67B4FA575")
AddinMain;
#endif
#endif /* __ONAddinFavAddLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


