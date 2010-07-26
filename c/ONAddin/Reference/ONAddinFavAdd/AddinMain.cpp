// AddinMain.cpp : Implementation of CAddinMain

#include "stdafx.h"
#include "AddinMain.h"


// CAddinMain

STDMETHODIMP CAddinMain::OnClick(BSTR bstrActivePageID, VARIANT_BOOL * pfEnabled)
{
    if (!CAddinDlg::IsCurrentlyShowing())
    {
        (new CAddinThread(bstrActivePageID))->Start();
    }

    *pfEnabled = TRUE;

    return S_OK;
}

STDMETHODIMP CAddinMain::OnEvent(OneNoteAddIn_Event evt, BSTR bstrParameter, VARIANT_BOOL * pfEnabled)
{
    *pfEnabled = TRUE;

    return S_OK;
}
