#include "StdAfx.h"
#include "FavAddDlgLauncher.h"
#include "onenote12.h"

BOOL ParseXml(WCHAR* wszXml, LPWSTR& wszId, LPWSTR& wszType)
{
    // ignore the root, direct to the children
    WCHAR* pTmp = wcschr(wszXml, L'>');
    _ASSERTE(pTmp); if (!pTmp) goto Exit;

    pTmp = wcschr(pTmp + 1, L'>');
    _ASSERTE(pTmp); if (!pTmp) goto Exit;

    pTmp = wcsstr(pTmp, L"isCurrentlyViewed");
    _ASSERTE(pTmp); if (!pTmp) goto Exit;

    WCHAR* pEnd = wcschr(pTmp, L'>');
    _ASSERTE(pEnd); if (!pEnd) goto Exit;

    if (pEnd) pEnd[1] = L'\0';
    WCHAR* wszLine = wcsrchr(wszXml, L'<');
    _ASSERTE(wszLine); if (!wszLine) goto Exit;

    // parse the type
    wszType = wszLine + 1;
    pTmp = wcschr(wszLine, L' ');
    _ASSERTE(pTmp); if (!pTmp) goto Exit;
    pTmp[0] = L'\0';

    // parse the id
    wszId = wcsstr(pTmp + 1, L"ID=\"");
    _ASSERTE(wszId); if (!wszId) goto Exit;
    wszId += 4;
    pTmp = wcschr(wszId, L'"');
    _ASSERTE(pTmp); if (!pTmp) goto Exit;
    pTmp[0] = L'\0';

    return TRUE;

Exit:
    return FALSE;
}

void CFavAddDlgLauncher::OpenAddFavoriteDlg()
{
    CComBSTR bstrId, bstrXml;
    WCHAR* wszId, * wszType;
    CComQIPtr<IApplication> ptrOneNoteApp;

    // create the addin com object, and open add new favorite dialog
    HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
    if (FAILED(hr)) goto Exit;

    if (!m_pAddFavAddIn)
    {
        CLSID clsid = {0};
        hr = ::CLSIDFromProgID(L"ONAddinFavAdd.AddinMain", &clsid);
        hr = ::CoCreateInstance(clsid, 0, CLSCTX_LOCAL_SERVER, __uuidof(IOneNoteAddIn), (LPVOID*)&m_pAddFavAddIn);
        if (FAILED(hr)) goto Exit;
    }

    // get the active page id
    do
    {
        hr = ptrOneNoteApp->GetHierarchy(bstrId, hsChildren, &bstrXml);
        if (!ParseXml(bstrXml, wszId, wszType)) goto Exit;
        bstrId = wszId;
    } while (_wcsicmp(wszType, L"one:Page"));

    // open add new favorite dialog here
    VARIANT_BOOL fEnabled = FALSE;
    hr = m_pAddFavAddIn->OnClick(bstrId, &fEnabled);

Exit:
    return;
}

CFavAddDlgLauncher::CFavAddDlgLauncher(void) : m_pAddFavAddIn(NULL)
{
}

CFavAddDlgLauncher::~CFavAddDlgLauncher(void)
{
    if (m_pAddFavAddIn)
    {
        m_pAddFavAddIn->Release();
        m_pAddFavAddIn = NULL;
    }
}
