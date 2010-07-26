// AddinDlg.cpp : Implementation of CAddinDlg

#include "stdafx.h"
#include "AddinDlg.h"
#include "onenote12.h"
#include <windowsx.h>

extern CONFavorite g_lastFavNavigatedByURL;

// CAddinDlg

TVINSERTSTRUCT& CAddinDlg::MakeTVInsertItem()
{
    ZeroMemory(&m_tvis, sizeof(m_tvis));
    ZeroMemory(m_tszText, sizeof(m_tszText));

    m_tvis.hInsertAfter = TVI_ROOT;
    m_tvis.item.mask = TVIF_TEXT|TVIF_IMAGE|TVIF_SELECTEDIMAGE|TVIF_PARAM|TVIF_HANDLE;
    m_tvis.item.pszText = m_tszText;
    m_tvis.item.cchTextMax = sizeof(m_tszText)/sizeof(TCHAR);
    m_tvis.item.iImage = 0;
    m_tvis.item.iSelectedImage = m_tvis.item.iImage;
    m_tvis.item.lParam = NULL;

    return m_tvis;
}

TVITEM& CAddinDlg::MakeTVItem(HTREEITEM hTreeItem)
{
    ZeroMemory(&m_tvItem, sizeof(m_tvItem));
    ZeroMemory(m_tszText, sizeof(m_tszText));

    m_tvItem.mask = TVIF_TEXT | TVIF_PARAM | TVIF_HANDLE;
    m_tvItem.hItem = hTreeItem;
    m_tvItem.cchTextMax = sizeof(m_tszText)/sizeof(TCHAR);
    m_tvItem.pszText = m_tszText;

    return m_tvItem;
}

void CAddinDlg::MakeUpTree(HWND hTree)
{
    HIMAGELIST hImageList = ImageList_Create(16, 16, ILC_COLOR16, 3, 10);

    DWORD NODETYPE_ARRAY[] = { NODE_PAGE, NODE_SECTION, NODE_SECTION_GROUP, NODE_NOTEBOOK };
    DWORD BITMAP_ID_ARRAY[] = { IDB_BMP_PAGE, IDB_BMP_SECTION, IDB_BMP_SECTIONGROUP, IDB_BMP_NOTEBOOK };

    for (int i=0; i<sizeof(NODETYPE_ARRAY)/sizeof(NODETYPE_ARRAY[0]); ++i)
    {
        HBITMAP hBitMap = ::LoadBitmap(g_hInst, MAKEINTRESOURCE(BITMAP_ID_ARRAY[i]));
        int iPos = ImageList_Add(hImageList, hBitMap, NULL);
        DeleteObject(hBitMap);
        if (iPos < 0) return;
        m_arrImgIdByType[NODETYPE_ARRAY[i]] = iPos;
    }

    SendMessage(hTree, TVM_SETIMAGELIST, 0, (LPARAM)hImageList);
}

void CAddinDlg::InitTree()
{
    //_ASSERTE(0);
    MakeUpTree(m_hTree);

    RecursiveLoadTree(m_hTree, NULL, m_onFavList.GetTree());

    if (m_onFavList.GetWildFavoriteCount() > 0)
    {
        // Add the wild favorites into the tree
        TVINSERTSTRUCT tvis = MakeTVInsertItem();
        tvis.item.pszText = _T("Wild Favorites");
        tvis.item.iImage = tvis.item.iSelectedImage = m_arrImgIdByType[NODE_NOTEBOOK];

        HTREEITEM hItem = TreeView_InsertItem(m_hTree, &tvis);

        for (int i=0; i<m_onFavList.GetWildFavoriteCount(); ++i)
        {
            CONFavorite* pWildFav = m_onFavList.GetWildFavorite(i);

            tvis = MakeTVInsertItem();
            tvis.hParent = hItem;
            tvis.item.pszText = pWildFav->GetTitle().GetBuffer();
            NodeType nodeType;
            switch (pWildFav->GetType())
            {
            case ONFT_PAGE: nodeType = NODE_PAGE; break;
            case ONFT_SECTION: nodeType = NODE_SECTION; break;
            case ONFT_SECTION_GROUP: nodeType = NODE_SECTION_GROUP; break;
            case ONFT_NOTEBOOK: nodeType = NODE_NOTEBOOK; break;
            }
            tvis.item.iImage = tvis.item.iSelectedImage = m_arrImgIdByType[nodeType];
            tvis.item.lParam = (LPARAM) pWildFav;

            HTREEITEM hItem = TreeView_InsertItem(m_hTree, &tvis);
        }
    }
}

