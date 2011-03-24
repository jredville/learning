//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "WindowImpl.h"

using namespace Hilo::WindowApiHelpers;

Window::Window() : m_hWnd(nullptr)
{
}

Window::~Window()
{
}

HRESULT Window::Show(bool isVisible)
{
    // Returns nonzero if the window was previously visible, 0 otherwise
    ::ShowWindow(m_hWnd, isVisible ? SW_NORMAL : SW_HIDE);

    return S_OK;
}

HRESULT  Window::GetSize(unsigned int* pixelWidth, unsigned int* pixelHeight)
{
    RECT rect;
    
    if (::GetWindowRect(m_hWnd, &rect))
    {
        if (nullptr != pixelWidth)
        {
            *pixelWidth = rect.right - rect.left;
        }
        
        if (nullptr != pixelHeight)
        {
            *pixelHeight = rect.bottom - rect.top;
        }
    }
    else
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }

    return S_OK;
}

HRESULT Window::SetSize(unsigned int pixelWidth, unsigned int pixelHeight)
{
    if (!::SetWindowPos(m_hWnd,
        0,
        0,
        0,
        pixelWidth,
        pixelHeight,
        SWP_NOMOVE | SWP_NOREPOSITION | SWP_NOZORDER
        ))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }

    
    return S_OK;
}

HRESULT  Window::SetPosition(unsigned int posX, unsigned int posY)
{
    if (!::SetWindowPos(m_hWnd,
        0,
        posX,
        posY,
        0,
        0,
        SWP_NOSIZE | SWP_NOREPOSITION | SWP_NOZORDER
        ))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }
    
    return S_OK;
}

HRESULT Window::GetRect(RECT* rect)
{
    if (nullptr == rect)
    {
        return E_POINTER;
    }

    if (!::GetWindowRect(m_hWnd, rect))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }

    return S_OK;
}

HRESULT Window::SetRect(RECT rect)
{
    HRESULT hr = ::SetWindowPos(
        m_hWnd,
        0,
        rect.left,
        rect.top,
        rect.right - rect.left,
        rect.bottom - rect.top,
        SWP_NOZORDER
        ) ? S_OK : E_FAIL;

    return hr;
}

HRESULT Window::Close()
{
    if (!::CloseWindow(m_hWnd))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }

    return S_OK;
}

HRESULT Window::GetWindowHandle(HWND* hWnd)
{
    if (hWnd == nullptr)
    {
        return E_POINTER;
    }

    *hWnd = m_hWnd;

    return S_OK;
}

HRESULT Window::SetWindowHandle(HWND hWnd)
{
    m_hWnd = hWnd;
    return S_OK;
}

HRESULT Window::GetMessageHandler(IWindowMessageHandler** messageHandler)
{
    return AssignToOutputPointer(messageHandler, m_messageHandler);
}

HRESULT Window::SetMessageHandler(IWindowMessageHandler* messageHandler)
{
    m_messageHandler = messageHandler;
    return S_OK;
}

HRESULT Window::GetClientRect(RECT* clientRect)
{
    if (!::GetClientRect(m_hWnd, clientRect))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }

    return S_OK;
}

HRESULT Window::SetTitle(const std::wstring& title)
{
    if (!::SetWindowText(m_hWnd, title.c_str()))
    {
        return HRESULT_FROM_WIN32(::GetLastError());
    }
     
    m_title = title;
    return S_OK;
}

HRESULT Window::GetTitle(std::wstring* title)
{
    if (nullptr == title)
    {
        return E_POINTER;
    }

    *title = m_title;
    return S_OK;
}

HRESULT Window::GetSmallIcon(HICON* icon)
{
    if (nullptr == icon)
    {
        return E_POINTER;
    }

    *icon = m_smallIcon;
    return S_OK;
}

HRESULT Window::GetLargeIcon(HICON* icon)
{
    if (nullptr == icon)
    {
        return E_POINTER;
    }

    *icon = m_largeIcon;
    return S_OK;
}

HRESULT Window::SetLargeIcon(HICON icon)
{
    // Ignore the return value which is the handle to the prev. icon
    ::SendMessage(m_hWnd, WM_SETICON, ICON_BIG, MAKELPARAM(icon, 0)); 

    return S_OK;
}

HRESULT Window::SetSmallIcon(HICON icon)
{
    // Ignore the return value which is the handle to the prev. icon
    ::SendMessage(m_hWnd, WM_SETICON, ICON_SMALL, MAKELPARAM(icon, 0));

    return S_OK;
}


HRESULT Window::RedrawWindow(bool eraseBackground)
{
    ::InvalidateRect(m_hWnd, nullptr, eraseBackground ? TRUE : FALSE);
    return S_OK;
}

HRESULT Window::RedrawWindow() 
{
    ::InvalidateRect(m_hWnd, nullptr, FALSE);
    return S_OK;
}

HRESULT Window::UpdateWindow() 
{
    ::UpdateWindow(m_hWnd);
    return S_OK;
}

HRESULT Window::SetFocus() 
{
    ::SetFocus(m_hWnd);
    return S_OK;
}

HRESULT Window::SetCapture() 
{
    ::SetCapture(m_hWnd);
    return S_OK;
}

HRESULT Window::IsMouseCaptured(bool* isMouseCaptured)
{
    if (nullptr == isMouseCaptured)
    {
        return E_POINTER;
    }

    *isMouseCaptured = ::GetCapture() == m_hWnd;
    return S_OK;
}
