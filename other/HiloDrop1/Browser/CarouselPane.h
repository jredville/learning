//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "CarouselThumbnail.h"
#include "Animation.h"
#include "AnimationFactory.h"
#include "AsyncLoaderHelper.h"
#include "WindowLayout.h"
#include "WindowLayoutChildInterface.h"
#include "CarouselPaneWindowInterface.h"
#include "WindowMessageHandlerImpl.h"

struct CarouselHistoryItem
{
    ComPtr<IThumbnail> Thumbnail;
    ComPtr<ICarouselThumbnailAnimation> ThumbnailAnimation;
    ComPtr<IOrbitAnimation> OrbitAnimation;
};

struct MouseMoveInfo
{
    POINT_2F point;
    UI_ANIMATION_SECONDS time;
};

enum MouseMoveDirection
{
    None = 0,
    Left = 1,
    Right = 2
};

class CarouselPaneMessageHandler :   
    public IPane,
    public IWindowLayoutChild,
    public ICarouselPaneWindow,
    public Hilo::WindowApiHelpers::IChildNotificationHandler,
    public Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient,
    public IInitializable,
    public Hilo::WindowApiHelpers::WindowMessageHandler
{
public:

    // Constants
    static const float MaxThumbnailSize;
    static const float KeyRotateSize;

    // IPane implementation
    HRESULT __stdcall Redraw();
    HRESULT __stdcall SetCurrentLocation(IShellItem* shellFolder, bool recursive);

    // IWindowLayoutChild implementation
    HRESULT __stdcall SetWindowLayout(IWindowLayout* layout);
    HRESULT __stdcall Finalize();

    // ICarouselPane implementation
    HRESULT __stdcall SetMediaPane(IPane* mediaPane);

    // IChildNotificationHandler
    HRESULT __stdcall OnChildChanged();

    // IAsyncLoaderMemoryManagerClient
    HRESULT __stdcall GetClientItemSize(unsigned int* clientItemSize);

protected:
    // Contructors/Destructor
    CarouselPaneMessageHandler();
    virtual ~CarouselPaneMessageHandler();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return 
            CastHelper<IPane>::CastTo(iid, this, object) ||
            CastHelper<IWindowLayoutChild>::CastTo(iid, this, object) ||
            CastHelper<ICarouselPaneWindow>::CastTo(iid, this, object) ||
            CastHelper<Hilo::WindowApiHelpers::IChildNotificationHandler>::CastTo(iid, this, object) ||
            CastHelper<Hilo::AsyncLoader::IAsyncLoaderMemoryManagerClient>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object) ||
            Hilo::WindowApiHelpers::WindowMessageHandler::QueryInterfaceHelper(iid, object);
    }

    // IInitiliazable
    HRESULT __stdcall Initialize();

    // WindowMessageHandler Events
    HRESULT OnCreate();
    HRESULT OnEraseBackground();
    HRESULT OnSize(unsigned int width, unsigned int height);
    HRESULT OnLeftMouseButtonDown(POINT_2F mousePosition);
    HRESULT OnLeftMouseButtonUp(POINT_2F mousePosition);
    HRESULT OnMouseMove(POINT_2F mousePosition);
    HRESULT OnMouseWheel(POINT_2F mousePosition, short delta, int keys);
    HRESULT OnKeyDown(unsigned int vKey);

