//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "MediaPane.h"
#include "ShellItemsLoader.h"
#include "WindowLayout.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::AsyncLoader;
using namespace Hilo::Direct2DHelpers;
using namespace Hilo::WindowApiHelpers;

const FLOAT MediaPaneMessageHandler::FontSizeRatio = 0.1f;

MediaPaneMessageHandler::MediaPaneMessageHandler() : 
    m_hWnd(nullptr), m_windowLayout(nullptr),
    m_leftArrowSelected(false), m_rightArrowSelected(false), 
    m_enableAnimation(true), m_currentAnimation(FlyIn),
    m_isSlideShowMode(false), m_updatingFolder(false)
{
}

 MediaPaneMessageHandler::~MediaPaneMessageHandler()
{
}

HRESULT MediaPaneMessageHandler::Initialize()
{
    HRESULT hr = CreateDeviceIndependentResources();

    if (SUCCEEDED(hr))
    {
        hr = SharedObject<AsyncLoaderHelper>::Create(&m_AsyncLoaderHelper);
    }

    if (SUCCEEDED(hr))
    {
        hr = SharedObject<ThumbnailsLayoutManager>::Create(&m_thumbnailsLayoutManager);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_AsyncLoaderHelper->ConnectClient(this);
    }

    return hr;
}

