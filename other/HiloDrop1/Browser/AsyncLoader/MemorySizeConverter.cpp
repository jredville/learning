//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include <math.h>
#include "MemorySizeConverter.h"

using namespace Hilo::AsyncLoader;

MemorySizeConverter::MemorySizeConverter() 
{
}

MemorySizeConverter::~MemorySizeConverter()
{
}

HRESULT MemorySizeConverter::MemorySizeToClientItemSize(unsigned int memorySize, unsigned int* clientItemSize)
{
    if ( nullptr == clientItemSize )
    {
        return E_POINTER;
    }
    HRESULT hr = S_OK;

    *clientItemSize = static_cast<unsigned int>(sqrt( static_cast<float>(memorySize) / 4 ));

    return hr;
}


HRESULT MemorySizeConverter::ClientItemSizeToMemorySize(unsigned int clientItemSize, unsigned int* memorySize)
{
    if ( nullptr == memorySize )
    {
        return E_POINTER;
    }
    HRESULT hr = S_OK;

    *memorySize = clientItemSize * clientItemSize * 4 /* bytes per pixel */;

    return hr;
}
