#pragma once

#include "..\ONAddinFavAdd\ONFavorite.h"

#define ONFAV_NO_FAVORITE 2

enum NodeType { NODE_ROOT, NODE_NOTEBOOK, NODE_SECTION_GROUP, NODE_SECTION, NODE_PAGE };

class CTreeNode
{
public:
    CTreeNode(NodeType type) : nodeType(type), iLevel(0) {};
    virtual ~CTreeNode() {};
    int IncreaseLevel()
    {
        for (int i=0; i<children.GetSize(); ++i)
        {
            children[i]->IncreaseLevel();
        }
        return iLevel++;
    };

    virtual void AddChild(CTreeNode* pNode)
    {
        children.Add(pNode);
        pNode->IncreaseLevel();
    }

    NodeType nodeType;
    int iLevel;
    CString strID;
    CString strName;
    CSimpleArray<CTreeNode*> children;
};

class CLeafNode : public CTreeNode
{
public:
    CLeafNode(CONFavorite* pONFav) : CTreeNode(NODE_PAGE), lpONFavorite(pONFav)
    {
        OneNoteFavoriteType favType = pONFav->GetType();
        switch(favType)
        {
        case ONFT_PAGE:
            nodeType = NODE_PAGE;
            break;
        case ONFT_SECTION:
            nodeType = NODE_SECTION;
            break;
        case ONFT_SECTION_GROUP:
            nodeType = NODE_SECTION_GROUP;
            break;
        case ONFT_NOTEBOOK:
            nodeType = NODE_NOTEBOOK;
            break;
        }
    };
    virtual ~CLeafNode() {};
    virtual void AddChild(CTreeNode* pNode)
    {
        // do nothing
    }

    CONFavorite* lpONFavorite;
};

class CONFavoriteList
{
public:
    CONFavoriteList(void);
    ~CONFavoriteList(void);

    BOOL Load(void);
    void BuildTree(OneNoteFavoriteType type);
    int GetFavoriteCount();
    CONFavorite* GetFavorite(int iIndex);

    int GetWildFavoriteCount();
    CONFavorite* GetWildFavorite(int iIndex);

    inline CTreeNode* GetTree() { return m_pTree; };
    BYTE GetExistStatus();
    void ResetTree();
    void Destroy();

private:
    void FreeTree(CTreeNode* pTreeNode);
    bool ParseXmlOut(TCHAR* tszXml, TCHAR** ptszName, NodeType* pType);

private:
    CSimpleArray<CONFavorite*> m_favorites;
    CSimpleArray<CONFavorite*> m_wildFavs;
    CTreeNode* m_pTree;
};
