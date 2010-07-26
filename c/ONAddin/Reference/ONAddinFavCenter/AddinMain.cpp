// AddinMain.cpp : Implementation of CAddinMain

#include "stdafx.h"
#include "AddinMain.h"
#include "AddinEventHandler.h"
#include "..\ONAddinFavAdd\ONFavorite.h"
#include "common.h"

// CAddinMain

CAddinMain* CAddinMain::s_pThis = NULL;

STDMETHODIMP CAddinMain::OnClick(BSTR bstrActivePageID, VARIANT_BOOL * pfEnabled)
{
    // show modal-less dialog
    if (ReadDataFile(0))
    {
        m_pSwitcher->ShowListViewDlg();
    }
    else
    {
        m_pSwitcher->ShowTreeViewDlg();
    }

    *pfEnabled = TRUE;

    return S_OK;
}

STDMETHODIMP CAddinMain::OnEvent(OneNoteAddIn_Event evt, BSTR bstrParameter, VARIANT_BOOL * pfEnabled)
{
    if (evtAddInNavigation == evt)
    {
        (new CAddinEventHandler(bstrParameter))->Start();
    }

    *pfEnabled = TRUE;

    return S_OK;
}

BOOL CAddinMain::CreateHiddenWindow()
{
    const TCHAR* tszClassName = _T("ONFAVADDIN_HOTKEY_MONITOR_CLASS");
    WNDCLASS    wc;
    wc.style            = CS_HREDRAW | CS_VREDRAW;
    wc.lpfnWndProc      = (WNDPROC) WndProc;
    wc.cbClsExtra       = 0;
    wc.cbWndExtra       = 0;
    wc.hInstance        = g_hInst;
    wc.hIcon            = NULL;
    wc.hCursor          = 0;
    wc.hbrBackground    = (HBRUSH) GetStockObject(WHITE_BRUSH);
    wc.lpszMenuName     = 0;
    wc.lpszClassName    = tszClassName;
    RegisterClass(&wc);
    
    m_hWndHidden = CreateWindow(tszClassName, _T(""), 0,
             CW_USEDEFAULT, CW_USEDEFAULT, CW_USEDEFAULT,
             CW_USEDEFAULT, NULL, NULL, g_hInst, NULL);

    return (m_hWndHidden != NULL);
}

LRESULT CALLBACK CAddinMain::WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        case WM_HOTKEY:
            if (FindWindow(_T("Framework::CFrame"), NULL))
            {
                HRESULT hr = S_OK;
                if (wParam == s_pThis->m_wFavCtrHotKey)
                {
                    VARIANT_BOOL fEnabled = FALSE;
                    hr = s_pThis->OnClick(NULL, &fEnabled);
                }
                else if (wParam == s_pThis->m_wAddFavHotKey)
                {
                    // create the addin com object, and open add new favorite dialog here
                    s_pThis->m_favAddDlgLauncher.OpenAddFavoriteDlg();
                }
            }
            break;
        default:
            return DefWindowProc(hWnd, message, wParam, lParam);
    }
    
    return 0;
}
