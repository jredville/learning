// AddinListViewDlg.cpp : Implementation of CAddinListViewDlg

#include "stdafx.h"
#include "AddinListViewDlg.h"
#include "onenote12.h"

extern CONFavorite g_lastFavNavigatedByURL;

// CAddinListViewDlg

LRESULT CAddinListViewDlg::OnBnClickedBtnListView(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    m_pSwitcher->ShowTreeViewDlg();
    return 0;
}

LRESULT CAddinListViewDlg::OnBnClickedChkPin(WORD /*wNotifyCode*/, WORD /*wID*/, HWND hWndCtl, BOOL& /*bHandled*/)
{
    //_ASSERTE(0);

    WriteDataFile(1, (BST_CHECKED == IsDlgButtonChecked(IDC_CHK_PIN))? 1 : 0);

    return 0;
}

void CAddinListViewDlg::InitListView()
{
    HIMAGELIST hImageList = ImageList_Create(16, 16, ILC_COLOR16, 3, 10);

    DWORD BITMAP_ID_ARRAY[] = { IDB_BMP_PAGE, IDB_BMP_SECTION, IDB_BMP_SECTIONGROUP, IDB_BMP_NOTEBOOK };

    for (int i=0; i<sizeof(BITMAP_ID_ARRAY)/sizeof(BITMAP_ID_ARRAY[0]); ++i)
    {
        HBITMAP hBitMap = ::LoadBitmap(g_hInst, MAKEINTRESOURCE(BITMAP_ID_ARRAY[i]));
        int iPos = ImageList_Add(hImageList, hBitMap, NULL);
        DeleteObject(hBitMap);
        if (iPos < 0) return;
    }

    HIMAGELIST hPrevImgList = ListView_SetImageList(m_hListView, hImageList, LVSIL_SMALL);
    //_ASSERTE(hPrevImgList); if (!hPrevImgList) return;

    // set the extend style
    DWORD dwStyle = ListView_GetExtendedListViewStyle(m_hListView);
    dwStyle |= LVS_EX_FULLROWSELECT | LVS_EX_ONECLICKACTIVATE | LVS_EX_TRACKSELECT | LVS_EX_UNDERLINEHOT;
    ListView_SetExtendedListViewStyle(m_hListView, dwStyle);

    // init the header of the list view
    LVCOLUMN lvc = {0};
    lvc.mask = LVCF_FMT | LVCF_WIDTH | LVCF_TEXT | LVCF_SUBITEM;
    lvc.fmt = LVCFMT_LEFT;
    lvc.cchTextMax = 64;

    lvc.cx = 60;
    lvc.pszText = _T("Type");
    lvc.iSubItem = 1;
    int iOpRet = ListView_InsertColumn(m_hListView, 1, &lvc);
    _ASSERTE(iOpRet >= 0); if (iOpRet < 0) return;

    lvc.cx = 250;
    lvc.pszText = _T("Favorite Title");
    lvc.iSubItem = 0;
    iOpRet = ListView_InsertColumn(m_hListView, 0, &lvc);
    _ASSERTE(iOpRet >= 0); if (iOpRet < 0) return;

    // init the list view items
    LoadListViewItems();
}

void CAddinListViewDlg::LoadListViewItems()
{
    LVITEM lvi = {0};
    lvi.cchTextMax = MAX_PATH;
    //_ASSERTE(0);
    for (int i=0; i<m_onFavList.GetFavoriteCount(); ++i)
    {
        CONFavorite* pONFav = m_onFavList.GetFavorite(i);

        lvi.mask = LVIF_TEXT | LVIF_IMAGE | LVIF_PARAM;
        lvi.pszText = pONFav->GetTitle().GetBuffer();
        lvi.iImage = pONFav->GetType();
        lvi.iItem = i;
        lvi.iSubItem = 0;
        lvi.lParam = (LPARAM) pONFav;
        lvi.iItem = ListView_InsertItem(m_hListView, &lvi);

        lvi.mask = LVIF_TEXT;
        lvi.iSubItem = 1;
        switch (lvi.iImage)
        {
        case 0: lvi.pszText = _T("Page"); break;
        case 1: lvi.pszText = _T("Section"); break;
        case 2: lvi.pszText = _T("SecGroup"); break;
        case 3: lvi.pszText = _T("Notebook"); break;
        }
        ListView_SetItem(m_hListView, &lvi);

        LOGGER << SV_DEBUG << lvi.iItem << L":" << lvi.iImage << _T(":") << pONFav->GetTitle() << _T("\t") << lvi.pszText << ENDL;
    }
}

