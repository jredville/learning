
/*	Microsoft Office OneNote 2007 C/C++ Header file
	Copyright (c) 2006 Microsoft Corporation. All rights reserved.
	For more information on the OneNote API please go to:
	http://msdn.microsoft.com/office/
*/

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

#ifndef __OneNote12_h__
#define __OneNote12_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IApplication_FWD_DEFINED__
#define __IApplication_FWD_DEFINED__
typedef interface IApplication IApplication;
#endif 	/* __IApplication_FWD_DEFINED__ */


#ifndef __IOneNoteAddIn_FWD_DEFINED__
#define __IOneNoteAddIn_FWD_DEFINED__
typedef interface IOneNoteAddIn IOneNoteAddIn;
#endif 	/* __IOneNoteAddIn_FWD_DEFINED__ */


#ifndef __IOneNoteAddIn_FWD_DEFINED__
#define __IOneNoteAddIn_FWD_DEFINED__
typedef interface IOneNoteAddIn IOneNoteAddIn;
#endif 	/* __IOneNoteAddIn_FWD_DEFINED__ */


#ifndef __Application_FWD_DEFINED__
#define __Application_FWD_DEFINED__

#ifdef __cplusplus
typedef class Application Application;
#else
typedef struct Application Application;
#endif /* __cplusplus */

#endif 	/* __Application_FWD_DEFINED__ */


/* header files for imported files */
#include "comadmin.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

/* interface __MIDL_itf_ExeServer12_0000 */
/* [local] */ 

typedef /* [uuid] */  DECLSPEC_UUID("97CB5BF9-BF0C-47e5-A9BB-6B189BCA3C25") 
enum HierarchyScope
    {	hsSelf	= 0,
	hsChildren	= 1,
	hsNotebooks	= 2,
	hsSections	= 3,
	hsPages	= 4
    } 	HierarchyScope;

typedef /* [uuid] */  DECLSPEC_UUID("65D0EDAB-9D2F-479c-81C2-E0B481734320") 
enum CreateFileType
    {	cftNone	= 0,
	cftNotebook	= 1,
	cftFolder	= 2,
	cftSection	= 3
    } 	CreateFileType;

typedef /* [uuid] */  DECLSPEC_UUID("8E4BA554-9AC4-4e7b-B6E6-39705192F8D1") 
enum PageInfo
    {	piBasic	= 0,
	piBinaryData	= 1,
	piSelection	= 2,
	piBinaryDataSelection	= piBinaryData | piSelection,
	piAll	= piBasic | piBinaryData | piSelection
    } 	PageInfo;

typedef /* [uuid] */  DECLSPEC_UUID("B6876F11-4F18-4913-BF40-7698D08C791D") 
enum PublishFormat
    {	pfOneNote	= 0,
	pfOneNotePackage	= 1,
	pfMHTML	= 2,
	pfPDF	= 3,
	pfXPS	= 4,
	pfWord	= 5,
	pfEMF	= 6
    } 	PublishFormat;

typedef /* [uuid] */  DECLSPEC_UUID("2A0B42F4-2F24-4392-A800-F0A979424A57") 
enum SpecialLocation
    {	slBackUpFolder	= 0,
	slUnfiledNotesSection	= 1,
	slDefaultNotebookFolder	= 2
    } 	SpecialLocation;

typedef /* [uuid] */  DECLSPEC_UUID("47656582-CDF9-4933-86A5-4D192D6AC8CC") 
enum NewPageStyle
    {	npsDefault	= 0,
	npsBlankPageWithTitle	= 1,
	npsBlankPageNoTitle	= 2
    } 	NewPageStyle;



extern RPC_IF_HANDLE __MIDL_itf_ExeServer12_0000_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ExeServer12_0000_v0_0_s_ifspec;

#ifndef __IApplication_INTERFACE_DEFINED__
#define __IApplication_INTERFACE_DEFINED__

