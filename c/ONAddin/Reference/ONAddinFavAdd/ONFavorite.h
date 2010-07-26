#pragma once

enum OneNoteFavoriteType
{
    ONFT_PAGE, ONFT_SECTION, ONFT_SECTION_GROUP, ONFT_NOTEBOOK
};

#define ON_FAV_ALREADY_EXITST 100

class CONFavorite
{
private:
    CString m_strTitle;
    CString m_strName;
    CString m_strURI;
    CString m_strFilePath;
    CString m_strId;
    OneNoteFavoriteType m_type;

public:
    CONFavorite(void);
    ~CONFavorite(void);

    inline void SetTitle(LPCTSTR tszTitle) { m_strTitle = tszTitle; }
    inline void SetName(LPCTSTR tszName) { m_strName = tszName; }
    inline void SetURI(LPCTSTR tszURI) { m_strURI = tszURI; }
    inline void SetId(LPCTSTR strId) { m_strId = strId; }
    inline void SetType(OneNoteFavoriteType type) { m_type = type; }

    inline CString& GetTitle() { return m_strTitle; }
    inline CString& GetName() { return m_strTitle; }
    inline CString& GetURI() { return m_strURI; }
    inline CString& GetId() { return m_strId; }
    inline OneNoteFavoriteType GetType() { return m_type; }

    static const TCHAR** GetONFavoriteFolderPaths(int* piCount);

    BOOL Save(bool fForceSave); // return ON_FAV_ALREADY_EXITST if the ON Favorite URL file exists!
    BOOL Load(const TCHAR* tszFolderPath);
    BOOL Delete();
    BOOL OpenProperty(void);
};

void DecodeString(TCHAR* tszData);
void MakeFileName(TCHAR* tszData);
