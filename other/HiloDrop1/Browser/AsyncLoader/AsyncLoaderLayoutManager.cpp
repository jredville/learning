//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include <string>
#include "AsyncLoaderLayoutManager.h"
#include "CriticalSectionLocker.h"

using namespace std;
using namespace Hilo::AsyncLoader;

AsyncLoaderLayoutManager::AsyncLoaderLayoutManager() :
    m_cs(nullptr),
    m_pageItemCount(0),
    m_lowerBoundary(0),
    m_upperBoundary(0),
    m_nextPagePercentage(200),        // Two full pages.
    m_previousPagePercentage(100),    // One full page.
    m_secondarySizePercentage(121),   // by default : secondary size = smaller size
                                      // smaller size = more items in the window by %10 ^ 2...
    // Current size items...
    m_currentPageStart(0),
    m_currentPageEnd(0),
    m_nextPageStart(0),
    m_nextPageEnd(0),
    m_previousPageStart(0),
    m_previousPageEnd(0),
    // Secondary size items...
    m_currentPageSecondarySizePageStart(0),
    m_currentPageSecondarySizePageEnd(0),
    m_nextPageSecondarySizePageStart(0),
    m_nextPageSecondarySizePageEnd(0),
    m_previousPageSecondarySizePageStart(0),
    m_previousPageSecondarySizePageEnd(0)
{
}

AsyncLoaderLayoutManager::~AsyncLoaderLayoutManager()
{
}

