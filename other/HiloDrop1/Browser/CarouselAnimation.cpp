//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "CarouselAnimation.h"

using namespace Hilo::AnimationHelpers;

CarouselAnimation::CarouselAnimation()
{
}

CarouselAnimation::~CarouselAnimation()
{
}

HRESULT CarouselAnimation::GetInfo(double* rotation, double* thumbnailScale, double* thumbnailOpacity)
{
    HRESULT hr = S_OK;

    double rotationValue = 0;
    double scale = 1;
    double opacity = 1;

    if (rotation)
    {
        hr = m_rotation->GetValue(&rotationValue);

        *rotation = rotationValue;
    }

    if (SUCCEEDED(hr) && thumbnailScale)
    {
        hr = m_thumbnailScale->GetValue(&scale);

        *thumbnailScale = scale;
    }

    if (SUCCEEDED(hr) && thumbnailOpacity)
    {
        hr = m_thumbnailOpacity->GetValue(&opacity);

        *thumbnailOpacity = opacity;
    }

    return hr;
}

HRESULT CarouselAnimation::Initialize(double rotation, double thumbnailScale, double thumbnailOpacity)
{
    HRESULT hr;

    // Retrieve animation objects
    hr = AnimationUtility::GetAnimationManager(&m_animationManager);
    
    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetTransitionLibrary(&m_transitionLibrary);
    }

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetAnimationTimer(&m_animationTimer);
    }

    if (SUCCEEDED(hr) && nullptr == m_rotation)
    {
        hr = m_animationManager->CreateAnimationVariable(rotation, &m_rotation);
    }

    if (SUCCEEDED(hr) && nullptr == m_thumbnailScale)
    {
        hr = m_animationManager->CreateAnimationVariable(thumbnailScale, &m_thumbnailScale);
    }

    if (SUCCEEDED(hr) && nullptr == m_thumbnailOpacity)
    {
        hr = m_animationManager->CreateAnimationVariable(thumbnailOpacity, &m_thumbnailOpacity);
    }

    return hr;
}

HRESULT CarouselAnimation::SetupRotation(double rotation, double duration)
{
    HRESULT hr = S_OK;
    
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize animation variables if necessary
    if (nullptr == m_animationManager)
    {
        hr = Initialize();
    }

    if (SUCCEEDED(hr))
    {
        // Initialize storyboard
        hr = m_animationManager->CreateStoryboard(&storyboard);
        
        if (SUCCEEDED(hr))
        {
            // Create rotation transition
            hr = m_transitionLibrary->CreateAccelerateDecelerateTransition(
                duration,
                rotation,
                0.0,
                1.0,
                &transition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_rotation, transition);
            }

            if (SUCCEEDED(hr))
            {
                hr = AnimationUtility::ScheduleStoryboard(storyboard);
            }
        }
    }

    return hr;
}

HRESULT CarouselAnimation::SetupScale(double thumbnailScale, double duration)
{
    HRESULT hr = S_OK;
    
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize animation variables if necessary
    if (nullptr == m_animationManager)
    {
        hr = Initialize();
    }

    if (SUCCEEDED(hr))
    {
        // Initialize storyboard
        hr = m_animationManager->CreateStoryboard(&storyboard);
        
        if (SUCCEEDED(hr))
        {
            // Create scale transition
            hr = m_transitionLibrary->CreateLinearTransition(
                duration,
                thumbnailScale,
                &transition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_thumbnailScale, transition);
            }

            if (SUCCEEDED(hr))
            {
                hr = AnimationUtility::ScheduleStoryboard(storyboard);
            }
        }
    }

    return hr;
}

HRESULT CarouselAnimation::SetupOpacity(double thumbnailOpacity, double duration)
{
    HRESULT hr = S_OK;
    
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize animation variables if necessary
    if (nullptr == m_animationManager)
    {
        hr = Initialize();
    }

    if (SUCCEEDED(hr))
    {
        // Initialize storyboard
        hr = m_animationManager->CreateStoryboard(&storyboard);
        
        if (SUCCEEDED(hr))
        {
            // Create rotation transition
            hr = m_transitionLibrary->CreateLinearTransition(
                duration,
                thumbnailOpacity,
                &transition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_thumbnailOpacity, transition);
            }

            if (SUCCEEDED(hr))
            {
                hr = AnimationUtility::ScheduleStoryboard(storyboard);
            }
        }
    }

    return hr;
}
