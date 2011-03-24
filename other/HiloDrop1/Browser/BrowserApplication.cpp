//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "BrowserApplication.h"
#include "CarouselPane.h"
#include "MediaPane.h"
#include "resource.h"

using namespace Hilo::WindowApiHelpers;
using namespace Hilo::Direct2DHelpers;

BrowserApplication::BrowserApplication()
{
}

BrowserApplication::~BrowserApplication()
{
}

// Initialize the application layout and message handler
HRESULT BrowserApplication::Initialize()
{
    HRESULT hr = S_OK;

    // Create main window...
    hr = WindowApplication::Initialize();

    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);        
    }

    if (SUCCEEDED(hr))
    {
        static const int MaxTitleLength = 100;
        wchar_t title[MaxTitleLength] = {0}; // The title bar text
        ::LoadString(HINST_THISCOMPONENT, IDS_APP_TITLE, title, MaxTitleLength);
    
        hr = applicationWindow->SetTitle(title);
    }

    if (SUCCEEDED(hr))
    {
        // Set the large icon
        hr = applicationWindow->SetLargeIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_BROWSER)));
    }

    if (SUCCEEDED(hr))
    {
        // Set the small icon
        hr = applicationWindow->SetSmallIcon(::LoadIcon(HINST_THISCOMPONENT, MAKEINTRESOURCE(IDI_SMALL)));
    }

    // Create child windows...
    ComPtr<IWindow> carouselPaneWindow;
    if (SUCCEEDED(hr))
    {
        hr = InitializeCarouselPane(&carouselPaneWindow);
    }

    ComPtr<IWindow> mediaPaneWindow;
    if (SUCCEEDED(hr))
    {
        hr = InitializeMediaPane(&mediaPaneWindow);
    }

    // Connect the carousel pane with the media pane...
    ComPtr<IWindowMessageHandler> mediaPaneHandler;
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneWindow->GetMessageHandler(&mediaPaneHandler);
    }

    ComPtr<IWindowMessageHandler> carouselPaneHandler;
    if (SUCCEEDED(hr))
    {
        hr = carouselPaneWindow->GetMessageHandler(&carouselPaneHandler);
    }

    ComPtr<IPane> mediaPane;
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneHandler.QueryInterface(&mediaPane);
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<ICarouselPaneWindow> carouselPane;
        hr = carouselPaneHandler.QueryInterface(&carouselPane);

        if (SUCCEEDED(hr))
        {
            hr = carouselPane->SetMediaPane(mediaPane);
        }
    }
    
    // Create and initialize window layout...
    if (SUCCEEDED(hr))
    {
        hr = SharedObject<WindowLayout>::Create(&m_WindowLayout);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->SetMainWindow(applicationWindow);
    }
    
    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->InsertChildWindow(carouselPaneWindow, &m_carouselPaneIndex);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->InsertChildWindow(mediaPaneWindow, &m_mediaPaneIndex);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->UpdateLayout();
    }

    // Set the current location...
    if (SUCCEEDED(hr))
    {
        // No location has been defined yet, so we just load the pictures library
        if (nullptr == m_currentBrowseLocationItem) 
        {
            // Defualt to Libraris library
            hr = ::SHCreateItemInKnownFolder(FOLDERID_PicturesLibrary, 0, nullptr, IID_PPV_ARGS(&m_currentBrowseLocationItem));
        }

        // If the Pictures Library was not not found
        if (FAILED(hr))
        {
            // Try obtaining the "Computer" known folder
            hr = ::SHGetKnownFolderItem(FOLDERID_ComputerFolder, static_cast<KNOWN_FOLDER_FLAG>(0), nullptr, IID_PPV_ARGS(&m_currentBrowseLocationItem));
        }
    }

    if (SUCCEEDED(hr))
    {
        ComPtr<IPane> carouselPane;
        hr = carouselPaneHandler.QueryInterface(&carouselPane);

        if (SUCCEEDED(hr))
        {
            hr = carouselPane->SetCurrentLocation(m_currentBrowseLocationItem, false);
        }
    }

    if (SUCCEEDED(hr))
    {
        hr = m_WindowLayout->UpdateLayout();
    }

    return hr;
}

