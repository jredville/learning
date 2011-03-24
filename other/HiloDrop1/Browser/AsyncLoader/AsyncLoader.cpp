//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AsyncLoader.h"
#include "LoadableItemList.h"
#include "AsyncLoaderLayoutManager.h"
#include "AsyncLoaderMemoryManager.h"

using namespace std;
using namespace Hilo::AsyncLoader;

const unsigned int AsyncLoader::PassEnumerator::m_passCount = 9;

AsyncLoader::PassEnumerator::PassEnumerator()
{
}

AsyncLoader::PassEnumerator::~PassEnumerator()
{
}

HRESULT AsyncLoader::PassEnumerator::GetCount(unsigned int* count)
{
    if ( nullptr == count )
    {
        return E_POINTER;
    }
    *count = m_passCount;
    return S_OK;
}

//
// PassEnumerator::GetAt() purpose is to define which page/which size should be loaded
// for a given pass.
//
// In the Hilo application, we want the combinations in the following order:
// 
//                     Other Page  Previous Page  Current Page  Next Page
//  Any Size                                          1
//  Primary Size            9           4             2            3
//  Secondary Size          8           7             5            6
//
// Note that the count and order of transitions changes do not allow us to 
// use nested loops:
//   [1-> 2] : Current Page [ Any Size -> Primary Size]
//   [2-> 3] : [Current Page -> Next Page] Primary size
//
HRESULT AsyncLoader::PassEnumerator::GetAt(unsigned int passIndex, LoadPage* loadPage, LoadSize* loadSize)
{
    if ( (passIndex >= m_passCount) || (nullptr == loadPage) || (nullptr == loadSize) )
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;
    switch( passIndex )
    {
    case 0 : *loadPage = CurrentPage;   *loadSize = AnySize;
        break;
    case 1 : *loadPage = CurrentPage;   *loadSize = PrimarySize;
        break;
    case 2 : *loadPage = NextPage;      *loadSize = PrimarySize;
        break;
    case 3 : *loadPage = PreviousPage;  *loadSize = PrimarySize;
        break;
    case 4 : *loadPage = CurrentPage;   *loadSize = SecondarySize;
        break;
    case 5 : *loadPage = NextPage;      *loadSize = SecondarySize;
        break;
    case 6 : *loadPage = PreviousPage;  *loadSize = SecondarySize;
        break;
    case 7: *loadPage = OtherPage;     *loadSize = SecondarySize;
        break;
    case 8: *loadPage = OtherPage;     *loadSize = PrimarySize;
        break;
    }
    return hr;
}


AsyncLoader::AsyncLoader() : 
    m_commandsQueuedEvent(nullptr),
    m_workerThreadControl(RunThread),
    m_workerThreadHandle(nullptr),
    m_workerThreadId(0),
    m_workerThreadExitCode(0)
{
    memset(&m_cs, 0, sizeof(m_cs));
    InitializeCriticalSection(&m_cs);
}

AsyncLoader::~AsyncLoader()
{
    if ( nullptr != m_commandsQueuedEvent )
    {
        CloseHandle(m_commandsQueuedEvent);
    }

    DeleteCriticalSection(&m_cs);
}

