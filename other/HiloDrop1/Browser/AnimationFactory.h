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

[uuid("64B40E60-9F22-4296-8F43-0B2157A852C4")]
__interface IAnimationFactory : public IUnknown 
{
public:
    HRESULT __stdcall CreateMediaPaneAnimation(__in AnimationType animationType, __out IMediaPaneAnimation** animatiion);
    HRESULT __stdcall CreateOrbitAnimation(__out IOrbitAnimation** animation);
    HRESULT __stdcall CreateCarouselAnimation(__out ICarouselAnimation** animation);
    HRESULT __stdcall CreateCarouselThumbnailAnimation(__out ICarouselThumbnailAnimation** animation);
};

extern HRESULT CreateAnimationFactory(__out IAnimationFactory **factory);
