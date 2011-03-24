//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "ThumbnailImpl.h"
#include "AsyncLoader\AsyncLoaderItemCache.h"

using namespace Hilo::WindowApiHelpers;
using namespace Hilo::Direct2DHelpers;

HRESULT Thumbnail::SetRenderingParameters(const RenderingParameters& renderingParameters)
{
    m_renderingParameters = renderingParameters;

    return S_OK;
}

HRESULT Thumbnail::SetParentWindow(IChildNotificationHandler* parentWindowMessageHandler)
{
    m_parentWindowMessageHandler = parentWindowMessageHandler;

    return S_OK;
}

HRESULT Thumbnail::SetDefaultBitmap(ID2D1Bitmap* bitmap)
{
    m_defaultBitmap = bitmap;

    return S_OK;
}

HRESULT Thumbnail::GetRect(D2D1_RECT_F* rect)
{
    (*rect) = m_rect;

    return S_OK;
}

HRESULT Thumbnail::SetRect(D2D1_RECT_F rect)
{
    m_rect = rect;
    
    return S_OK;
}

HRESULT Thumbnail::GetThumbnail(ID2D1Bitmap** bitmap)
{
    if (!IsOutputPointerValid(bitmap))
    {
        return E_POINTER;
    }

    HRESULT hr;

    CriticalSectionLocker l(m_cs);

    hr = AssignToOutputPointer(bitmap, m_thumbnailBitmap);
    if (FAILED(hr))
    {
        return hr;
    }

    return hr;;
}

HRESULT Thumbnail::SetThumbnail(ID2D1Bitmap* newBitmap)
{
    CriticalSectionLocker l(m_cs);

    m_thumbnailBitmap = newBitmap;
    
    return S_OK;
}

//
// SetThumbNailIfNull() does two things under the lock,
// - Checks if m_bitmap is null
// - and if so, it sets it to the passed in newBitmap.
//
// This function is used by the main UI thread to set the default bitmap.
// The AsyncLoader thread does not use this function - instead, it always
// overwrites the current m_bitmap value with the one it loaded.
//
// This means that:
// - If the UI thread makes it first, the item will get the default bitmap
//   until the AsyncLoader loads the image for this item.
// - If the AsyncLoader makes it first, the UI thread does not get to overwrite
//   the item's bitmap with the default one.
//
HRESULT Thumbnail::SetThumbnailIfNull(ID2D1Bitmap* newBitmap)
{
    CriticalSectionLocker l(m_cs);

    if (nullptr == m_thumbnailBitmap)
    {
        m_thumbnailBitmap = newBitmap;
    }

    return S_OK;
}

HRESULT Thumbnail::GetThumbnailInfo(ThumbnailInfo* thumbnailInfo)
{
    (*thumbnailInfo) = m_thumbnailInfo;

    return S_OK;
}

HRESULT Thumbnail::SetIsFullImage(bool isFullImage)
{
    m_isFullImage = isFullImage;

    return S_OK;
}

HRESULT Thumbnail::GetSelectionState(ThumbnailSelectionState* selectionState)
{
    if (nullptr == selectionState)
    {
        return E_POINTER;
    }
    
    (*selectionState) = m_selectionState;
    
    return S_OK;
}

HRESULT Thumbnail::SetSelectionState(ThumbnailSelectionState selectionState)
{
    m_selectionState = selectionState;
    
    return S_OK;
}

HRESULT Thumbnail::SetOpacity(float opacity)
{
    m_opacity = opacity;

    return S_OK;
}

HRESULT Thumbnail::SetFontSize(float size)
{
    m_fontSize = size;

    return S_OK;
}

Thumbnail::Thumbnail(ThumbnailInfo thumbnailInfo) :
    m_thumbnailInfo(thumbnailInfo), 
    m_selectionState(SelectionStateNone),
    m_cs(NULL),
    m_asyncLoadingEnabled(true),
    m_location(0),
    m_parentWindowMessageHandler(NULL)
{
}

Thumbnail::~Thumbnail()
{
}

HRESULT Thumbnail::Draw()
{
    return E_NOTIMPL;
}

HRESULT Thumbnail::ReleaseThumbnail()
{
    m_thumbnailBitmap = nullptr;

    return S_OK;
}

HRESULT Thumbnail::ReleaseFullImage()
{
    m_fullBitmap = nullptr;

    return S_OK;
}

HRESULT Thumbnail::DiscardResources()
{
    ReleaseFullImage();

    ReleaseThumbnail();

    return S_OK;
}

