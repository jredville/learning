//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "Direct2DUtility.h"

using namespace Hilo::Direct2DHelpers;

Direct2DUtility::Direct2DUtility()
{
}


Direct2DUtility::~Direct2DUtility()
{
}

//
// Creates a Direct2D bitmap from the specified
// file name.
//
HRESULT Direct2DUtility::LoadBitmapFromFile(
    ID2D1RenderTarget *renderTarget,
    const wchar_t *uri,
    unsigned int destinationWidth, 
    unsigned int destinationHeight,
    ID2D1Bitmap ** bitmap
    )
{
    HRESULT hr = S_OK;

    ComPtr<IWICBitmapDecoder> decoder;
    ComPtr<IWICBitmapFrameDecode> bitmapSource;
    ComPtr<IWICStream> stream;
    ComPtr<IWICFormatConverter> converter;
    ComPtr<IWICBitmapScaler> scaler;

    ComPtr<IWICImagingFactory> wicFactory;
    hr = GetWICFactory(&wicFactory);

    if (SUCCEEDED(hr))
    {
        hr = wicFactory->CreateDecoderFromFilename(
            uri,
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnLoad,
            &decoder
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create the initial frame.
        hr = decoder->GetFrame(0, &bitmapSource);
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&converter);
    }

    if (SUCCEEDED(hr))
    {
        // If a new width or height was specified, create an
        // IWICBitmapScaler and use it to resize the image.
        if (destinationWidth != 0 || destinationHeight != 0)
        {
            unsigned int originalWidth, originalHeight;
            hr = bitmapSource->GetSize(&originalWidth, &originalHeight);
            if (SUCCEEDED(hr))
            {
                if (destinationWidth == 0)
                {
                    float scalar = static_cast<float>(destinationHeight) / static_cast<float>(originalHeight);
                    destinationWidth = static_cast<unsigned int>(scalar * static_cast<float>(originalWidth));
                }
                else if (destinationHeight == 0)
                {
                    float scalar = static_cast<float>(destinationWidth) / static_cast<float>(originalWidth);
                    destinationHeight = static_cast<unsigned int>(scalar * static_cast<float>(originalHeight));
                }

                hr = wicFactory->CreateBitmapScaler(&scaler);
                if (SUCCEEDED(hr))
                {
                    hr = scaler->Initialize(
                        bitmapSource,
                        destinationWidth,
                        destinationHeight,
                        WICBitmapInterpolationModeCubic
                        );
                }
                if (SUCCEEDED(hr))
                {
                    hr = converter->Initialize(
                        scaler,
                        GUID_WICPixelFormat32bppPBGRA,
                        WICBitmapDitherTypeNone,
                        nullptr,
                        0.f,
                        WICBitmapPaletteTypeMedianCut
                        );
                }
            }
        }
        else // Don't scale the image.
        {
            hr = converter->Initialize(
                bitmapSource,
                GUID_WICPixelFormat32bppPBGRA,
                WICBitmapDitherTypeNone,
                nullptr,
                0.f,
                WICBitmapPaletteTypeMedianCut
                );
        }
    }


    if (SUCCEEDED(hr))
    {
        // Create a Direct2D bitmap from the WIC bitmap.
        hr = renderTarget->CreateBitmapFromWicBitmap(
            converter,
            nullptr,
            bitmap
            );
    }

    return hr;
}

