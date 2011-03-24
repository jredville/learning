//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

struct ThumbnailCell 
{
    ComPtr<IThumbnail> control;
    D2D1_RECT_F position;
    ThumbnailSelectionState selectionState;

    ThumbnailCell (IThumbnail* thumbnailControl, D2D1_RECT_F thumbnailPosition):
        control (thumbnailControl),
        position (thumbnailPosition)
    {
    }
};


[uuid("8EA37C96-7F73-427C-9C52-0C068EAA1676")]
__interface IThumbnailsLayoutManager : public IUnknown
{
    HRESULT __stdcall CreateLayout(D2D1_SIZE_F pageSize, const std::vector<ComPtr<IThumbnail>>& thumbnailControls, const IThumbnail* slideShowThumbnail, __in bool isSlideShowMode, __in float textBoxHeight);
    HRESULT __stdcall ResetLayoutInitialValue();
    HRESULT __stdcall SetArrowSize(D2D1_SIZE_F arrowSize);
    HRESULT __stdcall CalculateArrowPositions(D2D1_SIZE_F size);
    HRESULT __stdcall GetArrowPositions(__out D2D1_RECT_F* leftArrowRectangle, __out D2D1_RECT_F* rightArrowRectangle);
    HRESULT __stdcall GetThumbnailCells(std::vector<ThumbnailCell>** const thumbnailCells, bool isInCurrentPage);
    HRESULT __stdcall Zoom (float factor);
    HRESULT __stdcall Scroll (bool isToNextPage);
    HRESULT __stdcall UpdateThumbnailSize(bool isSlideShowMode);
    HRESULT __stdcall UpdateThumbnailSize(D2D1_SIZE_F pageSize);
    HRESULT __stdcall GetThumbnailSize(float* thumbnailSize);
    HRESULT __stdcall GetCurrentPageThumbnailCount(unsigned int* count);
    HRESULT __stdcall GetCurrentPageIndex(unsigned int* index);
    HRESULT __stdcall GetMaxPagesCount(unsigned int* count);
};
