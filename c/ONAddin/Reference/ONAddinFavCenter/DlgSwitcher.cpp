#include "StdAfx.h"
#include "DlgSwitcher.h"
#include "AddinDlg.h"
#include "AddinListViewDlg.h"

HWND CDlgSwitcher::s_hCurrentDlg = NULL;

extern HWND g_hWndHidden;

CDlgSwitcher::CDlgSwitcher(void) : m_pTreeDlg(NULL), m_pListDlg(NULL), m_hOneNoteWnd(NULL)
{
    //_ASSERTE(0);
    m_hOneNoteWnd = FindWindow(_T("Framework::CFrame"), NULL);
}

CDlgSwitcher::~CDlgSwitcher(void)
{
    CloseListViewDlg();
    CloseTreeViewDlg();

    delete m_pListDlg;
    m_pListDlg = NULL;

    delete m_pTreeDlg;
    m_pTreeDlg = NULL;
}

void CDlgSwitcher::ShowListViewDlg()
{
    CloseTreeViewDlg();

    if (!m_pListDlg)
    {
        m_pListDlg = new CAddinListViewDlg(this);
    }
    if (!m_pListDlg->IsWindow())
    {
        m_pListDlg->Create(m_hOneNoteWnd);
        s_hCurrentDlg = m_pListDlg->m_hWnd;
    }
    m_pListDlg->ShowWindow(SW_SHOW);
}

void CDlgSwitcher::ShowTreeViewDlg()
{
    CloseListViewDlg();

    if (!m_pTreeDlg)
    {
        m_pTreeDlg = new CAddinDlg(this);
    }
    if (!m_pTreeDlg->IsWindow())
    {
        m_pTreeDlg->Create(m_hOneNoteWnd);
        s_hCurrentDlg = m_pTreeDlg->m_hWnd;
    }
    m_pTreeDlg->ShowWindow(SW_SHOW);
}

void CDlgSwitcher::CloseListViewDlg()
{
    //_ASSERTE(0);
    if (m_pListDlg && m_pListDlg->IsWindow())
    {
        m_pListDlg->DestroyWindow();
        m_pListDlg->m_hWnd = NULL;
    }
}

void CDlgSwitcher::CloseTreeViewDlg()
{
    //_ASSERTE(0);
    if (m_pTreeDlg && m_pTreeDlg->IsWindow())
    {
        m_pTreeDlg->DestroyWindow();
        m_pTreeDlg->m_hWnd = NULL;
    }
}