// Page settings...
HRESULT AsyncLoaderLayoutManager::RecalculatePages()
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    int pageItemCount = m_pageItemCount;
    int nextPageItemCount = static_cast<int>((static_cast<float>(m_pageItemCount) / 100 * m_nextPagePercentage) + 1);
    int previousPageItemCount = static_cast<int>((static_cast<float>(m_pageItemCount) / 100 * m_previousPagePercentage) + 1);

    // Current size...

    m_currentPageStart = m_currentPageStart;
    m_currentPageEnd = m_currentPageStart + pageItemCount;

    m_nextPageStart = m_currentPageEnd;
    m_nextPageEnd = m_nextPageStart + nextPageItemCount;

    m_previousPageStart = m_currentPageStart - previousPageItemCount;
    m_previousPageEnd = m_currentPageStart;

    // Current size : fix boundaries...
    m_currentPageStart = min( max(m_currentPageStart, m_lowerBoundary), m_upperBoundary);
    m_currentPageEnd = max( min(m_currentPageEnd, m_upperBoundary), m_lowerBoundary);

    m_nextPageStart = min( max(m_nextPageStart, m_lowerBoundary), m_upperBoundary);
    m_nextPageEnd = max( min(m_nextPageEnd, m_upperBoundary), m_lowerBoundary);

    m_previousPageStart = min( max(m_previousPageStart, m_lowerBoundary), m_upperBoundary);
    m_previousPageEnd = max(min(m_previousPageEnd, m_upperBoundary), m_lowerBoundary);

    // Secondary size...
    int currentPageSecondarySizePageSize = static_cast<int>((static_cast<float>(pageItemCount) / 100 * m_secondarySizePercentage) + 1);
    int nextPageSecondarySizePageSize = static_cast<int>((static_cast<float>(currentPageSecondarySizePageSize) / 100 * m_nextPagePercentage) + 1);
    int previousPageSecondarySizePageSize  = static_cast<int>((static_cast<float>(currentPageSecondarySizePageSize) / 100 * m_previousPagePercentage) + 1);

    m_currentPageSecondarySizePageStart = m_currentPageStart; // Pivot.
    m_currentPageSecondarySizePageEnd = m_currentPageSecondarySizePageStart + currentPageSecondarySizePageSize;

    m_nextPageSecondarySizePageStart = m_currentPageSecondarySizePageEnd;
    m_nextPageSecondarySizePageEnd = m_nextPageSecondarySizePageStart + nextPageSecondarySizePageSize;

    m_previousPageSecondarySizePageStart = m_currentPageSecondarySizePageStart - previousPageSecondarySizePageSize;
    m_previousPageSecondarySizePageEnd = m_currentPageSecondarySizePageStart;

    // Secondary size : fix boundaries...
    m_currentPageSecondarySizePageStart = min(max(m_currentPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_currentPageSecondarySizePageEnd = max( min(m_currentPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    m_nextPageSecondarySizePageStart = min( max(m_nextPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_nextPageSecondarySizePageEnd = max( min(m_nextPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    m_previousPageSecondarySizePageStart = min( max(m_previousPageSecondarySizePageStart, m_lowerBoundary), m_upperBoundary);
    m_previousPageSecondarySizePageEnd = max( min(m_previousPageSecondarySizePageEnd, m_upperBoundary), m_lowerBoundary);

    hr = FireLayoutChanged();

    return hr;
}


HRESULT AsyncLoaderLayoutManager::SetCriticalSection(CRITICAL_SECTION* cs)
{
    m_cs = cs;
    return S_OK;
}

HRESULT AsyncLoaderLayoutManager::SetBoundaries(int start, int end)
{
    if ( start > end )
    {
        return E_INVALIDARG;
    }
    CriticalSectionLocker l(m_cs);

    m_lowerBoundary = start;
    m_upperBoundary = end;

    HRESULT hr = S_OK;

    hr = RecalculatePages();

    return S_OK;
}

HRESULT AsyncLoaderLayoutManager::SetCurrentPageItemCount(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_pageItemCount = count;

    HRESULT hr = S_OK;

    hr = RecalculatePages();

    return hr;
}

HRESULT AsyncLoaderLayoutManager::SetCurrentPage(int start)
{
    CriticalSectionLocker l(m_cs);

    m_currentPageStart = start;
    m_currentPageEnd = m_currentPageStart  + m_pageItemCount;

    HRESULT hr = S_OK;

    hr = RecalculatePages();

    return hr;
}

HRESULT AsyncLoaderLayoutManager::SetNextPagePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_nextPagePercentage = percentage;

    hr = RecalculatePages();

    return hr;
}

HRESULT AsyncLoaderLayoutManager::SetPreviousPagePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_previousPagePercentage = percentage;

    hr = RecalculatePages();

    return hr;
}

HRESULT AsyncLoaderLayoutManager::SetSecondarySizePercentage(unsigned int percentage)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_secondarySizePercentage = percentage;

    hr = RecalculatePages();

    return hr;
}

HRESULT AsyncLoaderLayoutManager::RegisterClient(IAsyncLoaderLayoutManagerClient* client)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_clients.push_back(ComPtr<IAsyncLoaderLayoutManagerClient>(client));

    return hr;
}

HRESULT AsyncLoaderLayoutManager::FireLayoutChanged()
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    vector<ComPtr<IAsyncLoaderLayoutManagerClient>>::iterator it;
    for (it = m_clients.begin() ; it < m_clients.end() ; ++it )
    {
        ComPtr<IAsyncLoaderLayoutManagerClient> p = *it;
        p->LayoutChanged();
    }

    return hr;
}


HRESULT AsyncLoaderLayoutManager::IsInPage(int location, LoadPage loadPage, LoadSize loadSize, bool* inPage)
{
    if ( nullptr == inPage )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    *inPage = false;

    int currentPageStart = m_currentPageStart;
    int currentPageEnd = m_currentPageEnd;
    int nextPageStart = m_nextPageStart;
    int nextPageEnd = m_nextPageEnd;
    int previousPageStart = m_previousPageStart;
    int previousPageEnd = m_previousPageEnd;

    HRESULT hr = S_OK;
    int newLocation = location;
    // Based on loadSize, scale location.
    switch(loadSize)
    {
    case AnySize:
    case PrimarySize:
        // No scaling is necessary...
        // No changes for intervals...
        break;
    case SecondarySize:
        newLocation = static_cast<int>((static_cast<float>(location - m_currentPageStart) / 100 * m_secondarySizePercentage) + m_currentPageStart);
        currentPageStart = m_currentPageSecondarySizePageStart;
        currentPageEnd = m_currentPageSecondarySizePageEnd;
        nextPageStart = m_nextPageSecondarySizePageStart;
        nextPageEnd = m_nextPageSecondarySizePageEnd;
        previousPageStart = m_previousPageSecondarySizePageStart;
        previousPageEnd = m_previousPageSecondarySizePageEnd;
        break;
    }

    switch(loadPage)
    {
    case OtherPage:
        if ( (newLocation < previousPageStart) || (nextPageEnd < newLocation) )
        {
            *inPage = true;
        }
        break;
    case CurrentPage:
        if ( (currentPageStart <= newLocation) && (newLocation < currentPageEnd) )
        {
            *inPage = true;
        }
        break;
    case NextPage:
        if ( (nextPageStart <= newLocation) && (newLocation < nextPageEnd) )
        {
            *inPage = true;
        }
        break;
    case PreviousPage:
        if ( (previousPageStart <= newLocation) && (newLocation < previousPageEnd) )
        {
            *inPage = true;
        }
        break;
    }

    return hr;
}


HRESULT AsyncLoaderLayoutManager::GetPageItemCount(LoadPage loadPage, LoadSize loadSize, unsigned int* count)
{
    if ( nullptr == count )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    switch( loadPage )
    {
    case CurrentPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = m_currentPageEnd - m_currentPageStart;
                break;
            case SecondarySize:
                *count = m_currentPageSecondarySizePageEnd - m_currentPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case NextPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = m_nextPageEnd - m_nextPageStart;
                break;
            case SecondarySize:
                *count = m_nextPageSecondarySizePageEnd - m_nextPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case PreviousPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = m_previousPageEnd - m_previousPageStart;
                break;
            case SecondarySize:
                *count = m_previousPageSecondarySizePageEnd - m_previousPageSecondarySizePageStart;
                break;
            }
        }
        break;
    case OtherPage:
        {
            switch( loadSize )
            {
            case AnySize:
            case PrimarySize:
                *count = (m_previousPageStart - m_lowerBoundary) + (m_upperBoundary - m_nextPageEnd);
                break;
            case SecondarySize:
                *count = (m_previousPageSecondarySizePageStart - m_lowerBoundary) + (m_upperBoundary - m_nextPageSecondarySizePageEnd);
                break;
            }
        }
        break;
    }
    return S_OK;
}


HRESULT AsyncLoaderLayoutManager::Shutdown()
{
    CriticalSectionLocker l(m_cs);

    for (auto it = m_clients.begin() ; it < m_clients.end() ; ++it)
    {
        *it = nullptr;
    }

    m_clients.clear();

    return S_OK;
}