void CAddinListViewDlg::CancelToolTip()
{
    KillTimer(m_hTimer);
    m_hTimer = 0;
    SendMessage(m_hToolTip, TTM_ACTIVATE, FALSE, 0);
}

LRESULT CAddinListViewDlg::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    CAxDialogImpl<CAddinListViewDlg>::OnInitDialog(uMsg, wParam, lParam, bHandled);
    bHandled = TRUE;

    WriteDataFile(0, 1);

    m_hListView = GetDlgItem(IDC_LIST_FAVORITES);
    m_hSwitchBtn = GetDlgItem(IDC_BTN_TREEVIEW);
    m_hChkPin = GetDlgItem(IDC_CHK_PIN);

    if (ONFAV_NO_FAVORITE == m_onFavList.Load())
    {
        //MessageBox(_T("There is no OneNote favorite in the OneNote Favorites Center!"),
        //    _T("OneNote Favorites Center"), MB_OK | MB_ICONEXCLAMATION);
        //m_pSwitcher->CloseListViewDlg();
        //return 1;
    }
    
    InitListView();
    CheckDlgButton(IDC_CHK_PIN, ReadDataFile(1)?BST_CHECKED:BST_UNCHECKED);
    ::SendMessage(m_hListView, WM_SETFOCUS, 0, 0);

    // init the icon
    SetIcon(LoadIcon(g_hInst, MAKEINTRESOURCE(IDI_ICON_TITLE)), FALSE);

    // the the minimum dialog size to the init size
    RECT rect;
    ::GetWindowRect(m_hWnd, &rect);
    m_minSize.x = rect.right - rect.left;
    m_minSize.y = rect.bottom - rect.top;
    ::GetClientRect(m_hListView, &rect);
    m_sizeListView.cx = rect.right;
    m_sizeListView.cy = rect.bottom;
    ::GetClientRect(m_hSwitchBtn, &rect);
    m_sizeSwitchBtn.cx = rect.right;
    m_sizeSwitchBtn.cy = rect.bottom;
    ::GetClientRect(m_hChkPin, &rect);
    m_sizeChkPin.cx = rect.right;
    m_sizeChkPin.cy = rect.bottom;

    // move the controls to the right positions
    OnSize(0, 0, 0, bHandled);

    // create tooltip window
    m_hToolTip = CreateWindowEx( 0, 
                                TOOLTIPS_CLASS, 
                                NULL, 
                                WS_POPUP | TTS_ALWAYSTIP, 
                                CW_USEDEFAULT, 
                                CW_USEDEFAULT, 
                                10, 
                                10, 
                                m_hListView, 
                                NULL, 
                                g_hInst, 
                                NULL);

    // add an empty tool to the tooltip
    m_toolInfo.cbSize = 44;
    m_toolInfo.uFlags = 0;
    m_toolInfo.uId = 38;
    m_toolInfo.hwnd = m_hListView;
    m_toolInfo.hinst  = NULL;
    m_toolInfo.lpszText = _T("Test");
    ::GetClientRect(m_hListView, &rect);
    m_toolInfo.rect = rect;
    BOOL fRet = (BOOL)SendMessage(m_hToolTip, TTM_ADDTOOL, 0, (LPARAM)&m_toolInfo);
    fRet = (BOOL)SendMessage(m_hToolTip, TTM_SETMAXTIPWIDTH, 0, 1024);
    _ASSERTE(fRet);

    return 1;  // Let the system set the focus
}

LRESULT CAddinListViewDlg::OnSize(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    //_ASSERTE(0);
    RECT rect;
    ::GetClientRect(m_hWnd, &rect);
    LONG lDlgWidth = rect.right;
    LONG lDlgHeight = rect.bottom;

    const LONG MARGIN = 6;
    const LONG GAP = 3;

    ListView_SetColumnWidth(m_hListView, 0, m_sizeListView.cx - 65);
    ::MoveWindow(
        m_hListView, 
        MARGIN,
        MARGIN, 
        lDlgWidth - 2 * MARGIN, 
        lDlgHeight - m_sizeSwitchBtn.cy - 2 * MARGIN - GAP,
        TRUE
        );
    ::MoveWindow(
        m_hChkPin, 
        MARGIN, 
        lDlgHeight - m_sizeSwitchBtn.cy - MARGIN, 
        m_sizeChkPin.cx, 
        m_sizeChkPin.cy,
        TRUE
        );
    ::MoveWindow(
        m_hSwitchBtn, 
        lDlgWidth - MARGIN - m_sizeSwitchBtn.cx, 
        lDlgHeight - m_sizeSwitchBtn.cy - MARGIN, 
        m_sizeSwitchBtn.cx, 
        m_sizeSwitchBtn.cy,
        TRUE
        );

    return 0;
}

