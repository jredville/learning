//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include <math.h>
#include "AsyncLoaderMemoryManager.h"
#include "CriticalSectionLocker.h"

using namespace std;
using namespace Hilo::AsyncLoader;

const unsigned int AsyncLoaderMemoryManager::PageMatrixColumnCount = 3;
const unsigned int AsyncLoaderMemoryManager::PageMatrixRowCount = 2;
const unsigned int AsyncLoaderMemoryManager::InitialLowerBound = 20;
const unsigned int AsyncLoaderMemoryManager::InitialUpperBound = 260;
const unsigned int AsyncLoaderMemoryManager::InitialLoadSize = 256;

AsyncLoaderMemoryManager::AsyncLoaderMemoryManager() :
    m_seedPivot(0),
    m_seedPivotIndex(0),
    m_lowerBoundary(InitialLowerBound),
    m_upperBoundary(InitialUpperBound),
    m_overlapStepCount(1),
    m_cs(nullptr),
    m_memoryCap(0),
    m_stepRatio(0.1f),
    m_pageItemCount(PageMatrixRowCount, PageMatrixColumnCount),
    m_clientItemSizeToLoad(PageMatrixRowCount, PageMatrixColumnCount),
    // Primary size range and ratios...
    m_primaryStepUpCount(2),
    m_primaryStepDownCount(4),
    m_primaryUpRatio(1),
    m_primaryDownRatio(1),
    // Upper secondary size range and ratios...
    m_secondaryStepUpCountStart(1),
    m_secondaryStepUpCountEnd(2),
    m_secondaryUpRatioStart(1),
    m_secondaryUpRatioEnd(1),
    // Lower secondary size range and ratios...
    m_secondaryStepDownCountStart(2),
    m_secondaryStepDownCountEnd(4),
    m_secondaryDownRatioStart(1),
    m_secondaryDownRatioEnd(1)
{
    UpdateRatios();
}

AsyncLoaderMemoryManager::~AsyncLoaderMemoryManager()
{
}

HRESULT AsyncLoaderMemoryManager::UpdateRatios()
{
    CriticalSectionLocker l(m_cs);

    if ( ( m_secondaryStepUpCountEnd < m_secondaryStepUpCountStart ) ||
        ( m_secondaryStepDownCountEnd < m_secondaryStepDownCountStart ) )
    {
        return E_POINTER;
    }

    m_primaryUpRatio = 1;
    for (unsigned int i = 0 ; i < m_primaryStepUpCount ; ++i )
    {
        m_primaryUpRatio *= (1 + m_stepRatio);
    }
    m_primaryDownRatio = 1;
    for (unsigned int i = 0 ; i < m_primaryStepDownCount ; ++i )
    {
        m_primaryDownRatio /= (1 + m_stepRatio);
    }

    m_secondaryUpRatioStart = 1;
    for (unsigned int i = 0 ; i < m_secondaryStepUpCountStart ; ++i )
    {
        m_secondaryUpRatioStart *= (1 + m_stepRatio);
    }

    m_secondaryUpRatioEnd = 1;
    for (unsigned int i = 0 ; i < m_secondaryStepUpCountEnd ; ++i )
    {
        m_secondaryUpRatioEnd *= (1 + m_stepRatio);
    }

    m_secondaryDownRatioStart = 1;
    for (unsigned int i = 0 ; i < m_secondaryStepDownCountStart ; ++i )
    {
        m_secondaryDownRatioStart /= (1 + m_stepRatio);
    }

    m_secondaryDownRatioEnd = 1;
    for (unsigned int i = 0 ; i < m_secondaryStepDownCountEnd ; ++i )
    {
        m_secondaryDownRatioEnd /= (1 + m_stepRatio);
    }
    
    return ClearTables();
}