void CAddinDlg::RecursiveLoadTree(HWND hTree, HTREEITEM hParent, CTreeNode* pTreeNode)
{
    for (int i=0; i<pTreeNode->children.GetSize(); ++i)
    {
        CTreeNode* pSubTreeNode = pTreeNode->children[i];

        TVINSERTSTRUCT tvis = MakeTVInsertItem();
        tvis.hParent = hParent;
        tvis.item.pszText = pSubTreeNode->strName.GetBuffer();
        tvis.item.iImage = tvis.item.iSelectedImage = m_arrImgIdByType[pSubTreeNode->nodeType];
        CLeafNode* pLeafNode = dynamic_cast<CLeafNode*> (pSubTreeNode);
        tvis.item.lParam = (pLeafNode != NULL) ? (LPARAM)(pLeafNode->lpONFavorite) : NULL;

        HTREEITEM hItem = TreeView_InsertItem(hTree, &tvis);

        RecursiveLoadTree(hTree, hItem, pSubTreeNode);
    }
}

void CAddinDlg::InitComboBox(OneNoteFavoriteType type)
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

    COMBOBOXEXITEM cbei;
    cbei.iItem = -1;
    cbei.mask = CBEIF_TEXT | CBEIF_IMAGE | CBEIF_SELECTEDIMAGE | CBEIF_LPARAM;
    cbei.cchTextMax = 64;

    int i = 0;
    int iIndex = 0;
    BYTE bExist = m_onFavList.GetExistStatus();

    if (bExist & (0x01))
    {
        cbei.pszText = _T("Page");
        cbei.iImage = cbei.iSelectedImage = 0;
        cbei.lParam = 0;
        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_INSERTITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);
        if (ONFT_PAGE == type) iIndex = i;
        i++;
    }
    if (bExist & (0x02))
    {
        cbei.pszText = _T("Section");
        cbei.iImage = cbei.iSelectedImage = 1;
        cbei.lParam = 1;
        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_INSERTITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);
        if (ONFT_SECTION == type) iIndex = i;
        i++;
    }
    if (bExist & (0x04))
    {
        cbei.pszText = _T("SecGroup");
        cbei.iImage = cbei.iSelectedImage = 2;
        cbei.lParam = 2;
        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_INSERTITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);
        if (ONFT_SECTION_GROUP == type) iIndex = i;
        i++;
    }
    if (bExist & (0x08))
    {
        cbei.pszText = _T("Notebook");
        cbei.iImage = cbei.iSelectedImage = 3;
        cbei.lParam = 3;
        LRESULT ret = ::SendMessage(m_hComboTypes, CBEM_INSERTITEM, 0, (LPARAM)&cbei);
        _ASSERTE(ret >= 0);
        if (ONFT_NOTEBOOK == type) iIndex = i;
        i++;
    }
    
    ComboBox_SetCurSel(m_hComboTypes, iIndex);
}

