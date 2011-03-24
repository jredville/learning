//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "ComPtr.h"

// This error code is not defined in the Windows headers so we
// define it here for readibility.
const static HRESULT E_USER_CANCELED_OPERATION = 0x800704C7;

template<class T>
inline bool IsOutputPointerValid(T* p)
{
    bool valid = (nullptr != p) && (nullptr == *p);
    assert(valid);
    return valid;
}

template<class T>
HRESULT AssignToOutputPointer(T** pp, ComPtr<T>p)
{
    if ( !IsOutputPointerValid(pp) )
    {
        return E_POINTER;
    }

    (*pp) = p;
    if ( nullptr != (*pp) )
    {
        (*pp)->AddRef();
    }

    return S_OK;
}