/* interface IApplication */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_IApplication;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("2DA16203-3F58-404f-839D-E4CDE7DD0DED")
    IApplication : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE GetHierarchy( 
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ HierarchyScope hsScope,
            /* [out] */ BSTR *pbstrHierarchyXmlOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdateHierarchy( 
            /* [in] */ BSTR bstrChangesXmlIn) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenHierarchy( 
            /* [in] */ BSTR bstrPath,
            /* [in] */ BSTR bstrRelativeToObjectID,
            /* [out] */ BSTR *pbstrObjectID,
            /* [defaultvalue][in] */ CreateFileType cftIfNotExist = cftNone) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeleteHierarchy( 
            /* [in] */ BSTR bstrObjectID,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CreateNewPage( 
            /* [in] */ BSTR bstrSectionID,
            /* [out] */ BSTR *pbstrPageID,
            /* [defaultvalue][in] */ NewPageStyle npsNewPageStyle = npsDefault) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE CloseNotebook( 
            /* [in] */ BSTR bstrNotebookID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHierarchyParent( 
            /* [in] */ BSTR bstrObjectID,
            /* [out] */ BSTR *pbstrParentID) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetPageContent( 
            /* [in] */ BSTR bstrPageID,
            /* [out] */ BSTR *pbstrPageXmlOut,
            /* [defaultvalue][in] */ PageInfo pageInfoToExport = piBasic) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE UpdatePageContent( 
            /* [in] */ BSTR bstrPageChangesXmlIn,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetBinaryPageContent( 
            /* [in] */ BSTR bstrPageID,
            /* [in] */ BSTR bstrCallbackID,
            /* [out] */ BSTR *pbstrBinaryObjectB64Out) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE DeletePageContent( 
            /* [in] */ BSTR bstrPageID,
            /* [in] */ BSTR bstrObjectID,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE NavigateTo( 
            /* [in] */ BSTR bstrHierarchyObjectID,
            /* [defaultvalue][in] */ BSTR bstrObjectID = 0,
            /* [defaultvalue][in] */ VARIANT_BOOL fNewWindow = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE Publish( 
            /* [in] */ BSTR bstrHierarchyID,
            /* [in] */ BSTR bstrTargetFilePath,
            /* [defaultvalue][in] */ PublishFormat pfPublishFormat = pfOneNote,
            /* [defaultvalue][in] */ BSTR bstrCLSIDofExporter = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE OpenPackage( 
            /* [in] */ BSTR bstrPathPackage,
            /* [in] */ BSTR bstrPathDest,
            /* [out] */ BSTR *pbstrPathOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetHyperlinkToObject( 
            /* [in] */ BSTR bstrHierarchyID,
            /* [in] */ BSTR bstrPageContentObjectID,
            /* [out] */ BSTR *pbstrHyperlinkOut) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindPages( 
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ BSTR bstrSearchString,
            /* [out] */ BSTR *pbstrHierarchyXmlOut,
            /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages = 0,
            /* [defaultvalue][in] */ VARIANT_BOOL fDisplay = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE FindMeta( 
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ BSTR bstrSearchStringName,
            /* [out] */ BSTR *pbstrHierarchyXmlOut,
            /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages = 0) = 0;
        
        virtual HRESULT STDMETHODCALLTYPE GetSpecialLocation( 
            /* [in] */ SpecialLocation slToGet,
            /* [out] */ BSTR *pbstrSpecialLocationPath) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct IApplicationVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IApplication * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IApplication * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IApplication * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IApplication * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IApplication * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IApplication * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IApplication * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *GetHierarchy )( 
            IApplication * This,
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ HierarchyScope hsScope,
            /* [out] */ BSTR *pbstrHierarchyXmlOut);
        
        HRESULT ( STDMETHODCALLTYPE *UpdateHierarchy )( 
            IApplication * This,
            /* [in] */ BSTR bstrChangesXmlIn);
        
        HRESULT ( STDMETHODCALLTYPE *OpenHierarchy )( 
            IApplication * This,
            /* [in] */ BSTR bstrPath,
            /* [in] */ BSTR bstrRelativeToObjectID,
            /* [out] */ BSTR *pbstrObjectID,
            /* [defaultvalue][in] */ CreateFileType cftIfNotExist);
        
        HRESULT ( STDMETHODCALLTYPE *DeleteHierarchy )( 
            IApplication * This,
            /* [in] */ BSTR bstrObjectID,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified);
        
        HRESULT ( STDMETHODCALLTYPE *CreateNewPage )( 
            IApplication * This,
            /* [in] */ BSTR bstrSectionID,
            /* [out] */ BSTR *pbstrPageID,
            /* [defaultvalue][in] */ NewPageStyle npsNewPageStyle);
        
        HRESULT ( STDMETHODCALLTYPE *CloseNotebook )( 
            IApplication * This,
            /* [in] */ BSTR bstrNotebookID);
        
        HRESULT ( STDMETHODCALLTYPE *GetHierarchyParent )( 
            IApplication * This,
            /* [in] */ BSTR bstrObjectID,
            /* [out] */ BSTR *pbstrParentID);
        
        HRESULT ( STDMETHODCALLTYPE *GetPageContent )( 
            IApplication * This,
            /* [in] */ BSTR bstrPageID,
            /* [out] */ BSTR *pbstrPageXmlOut,
            /* [defaultvalue][in] */ PageInfo pageInfoToExport);
        
        HRESULT ( STDMETHODCALLTYPE *UpdatePageContent )( 
            IApplication * This,
            /* [in] */ BSTR bstrPageChangesXmlIn,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified);
        
        HRESULT ( STDMETHODCALLTYPE *GetBinaryPageContent )( 
            IApplication * This,
            /* [in] */ BSTR bstrPageID,
            /* [in] */ BSTR bstrCallbackID,
            /* [out] */ BSTR *pbstrBinaryObjectB64Out);
        
        HRESULT ( STDMETHODCALLTYPE *DeletePageContent )( 
            IApplication * This,
            /* [in] */ BSTR bstrPageID,
            /* [in] */ BSTR bstrObjectID,
            /* [defaultvalue][in] */ DATE dateExpectedLastModified);
        
        HRESULT ( STDMETHODCALLTYPE *NavigateTo )( 
            IApplication * This,
            /* [in] */ BSTR bstrHierarchyObjectID,
            /* [defaultvalue][in] */ BSTR bstrObjectID,
            /* [defaultvalue][in] */ VARIANT_BOOL fNewWindow);
        
        HRESULT ( STDMETHODCALLTYPE *Publish )( 
            IApplication * This,
            /* [in] */ BSTR bstrHierarchyID,
            /* [in] */ BSTR bstrTargetFilePath,
            /* [defaultvalue][in] */ PublishFormat pfPublishFormat,
            /* [defaultvalue][in] */ BSTR bstrCLSIDofExporter);
        
        HRESULT ( STDMETHODCALLTYPE *OpenPackage )( 
            IApplication * This,
            /* [in] */ BSTR bstrPathPackage,
            /* [in] */ BSTR bstrPathDest,
            /* [out] */ BSTR *pbstrPathOut);
        
        HRESULT ( STDMETHODCALLTYPE *GetHyperlinkToObject )( 
            IApplication * This,
            /* [in] */ BSTR bstrHierarchyID,
            /* [in] */ BSTR bstrPageContentObjectID,
            /* [out] */ BSTR *pbstrHyperlinkOut);
        
        HRESULT ( STDMETHODCALLTYPE *FindPages )( 
            IApplication * This,
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ BSTR bstrSearchString,
            /* [out] */ BSTR *pbstrHierarchyXmlOut,
            /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages,
            /* [defaultvalue][in] */ VARIANT_BOOL fDisplay);
        
        HRESULT ( STDMETHODCALLTYPE *FindMeta )( 
            IApplication * This,
            /* [in] */ BSTR bstrStartNodeID,
            /* [in] */ BSTR bstrSearchStringName,
            /* [out] */ BSTR *pbstrHierarchyXmlOut,
            /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages);
        
        HRESULT ( STDMETHODCALLTYPE *GetSpecialLocation )( 
            IApplication * This,
            /* [in] */ SpecialLocation slToGet,
            /* [out] */ BSTR *pbstrSpecialLocationPath);
        
        END_INTERFACE
    } IApplicationVtbl;

    interface IApplication
    {
        CONST_VTBL struct IApplicationVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IApplication_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IApplication_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IApplication_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IApplication_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define IApplication_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define IApplication_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define IApplication_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define IApplication_GetHierarchy(This,bstrStartNodeID,hsScope,pbstrHierarchyXmlOut)	\
    (This)->lpVtbl -> GetHierarchy(This,bstrStartNodeID,hsScope,pbstrHierarchyXmlOut)

#define IApplication_UpdateHierarchy(This,bstrChangesXmlIn)	\
    (This)->lpVtbl -> UpdateHierarchy(This,bstrChangesXmlIn)

#define IApplication_OpenHierarchy(This,bstrPath,bstrRelativeToObjectID,pbstrObjectID,cftIfNotExist)	\
    (This)->lpVtbl -> OpenHierarchy(This,bstrPath,bstrRelativeToObjectID,pbstrObjectID,cftIfNotExist)

#define IApplication_DeleteHierarchy(This,bstrObjectID,dateExpectedLastModified)	\
    (This)->lpVtbl -> DeleteHierarchy(This,bstrObjectID,dateExpectedLastModified)

#define IApplication_CreateNewPage(This,bstrSectionID,pbstrPageID,npsNewPageStyle)	\
    (This)->lpVtbl -> CreateNewPage(This,bstrSectionID,pbstrPageID,npsNewPageStyle)

#define IApplication_CloseNotebook(This,bstrNotebookID)	\
    (This)->lpVtbl -> CloseNotebook(This,bstrNotebookID)

#define IApplication_GetHierarchyParent(This,bstrObjectID,pbstrParentID)	\
    (This)->lpVtbl -> GetHierarchyParent(This,bstrObjectID,pbstrParentID)

#define IApplication_GetPageContent(This,bstrPageID,pbstrPageXmlOut,pageInfoToExport)	\
    (This)->lpVtbl -> GetPageContent(This,bstrPageID,pbstrPageXmlOut,pageInfoToExport)

#define IApplication_UpdatePageContent(This,bstrPageChangesXmlIn,dateExpectedLastModified)	\
    (This)->lpVtbl -> UpdatePageContent(This,bstrPageChangesXmlIn,dateExpectedLastModified)

#define IApplication_GetBinaryPageContent(This,bstrPageID,bstrCallbackID,pbstrBinaryObjectB64Out)	\
    (This)->lpVtbl -> GetBinaryPageContent(This,bstrPageID,bstrCallbackID,pbstrBinaryObjectB64Out)

#define IApplication_DeletePageContent(This,bstrPageID,bstrObjectID,dateExpectedLastModified)	\
    (This)->lpVtbl -> DeletePageContent(This,bstrPageID,bstrObjectID,dateExpectedLastModified)

#define IApplication_NavigateTo(This,bstrHierarchyObjectID,bstrObjectID,fNewWindow)	\
    (This)->lpVtbl -> NavigateTo(This,bstrHierarchyObjectID,bstrObjectID,fNewWindow)

#define IApplication_Publish(This,bstrHierarchyID,bstrTargetFilePath,pfPublishFormat,bstrCLSIDofExporter)	\
    (This)->lpVtbl -> Publish(This,bstrHierarchyID,bstrTargetFilePath,pfPublishFormat,bstrCLSIDofExporter)

#define IApplication_OpenPackage(This,bstrPathPackage,bstrPathDest,pbstrPathOut)	\
    (This)->lpVtbl -> OpenPackage(This,bstrPathPackage,bstrPathDest,pbstrPathOut)

#define IApplication_GetHyperlinkToObject(This,bstrHierarchyID,bstrPageContentObjectID,pbstrHyperlinkOut)	\
    (This)->lpVtbl -> GetHyperlinkToObject(This,bstrHierarchyID,bstrPageContentObjectID,pbstrHyperlinkOut)

#define IApplication_FindPages(This,bstrStartNodeID,bstrSearchString,pbstrHierarchyXmlOut,fIncludeUnindexedPages,fDisplay)	\
    (This)->lpVtbl -> FindPages(This,bstrStartNodeID,bstrSearchString,pbstrHierarchyXmlOut,fIncludeUnindexedPages,fDisplay)

#define IApplication_FindMeta(This,bstrStartNodeID,bstrSearchStringName,pbstrHierarchyXmlOut,fIncludeUnindexedPages)	\
    (This)->lpVtbl -> FindMeta(This,bstrStartNodeID,bstrSearchStringName,pbstrHierarchyXmlOut,fIncludeUnindexedPages)

#define IApplication_GetSpecialLocation(This,slToGet,pbstrSpecialLocationPath)	\
    (This)->lpVtbl -> GetSpecialLocation(This,slToGet,pbstrSpecialLocationPath)

#endif /* COBJMACROS */


#endif 	/* C style interface */



HRESULT STDMETHODCALLTYPE IApplication_GetHierarchy_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrStartNodeID,
    /* [in] */ HierarchyScope hsScope,
    /* [out] */ BSTR *pbstrHierarchyXmlOut);


void __RPC_STUB IApplication_GetHierarchy_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_UpdateHierarchy_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrChangesXmlIn);


void __RPC_STUB IApplication_UpdateHierarchy_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_OpenHierarchy_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPath,
    /* [in] */ BSTR bstrRelativeToObjectID,
    /* [out] */ BSTR *pbstrObjectID,
    /* [defaultvalue][in] */ CreateFileType cftIfNotExist);


void __RPC_STUB IApplication_OpenHierarchy_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_DeleteHierarchy_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrObjectID,
    /* [defaultvalue][in] */ DATE dateExpectedLastModified);


void __RPC_STUB IApplication_DeleteHierarchy_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_CreateNewPage_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrSectionID,
    /* [out] */ BSTR *pbstrPageID,
    /* [defaultvalue][in] */ NewPageStyle npsNewPageStyle);


void __RPC_STUB IApplication_CreateNewPage_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_CloseNotebook_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrNotebookID);


void __RPC_STUB IApplication_CloseNotebook_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_GetHierarchyParent_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrObjectID,
    /* [out] */ BSTR *pbstrParentID);


void __RPC_STUB IApplication_GetHierarchyParent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_GetPageContent_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPageID,
    /* [out] */ BSTR *pbstrPageXmlOut,
    /* [defaultvalue][in] */ PageInfo pageInfoToExport);