LRESULT CAddinDlg::OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    CAxDialogImpl<CAddinDlg>::OnInitDialog(uMsg, wParam, lParam, bHandled);
    bHandled = TRUE;

    WriteDataFile(0, 0);

    m_hTree = GetDlgItem(IDC_TREE_FAVORITES);
    m_hChkExpand = GetDlgItem(IDC_CHK_EXPAND);
    m_hChkPin = GetDlgItem(IDC_CHK_PIN);
    m_hChkNewWin = GetDlgItem(IDC_CHK_NEWWIN);
    m_hSwitchBtn = GetDlgItem(IDC_BTN_LISTVIEW);
    m_hComboTypes = GetDlgItem(IDC_COMBO_TYPE);

    //_ASSERTE(0);
    if (ONFAV_NO_FAVORITE == m_onFavList.Load())
    {
        //MessageBox(_T("There is no OneNote favorite in the OneNote Favorites Center!"), _T("OneNote Favorites Center"), MB_OK | MB_ICONEXCLAMATION);
        //m_pSwitcher->CloseTreeViewDlg();
        //return 1;
    }
    OneNoteFavoriteType type = (OneNoteFavoriteType) ReadDataFile(3);
    m_onFavList.BuildTree(type);

    InitTree();

    InitComboBox(type);
    
    CheckDlgButton(IDC_CHK_PIN, ReadDataFile(1)?BST_CHECKED:BST_UNCHECKED);
    CheckDlgButton(IDC_CHK_EXPAND, ReadDataFile(2)?BST_CHECKED:BST_UNCHECKED);
    ::SendMessage(m_hTree, WM_SETFOCUS, 0, 0);
    OnBnClickedChkExpand(0, 0, 0, bHandled);

    // the the minimum dialog size to the init size, init the controls' sizes
    RECT rect;
    ::GetWindowRect(m_hWnd, &rect);
    m_minSize.x = rect.right - rect.left;
    m_minSize.y = rect.bottom - rect.top;
    ::GetClientRect(m_hTree, &rect);
    m_sizeTreeView.cx = rect.right;
    m_sizeTreeView.cy = rect.bottom;
    ::GetClientRect(m_hSwitchBtn, &rect);
    m_sizeSwitchBtn.cx = rect.right;
    m_sizeSwitchBtn.cy = rect.bottom;
    ::GetClientRect(m_hChkExpand, &rect);
    m_sizeChkExpand.cx = rect.right;
    m_sizeChkExpand.cy = rect.bottom;
    ::GetClientRect(m_hChkPin, &rect);
    m_sizeChkPin.cx = rect.right;
    m_sizeChkPin.cy = rect.bottom;
    ::GetClientRect(m_hComboTypes, &rect);
    m_sizeComboTypes.cx = rect.right;
    m_sizeComboTypes.cy = rect.bottom;

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
                                m_hTree, 
                                NULL, 
                                g_hInst, 
                                NULL);

    // add an empty tool to the tooltip
    m_toolInfo.cbSize = 44;
    m_toolInfo.uFlags = 0;
    m_toolInfo.uId = 16;
    m_toolInfo.hwnd = m_hTree;
    m_toolInfo.hinst  = NULL;
    m_toolInfo.lpszText = NULL;
    ::GetClientRect(m_hTree, &rect);
    m_toolInfo.rect = rect;
    BOOL fRet = (BOOL)SendMessage(m_hToolTip, TTM_ADDTOOL, 0, (LPARAM)&m_toolInfo);
    fRet = (BOOL)SendMessage(m_hToolTip, TTM_SETMAXTIPWIDTH, 0, 1024);
    _ASSERTE(fRet);

    // init the icon
    SetIcon(LoadIcon(g_hInst, MAKEINTRESOURCE(IDI_ICON_TITLE)), FALSE);

    return 1;  // Let the system set the focus
}

