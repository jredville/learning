#include <tchar.h>
#include <windows.h>
#include "resource.h"
#include "main.h"
const WCHAR g_szClassName[] = _T("myWindowClass");
int CreateThisWindow(HWND hwnd);
int SetIcon(HWND hwnd, WCHAR name[], WCHAR msg[], int iconSize);
// Step 4: the Window Procedure
LRESULT CALLBACK WndProc(HWND hwnd, UINT msg, WPARAM wParam, LPARAM lParam)
{
    switch(msg)
    {
        case WM_CREATE:
            CreateThisWindow(hwnd);
            break;
        case WM_CLOSE:
            DestroyWindow(hwnd);
            break;
        case WM_COMMAND:
            switch(LOWORD(wParam)) {
                case ID_FILE_EXIT:
                    PostMessage(hwnd, WM_CLOSE, 0,0);
                    break;
                case ID_STUFF_GO:
                    MessageBox(hwnd, _T("What do you want!!!"), _T("AWAY"), MB_OK | MB_ICONINFORMATION);
                    break;
            }
        case WM_DESTROY:
            PostQuitMessage(0);
            break;
        case WM_LBUTTONDOWN:
            DisplayMouseDialog(hwnd);
            break;
        default:
            return DefWindowProc(hwnd, msg, wParam, lParam);
    }
    return 0;
}

int CreateThisWindow(HWND hwnd) {
    HMENU hMenu, hSubMenu;
    int res;
    WCHAR icon1[] = _T("menu_one.ico");

    hMenu = CreateMenu();

    hSubMenu = CreatePopupMenu();
    AppendMenu(hSubMenu, MF_STRING, ID_FILE_EXIT, _T("E&xit"));
    AppendMenu(hMenu, MF_STRING | MF_POPUP, (UINT)hSubMenu, _T("&File"));

    hSubMenu = CreatePopupMenu();
    AppendMenu(hSubMenu, MF_STRING, ID_STUFF_GO, _T("&Go"));
    AppendMenu(hMenu, MF_STRING | MF_POPUP, (UINT)hSubMenu, _T("&Stuff"));

    SetMenu(hwnd, hMenu);
    res = SetIcon(hwnd, icon1, _T("large"), ICON_BIG);
    res += SetIcon(hwnd, icon1, _T("small"), ICON_SMALL);
    return res;
}

int SetIcon(HWND hwnd, WCHAR name[], WCHAR msg[], int iconSize) {
    int size;
    HICON icon;
    if (iconSize == ICON_SMALL) {
        size = 16;
    } else {
        size = 32;
    }
    icon = (HICON)LoadImage(NULL, name, IMAGE_ICON, size, size, LR_LOADFROMFILE);
    if(icon) {
        SendMessage(hwnd, WM_SETICON, iconSize, (LPARAM)icon);
        return 0;
    } else {

        MessageBox(hwnd, _T("Could not load icon!"), _T("Error"), MB_OK | MB_ICONERROR);
        return -1;
    }
}

int DisplayMouseDialog(HWND hwnd) {

    WCHAR szFileName[MAX_PATH];
    HINSTANCE hInstance = GetModuleHandle(NULL);
    GetModuleFileName(hInstance, szFileName, _countof(szFileName));
    return MessageBox(hwnd, szFileName, _T("This program is:"), MB_OK | MB_ICONINFORMATION);
}

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
    LPSTR lpCmdLine, int nCmdShow)
{
    WNDCLASSEX wc;
    HWND hwnd;
    MSG Msg;
    HINSTANCE hInst = GetModuleHandle(NULL);
    //Step 1: Registering the Window Class
    wc.cbSize        = sizeof(WNDCLASSEX);
    wc.style         = 0;
    wc.lpfnWndProc   = WndProc;
    wc.cbClsExtra    = 0;
    wc.cbWndExtra    = 0;
    wc.hInstance     = hInstance;
    wc.hIcon         = LoadIcon(NULL, IDI_APPLICATION);
    wc.hCursor       = LoadCursor(NULL, IDC_ARROW);
    wc.hbrBackground = (HBRUSH)(COLOR_WINDOW+1);
    wc.lpszMenuName  = NULL;
    wc.lpszClassName = g_szClassName;
    wc.hIconSm       = LoadIcon(NULL, IDI_APPLICATION);

    if(!RegisterClassEx(&wc))
    {
        MessageBox(NULL, _T("Window Registration Failed!"), _T("Error!"),
            MB_ICONEXCLAMATION | MB_OK);
        return 0;
    }

    // Step 2: Creating the Window
    hwnd = CreateWindowEx(
        WS_EX_WINDOWEDGE,
        g_szClassName,
        _T("The title of my window"),
        WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, CW_USEDEFAULT, 240, 120,
        NULL, NULL, hInstance, NULL);

    if(hwnd == NULL)
    {
        MessageBox(NULL, _T("Window Creation Failed!"), _T("Error!"),
            MB_ICONEXCLAMATION | MB_OK);
        return 0;
    }

    ShowWindow(hwnd, nCmdShow);
    UpdateWindow(hwnd);

    // Step 3: The Message Loop
    while(GetMessage(&Msg, NULL, 0, 0) > 0)
    {
        TranslateMessage(&Msg);
        DispatchMessage(&Msg);
    }
    return Msg.wParam;
}
