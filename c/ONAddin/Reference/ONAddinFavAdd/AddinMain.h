// AddinMain.h : Declaration of the CAddinMain

#pragma once
#include "resource.h"       // main symbols

#include "ONAddinFavAdd_i.h"
#include "AddinDlg.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

// CAddinThread

class CAddinThread : public CThread
{
public:
    CAddinThread(BSTR bstrActivePageID) : CThread(true)
    {
        m_bstrActivePageID = ::SysAllocString(bstrActivePageID);
    };

    ~CAddinThread()
    {
        ::SysFreeString(m_bstrActivePageID);
    };

private:
    BSTR m_bstrActivePageID;

private:
    int Run()
    {
        ::CoInitialize(0);
        HWND hWnd = FindWindow(_T("Framework::CFrame"), NULL);
        CAddinDlg dlg(m_bstrActivePageID);
        dlg.DoModal(hWnd);
        ::CoUninitialize();
        return 0;
    };
};

// CAddinMain

class ATL_NO_VTABLE CAddinMain :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CAddinMain, &CLSID_AddinMain>,
	public IDispatchImpl<IOneNoteAddIn, &IID_IOneNoteAddIn, &LIBID_ONAddinFavAddLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
	CAddinMain()
	{
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ADDINMAIN)


BEGIN_COM_MAP(CAddinMain)
	COM_INTERFACE_ENTRY(IOneNoteAddIn)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
        LOGGER << SV_DEBUG << L"ONAddinFavAdd constructing ..." << ENDL;
		return S_OK;
	}

	void FinalRelease()
	{
        LOGGER << SV_DEBUG << L"ONAddinFavAdd releasing ..." << ENDL;
	}

public:
    STDMETHOD(OnClick)(/*[in]*/ BSTR bstrActivePageID, /*[out, retval]*/ VARIANT_BOOL * pfEnabled);
    STDMETHOD(OnEvent)(/*[in]*/ OneNoteAddIn_Event evt, /*[in]*/ BSTR bstrParameter, /*[out, retval]*/ VARIANT_BOOL * pfEnabled);
};

OBJECT_ENTRY_AUTO(__uuidof(AddinMain), CAddinMain)
