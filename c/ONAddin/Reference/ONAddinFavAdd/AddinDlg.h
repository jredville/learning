// AddinDlg.h : Declaration of the CAddinDlg

#pragma once

#include "resource.h"       // main symbols
#include <atlhost.h>
#include "ONFavorite.h"

// CAddinDlg

class CAddinDlg : public CAxDialogImpl<CAddinDlg>
{
private:
    BSTR m_bstrActivePageID;

    HWND m_hStcTitle;
    HWND m_hEditName;
    HWND m_hEditURI;
    HWND m_hComboTypes;
    HFONT m_hFont;

public:
    static HWND m_hAddinDlg;
    CAddinDlg(BSTR bstrActivePageID) : m_bstrActivePageID(bstrActivePageID)
    {
        m_hFont = CreateFont(
            16,                             // height of font
            0,                              // average character width
            0,                              // angle of escapement
            0,                              // base-line orientation angle
            FW_BOLD,                        // font weight
            false,                          // italic attribute option
            false,                          // underline attribute option
            0,                              // strikeout attribute option
            DEFAULT_CHARSET,                // character set identifier
            OUT_DEFAULT_PRECIS,             // output precision
            CLIP_DEFAULT_PRECIS,            // clipping precision
            CLEARTYPE_QUALITY,              // output quality
            DEFAULT_PITCH,                  // pitch and family
            _T("Tahoma"));                  // typeface name
        _ASSERTE(m_hFont);
    }

    ~CAddinDlg()
    {
        DeleteObject(m_hFont);
    }

    enum { IDD = IDD_ADDINDLG };

BEGIN_MSG_MAP(CAddinDlg)
    MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
    MESSAGE_HANDLER(WM_DESTROY, OnDestroy)
    COMMAND_HANDLER(IDOK, BN_CLICKED, OnClickedOK)
    COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnClickedCancel)
    COMMAND_HANDLER(IDC_COMBO_TYPE, CBN_SELCHANGE, OnCbnSelchangeComboType)
    CHAIN_MSG_MAP(CAxDialogImpl<CAddinDlg>)
END_MSG_MAP()

    LRESULT OnDestroy(UINT /*uMsg*/, WPARAM /*wParam*/, LPARAM /*lParam*/, BOOL& /*bHandled*/);
    LRESULT OnInitDialog(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled);
    LRESULT OnClickedOK(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    LRESULT OnClickedCancel(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled);
    LRESULT OnCbnSelchangeComboType(WORD /*wNotifyCode*/, WORD /*wID*/, HWND /*hWndCtl*/, BOOL& /*bHandled*/);

    static BOOL IsCurrentlyShowing();

private:
    void InitComboBox();
    bool ParseXmlOut(TCHAR* tszXml, TCHAR** ptszName, OneNoteFavoriteType* pFavType);
    bool LoadAllLinks();
    static LONG s_lShowingFlag;
};


