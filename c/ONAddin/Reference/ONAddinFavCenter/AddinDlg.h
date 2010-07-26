// AddinDlg.h : Declaration of the CAddinDlg

#pragma once

#include "resource.h"       // main symbols
#include <atlhost.h>
#include "ONFavoriteList.h"
#include "common.h"
#include "DlgSwitcher.h"

// CAddinDlg

class CAddinDlg : public CAxDialogImpl<CAddinDlg>
{
private:
    // for data mapping
    CONFavoriteList m_onFavList;

    // for images of the treeview
    int m_arrImgIdByType[10];

    // store the HWND of the treeview
    HWND m_hTree;

    // for resizing
    POINT m_minSize;
    HWND m_hChkExpand;
    HWND m_hChkPin;
    HWND m_hChkNewWin;
    HWND m_hSwitchBtn;
    HWND m_hComboTypes;
    SIZE m_sizeTreeView;
    SIZE m_sizeSwitchBtn;
    SIZE m_sizeChkExpand;
    SIZE m_sizeChkPin;
    SIZE m_sizeComboTypes;

    // for tooltips
    HWND m_hToolTip;
    TOOLINFO m_toolInfo;
    HTREEITEM m_hPrevTreeItem;
    UINT_PTR m_hTimer;

    // for popup menu
    HTREEITEM m_hTreeItemForPopup;

    // for editing
    bool m_fEditing;

    // for MakeTVInsertItem & MakeTVItem
    TCHAR m_tszText[ MAX_PATH ]; // also used for editing
    TVINSERTSTRUCT m_tvis;
    TVITEM m_tvItem;

    CDlgSwitcher* m_pSwitcher;

public:
    CAddinDlg(CDlgSwitcher* pSwitcher) : m_hTreeItemForPopup(0), m_fEditing(false), 
        m_hTree(0), m_hChkExpand(0), m_hChkPin(0), m_hChkNewWin(0), m_hSwitchBtn(0), m_hToolTip(0), m_hPrevTreeItem(0),
        m_hTimer(0), m_pSwitcher(pSwitcher)
    {
    }

    ~CAddinDlg()
    {
    }

    enum { IDD = IDD_ADDINDLG };

BEGIN_MSG_MAP(CAddinDlg)
    MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
    MESSAGE_HANDLER(WM_DESTROY, OnDestroy)
    MESSAGE_HANDLER(WM_SIZE, OnSize)
    MESSAGE_HANDLER(WM_TIMER, OnTimer)
    MESSAGE_HANDLER(WM_GETMINMAXINFO, OnGetMinMaxInfo)
    /////////////////////////
    MESSAGE_HANDLER(WM_KEYDOWN, OnDestroy)
    /////////////////////////

    COMMAND_HANDLER(IDOK, BN_CLICKED, OnClickedOK)
    COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnClickedCancel)
    COMMAND_HANDLER(IDC_CHK_EXPAND, BN_CLICKED, OnBnClickedChkExpand)
    COMMAND_HANDLER(IDC_CHK_PIN, BN_CLICKED, OnBnClickedChkPin)
    COMMAND_HANDLER(IDC_COMBO_TYPE, CBN_SELCHANGE, OnCbnSelchangeComboType)
    COMMAND_HANDLER(IDC_BTN_LISTVIEW, BN_CLICKED, OnBnClickedBtnListview)

    NOTIFY_HANDLER(IDC_TREE_FAVORITES, NM_CLICK, OnNMClickTreeFavorites)
    NOTIFY_HANDLER(IDC_TREE_FAVORITES, NM_RCLICK, OnNMRclickTreeFavorites)
    NOTIFY_HANDLER(IDC_TREE_FAVORITES, NM_SETCURSOR, OnNMCursorTreeFavorites)
    NOTIFY_HANDLER(IDC_TREE_FAVORITES, TVN_BEGINLABELEDIT, OnTvnBeginlabeleditTreeFavorites)
    NOTIFY_HANDLER(IDC_TREE_FAVORITES, TVN_ENDLABELEDIT, OnTvnEndlabeleditTreeFavorites)
    NOTIFY_HANDLER(IDC_TREE_FAVORITES, TVN_KEYDOWN, OnTvnKeydownTreeFavorites)

    COMMAND_ID_HANDLER(ID_POPUP_OPEN, OnPopupOpen)
    COMMAND_ID_HANDLER(ID_POPUP_OPENINNEWWINDOW, OnPopupOpeninnewwindow)
    COMMAND_ID_HANDLER(ID_POPUP_RENAME, OnPopupRename)
    COMMAND_ID_HANDLER(ID_POPUP_DELETE, OnPopupDelete)
    COMMAND_ID_HANDLER(ID_POPUP_PROPERTIES, OnPopupProperties)

    CHAIN_MSG_MAP(CAxDialogImpl<CAddinDlg>)
END_MSG_MAP()

    LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    LRESULT OnSize(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    LRESULT OnDestroy(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
    {
        if (m_hToolTip)
        {
            ::DestroyWindow(m_hToolTip);
        }
        m_hToolTip = NULL;
        KillTimer(m_hTimer);
        m_hTimer = 0;

        m_onFavList.Destroy();

        return 0;
    }
    LRESULT OnTimer(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
    {
        SendMessage(m_hToolTip, TTM_TRACKACTIVATE, TRUE, (LPARAM)&m_toolInfo);
        return 0;
    }
    LRESULT OnGetMinMaxInfo(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
    {
        MINMAXINFO* pInfo = (MINMAXINFO*)lParam;
        pInfo->ptMinTrackSize = m_minSize;
        return 0;
    }

    LRESULT OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    LRESULT OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);

    LRESULT OnBnClickedChkExpand(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnBnClickedChkPin(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnNMClickTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnTvnKeydownTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnNMRclickTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnNMCursorTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnTvnBeginlabeleditTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnTvnEndlabeleditTreeFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnCbnSelchangeComboType(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnBnClickedBtnListview(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);

    LRESULT OnPopupOpen(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupOpeninnewwindow(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupRename(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupDelete(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupProperties(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);

private:
    void InitTree();
    void InitComboBox(OneNoteFavoriteType type);
    void RecursiveLoadTree(HWND hTree, HTREEITEM hParent, CTreeNode* pTreeNode);
    void MakeUpTree(HWND hTree);
    void LaunchTreeItem(HTREEITEM hTreeItem, BOOL fNewWidnow = FALSE);
    HTREEITEM GetCurrentHitTreeItem(POINT* pCursorPos = NULL);
    void PopupContextMenu(HTREEITEM hTreeItem, const POINT& pt);
    TVINSERTSTRUCT& MakeTVInsertItem();
    TVITEM& MakeTVItem(HTREEITEM hTreeItem);
    void RecursiveExpandTree(HTREEITEM hItem, UINT uiCode);
};
