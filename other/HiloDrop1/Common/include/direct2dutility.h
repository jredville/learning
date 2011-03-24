//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

// Common Typedefs
typedef D2D1_POINT_2F POINT_2F;

namespace Hilo 
{ 
    namespace Direct2DHelpers
    {
        //
        // This class provides various utility functions to work with 
        // Direct2D and related APIs such as WIC and DirectWrite
        //
        class Direct2DUtility
        {
        public:
            static HRESULT LoadBitmapFromFile(
                ID2D1RenderTarget *renderTarget,
                const wchar_t *uri,
                unsigned int destinationWidth, 
                unsigned int destinationHeight,
                ID2D1Bitmap **bitmap
                );

            static HRESULT LoadBitmapFromResource(
                ID2D1RenderTarget *renderTarget,
                const wchar_t *resourceName,
                const wchar_t *resourceType,
                unsigned int destinationWidth,
                unsigned int destinationHeight,
                ID2D1Bitmap **bitmap
                );

            static HRESULT DecodeImageFromThumbCache(
                IShellItem *shellItem, 
                ID2D1RenderTarget* renderTarget,
                unsigned int thumbnailSize,
                ID2D1Bitmap **bitmap
                );

            static HRESULT GetD2DFactory(__out ID2D1Factory** factory);
            static HRESULT GetWICFactory(__out IWICImagingFactory** factory);
            static HRESULT GetDWriteFactory(__out IDWriteFactory** factory);
    
            static POINT_2F GetMousePositionForCurrentDpi(LPARAM lParam)
            {
                static POINT_2F dpi = {96, 96}; // The default DPI

                ComPtr<ID2D1Factory> factory;
                if (SUCCEEDED(GetD2DFactory(&factory)))
                {
                    factory->GetDesktopDpi(&dpi.x, &dpi.y);
                }

                return D2D1::Point2F(
                    static_cast<int>(static_cast<short>(LOWORD(lParam))) * 96 / dpi.x,
                    static_cast<int>(static_cast<short>(HIWORD(lParam))) * 96 / dpi.y);
            }

            static POINT_2F GetMousePositionForCurrentDpi(float x, float y)
            {
                static POINT_2F dpi = {96, 96}; // The default DPI
                ComPtr<ID2D1Factory> factory;

                if (SUCCEEDED(GetD2DFactory(&factory)))
                {
                    factory->GetDesktopDpi(&dpi.x, &dpi.y);
                }

                return D2D1::Point2F(x * 96 / dpi.x, y * 96 / dpi.y);
            }

            static float ScaleValueForCurrentDPI(float value)
            {
                POINT_2F dpi = {96, 96};
                ComPtr<ID2D1Factory> factory;
            
                if (SUCCEEDED(GetD2DFactory(&factory)))
                {
                    factory->GetDesktopDpi(&dpi.x, &dpi.y);
                }
        
                return value * dpi.x / 96;
            }

            inline static bool HitTest(const D2D1_RECT_F& rect, const D2D1_POINT_2F& point)
            {
                return (rect.left <= point.x && rect.right >= point.x && rect.top <= point.y && rect.bottom >= point.y);
            }

            inline static float GetRectWidth(const D2D1_RECT_F& rect)
            {
                return std::abs(rect.right - rect.left);
            }

            inline static float GetRectHeight(const D2D1_RECT_F& rect)
            {
                return std::abs(rect.bottom - rect.top);
            }

        private:
            Direct2DUtility();
            ~Direct2DUtility();
        };
    }
}
