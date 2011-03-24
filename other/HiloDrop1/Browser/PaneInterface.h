//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

[uuid("CA7AE569-7FF8-4928-A0AE-7BC6FC3CC613")]
__interface IPane : public IUnknown
{
    HRESULT __stdcall Redraw();
    HRESULT __stdcall SetCurrentLocation(__in IShellItem* shellFolder, __in bool recursive);
};
