//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once

#include "targetver.h"

// Exclude rarely-used stuff from Windows headers
#define WIN32_LEAN_AND_MEAN


// Better type saftey for Shell IDLists 
// For more info refer to http://msdn.microsoft.com/en-us/library/bb773321(VS.85).aspx
#define STRICT_TYPED_ITEMIDS

// Main Windows Headers
#include <Windows.h>

// Other Windows Headers
#include <D2d1.h>
#include <DWrite.h>
#include <PropKey.h>
#include <PropVarUtil.h>
#include <ShlObj.h>
#include <StructuredQuery.h>
#include <ThumbCache.h>
#include <UIAnimation.h>
#include <WinCodec.h>

using namespace D2D1;

// C RunTime Header Files
#include <assert.h>
#include <malloc.h>
#include <memory.h>
#include <stdlib.h>
#include <tchar.h>

// Standrd library declarations
#include <algorithm>
#include <map>
#include <queue>
#include <string>
#include <vector>

// Common Typedefs
typedef D2D1_POINT_2F POINT_2F;

//Common constants
const double PI = 3.14159265358979323846;

// Commonly used headers
#include "ComPtr.h"
#include "SharedObject.h"
#include "ComHelpers.h"
#include "Window.h"
#include "WindowMessageHandler.h"
#include "WindowFactory.h"
#include "Direct2DUtility.h"
#include "AnimationFactory.h"
#include "AnimationUtility.h"

// Usefule macros
#ifndef HINST_THISCOMPONENT
EXTERN_C IMAGE_DOS_HEADER __ImageBase;
#define HINST_THISCOMPONENT ((HINSTANCE)&__ImageBase)
#endif
