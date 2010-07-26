// ONAddinFavAdd.cpp : Implementation of WinMain


#include "stdafx.h"
#include "resource.h"
#include "ONAddinFavAdd_i.h"
#include "AddinMain.h"
#include "AddinDlg.h"

HINSTANCE g_hInst = NULL;
HWND CAddinDlg::m_hAddinDlg = NULL;

class CONAddinFavAddModule : public CAtlExeModuleT< CONAddinFavAddModule >
{
public :
	DECLARE_LIBID(LIBID_ONAddinFavAddLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_ONADDINFAVADD, "{73592184-9D3B-41E2-BF4B-DDFBA029C9BF}")
	void RunMessageLoop() throw()
    {
        CAtlExeModuleT< CONAddinFavAddModule >::RunMessageLoop();
	};
};

CONAddinFavAddModule _AtlModule;

//
extern "C" int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE /*hPrevInstance*/, 
                                LPTSTR /*lpCmdLine*/, int nShowCmd)
{
    LOGGER.Initialize();
    //LOGGER.AddRecorder(new CFileLogRecorder(SV_DEBUG, _T("D:\\ONAddinFav.log")));

	g_hInst = hInstance;

    return _AtlModule.WinMain(nShowCmd);
}

