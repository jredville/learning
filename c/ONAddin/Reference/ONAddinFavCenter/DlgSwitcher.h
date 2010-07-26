#pragma once

class CAddinDlg;
class CAddinListViewDlg;

class CDlgSwitcher
{
public:
    CDlgSwitcher(void);
    ~CDlgSwitcher(void);

    void ShowListViewDlg();
    void ShowTreeViewDlg();
    void CloseListViewDlg();
    void CloseTreeViewDlg();
    static HWND s_hCurrentDlg;

private:
    HWND m_hOneNoteWnd;
    CAddinDlg* m_pTreeDlg;
    CAddinListViewDlg* m_pListDlg;
};
