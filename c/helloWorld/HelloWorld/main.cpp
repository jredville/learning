#define WIN32_LEAN_AND_MEAN

#include <windows.h>

int APIENTRY WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nShowCmd)
{
	MessageBox(NULL, L"Goodbye, cruel world!", L"Note", MB_OK);
	return 0;
}
