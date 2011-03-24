//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "ThumbnailControl.h"
#include "ThumbnailsLayoutInterface.h"

class ThumbnailsLayoutManager : public IThumbnailsLayoutManager
{
private:
    // Constants
    static const float HorizontalMarginDelta; 
    static const float XGapRatio;
    static const float YGapRatio;

    // Arrows
    D2D1_RECT_F m_rightArrowRectangle;
    D2D1_RECT_F m_leftArrowRectangle;
    D2D1_SIZE_F m_arrowSize;

    // Thumbnail cells
    std::vector<ThumbnailCell> m_prevThumbnailCells;
    std::vector<ThumbnailCell> m_thumbnailCells;

    // The minimal value of thumbnail height and thumbnail width
    float m_thumbnailSize;
    float m_lastSavedThumbnailSize;

    // Page related information
    unsigned int m_currentPage;
    unsigned int m_thumbnailsPerPage;
    unsigned int m_maxPages;


protected:
    // Constructor
    ThumbnailsLayoutManager();

    // Destructor
    virtual ~ThumbnailsLayoutManager();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IThumbnailsLayoutManager>::CastTo(iid, this, object);
    }

public:
    // Constants
    static const float DefaultThumbnailSize;
    static const float MinThumbnailSize;
    static const float MaxThumbnailSize;
    static const float MaxThumbnailSizeWithNoText;
    static const float ShadowSizeRatio;
    static const float YSelectionBoxDeltaRatio;
    static const float XSelectionBoxDeltaRatio;

    // Implementation
    HRESULT __stdcall CreateLayout(D2D1_SIZE_F pageSize, const std::vector<ComPtr<IThumbnail>>& thumbnailControls, const IThumbnail* slideShowThumbnail, __in bool isSlideShowMode, __in float textBoxHeight);
    HRESULT __stdcall ResetLayoutInitialValue();

    // Arrow related
    HRESULT __stdcall SetArrowSize(D2D1_SIZE_F arrowSize);
    HRESULT __stdcall CalculateArrowPositions(D2D1_SIZE_F size);
    HRESULT __stdcall GetArrowPositions(__out D2D1_RECT_F* leftArrowRectangle, __out D2D1_RECT_F* rightArrowRectangle);

    // Get the address of vector of thumbnail cells
    HRESULT __stdcall GetThumbnailCells(std::vector<ThumbnailCell>** const thumbnailCells, bool isInCurrentPage);

    // Zoom/Scroll
    HRESULT __stdcall Zoom (float factor);
    HRESULT __stdcall Scroll (bool isToNextPage);

    // Update the thumbnail size when the slide show mode is changed
    HRESULT __stdcall UpdateThumbnailSize(bool isSlideShowMode);
    // Update the thumbnail size when the page size is changed
    HRESULT __stdcall UpdateThumbnailSize(D2D1_SIZE_F pageSize);
    // Get thumbnail size
    HRESULT __stdcall GetThumbnailSize(float* thumbnailSize);

    // Get basic information related with page
    HRESULT __stdcall GetCurrentPageThumbnailCount(unsigned int* count);
    HRESULT __stdcall GetCurrentPageIndex(unsigned int* index);
    HRESULT __stdcall GetMaxPagesCount(unsigned int* count);

};