LRESULT CAddinDlg::OnNMCursorTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    LPNMMOUSE lpMouse = reinterpret_cast<LPNMMOUSE>(pNMHDR);

    CString strInfo;
    TVITEM tvItem;

    HTREEITEM hItem = GetCurrentHitTreeItem();
    if (!hItem) 
    {
        // try to de-activate the tooltip
        SendMessage(m_hToolTip, TTM_ACTIVATE, FALSE, 0);
        goto Exit;
    }
    if (m_hPrevTreeItem == hItem) goto Exit;

    tvItem = MakeTVItem(hItem);
    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (!fRet || !tvItem.lParam) 
    {
        // try to de-activate the tooltip
        SendMessage(m_hToolTip, TTM_ACTIVATE, FALSE, 0);
        goto Exit;
    }

    LOGGER << SV_DEBUG << _T("Hit tree item favorite: ") << tvItem.pszText << ENDL;

    RECT rect;
    fRet = TreeView_GetItemRect(m_hTree, hItem, &rect, FALSE);
    if (!fRet) goto Exit;

    // update the tooltip's text
    CONFavorite* pONFav = (CONFavorite*)tvItem.lParam;
    m_toolInfo.lpszText = GetToolTipInfo(pONFav).GetBuffer();
    fRet = (BOOL)SendMessage(m_hToolTip, TTM_UPDATETIPTEXT, 0, (LPARAM)&m_toolInfo);

    // try to activate the tooltip
    SendMessage(m_hToolTip, TTM_ACTIVATE, TRUE, 0);

    // delay show the tooltip
    KillTimer(m_hTimer);
    m_hTimer = SetTimer(0, GetDoubleClickTime(), NULL);

Exit:
    m_hPrevTreeItem = hItem;
    return 0;
}

LRESULT CAddinDlg::OnSize(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    //_ASSERTE(0);
    RECT rect;
    ::GetClientRect(m_hWnd, &rect);
    LONG lDlgWidth = rect.right;
    LONG lDlgHeight = rect.bottom;

    const LONG MARGIN = 6;
    const LONG GAP = 3, GAP2 = 1;

    ::MoveWindow(
        m_hTree, 
        MARGIN,
        MARGIN, 
        lDlgWidth - 2 * MARGIN, 
        lDlgHeight - m_sizeSwitchBtn.cy - 2 * MARGIN - GAP,
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
    ::MoveWindow(
        m_hChkPin, 
        MARGIN, 
        lDlgHeight - m_sizeSwitchBtn.cy - MARGIN, 
        m_sizeChkPin.cx, 
        m_sizeChkPin.cy,
        TRUE
        );
    ::MoveWindow(
        m_hChkExpand, 
        MARGIN + m_sizeChkPin.cx + GAP2,
        lDlgHeight - m_sizeSwitchBtn.cy - MARGIN, 
        m_sizeChkExpand.cx, 
        m_sizeChkExpand.cy,
        TRUE
        );
    ::MoveWindow(
        m_hComboTypes, 
        MARGIN + m_sizeChkPin.cx + m_sizeChkExpand.cx + GAP2 + GAP2,
        lDlgHeight - m_sizeSwitchBtn.cy - MARGIN + 2, 
        m_sizeComboTypes.cx, 
        m_sizeComboTypes.cy,
        TRUE
        );

    return 0;
}

LRESULT CAddinDlg::OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    //_ASSERTE(0);

    if (!m_fEditing)
    {
        if (::GetFocus() == m_hTree)
        {
            LaunchTreeItem(TreeView_GetSelection(m_hTree));
        }
        else
        {
            m_pSwitcher->CloseTreeViewDlg();
        }
    }
    else
    {
        TreeView_EndEditLabelNow(m_hTree, FALSE);
    }

    bHandled = TRUE;

    return 0;
}

LRESULT CAddinDlg::OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    //_ASSERTE(0);
    if (!m_fEditing)
    {
        m_pSwitcher->CloseTreeViewDlg();
    }
    else
    {
        TreeView_EndEditLabelNow(m_hTree, TRUE);
    }

    bHandled = TRUE;

    return 0;
}

