// AddinMain.h : Declaration of the CAddinMain

#pragma once
#include "resource.h"       // main symbols

#include "ONAddinFavCenter_i.h"

#include "AddinDlg.h"
#include "AddinListViewDlg.h"

#include "FavAddDlgLauncher.h"

#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif



// CAddinMain

class ATL_NO_VTABLE CAddinMain :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CAddinMain, &CLSID_AddinMain>,
	public IDispatchImpl<IOneNoteAddIn, &IID_IOneNoteAddIn, &LIBID_ONAddinFavCenterLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
{
public:
    CAddinMain()
	{
        LOGGER << SV_DEBUG << L"Addin loaded..." << ENDL;
        m_pSwitcher = new CDlgSwitcher();
        s_pThis = this;
	}

DECLARE_REGISTRY_RESOURCEID(IDR_ADDINMAIN)


BEGIN_COM_MAP(CAddinMain)
	COM_INTERFACE_ENTRY(IOneNoteAddIn)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
	    INITCOMMONCONTROLSEX InitCtrls;
	    InitCtrls.dwSize = sizeof(InitCtrls);
	    InitCtrls.dwICC = ICC_WIN95_CLASSES;
	    InitCommonControlsEx(&InitCtrls);

        // create the hidden window for hotkey
        if (CreateHiddenWindow())
        {
            // register the hotkey (CTRL+ALT+I)
            m_wFavCtrHotKey = ::GlobalFindAtom(_T("ONFAVADDIN_FAVCENTER_HOTKEY"));
            if (!m_wFavCtrHotKey) m_wFavCtrHotKey = ::GlobalAddAtom(_T("ONFAVADDIN_FAVCENTER_HOTKEY"));
            ::RegisterHotKey(m_hWndHidden, m_wFavCtrHotKey, MOD_CONTROL | MOD_ALT, _T('I'));

            // register the hotkey (CTRL+ALT+D)
            m_wAddFavHotKey = ::GlobalFindAtom(_T("ONFAVADDIN_ADDFAV_HOTKEY"));
            if (!m_wAddFavHotKey) m_wAddFavHotKey = ::GlobalAddAtom(_T("ONFAVADDIN_ADDFAV_HOTKEY"));
            ::RegisterHotKey(m_hWndHidden, m_wAddFavHotKey, MOD_CONTROL | MOD_ALT, _T('D'));
        }

		return S_OK;
	}

	void FinalRelease()
	{
        LOGGER << SV_DEBUG << L"ONAddinFavCenter releasing..." << ENDL;

        ::UnregisterHotKey(m_hWndHidden, m_wFavCtrHotKey);
        ::UnregisterHotKey(m_hWndHidden, m_wAddFavHotKey);

        ::DestroyWindow(m_hWndHidden);
        m_hWndHidden = NULL;

        delete m_pSwitcher;
        m_pSwitcher = NULL;
	}

public:
    STDMETHOD(OnClick)(/*[in]*/ BSTR bstrActivePageID, /*[out, retval]*/ VARIANT_BOOL * pfEnabled);
    STDMETHOD(OnEvent)(/*[in]*/ OneNoteAddIn_Event evt, /*[in]*/ BSTR bstrParameter, /*[out, retval]*/ VARIANT_BOOL * pfEnabled);

private:
    static LRESULT CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);
    BOOL CreateHiddenWindow();

private:
    CDlgSwitcher* m_pSwitcher;
    HWND m_hWndHidden;
    ATOM m_wAddFavHotKey;
    ATOM m_wFavCtrHotKey;
    CFavAddDlgLauncher m_favAddDlgLauncher;
    static CAddinMain* s_pThis;
};

OBJECT_ENTRY_AUTO(__uuidof(AddinMain), CAddinMain)
