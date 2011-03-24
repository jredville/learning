//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "LineAnimation.h"


LineAnimation::LineAnimation()
{
}


LineAnimation::~LineAnimation()
{
}

// Read animation position for the specific control
HRESULT LineAnimation::GetAnimationPosition(IThumbnail *control, D2D1_POINT_2F *point)
{
    HRESULT hr = E_FAIL;

    if (nullptr == control || nullptr == point)
    {
        return E_POINTER;
    }

    LineAnimationMap :: const_iterator iter;
    iter = m_animationPoints.find(control);
    // every input control is expected to have an animation variable
    if (iter != m_animationPoints.end())
    {
        double pointX, pointY;
        hr = iter->second.animationX->GetValue(&pointX);
        point->x = static_cast<float>(pointX);
        if (SUCCEEDED(hr))
        {
            hr = iter->second.animationY->GetValue(&pointY);
            point->y = static_cast<float>(pointY);
        }
    }
    return hr;
}

// Get all the animated thumbnail cells
HRESULT LineAnimation::GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells)
{    
    animatedCells.assign(m_animatedThumbnailCells.begin(), m_animatedThumbnailCells.end());
    return S_OK;
}

