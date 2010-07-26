#include "StdAfx.h"
#include "ONFavoriteList.h"
#include "onenote12.h"

CONFavoriteList::CONFavoriteList(void): m_pTree(new CTreeNode(NODE_ROOT))
{
}

CONFavoriteList::~CONFavoriteList(void)
{
    Destroy();
}

void CONFavoriteList::Destroy()
{
    for (int i=0; i<this->GetFavoriteCount(); ++i)
    {
        CONFavorite* pONFav = this->GetFavorite(i);
        delete pONFav;
    }
    m_favorites.RemoveAll();
    if (m_pTree)
    {
        ResetTree();
    }
}

BOOL CONFavoriteList::Load(void)
{
    //_ASSERTE(0);
    BOOL fResult = TRUE;

    int iPathCount = 0;
    const TCHAR** ptszPaths = CONFavorite::GetONFavoriteFolderPaths(&iPathCount);

    for (int i=0; i<iPathCount; ++i)
    {
        CString strONFavPath = ptszPaths[i];
        WIN32_FIND_DATA fd = {0};
        HANDLE hFind = ::FindFirstFile(strONFavPath + _T("\\*.url"), &fd);
        if (hFind == INVALID_HANDLE_VALUE)
        {
            continue;
        }

        BOOL fRet = TRUE;
        while (fRet)
        {
            if (fd.dwFileAttributes != FILE_ATTRIBUTE_DIRECTORY)
            {
                CONFavorite* pONFav = new CONFavorite();
                fd.cFileName[_tcslen(fd.cFileName) - 4] = 0; // truncate the ".url" part of the file name
                pONFav->SetName(fd.cFileName);
                if (pONFav->Load(strONFavPath))
                {
                    m_favorites.Add(pONFav);
                }
                else
                {
                    delete pONFav;
                    fResult = FALSE;
                    LOGGER << SV_ERROR << _T("Failed to load the ON favorite: ") << fd.cFileName << ENDL;
                }
            }
            fRet = ::FindNextFile(hFind, &fd);
        }

        ::FindClose(hFind);
    }

    if (m_favorites.GetSize() == 0)
    {
        return ONFAV_NO_FAVORITE; // ONFAV_NO_FAVORITE means no favorite avalible
    }

    return fResult;
}

void CONFavoriteList::FreeTree(CTreeNode* pTreeNode)
{
    for (int i=0; i<pTreeNode->children.GetSize(); ++i)
    {
        CTreeNode* pSubNode = pTreeNode->children[i];
        FreeTree(pSubNode);
    }
    delete pTreeNode;
}

bool CONFavoriteList::ParseXmlOut(TCHAR* tszXml, TCHAR** ptszName, NodeType* pType)
{
    bool fRet = false;

    // parse the name
    TCHAR* tszTemp = NULL;
    TCHAR* tszName = _tcsstr(tszXml, _T("name=\""));
    // if there is no name attribute, just set name to NULL
    if (tszName)
    {
        tszName += 6;
        tszTemp = _tcschr(tszName, _T('"'));
        _ASSERTE(tszTemp); if (!tszTemp) goto Erro;
        tszTemp[0] = _T('\0');

        // decode the name (&quot; &amp; &lt; &gt;)
        DecodeString(tszName);
    }

    // parse the node type
    NodeType nodeType;
    TCHAR* tszType = _tcschr(tszXml, _T('<'));
    _ASSERTE(tszType); if (!tszType) goto Erro;
    tszType = _tcschr(tszType + 1, _T('<'));
    _ASSERTE(tszType); if (!tszType) goto Erro;
    ++tszType;
    tszTemp = _tcschr(tszType, _T(' '));
    _ASSERTE(tszTemp); if (!tszTemp) goto Erro;
    tszTemp[0] = _T('\0');

    if (!_tcsicmp(tszType, _T("one:section")))
    {
        nodeType = NODE_SECTION;
    }
    else if (!_tcsicmp(tszType, _T("one:sectiongroup")))
    {
        nodeType = NODE_SECTION_GROUP;
    }
    else if (!_tcsicmp(tszType, _T("one:notebook")))
    {
        nodeType = NODE_NOTEBOOK;
    }
    else if (!_tcsicmp(tszType, _T("one:opensections")))
    {
        nodeType = NODE_NOTEBOOK;
        tszName = _T("OpenSections");
    }
    else if (!_tcsicmp(tszType, _T("one:unfilednotes")))
    {
        nodeType = NODE_NOTEBOOK;
        tszName = _T("UnfiledNotes");
    }
    else
    {
        _ASSERTE(!_T("Unexpected node type!"));
    }

    *ptszName = tszName;
    *pType = nodeType;

    fRet = true;

Erro:
    return fRet;
}

void CONFavoriteList::ResetTree()
{
    FreeTree(m_pTree);
    m_pTree = NULL;
    m_wildFavs.RemoveAll();
}

