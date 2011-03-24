//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "FlyerAnimation.h"
#include "SlideAnimation.h"
#include "MoverAnimation.h"
#include "OrbitAnimation.h"
#include "CarouselAnimation.h"
#include "CarouselThumbnailAnimation.h"
#include "AnimationFactoryImpl.h"

AnimationFactoryImpl::AnimationFactoryImpl()
{
}


AnimationFactoryImpl::~AnimationFactoryImpl()
{
}

HRESULT AnimationFactoryImpl::CreateMoverAnimation(IMediaPaneAnimation **animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<IMediaPaneAnimation> newAnimation;
    HRESULT hr = SharedObject<MoverAnimation>::Create(&newAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, newAnimation);
}

HRESULT AnimationFactoryImpl::CreateSlideAnimation(IMediaPaneAnimation **animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<IMediaPaneAnimation> newAnimation;
    HRESULT hr = SharedObject<SlideAnimation>::Create(&newAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, newAnimation);
}

HRESULT AnimationFactoryImpl::CreateFlyerAnimation(IMediaPaneAnimation **animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<IMediaPaneAnimation> newAnimation;
    HRESULT hr = SharedObject<FlyerAnimation>::Create(&newAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, newAnimation);
}

HRESULT AnimationFactoryImpl::CreateMediaPaneAnimation(AnimationType at, IMediaPaneAnimation **animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    switch (at)
    {
    case MoveAround:
        return CreateMoverAnimation(animation);
    case Slide:
        return CreateSlideAnimation(animation);
    case FlyIn:
        return CreateFlyerAnimation(animation);
    }

    return E_NOTIMPL;
}

HRESULT AnimationFactoryImpl::CreateOrbitAnimation(IOrbitAnimation** animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<IOrbitAnimation> orbitAnimation;
    HRESULT hr = SharedObject<OrbitAnimation>::Create(&orbitAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, orbitAnimation);
}

HRESULT AnimationFactoryImpl::CreateCarouselAnimation(ICarouselAnimation** animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<ICarouselAnimation> carouselAnimation;
    HRESULT hr = SharedObject<CarouselAnimation>::Create(&carouselAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, carouselAnimation);
}

HRESULT AnimationFactoryImpl::CreateCarouselThumbnailAnimation(ICarouselThumbnailAnimation** animation)
{
    if (!IsOutputPointerValid(animation))
    {
        return E_POINTER;
    }

    ComPtr<ICarouselThumbnailAnimation> carouselThumbnailAnimation;
    HRESULT hr = SharedObject<CarouselThumbnailAnimation>::Create(&carouselThumbnailAnimation);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(animation, carouselThumbnailAnimation);
}

HRESULT CreateAnimationFactory(IAnimationFactory **factory)
{
    if (!IsOutputPointerValid(factory))
    {
        return E_POINTER;
    }

    // There's only one animation factory per app, so we make it static
    static ComPtr<IAnimationFactory> animationFactoryImpl;
    HRESULT hr = S_OK;

    if (!animationFactoryImpl)
    {
        hr = SharedObject<AnimationFactoryImpl>::Create(&animationFactoryImpl);
    }

    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(factory, animationFactoryImpl);
}
