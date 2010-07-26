// AddinDlg.cpp : Implementation of CAddinDlg

#include "stdafx.h"
#include "AddinDlg.h"
#include "onenote12.h"
#include <windowsx.h>

extern HINSTANCE g_hInst;
LONG CAddinDlg::s_lShowingFlag = 0;

// CAddinDlg

void CAddinDlg::InitComboBox()
{
    HIMAGELIST hImageList = ImageList_Create(16, 16, ILC_COLOR16, 3, 10);
    DWORD BITMAP_ID_ARRAY[] = { IDB_BMP_PAGE, IDB_BMP_SECTION, IDB_BMP_SECTIONGROUP, IDB_BMP_NOTEBOOK };
    for (int i=0; i<sizeof(BITMAP_ID_ARRAY)/sizeof(BITMAP_ID_ARRAY[0]); ++i)
    {
        HBITMAP hBitMap = ::LoadBitmap(g_hInst, MAKEINTRESOURCE(BITMAP_ID_ARRAY[i]));
        int iPos = ImageList_Add(hImageList, hBitMap, NULL);
        DeleteObject(hBitMap);
        _ASSERTE(iPos >= 0);
    }
    LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_SETIMAGELIST, 0, (LPARAM)hImageList);
    _ASSERTE(!ret);
}

bool CAddinDlg::ParseXmlOut(TCHAR* tszXml, TCHAR** ptszName, OneNoteFavoriteType* pFavType)
{
    bool fRet = false;

    OneNoteFavoriteType favType;
    // parse the node type
    TCHAR* tszType = _tcschr(tszXml, _T('<'));
    _ASSERTE(tszType); if (!tszType) goto Erro;
    tszType = _tcschr(tszType + 1, _T('<'));
    _ASSERTE(tszType); if (!tszType) goto Erro;
    ++tszType;
    TCHAR* tszTemp = _tcschr(tszType, _T(' '));
    _ASSERTE(tszTemp); if (!tszTemp) goto Erro;
    tszTemp[0] = _T('\0');

    if (!_tcsicmp(tszType, _T("one:page")))
    {
        favType = ONFT_PAGE;
    }
    else if (!_tcsicmp(tszType, _T("one:section")))
    {
        favType = ONFT_SECTION;
    }
    else if (!_tcsicmp(tszType, _T("one:sectiongroup")))
    {
        favType = ONFT_SECTION_GROUP;
    }
    else if (!_tcsicmp(tszType, _T("one:notebook")))
    {
        favType = ONFT_NOTEBOOK;
    }
    else
    {
        goto Erro;
    }

    // parse the name
    TCHAR* tszName = _tcsstr(tszTemp + 1, _T("name=\""));
    _ASSERTE(tszName); if (!tszName) goto Erro;
    tszName += 6;
    tszTemp = _tcschr(tszName, _T('"'));
    _ASSERTE(tszTemp); if (!tszTemp) goto Erro;
    tszTemp[0] = _T('\0');

    // decode the name (&quot; &amp; &lt; &gt;)
    DecodeString(tszName);

    // decode the name (remove \ : * ? " < > |, and replace / with -)
    MakeFileName(tszName);

    *ptszName = tszName;
    *pFavType = favType;

    fRet = true;

Erro:
    return fRet;
}