void CONFavoriteList::BuildTree(OneNoteFavoriteType type)
{
    //_ASSERTE(0);
    CSimpleMap<CComBSTR, CTreeNode*> map;

    if (!m_pTree) m_pTree = new CTreeNode(NODE_ROOT);

    CComQIPtr<IApplication> ptrOneNoteApp;
    HRESULT hr = ptrOneNoteApp.CoCreateInstance(__uuidof(Application));
    if (FAILED(hr)) goto Exit;

    for (int i=0; i<this->GetFavoriteCount(); ++i)
    {
        CONFavorite* pONFav = this->GetFavorite(i);
        if (pONFav->GetType() != type)
        {
            continue;
        }
        CString strId = pONFav->GetId();
        if (strId == _T("")) 
        {
            m_wildFavs.Add(pONFav);
            continue;
        }
        /////////////////////////////////////////////////////////////////////////
        CComBSTR bstrId = strId;
        CComBSTR bstrXmlOut, bstrParentId;
        hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        if (FAILED(hr) ||
            // if the page/section in "Misplaced Sections" but not in normal notebooks, 
            // the call GetHierarchyParent will success
            // but the call GetHierarchy will failed!
            // Werid!
            FAILED(ptrOneNoteApp->GetHierarchy(bstrId, hsSelf, &bstrXmlOut))
            )
        {
            // set the id to empty so that "open in new window" menu item could be disabled
            pONFav->SetId(_T(""));
            m_wildFavs.Add(pONFav);
            continue;
        }
        bstrId = bstrParentId;

        CTreeNode* pChildNode = new CLeafNode(pONFav);
        pChildNode->strID = strId;
        pChildNode->strName = pONFav->GetTitle();
        if (NODE_NOTEBOOK == pChildNode->nodeType)
        {
            m_pTree->AddChild(pChildNode);
            continue;
        }

        while (bstrId != _T(""))
        {
            bool fAlreadyInMap = true;
            CTreeNode* pNode = map.Lookup(bstrId);
            if (!pNode)
            {
                fAlreadyInMap = false;

                hr = ptrOneNoteApp->GetHierarchy(bstrId, hsSelf, &bstrXmlOut);
                _ASSERTE(SUCCEEDED(hr));

                TCHAR* tszName = NULL;
                NodeType nodeType;
                ParseXmlOut(bstrXmlOut, &tszName, &nodeType);

                pNode = new CTreeNode(nodeType);
                pNode->strName = tszName;
                pNode->strID = bstrId;
                map.Add(bstrId, pNode);
            }
            pNode->AddChild(pChildNode);
            
            if (!fAlreadyInMap && NODE_NOTEBOOK == pNode->nodeType)
            {
                m_pTree->AddChild(pNode);
                break; // loop till notebook level
            }
            // if parent node is already in the map, it's no need to search up
            // because the parent's parents... must be the same
            if (fAlreadyInMap)
            {
                break;
            }

            pChildNode = pNode;
            hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
            _ASSERTE(SUCCEEDED(hr));
            bstrId = bstrParentId;
        }
        /////////////////////////////////////////////////////////////////////////
        //CComBSTR bstrXmlOut;
        //CComBSTR bstrId = strId;
        //CComBSTR bstrParentId;
        //hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        //if (FAILED(hr))
        //{
        //    // set the id to empty so that "open in new window" menu item could be disabled
        //    pONFav->SetId(_T(""));
        //    m_wildFavs.Add(pONFav);
        //    continue;
        //}

        //CTreeNode* pTreeNode = new CLeafNode(pONFav);
        //pTreeNode->strID = strId;
        //pTreeNode->strName = pONFav->GetTitle();
        //if (NODE_NOTEBOOK == pTreeNode->nodeType)
        //{
        //    m_pTree->AddChild(pTreeNode);
        //    continue;
        //}

        //// get the parent and parent's parent ...
        //bool fContinue = true;
        //hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        //while (SUCCEEDED(hr))
        //{
        //    bool fAlreadyInMap = true;
        //    CTreeNode* pParentNode = map.Lookup(bstrParentId);
        //    if (!pParentNode)
        //    {
        //        fAlreadyInMap = false;

        //        hr = ptrOneNoteApp->GetHierarchy(bstrParentId, hsSections, &bstrXmlOut);
        //        _ASSERTE(SUCCEEDED(hr));

        //        TCHAR* tszName = NULL;
        //        NodeType nodeType;
        //        ParseXmlOut(bstrXmlOut, &tszName, &nodeType);

        //        pParentNode = new CTreeNode(nodeType);
        //        pParentNode->strName = tszName;
        //        pParentNode->strID = bstrParentId;
        //        map.Add(bstrParentId, pParentNode);
        //    }
        //    pParentNode->AddChild(pTreeNode);
        //    
        //    if (!fAlreadyInMap && NODE_NOTEBOOK == pParentNode->nodeType)
        //    {
        //        m_pTree->AddChild(pParentNode);
        //        break; // loop till notebook level
        //    }
        //    // if parent node is already in the map, it's no need to search up
        //    // because the parent's parents... must be the same
        //    if (fAlreadyInMap)
        //    {
        //        break;
        //    }

        //    pTreeNode = pParentNode;
        //    bstrId = bstrParentId;
        //    hr = ptrOneNoteApp->GetHierarchyParent(bstrId, &bstrParentId);
        //    _ASSERTE(SUCCEEDED(hr));
        //}
    }

Exit:
    return;
}

int CONFavoriteList::GetFavoriteCount()
{
    return m_favorites.GetSize();
}

CONFavorite* CONFavoriteList::GetFavorite(int iIndex)
{
    if (iIndex < 0 && iIndex >= this->GetFavoriteCount())
    {
        return NULL;
    }
    return m_favorites[iIndex];
}

int CONFavoriteList::GetWildFavoriteCount()
{
    return m_wildFavs.GetSize();
}

CONFavorite* CONFavoriteList::GetWildFavorite(int iIndex)
{
    if (iIndex < 0 && iIndex >= this->GetWildFavoriteCount())
    {
        return NULL;
    }
    return m_wildFavs[iIndex];
}

BYTE CONFavoriteList::GetExistStatus()
{
    BYTE bRet = 0;
    for (int i=0; i<GetFavoriteCount(); ++i)
    {
        CONFavorite* pONFav = GetFavorite(i);
        if (pONFav)
        {
            bRet |= (1 << pONFav->GetType());
        }
    }
    return bRet;
}
