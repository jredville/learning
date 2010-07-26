#include "StdAfx.h"
#include "ONFavorite.h"
#include <shlobj.h>

CONFavorite::CONFavorite(void)
{
}

CONFavorite::~CONFavorite(void)
{
}

const TCHAR ** CONFavorite::GetONFavoriteFolderPaths(int* piCount)
{
    static TCHAR tszPath1[ MAX_PATH ] = {0};
    static TCHAR tszPath2[ MAX_PATH ] = {0};
    static TCHAR* ptszPath[2] = {tszPath1, tszPath2};

    TCHAR tszPath[ MAX_PATH ] = {0};

    if (tszPath[0] == _T('\0'))
    {
        //CRegKey reg;
        //LONG lRet = reg.Open(HKEY_CURRENT_USER, _T("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Shell Folders"), KEY_READ);
        //if (ERROR_SUCCESS != lRet) return NULL;

        //ULONG ulChars = MAX_PATH;
        //lRet = reg.QueryStringValue(_T("Favorites"), tszPath, &ulChars);
        //if (ERROR_SUCCESS != lRet) return NULL;

        BOOL fRet = ::SHGetSpecialFolderPath(NULL, tszPath, CSIDL_FAVORITES, TRUE);
        _ASSERTE(fRet);
        if (!fRet) return NULL;
        HRESULT hr = StringCchPrintf(ptszPath[0], MAX_PATH, _T("%s%s"), tszPath, _T("\\Links\\OneNote\\"));
        if (FAILED(hr)) return NULL;
        hr = StringCchPrintf(ptszPath[1], MAX_PATH, _T("%s%s"), tszPath, _T("\\OneNote\\"));
        if (FAILED(hr)) return NULL;
    }

    *piCount = 2;

    return (const TCHAR**) ptszPath;
}

