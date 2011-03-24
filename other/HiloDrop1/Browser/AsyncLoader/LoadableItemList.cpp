//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "LoadableItemList.h"
#include "CriticalSectionLocker.h"

using namespace std;
using namespace Hilo::AsyncLoader;

LoadableItemList::LoadableItemList() : 
    m_cs(nullptr),
    m_currentItem(0),
    m_passCount(1),
    m_currentPass(0),
    m_resetOnAdd(true)
{
}

LoadableItemList::~LoadableItemList()
{
}

HRESULT LoadableItemList::SetCriticalSection(CRITICAL_SECTION* cs)
{
    m_cs = cs;
    return S_OK;
}

HRESULT LoadableItemList::SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager)
{
    m_layoutManager = layoutManager;
    return S_OK;
}

HRESULT LoadableItemList::Add(IAsyncLoaderItem* item)
{
    if ( nullptr == item )
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    CriticalSectionLocker l(m_cs);

    m_loadableItems.push_back(ComPtr<IAsyncLoaderItem>(item));
    if (m_resetOnAdd)
    {
        ResetEnumeration();
    }

    if ( nullptr != m_layoutManager )
    {
        hr = m_layoutManager->SetBoundaries(0, static_cast<unsigned int>(m_loadableItems.size()));

        if ( FAILED(hr) )
        {
            return hr;
        }
    }
    return hr;
}

HRESULT LoadableItemList::GetCount(unsigned int* count)
{
    if ( nullptr == count )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    *count = static_cast<unsigned int>(m_loadableItems.size());

    return S_OK;
}

HRESULT LoadableItemList::ResetOnAdd(bool reset)
{
    CriticalSectionLocker l(m_cs);

    m_resetOnAdd = reset;

    return S_OK;
}

HRESULT LoadableItemList::ResetEnumeration()
{
    CriticalSectionLocker l(m_cs);

    m_currentItem = 0;
    m_currentPass = 0;

    return S_OK;
}

HRESULT LoadableItemList::EnumerateNextItem(IAsyncLoaderItem** item, unsigned int* pass)
{
    if ( !IsOutputPointerValid(item) || (nullptr == pass) )
    {
        return E_POINTER;
    }
    CriticalSectionLocker l(m_cs);

    if ( 0 == m_loadableItems.size() )
    {
        // No more passes.
        *pass = 0;
        m_currentItem = 0;
        m_currentPass = 0;
        return S_FALSE;
    }

    // We've finished enumerating all items in the list...
    if ( m_currentItem >= m_loadableItems.size() )
    {
        // Move to the next pass...
        ++m_currentPass;

        // Should we do more passes?
        if ( m_currentPass < m_passCount )
        {
            // Reset...
            m_currentItem = 0;
        }
        else
        {
            // No more passes.
            *pass = 0;
            return S_FALSE;
        }
    }

    AssignToOutputPointer(item, m_loadableItems[m_currentItem]);
    *pass = m_currentPass;

    ++m_currentItem;

    return S_OK;
}

HRESULT LoadableItemList::SetPassCount(unsigned int passCount)
{
    CriticalSectionLocker l(m_cs);

    // Cannot have less than one pass...
    if ( passCount < 1 )
    {
        return E_FAIL;
    }

    m_passCount = passCount;
    m_currentPass = 0;
    m_currentItem = 0;

    return S_OK;
}

HRESULT LoadableItemList::Clear()
{
    CriticalSectionLocker l(m_cs);
    HRESULT hr = S_OK;

    // Reset internal vector...
    for (auto it = m_loadableItems.begin() ; it < m_loadableItems.end() ; ++it )
    {
        (*it)->Shutdown();
        (*it) = nullptr;
    }
    m_loadableItems.clear();

    // Reset enumeration state...
    m_currentPass = 0;
    m_currentItem = 0;

    ResetEnumeration();

    // Notify the layout manager of the change...
    if ( nullptr != m_layoutManager )
    {
        hr = m_layoutManager->SetBoundaries(0, static_cast<unsigned int>(m_loadableItems.size()));
        if ( FAILED(hr) )
        {
            return hr;
        }
    }

    return hr;
}

HRESULT LoadableItemList::Shutdown()
{
    HRESULT hr = Clear();

    m_layoutManager = nullptr;
    
    return hr;
}