LVITEM& CAddinListViewDlg::MakeLVItem(int iItem)
{
    ZeroMemory(&m_lvItem, sizeof(m_lvItem));
    ZeroMemory(m_tszText, sizeof(m_tszText));

    m_lvItem.mask = LVIF_TEXT | LVIF_PARAM;
    m_lvItem.iItem = iItem;
    m_lvItem.iSubItem = 0;
    m_lvItem.cchTextMax = sizeof(m_tszText)/sizeof(TCHAR);
    m_lvItem.pszText = m_tszText;

    return m_lvItem;
}

int CAddinListViewDlg::GetCurrentHitListItem(POINT* pCursorPos)
{
    LVHITTESTINFO lvhtInfo = {0};
    ::GetCursorPos(&lvhtInfo.pt);
    if (pCursorPos)
    {
        *pCursorPos = lvhtInfo.pt;
    }
    ::ScreenToClient(m_hListView, &lvhtInfo.pt);
    return ListView_HitTest(m_hListView, &lvhtInfo);
}

void CAddinListViewDlg::PopupContextMenu(int iItem, const POINT& pt)
{
    //_ASSERTE(0);
    m_iListItemForPopup = iItem;

    LVITEM lvItem = MakeLVItem(iItem);

    BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
    if (fRet && lvItem.lParam) // if favorite node
    {
        HMENU hMenu = ::LoadMenu(g_hInst, MAKEINTRESOURCE(IDR_MENU_CONTEXT));
        HMENU hSubMenu = ::GetSubMenu(hMenu, 0);

        // if the favorite id is empty, disable the "open in new window" menu item
        CONFavorite* pONFav = (CONFavorite*)lvItem.lParam;
        if (!pONFav->GetId().Compare(_T("")))
        {
            fRet = ::EnableMenuItem(hMenu, ID_POPUP_OPENINNEWWINDOW, MF_DISABLED | MF_GRAYED);
        }
        else
        {
            fRet = ::EnableMenuItem(hMenu, ID_POPUP_OPENINNEWWINDOW, MF_ENABLED);
        }

        CancelToolTip();

        // popup the menu
        fRet = TrackPopupMenu(hSubMenu, TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, 0, m_hWnd, NULL);
        DestroyMenu(hSubMenu);
        DestroyMenu(hMenu);
    }
}

void CAddinListViewDlg::LaunchListItem(int iItem, BOOL fNewWidnow)
{
    if (iItem < 0) return;
    //int iSelectedCount = ListView_GetSelectedCount(m_hListView);
    //if (iSelectedCount > 1) return;

    LVITEM lvItem = MakeLVItem(iItem);

    BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
    if (fRet && lvItem.lParam) // if favorite list item
    {
        CONFavorite* pONFav = (CONFavorite*)(lvItem.lParam);
        CComBSTR bstrId(pONFav->GetId());

        CComQIPtr<IApplication> ptrOneNoteApp;
        HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
        if (FAILED(hr)) 
        {
            m_pSwitcher->CloseListViewDlg();
            return;
        }
    
        if (0 == ::SysStringLen(bstrId))
        {
            hr = E_FAIL;
        }
        else
        {
            hr = ptrOneNoteApp->NavigateTo(bstrId, NULL, fNewWidnow || (BST_CHECKED == IsDlgButtonChecked(IDC_CHK_NEWWIN))? TRUE : FALSE);
        }

        // close current dialog by the settings
        if (!IsDlgButtonChecked(IDC_CHK_PIN))
        {
            m_pSwitcher->CloseListViewDlg();
        }

        if (FAILED(hr))
        {
            // using the shell URL
            INT_PTR iRet = (INT_PTR)::ShellExecute(NULL, _T("open"), pONFav->GetURI(), NULL, NULL, SW_SHOWNORMAL);
            if (iRet <= 32)
            {
                MessageBox(_T("Failed to navigate!"), _T("Navigate to OneNote Favorite"), MB_OK | MB_ICONERROR);
            }
            // remember the favorite navigated by URL
            g_lastFavNavigatedByURL = *pONFav;
        }
    }
}