BOOL CONFavorite::Save(bool fForceSave)
{
    BOOL fRet = FALSE;
    int iPathCount = 0;
    LPCTSTR* ptszFavFolderPath = GetONFavoriteFolderPaths(&iPathCount);
    LPCTSTR tszFavFolderPath = NULL;

    CString strPath, strType;

    if (!ptszFavFolderPath) goto Exit;

    // find the first exist path
    for (int i=0; i<iPathCount; ++i)
    {
        if (::PathFileExists(ptszFavFolderPath[i]))
        {
            tszFavFolderPath = ptszFavFolderPath[i];
            break;
        }
    }
    if (!tszFavFolderPath)
    {
        tszFavFolderPath = ptszFavFolderPath[0];
        fRet = ::CreateDirectory(tszFavFolderPath, 0);
        if (!fRet)
        {
            fRet = FALSE;
            goto Exit;
        }
    }

    strPath = tszFavFolderPath;
    strPath.Append(m_strName);
    strPath.Append(_T(".url"));

    if (!fForceSave && ::PathFileExists(strPath))
    {
        fRet = ON_FAV_ALREADY_EXITST; // return ON_FAV_ALREADY_EXITST if the ON Favorite URL file exists!
        goto Exit;
    }

    HANDLE hFile = ::CreateFile(strPath, GENERIC_WRITE, FILE_SHARE_READ, 0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
    if (INVALID_HANDLE_VALUE == hFile)
    {
        fRet = FALSE;
        goto Exit;
    }
    ::CloseHandle(hFile);

    fRet = WritePrivateProfileString(TEXT("DEFAULT"), TEXT("BASEURL"), m_strURI, strPath);
    if (!fRet) goto Exit;
    fRet = WritePrivateProfileString(TEXT("DEFAULT"), TEXT("TITLE"), m_strTitle, strPath);
    if (!fRet) goto Exit;
    fRet = WritePrivateProfileString(TEXT("DEFAULT"), TEXT("FAVID"), m_strId, strPath);
    if (!fRet) goto Exit;
    switch(m_type)
    {
    case ONFT_PAGE:
        strType = _T("PAGE");
        break;
    case ONFT_SECTION:
        strType = _T("SECTION");
        break;
    case ONFT_SECTION_GROUP:
        strType = _T("SECTION_GROUP");
        break;
    case ONFT_NOTEBOOK:
        strType = _T("NOTEBOOK");
        break;
    }
    fRet = WritePrivateProfileString(TEXT("DEFAULT"), TEXT("TYPE"), strType, strPath);
    if (!fRet) goto Exit;
    fRet = WritePrivateProfileString(TEXT("InternetShortcut"), TEXT("URL"), m_strURI, strPath);
    if (!fRet) goto Exit;
    fRet = WritePrivateProfileString(TEXT("InternetShortcut"), TEXT("IDList"), _T(""), strPath);
    if (!fRet) goto Exit;
    fRet = WritePrivateProfileString(TEXT("{000214A0-0000-0000-C000-000000000046}"), TEXT("Prop3"), _T("19,2"), strPath);

Exit:
    return fRet;
}

BOOL CONFavorite::Load(const TCHAR* tszFolderPath)
{
    m_strFilePath = tszFolderPath;
    m_strFilePath.Append(m_strName);
    m_strFilePath.Append(_T(".url"));

    TCHAR tszValue[ MAX_PATH ];
    BOOL fRet = GetPrivateProfileString(TEXT("DEFAULT"), TEXT("BASEURL"), TEXT(""), tszValue, MAX_PATH, m_strFilePath);
    if (!fRet) 
    {
        fRet = GetPrivateProfileString(TEXT("InternetShortcut"), TEXT("URL"), TEXT(""), tszValue, MAX_PATH, m_strFilePath);
    }
    if (!fRet) goto Exit;
    m_strURI = tszValue;

    fRet = GetPrivateProfileString(TEXT("DEFAULT"), TEXT("TITLE"), TEXT(""), tszValue, MAX_PATH, m_strFilePath);
    if (!fRet)
    {
        m_strTitle = m_strName;
        fRet = TRUE;
    }
    else
    {
        m_strTitle = tszValue;
    }
    fRet = GetPrivateProfileString(TEXT("DEFAULT"), TEXT("FAVID"), TEXT(""), tszValue, MAX_PATH, m_strFilePath);
    if (!fRet)
    {
        // backward compatible
        fRet = GetPrivateProfileString(TEXT("DEFAULT"), TEXT("PAGEID"), TEXT(""), tszValue, MAX_PATH, m_strFilePath);
    }
    m_strId = tszValue;

    fRet = GetPrivateProfileString(TEXT("DEFAULT"), TEXT("TYPE"), TEXT("PAGE"), tszValue, MAX_PATH, m_strFilePath);
    if (!fRet) goto Exit;
    if (!_tcsicmp(tszValue, _T("PAGE")))
    {
        m_type = ONFT_PAGE;
    }
    else if (!_tcsicmp(tszValue, _T("SECTION")))
    {
        m_type = ONFT_SECTION;
    }
    else if (!_tcsicmp(tszValue, _T("SECTION_GROUP")))
    {
        m_type = ONFT_SECTION_GROUP;
    }
    else if (!_tcsicmp(tszValue, _T("NOTEBOOK")))
    {
        m_type = ONFT_NOTEBOOK;
    }
    else
    {
        m_type = ONFT_PAGE;
    }

    fRet = TRUE;

Exit:
    return fRet;
}

BOOL CONFavorite::Delete()
{
    // get the file path termiated by double '\0'
    // because SHFileOperation requires to do so
    int iLen = m_strFilePath.GetLength();
    TCHAR* tszPath = new TCHAR[iLen + 2];
    StringCchCopy(tszPath, iLen + 2, m_strFilePath);
    tszPath[iLen + 1] = _T('\0');

    // delete the favorite file to the recycle bin
    SHFILEOPSTRUCT fo = {0};
    fo.wFunc = FO_DELETE;
    fo.pFrom = tszPath;
    fo.fFlags = FOF_ALLOWUNDO /*| FOF_NOERRORUI*/;
    int iRet = ::SHFileOperation(&fo);

    delete [] tszPath;

    return 0 == iRet;
}

BOOL CONFavorite::OpenProperty(void)
{
    // open the windows file property dialog
    SHELLEXECUTEINFO ShExecInfo = {0};
    ShExecInfo.cbSize = sizeof(SHELLEXECUTEINFO);
    ShExecInfo.fMask = SEE_MASK_INVOKEIDLIST;
    ShExecInfo.hwnd = NULL;
    ShExecInfo.lpVerb = _T("properties");
    ShExecInfo.lpFile = m_strFilePath;
    ShExecInfo.lpParameters = _T("");
    ShExecInfo.lpDirectory = NULL;
    ShExecInfo.nShow = SW_SHOW;
    ShExecInfo.hInstApp = NULL;

    BOOL fRet = ShellExecuteEx(&ShExecInfo);

    return fRet;
}


// decode the string (for &quot; &amp; &lt; &gt;)
void DecodeString(TCHAR* tszData)
{
    TCHAR* p = tszData;
    TCHAR* p0 = tszData;
    while (*p)
    {
        if (*p == _T('&'))
        {
            if (!_tcsncmp(p, _T("&quot;"), 6))
            {
                p += 5;
                *p = _T('"');
            }
            else if (!_tcsncmp(p, _T("&amp;"), 5))
            {
                p += 4;
                *p = _T('&');
            }
            else if (!_tcsncmp(p, _T("&lt;"), 4))
            {
                p += 3;
                *p = _T('<');
            }
            else if (!_tcsncmp(p, _T("&gt;"), 4))
            {
                p += 3;
                *p = _T('>');
            }
        }
        *(p0++) = *(p++);
    }
    *p0 = _T('\0');
}

// decode the string to file name (remove \ : * ? " < > |, and replace / with -)
void MakeFileName(TCHAR* tszData)
{
    TCHAR* p = tszData;
    TCHAR* p0 = tszData;
    while (*p)
    {
        if (*p == _T('\\') || *p == _T(':') || *p == _T('*') || *p == _T('?') ||
            *p == _T('"') || *p == _T('<') || *p == _T('>') || *p == _T('|'))
        {
            ++p;
            continue;
        }
        else if (*p == _T('/'))
        {
            *p = _T('-');
        }
        *(p0++) = *(p++);
    }
    *p0 = _T('\0');
}