HRESULT AsyncLoader::Initialize()
{
    WCHAR eventSeedString[3];
    static unsigned int eventSeed = 0;
    if ( eventSeed > 99 )
    {
        // To avoid going beyond the allocated buffer.
        return E_FAIL;
    }
    wsprintf(eventSeedString, L"%02d", eventSeed);
    std::wstring eventName = L"CommandsQueuedEvent" + std::wstring(eventSeedString);
    ++eventSeed;

    m_commandsQueuedEvent = CreateEvent(NULL /* default security */ , true /* manual */, true /* signaled */, eventName.c_str());
    if ( NULL == m_commandsQueuedEvent )
    { 
        return E_FAIL;
    }

    HRESULT hr = S_OK;
    unsigned int passCount = 0;
    hr = passEnumerator.GetCount(&passCount);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = SharedObject<LoadableItemList>::Create(&m_loadableItems);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_loadableItems->SetCriticalSection(&m_cs);
    if ( FAILED(hr) )
    {
        return hr;
    }
    m_loadableItems->SetPassCount(passCount);

    // Create layout manager...
    hr = SharedObject<AsyncLoaderLayoutManager>::Create(&m_layoutManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_layoutManager->SetCriticalSection(&m_cs);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_loadableItems->SetLayoutManager(m_layoutManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_layoutManager->RegisterClient(this);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Create memory manager...
    hr = SharedObject<AsyncLoaderMemoryManager>::Create(&m_memoryManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_layoutManager->SetCriticalSection(&m_cs);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_memoryManager->SetLayoutManager(m_layoutManager);
    if ( FAILED(hr) )
    {
        return hr;
    }

    ComPtr<IAsyncLoaderLayoutManagerClient> layoutManagerClient;
    hr = m_memoryManager.QueryInterface(&layoutManagerClient);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_layoutManager->RegisterClient(layoutManagerClient);

    return hr;
}

HRESULT AsyncLoader::ResetEvents()
{
    if ( nullptr == m_commandsQueuedEvent )
    {
        return S_OK;
    }

    // Reset...
    if ( FALSE == ::ResetEvent(m_commandsQueuedEvent))
    {
        return E_FAIL;
    }

    return S_OK;
}

HRESULT AsyncLoader::WaitForEvents()
{
    if ( nullptr == m_commandsQueuedEvent )
    {
        return S_OK;
    }

    // Wait...
    HRESULT hr = S_OK;
    unsigned long waitResult;
    waitResult = WaitForSingleObject( m_commandsQueuedEvent, INFINITE);

    switch (waitResult) 
    {
        // Event object was signaled
        case WAIT_OBJECT_0: 
            break; 
        default: 
            // GetLastError()
            hr = E_FAIL;
    }

    return hr;
}

HRESULT AsyncLoader::MoreEvents()
{
    return (FALSE == ::SetEvent(m_commandsQueuedEvent)) ? E_FAIL : S_OK;
}

HRESULT AsyncLoader::GetCriticalSection(CRITICAL_SECTION** cs)
{
    if ( nullptr == cs )
    {
        return E_POINTER;
    }

    *cs = &m_cs;
    return S_OK;
}


HRESULT AsyncLoader::GetItemsList(IAsyncLoaderItemList** itemList)
{
    if ( !IsOutputPointerValid(itemList) )
    {
        return E_POINTER;
    }

    return AssignToOutputPointer(itemList, m_loadableItems);
}

// Page layout ...
HRESULT AsyncLoader::GetItemLayoutManager(IAsyncLoaderLayoutManager** layoutManager)
{
    if ( !IsOutputPointerValid(layoutManager) )
    {
        return E_POINTER;
    }

    return AssignToOutputPointer(layoutManager, m_layoutManager);
}


HRESULT AsyncLoader::GetMemoryManager(IAsyncLoaderMemoryManager** memoryManager)
{
    if ( !IsOutputPointerValid(memoryManager) )
    {
        return E_POINTER;
    }

    return AssignToOutputPointer(memoryManager, m_memoryManager);
}

// Thread control...

unsigned long WINAPI AsyncLoader::ThreadProc(void* threadData)
{
    AsyncLoader* asyncLoader = reinterpret_cast<AsyncLoader*>(threadData);

    HRESULT hr = S_OK;
    hr = CoInitialize(NULL);
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = asyncLoader->Pump();

    CoUninitialize();
    return hr;
}


HRESULT AsyncLoader::StartWorkerThread()
{
    // Only one thread can be started...
    if ( NULL != m_workerThreadHandle )
    {
        return S_OK;
    }

    m_workerThreadHandle = CreateThread(
                      NULL              // Thread attributes...
                    , 0                 // Stack size. If 0, use default stack size.
                    , ThreadProc
                    , reinterpret_cast<void*>(this) // Pointer to data
                    , 0                 // run immediately after creation
                    , &m_workerThreadId);
    if ( nullptr == m_workerThreadHandle )
    {
        // We should call GetLastError(), and communicate the error.
        return E_FAIL;
    }

    if ( !SetThreadPriority(m_workerThreadHandle, THREAD_PRIORITY_IDLE) )
    {
        return E_FAIL;
    }

    return S_OK;
}
    
HRESULT AsyncLoader::IsWorkingThreadActive(bool* active)
{
    if ( nullptr == active )
    {
        return E_POINTER;
    }

    if ( nullptr == m_workerThreadHandle )
    {
        *active = false;
        return S_OK;
    }

    unsigned long exitCode = 0;
    if ( !GetExitCodeThread(m_workerThreadHandle, &exitCode) )
    {
        return E_FAIL;
    }

    if ( STILL_ACTIVE == exitCode )
    {
        *active = true;
        return S_OK;
    }

    *active = false;
    m_workerThreadExitCode = exitCode;

    return S_OK;
}

HRESULT AsyncLoader::PauseWorkerThread()
{
    HRESULT hr = S_OK;

    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = PauseThread;

    hr = MoreEvents();  // Signal...

    return hr;
}

HRESULT AsyncLoader::ResumeWorkerThread()
{
    HRESULT hr = S_OK;

    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = RunThread;

    hr = MoreEvents();  // Signal...

    return hr;
}

HRESULT AsyncLoader::EndWorkerThread()
{
    HRESULT hr = S_OK;

    // No synchronization here since m_workerThreadControl new value
    // is independent of its previous value.
    m_workerThreadControl = EndThread;

    hr = MoreEvents();  // Signal...

    return hr;
}

HRESULT AsyncLoader::ProcessNextItem(ComPtr<IAsyncLoaderItem> loadableItem, unsigned int pass)
{
    if ( nullptr == loadableItem )
    {
        return E_FAIL;
    }

    HRESULT hr = S_OK;
    LoadPage loadPage = OtherPage;
    LoadSize loadSize = AnySize;

    // Get pass information...
    hr = passEnumerator.GetAt(pass, &loadPage, &loadSize);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Get item location...
    int location = 0;
    hr = loadableItem->GetLocation(&location);
    if ( FAILED(hr) )
    {
        return hr;
    }

    // Is 'this location' in 'this category'?
    //   where 'this category' is (loadPage, loadSize)
    bool inPage = false;
    hr = m_layoutManager->IsInPage(location, loadPage, loadSize, &inPage);
    if ( FAILED(hr) )
    {
        return hr;
    }

    if (!inPage )
    {
        // No action...
        return S_OK;
    }

    if ( OtherPage != loadPage )
    {
        hr = loadableItem->Load(loadPage, loadSize);
    }
    else
    {
        hr = loadableItem->Unload(loadSize);
    }

    return hr;
}

HRESULT AsyncLoader::Pump()
{
    HRESULT hr = S_OK;

    unsigned int debugPass = 0;
    while (  ( EndThread != m_workerThreadControl ) && ( SUCCEEDED(hr) ) )
    {
        switch( m_workerThreadControl )
        {
        case RunThread:
            {
                // Get next item...
                ComPtr<IAsyncLoaderItem> loadableItem;
                unsigned int pass;
                {
                    CriticalSectionLocker l(&m_cs);
                    hr = m_loadableItems->EnumerateNextItem(&loadableItem, &pass);
                    if ( FAILED(hr) )
                    {
                        break;
                    }
                    HRESULT tempHr = S_OK;
                    tempHr = ResetEvents();
                    if ( FAILED(tempHr) )
                    {
                        break;
                    }
                }

                if ( debugPass != pass )
                {
                    debugPass = pass;
                }

                // There was no next item..
                if ( S_FALSE == hr )
                {
                    // Wait...
                    hr = WaitForEvents();
                }
                else
                {
                    // There is a next item...
                    hr = ProcessNextItem(loadableItem, pass);
                }
            }
            break;
        case PauseThread:
            // Reset...
            hr = ResetEvents();
            if ( SUCCEEDED(hr) )
            {
                // Wait...
                hr = WaitForEvents();
            }
            break;
        }
    }

    return hr;
}

HRESULT AsyncLoader::LayoutChanged()
{
    HRESULT hr = S_OK;

    hr = m_loadableItems->ResetEnumeration();
    if ( FAILED(hr) )
    {
        return E_FAIL;
    }

    hr = MoreEvents();

    return hr;
}

HRESULT AsyncLoader::Shutdown()
{
    HRESULT hr = S_OK;

    hr = m_memoryManager->Shutdown();
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_layoutManager->Shutdown();
    if ( FAILED(hr) )
    {
        return hr;
    }

    hr = m_loadableItems->Shutdown();
    if ( FAILED(hr) )
    {
        return hr;
    }

    return hr;
}

