//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "CarouselPane.h"
#include "ShellItemsLoader.h"
#include "WindowLayout.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::AsyncLoader;
using namespace Hilo::Direct2DHelpers;

const float CarouselPaneMessageHandler::ThumbnailWidth = 80;
const float CarouselPaneMessageHandler::ThumbnailHeight = 80;
const float CarouselPaneMessageHandler::HistoryThumbnailWidth = 80;
const float CarouselPaneMessageHandler::HistoryThumbnailHeight = 80;
const float CarouselPaneMessageHandler::CarouselPaneMarginSize = 10;
const float CarouselPaneMessageHandler::OrbitMarginXSize = 60;
const float CarouselPaneMessageHandler::OrbitMarginYSize = 40;
const float CarouselPaneMessageHandler::MaxThumbnailSize = 96;
const int CarouselPaneMessageHandler::MaxHistoryIncrement = 250;
const int CarouselPaneMessageHandler::CarouselSpeedFactor = 500;
const int CarouselPaneMessageHandler::BackgroundColor = 0xB8BEFC;

const double CarouselPaneMessageHandler::InnerOrbitOpacity = 0.6;
const double CarouselPaneMessageHandler::HistoryExpansionTime = 0.25;
const float CarouselPaneMessageHandler::MaxInnerOrbitHeight = 100;
const float CarouselPaneMessageHandler::ThumbnailTextHeight = 36;
const float CarouselPaneMessageHandler::BackButtonWidth = 32;
const float CarouselPaneMessageHandler::BackButtonHeight = 32;
const float CarouselPaneMessageHandler::HistoryThumbnailMarginTop = 20;
const float CarouselPaneMessageHandler::HistoryThumbnailMarginLeft = 20;
const float CarouselPaneMessageHandler::KeyRotateSize = 750;

using namespace Hilo::WindowApiHelpers;

CarouselPaneMessageHandler::CarouselPaneMessageHandler() :
    m_hWnd (nullptr),
    m_isHistoryExpanded(false),
    m_carouselSpinValue(0),
    m_updatingFolder(false),
    m_MouseDirection(None),
    m_isRotationClockwise(false),
    m_mouseDownPoint(Point2F(0, 0))
{
}

CarouselPaneMessageHandler::~CarouselPaneMessageHandler()
{
}