LRESULT CAddinListViewDlg::OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    //_ASSERTE(0);

    if (!m_fEditing)
    {
        if (::GetFocus() == m_hListView)
        {
            LaunchListItem(ListView_GetSelectedItem(m_hListView));
        }
        else
        {
            m_pSwitcher->CloseListViewDlg();
        }
    }
    else
    {
        //ListView_CancelEditLabel(m_hListView);
    }

    bHandled = TRUE;

	return 0;
}

LRESULT CAddinListViewDlg::OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    if (!m_fEditing)
    {
	    m_pSwitcher->CloseListViewDlg();
    }
    else
    {
        ListView_CancelEditLabel(m_hListView);
    }

    bHandled = TRUE;

	return 0;
}

LRESULT CAddinListViewDlg::OnNMClickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    //_ASSERTE(0);
    NMITEMACTIVATE* pmia = reinterpret_cast<NMITEMACTIVATE*>(pNMHDR);
    if (LVKF_CONTROL & pmia->uKeyFlags)
    {
        // if press ctrl key, then do the multi selection
    }
    else if (!m_fEditing)
    {
        int iSelectedCount = ListView_GetSelectedCount(m_hListView);
        if (1 == iSelectedCount)
        {
            LaunchListItem(GetCurrentHitListItem());
        }
    }
    return 0;
}

LRESULT CAddinListViewDlg::OnNMRclickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    POINT pt;
    ::GetCursorPos(&pt);
    //int iItem = GetCurrentHitListItem(&pt);
    int iItem = ListView_GetSelectedItem(m_hListView);
    PopupContextMenu(iItem, pt);

    return 0;
}

LRESULT CAddinListViewDlg::OnLvnBeginlabeleditListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    // Limit text to 256 characters
    HWND hEdit = ListView_GetEditControl(m_hListView);
    ::SendMessage(hEdit, EM_SETLIMITTEXT, 256, 0);
    m_fEditing = true;

    return 0;
}

LRESULT CAddinListViewDlg::OnLvnEndlabeleditListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    //_ASSERTE(0);

    if (!m_fEditing) return 0;

    m_fEditing = false;

    int iItem = ListView_GetSelectedItem(m_hListView);

    LVITEM lvItem = MakeLVItem(iItem);

    BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
    if (fRet && lvItem.lParam)
    {
        HWND hEdit = ListView_GetEditControl(m_hListView);
        ::GetWindowText(hEdit, m_tszText, sizeof(m_tszText)/sizeof(TCHAR));

        CONFavorite* pONFav = (CONFavorite*)lvItem.lParam;
        pONFav->SetTitle(m_tszText);
        fRet = pONFav->Save(true);
    }

    return fRet? 1 : 0;
}

LRESULT CAddinListViewDlg::OnLvnKeydownListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    //_ASSERTE(0);
    LPNMTVKEYDOWN pTVKeyDown = reinterpret_cast<LPNMTVKEYDOWN>(pNMHDR);

    int iItem = ListView_GetSelectedItem(m_hListView);

    BOOL fRet = TRUE;
    bHandled = TRUE;
    if (VK_SPACE == pTVKeyDown->wVKey || VK_RETURN == pTVKeyDown->wVKey)
    {
        LaunchListItem(iItem);
    }
    else if (VK_ESCAPE == pTVKeyDown->wVKey)
    {
        OnClickedCancel(0, 0, 0, fRet);
    }
    else if (VK_F2 == pTVKeyDown->wVKey)
    {
        m_iListItemForPopup = iItem;
        OnPopupRename(0, 0, 0, fRet);
    }
    else if (VK_DELETE == pTVKeyDown->wVKey)
    {
        m_iListItemForPopup = iItem;
        OnPopupDelete(0, 0, 0, fRet);
    }
    else if (VK_APPS == pTVKeyDown->wVKey)
    {
        // get the position of TREEITEM
        POINT pt = {0};
        RECT rect;
        fRet = ListView_GetItemRect(m_hListView, iItem, &rect, FALSE);
        if (fRet)
        {
            pt.x = rect.left;
            pt.y = rect.top;
            ::ClientToScreen(m_hListView, &pt);
            pt.y += rect.bottom - rect.top;
        }
        else
        {
            pt.x = 100;
            pt.y = 100;
        }
        PopupContextMenu(iItem, pt);
    }
    else
    {
        bHandled = FALSE;
    }

    return 0;
}


LRESULT CAddinListViewDlg::OnPopupOpen(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LaunchListItem(m_iListItemForPopup);

    return 0;
}

LRESULT CAddinListViewDlg::OnPopupOpeninnewwindow(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LaunchListItem(m_iListItemForPopup, TRUE);

    return 0;
}