HRESULT AsyncLoaderMemoryManager::SetStepRatio(float stepRatio)
{
    CriticalSectionLocker l(m_cs);

    m_stepRatio = stepRatio;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetPrimaryStepUpCount(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_primaryStepUpCount = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetPrimaryStepDownCount(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_primaryStepDownCount = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetSecondaryStepUpCountStart(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_secondaryStepUpCountStart = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetSecondaryStepUpCountEnd(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_secondaryStepUpCountEnd = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetSecondaryStepDownCountStart(unsigned int count)
{
    CriticalSectionLocker l(m_cs);
    
    m_secondaryStepDownCountStart = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::SetSecondaryStepDownCountEnd(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    m_secondaryStepDownCountEnd = count;
    return UpdateRatios();
}

HRESULT AsyncLoaderMemoryManager::ClearTables()
{
    CriticalSectionLocker l(m_cs);

    for ( unsigned int i = 0; i < PageMatrixRowCount ; ++i )
    {
        for ( unsigned int j = 0; j < PageMatrixColumnCount ; ++j )
        {
            m_clientItemSizeToLoad[i][j] = 0;
        }
    }

    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::SetCriticalSection(CRITICAL_SECTION* _cs)
{
    m_cs = _cs;
    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::SetMemorySizeConverter(IMemorySizeConverter* _sizeConverter)
{
    CriticalSectionLocker l(m_cs);

    m_sizeConverter = _sizeConverter;

    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::SetMemoryCap(unsigned int memorySize)
{
    CriticalSectionLocker l(m_cs);

    m_memoryCap = memorySize;    
    return ClearTables();
}

HRESULT AsyncLoaderMemoryManager::AssignItemSizeToPage(unsigned int* availableMemory, LoadPage loadPage, LoadSize loadSize, unsigned int* clientItemSize)
{
    if ( (nullptr == availableMemory) || (nullptr == clientItemSize) )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    // Assume client item size is in the same units as KB.
    unsigned int sizeKB = *clientItemSize;
    
    if ( nullptr != m_sizeConverter )
    {
        // If the converted is defined, do the proper conversion...
        hr = m_sizeConverter->ClientItemSizeToMemorySize(*clientItemSize, &sizeKB);
        if ( FAILED(hr) )
        {
            return hr;
        }
    }

    unsigned int memoryNeeded = 0;
    unsigned int itemCount = 0;

    itemCount = m_pageItemCount[loadSize][loadPage];

    memoryNeeded = itemCount * sizeKB;
    if ( memoryNeeded > (*availableMemory) )
    {
        memoryNeeded = (*availableMemory);
        sizeKB = memoryNeeded / itemCount;
    }
    (*availableMemory) -= memoryNeeded;


    unsigned int tempUserSize = sizeKB;
    if ( nullptr != m_sizeConverter )
    {
        hr = m_sizeConverter->MemorySizeToClientItemSize(sizeKB, &tempUserSize);
        if ( FAILED(hr) )
        {
            return hr;
        }
    }

    m_clientItemSizeToLoad[loadSize][loadPage] = tempUserSize;
    (*clientItemSize) = tempUserSize;

    return hr;
}

HRESULT AsyncLoaderMemoryManager::AssignItemSizesToPageGroup(unsigned int* availableMemory, LoadSize size, unsigned int clientItemSize)
{
    if (nullptr == availableMemory)
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    hr = AssignItemSizeToPage(availableMemory, CurrentPage, size, &clientItemSize);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = AssignItemSizeToPage(availableMemory, NextPage, size, &clientItemSize);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = AssignItemSizeToPage(availableMemory, PreviousPage, size, &clientItemSize);

    return hr;
}

HRESULT AsyncLoaderMemoryManager::PopulatePivots(unsigned int newSeedPivot)
{
    CriticalSectionLocker l(m_cs);

    m_clientItemSizesToLoad.clear();
    m_pivots.clear();

    m_seedPivot = newSeedPivot;

    if ( 0 == m_seedPivot )
    {
        return S_OK;
    }

    m_clientItemSizesToLoad.push_back(m_seedPivot);
    m_pivots.push_back(1);

    unsigned int pivotStep = m_primaryStepUpCount + m_primaryStepDownCount - m_overlapStepCount;
    unsigned int value = 0;
    unsigned int step = 0;

    m_seedPivotIndex = 0;

    value = newSeedPivot;
    step = 0;
    while ( value > m_lowerBoundary )
    {
        value = static_cast<unsigned int>(value / ( 1 + m_stepRatio ));
        m_clientItemSizesToLoad.insert(m_clientItemSizesToLoad.begin(), value);
        ++step;
        m_pivots.insert(m_pivots.begin(), 0 == step % pivotStep);
        ++m_seedPivotIndex;
    }

    value = newSeedPivot;
    step = 0;
    while ( value < m_upperBoundary )
    {
        value = static_cast<unsigned int>(value * ( 1 + m_stepRatio ));
        m_clientItemSizesToLoad.push_back(value);
        ++step;
        m_pivots.push_back(0 == step % pivotStep);
    }

    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::GetUncappedLoadSizes(unsigned int clientItemSize, unsigned int* uncappedPrimaryLoadSize, unsigned int* uncappedSecondaryLoadSize)
{
    if ( (nullptr == uncappedPrimaryLoadSize) || (nullptr == uncappedSecondaryLoadSize) )
    {
        return E_POINTER;
    }

    if ( 0 == m_clientItemSizesToLoad.size() )
    {
        return S_OK;
    }

    CriticalSectionLocker l(m_cs);

    // Find the range...
    bool intervalUpperFound = false;
    unsigned int intervalUpperBoundIndex = 0;
    for (unsigned int i = 0; i < m_clientItemSizesToLoad.size() - 1; ++i )
    {
        if ( clientItemSize < m_clientItemSizesToLoad[i] )
        {
            intervalUpperFound = true;
            intervalUpperBoundIndex = i;
            break;
        }
    }

    if ( !intervalUpperFound )
    {
        *uncappedPrimaryLoadSize = InitialLoadSize;
        *uncappedSecondaryLoadSize = 0;
        return S_OK;
    }

    // Find previous pivot...
    bool previousPivotFound = false;
    unsigned int previousPivotIndex = 0;
    if ( intervalUpperBoundIndex != 0 )
    {
        for (unsigned int i = intervalUpperBoundIndex; i > 0; --i )
        {
            if ( 1 == m_pivots[i] )
            {
                previousPivotFound = true;
                previousPivotIndex = i;
                break;
            }
        }
    }

    // Find next pivot...
    bool nextPivotFound = false;
    unsigned int nextPivotIndex = 0;
    if ( intervalUpperBoundIndex != m_pivots.size() - 1 )
    {
        for (unsigned int i = intervalUpperBoundIndex; i < m_pivots.size(); ++i )
        {
            if ( 1 == m_pivots[i] )
            {
                nextPivotFound = true;
                nextPivotIndex = i;
                break;
            }
        }
    }

    // Working with previous pivot...
    bool inPreviousPivotRange = false;
    if ( previousPivotFound )
    {
        if ( intervalUpperBoundIndex - previousPivotIndex <= m_primaryStepUpCount )
        {
            inPreviousPivotRange = true;
        }
    }

    // Working with next pivot...
    bool inNextPivotRange = false;
    if ( nextPivotFound )
    {
        if ( nextPivotIndex - intervalUpperBoundIndex < m_primaryStepDownCount )
        {
            inNextPivotRange = true;
        }
    }

    unsigned int primaryLoadSize = 0;
    unsigned int secondaryLoadSize = 0;

    if ( (inPreviousPivotRange) && (inNextPivotRange) )
    {
        // common
        primaryLoadSize = m_clientItemSizesToLoad[nextPivotIndex];
        secondaryLoadSize = m_clientItemSizesToLoad[previousPivotIndex];
        if ( primaryLoadSize == secondaryLoadSize )
        {
            secondaryLoadSize = 0;
        }
    }
    else if ( inPreviousPivotRange )
    {
        primaryLoadSize = m_clientItemSizesToLoad[previousPivotIndex];
        secondaryLoadSize = 0;
    }
    else // inNextPivotRange has to be true, or else, it is disjoint.
    {
        primaryLoadSize = m_clientItemSizesToLoad[nextPivotIndex];
        secondaryLoadSize = 0;
    }

    *uncappedPrimaryLoadSize = primaryLoadSize;
    *uncappedSecondaryLoadSize = secondaryLoadSize;

    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::AssignItemSizesToAll(unsigned int clientItemSize)
{
    CriticalSectionLocker l(m_cs);

    if ( (0 != clientItemSize) && (0 == m_seedPivot) )
    {
        m_seedPivot = clientItemSize;
        PopulatePivots(m_seedPivot);
    }

    HRESULT hr = S_OK;
    unsigned int uncappedPrimaryLoadSize = 0;
    unsigned int uncappedSecondaryLoadSize = 0;

    // Get the client item size assuming there is enough memory...
    hr = GetUncappedLoadSizes(clientItemSize, &uncappedPrimaryLoadSize, &uncappedSecondaryLoadSize);
    if ( FAILED(hr) )
    {
        return hr;
    }

    unsigned int availableMemory = m_memoryCap;

    // Recalculate the primary item size based on available memory...
    hr = AssignItemSizesToPageGroup(&availableMemory, PrimarySize, uncappedPrimaryLoadSize);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Recalculate the secondary item size based on available memory...
    hr = AssignItemSizesToPageGroup(&availableMemory, SecondarySize, uncappedSecondaryLoadSize);

    return hr;
}

HRESULT AsyncLoaderMemoryManager::SetLayoutManager(IAsyncLoaderLayoutManager* layoutManager)
{
    if ( nullptr == layoutManager )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    m_layoutManager = layoutManager;

    return S_OK;
}

HRESULT AsyncLoaderMemoryManager::RegisterClient(IAsyncLoaderMemoryManagerClient* client)
{
    if ( nullptr == client )
    {
        return E_POINTER;
    }

    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_client = client;

    return hr;
}

HRESULT AsyncLoaderMemoryManager::GetClientItemSize(LoadPage loadPage, LoadSize loadSize, unsigned int* clientItemSizeToLoad)
{
    if ( nullptr == clientItemSizeToLoad )
    {
        return E_POINTER;
    }

    if ( OtherPage == loadPage )
    {
        return E_FAIL;
    }

    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    if ( AnySize == loadSize )
    {
        loadSize = PrimarySize;
    }

    *clientItemSizeToLoad = m_clientItemSizeToLoad[loadSize][loadPage];

    return hr;
}

HRESULT AsyncLoaderMemoryManager::LayoutChanged()
{
    if ( nullptr == m_layoutManager )
    {
        return E_FAIL;
    }

    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    // Get and cache the item count for each page from the layout manager...
    for (unsigned int size = SecondarySize; size <= PrimarySize ; ++size)
    {
        for (unsigned int page = PreviousPage; page <= NextPage ; ++page )
        {
            unsigned int count = 0;
            hr = m_layoutManager->GetPageItemCount(static_cast<LoadPage>(page), static_cast<LoadSize>(size), &count);
            if ( FAILED(hr) )
            {
                return hr;
            }
            m_pageItemCount[size][page] = count;
        }
    }

    // Get the current client item size...
    unsigned int clientItemSize = 0;
    if ( nullptr != m_client )
    {
        hr = m_client->GetClientItemSize(&clientItemSize);
        if ( FAILED(hr) )
        {
            return hr;
        }
    }

    // Update allocations...
    hr = AssignItemSizesToAll(clientItemSize);

    return hr;
}

HRESULT AsyncLoaderMemoryManager::Shutdown()
{
    m_sizeConverter = nullptr;
    m_layoutManager = nullptr;
    m_client = nullptr;

    return S_OK;
}