void __RPC_STUB IApplication_GetPageContent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_UpdatePageContent_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPageChangesXmlIn,
    /* [defaultvalue][in] */ DATE dateExpectedLastModified);


void __RPC_STUB IApplication_UpdatePageContent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_GetBinaryPageContent_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPageID,
    /* [in] */ BSTR bstrCallbackID,
    /* [out] */ BSTR *pbstrBinaryObjectB64Out);


void __RPC_STUB IApplication_GetBinaryPageContent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_DeletePageContent_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPageID,
    /* [in] */ BSTR bstrObjectID,
    /* [defaultvalue][in] */ DATE dateExpectedLastModified);


void __RPC_STUB IApplication_DeletePageContent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_NavigateTo_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrHierarchyObjectID,
    /* [defaultvalue][in] */ BSTR bstrObjectID,
    /* [defaultvalue][in] */ VARIANT_BOOL fNewWindow);


void __RPC_STUB IApplication_NavigateTo_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_Publish_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrHierarchyID,
    /* [in] */ BSTR bstrTargetFilePath,
    /* [defaultvalue][in] */ PublishFormat pfPublishFormat,
    /* [defaultvalue][in] */ BSTR bstrCLSIDofExporter);


void __RPC_STUB IApplication_Publish_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_OpenPackage_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrPathPackage,
    /* [in] */ BSTR bstrPathDest,
    /* [out] */ BSTR *pbstrPathOut);