LRESULT CAddinDlg::OnNMClickTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    //_ASSERTE(0);
    if (!m_fEditing)
    {
        HTREEITEM hItem = GetCurrentHitTreeItem();
        TreeView_SelectItem(m_hTree, hItem);
        LaunchTreeItem(hItem);
        bHandled = TRUE;
    }
    return 0;
}

LRESULT CAddinDlg::OnNMRclickTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    POINT pt;
    HTREEITEM hItem = GetCurrentHitTreeItem(&pt);
    TreeView_SelectItem(m_hTree, hItem);
    PopupContextMenu(hItem, pt);
    bHandled = TRUE;

    return 0;
}

LRESULT CAddinDlg::OnTvnKeydownTreeFavorites(int idCtrl, LPNMHDR pNMHDR, BOOL& bHandled)
{
    //_ASSERTE(0);
    LPNMTVKEYDOWN pTVKeyDown = reinterpret_cast<LPNMTVKEYDOWN>(pNMHDR);

    HTREEITEM hTreeItem = TreeView_GetSelection(m_hTree);

    BOOL fRet = TRUE;
    bHandled = TRUE;
    if (VK_SPACE == pTVKeyDown->wVKey || VK_RETURN == pTVKeyDown->wVKey)
    {
        LaunchTreeItem(hTreeItem);
    }
    else if (VK_ESCAPE == pTVKeyDown->wVKey)
    {
        OnClickedCancel(0, 0, 0, fRet);
    }
    else if (VK_F2 == pTVKeyDown->wVKey)
    {
        m_hTreeItemForPopup = hTreeItem;
        OnPopupRename(0, 0, 0, fRet);
    }
    else if (VK_DELETE == pTVKeyDown->wVKey)
    {
        m_hTreeItemForPopup = hTreeItem;
        OnPopupDelete(0, 0, 0, fRet);
    }
    else if (VK_APPS == pTVKeyDown->wVKey)
    {
        // get the position of TREEITEM
        POINT pt = {0};
        RECT rect;
        fRet = TreeView_GetItemRect(m_hTree, hTreeItem, &rect, FALSE);
        if (fRet)
        {
            pt.x = rect.left;
            pt.y = rect.top;
            ::ClientToScreen(m_hTree, &pt);
            pt.y += rect.bottom - rect.top;
        }
        else
        {
            pt.x = 100;
            pt.y = 100;
        }
        PopupContextMenu(hTreeItem, pt);
    }
    else
    {
        bHandled = FALSE;
    }

    return 0;
}

void CAddinDlg::RecursiveExpandTree(HTREEITEM hItem, UINT uiCode)
{
    TreeView_Expand(m_hTree, hItem, uiCode);
    hItem = TreeView_GetChild(m_hTree, hItem);
    while (hItem)
    {
        RecursiveExpandTree(hItem, uiCode);
        hItem = TreeView_GetNextSibling(m_hTree, hItem);
    }
}

LRESULT CAddinDlg::OnBnClickedChkExpand(WORD /*wNotifyCode*/, WORD /*wID*/, HWND hWndCtl, BOOL& bHandled)
{
    //_ASSERTE(0);

    UINT uiCode = (BST_CHECKED == IsDlgButtonChecked(IDC_CHK_EXPAND))? TVE_EXPAND : TVE_COLLAPSE;

    HTREEITEM hItemRoot = TreeView_GetRoot(m_hTree);
    HTREEITEM hItem = hItemRoot;
    while (hItem)
    {
        RecursiveExpandTree(hItem, uiCode);
        hItem = TreeView_GetNextSibling(m_hTree, hItem);
    }
    TreeView_SelectItem(m_hTree, hItemRoot);
    TreeView_EnsureVisible(m_hTree, hItemRoot);
    ::SendMessage(m_hTree, WM_SETFOCUS, 0, 0);

    WriteDataFile(2, (BST_CHECKED == IsDlgButtonChecked(IDC_CHK_EXPAND))? 1 : 0);

    return 0;
}

