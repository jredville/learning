#include "StdAfx.h"
#include "AddinEventHandler.h"
#include "onenote12.h"
#include "..\ONAddinFavAdd\ONFavorite.h"

CONFavorite g_lastFavNavigatedByURL;

CAddinEventHandler::CAddinEventHandler(BSTR bstrParameter) : CThread(true)
{
    m_bstrParameter = ::SysAllocString(bstrParameter);
}

CAddinEventHandler::~CAddinEventHandler(void)
{
    ::SysFreeString(m_bstrParameter);
}

int CAddinEventHandler::Run()
{
    ::CoInitialize(NULL);

    CString strLastUrl = g_lastFavNavigatedByURL.GetURI();
    CComBSTR bstrId(m_bstrParameter);
    CComQIPtr<IApplication> ptrOneNoteApp;

    // if g_lastFavNavigatedByURL is not empty
    if (strLastUrl.GetLength() <= 0) goto Exit;
    //_ASSERTE(0);
    HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
    if (FAILED(hr)) goto Exit;

    while (bstrId != _T(""))
    {
        CComBSTR bstrLink;
        hr = ptrOneNoteApp->GetHyperlinkToObject(bstrId, NULL, &bstrLink);
        if (FAILED(hr)) goto Exit;
        if (!strLastUrl.Compare(bstrLink))
        {
            g_lastFavNavigatedByURL.SetId(bstrId);
            g_lastFavNavigatedByURL.Save(true);
            break;
        }
        CComBSTR bstrParentId;
        hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        if (FAILED(hr)) goto Exit;
        bstrId = bstrParentId;
    }

    {
        // empty g_lastFavNavigatedByURL
        g_lastFavNavigatedByURL = CONFavorite();
    }

Exit:
    ::CoUninitialize();
    return 0;
}

