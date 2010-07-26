#pragma once

struct IOneNoteAddIn;

class CFavAddDlgLauncher
{
public:
    CFavAddDlgLauncher(void);
    ~CFavAddDlgLauncher(void);

    void OpenAddFavoriteDlg(void);

private:
    IOneNoteAddIn* m_pAddFavAddIn;
};