LRESULT CAddinDlg::OnBnClickedChkPin(WORD /*wNotifyCode*/, WORD /*wID*/, HWND hWndCtl, BOOL& /*bHandled*/)
{
    //_ASSERTE(0);

    WriteDataFile(1, (BST_CHECKED == IsDlgButtonChecked(IDC_CHK_PIN))? 1 : 0);

    return 0;
}

void CAddinDlg::PopupContextMenu(HTREEITEM hTreeItem, const POINT& pt)
{
    //_ASSERTE(0);
    m_hTreeItemForPopup = hTreeItem;

    TVITEM tvItem = MakeTVItem(hTreeItem);

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam) // if favorite node
    {
        HMENU hMenu = ::LoadMenu(g_hInst, MAKEINTRESOURCE(IDR_MENU_CONTEXT));
        HMENU hSubMenu = ::GetSubMenu(hMenu, 0);

        // if the favorite id is empty, disable the "open in new window" menu item
        CONFavorite* pONFav = (CONFavorite*)tvItem.lParam;
        if (!pONFav->GetId().Compare(_T("")))
        {
            fRet = ::EnableMenuItem(hMenu, ID_POPUP_OPENINNEWWINDOW, MF_DISABLED | MF_GRAYED);
        }
        else
        {
            fRet = ::EnableMenuItem(hMenu, ID_POPUP_OPENINNEWWINDOW, MF_ENABLED);
        }

        // popup the menu
        fRet = TrackPopupMenu(hSubMenu, TPM_LEFTALIGN | TPM_RIGHTBUTTON, pt.x, pt.y, 0, m_hWnd, NULL);
        DestroyMenu(hSubMenu);
        DestroyMenu(hMenu);
    }
}

void CAddinDlg::LaunchTreeItem(HTREEITEM hTreeItem, BOOL fNewWidnow)
{
    TVITEM tvItem = MakeTVItem(hTreeItem);

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam) // if favorite tree item
    {
        CONFavorite onFav = *(CONFavorite*)(tvItem.lParam);
        CComBSTR bstrId(onFav.GetId());

        CComQIPtr<IApplication> ptrOneNoteApp;
        HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
        if (FAILED(hr)) 
        {
            m_pSwitcher->CloseTreeViewDlg();
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

        if (FAILED(hr))
        {
            // using the shell URL
            INT_PTR iRet = (INT_PTR)::ShellExecute(NULL, _T("open"), onFav.GetURI(), NULL, NULL, SW_SHOWNORMAL);
            if (iRet <= 32)
            {
                MessageBox(_T("Failed to navigate!"), _T("Navigate to OneNote Favorite"), MB_OK | MB_ICONERROR);
            }
            // remember the favorite navigated by URL
            g_lastFavNavigatedByURL = onFav;
        }

        // close current dialog by the settings
        if (!IsDlgButtonChecked(IDC_CHK_PIN))
        {
            m_pSwitcher->CloseTreeViewDlg();
        }
    }
}

HTREEITEM CAddinDlg::GetCurrentHitTreeItem(POINT* pCursorPos)
{
    TVHITTESTINFO tvhtInfo = {0};
    ::GetCursorPos(&tvhtInfo.pt);
    if (pCursorPos)
    {
        *pCursorPos = tvhtInfo.pt;
    }
    ::ScreenToClient(m_hTree, &tvhtInfo.pt);
    HTREEITEM hItem = TreeView_HitTest(m_hTree, &tvhtInfo);
    return hItem;
}

LRESULT CAddinDlg::OnPopupOpen(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LaunchTreeItem(m_hTreeItemForPopup);

    return 0;
}

LRESULT CAddinDlg::OnPopupOpeninnewwindow(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    LaunchTreeItem(m_hTreeItemForPopup, TRUE);

    return 0;
}

LRESULT CAddinDlg::OnPopupRename(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    HWND hEdit = TreeView_EditLabel(m_hTree, m_hTreeItemForPopup);

    return 0;
}