void __RPC_STUB IApplication_OpenPackage_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_GetHyperlinkToObject_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrHierarchyID,
    /* [in] */ BSTR bstrPageContentObjectID,
    /* [out] */ BSTR *pbstrHyperlinkOut);


void __RPC_STUB IApplication_GetHyperlinkToObject_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_FindPages_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrStartNodeID,
    /* [in] */ BSTR bstrSearchString,
    /* [out] */ BSTR *pbstrHierarchyXmlOut,
    /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages,
    /* [defaultvalue][in] */ VARIANT_BOOL fDisplay);


void __RPC_STUB IApplication_FindPages_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_FindMeta_Proxy( 
    IApplication * This,
    /* [in] */ BSTR bstrStartNodeID,
    /* [in] */ BSTR bstrSearchStringName,
    /* [out] */ BSTR *pbstrHierarchyXmlOut,
    /* [defaultvalue][in] */ VARIANT_BOOL fIncludeUnindexedPages);


void __RPC_STUB IApplication_FindMeta_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IApplication_GetSpecialLocation_Proxy( 
    IApplication * This,
    /* [in] */ SpecialLocation slToGet,
    /* [out] */ BSTR *pbstrSpecialLocationPath);


void __RPC_STUB IApplication_GetSpecialLocation_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IApplication_INTERFACE_DEFINED__ */


