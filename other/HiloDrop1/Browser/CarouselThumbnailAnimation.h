//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "Animation.h"

class CarouselThumbnailAnimation : public ICarouselThumbnailAnimation
{
public:
    // Getters
    HRESULT __stdcall GetInfo(D2D1_POINT_2F* point, double* opacity);

    // Methods
    HRESULT __stdcall Initialize(D2D1_POINT_2F initialPoint, double initialOpacity = 1);
    HRESULT __stdcall Setup(D2D1_POINT_2F targetPoint, double targetOpacity, double duration = 0);
    HRESULT __stdcall Setup(
        D2D1_POINT_2F keyFramePoint,
        D2D1_POINT_2F targetPoint,
        double duration = 0
        );

protected:
    // Constructor / Destructor
    CarouselThumbnailAnimation();
    virtual ~CarouselThumbnailAnimation();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<ICarouselThumbnailAnimation>::CastTo(iid, this, object);
    }


private:
    // Animation variables
    ComPtr<IUIAnimationVariable> m_pointX;
    ComPtr<IUIAnimationVariable> m_pointY;
    ComPtr<IUIAnimationVariable> m_opacity;
};