private:
    // Constants
    static const float ThumbnailWidth;
    static const float ThumbnailHeight;
    static const float HistoryThumbnailWidth;
    static const float HistoryThumbnailHeight;
    static const float CarouselPaneMarginSize;
    static const float OrbitMarginXSize;
    static const float OrbitMarginYSize;
    static const int MaxHistoryIncrement;
    static const int CarouselSpeedFactor;
    static const int BackgroundColor;

    static const double InnerOrbitOpacity;
    static const double HistoryExpansionTime;
    
    static const float BackButtonWidth;
    static const float BackButtonHeight;
    static const float HistoryThumbnailMarginTop;
    static const float HistoryThumbnailMarginLeft;
    static const float MaxInnerOrbitHeight;
    static const float ThumbnailTextHeight;

    // Asynchronous loading support
    ComPtr<Hilo::AsyncLoader::IAsyncLoaderHelper> m_AsyncLoaderHelper;

    // Window information
    HWND m_hWnd;
    
    MouseMoveDirection m_MouseDirection;
    bool m_isRotationClockwise;
    POINT_2F m_mouseDownPoint;
    float m_carouselSpinValue;
    std::deque<MouseMoveInfo> m_mouseMovePoints;

    // Animation variables
    ComPtr<IAnimationFactory> m_animationFactory;
    ComPtr<IOrbitAnimation> m_innerOrbitAnimation;
    ComPtr<ICarouselAnimation> m_carouselAnimation;
    
    // Window state/objects
    ComPtr<IWindowLayout> m_windowLayout;
    bool m_isHistoryExpanded;
    bool m_updatingFolder;
    D2D1_RECT_F m_backButtonRect;

    // Geometry / Direct2d render resources
    ComPtr<ID2D1Factory> m_d2dFactory;
    ComPtr<IDWriteFactory> m_dWriteFactory;
    ComPtr<ID2D1HwndRenderTarget> m_renderTarget;
    ComPtr<ID2D1RadialGradientBrush> m_radialGradientBrush;
    ComPtr<ID2D1LinearGradientBrush> m_backgroundLinearGradientBrush;
    ComPtr<ID2D1SolidColorBrush> m_fontBrush;
    ComPtr<IDWriteTextFormat> m_textFormat; 
    ComPtr<ID2D1Bitmap> m_defaultFolderBitmap;
    ComPtr<ID2D1Bitmap> m_arrowBitmap;

    RenderingParameters m_renderingParameters;
    
    // Carousel Items
    std::vector<ComPtr<IThumbnail> > m_carouselItems;
    std::vector<CarouselHistoryItem> m_carouselHistoryItems;

    // Media Pane
    ComPtr<IPane> m_mediaPane;
    
    // Methods

    // Item list maintenance
    HRESULT RemoveAllItems();

    // Animation methods
    HRESULT AnimateHistoryAddition();
    HRESULT AnimateHistoryExpansion();
    HRESULT AnimateNewFolderSet();
    HRESULT RotateCarousel(double speed);
    HRESULT UpdateCarouselLocation(double rotationLocation);
    HRESULT NavigateBack();
    HRESULT NavigateToHistoryItem(unsigned int historyItemIndex);
    
    // Direct2D methods
    HRESULT CreateDeviceResources();
    HRESULT CreateDeviceIndependentResources();
    HRESULT DiscardDeviceResources();
    
    // Rendering methods
    HRESULT DrawClientArea();
    void DrawHistoryItems();
    void DrawOrbitItems();

    HRESULT ResetOrbitValues();

    // Calculations
    D2D1_ELLIPSE CalculateInnerOrbit();
    D2D1_ELLIPSE CalculateHistoryOrbit(unsigned int index);
    double CalculateHistoryOrbitOpacity(unsigned int index);
    double CalculateHistoryThumbnailOpacity(unsigned int index);
    D2D1_POINT_2F CalculateHistoryThumbnailPoint(unsigned int index);
    D2D1_POINT_2F CalculatePointAtAngle(D2D1_ELLIPSE* ellipse, double angle);

    // Carousel add/remove methods
    HRESULT AddOrbitItem(IThumbnail* item);
    HRESULT AddHistoryItem(IThumbnail* item);
    HRESULT AddHistoryItem(unsigned int orbitItemIndex);
    void InvalidateParent();

    // Mouse methods
    HRESULT CheckForMouseHover(POINT_2F mousePosition);
    HRESULT ClearMouseHover();

};