/* interface __MIDL_itf_ExeServer12_0124 */
/* [local] */ 

typedef /* [uuid] */  DECLSPEC_UUID("7EE8E517-FC5F-4ef6-9B3F-A6675CD07614") 
enum ToolbarIds
    {	tbMenuBar	= 0,
	tbStandard	= 1,
	tbFormatting	= 2,
	tbWritingTools	= 3,
	tbDrawingTools	= 4,
	tbAudioVideo	= 5,
	tbOutlining	= 6,
	tbMyPens	= 7,
	tbNoteFlags	= 8,
	tbOutlookTasks	= 9,
	tbFullPageView	= 10
    } 	ToolbarIds;

typedef /* [uuid] */  DECLSPEC_UUID("AAE363E2-3D91-4b0c-9021-EFDA0ACBD858") 
enum OneNoteAddIn_Event
    {	evtAddInNavigation	= 0,
	evtAddInHierarchyChange	= 1
    } 	OneNoteAddIn_Event;


extern RPC_IF_HANDLE __MIDL_itf_ExeServer12_0124_v0_0_c_ifspec;
extern RPC_IF_HANDLE __MIDL_itf_ExeServer12_0124_v0_0_s_ifspec;

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
            /* [iid_is][out] */ void **ppvObject);
        
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
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define IOneNoteAddIn_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define IOneNoteAddIn_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define IOneNoteAddIn_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define IOneNoteAddIn_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define IOneNoteAddIn_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define IOneNoteAddIn_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define IOneNoteAddIn_OnClick(This,bstrActivePageID,pfEnabled)	\
    (This)->lpVtbl -> OnClick(This,bstrActivePageID,pfEnabled)