//  This method creates resources which are bound to a particular 
//  Direct2D render target. 
HRESULT MediaPaneMessageHandler::CreateDeviceResources()
{
    HRESULT hr = S_OK;

    if (nullptr == m_renderTarget)
    {
        RECT rect;

        ComPtr<IWindow> window;

        GetWindow(&window);
        window->GetWindowHandle(&m_hWnd);
        GetClientRect(m_hWnd, &rect);

        D2D1_SIZE_U size = SizeU(
            rect.right - rect.left,
            rect.bottom - rect.top
            );

        // Create a Direct2D render target.
        hr = m_d2dFactory->CreateHwndRenderTarget(
            RenderTargetProperties(),
            HwndRenderTargetProperties(m_hWnd, size),
            &m_renderTarget
            );

        D2D1_GRADIENT_STOP gradientStops[2];
        if (SUCCEEDED(hr))
        {
            gradientStops[0].color = ColorF(ColorF::White);
            gradientStops[0].position = 0.75f;
            gradientStops[1].color = ColorF(BackgroundColor);
            gradientStops[1].position = 1;
        }

        ComPtr<ID2D1GradientStopCollection> gradientStopCollection;
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

        D2D1_SIZE_F renderTargetSize = m_renderTarget->GetSize();

        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateLinearGradientBrush(
                LinearGradientBrushProperties(
                Point2F(renderTargetSize.width, 0),
                Point2F(renderTargetSize.width, renderTargetSize.height)),
                gradientStopCollection,
                &m_backgroundLinearGradientBrush
                );        
        }

        if (SUCCEEDED(hr))
        {
            hr = m_renderTarget->CreateSolidColorBrush(
                ColorF(ColorF::Black),
                &m_solidBrush);
        }

        if (SUCCEEDED(hr))
        {
            hr = Direct2DUtility::LoadBitmapFromResource(m_renderTarget, L"ArrowImage", L"PNG", 0, 0, &m_arrowBitmap);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_thumbnailsLayoutManager->SetArrowSize(m_arrowBitmap->GetSize());
        }


        if (SUCCEEDED(hr))
        {
            hr = Direct2DUtility::LoadBitmapFromResource(m_renderTarget, L"DefaultThumbNailImage", L"PNG", 0, 0, &m_defaultThumbnailBitmap);
        }

        if (SUCCEEDED(hr))    
        {
            CreateThumbnailCells(renderTargetSize);
        }

        if (SUCCEEDED(hr))    
        {
            m_thumbnailsLayoutManager->CalculateArrowPositions(renderTargetSize);
        }

        if (SUCCEEDED(hr))
        {
            m_renderingParameters.solidBrush = m_solidBrush;
            m_renderingParameters.renderTarget = m_renderTarget;
        }

        if (SUCCEEDED(hr))
        {
            // Make sure rendering parameters are updated in all thumbnails, 
            // if they've already been created
            for ( auto thumbnail = m_thumbnailControls.begin() ; thumbnail < m_thumbnailControls.end(); thumbnail++ )
            {
                (*thumbnail)->SetRenderingParameters(m_renderingParameters);
            }
        }

        if (SUCCEEDED(hr) )
        {
            hr = m_AsyncLoaderHelper->StartBackgroundLoading();
        }

        // Schedule animation
        if (SUCCEEDED(hr))
        {
            SetupAnimation (FlyIn, renderTargetSize);
        }
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::DiscardDeviceResources()
{
    for (auto thumbnail = m_thumbnailControls.begin(); thumbnail < m_thumbnailControls.end(); thumbnail++)
    {
        (*thumbnail)->DiscardResources();
    }

    m_arrowBitmap = nullptr;
    m_solidBrush = nullptr;
    m_backgroundLinearGradientBrush = nullptr;
    m_renderTarget = nullptr;

    return S_OK;
}

HRESULT MediaPaneMessageHandler::CreateDeviceIndependentResources()
{
    static const WCHAR initialText [] = L"Some Static Arbitary Text Here. This will be used as a file title placeholder to determine the text height.";

    HRESULT hr = S_OK;

    hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);

    if (SUCCEEDED(hr))
    {
        hr = Direct2DUtility::GetDWriteFactory(&m_dWriteFactory);
    }

    if (SUCCEEDED(hr))
    {
        // Create text format
        hr = m_dWriteFactory->CreateTextFormat(
            L"Arial",
            nullptr, // Use Windows fonts
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            10, // An arbitary font size that'll be overriden by the textLayout
            L"en-us",
            &m_textFormat
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create an arbitary text layout object
        // Width and height will be determined later.
        hr = m_dWriteFactory->CreateTextLayout(
            initialText,
            ARRAYSIZE(initialText) - 1,
            m_textFormat,
            50,
            50,
            &m_textLayout
            );
    }

    if (SUCCEEDED(hr))
    {
        m_textLayout->SetWordWrapping(DWRITE_WORD_WRAPPING_WRAP);
    }

    if (SUCCEEDED(hr))
    {
        m_textLayout->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_NEAR);
    }

    if (SUCCEEDED(hr))
    {
        m_textLayout->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);
    }

    if (SUCCEEDED(hr))
    {
        hr =  CreateAnimationFactory(&m_animationFactory);
    }

    // Initialize animation controllers
    if (SUCCEEDED(hr))
    {
        m_animationFactory->CreateMediaPaneAnimation(FlyIn, &m_animationController);
    }

    if (SUCCEEDED(hr))
    {
        m_renderingParameters.textLayout = m_textLayout;
        m_renderingParameters.textFormat = m_textFormat;
    }


    return hr;
}

HRESULT MediaPaneMessageHandler::OnEraseBackground()
{
    // To prevent flickering, returning Success would indicate
    // this message has been handled, so background will not be erased
    return S_OK;
}

HRESULT MediaPaneMessageHandler::Redraw()
{
    HRESULT hr = CreateDeviceResources();

    if (SUCCEEDED(hr))
    {
        if (m_enableAnimation)
        {
            RenderAnimation();
        }
        else
        {
            DrawClientArea();
        }
    }
    return hr;
}

