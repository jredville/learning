//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "PaneInterface.h"
#include "ImageThumbnailControl.h"
#include "WindowMessageHandlerImpl.h"
#include "AsyncLoaderHelper.h"
#include "ImageThumbnailControl.h"
#include "ThumbnailsLayoutInterface.h"
#include "WindowLayout.h"
#include "WindowLayoutChildInterface.h"
#include "WindowMessageHandlerImpl.h"

class WindowLayout;

class MediaPaneMessageHandler : public IPane,
                                public IWindowLayoutChild,
                                public Hilo::WindowApiHelpers::IChildNotificationHandler,
                                public Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient,
                                public IInitializable,
                                public Hilo::WindowApiHelpers::WindowMessageHandler
{
private:
    ComPtr<Hilo::AsyncLoader::IAsyncLoaderHelper> m_AsyncLoaderHelper;

    // Thumnails layout manager
    ComPtr<IThumbnailsLayoutManager> m_thumbnailsLayoutManager;

    // Hanlder of media pane window
    HWND m_hWnd;

    // Arrow related variables
    bool m_leftArrowSelected;
    bool m_rightArrowSelected;
    bool m_leftArrowClicked;
    bool m_rightArrowClicked;

    // Slide show related variables
    bool m_isSlideShowMode;
    int  m_firstSlideShowCell;

    // Variables
    bool m_enableAnimation;
    bool m_updatingFolder;

    // The window layout management class
    ComPtr<IWindowLayout> m_windowLayout;

    // Factories
    ComPtr<ID2D1Factory> m_d2dFactory;
    ComPtr<IDWriteFactory> m_dWriteFactory;

    // Direct2D resources
    ComPtr<ID2D1HwndRenderTarget> m_renderTarget;
    ComPtr<ID2D1LinearGradientBrush> m_backgroundLinearGradientBrush;
    ComPtr<ID2D1SolidColorBrush> m_solidBrush;
    ComPtr<ID2D1Bitmap> m_arrowBitmap;
    ComPtr<ID2D1Bitmap> m_defaultThumbnailBitmap;
    RenderingParameters m_renderingParameters;

    // DirectWrite resources
    ComPtr<IDWriteTextFormat> m_textFormat;
    ComPtr<IDWriteTextLayout> m_textLayout;

    // Thumbnails
    ComPtr<IThumbnail> m_slideShowThumbnail;
    std::vector<ComPtr<IThumbnail> > m_thumbnailControls;

    // Unique animation controller
    ComPtr<IMediaPaneAnimation> m_animationController;
    ComPtr<IAnimationFactory> m_animationFactory;

    // Track the current animation type
    AnimationType m_currentAnimation;

private:
    // constants 
    static const FLOAT FontSizeRatio;

    // Clear thumbnail controls
    HRESULT RemoveAllItems();

    // Create/Descard D2D resources
    HRESULT CreateDeviceIndependentResources();
    HRESULT CreateDeviceResources();
    HRESULT DiscardDeviceResources();
    void PrepareBackgroundBrush(D2D1_SIZE_F size);

    // Layout Operations
    void CreateThumbnailCells(D2D1_SIZE_F size);
    void UpdateTextLayout();
    void RearrangeCells(bool isZoomOut);

    // Slide Show
    void ToogleSlideShow(bool isSlideShow);

    // Basic Scroll/Zoom operations
    bool MoveToCell(unsigned int cell);
    bool PreviousPage();
    bool NextPage();
    bool ZoomIn();
    bool ZoomOut();

    // Draw operations
    void DrawArrows();
    void DrawClientArea();
    void InvalidateParent();

    // Animation operations:
    void DrawAnimatedThumbnailCells();
    HRESULT RenderScrollingAnimation(bool nextPage);
    HRESULT RenderAnimation();
    HRESULT SetupAnimation(AnimationType animationType, D2D1_SIZE_F size);

    // Helper functions:
    float GetThumbnailSize();
    unsigned int GetCurrentPageIndex();
    unsigned int GetCurrentPageThumbnailCount();
    unsigned int GetMaxPagesCount();
    float GetFontSize();

protected:
    // Events
    HRESULT OnEraseBackground();
    HRESULT OnSize(unsigned int width, unsigned int height);
    HRESULT OnKeyDown(unsigned int vKey);
    HRESULT OnLeftMouseButtonDown(POINT_2F mousePos);
    HRESULT OnLeftMouseButtonUp(POINT_2F mousePosition);
    HRESULT OnLeftMouseButtonDoubleClick(POINT_2F mousePos);
    HRESULT OnMouseMove(POINT_2F mousePosition);
    HRESULT OnMouseWheel(POINT_2F mousePosition, short delta, int keys);
    HRESULT OnAppCommandBrowserBackward();
    HRESULT OnAppCommandBrowserForward();


    MediaPaneMessageHandler();
    virtual ~MediaPaneMessageHandler();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return 
            CastHelper<IPane>::CastTo(iid, this, object) ||
            CastHelper<IWindowLayoutChild>::CastTo(iid, this, object) ||
            CastHelper<IChildNotificationHandler>::CastTo(iid, this, object) ||
            CastHelper<Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Hilo::WindowApiHelpers::WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

    HRESULT __stdcall Initialize();
public:    
    // Constants
    static const int BackgroundColor = 0xB8BEFC;

    // IPane
    HRESULT __stdcall Redraw();
    HRESULT __stdcall SetCurrentLocation(IShellItem* shellFolder, bool recursive);

    // IWindowLayoutChild
    HRESULT __stdcall SetWindowLayout(IWindowLayout* layout)
    {
        m_windowLayout = layout;

        return S_OK;
    }
    HRESULT __stdcall Finalize();

    // IChildNotificationHandler
    HRESULT __stdcall OnChildChanged() ;

    // IAsyncLoaderMemoryManagerClient
    HRESULT __stdcall GetClientItemSize(unsigned int* clientItemSize);


};