//
// Creates a Direct2D bitmap from the specified
// resource name and type.
//
HRESULT Direct2DUtility::LoadBitmapFromResource(
    ID2D1RenderTarget *renderTarget,
    const wchar_t *resourceName,
    const wchar_t *resourceType,
    unsigned int destinationWidth,
    unsigned int destinationHeight,
    ID2D1Bitmap **bitmap
    )
{
    HRESULT hr = S_OK;
    ComPtr<IWICImagingFactory> wicFactory;
    ComPtr<IWICBitmapDecoder> decoder;
    ComPtr<IWICBitmapFrameDecode> bitmapSource;
    ComPtr<IWICStream> stream;
    ComPtr<IWICFormatConverter> formatConverter;
    ComPtr<IWICBitmapScaler> scaler;

    HRSRC imageResHandle = nullptr;
    HGLOBAL imageResDataHandle = nullptr;
    void *imageFile = nullptr;
    unsigned long imageFileSize = 0;

    // Locate the resource.
    imageResHandle = ::FindResourceW(HINST_THISCOMPONENT, resourceName, resourceType);

    hr = imageResHandle ? S_OK : E_FAIL;
    if (SUCCEEDED(hr))
    {
        // Load the resource.
        imageResDataHandle = ::LoadResource(HINST_THISCOMPONENT, imageResHandle);

        hr = imageResDataHandle ? S_OK : E_FAIL;
    }
    if (SUCCEEDED(hr))
    {
        // Lock it to get a system memory pointer.
        imageFile = ::LockResource(imageResDataHandle);

        hr = imageFile ? S_OK : E_FAIL;
    }
    if (SUCCEEDED(hr))
    {
        // Calculate the size.
        imageFileSize = SizeofResource(HINST_THISCOMPONENT, imageResHandle);

        hr = imageFileSize ? S_OK : E_FAIL;
    }

    if (SUCCEEDED(hr))
    {
        hr = GetWICFactory(&wicFactory);
    }

    if (SUCCEEDED(hr))
    {
        // Create a WIC stream to map onto the memory.
        hr = wicFactory->CreateStream(&stream);
    }

    if (SUCCEEDED(hr))
    {
        // Initialize the stream with the memory pointer and size.
        hr = stream->InitializeFromMemory(
            reinterpret_cast<unsigned char*>(imageFile),
            imageFileSize
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create a decoder for the stream.
        hr = wicFactory->CreateDecoderFromStream(
            stream,
            nullptr,
            WICDecodeMetadataCacheOnLoad,
            &decoder
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create the initial frame.
        hr = decoder->GetFrame(0, &bitmapSource);
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&formatConverter);
    }

    if (SUCCEEDED(hr))
    {
        // If a new width or height was specified, create an
        // IWICBitmapScaler and use it to resize the image.
        if (destinationWidth != 0 || destinationHeight != 0)
        {
            unsigned int originalWidth, originalHeight;
            hr = bitmapSource->GetSize(&originalWidth, &originalHeight);
            if (SUCCEEDED(hr))
            {
                if (destinationWidth == 0)
                {
                    float scalar = static_cast<float>(destinationHeight) / static_cast<float>(originalHeight);
                    destinationWidth = static_cast<unsigned int>(scalar * static_cast<float>(originalWidth));
                }
                else if (destinationHeight == 0)
                {
                    float scalar = static_cast<float>(destinationWidth) / static_cast<float>(originalWidth);
                    destinationHeight = static_cast<unsigned int>(scalar * static_cast<float>(originalHeight));
                }

                hr = wicFactory->CreateBitmapScaler(&scaler);
                if (SUCCEEDED(hr))
                {
                    hr = scaler->Initialize(
                        bitmapSource,
                        destinationWidth,
                        destinationHeight,
                        WICBitmapInterpolationModeCubic
                        );
                    if (SUCCEEDED(hr))
                    {
                        hr = formatConverter->Initialize(
                            scaler,
                            GUID_WICPixelFormat32bppPBGRA,
                            WICBitmapDitherTypeNone,
                            nullptr,
                            0.f,
                            WICBitmapPaletteTypeMedianCut
                            );
                    }
                }
            }
        }
        else
        {
            hr = formatConverter->Initialize(
                bitmapSource,
                GUID_WICPixelFormat32bppPBGRA,
                WICBitmapDitherTypeNone,
                nullptr,
                0.f,
                WICBitmapPaletteTypeMedianCut
                );
        }
    }

    if (SUCCEEDED(hr))
    {
        //create a Direct2D bitmap from the WIC bitmap.
        hr = renderTarget->CreateBitmapFromWicBitmap(
            formatConverter,
            nullptr,
            bitmap
            );
    }

    return hr;
}

//
// Create a Direct2D Bitmap from a Shell Thumbnail cache
//
HRESULT Direct2DUtility::DecodeImageFromThumbCache(
    IShellItem *shellItem,
    ID2D1RenderTarget* renderTarget,
    unsigned int thumbnailSize,
    ID2D1Bitmap **bitmap
    )
{
    HRESULT hr = S_OK;

    ComPtr<IShellItemImageFactory> imageFactory;
    ComPtr<IWICFormatConverter> converter;

    hr = shellItem->QueryInterface(IID_PPV_ARGS(&imageFactory));

    HBITMAP hBitmap = nullptr;
    if (SUCCEEDED(hr))
    {
        SIZE size = {thumbnailSize, thumbnailSize};
        hr = imageFactory->GetImage(
            size,
            SIIGBF_BIGGERSIZEOK, // improves performance, since we'll need to resize anyway
            &hBitmap
            );
    }

    hr = nullptr == hBitmap ? E_FAIL : hr;

    ComPtr<IWICImagingFactory> wicFactory;
    if (SUCCEEDED(hr))
    {
        hr = GetWICFactory(&wicFactory);
    }

    ComPtr<IWICBitmap> wicBitmap;
    if (SUCCEEDED(hr))
    {
        hr = wicFactory->CreateBitmapFromHBITMAP(
            hBitmap,
            nullptr,
            WICBitmapUseAlpha,
            &wicBitmap
            );
    }

    // Make sure to free the resource as soon as it's not needed.
    if (nullptr != hBitmap)
    {
        ::DeleteObject(hBitmap);
        hBitmap = nullptr;
    }

    if (SUCCEEDED(hr))
    {
        // Convert the image format to 32bppPBGRA
        // (DXGI_FORMAT_B8G8R8A8_UNORM + D2D1_ALPHA_MODE_PREMULTIPLIED).
        hr = wicFactory->CreateFormatConverter(&converter);
    }

    if (SUCCEEDED(hr))
    {
        hr = converter->Initialize(
            wicBitmap,
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.f,
            WICBitmapPaletteTypeMedianCut
            );
    }

    if (SUCCEEDED(hr))
    {
        // Create a D2D bitmap from the WIC bitmap
        hr = renderTarget->CreateBitmapFromWicBitmap(
            converter,
            nullptr,
            bitmap
            );
    }

    return hr;
}

//
// Get a Direct2D factory
//
HRESULT Direct2DUtility::GetD2DFactory(ID2D1Factory** factory)
{
    static ComPtr<ID2D1Factory> m_pD2DFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pD2DFactory)
    {
#if defined(DEBUG) || defined(_DEBUG)
        D2D1_FACTORY_OPTIONS options;
        options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;

        hr = D2D1CreateFactory(
            D2D1_FACTORY_TYPE_MULTI_THREADED,
            options,
            &m_pD2DFactory
            );
#else
        hr = D2D1CreateFactory(
            D2D1_FACTORY_TYPE_MULTI_THREADED,
            &m_pD2DFactory
            );
#endif
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pD2DFactory);
    }

    return hr;
}

//
// Get a WIC Imaging factory
//
HRESULT Direct2DUtility::GetWICFactory(IWICImagingFactory** factory)
{
    static ComPtr<IWICImagingFactory> m_pWICFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pWICFactory)
    {
        hr = CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_pWICFactory)
            );
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pWICFactory);
    }

    return hr;
}

//
// Get a DirectWrite factory
//
HRESULT Direct2DUtility::GetDWriteFactory(IDWriteFactory** factory )
{
    static ComPtr<IDWriteFactory> m_pDWriteFactory;
    HRESULT hr = S_OK;

    if (nullptr == m_pDWriteFactory)
    {
        hr = DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(m_pDWriteFactory), reinterpret_cast<IUnknown**>(&m_pDWriteFactory));
    }

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(factory, m_pDWriteFactory);
    }

    return hr;
}