bool CAddinDlg::LoadAllLinks()
{
    //_ASSERTE(0);
    CComBSTR bstrId = m_bstrActivePageID;
    CComQIPtr<IApplication> ptrOneNoteApp;

    if (0 == m_bstrActivePageID[0]) goto Erro;

    HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
    if (FAILED(hr)) goto Erro;

    bool fFirst = true;
    while (bstrId && bstrId != _T(""))
    {
        // Get hyper link
        BSTR bstrLink = NULL;
        hr = ptrOneNoteApp->GetHyperlinkToObject(bstrId, NULL, &bstrLink);
        if (FAILED(hr)) goto Erro;

        // Get name & type
        BSTR bstrXmlOut = NULL;
        hr = ptrOneNoteApp->GetHierarchy(bstrId, fFirst?hsPages:hsSelf, &bstrXmlOut);
        if (FAILED(hr)) goto Erro;

        TCHAR* tszName = NULL;
        OneNoteFavoriteType favType;
        bool fParseRet = ParseXmlOut(bstrXmlOut, &tszName, &favType);
        if (!fParseRet) break;

        // if the first (page), then update the UI with the text
        if (fFirst)
        {
            ::SetWindowText(m_hEditName, tszName);
            ::SetWindowText(m_hEditURI, bstrLink);
            fFirst = false;
        }

        // make a new ON fav object
        CONFavorite* pONFav = new CONFavorite;
        if (!pONFav) goto Erro;
        pONFav->SetId(bstrId);
        pONFav->SetName(tszName);
        pONFav->SetTitle(tszName);
        pONFav->SetURI(bstrLink);
        pONFav->SetType(favType);

        ::SysFreeString(bstrLink);
        ::SysFreeString(bstrXmlOut);

        // bind the ON fav object to the combobox item
        COMBOBOXEXITEM cbei;
        cbei.iItem = -1;
        cbei.mask = CBEIF_TEXT | CBEIF_IMAGE | CBEIF_SELECTEDIMAGE | CBEIF_LPARAM;
        cbei.cchTextMax = 64;
        cbei.lParam = (LPARAM) pONFav;
        switch(favType)
        {
        case ONFT_PAGE:
            cbei.pszText = _T("Page");
            cbei.iImage = cbei.iSelectedImage = 0;
            break;
        case ONFT_SECTION:
            cbei.pszText = _T("Section");
            cbei.iImage = cbei.iSelectedImage = 1;
            break;
        case ONFT_SECTION_GROUP:
            cbei.pszText = _T("SecGroup");
            cbei.iImage = cbei.iSelectedImage = 2;
            break;
        case ONFT_NOTEBOOK:
            cbei.pszText = _T("Notebook");
            cbei.iImage = cbei.iSelectedImage = 3;
            break;
        }

        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_INSERTITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);

        // go up to the parent
        BSTR bstrParentId = NULL;
        hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        if (FAILED(hr)) goto Erro;
        bstrId = bstrParentId;
        ::SysFreeString(bstrParentId);
    }

    return true;

Erro:
    return false;
}

LRESULT CAddinDlg::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
	CAxDialogImpl<CAddinDlg>::OnInitDialog(uMsg, wParam, lParam, bHandled);
	bHandled = TRUE;

    m_hAddinDlg = m_hWnd;

    m_hEditName = GetDlgItem(IDC_EDIT_NAME);
    m_hEditURI = GetDlgItem(IDC_EDIT_PATH);
    m_hStcTitle = GetDlgItem(IDC_STC_TITLE);
    m_hComboTypes = GetDlgItem(IDC_COMBO_TYPE);

    // change the title label font
    ::SendMessage(m_hStcTitle, WM_SETFONT, (WPARAM)m_hFont, 1);

    // Load the Onenote Favorite icon
    HWND hStcIcon = GetDlgItem(IDC_STC_ICON);
    ::MoveWindow(hStcIcon, 25, 25, 34, 36, TRUE);
    HANDLE hImg = LoadImage(g_hInst, MAKEINTRESOURCE(IDB_BMP_ICON), IMAGE_BITMAP, 0, 0, LR_LOADTRANSPARENT | LR_LOADMAP3DCOLORS);
    ::SendMessage(hStcIcon, STM_SETIMAGE, IMAGE_BITMAP, (LPARAM)hImg);

    // init the types in the combobox
    InitComboBox();

    if (LoadAllLinks())
    {
        ComboBox_SetCurSel(m_hComboTypes, 0);
    }
    else
    {
        MessageBox(_T("Cannot create OneNote favorite for this page!"), _T("Add a OneNote Favorite"), MB_OK | MB_ICONERROR);
        EndDialog(IDCANCEL);
    }

    ::InterlockedExchange(&s_lShowingFlag, 1);

	return 1;  // Let the system set the focus
}

