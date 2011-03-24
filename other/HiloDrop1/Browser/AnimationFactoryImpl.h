//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "animationfactory.h"
class AnimationFactoryImpl : public IAnimationFactory
{
private:
    HRESULT CreateMoverAnimation(IMediaPaneAnimation **animation);
    HRESULT CreateSlideAnimation(IMediaPaneAnimation **animation);
    HRESULT CreateFlyerAnimation(IMediaPaneAnimation **animation);

protected:
    AnimationFactoryImpl();
    virtual ~AnimationFactoryImpl();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IAnimationFactory>::CastTo(iid, this, object);
    }

public:
    HRESULT __stdcall CreateMediaPaneAnimation(AnimationType animationType, __out IMediaPaneAnimation **animation);
    HRESULT __stdcall CreateOrbitAnimation(__out IOrbitAnimation** animation);
    HRESULT __stdcall CreateCarouselAnimation(__out ICarouselAnimation** animation);
    HRESULT __stdcall CreateCarouselThumbnailAnimation(__out ICarouselThumbnailAnimation** animation);
};