LRESULT CAddinDlg::OnPopupDelete(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    TVITEM tvItem = MakeTVItem(m_hTreeItemForPopup);

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam) // delete the favorite file
    {
        CONFavorite* pONFav = (CONFavorite*) tvItem.lParam;
        fRet = pONFav->Delete();
    }

    if (!fRet) return 0;

    // delete the tree item (and its parents if necessary) in the tree control
    HTREEITEM hItem = m_hTreeItemForPopup;
    while (hItem)
    {
        HTREEITEM hParentItem = TreeView_GetParent(m_hTree, hItem);
        fRet = TreeView_DeleteItem(m_hTree, hItem);
        if (!fRet) break;
        hItem = hParentItem;
        if (TreeView_GetChild(m_hTree, hItem)) // if the parent item has no more child
        {
            break;
        }
    }

    return 0;
}

LRESULT CAddinDlg::OnPopupProperties(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    TVITEM tvItem = MakeTVItem(m_hTreeItemForPopup);

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam) // delete the favorite file
    {
        CONFavorite* pONFav = (CONFavorite*) tvItem.lParam;
        fRet = pONFav->OpenProperty();
    }

    return 0;
}

LRESULT CAddinDlg::OnTvnBeginlabeleditTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/)
{
    HTREEITEM hTreeItem = TreeView_GetSelection(m_hTree);

    TVITEM tvItem = MakeTVItem(hTreeItem);

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam) // if favorite
    {
        // Limit text to 256 characters
        HWND hEdit = TreeView_GetEditControl(m_hTree);
        ::SendMessage(hEdit, EM_SETLIMITTEXT, 256, 0);
        m_fEditing = true;
    }
    else // prevent editing the section, section group, notebook
    {
        TreeView_EndEditLabelNow(m_hTree, TRUE);
        TreeView_SelectItem(m_hTree, hTreeItem);
    }

    return 0;
}

LRESULT CAddinDlg::OnTvnEndlabeleditTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& bHandled)
{
    //_ASSERTE(0);
    // return if editing a folder (instead of a favorite)
    if (!m_fEditing) return 0;

    m_fEditing = false;

    TVITEM tvItem = MakeTVItem(TreeView_GetSelection(m_hTree));

    BOOL fRet = TreeView_GetItem(m_hTree, &tvItem);
    if (fRet && tvItem.lParam)
    {
        HWND hEdit = TreeView_GetEditControl(m_hTree);
        ::GetWindowText(hEdit, m_tszText, sizeof(m_tszText)/sizeof(TCHAR));

        CONFavorite* pONFav = (CONFavorite*)tvItem.lParam;
        pONFav->SetTitle(m_tszText);
        fRet = pONFav->Save(true);
    }

    return fRet? 1 : 0;
}

LRESULT CAddinDlg::OnCbnSelchangeComboType(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    //_ASSERTE(0);
    LRESULT ret = ::SendMessage(m_hComboTypes, CB_GETCURSEL, 0, 0);
    _ASSERTE(ret != CB_ERR);

    COMBOBOXEXITEM cbei;
    cbei.iItem = ret;
    cbei.mask = CBEIF_LPARAM;
    ret = ::SendMessage(m_hComboTypes, CBEM_GETITEM, 0, (LPARAM)&cbei);
    _ASSERTE(ret >= 0);

    m_onFavList.ResetTree();
    m_onFavList.BuildTree((OneNoteFavoriteType) cbei.lParam);
    TreeView_DeleteAllItems(m_hTree);
    InitTree();

    OnBnClickedChkExpand(wNotifyCode, wID, hWndCtl, bHandled);

    // save this into the file
    WriteDataFile(3, cbei.lParam);

    return 0;
}

LRESULT CAddinDlg::OnBnClickedBtnListview(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/)
{
    m_pSwitcher->ShowListViewDlg();

    return 0;
}
