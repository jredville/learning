
#pragma once

extern HINSTANCE g_hInst;

static BOOL GetWindowRelatedRect(HWND hWnd, HWND hParentWnd, LPRECT lpRect)
{
    POINT p = {0, 0};
    BOOL fRet = ::GetWindowRect(hWnd, lpRect);
    if (!fRet) goto Exit;

    p.x = lpRect->left;
    p.y = lpRect->top;
    fRet = ::ScreenToClient(hParentWnd, &p);
    if (!fRet) goto Exit;
    lpRect->left = p.x;
    lpRect->top = p.y;

    p.x = lpRect->right;
    p.y = lpRect->bottom;
    fRet = ::ScreenToClient(hParentWnd, &p);
    if (!fRet) goto Exit;
    lpRect->right = p.x;
    lpRect->bottom = p.y;

Exit:
    return fRet;
}

static int ListView_GetSelectedItem(HWND hListView)
{
    int iItemCount = ListView_GetItemCount(hListView);

    UINT uiRequiredState = LVIS_SELECTED;
    if (iItemCount > 1)
    {
        uiRequiredState = LVIS_FOCUSED;
    }

    for (int i=0; i<iItemCount; ++i)
    {
        UINT uiState = ListView_GetItemState(hListView, i, uiRequiredState);
        if (uiRequiredState & uiState)
        {
            return i;
        }
    }

    return -1;
}

static CSimpleArray<int> ListView_GetSelectedItems(HWND hListView)
{
    CSimpleArray<int> arrItems;

    int iItemCount = ListView_GetItemCount(hListView);

    for (int i=0; i<iItemCount; ++i)
    {
        UINT uiState = ListView_GetItemState(hListView, i, LVIS_SELECTED);
        if (LVIS_SELECTED & uiState)
        {
            arrItems.Add(i);
        }
    }
    
    return arrItems;
}

static TCHAR* GetDataFilePath()
{
    static TCHAR tszModulePath[MAX_PATH] = {0};
    if (tszModulePath[0] == 0)
    {
        ::GetModuleFileName(g_hInst, tszModulePath, MAX_PATH);
        TCHAR* tszTemp = _tcsrchr(tszModulePath, _T('\\'));
        tszTemp[0] = 0;
        StringCchCat(tszModulePath, MAX_PATH, _T("\\conf.dat"));
    }
    return tszModulePath;
}

static void WriteDataFile(UINT uiIndex, DWORD dwData)
{
    HANDLE hFile = ::CreateFile(GetDataFilePath(), GENERIC_WRITE, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile != INVALID_HANDLE_VALUE) 
    {
        if (uiIndex > 0)
        {
            ::SetFilePointer(hFile, uiIndex * sizeof(DWORD), 0, FILE_BEGIN);
        }
        DWORD dwWritten = 0;
        BOOL fRet = ::WriteFile(hFile, &dwData, sizeof(DWORD), &dwWritten, NULL);
        ::CloseHandle(hFile);
    }
}

static DWORD ReadDataFile(UINT uiIndex)
{
    DWORD dwData = 0;

    HANDLE hFile = ::CreateFile(GetDataFilePath(), GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
    if (hFile != INVALID_HANDLE_VALUE) 
    { 
        if (uiIndex > 0)
        {
            ::SetFilePointer(hFile, uiIndex * sizeof(DWORD), 0, FILE_BEGIN);
        }
        DWORD dwRead = 0;
        BOOL fRet = ::ReadFile(hFile, &dwData, sizeof(DWORD), &dwRead, NULL);
        ::CloseHandle(hFile);
    }

    return dwData;
}

static CString& GetToolTipInfo(CONFavorite* pONFav)
{
    static CString strToolTipInfo;

    strToolTipInfo = pONFav->GetName();
    strToolTipInfo.Append(_T(".url\r\n"));
    CString strURI = pONFav->GetURI();
    TCHAR tchDelimit = _T('?');
    if (!_tcsncmp(strURI.GetBuffer(), _T("onenote:"), 8)) tchDelimit = _T('#');
    int iQPos = strURI.Find(tchDelimit);
    if (iQPos > 0)
    {
        strURI = strURI.Left(iQPos + 1);
        strURI.Append(_T("..."));
    }
    strToolTipInfo.Append(strURI);

    return strToolTipInfo;
}