//
// There is no synchronization here since locking is done against a copy of the
// shared resource. The new value will affect the next time m_cs is used.
//
HRESULT Thumbnail::SetCriticalSection(CRITICAL_SECTION* _cs)
{
    m_cs = _cs;
    m_asyncLoaderItemCache->SetCriticalSection(_cs);
    return S_OK;
}

//
// There is no synchronization here since this class does not own any shared 
// resources.
// The IAsyncLoaderItemCache implementer can decide whether synchronization
// is need or not.
//
HRESULT Thumbnail::SetMemoryManager(IAsyncLoaderMemoryManager* _p)
{
    m_asyncLoaderItemCache->SetMemoryManager(_p);
    return S_OK;
}

//
// There is no synchronization here since chagning the location value does
// not impact the operation of the asynchronous loading. The new value
// will be picked again later anyway.
//
HRESULT Thumbnail::GetLocation(int* location)
{
    if ( nullptr == location)
    {
        return E_POINTER;
    }

    *location = m_location;
    return S_OK;
}

//
// There is no synchronization here since chagning the location value does
// not impact the operation of the asynchronous loading. The new value
// will be picked again later anyway.
//
HRESULT Thumbnail::SetLocation(int location)
{
    m_location = location;
    return S_OK;
}

//
// Enable asynchronous loading
//
HRESULT Thumbnail::EnableAsyncLoading(bool enable)
{
    m_asyncLoadingEnabled = enable;
    return S_OK;
}

//
// There is no synchronization here since the shared resources are read-only, 
// and are not expected to change during the life-time of the 
// Thumbnail object.
//
HRESULT Thumbnail::LoadResource(unsigned int size, IUnknown** resource)
{
    if ( nullptr == resource || nullptr == m_renderingParameters.renderTarget)
    {
        return E_POINTER;
    }

    HRESULT hr = S_OK;    
    ComPtr<IShellItem> shellItem;

    hr = ::SHCreateItemFromParsingName(m_thumbnailInfo.GetFileName().c_str(), nullptr /*bindctx*/, IID_PPV_ARGS(&shellItem));    
    if (FAILED(hr))
    {
        return hr;
    }
    
    ComPtr<ID2D1Bitmap> bitmap;
    
    hr = Direct2DUtility::DecodeImageFromThumbCache(shellItem, m_renderingParameters.renderTarget, size, &bitmap);
    if ( FAILED(hr) )
    {
        return hr;
    }

    ComPtr<IUnknown> res;
    hr = bitmap.QueryInterface(&res);
    if (FAILED(hr))
    {
        return hr;
    }

    return AssignToOutputPointer(resource, res);
}

//
// Synchronization in this function is done at the lower-level SetThumbNail().
//
HRESULT Thumbnail::SetResource(IUnknown* resource)
{
    HRESULT hr = S_OK;

    if ( nullptr == resource )
    {
        hr = SetThumbnail(nullptr);
    }
    else
    {
        ComPtr<ID2D1Bitmap> bitmap;
        hr = resource->QueryInterface(IID_PPV_ARGS(&bitmap));
        if (FAILED(hr))
        {
            return hr;
        }

        hr = SetThumbnail(bitmap);
    }
    return hr;
}

//
// There is no synchronization here since this class does not own any shared 
// resources.
// The IChildNotificationHandler implementer can decide whether synchronization
// is need or not.
//
HRESULT Thumbnail::RenderResource()
{
    return m_parentWindowMessageHandler->OnChildChanged();
}

//
// There is no synchronization here since this class does not own any shared 
// resources.
// The IAsyncLoaderItemCache implementer can decide whether synchronization
// is need or not.
//
HRESULT Thumbnail::Load(LoadPage loadPage, LoadSize loadSize)
{
    return m_asyncLoaderItemCache->Load(loadPage, loadSize);
}

//
// There is no synchronization here since this class does not own any shared 
// resources.
// The IAsyncLoaderItemCache implementer can decide whether synchronization
// is need or not.
//
HRESULT Thumbnail::Unload(LoadSize loadSize)
{
    return m_asyncLoaderItemCache->Unload(loadSize);
}

//
// There is no synchronization here since this class does not own any shared 
// resources.
// The IAsyncLoaderItemCache implementer can decide whether synchronization
// is need or not.
//
HRESULT Thumbnail::Shutdown()
{
    return m_asyncLoaderItemCache->Shutdown();
}
