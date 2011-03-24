//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "ThumbnailsLayoutManager.h"


const float ThumbnailsLayoutManager::HorizontalMarginDelta = 2.0f; 
const float ThumbnailsLayoutManager::DefaultThumbnailSize = 128.0f;
const float ThumbnailsLayoutManager::MaxThumbnailSize = 256.0f;
const float ThumbnailsLayoutManager::MinThumbnailSize = 48.0f;
const float ThumbnailsLayoutManager::MaxThumbnailSizeWithNoText = 64.0f;

const float ThumbnailsLayoutManager::XGapRatio = 0.15f;
const float ThumbnailsLayoutManager::YGapRatio = 0.10f;
const float ThumbnailsLayoutManager::YSelectionBoxDeltaRatio = 0.04f;
const float ThumbnailsLayoutManager::XSelectionBoxDeltaRatio = 0.04f;
const float ThumbnailsLayoutManager::ShadowSizeRatio = 0.05f;

ThumbnailsLayoutManager::ThumbnailsLayoutManager():
    m_thumbnailSize(DefaultThumbnailSize),
    m_currentPage(0), m_thumbnailsPerPage(0), m_maxPages(0)
{
}


ThumbnailsLayoutManager::~ThumbnailsLayoutManager()
{
}

// Layout of thumbnails
HRESULT ThumbnailsLayoutManager:: CreateLayout(D2D1_SIZE_F targetSize, const std::vector<ComPtr<IThumbnail>>& thumbnailControls, const IThumbnail* slideShowThumbnail, bool isSlideShowMode, float textBoxHeight)
{
    m_prevThumbnailCells.clear(); // To free resources

    if (!m_thumbnailCells.empty())
    {
        m_prevThumbnailCells.assign(m_thumbnailCells.begin(), m_thumbnailCells.end());
    }

    m_thumbnailCells.clear();

    float xGap = m_thumbnailSize * XGapRatio;
    float yGap = m_thumbnailSize * YGapRatio;
    float shadowHeight =  m_thumbnailSize * ShadowSizeRatio * 0.5f; // shadow height is approximated because we can't know the image size until it's loaded
    float arrowZoneWidth = (m_arrowSize.width * 0.5f) + (ThumbnailsLayoutManager::HorizontalMarginDelta * 2);

    unsigned int numColumns = std::max<int>(1, static_cast<int>((targetSize.width - (arrowZoneWidth * 2)) / (m_thumbnailSize + xGap)));
    unsigned int numRows = std::max<int>(1, static_cast<int>((targetSize.height + textBoxHeight)/ (m_thumbnailSize + yGap + textBoxHeight + shadowHeight)));

    int prevThumbnailsPerPage = m_thumbnailsPerPage;
    m_thumbnailsPerPage = numColumns * numRows;

    if (prevThumbnailsPerPage != 0)
    {
        m_currentPage = static_cast<int>(m_currentPage * (prevThumbnailsPerPage / static_cast<float>(m_thumbnailsPerPage)));
    }

    m_maxPages = static_cast<int>(ceil(thumbnailControls.size() / static_cast<float>(m_thumbnailsPerPage)));

    // Iniitalize slide show mode
    if (nullptr != slideShowThumbnail)
    {
        unsigned int currentPage = m_thumbnailsPerPage * m_currentPage;
        while (currentPage < thumbnailControls.size() && thumbnailControls[currentPage] != slideShowThumbnail)
        {
            currentPage++;
        }

        m_currentPage = currentPage;
    }

    D2D1_RECT_F drawingRect = {0};
    unsigned int curRow = 0, curColumn = 0;

    float xDelta = 0.5f * (targetSize.width - (arrowZoneWidth * 2) - (numColumns * (m_thumbnailSize + xGap)) + xGap);

    for ( unsigned int i = m_thumbnailsPerPage * m_currentPage; i < thumbnailControls.size() ; i++)
    {
        if (curColumn >= numColumns)
        {
            curColumn= 0;
            curRow++;
        }

        if (curRow >= numRows) // if there's no space, don't draw more images
        {
            break;
        }

        drawingRect.left  = xDelta + arrowZoneWidth + (curColumn * (xGap + m_thumbnailSize));
        drawingRect.right = drawingRect.left + m_thumbnailSize;

        float totalHeight = yGap + textBoxHeight + shadowHeight + m_thumbnailSize;

        if (isSlideShowMode)
        {
            drawingRect.top =  yGap + shadowHeight + (0.5f * (targetSize.height - (totalHeight - yGap))); // center in full screen
        }
        else
        {
            drawingRect.top = yGap + (curRow * totalHeight);
        }

        drawingRect.bottom = drawingRect.top + m_thumbnailSize;

        thumbnailControls[i]->SetRect(drawingRect);

        m_thumbnailCells.push_back(ThumbnailCell(thumbnailControls[i], drawingRect));
        curColumn++;
    }   
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::ResetLayoutInitialValue()
{
    m_currentPage = 0;
    m_thumbnailsPerPage = 0;
    m_maxPages = 0;

    return S_OK;
}

// Arrow
HRESULT ThumbnailsLayoutManager:: SetArrowSize(D2D1_SIZE_F arrowSize)
{
    m_arrowSize = arrowSize;
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::CalculateArrowPositions(D2D1_SIZE_F size)
{
    HRESULT hr = S_OK;
    if (m_arrowSize.width <=0 || m_arrowSize.height <=0)
    {
        hr = E_FAIL;
    }
    if (SUCCEEDED (hr))
    {
        static float width = m_arrowSize.width * 0.5f;
        static float height = m_arrowSize.height * 0.5f;

        m_leftArrowRectangle.left = HorizontalMarginDelta;
        m_leftArrowRectangle.right = m_leftArrowRectangle.left + width;

        m_leftArrowRectangle.top = size.height * 0.5f;
        m_leftArrowRectangle.bottom = m_leftArrowRectangle.top + height;

        m_rightArrowRectangle.left = size.width - width - HorizontalMarginDelta;
        m_rightArrowRectangle.right = m_rightArrowRectangle.left +  width;

        m_rightArrowRectangle.top = m_leftArrowRectangle.top;
        m_rightArrowRectangle.bottom = m_leftArrowRectangle.bottom ;
    }
    return hr;
}

HRESULT ThumbnailsLayoutManager::GetArrowPositions(D2D1_RECT_F *leftArrowRectangle, D2D1_RECT_F *rightArrowRectangle)
{
    if (nullptr == leftArrowRectangle || nullptr == rightArrowRectangle)
    {
        return E_POINTER;
    }

    *leftArrowRectangle = m_leftArrowRectangle;
    *rightArrowRectangle = m_rightArrowRectangle;
    
    return S_OK;
}

// Thumbnail cells
HRESULT  ThumbnailsLayoutManager::GetThumbnailCells(std::vector<ThumbnailCell>** const thumbnailCells, bool isInCurrentPage)
{
    if (nullptr == thumbnailCells)
    {
        return E_POINTER;
    }
    if (isInCurrentPage)
    {
        *thumbnailCells = &m_thumbnailCells;
    }
    else
    {
        *thumbnailCells = &m_prevThumbnailCells;
    }

    return S_OK;
}

// Thumbnail size
HRESULT ThumbnailsLayoutManager::UpdateThumbnailSize(D2D1_SIZE_F pageSize)
{
    m_thumbnailSize = std::min<float>(pageSize.height, pageSize.width);
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::UpdateThumbnailSize(bool isSlideShowMode)
{
    if (isSlideShowMode)
    {
        m_lastSavedThumbnailSize = m_thumbnailSize; // Save the last thumbnail size to use later
    }
    else
    {
        m_thumbnailSize = m_lastSavedThumbnailSize;
    }
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::GetThumbnailSize (float* thumbnailSize)
{
    if (nullptr == thumbnailSize)
    {
        return E_POINTER;
    }

    *thumbnailSize = m_thumbnailSize;
    return S_OK;
}

// Zoom/Scroll
HRESULT ThumbnailsLayoutManager::Zoom (float factor)
{
    m_thumbnailSize *= factor;
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::Scroll (bool isToNextPage)
{
    HRESULT hr = S_OK;

    if (isToNextPage)
    {
        if (m_maxPages > 0 && m_currentPage < m_maxPages -1)
        {
            m_currentPage++;
        }
        else
        {
            hr = E_INVALIDARG; // can't move to next page
        }
    }
    else
    {
        if (m_currentPage > 0)
        {
            m_currentPage--;
        }
        else
        {
            hr = E_INVALIDARG; // can't move to previous page
        }
    }
    return hr;
}

// Page related
HRESULT ThumbnailsLayoutManager::GetCurrentPageIndex (unsigned int* index)
{
    if (nullptr == index)
    {
        return E_POINTER;
    }

    *index = m_currentPage;
    return S_OK;
}


HRESULT ThumbnailsLayoutManager::GetCurrentPageThumbnailCount (unsigned int* count)
{
    if (nullptr == count)
    {
        return E_POINTER;
    }

    *count = m_thumbnailsPerPage;
    return S_OK;
}

HRESULT ThumbnailsLayoutManager::GetMaxPagesCount (unsigned int* count)
{
    if (nullptr == count)
    {
        return E_POINTER;
    }

    *count = m_maxPages;
    return S_OK;
}