HRESULT MediaPaneMessageHandler::RenderAnimation()
{
    UI_ANIMATION_SECONDS secondsNow = 0;
    UI_ANIMATION_MANAGER_STATUS status = UI_ANIMATION_MANAGER_IDLE;
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTimer> animationTimer;
    
    HRESULT hr = AnimationUtility::GetAnimationTimer(&animationTimer);

    if (SUCCEEDED (hr))
    {
        hr = animationTimer->GetTime(&secondsNow);
    }

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetAnimationManager(&animationManager);
    }

    if (SUCCEEDED (hr))
    {
        hr = animationManager->Update(secondsNow);
    }
    
    if (SUCCEEDED(hr))
    {
        DrawClientArea();
    }
    
    if (SUCCEEDED(hr))
    {
        hr = animationManager->GetStatus(&status);
    }
    
    if (SUCCEEDED(hr))
    {
        if (status == UI_ANIMATION_MANAGER_BUSY)
        {
            InvalidateParent();
        }
        else
        {
            m_enableAnimation = false;
        }
    }

    return hr;
}

void MediaPaneMessageHandler::InvalidateParent()
{
    ::InvalidateRect(::GetParent(m_hWnd), nullptr, false);
}

void MediaPaneMessageHandler::DrawClientArea()
{
    HRESULT hr = S_OK;
    if (!(m_renderTarget->CheckWindowState() & D2D1_WINDOW_STATE_OCCLUDED))
    {
        m_renderTarget->BeginDraw();

        D2D1_MATRIX_3X2_F identity = Matrix3x2F::Identity();
        m_renderTarget->SetTransform(identity);

        D2D1_SIZE_F size = m_renderTarget->GetSize();

        // Paint background
        m_renderTarget->FillRectangle(RectF(0, 0, size.width, size.height), m_backgroundLinearGradientBrush);

        if (m_enableAnimation)
        {
            DrawAnimatedThumbnailCells();
        }
        else
        {
            float thumbnailSize = GetThumbnailSize();
            // Get the thumbnail cells
            std::vector<ThumbnailCell> *thumbnailCells = nullptr;
            m_thumbnailsLayoutManager->GetThumbnailCells(&thumbnailCells, true);

            if (nullptr != thumbnailCells)
            {
                // Draw the thumbnails cells 
                for ( auto cell = thumbnailCells->begin(); cell < thumbnailCells->end(); cell++ )
                {
                    cell->control->SetIsFullImage(m_isSlideShowMode && (thumbnailSize > ThumbnailsLayoutManager::MaxThumbnailSize));
                    cell->control->Draw();
                }
            }
        }

        // Draw the arrows
        DrawArrows();
        hr = m_renderTarget->EndDraw();

        if (hr == D2DERR_RECREATE_TARGET)
        {
            hr = DiscardDeviceResources();            
        }

        if (FAILED (hr))
        {
#ifdef _DEBUG
            char msg[1024];
            sprintf_s(msg,1000, "D2D Error generated in DrawClientArea() API : Error code = [0x%x] \r\n", hr);
            ::OutputDebugStringA(msg);
#endif
        }
    }
}

void MediaPaneMessageHandler::DrawArrows()
{
    D2D1_RECT_F leftArrowRectangle;
    D2D1_RECT_F rightArrowRectangle;
    HRESULT hr = m_thumbnailsLayoutManager->GetArrowPositions (&leftArrowRectangle, &rightArrowRectangle);

    if (SUCCEEDED (hr))
    {
        unsigned int currentPage = GetCurrentPageIndex();
        unsigned int maxPages = GetMaxPagesCount();
        if (currentPage > 0)
        {
            m_renderTarget->DrawBitmap(m_arrowBitmap, leftArrowRectangle, m_leftArrowSelected || m_leftArrowClicked ? 1.0f : 0.5f);
        }

        if (maxPages > 0 && currentPage < maxPages - 1)
        {
            // Rotate the arrow 180 deg
            m_renderTarget->SetTransform(
                D2D1::Matrix3x2F::Rotation(
                180.0f,
                D2D1::Point2F(rightArrowRectangle.left + (rightArrowRectangle.right - rightArrowRectangle.left) / 2.0f, 
                rightArrowRectangle.top + (rightArrowRectangle.bottom - rightArrowRectangle.top) / 2.0f))
                );

            m_renderTarget->DrawBitmap(m_arrowBitmap, rightArrowRectangle, m_rightArrowSelected || m_rightArrowClicked ? 1.0f : 0.25f);
        }
    }
}

