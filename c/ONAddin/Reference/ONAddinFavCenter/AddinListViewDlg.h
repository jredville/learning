// AddinListViewDlg.h : Declaration of the CAddinListViewDlg

#pragma once

#include "resource.h"       // main symbols
#include <atlhost.h>

#include "ONFavoriteList.h"
#include "common.h"
#include "DlgSwitcher.h"

// CAddinListViewDlg

class CAddinListViewDlg : public CAxDialogImpl<CAddinListViewDlg>
{
private:
    HWND m_hListView;

    // for data mapping
    CONFavoriteList m_onFavList;

    // for resizing
    POINT m_minSize;
    HWND m_hSwitchBtn;
    HWND m_hChkPin;
    SIZE m_sizeListView;
    SIZE m_sizeSwitchBtn;
    SIZE m_sizeChkPin;

    int m_iListItemForPopup;

    // for MakeTVInsertItem & MakeTVItem
    TCHAR m_tszText[ MAX_PATH ]; // also used for editing
    LVITEM m_lvItem;

    // for editing
    bool m_fEditing;

    // for tooltips
    HWND m_hToolTip;
    TOOLINFO m_toolInfo;
    int m_iPrevListItem;
    UINT_PTR m_hTimer;

    // for sorting
    int m_iSortedColumn;
    bool m_fSortedAsc;

    CDlgSwitcher* m_pSwitcher;

public:
    CAddinListViewDlg(CDlgSwitcher* pSwitcher) : m_iListItemForPopup(0), m_fEditing(false), m_iPrevListItem(-1),
        m_iSortedColumn(-1), m_fSortedAsc(false), m_hTimer(0), m_pSwitcher(pSwitcher), m_hChkPin(0)
    {
    }

    ~CAddinListViewDlg()
    {
    }

    enum { IDD = IDD_ADDINLISTVIEWDLG };

BEGIN_MSG_MAP(CAddinListViewDlg)
    MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
    MESSAGE_HANDLER(WM_DESTROY, OnDestroy)
    MESSAGE_HANDLER(WM_SIZE, OnSize)
    MESSAGE_HANDLER(WM_TIMER, OnTimer)
    MESSAGE_HANDLER(WM_GETMINMAXINFO, OnGetMinMaxInfo)
    MESSAGE_HANDLER(WM_MOUSEMOVE, OnMouseMove)

    COMMAND_HANDLER(IDOK, BN_CLICKED, OnClickedOK)
    COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnClickedCancel)
    COMMAND_HANDLER(IDC_BTN_TREEVIEW, BN_CLICKED, OnBnClickedBtnListView)
    COMMAND_HANDLER(IDC_CHK_PIN, BN_CLICKED, OnBnClickedChkPin)

    NOTIFY_HANDLER(IDC_LIST_FAVORITES, NM_CLICK, OnNMClickListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, NM_RCLICK, OnNMRclickListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, NM_HOVER, OnNMHoverListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, LVN_BEGINLABELEDIT, OnLvnBeginlabeleditListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, LVN_ENDLABELEDIT, OnLvnEndlabeleditListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, LVN_KEYDOWN, OnLvnKeydownListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, LVN_HOTTRACK, OnLvnHotTrackListFavorites)
    NOTIFY_HANDLER(IDC_LIST_FAVORITES, LVN_COLUMNCLICK, OnLvnColumnclickListFavorites)

    COMMAND_ID_HANDLER(ID_POPUP_OPEN, OnPopupOpen)
    COMMAND_ID_HANDLER(ID_POPUP_OPENINNEWWINDOW, OnPopupOpeninnewwindow)
    COMMAND_ID_HANDLER(ID_POPUP_RENAME, OnPopupRename)
    COMMAND_ID_HANDLER(ID_POPUP_DELETE, OnPopupDelete)
    COMMAND_ID_HANDLER(ID_POPUP_PROPERTIES, OnPopupProperties)

    CHAIN_MSG_MAP(CAxDialogImpl<CAddinListViewDlg>)
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
    LRESULT OnMouseMove(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/)
    {
        // cancel the tooltip when the mouse out of the listview control
        CancelToolTip();
        m_iPrevListItem = -1;
        return 0;
    }

    LRESULT OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    LRESULT OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);

    LRESULT OnBnClickedChkPin(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnBnClickedBtnListView(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnNMClickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnNMRclickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnLvnBeginlabeleditListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnLvnEndlabeleditListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnLvnKeydownListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnNMHoverListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnLvnHotTrackListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);
    LRESULT OnLvnColumnclickListFavorites(int /*idCtrl*/, LPNMHDR pNMHDR, BOOL& /*bHandled*/);

    LRESULT OnPopupOpen(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupOpeninnewwindow(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupRename(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupDelete(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);
    LRESULT OnPopupProperties(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);

private:
    void InitListView();
    void LoadListViewItems();
    int GetCurrentHitListItem(POINT* pCursorPos = NULL);
    void LaunchListItem(int iItem, BOOL fNewWidnow = FALSE);
    void PopupContextMenu(int iItem, const POINT& pt);
    LVITEM& MakeLVItem(int iItem);
    static int CALLBACK CompareFunc(LPARAM lParam1, LPARAM lParam2, LPARAM lParamSort);
    void CancelToolTip();
};