HRESULT CarouselPaneMessageHandler::Initialize()
{
    HRESULT hr = SharedObject<AsyncLoaderHelper>::Create(&m_AsyncLoaderHelper);

    if (SUCCEEDED(hr))
    {
        hr = m_AsyncLoaderHelper->ConnectClient(this);
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::AddHistoryItem(unsigned int carouselItemIndex)
{
    HRESULT hr = S_OK;
    
    if (carouselItemIndex < m_carouselItems.size())
    {
        hr = AddHistoryItem(m_carouselItems[carouselItemIndex]);
    }
    else
    {
        hr = E_INVALIDARG;
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::AddHistoryItem(IThumbnail* item)
{
    HRESULT hr = S_OK;

    // Create new carousel history item
    CarouselHistoryItem newHistoryItem;

    // Initialize thumbnail
    ThumbnailInfo info(nullptr);
    hr = item->GetThumbnailInfo(&info);

    if (SUCCEEDED(hr))
    {
        // Create a new history thumbnail
        hr = SharedObject<CarouselThumbnail>::Create(info, &newHistoryItem.Thumbnail);

        if (FAILED(hr))
        {
            return hr;
        }

        // Set the rendering parameters for this new thumbnail
        hr = newHistoryItem.Thumbnail->SetRenderingParameters(m_renderingParameters);
    }
    
    if (SUCCEEDED(hr))
    {
        // Disable asynchronous loading for history thumbnails
        ComPtr<IAsyncLoaderItem> asyncLoaderItem;
        if (SUCCEEDED(newHistoryItem.Thumbnail.QueryInterface(&asyncLoaderItem)))
        {
            asyncLoaderItem->EnableAsyncLoading(false);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Initialize thumbnail animation
        hr = m_animationFactory->CreateCarouselThumbnailAnimation(&newHistoryItem.ThumbnailAnimation);
    }

    if (SUCCEEDED(hr))
    {
        D2D1_RECT_F currentRect;
        hr = item->GetRect(&currentRect);

        if (SUCCEEDED(hr))
        {
            newHistoryItem.ThumbnailAnimation->Initialize(
                D2D1::Point2F(currentRect.left, currentRect.top), 1);

            newHistoryItem.Thumbnail->SetRect(
                D2D1::RectF(
                0.0f,
                0.0f,
                static_cast<float>(ThumbnailWidth),
                static_cast<float>(ThumbnailHeight)
                )
            );

            // Initialize orbit animation
            hr = m_animationFactory->CreateOrbitAnimation(&newHistoryItem.OrbitAnimation);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Add history item
        m_carouselHistoryItems.push_back(newHistoryItem);

        // Load new orbit items
        hr = SetCurrentLocation(info.shellItem, false);
    }

    if (SUCCEEDED(hr))
    {
        // Kick off animation for adding a history item
        AnimateHistoryAddition();
    }

    return hr;
}

void CarouselPaneMessageHandler::InvalidateParent()
{
    ::InvalidateRect(::GetParent(m_hWnd), NULL, false);
}

HRESULT CarouselPaneMessageHandler::SetCurrentLocation(IShellItem* shellFolder, bool /*recursive*/)
{
    HRESULT hr = S_OK;

    // Clear inner orbit
    RemoveAllItems();

    // Retrieve list of subfolders
    std::vector<ComPtr<IShellItem> > shellItems;

    hr = ShellItemsLoader::EnumerateFolderItems(shellFolder, FileTypeFolder, false, shellItems);

    // Add each subfolder ShellItem to the inner carousel orbit
    if (SUCCEEDED(hr))
    {
        for ( auto ti = shellItems.begin() ; ti < shellItems.end(); ti++ )
        {
            ThumbnailInfo info(*ti);
            info.fileType = FileTypeFolder;

            ComPtr<IThumbnail> thumbnail;
            hr = SharedObject<CarouselThumbnail>::Create(info, &thumbnail);
            
            if (FAILED(hr))
            {
                // We can ignore the creation error for one of the items.
                hr = S_OK;
                continue;
            }

            // Set default bitmap and parameters
            thumbnail->SetDefaultBitmap(m_defaultFolderBitmap);
            thumbnail->SetRenderingParameters(m_renderingParameters);
            thumbnail->SetParentWindow(this);

            // Add thumbnail to inner orbit
            m_carouselItems.push_back(thumbnail);

            // Initialize asynchronous loader
            ComPtr<IUnknown> itemIUnknown;
            hr = thumbnail.QueryInterface(&itemIUnknown);

            if (SUCCEEDED(hr))
            {
                hr = m_AsyncLoaderHelper->ConnectItem(itemIUnknown, static_cast<unsigned int>(m_carouselItems.size()) - 1);
            }

        }
    }

    // Update async loader with number of items
    m_AsyncLoaderHelper->SetPageItemCount(static_cast<unsigned int>(m_carouselItems.size()));
    
    // If this is the initial location, add to history list
    if (m_carouselHistoryItems.size() == 0)
    {
        // Reset orbit values
        ResetOrbitValues();
        
        // Create new carousel history item
        CarouselHistoryItem newHistoryItem;

        // Create animations
        m_animationFactory->CreateOrbitAnimation(&newHistoryItem.OrbitAnimation);
        m_animationFactory->CreateCarouselThumbnailAnimation(&newHistoryItem.ThumbnailAnimation);

        // Initialize history thumbnail
        hr = SharedObject<CarouselThumbnail>::Create(ThumbnailInfo(shellFolder), &(newHistoryItem.Thumbnail));
        if (SUCCEEDED(hr) && (nullptr != newHistoryItem.Thumbnail))
        {
            ComPtr<IAsyncLoaderItem> asyncLoaderItem;
            hr = newHistoryItem.Thumbnail.QueryInterface(&asyncLoaderItem);
            if (SUCCEEDED(hr))
            {
                asyncLoaderItem->EnableAsyncLoading(false);
            }
        }

        if (nullptr != newHistoryItem.Thumbnail)
        {
            newHistoryItem.Thumbnail->SetRenderingParameters(m_renderingParameters);

            // Add history item to carousel history
            m_carouselHistoryItems.push_back(newHistoryItem);
        }

        // Update thumbnail with correct values
        D2D1_POINT_2F point = CalculateHistoryThumbnailPoint(0);

        m_carouselHistoryItems[0].ThumbnailAnimation->Initialize(point, 1);

        m_carouselHistoryItems[0].Thumbnail->SetRect(
            D2D1::RectF(
            0,
            0,
            static_cast<float>(ThumbnailWidth),
            static_cast<float>(ThumbnailHeight)
            )
            );

        // Initialize orbit animation
        m_carouselHistoryItems[0].OrbitAnimation->Initialize(CalculateHistoryOrbit(0), 0.4);
    }

    // Update the current location for the media pane
    if (m_mediaPane)
    {
        hr = m_mediaPane->SetCurrentLocation(shellFolder, false);
    }

    m_updatingFolder = true;
    
    if (SUCCEEDED(hr))
    {
        hr = AnimateNewFolderSet();
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::OnCreate()
{
    HRESULT hr = CreateDeviceIndependentResources();

    if (SUCCEEDED(hr))
    {
        hr = CreateDeviceResources();
    }

    return hr;
}

// Called whenever the application needs to display the client window. 
HRESULT CarouselPaneMessageHandler::Redraw()
{
    HRESULT hr = S_OK;

    // Update the animation manager with the current time
    hr = AnimationUtility::UpdateAnimationManagerTime();

    // Continue redrawing as long as there are animations scheduled
    bool isBusy;

    if (SUCCEEDED(AnimationUtility::IsAnimationManagerBusy(&isBusy)))
    {
        if (isBusy)
        {
            InvalidateParent();
        }
    }
    
    if (SUCCEEDED(hr))
    {
        // Draw the client area
        hr = DrawClientArea();
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::DrawClientArea()
{
    HRESULT hr = CreateDeviceResources();

    if (SUCCEEDED(hr) && !(m_renderTarget->CheckWindowState() & D2D1_WINDOW_STATE_OCCLUDED))
    {
        // Size of the current render target
        D2D1_SIZE_F size = m_renderTarget->GetSize();

        // Begin drawing
        m_renderTarget->BeginDraw();
        m_renderTarget->SetTransform(Matrix3x2F::Identity());
        m_renderTarget->FillRectangle(RectF(0, 0, size.width, size.height), m_backgroundLinearGradientBrush);
        
        // Draw back button
        if (m_carouselHistoryItems.size() > 1 && m_isHistoryExpanded == false)
        {
            m_renderTarget->DrawBitmap(m_arrowBitmap, m_backButtonRect);
        }

        // Draw the carousel pane
        DrawHistoryItems();
        DrawOrbitItems();

        // End Direct2D rendering
        hr = m_renderTarget->EndDraw();

        if (hr == D2DERR_RECREATE_TARGET)
        {
            hr = S_OK;
            DiscardDeviceResources();
        }
    }

    return hr;
}

void CarouselPaneMessageHandler::DrawHistoryItems()
{
    HRESULT hr = S_OK;
    
    // Draw all history orbits
    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
    {
        // Render orbit
        D2D1_ELLIPSE orbit;
        double opacity;

        hr = iter->OrbitAnimation->GetEllipse(&orbit, &opacity);

        if (SUCCEEDED(hr))
        {
            m_radialGradientBrush->SetCenter(orbit.point);
            m_radialGradientBrush->SetRadiusX(orbit.radiusX);
            m_radialGradientBrush->SetRadiusY(orbit.radiusY);
            m_radialGradientBrush->SetOpacity(static_cast<float>(opacity));

            m_renderTarget->FillEllipse(orbit, m_radialGradientBrush);
        }
    }

    // Draw all history thumbnails
    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
    {
        // Render thumbnail
        D2D1_POINT_2F point;
        double opacity;

        hr = iter->ThumbnailAnimation->GetInfo(&point, &opacity);

        if (SUCCEEDED(hr))
        {
            iter->Thumbnail->SetRect(
                D2D1::RectF(
                point.x,
                point.y,
                point.x + HistoryThumbnailWidth,
                point.y + HistoryThumbnailHeight
                )
                );

            iter->Thumbnail->SetFontSize(14);
            iter->Thumbnail->SetOpacity(static_cast<float>(opacity));
            iter->Thumbnail->Draw();
        }
    }
}

void CarouselPaneMessageHandler::DrawOrbitItems()
{
    // Read animation variables
    double rotationLocation = 0;
    double orbitIconsOpacity = 1;
    double orbitIconsScale = 1;

    m_carouselAnimation->GetInfo(&rotationLocation, &orbitIconsScale, &orbitIconsOpacity);

    D2D1_ELLIPSE innerOrbit;
    double opacity;

    m_innerOrbitAnimation->GetEllipse(&innerOrbit, &opacity);

    // Draw inside orbit
    if (m_carouselItems.size() > 0)
    {
        m_radialGradientBrush->SetCenter(innerOrbit.point);
        m_radialGradientBrush->SetRadiusX(innerOrbit.radiusX);
        m_radialGradientBrush->SetRadiusY(innerOrbit.radiusY);
        m_radialGradientBrush->SetOpacity(static_cast<float>(opacity));

        m_renderTarget->FillEllipse(innerOrbit, m_radialGradientBrush);
    }

    // Placement of icons on the orbit are equally spaced (for now)
    double increment = (2 * PI) / m_carouselItems.size();

    // Draw icons on orbit in two passes. The first pass draws the icons
    // in the back. The second pass draws icons in the front.
    for (int pass = 0; pass < 2; pass++)
    {
        // Keep the orignal value of rotation
        double currentRotation = rotationLocation;

        if (GetCapture() == m_hWnd && m_isHistoryExpanded == false)
        {
            if (m_carouselSpinValue != 0)
            {
                UpdateCarouselLocation(rotationLocation + 0.006 * m_carouselSpinValue);
                m_carouselSpinValue = 0;
            }
        }

        for (auto iter = m_carouselItems.begin(); iter < m_carouselItems.end(); iter++)
        {
            // Determine when to draw based on relative value of y
            if ((sin(currentRotation) > 0 && pass == 0) ||
                (sin(currentRotation) <= 0 && pass == 1))
            {
                // Get point to draw current orbit
                D2D1_POINT_2F point = CalculatePointAtAngle(&innerOrbit, currentRotation);

                // Calculate scaling based on relative value of y
                float scale = 1.25f - 0.5f * ((1.0f + static_cast<float>(sin(currentRotation))) * 0.5f);
                scale *= static_cast<float>(orbitIconsScale);

                D2D1_RECT_F rect_f;
                rect_f.left = point.x - (ThumbnailWidth * scale * 0.5f);
                rect_f.right = point.x + (ThumbnailWidth * scale * 0.5f);
                rect_f.top = point.y - (ThumbnailHeight * scale) + (ThumbnailHeight - ThumbnailWidth);
                rect_f.bottom = rect_f.top + (ThumbnailHeight * scale);

                // Draw the carousel thumbnail
                (*iter)->SetFontSize(12 * scale);
                (*iter)->SetOpacity(static_cast<float>(orbitIconsOpacity));
                (*iter)->SetRect(rect_f);
                (*iter)->Draw();
            }

            // Increment location to next spot
            currentRotation += increment;
        }
    }
}

D2D1_ELLIPSE CarouselPaneMessageHandler::CalculateInnerOrbit()
{
    D2D1_POINT_2F center;
    float radiusX;
    float radiusY;

    float maxRadiusX = (m_renderTarget->GetSize().width / 2) - OrbitMarginXSize * 2;
    
    radiusX = min(maxRadiusX, max(1, m_carouselItems.size() - 1) * ThumbnailWidth * 0.60f);
    radiusY = min(MaxInnerOrbitHeight, max(1, m_carouselItems.size()- 1) * ThumbnailWidth * 0.20f);
    
    if (m_isHistoryExpanded)
    {
        center.x = m_renderTarget->GetSize().width * (0.875f) - CarouselPaneMarginSize;
        center.y = m_renderTarget->GetSize().height * (0.875f) - CarouselPaneMarginSize;

        radiusX = radiusX / 3;
        radiusY = radiusY / 3;
    }
    else
    {
        center.x = m_renderTarget->GetSize().width / 2;
        center.y = 10 + ThumbnailHeight + radiusY;
    }

    return D2D1::Ellipse(center, radiusX, radiusY);
}

D2D1_ELLIPSE CarouselPaneMessageHandler::CalculateHistoryOrbit(unsigned int index)
{
    D2D1_POINT_2F center;
    float radiusX;
    float radiusY;

    if (m_isHistoryExpanded)
    {
        // Center is bottom right corner of render target
        center.x = m_renderTarget->GetSize().width;
        center.y = m_renderTarget->GetSize().height;

        D2D_POINT_2F point = CalculateHistoryThumbnailPoint(index);

        double a = center.x - point.x;
        double b = center.y - point.y;
        double c = sqrt( a*a + b*b );

        double angle = PI - a / c;

        radiusX = static_cast<float>(abs(a / cos(angle)));
        radiusY = static_cast<float>(abs(b / sin(angle)));

        center.x += ThumbnailWidth * 0.5f;
        center.y += ThumbnailHeight * 0.5f;
    }
    else
    {
        radiusX = m_renderTarget->GetSize().width / 1.0f;
        radiusY = m_renderTarget->GetSize().height / 1.5f;

        center.x = m_renderTarget->GetSize().width / 2;
        center.y = 30 + radiusY - 10 * (m_carouselHistoryItems.size() - index - 1);
    }

    return D2D1::Ellipse(center, radiusX, radiusY);
}

double CarouselPaneMessageHandler::CalculateHistoryOrbitOpacity(unsigned int index)
{
    return (m_isHistoryExpanded) ? 0.6 : min(1.0, max(0.4 - 0.1 * (m_carouselHistoryItems.size() - index), 0.0));
}

double CarouselPaneMessageHandler::CalculateHistoryThumbnailOpacity(unsigned int index)
{
    return (m_isHistoryExpanded) ? 1 : min(1, max(1 - 0.33 * (m_carouselHistoryItems.size() - index - 1) , 0));
}

D2D1_POINT_2F CarouselPaneMessageHandler::CalculateHistoryThumbnailPoint(unsigned int index)
{
    D2D1_POINT_2F point;

    if (m_isHistoryExpanded)
    {
        // Defines the line that the icons occupy
        D2D_POINT_2F point1;
        D2D_POINT_2F point2;

        // Calculate top left point
        point1.x = ThumbnailWidth * 0.5f + CarouselPaneMarginSize;
        point1.y = ThumbnailHeight * 0.5f + CarouselPaneMarginSize;

        point2.x = m_renderTarget->GetSize().width * 0.75f - CarouselPaneMarginSize - ThumbnailWidth * 0.5f;
        point2.y = m_renderTarget->GetSize().height * 0.875f - CarouselPaneMarginSize - ThumbnailHeight * 0.5f;

        // Calculate increment
        float incrementX = (point2.x - point1.x) / (m_carouselHistoryItems.size() - 1.0f);
        float incrementY = (point2.y - point1.y) / (m_carouselHistoryItems.size() - 1.0f);

        if (incrementX > MaxHistoryIncrement || incrementY > MaxHistoryIncrement)
        {
            float scale = 0.0;

            if (incrementX > MaxHistoryIncrement)
            {
                scale = MaxHistoryIncrement / incrementX;
            }
            else
            {
                scale = MaxHistoryIncrement / incrementY;
            }

            incrementX *= scale;
            incrementY *= scale;
        }

        point.x = point1.x + incrementX * index - static_cast<float>(ThumbnailWidth) * 0.5f;
        point.y = point1.y + incrementY * index - static_cast<float>(ThumbnailHeight) * 0.5f;
    }
    else
    {
        point.x = HistoryThumbnailMarginLeft - 7 * static_cast<float>(m_carouselHistoryItems.size() - index - 1);
        point.y = HistoryThumbnailMarginTop - 7 * static_cast<float>(m_carouselHistoryItems.size() - index - 1);
    }

    return point;
}

D2D1_POINT_2F CarouselPaneMessageHandler::CalculatePointAtAngle(D2D1_ELLIPSE* ellipse, double angle)
{
    return D2D1::Point2F(
        static_cast<float>(ellipse->point.x + ellipse->radiusX * cos(angle)),
        static_cast<float>(ellipse->point.y - ellipse->radiusY * sin(angle))
        );
}

// If the application receives a WM_SIZE message.
HRESULT CarouselPaneMessageHandler::OnSize(unsigned int width, unsigned int height)
{
    HRESULT hr = S_FALSE;

    if (m_renderTarget)
    {
        D2D1_SIZE_U size = {width,height};
        hr = m_renderTarget->Resize(size);

        if (SUCCEEDED(hr))
        {
            if (m_updatingFolder == false)
            {
                hr = ResetOrbitValues();
            }
            else
            {
                m_updatingFolder = false;
            }
        }
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::OnEraseBackground()
{
    // To prevent flickering, returning Success would indicate
    // this message has been handled
    return S_OK;
}

HRESULT CarouselPaneMessageHandler::OnLeftMouseButtonDown(POINT_2F mousePosition)
{
    HRESULT hr;
    
    // Clear queue of mouse points
    m_mouseMovePoints.clear();

    // Reset carousel spin value
    m_carouselSpinValue = 0;

    // Store mouse down point for later use
    m_mouseDownPoint = mousePosition;
    
    // Queue that tracks mouse points for inertia
    MouseMoveInfo info;
    info.point = m_mouseDownPoint;
    
    // Get time from animation utiity
    hr = AnimationUtility::GetAnimationTimerTime(&info.time);

    if (SUCCEEDED(hr))
    {
        m_mouseMovePoints.push_back(info);
    }

    // Capture mouse focus
    SetCapture(m_hWnd);

    return S_OK;
}

HRESULT CarouselPaneMessageHandler::OnLeftMouseButtonUp(POINT_2F mousePosition)
{
    HRESULT hr = S_OK;
    bool clickProcessed = false;

    // Release mouse capture
    ReleaseCapture();

    // Clear mouse direction
    m_MouseDirection = None;

    // Deterine which item was clicked
    if (!m_isHistoryExpanded)
    {
        ClearMouseHover();
        
        if (m_mouseMovePoints.size() < 3)
        {
            for (auto iter = m_carouselItems.begin(); iter < m_carouselItems.end(); iter++)
            {
                D2D1_RECT_F rect;
                hr = ((*iter)->GetRect(&rect));

                if (SUCCEEDED(hr))
                {
                    if (Direct2DUtility::HitTest(rect, mousePosition))
                    {
                        // Check if this item was the target of MouseDown
                        if (Direct2DUtility::HitTest(rect, m_mouseDownPoint))
                        {
                            // Add item to history
                            ClearMouseHover();
                            AddHistoryItem(*iter);
                            clickProcessed = true;
                            break;
                        }
                    }
                }
            }
        }

        if (clickProcessed == false && m_mouseMovePoints.size() > 0)
        {
            UI_ANIMATION_SECONDS endTime;
            hr = AnimationUtility::GetAnimationTimerTime(&endTime);
            
            if (SUCCEEDED(hr))
            {
                double distance = fabs(mousePosition.x - m_mouseMovePoints.front().point.x);
                double time = endTime - m_mouseMovePoints.front().time;
                double speed = distance / time;

                if (!m_isRotationClockwise)
                {
                    speed *= -1;
                }
                
                RotateCarousel( min(2000, max(-2000, speed)));
            }
        }
    }

    if (!clickProcessed && m_carouselHistoryItems.size() > 1)
    {
        unsigned int index = 0;

        for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
        {
            D2D1_RECT_F rect;
            hr = (*iter).Thumbnail->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    if (m_isHistoryExpanded)
                    {
                        NavigateToHistoryItem(index);
                    }

                    ClearMouseHover();
                    AnimateHistoryExpansion();
                    clickProcessed = true;

                    break;
                }
            }

            index++;
        }
    }

    if (!clickProcessed)
    {
        if (m_isHistoryExpanded)
        {
            // Check if user clicked on inner orbit
            D2D1_ELLIPSE orbit;
            hr = m_innerOrbitAnimation->GetEllipse(&orbit, nullptr);

            if (SUCCEEDED(hr))
            {
                // Check if user clicked in inner orbit
                if (mousePosition.x >= orbit.point.x - orbit.radiusX &&
                    mousePosition.x <= orbit.point.x + orbit.radiusX &&
                    mousePosition.y >= orbit.point.y - orbit.radiusY &&
                    mousePosition.y <= orbit.point.y + orbit.radiusY)
                {
                    hr = AnimateHistoryExpansion();
                }
            }
        }
        else 
        {
            if (mousePosition.x >= m_backButtonRect.left &&
                mousePosition.x <= m_backButtonRect.right &&
                mousePosition.y >= m_backButtonRect.top &&
                mousePosition.y <= m_backButtonRect.bottom)
            {
                hr = NavigateBack();
            }
        }
    }

    return S_OK;
}

HRESULT CarouselPaneMessageHandler::OnMouseMove(POINT_2F mousePosition)
{
    HRESULT hr = S_OK;

    // Check if mouse is captured
    if (GetCapture() == m_hWnd)
    {
        // Clear mouse hover
        ClearMouseHover();
        
        // Allows the carousel to spin during next render
        m_carouselSpinValue = mousePosition.x - m_mouseMovePoints.back().point.x;

        // Limit size of deque of mouse move information
        if (m_mouseMovePoints.size() > 3)
        {
            m_mouseMovePoints.pop_front();
        }
        
        // Check for change in direction and update direction of rotation
        switch(m_MouseDirection)
        {
            case Left:
            {
                // Indicates mouse is no longer moving left
                if (m_carouselSpinValue > 0)
                {
                    m_MouseDirection = Right;
                    m_isRotationClockwise = mousePosition.y < CalculateInnerOrbit().point.y;
                }

                break;
            }
            
            case Right:
            {
                // Indicates mouse is no longer moving right
                if (m_carouselSpinValue < 0)
                {
                    m_MouseDirection = Left;
                    m_isRotationClockwise = mousePosition.y > CalculateInnerOrbit().point.y;
                }

                break;
            }

            default:
            {
                if (m_carouselSpinValue > 0)
                {
                    m_MouseDirection = Right;
                    m_isRotationClockwise = mousePosition.y < CalculateInnerOrbit().point.y;
                }
                else
                {
                    m_MouseDirection = Left;
                    m_isRotationClockwise = mousePosition.y > CalculateInnerOrbit().point.y;
                }
            }
        }


        if (m_isRotationClockwise)
        {
            m_carouselSpinValue = fabs(m_carouselSpinValue) * -1;
        }
        else
        {
            m_carouselSpinValue = fabs(m_carouselSpinValue);
        }

        // Store time and mouse information for intertia calculation
        MouseMoveInfo info;
        info.point = mousePosition;
        hr = AnimationUtility::GetAnimationTimerTime(&info.time);

        if (SUCCEEDED(hr))
        {
            m_mouseMovePoints.push_back(info);
        }

        InvalidateParent();
    }
    else
    {
        CheckForMouseHover(mousePosition);
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::OnMouseWheel(POINT_2F /*mousePosition*/, short delta, int /*keys*/) 
{
    return RotateCarousel(delta * 5);
}

HRESULT CarouselPaneMessageHandler::OnKeyDown(unsigned int vKey)
{
    HRESULT hr = S_OK;

    if (vKey == VK_LEFT)
    {
        hr = RotateCarousel(KeyRotateSize);
    }
    else if (vKey == VK_RIGHT)
    {
        hr = RotateCarousel(-KeyRotateSize);
    }
    else if (vKey == VK_BACK)
    {
        if (m_carouselHistoryItems.size() > 1)
        {
            // Navigate to penultimate item in the history
            NavigateBack();
        }
    }

    return hr;
}

//
//  This method creates resources which are bound to a particular
//  Direct2D render target. It's all centralized here, in case the resources
//  need to be recreated in case of Direct3D device loss (eg. display
//  change, remoting, removal of video card, etc).
//
HRESULT CarouselPaneMessageHandler::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    if (nullptr == m_renderTarget)
    {
        ComPtr<IWindow> window;
        GetWindow(&window);
        hr = window->GetWindowHandle(&m_hWnd);

        RECT rect;
        GetClientRect(m_hWnd, &rect);

        D2D1_SIZE_U size = SizeU(rect.right, rect.bottom);

        // Create a Direct2D render target
        hr = m_d2dFactory->CreateHwndRenderTarget(
            RenderTargetProperties(),
            HwndRenderTargetProperties(m_hWnd, size),
            &m_renderTarget
            );

        if (SUCCEEDED(hr))
        {
            // Load back button image
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"BackArrowImage",
                L"PNG",
                0,
                0,
                &m_arrowBitmap
                );
        }

        if (SUCCEEDED(hr))
        {
            // Load default thumbnail image...
            hr = Direct2DUtility::LoadBitmapFromResource(
                m_renderTarget,
                L"DefaultFolderImage",
                L"PNG",
                0,
                0,
                &m_defaultFolderBitmap
                );
        }

        if (SUCCEEDED(hr))
        {
            // Create gradient brush for background
            ComPtr<ID2D1GradientStopCollection> gradientStopCollection;
            D2D1_GRADIENT_STOP gradientStops[3];

            if (SUCCEEDED(hr))
            {
                gradientStops[0].color = ColorF(BackgroundColor);
                gradientStops[0].position = 0.0f;
                gradientStops[1].color = ColorF(ColorF::White);
                gradientStops[1].position = 0.25f;
                
            }

            if (SUCCEEDED(hr))
            {

                hr = m_renderTarget->CreateGradientStopCollection(
                    gradientStops,
                    2,
                    D2D1_GAMMA_2_2,
                    D2D1_EXTEND_MODE_CLAMP,
                    &gradientStopCollection
                    );
            };

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateLinearGradientBrush(
                    LinearGradientBrushProperties(
                    Point2F(m_renderTarget->GetSize().width, 0),
                    Point2F(m_renderTarget->GetSize().width, m_renderTarget->GetSize().height)),
                    gradientStopCollection,
                    &m_backgroundLinearGradientBrush
                    );
            }
            
            gradientStopCollection = nullptr;

            // Create gradient brush for drawing orbital rings
            gradientStops[0].color = D2D1::ColorF(D2D1::ColorF::White, 1.0f);
            gradientStops[0].position = 0.7f;
            gradientStops[1].color = D2D1::ColorF(D2D1::ColorF::LightBlue, 1.0f);
            gradientStops[1].position = 0.95f;
            gradientStops[2].color = D2D1::ColorF(D2D1::ColorF::White, 1.0f);
            gradientStops[2].position = 1.0f;

            hr = m_renderTarget->CreateGradientStopCollection(
                gradientStops,
                3,
                D2D1_GAMMA_2_2,
                D2D1_EXTEND_MODE_CLAMP,
                &gradientStopCollection
                );

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateRadialGradientBrush(
                    D2D1::RadialGradientBrushProperties(
                    D2D1::Point2F(0, 0),
                    D2D1::Point2F(0, 0),
                    200,
                    200
                    ),
                    gradientStopCollection,
                    &m_radialGradientBrush
                    );
            }

            if (SUCCEEDED(hr))
            {
                hr = m_renderTarget->CreateSolidColorBrush(
                    D2D1::ColorF(D2D1::ColorF::Black),
                    &m_fontBrush
                    );
            }

            if (SUCCEEDED(hr))
            {
                // Set rendering parameters
                m_renderingParameters.renderTarget = m_renderTarget;
                m_renderingParameters.solidBrush = m_fontBrush;
                m_renderingParameters.textFormat = m_textFormat;
            }

            if (SUCCEEDED(hr))
            {
                hr = m_AsyncLoaderHelper->StartBackgroundLoading();
            }
        }
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::DiscardDeviceResources()
{
    HRESULT hr = S_OK;
    
    for (auto iter = m_carouselItems.begin(); iter < m_carouselItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;
        
        if (SUCCEEDED(hr))
        {
            hr = (*iter).QueryInterface(&thumbnail);
        }
        
        thumbnail->DiscardResources();
    }

    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;
        
        if (SUCCEEDED(hr))
        {
            hr = (iter)->Thumbnail.QueryInterface(&thumbnail);
        }
        
        if (SUCCEEDED(hr))
        {
            thumbnail->DiscardResources();
        }
    }

    m_arrowBitmap = nullptr;
    m_fontBrush = nullptr;
    m_backgroundLinearGradientBrush = nullptr;
    m_radialGradientBrush = nullptr;
    m_renderTarget = nullptr;

    return S_OK;
}

HRESULT CarouselPaneMessageHandler::CreateDeviceIndependentResources()
{
    HRESULT hr = S_OK;

    hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);

    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    }

    // Create text format
    if (SUCCEEDED(hr))
    {
        hr = m_dWriteFactory->CreateTextFormat(
            L"Arial",
            nullptr,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            12,
            L"en-us",
            &m_textFormat
            );
    }

    // Initialize animation controllers
    if (SUCCEEDED(hr))
    {
        hr =  CreateAnimationFactory(&m_animationFactory);
    }

    if (SUCCEEDED(hr))
    {
        m_animationFactory->CreateOrbitAnimation(&m_innerOrbitAnimation);
    }

    if (SUCCEEDED(hr))
    {
        m_animationFactory->CreateCarouselAnimation(&m_carouselAnimation);
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::ResetOrbitValues()
{
    HRESULT hr = S_OK;
    
    // Update inner orbit location
    hr = m_innerOrbitAnimation->Setup(CalculateInnerOrbit(), InnerOrbitOpacity, 0);

    if (SUCCEEDED(hr))
    {
        // Update history orbits and thumbnails
        unsigned int index = 0;

        for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++, index++)
        {
            if (SUCCEEDED(hr))
            {
                D2D1_ELLIPSE orbit = CalculateHistoryOrbit(index);
                double opacity = CalculateHistoryOrbitOpacity(index);
                hr = iter->OrbitAnimation->Setup(orbit, opacity, 0);
            }

            if (SUCCEEDED(hr))
            {
                D2D1_POINT_2F point = CalculateHistoryThumbnailPoint(index);
                double opacity = CalculateHistoryThumbnailOpacity(index);
                hr = iter->ThumbnailAnimation->Setup(point, opacity, 0);
            }
        }

        InvalidateParent();
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::AnimateHistoryAddition()
{
    HRESULT hr = S_OK;

    // Offset for orbit animation to begin
    const UI_ANIMATION_SECONDS totalTime  = 0.5;

    // Animate each outside history orbit
    unsigned int index = 0;

    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++, index++)
    {
        if (index < m_carouselHistoryItems.size() - 1)
        {
            if (SUCCEEDED(hr))
            {
                // Update orbit
                hr = iter->OrbitAnimation->Setup(
                    CalculateHistoryOrbit(index),
                    CalculateHistoryOrbitOpacity(index),
                    totalTime
                    );
            }

            // Update thumbnail
            if (SUCCEEDED(hr))
            {
                hr = iter->ThumbnailAnimation->Setup(
                    CalculateHistoryThumbnailPoint(index),
                    CalculateHistoryThumbnailOpacity(index),
                    totalTime
                    );
            }
        }
        else
        {
            // Get starting inner orbit for history
            D2D1_ELLIPSE startOrbit;
            D2D1_ELLIPSE targetOrbit = CalculateHistoryOrbit(index);

            hr = m_innerOrbitAnimation->GetEllipse(&startOrbit, nullptr);

            if (SUCCEEDED(hr))
            {
                // Set history orbit at current inner orbit
                hr = iter->OrbitAnimation->Initialize(startOrbit, InnerOrbitOpacity);
            }

            if (SUCCEEDED(hr))
            {
                // Animate history orbit to final location
                hr = iter->OrbitAnimation->Setup(targetOrbit, CalculateHistoryOrbitOpacity(index), totalTime);
            }

            if (SUCCEEDED(hr))
            {
                // Animate history thumbnail to correct location
                D2D1_POINT_2F targetPoint = CalculateHistoryThumbnailPoint(index);
                D2D1_POINT_2F keyFramePoint;

                keyFramePoint.x = startOrbit.point.x + startOrbit.radiusX - ThumbnailWidth;
                keyFramePoint.y = startOrbit.point.y - ThumbnailHeight;

                m_carouselHistoryItems.back().ThumbnailAnimation->Setup(
                    keyFramePoint,
                    targetPoint,
                    totalTime);
            }
        }
    }

    if (SUCCEEDED(hr))
    {
        D2D1_ELLIPSE targetOrbit = CalculateInnerOrbit();
        
        // Start inner orbit at center and animate to normal spot
        hr = m_innerOrbitAnimation->Setup(D2D1::Ellipse(targetOrbit.point, 0, 0), InnerOrbitOpacity, 0);

        if (SUCCEEDED(hr))
        {
            hr = m_innerOrbitAnimation->Setup(targetOrbit, InnerOrbitOpacity, totalTime);
        }

        if (SUCCEEDED(hr))
        {
            // Update opacity of icons on inner orbit
            hr = m_carouselAnimation->SetupOpacity(0, 0);
            hr = m_carouselAnimation->SetupOpacity(1.0, totalTime);
        }

        if (SUCCEEDED(hr))
        {
            // Shrink icons on inner orbit
            hr = m_carouselAnimation->SetupScale(0.5, 0);
            hr = m_carouselAnimation->SetupScale(1.0, totalTime);
        }
    }

    // Force redraw
    InvalidateParent();

    return hr;
}

HRESULT CarouselPaneMessageHandler::AnimateHistoryExpansion()
{
    HRESULT hr = S_OK;

    // Toggle expansion
    m_isHistoryExpanded = !m_isHistoryExpanded;

    // Animate each outside history orbit
    unsigned int index = 0;

    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++, index++)
    {
        if (SUCCEEDED(hr))
        {
            // Update orbit
            hr = iter->OrbitAnimation->Setup(
                CalculateHistoryOrbit(index),
                CalculateHistoryOrbitOpacity(index),
                HistoryExpansionTime);
        }

        if (SUCCEEDED(hr))
        {
            // Update target point
            hr = iter->ThumbnailAnimation->Setup(
                CalculateHistoryThumbnailPoint(index),
                CalculateHistoryThumbnailOpacity(index),
                HistoryExpansionTime);
        }
    }

    if (SUCCEEDED(hr))
    {
        // Move inner orbit to bottom-right corner
        hr = m_innerOrbitAnimation->Setup(CalculateInnerOrbit(), InnerOrbitOpacity, HistoryExpansionTime);
    }

    if (SUCCEEDED(hr))
    {
        // Update opacity of icons on inner orbit
        hr = m_carouselAnimation->SetupOpacity(m_isHistoryExpanded ? 0.6 : 1.0, HistoryExpansionTime);
    }

    if (SUCCEEDED(hr))
    {
        // Shrink icons on inner orbit
        hr = m_carouselAnimation->SetupScale(m_isHistoryExpanded ? 0.5 : 1.0, HistoryExpansionTime);
    }

    // Force redraw
    InvalidateParent();

    return hr;
}

HRESULT CarouselPaneMessageHandler::AnimateNewFolderSet()
{
    // For now we are simply snapping the carousel pane to a neutral position
    HRESULT hr = S_OK;

    double targetRotation = 0;
    
    if (m_carouselItems.size() > 0)
    {
        if (m_carouselItems.size() == 1)
        {
            targetRotation = 5 * PI / 4;
        }
        else
        {
            targetRotation = ((3 * PI) / 2) - (PI / m_carouselItems.size());
        }

        if (m_carouselItems.size() == 4)
        {
            targetRotation += PI / 6;
        }
    }
    
    hr = m_carouselAnimation->SetupRotation(targetRotation, 0);

    if (SUCCEEDED(hr))
    {
        // Calculate location of back button
        if (m_carouselHistoryItems.size() > 0)
        {
            m_backButtonRect.left = HistoryThumbnailMarginLeft + (ThumbnailWidth / 2) - (BackButtonWidth / 2);
            m_backButtonRect.top = CarouselPaneMarginSize + ThumbnailHeight + ThumbnailTextHeight + 2;
            m_backButtonRect.right = m_backButtonRect.left + BackButtonWidth;
            m_backButtonRect.bottom = m_backButtonRect.top + BackButtonHeight;
        }
    }
    
    if (SUCCEEDED(hr))
    {
        float minWindowHeight = Direct2DUtility::ScaleValueForCurrentDPI(m_backButtonRect.bottom + m_carouselHistoryItems.size());
        
        if (m_carouselItems.size() == 0)
        {
            m_windowLayout->SetChildWindowLayoutHeight(
                0, 
                static_cast<unsigned int>(minWindowHeight)
                );
        }
        else
        {
            D2D1_ELLIPSE innerOrbit;

            if (m_isHistoryExpanded)
            {
                m_isHistoryExpanded = false;
                innerOrbit = CalculateInnerOrbit();
                m_isHistoryExpanded = true;
            }
            else
            {
                innerOrbit = CalculateInnerOrbit();
            }
            
            float height = Direct2DUtility::ScaleValueForCurrentDPI(innerOrbit.point.y + innerOrbit.radiusY + ThumbnailTextHeight);
            height = max(minWindowHeight, height);

            m_windowLayout->SetChildWindowLayoutHeight(
                0,
                static_cast<unsigned int>(height)
                );
        }

        m_windowLayout->UpdateLayout();
    }

    return hr;

}

HRESULT CarouselPaneMessageHandler::RotateCarousel(double speed)
{
    HRESULT hr;

    // Get current value of transition and geometry length
    double currentRotation;
    hr = m_carouselAnimation->GetInfo(&currentRotation, nullptr, nullptr);

    if (SUCCEEDED(hr))
    {
        // Rotate counter-clockwise
        currentRotation -= PI * (speed / CarouselSpeedFactor);

        hr = m_carouselAnimation->SetupRotation(
            currentRotation,
            max(speed / CarouselSpeedFactor, 3.0f)
            );
    }

    InvalidateParent();

    return hr;
}

HRESULT CarouselPaneMessageHandler::UpdateCarouselLocation(double rotationLocation)
{
    return m_carouselAnimation->SetupRotation(rotationLocation, 0);
}

HRESULT CarouselPaneMessageHandler::NavigateBack()
{
    HRESULT hr = S_OK;
    
    if (m_carouselHistoryItems.size() > 1)
    {
        D2D1_ELLIPSE startOrbit = CalculateHistoryOrbit(static_cast<unsigned int>(m_carouselHistoryItems.size() - 1));
        
        // Navigate to penultimate item in history
        hr = NavigateToHistoryItem(static_cast<unsigned int>(m_carouselHistoryItems.size() - 2));

        if (SUCCEEDED(hr))
        {
            // Custom animations just for navigating back
            D2D1_ELLIPSE newOrbit = CalculateInnerOrbit();
            newOrbit.radiusX = 0;
            newOrbit.radiusY = 0;

            hr = m_innerOrbitAnimation->Setup(startOrbit, InnerOrbitOpacity, 0);
        }

        if (SUCCEEDED(hr))
        {
            // Update opacity of icons on inner orbit
            hr = m_carouselAnimation->SetupOpacity(0, 0);
        }

        if (SUCCEEDED(hr))
        {
            // Shrink icons on inner orbit
            hr = m_carouselAnimation->SetupScale(0.5, 0);
        }
        
        if (SUCCEEDED(hr))
        {
            // Force history to not be expanded
            m_isHistoryExpanded = true;
            
            // Start expansion animation
            hr = AnimateHistoryExpansion();
        }

        if (SUCCEEDED(hr))
        {
            // Make sure nothing is highlighted
            hr = ClearMouseHover();
        }
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::NavigateToHistoryItem(unsigned int historyItemIndex)
{
    HRESULT hr = S_OK;
    
    if (historyItemIndex < m_carouselHistoryItems.size() - 1)
    {
        ThumbnailInfo info;
        hr = m_carouselHistoryItems[historyItemIndex].Thumbnail->GetThumbnailInfo(&info);

        if (SUCCEEDED(hr))
        {
            hr = SetCurrentLocation(info.shellItem, false);
        }
    }

    // Update history stack
    while (m_carouselHistoryItems.size() > historyItemIndex + 1)
    {
        m_carouselHistoryItems.pop_back();
    }

    return S_OK;
}

HRESULT CarouselPaneMessageHandler::CheckForMouseHover(POINT_2F mousePosition)
{
    bool iconFound = false;
    bool needsRedraw = false;

    HRESULT hr = S_OK;

    // Deterine if any inner orbit item has mouse focus
    for (auto iter = m_carouselItems.begin(); iter < m_carouselItems.end(); iter++)
    {
        ThumbnailSelectionState selectionState;
        (*iter)->GetSelectionState(&selectionState);

        if (iconFound || m_isHistoryExpanded)
        {
            // Only allow one item to be selected
            if (selectionState == SelectionStateHoverOn)
            {
                (*iter)->SetSelectionState(SelectionStateNone);
                needsRedraw = true;
            }
        }
        else
        {
            D2D1_RECT_F rect;
            hr = (*iter)->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    iconFound = true;

                    if (selectionState == SelectionStateNone)
                    {
                        (*iter)->SetSelectionState(SelectionStateHoverOn);
                        needsRedraw = true;
                    }
                }
                else if (selectionState == SelectionStateHoverOn)
                {
                    (*iter)->SetSelectionState(SelectionStateNone);
                    needsRedraw = true;
                }
            }
        }
    }

    // Determine if any history item has mouse focus
    for ( auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
    {
        ThumbnailSelectionState selectionState = SelectionStateNone;
        
        if (SUCCEEDED(hr))
        {
            iter->Thumbnail->GetSelectionState(&selectionState);
        }

        if (needsRedraw || !m_isHistoryExpanded)
        {
            // Only allow one item to be selected
            if (selectionState == SelectionStateHoverOn)
            {
                iter->Thumbnail->SetSelectionState(SelectionStateNone);
                needsRedraw = true;
            }
        }
        else
        {
            D2D1_RECT_F rect;
            hr = iter->Thumbnail->GetRect(&rect);

            if (SUCCEEDED(hr))
            {
                if (Direct2DUtility::HitTest(rect, mousePosition))
                {
                    if (selectionState == SelectionStateNone)
                    {
                        iter->Thumbnail->SetSelectionState(SelectionStateHoverOn);
                        needsRedraw = true;
                    }
                }
                else if (selectionState == SelectionStateHoverOn)
                {
                    iter->Thumbnail->SetSelectionState(SelectionStateNone);
                    needsRedraw = true;
                }
            }
        }
    }

    // Redraw
    if (needsRedraw)
    {
        InvalidateParent();
    }

    return hr;
}

HRESULT CarouselPaneMessageHandler::RemoveAllItems()
{
    HRESULT hr = S_OK;

    m_AsyncLoaderHelper->ClearItems();

    m_carouselItems.clear();

    return hr;
}

HRESULT CarouselPaneMessageHandler::ClearMouseHover()
{
    HRESULT hr = S_OK;
    
    for ( auto iter = m_carouselItems.begin(); iter < m_carouselItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;
        
        if (SUCCEEDED(hr))
        {
            hr = (*iter).QueryInterface(&thumbnail);
        }
        
        thumbnail->SetSelectionState(SelectionStateNone);
    }

    for (auto iter = m_carouselHistoryItems.begin(); iter < m_carouselHistoryItems.end(); iter++)
    {
        ComPtr<IThumbnail> thumbnail;
        
        if (SUCCEEDED(hr))
        {
            hr = iter->Thumbnail.QueryInterface(&thumbnail);
        }
        
        if (SUCCEEDED(hr))
        {
            hr = thumbnail->SetSelectionState(SelectionStateNone);
        }
    }

    return hr;
}

// IAsyncLoaderMemoryManagerClient
HRESULT CarouselPaneMessageHandler::GetClientItemSize(unsigned int* clientItemSize)
{
    if ( nullptr == clientItemSize )
    {
        return E_POINTER;
    }
    HRESULT hr = S_OK;

    // Locking is not important for this resource.
    *clientItemSize = static_cast<unsigned int>(ThumbnailWidth);

    return hr;
}

// IChildNotificationHandler
HRESULT CarouselPaneMessageHandler::OnChildChanged() 
{ 
    InvalidateParent();
    return S_OK;
}

HRESULT CarouselPaneMessageHandler::Finalize()
{
    HRESULT hr = S_OK;

    // Remove reference to dependent objects
    m_windowLayout = nullptr;
    m_mediaPane = nullptr;
    
    // Clear all itms from inner orbit and corresponding asynchronous loader
    hr = RemoveAllItems();

    if (FAILED(hr))
    {
        return hr;
    }

    // Shutdown the asynchronous loader
    hr = m_AsyncLoaderHelper->Shutdown();

    return hr;
}

HRESULT CarouselPaneMessageHandler::SetWindowLayout(IWindowLayout* layout)
{
    m_windowLayout = layout;

    return S_OK;
}

HRESULT CarouselPaneMessageHandler::SetMediaPane(IPane* mediaPane)
{
    m_mediaPane = mediaPane;

    return S_OK;
}
