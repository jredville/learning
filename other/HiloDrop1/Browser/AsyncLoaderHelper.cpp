//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AsyncLoaderHelper.h"
#include "AsyncLoader\CriticalSectionLocker.h"

using namespace std;
using namespace Hilo::AsyncLoader;

AsyncLoaderHelper::AsyncLoaderHelper() : 
    m_cs(nullptr),
    m_pageItemCount(10),
    m_pageStart(0),
    m_nextPagePercentage(200),
    m_previousPagePercentage(100),
    m_smallerSizePercentage(120)      // smaller size = more items in the window by %20...
{
}

AsyncLoaderHelper::~AsyncLoaderHelper()
{
    bool active = true;
    do {
        HRESULT hr = S_OK;
        hr = m_asyncLoader->IsWorkingThreadActive(&active);
        if ( FAILED(hr) )
        {
            active = false;
        }
        else
        {
            if (active)
            {
                m_asyncLoader->EndWorkerThread();
                ::Sleep(100);
            }
        }
    } while (active);
}

HRESULT AsyncLoaderHelper::Initialize()
{
    HRESULT hr = E_FAIL;

    hr = SharedObject<AsyncLoader>::Create(&m_asyncLoader);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_asyncLoader->GetCriticalSection(&m_cs);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_asyncLoader->GetItemsList(&m_asyncLoaderItemList);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_asyncLoader->GetItemLayoutManager(&m_asyncLoaderLayoutManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Configure layout manager...
    m_asyncLoaderLayoutManager->SetCurrentPageItemCount((int)m_pageItemCount);
    m_asyncLoaderLayoutManager->SetCurrentPage((int)m_pageStart);


    // Configure the memory manager...
    hr = m_asyncLoader->GetMemoryManager(&m_asyncLoaderMemoryManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Connect the converter...
    hr = SharedObject<MemorySizeConverter>::Create(&m_memorySizeConverter);
    if ( FAILED(hr) )
    {
        return hr;
    }
    hr = m_asyncLoaderMemoryManager->SetMemorySizeConverter(m_memorySizeConverter);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Configure the memory manager...
    m_asyncLoaderMemoryManager->SetMemoryCap(500000000);

    return hr;
}

HRESULT AsyncLoaderHelper::StartBackgroundLoading()
{
    HRESULT hr;

    hr = m_asyncLoader->StartWorkerThread();

    return hr;
}


HRESULT AsyncLoaderHelper::PauseBackgroundLoading()
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;
    hr = m_asyncLoader->PauseWorkerThread();
    return hr;
}

HRESULT AsyncLoaderHelper::ResumeBackgroundLoading()
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;
    hr = m_asyncLoader->ResumeWorkerThread();
    return hr;
}

HRESULT AsyncLoaderHelper::Shutdown()
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;
    hr = m_asyncLoader->EndWorkerThread();
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_asyncLoader->Shutdown();
    if ( FAILED(hr) )
    {
        return hr;
    }

    return hr;
}

HRESULT AsyncLoaderHelper::GetAsyncLoader(IAsyncLoader** p)
{
    if ( !IsOutputPointerValid(p) )
    {
        return E_POINTER;
    }
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;
    hr = AssignToOutputPointer(p, m_asyncLoader);
    if ( FAILED(hr) )
    {
        return hr;
    }

    return hr;
}

HRESULT AsyncLoaderHelper::SetPageItemCount(unsigned int count)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_asyncLoaderLayoutManager->SetCurrentPageItemCount(count);
    return hr;
}


HRESULT AsyncLoaderHelper::SetCurrentPagePivot(unsigned int pivot)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;
    m_pageStart = static_cast<float>(pivot);
    m_asyncLoaderLayoutManager->SetCurrentPage(static_cast<int>(m_pageStart));
    return hr;
}


HRESULT AsyncLoaderHelper::Scroll(int scrollBy)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_pageStart += scrollBy;
    m_asyncLoaderLayoutManager->SetCurrentPage((int)m_pageStart);
    return hr;
}

HRESULT AsyncLoaderHelper::Zoom(float factor)
{
    CriticalSectionLocker l(m_cs);

    HRESULT hr = S_OK;

    m_pageItemCount = m_pageItemCount * factor;

    m_asyncLoaderLayoutManager->SetCurrentPageItemCount(static_cast<unsigned int>(m_pageItemCount));
    return hr;
}


HRESULT AsyncLoaderHelper::ConnectClient(IAsyncLoaderMemoryManagerClient* client)
{
    HRESULT hr = S_OK;

    hr = m_asyncLoaderMemoryManager->RegisterClient(client);

    return hr;
}

HRESULT AsyncLoaderHelper::ConnectItem(IUnknown* newItem, int location)
{
    if ( nullptr == newItem )
    {
        return E_POINTER;
    }
    HRESULT hr = S_OK;
    ComPtr<IAsyncLoaderItem> asyncLoaderItem;

    hr = newItem->QueryInterface(IID_PPV_ARGS(&asyncLoaderItem));
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    hr = asyncLoaderItem->SetMemoryManager(m_asyncLoaderMemoryManager);
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    hr = asyncLoaderItem->SetLocation(location);
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    hr = asyncLoaderItem->SetCriticalSection(m_cs);
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    hr = m_asyncLoaderItemList->Add(asyncLoaderItem);
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    return hr;
}

HRESULT AsyncLoaderHelper::ClearItems()
{
    HRESULT hr = S_OK;

    hr = m_asyncLoaderItemList->Clear();

    return hr;
}