void MediaPaneMessageHandler::DrawAnimatedThumbnailCells()
{
    std::vector<AnimatedThumbnailCell> animatedThumbnailCells;
    m_animationController->GetAnimatedThumbnailCells(animatedThumbnailCells);

    for ( unsigned int i = 0; i < animatedThumbnailCells.size(); i++)
    {
        auto cell = animatedThumbnailCells[i].cell;

        D2D1_POINT_2F center;
        HRESULT hr = m_animationController->GetAnimationPosition(cell.control, &center);

        if (SUCCEEDED (hr))
        {
            float width = cell.position.right - cell.position.left;
            float height = cell.position.bottom -cell.position.top;

            D2D1_RECT_F position;
            position.left = center.x - width/2;
            position.right = center.x + width/2;
            position.top = center.y - height/2;
            position.bottom = center.y + height/2;

            cell.control->SetRect(position);
            cell.control->SetIsFullImage(m_isSlideShowMode && (GetThumbnailSize() > ThumbnailsLayoutManager::MaxThumbnailSize));
            cell.control->Draw();
        }
    }
}

HRESULT MediaPaneMessageHandler::OnSize(unsigned int width, unsigned int height)
{
    HRESULT hr = S_OK;

    if (m_renderTarget)
    {
        D2D1_SIZE_U size;
        size.width = width;
        size.height = height;

        // Note: This method can fail, but it's okay to ignore the
        // error here -- it will be repeated on the next call to
        // EndDraw.
        m_renderTarget->Resize(size);

        D2D1_SIZE_F renderTargetSize = m_renderTarget->GetSize();

        if (m_isSlideShowMode)
        {
            float height = renderTargetSize.height - renderTargetSize.height * ThumbnailsLayoutManager::ShadowSizeRatio;
            m_thumbnailsLayoutManager->UpdateThumbnailSize(D2D1::SizeF(renderTargetSize.width, height));
        }

        if (m_updatingFolder)
        {
            // Initialize FlyIn animation
            m_thumbnailsLayoutManager->ResetLayoutInitialValue();
            CreateThumbnailCells(renderTargetSize);
            m_updatingFolder = false;
            // Do not set up animation when the view is changed to slide show mode at the first time
            if (!m_isSlideShowMode)
            {
                SetupAnimation(FlyIn, renderTargetSize);
            }
        }
        else
        {
            // Initialize MoveAround Animation
            CreateThumbnailCells(renderTargetSize);
            SetupAnimation (MoveAround, renderTargetSize);
        }

        PrepareBackgroundBrush(renderTargetSize);
        m_thumbnailsLayoutManager->CalculateArrowPositions(renderTargetSize);
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::RenderScrollingAnimation(bool nextPage)
{
    CreateThumbnailCells(m_renderTarget->GetSize());
    if (nextPage)
    {
        SetupAnimation (Slide, m_renderTarget->GetSize());
    }
    else
    { 
        SetupAnimation (Slide,  D2D1::SizeF (- m_renderTarget->GetSize().width, - m_renderTarget->GetSize().height));
    }
    InvalidateParent();

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonDown(POINT_2F mousePos)
{
    // Change selection
    bool invalidate = false;
    m_leftArrowClicked = false;
    m_rightArrowClicked = false;

    D2D1_RECT_F leftArrowRectangle;
    D2D1_RECT_F rightArrowRectangle;
    m_thumbnailsLayoutManager->GetArrowPositions(&leftArrowRectangle, &rightArrowRectangle);

    if (leftArrowRectangle.left <= mousePos.x && leftArrowRectangle.right >= mousePos.x && leftArrowRectangle.top <= mousePos.y && leftArrowRectangle.bottom >= mousePos.y)
    {
        m_leftArrowClicked = true;
        invalidate = true;
    }
    else if (rightArrowRectangle.left <= mousePos.x && rightArrowRectangle.right >= mousePos.x && rightArrowRectangle.top <= mousePos.y && rightArrowRectangle.bottom >= mousePos.y)
    {
        m_rightArrowClicked = true;
        invalidate = true;
    }

    // Redraw the window
    if (invalidate)
    {
        InvalidateParent();
    }
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonUp(POINT_2F /*mousePosition*/)
{
    bool handled = false;
    bool nextPage = true;

    if (m_leftArrowClicked && PreviousPage())
    {
        handled = true;
        nextPage = false;
    }
    else if (m_rightArrowClicked && NextPage())
    {
        handled = true;
    }

    m_leftArrowClicked = m_rightArrowClicked = false;

    if (handled)
    {
        RenderScrollingAnimation(nextPage);
    }

    return S_OK;

}

HRESULT MediaPaneMessageHandler::OnLeftMouseButtonDoubleClick(POINT_2F mousePos)
{    
    if (m_isSlideShowMode)
    {
        D2D1_RECT_F leftArrowRectangle;
        D2D1_RECT_F rightArrowRectangle;
        m_thumbnailsLayoutManager->GetArrowPositions(&leftArrowRectangle, &rightArrowRectangle);
        // Clicking anywhere except in arrows will switch to the normal view
        if (!Direct2DUtility::HitTest(leftArrowRectangle, mousePos) && !Direct2DUtility::HitTest(rightArrowRectangle, mousePos))
        {
            ToogleSlideShow(false);
        }
    }
    else
    {
        // Find out if a cell was double clicked
        std::vector<ThumbnailCell>* thumbnailCells = nullptr;
        m_thumbnailsLayoutManager->GetThumbnailCells(&thumbnailCells, true);
        if (nullptr != thumbnailCells)
        {
            for ( auto cell = thumbnailCells->begin() ; cell < thumbnailCells->end(); cell++ )
            {
                if (Direct2DUtility::HitTest(cell->position, mousePos))
                {
                    m_slideShowThumbnail = cell->control; // Save the double clicked control
                    ToogleSlideShow(true);
                    break;
                }
            }
        }
    }

    return S_OK;
}

void MediaPaneMessageHandler::ToogleSlideShow(bool isSlideShow)
{
    m_isSlideShowMode = isSlideShow;
    m_thumbnailsLayoutManager->UpdateThumbnailSize(m_isSlideShowMode);
    if (!isSlideShow)
    {
        // Make sure memory is released for stored full images
        for (auto thumbnail = m_thumbnailControls.begin(); thumbnail < m_thumbnailControls.end(); thumbnail++)
        {
            (*thumbnail)->ReleaseFullImage();
        }
    }

    m_windowLayout->SwitchDisplayMode(m_isSlideShowMode);

}

HRESULT MediaPaneMessageHandler::OnMouseWheel(POINT_2F /*mousePosition*/, short delta, int keys) 
{  
    if ((keys & MK_CONTROL) == MK_CONTROL)
    {
        int numPages = abs(delta / WHEEL_DELTA);

        for (int i = 0; i < numPages; i++)
        {
            if (delta > 0)
            {
                if (ZoomIn())
                {
                    RearrangeCells(false);
                }
            }
            else
            {
                if (ZoomOut())
                {
                    RearrangeCells(true);
                }
            }
        }
    }
    else // scroll to the next page
    {
        if (delta < 0)
        {
            if (NextPage())
            {
                RenderScrollingAnimation(true);
            }
        }
        else if (PreviousPage())
        {
            RenderScrollingAnimation(false);
        }
    }

    return S_OK; 
}

HRESULT MediaPaneMessageHandler::OnMouseMove(POINT_2F mousePosition)
{
    // Don't do calculations while animation is taking place
    if (m_enableAnimation)
        return S_OK;

    m_leftArrowSelected = false;
    m_rightArrowSelected = false;

    D2D1_RECT_F leftArrowRectangle;
    D2D1_RECT_F rightArrowRectangle;
    m_thumbnailsLayoutManager->GetArrowPositions(&leftArrowRectangle, &rightArrowRectangle);

    if (Direct2DUtility::HitTest(leftArrowRectangle, mousePosition))
    {
        m_leftArrowSelected = true;
    }
    else if (Direct2DUtility::HitTest(rightArrowRectangle, mousePosition))
    {
        m_rightArrowSelected = true;
    }

    std::vector<ThumbnailCell>* thumbnailCells = nullptr;
    m_thumbnailsLayoutManager->GetThumbnailCells(&thumbnailCells, true);
    if (nullptr == thumbnailCells)
    {
        return E_POINTER;
    }
    for (auto cell = thumbnailCells->begin(); cell < thumbnailCells->end(); cell++)
    {
        if (Direct2DUtility::HitTest(cell->position, mousePosition))
        {
            cell->selectionState = static_cast<ThumbnailSelectionState>(cell->selectionState | SelectionStateHoverOn);
        }
        else
        {
            cell->selectionState = static_cast<ThumbnailSelectionState>(cell->selectionState & ~SelectionStateHoverOn);
        }

        cell->control->SetSelectionState(cell->selectionState);
    }

    // Redraw
    InvalidateParent();

    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnKeyDown(unsigned int vKey)
{
    if (vKey == VK_ADD || vKey == VK_OEM_PLUS)
    {
        if (ZoomIn())
        {
            RearrangeCells(false);
        }
    }
    else if (vKey == VK_SUBTRACT || vKey == VK_OEM_MINUS)
    {
        if (ZoomOut())
        {
            RearrangeCells(true);
        }
    }
    else if (vKey == VK_PRIOR) // prior page
    {
        if (PreviousPage())
        {
            RenderScrollingAnimation(false);
        }

    }
    else if (vKey == VK_NEXT) // next page
    {
        if (NextPage())
        {
            RenderScrollingAnimation(true);
        }
    }
    return S_OK;
}

HRESULT MediaPaneMessageHandler::OnAppCommandBrowserBackward() 
{ 
    HRESULT hr = S_OK;

    if (PreviousPage())
    {
        hr = RenderScrollingAnimation(false);
    }

    return hr;
}

HRESULT MediaPaneMessageHandler::OnAppCommandBrowserForward() 
{ 
    HRESULT hr = S_OK;

    if (NextPage())
    {
        hr = RenderScrollingAnimation(true);
    }

    return hr;
}

void MediaPaneMessageHandler::PrepareBackgroundBrush(D2D1_SIZE_F size)
{
    m_backgroundLinearGradientBrush->SetStartPoint(Point2F(size.width, 0));
    m_backgroundLinearGradientBrush->SetEndPoint(Point2F(size.width, size.height));
}

void MediaPaneMessageHandler::CreateThumbnailCells(D2D1_SIZE_F size)
{
    UpdateTextLayout();
    m_thumbnailsLayoutManager->CreateLayout(size, m_thumbnailControls, m_slideShowThumbnail, m_isSlideShowMode, m_textLayout->GetMaxHeight());
    // reset slide show thumbnail
    m_slideShowThumbnail = nullptr;
    unsigned int thumbnailCountPerPage = GetCurrentPageThumbnailCount();
    m_AsyncLoaderHelper->SetPageItemCount(thumbnailCountPerPage);
    m_AsyncLoaderHelper->SetCurrentPagePivot(thumbnailCountPerPage * GetCurrentPageIndex());
}

void MediaPaneMessageHandler::UpdateTextLayout()
{
    static DWRITE_TEXT_RANGE defaultTextRange = {0, _MAX_PATH};

    HRESULT hr = m_textLayout->SetMaxWidth(GetThumbnailSize());

    if (SUCCEEDED(hr))
    {
        m_textLayout->SetFontSize(GetFontSize(), defaultTextRange);
    }

    DWRITE_TEXT_METRICS textMetrics = { };
    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->GetMetrics(&textMetrics);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_textLayout->SetMaxHeight(2 * (textMetrics.height / static_cast<float>(textMetrics.lineCount)));
    }
}

bool MediaPaneMessageHandler::PreviousPage()
{
    bool handled = false;
    HRESULT hr = m_thumbnailsLayoutManager->Scroll(false);
    if (SUCCEEDED (hr))
    {
        m_AsyncLoaderHelper->Scroll(-static_cast<int>(GetCurrentPageThumbnailCount()));

        handled = true;
    }

    return handled;
}

bool MediaPaneMessageHandler::NextPage()
{
    bool handled = false;
    HRESULT hr = m_thumbnailsLayoutManager->Scroll(true);
    if (SUCCEEDED (hr))
    {
        m_AsyncLoaderHelper->Scroll(static_cast<int>(GetCurrentPageThumbnailCount()));

        handled = true;
    }

    return handled;
}

bool MediaPaneMessageHandler::ZoomIn()
{
    bool handled = false;
    float maxSize;
    if (m_isSlideShowMode)
    {
        maxSize = std::min<float>(m_renderTarget->GetSize().height, m_renderTarget->GetSize().width) * 2;
    }
    else
    {
        maxSize = ThumbnailsLayoutManager::MaxThumbnailSize;
    }
    if (GetThumbnailSize() < maxSize)
    {
        float zoomFactor = 1.1f;
        m_thumbnailsLayoutManager->Zoom(zoomFactor);
        m_AsyncLoaderHelper->Zoom(zoomFactor);
        handled = true;
    }

    return handled;
}

bool MediaPaneMessageHandler::ZoomOut()
{
    bool handled = false;
    if (GetThumbnailSize() > ThumbnailsLayoutManager::MinThumbnailSize)
    {
        float zoomFactor = 1.1f;
        m_thumbnailsLayoutManager->Zoom(1/zoomFactor);
        m_AsyncLoaderHelper->Zoom(1/zoomFactor);
        handled = true;
    }
    return handled;
}

HRESULT MediaPaneMessageHandler::RemoveAllItems()
{
    HRESULT hr = S_OK;

    m_AsyncLoaderHelper->ClearItems();

    m_thumbnailControls.clear();

    return hr;
}

// IMediaPane
HRESULT MediaPaneMessageHandler::SetCurrentLocation(IShellItem* shellFolder, bool recursive)
{
    HRESULT hr = S_OK;

    // Remove all thumnails stored, if any
    RemoveAllItems();

    std::vector<ComPtr<IShellItem> > shellItems;

    // Ignore the returned HRESULT because it's possible no elements are found in this folder
    ShellItemsLoader::EnumerateFolderItems(shellFolder, FileTypeImage, recursive, shellItems);

    for ( auto shellItem = shellItems.begin() ; shellItem < shellItems.end(); shellItem++ )
    {
        ThumbnailInfo info(*shellItem);
        info.fileType = FileTypeImage;
        
        ComPtr<IThumbnail> imageThumnailControl;
        if (SUCCEEDED(SharedObject<ImageThumbnail>::Create(info, &imageThumnailControl)))
        {
            imageThumnailControl->SetDefaultBitmap(m_defaultThumbnailBitmap);
            imageThumnailControl->SetRenderingParameters(m_renderingParameters);
            imageThumnailControl->SetParentWindow(this);

            m_thumbnailControls.push_back(imageThumnailControl);

            ComPtr<IUnknown> itemIUnknown;
            hr = imageThumnailControl.QueryInterface(&itemIUnknown);
            
            if ( SUCCEEDED(hr) )
            {
                hr = m_AsyncLoaderHelper->ConnectItem(itemIUnknown, static_cast<unsigned int>(m_thumbnailControls.size()) - 1);
            }
        }
    }

    m_updatingFolder = true;
    InvalidateParent();

    return hr;
}

void MediaPaneMessageHandler::RearrangeCells(bool isZoomOut)
{
    m_enableAnimation = false;

    if (m_renderTarget)
    {
        CreateThumbnailCells(m_renderTarget->GetSize());
    }

    if (m_isSlideShowMode && GetCurrentPageThumbnailCount() > 1 && isZoomOut)
    {
        m_isSlideShowMode = false;
        m_thumbnailsLayoutManager->UpdateThumbnailSize(m_isSlideShowMode);

        // Rearrange thumnail cells
        ToogleSlideShow(false);
    }

    InvalidateParent();
}

HRESULT MediaPaneMessageHandler::SetupAnimation(AnimationType animationType, D2D1_SIZE_F size)
{
    HRESULT hr = S_OK;
    m_enableAnimation = true;

    if (m_currentAnimation != animationType)
    {
        if (nullptr != m_animationController)
        {
            m_animationController = nullptr;
        }

        hr = m_animationFactory->CreateMediaPaneAnimation(animationType, &m_animationController);
        m_currentAnimation = animationType;
    }

    if (SUCCEEDED (hr))
    {
        std::vector<ThumbnailCell>* thumbnailCells = nullptr;
        std::vector<ThumbnailCell>* prevThumbnailCells = nullptr;

        m_thumbnailsLayoutManager->GetThumbnailCells(&thumbnailCells, true);
        m_thumbnailsLayoutManager->GetThumbnailCells(&prevThumbnailCells, false);
        
        if ((nullptr != thumbnailCells) && (nullptr != prevThumbnailCells))
        {
            hr = m_animationController->Setup(*prevThumbnailCells, *thumbnailCells, size);
        }
    }

    return hr;
}

// IAsyncLoaderMemoryManagerClient
HRESULT MediaPaneMessageHandler::GetClientItemSize(unsigned int* clientItemSize)
{
    if ( nullptr == clientItemSize )
    {
        return E_POINTER;
    }
    HRESULT hr = S_OK;

    // Locking is not important for this resource.
    *clientItemSize = static_cast<unsigned int>(GetThumbnailSize());

    return hr;
}

// IChildNotificationHandler
HRESULT MediaPaneMessageHandler::OnChildChanged() 
{ 
    InvalidateParent();
    return S_OK;
}

HRESULT MediaPaneMessageHandler::Finalize()
{
    HRESULT hr = S_OK;

     hr = RemoveAllItems();

    if (FAILED(hr))
    {
        return hr;
    }

    hr = m_AsyncLoaderHelper->Shutdown();
    return hr;
}

// Helper functions
float MediaPaneMessageHandler::GetThumbnailSize()
{
    float thumbnailSize = ThumbnailsLayoutManager::DefaultThumbnailSize;
    if (m_thumbnailsLayoutManager)
    {
        m_thumbnailsLayoutManager->GetThumbnailSize(&thumbnailSize);
    }
    return thumbnailSize;
}

unsigned int MediaPaneMessageHandler::GetCurrentPageIndex()
{
    unsigned int index = 0;
    if (m_thumbnailsLayoutManager)
    {
        m_thumbnailsLayoutManager->GetCurrentPageIndex(&index);
    }
    return index;
}

unsigned int MediaPaneMessageHandler::GetCurrentPageThumbnailCount()
{
    unsigned int count = 0;
    if (m_thumbnailsLayoutManager)
    {
        m_thumbnailsLayoutManager->GetCurrentPageThumbnailCount(&count);
    }
    return count;
}

unsigned int MediaPaneMessageHandler::GetMaxPagesCount()
{
    unsigned int count = 0;
    if (m_thumbnailsLayoutManager)
    {
        m_thumbnailsLayoutManager->GetMaxPagesCount(&count);
    }
    return count;
}

float MediaPaneMessageHandler::GetFontSize()
{
    return FontSizeRatio * GetThumbnailSize();
}
