// ONAddinFavCenter.cpp : Implementation of WinMain


#include "stdafx.h"
#include "resource.h"
#include "ONAddinFavCenter_i.h"
#include "DlgSwitcher.h"

HINSTANCE g_hInst = NULL;
HWND g_hWndHidden = NULL;

class CONAddinFavCenterModule : public CAtlExeModuleT< CONAddinFavCenterModule >
{
public :
	DECLARE_LIBID(LIBID_ONAddinFavCenterLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ONADDINFAVCENTER, "{B6931762-8191-4C7A-A1DD-9C85AFA2B590}")

	void RunMessageLoop() throw()
	{
        MSG msg;
        while (GetMessage(&msg, 0, 0, 0) > 0) 
        {
            if (!CDlgSwitcher::s_hCurrentDlg ||
                !::IsDialogMessage(CDlgSwitcher::s_hCurrentDlg, &msg))
            {
                TranslateMessage(&msg);
                DispatchMessage(&msg);
            }
        }
	};
};

CONAddinFavCenterModule _AtlModule;

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    return DefWindowProc(hWnd, message, wParam, lParam);
}

//
extern "C" int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE /*hPrevInstance*/, 
                                LPTSTR lpCmdLine, int nShowCmd)
{
    LOGGER.Initialize();
    //LOGGER.AddRecorder(new CFileLogRecorder(SV_DEBUG, _T("D:\\ONAddinFavCenter.log")));

    g_hInst = hInstance;

    return _AtlModule.WinMain(nShowCmd);
}