LRESULT CAddinDlg::OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    // get saved ON fav object
    LRESULT ret = ::SendMessage(m_hComboTypes, CB_GETCURSEL, 0, 0);
    _ASSERTE(ret != CB_ERR);

    COMBOBOXEXITEM cbei;
    cbei.iItem = ret;
    cbei.mask = CBEIF_LPARAM;
    ret = ::SendMessage(m_hComboTypes, CBEM_GETITEM, 0, (LPARAM)&cbei);
    _ASSERTE(ret >= 0);

    CONFavorite* pONFav = (CONFavorite*) cbei.lParam;
    _ASSERTE(pONFav);

    CString strName;
	int iLen = ::GetWindowTextLength(m_hEditName);
	::GetWindowText(m_hEditName, strName.GetBufferSetLength(iLen), iLen + 1);

    CString strErrorMsg;

    // verify the name
    strName.Trim();
    if (strName.GetLength() == 0)
    {
        strErrorMsg = _T("Please enter the name of the OneNote favorite!");
        goto Erro;
    }
    if (strName.FindOneOf(_T("\\/:*?\"<>|")) >= 0)
    {
        strErrorMsg = _T("A file name cannot contain any of the following characters:\n\
                \\ / : * ? \" < > |");
        goto Erro;
    }

    // create the Onenote favorite object and save it
    pONFav->SetName(strName);
    pONFav->SetTitle(strName);

    BOOL fRet = pONFav->Save(false);
    if (fRet == ON_FAV_ALREADY_EXITST)
    {
        strErrorMsg = _T("The name you have entered for the shortcut already exists in your OneNote Favorites. Would you like to overwrite it?");
        if (IDNO == MessageBox(strErrorMsg, _T("Add a OneNote Favorite"), MB_YESNO | MB_ICONEXCLAMATION))
        {
            goto Exit;
        }
    }
    else if (fRet)
    {
        fRet = pONFav->Save(true);
    }

    if (!fRet)
    {
        strErrorMsg.Format(_T("Failed to save the OneNote favoite! Err: %X"), GetLastError());
        goto Erro;
    }

    EndDialog(wID);

Exit:
    return 0;

Erro:
    MessageBox(strErrorMsg, _T("Add to OneNote Favorites"), MB_ICONERROR | MB_OK);
	goto Exit;
}

LRESULT CAddinDlg::OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
	EndDialog(wID);
	return 0;
}

LRESULT CAddinDlg::OnCbnSelchangeComboType(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LRESULT ret = ::SendMessage(m_hComboTypes, CB_GETCURSEL, 0, 0);
    _ASSERTE(ret != CB_ERR);

    COMBOBOXEXITEM cbei;
    cbei.iItem = ret;
    cbei.mask = CBEIF_LPARAM;
    ret = ::SendMessage(m_hComboTypes, CBEM_GETITEM, 0, (LPARAM)&cbei);
    _ASSERTE(ret >= 0);

    CONFavorite* pONFav = (CONFavorite*) cbei.lParam;
    ::SetWindowText(m_hEditName, pONFav->GetName());
    ::SetWindowText(m_hEditURI, pONFav->GetURI());

    return 0;
}

LRESULT CAddinDlg::OnDestroy(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
{
    LRESULT lCount = ::SendMessage(m_hComboTypes, CB_GETCOUNT, 0, 0);
    _ASSERTE(lCount != CB_ERR);

    for (INT_PTR i=0; i<lCount; ++i)
    {
        COMBOBOXEXITEM cbei;
        cbei.iItem = i;
        cbei.mask = CBEIF_LPARAM;
        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_GETITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);

        CONFavorite* pONFav = (CONFavorite*) cbei.lParam;
        delete pONFav;
    }

    ::InterlockedExchange(&s_lShowingFlag, 0);

    return 0;
}

BOOL CAddinDlg::IsCurrentlyShowing()
{
    LONG lShowingFlag = ::InterlockedExchange(&s_lShowingFlag, s_lShowingFlag);
    return (lShowingFlag == 1);
}