#define IOneNoteAddIn_OnEvent(This,evt,bstrParameter,pfEnabled)	\
    (This)->lpVtbl -> OnEvent(This,evt,bstrParameter,pfEnabled)

#endif /* COBJMACROS */


#endif 	/* C style interface */



HRESULT STDMETHODCALLTYPE IOneNoteAddIn_OnClick_Proxy( 
    IOneNoteAddIn * This,
    /* [in] */ BSTR bstrActivePageID,
    /* [retval][out] */ VARIANT_BOOL *pfEnabled);


void __RPC_STUB IOneNoteAddIn_OnClick_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);


HRESULT STDMETHODCALLTYPE IOneNoteAddIn_OnEvent_Proxy( 
    IOneNoteAddIn * This,
    /* [in] */ OneNoteAddIn_Event evt,
    /* [in] */ BSTR bstrParameter,
    /* [retval][out] */ VARIANT_BOOL *pfEnabled);


void __RPC_STUB IOneNoteAddIn_OnEvent_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __IOneNoteAddIn_INTERFACE_DEFINED__ */



#ifndef __OneNote12_LIBRARY_DEFINED__
#define __OneNote12_LIBRARY_DEFINED__

/* library OneNote12 */
/* [custom][helpstring][uuid][version] */ 


typedef /* [uuid] */  DECLSPEC_UUID("D6362D32-B464-4a8b-BEFC-0E4F0C1709A0") 
enum Error
    {	hrMalformedXML	= 0x80042000,
	hrInvalidXML	= 0x80042001,
	hrCreatingSection	= 0x80042002,
	hrOpeningSection	= 0x80042003,
	hrSectionDoesNotExist	= 0x80042004,
	hrPageDoesNotExist	= 0x80042005,
	hrFileDoesNotExist	= 0x80042006,
	hrInsertingImage	= 0x80042007,
	hrInsertingInk	= 0x80042008,
	hrInsertingHtml	= 0x80042009,
	hrNavigatingToPage	= 0x8004200a,
	hrSectionReadOnly	= 0x8004200b,
	hrPageReadOnly	= 0x8004200c,
	hrInsertingOutlineText	= 0x8004200d,
	hrPageObjectDoesNotExist	= 0x8004200e,
	hrBinaryObjectDoesNotExist	= 0x8004200f,
	hrLastModifiedDateDidNotMatch	= 0x80042010,
	hrGroupDoesNotExist	= 0x80042011,
	hrPageDoesNotExistInGroup	= 0x80042012,
	hrNoActiveSelection	= 0x80042013,
	hrObjectDoesNotExist	= 0x80042014,
	hrNotebookDoesNotExist	= 0x80042015,
	hrInsertingFile	= 0x80042016,
	hrInvalidName	= 0x80042017,
	hrFolderDoesNotExist	= 0x80042018,
	hrInvalidQuery	= 0x80042019,
	hrFileAlreadyExists	= 0x8004201a,
	hrSectionEncryptedAndLocked	= 0x8004201b,
	hrDisabledByPolicy	= 0x8004201c,
	hrNotYetSynchronized	= 0x8004201d,
	hrLegacySection	= 0x8004201e
    } 	Error;


EXTERN_C const IID LIBID_OneNote12;

EXTERN_C const CLSID CLSID_Application;

#ifdef __cplusplus

class DECLSPEC_UUID("0039FFEC-A022-4232-8274-6B34787BFC27")
Application;
#endif
#endif /* __OneNote12_LIBRARY_DEFINED__ */

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