LRESULT CAddinListViewDlg::OnPopupRename(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    HWND hEdit = ListView_EditLabel(m_hListView, m_iListItemForPopup);

    return 0;
}

LRESULT CAddinListViewDlg::OnPopupDelete(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    CSimpleArray<int> arrItems = ListView_GetSelectedItems(m_hListView);

    int iDeletedCount = 0;
    for (int i=0; i<arrItems.GetSize(); ++i)
    {
        int iItem = arrItems[i] - iDeletedCount; // the item id will be changed after deleting some list items
        LVITEM lvItem = MakeLVItem(iItem);
        BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
        if (fRet && lvItem.lParam) // delete the favorite file
        {
            CONFavorite* pONFav = (CONFavorite*) lvItem.lParam;
            fRet = pONFav->Delete();
        }
        if (fRet) 
        {
            // delete the list item
            fRet = ListView_DeleteItem(m_hListView, iItem);
            if (fRet) iDeletedCount++;
        }
    }

    return 0;
}

LRESULT CAddinListViewDlg::OnPopupProperties(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LVITEM lvItem = MakeLVItem(m_iListItemForPopup);

    BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
    if (fRet && lvItem.lParam) // delete the favorite file
    {
        CONFavorite* pONFav = (CONFavorite*) lvItem.lParam;
        fRet = pONFav->OpenProperty();
    }

    return 0;
}

LRESULT CAddinListViewDlg::OnNMHoverListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    bHandled = TRUE;
    return 1;
}

LRESULT CAddinListViewDlg::OnLvnHotTrackListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    LVITEM lvItem;
    POINT ptCursorPos;

    int iItem = GetCurrentHitListItem(&ptCursorPos);
    //_ASSERTE(0);
    if (iItem < 0) 
    {
        // try to de-activate the tooltip
        SendMessage(m_hToolTip, TTM_ACTIVATE, FALSE, 0);
        goto Exit;
    }
    if (m_iPrevListItem == iItem) goto Exit;

    lvItem = MakeLVItem(iItem);
    BOOL fRet = ListView_GetItem(m_hListView, &lvItem);
    if (!fRet || !lvItem.lParam) 
    {
        // try to de-activate the tooltip
        SendMessage(m_hToolTip, TTM_ACTIVATE, FALSE, 0);
        goto Exit;
    }

    LOGGER << SV_DEBUG << _T("Hit list item favorite: ") << lvItem.pszText << ENDL; 

    RECT rect;
    fRet = ListView_GetItemRect(m_hListView, iItem, &rect, FALSE);
    if (!fRet) goto Exit;

    // update the tooltip's text
    CONFavorite* pONFav = (CONFavorite*)lvItem.lParam;
    m_toolInfo.lpszText = GetToolTipInfo(pONFav).GetBuffer();
    SendMessage(m_hToolTip, TTM_UPDATETIPTEXT, 0, (LPARAM)&m_toolInfo);

    // try to activate the tooltip
    SendMessage(m_hToolTip, TTM_ACTIVATE, TRUE, 0);

    // delay show the tooltip
    KillTimer(m_hTimer);
    m_hTimer = SetTimer(0, GetDoubleClickTime(), NULL);

Exit:
    m_iPrevListItem = iItem;
    return 0;
}

int CALLBACK CAddinListViewDlg::CompareFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort)
{
    CAddinListViewDlg* pDlg = (CAddinListViewDlg*)lParamSort;

    CONFavorite* pONFav1 = (CONFavorite*)lParam1;
    CONFavorite* pONFav2 = (CONFavorite*)lParam2;

    if (0 == pDlg->m_iSortedColumn)
    {
        return pDlg->m_fSortedAsc ?
            pONFav1->GetTitle().Compare(pONFav2->GetTitle()) :
            pONFav2->GetTitle().Compare(pONFav1->GetTitle());
    }
    else if (1 == pDlg->m_iSortedColumn)
    {
        return pDlg->m_fSortedAsc ?
            pONFav1->GetType() - pONFav2->GetType() :
            pONFav2->GetType() - pONFav1->GetType();
    }

    return 0;
}

LRESULT CAddinListViewDlg::OnLvnColumnclickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    LPNMLISTVIEW pNMLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);
    
    if (m_iSortedColumn != pNMLV->iSubItem)
    {
        m_fSortedAsc = true;
        m_iSortedColumn = pNMLV->iSubItem;
    }
    else
    {
        m_fSortedAsc = !m_fSortedAsc;
    }

    BOOL fRet = ListView_SortItems(m_hListView, CompareFunc, (LPARAM)this);

    return 0;
}