HRESULT BrowserApplication::InitializeCarouselPane(IWindow** window)
{
    static POINT location = {0, 0};
    static SIZE size = 
    {
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(800)), 
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(300))
    };

    HRESULT hr = S_OK;

    ComPtr<IWindowMessageHandler> carouselPaneMessageHandler;
    hr = SharedObject<CarouselPaneMessageHandler>::Create(&carouselPaneMessageHandler);

    if (FAILED(hr))
    {
        return hr;
    }

    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);        
    }

    ComPtr<IWindowFactory> windowFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWindowFactory(&windowFactory);        
    }

    ComPtr<IWindow> carouselPane;

    hr = windowFactory->Create(
        location,
        size,
        carouselPaneMessageHandler,
        applicationWindow,
        &carouselPane);

    if (SUCCEEDED(hr))
    {
        hr = carouselPane->SetMessageHandler(carouselPaneMessageHandler);
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(window, carouselPane);
    }

    return hr;
}

HRESULT BrowserApplication::InitializeMediaPane(IWindow** window)
{
    static POINT location = {0, 0};
    static SIZE size = 
    {
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(800)), 
        static_cast<long>(Direct2DUtility::ScaleValueForCurrentDPI(300))
    };

    HRESULT hr = S_OK;

    ComPtr<IWindowMessageHandler> mediaPaneMessageHandler;
    hr = SharedObject<MediaPaneMessageHandler>::Create(&mediaPaneMessageHandler);
    if (FAILED(hr))
    {
        return hr;
    }

    ComPtr<IWindow> applicationWindow;
    if (SUCCEEDED(hr))
    {
        hr = GetMainWindow(&applicationWindow);        
    }

    ComPtr<IWindowFactory> windowFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWindowFactory(&windowFactory);        
    }

    ComPtr<IWindow> mediaPane;

    hr = windowFactory->Create(
        location,
        size,
        mediaPaneMessageHandler,
        applicationWindow,
        &mediaPane);

    if (SUCCEEDED(hr))
    {
        hr = mediaPane->SetMessageHandler(mediaPaneMessageHandler);
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(window, mediaPane);
    }

    return hr;
}

HRESULT BrowserApplication::OnSize(unsigned int /*width*/, unsigned int /*height*/)
{
    if (nullptr != m_WindowLayout)
    {
        m_WindowLayout->UpdateLayout();
    }
    return S_OK;
}

HRESULT BrowserApplication::OnKeyDown(unsigned int /*vKey*/)
{
    // Make sure to notify children
    return S_FALSE;
}

HRESULT BrowserApplication::OnMouseWheel(POINT_2F /*mousePosition*/, short /*delta*/, int /*keys*/) 
{  
    // Make sure to notify children
    return S_FALSE;
}

HRESULT BrowserApplication::OnDestroy()
{
    m_WindowLayout->Finalize();
    
    return WindowApplication::OnDestroy();
}

HRESULT BrowserApplication::OnRender()
{
    HRESULT hr;
    
    ComPtr<IWindow> mediaPaneWindow;
    ComPtr<IWindowMessageHandler> mediaPaneHandler;
    
    hr = m_WindowLayout->GetChildWindow(m_mediaPaneIndex, &mediaPaneWindow);
    
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneWindow->GetMessageHandler(&mediaPaneHandler);
    }
    
    unsigned int carouselPaneHeight;
    hr = m_WindowLayout->GetChildWindowLayoutHeight(m_carouselPaneIndex, &carouselPaneHeight);

    if (SUCCEEDED(hr))
    {
        if (carouselPaneHeight > 0)
        {
            ComPtr<IWindow> carouselPaneWindow;
            ComPtr<IWindowMessageHandler> carouselPaneHandler;
            
            if (SUCCEEDED(hr))
            {
                hr = m_WindowLayout->GetChildWindow(m_carouselPaneIndex, &carouselPaneWindow);
            }

            if (SUCCEEDED(hr))
            {
                hr = carouselPaneWindow->GetMessageHandler(&carouselPaneHandler);
            }

            ComPtr<IPane> carouselPane;
            if (SUCCEEDED(hr))
            {
                hr = carouselPaneHandler.QueryInterface(&carouselPane);
            }

            if (SUCCEEDED(hr))
            {
                hr = carouselPane->Redraw();
            }
        }
    }

    ComPtr<IPane> mediaPane;
    if (SUCCEEDED(hr))
    {
        hr = mediaPaneHandler.QueryInterface(&mediaPane);
    }

    if (SUCCEEDED(hr))
    {
        hr = mediaPane->Redraw();
    }

    return S_OK;
}

