    
//cslog.h
///////////////////////////////////////////////////////////////////////////////
//
// CS Log - Version 3.00
// --------------------------------------------------------
// Copyright (C) 2004, by Shu Chen (shuchen.net@gmail.net).
//
///////////////////////////////////////////////////////////////////////////////

#if !defined(_CSLOG_)
#define _CSLOG_
#pragma once

#include <ctime>
#include <vector>
using namespace std;

#include "csutil.h"
using namespace CS::UTIL;

#define LOGGER (*(CS::LOG::CLog::Instance()))

#ifdef _DEBUG
    #define MYASSERT(exp) \
        if (!(exp)) \
        { \
            LOGGER << SV_ERROR \
                << TEXT("[ASSERT] ") << TEXT(#exp) \
                << TEXT("; FILE: ") << __FILE__ \
                << TEXT("; LINE: ") << __LINE__ \
                << TEXT("!") << ENDL; \
            goto Exit; \
        }
#else
    #define MYASSERT(exp) goto Exit;
#endif

namespace CS
{
namespace LOG
{

enum SEVERITY{ SV_DEBUG, SV_INFO, SV_WARNING, SV_ERROR, SV_FATAL };

class CLog;
class CLogRecorder
{
friend class CLog;
public:
    CLogRecorder(SEVERITY severity, const TCHAR* tszFormat)
        : m_severity(severity), m_fCanWriteMessage(true)
    {
        StringCchCopy(m_tszFormat, sizeof(m_tszFormat)/sizeof(TCHAR), tszFormat);
    };
    virtual ~CLogRecorder() {};

protected:
    SEVERITY m_severity;
    TCHAR m_tszFormat[256];
    bool m_fCanWriteMessage;

private:
    virtual void Write(const TCHAR* tszInput) = 0;

    void WriteMessage(const TCHAR* tszInput)
    {
        if (m_fCanWriteMessage)
        {
            Write(tszInput);
        }
    };

    void WriteHeader(SEVERITY severity)
    {
        m_fCanWriteMessage = (severity >= m_severity);
        if (m_fCanWriteMessage)
        {
            TCHAR tszOutString[512] = {0};
            FormatLogHeader(severity, tszOutString, sizeof(tszOutString)/sizeof(TCHAR));
            Write(tszOutString);
        }
    };

    virtual void FormatLogHeader(SEVERITY severity, TCHAR* tszOutstring, UINT cchOutstring)
    {
        TCHAR tszThreadID[10] = {0};
        StringCchPrintf(tszThreadID, sizeof(tszThreadID)/sizeof(TCHAR), TEXT("%d"), GetCurrentThreadId());

        TCHAR tszDate[80] = {0};
        TCHAR tszTime[80] = {0};
        SYSTEMTIME today;
        GetLocalTime(&today);
        StringCchPrintf(tszDate, sizeof(tszDate)/sizeof(TCHAR),
            TEXT("%04d-%02d-%02d"),
            today.wYear,
            today.wMonth,
            today.wDay
            );
        StringCchPrintf(tszTime, sizeof(tszTime)/sizeof(TCHAR),
            TEXT("%02d:%02d:%02d,%03d"),
            today.wHour,
            today.wMinute,
            today.wSecond,
            today.wMilliseconds
            );

        TCHAR* p = m_tszFormat;
        TCHAR* pFrom = m_tszFormat;
        while (p != NULL && TEXT('\0') != *p)
        {
            if (TEXT('%') == *p)
            {
                TCHAR tch = *(p+1);
                if (TEXT('d') == tch || TEXT('t') == tch || TEXT('e') == tch || TEXT('h') == tch)
                {
                    StringCchCatN(tszOutstring, cchOutstring, pFrom, p - pFrom);
                    pFrom = p + 2;
                    ++p;

                    switch(tch)
                    {
                    case TEXT('d'):
                        StringCchCat(tszOutstring, cchOutstring, tszDate);
                        break;
                    case TEXT('t'):
                        StringCchCat(tszOutstring, cchOutstring, tszTime);
                        break;
                    case TEXT('e'):
                        {
                        TCHAR* tszSeverity = NULL;
                        switch(severity) {
                        case SV_DEBUG:
                            tszSeverity = TEXT("DBG ");
                            break;
                        case SV_INFO:
                            tszSeverity = TEXT("INFO");
                            break;
                        case SV_WARNING:
                            tszSeverity = TEXT("WARN");
                            break;
                        case SV_ERROR:
                            tszSeverity = TEXT("ERR ");
                            break;
                        case SV_FATAL:
                            tszSeverity = TEXT("FTL ");
                            break;
                        default:
                            tszSeverity = TEXT("UNK ");
                            break;
                        }
                        StringCchCat(tszOutstring, cchOutstring, tszSeverity);
                        break;
                        }
                    case TEXT('h'):
                        StringCchCat(tszOutstring, cchOutstring, tszThreadID);
                        break;
                    }
                }
            }

            ++p;
        }
    };
};

enum ENDLINE {ENDL};

class CLog : public CSingleton<CLog>
{
    friend class CSingleton<CLog>;

    CLog() 
    {
        m_nLockCount = 0;
        m_logstatus = LGSTS_END;
        m_severity = SV_INFO;
        m_bCanWriteSererity = true;

        ZeroMemory(m_tszEnd, sizeof(m_tszEnd));
        ZeroMemory(m_tszSeperator, sizeof(m_tszSeperator));
    };

public:
    enum LOGSTATUS { LGSTS_END, LGSTS_HEADER, LGSTS_MESSAGE };

    ~CLog() 
    {
        vector<CLogRecorder*>::iterator itr;
        for(itr=m_vecRecorder.begin(); itr<m_vecRecorder.end(); itr++)
        {
            if (*itr) delete (*itr);
        }
    };

    void Initialize(const TCHAR* tszEnd = TEXT("\r\n"), const TCHAR* tszSeperator = TEXT(" | "))
    {
        StringCchPrintf(m_tszEnd, sizeof(m_tszEnd)/sizeof(TCHAR), tszEnd);
        StringCchPrintf(m_tszSeperator, sizeof(m_tszSeperator)/sizeof(TCHAR), tszSeperator);
    };

    void AddRecorder(CLogRecorder* recorder)
    {
        m_vecRecorder.push_back(recorder);
    };
    
    inline SEVERITY GetCurrentSeverity() // should be called by the recorder classes
    {
        return m_severity;
    };

    void LogFormatedLine(const TCHAR* tszFormat, ...)
    {
        TCHAR* tszLine = NULL;
        va_list arglist;
        va_start(arglist, tszFormat);
        int iLen = _vsctprintf(tszFormat, arglist) + 1;
        tszLine = new TCHAR[iLen];
        if (NULL != tszLine)
        {
            StringCchVPrintf(tszLine, iLen, tszFormat, arglist);
            *this << tszLine << ENDL;
            delete [] tszLine;
        }
    };

    CLog& operator << (const SEVERITY severity)
    {
        m_criticalSection.Enter();
        m_severity = severity;
        m_nLockCount++;
        if (!m_bCanWriteSererity)  return *this;
        if (m_logstatus == LGSTS_MESSAGE)
        {
            WriteMessage(m_tszSeperator);
        }
        WriteHeader(severity);
        m_logstatus = LGSTS_HEADER;
        m_bCanWriteSererity = false;
        return *this;
    };

    CLog& operator << (const ENDLINE)
    {
        WriteMessage(m_tszEnd);
        m_severity = SV_INFO;
        m_bCanWriteSererity = true;
        if (m_logstatus != LGSTS_END) 
        {
            m_logstatus = LGSTS_END;
            UINT nLockCount = ::InterlockedExchange((LONG*)&m_nLockCount, 0);
            while (nLockCount > 0) 
            {
                m_criticalSection.Leave();            
                nLockCount--;
            }
        }
        return *this;
    };

    CLog& operator << (TCHAR* tszInput)
    {
        m_criticalSection.Enter();
        m_nLockCount++;
        if (m_logstatus == LGSTS_HEADER)
        {
            WriteMessage(m_tszSeperator);
        }
        WriteMessage(tszInput);
        m_logstatus = LGSTS_MESSAGE;
        return *this;
    };

    CLog& operator << (double dInput)
    {
        return (*this) << (float)dInput;
    };

    CLog& operator << (float fInput)
    {
        TCHAR tszInput[128] = {0};
        StringCchPrintf(tszInput, sizeof(tszInput)/sizeof(TCHAR), TEXT("%02.02f"), fInput);
        return (*this) << tszInput;
    };

    template<typename T>
    CLog& operator << (const T* pInput)
    {
        return *this << (T*)pInput;
    };

    template<typename T>
    CLog& operator << (T* pInput)
    {
        TCHAR tszInput[128] = {0};
        StringCchPrintf(tszInput, sizeof(tszInput)/sizeof(TCHAR), TEXT("0x%p"), pInput);
        return (*this) << tszInput;
    };

    template <typename T>
    CLog& operator << (T input)
    {
        TCHAR tszInput[128] = {0};
        StringCchPrintf(tszInput, sizeof(tszInput)/sizeof(TCHAR), TEXT("%d"), input);
        return (*this) << tszInput;
    };

private:
    void WriteHeader(SEVERITY severity)
    {
        vector<CLogRecorder*>::iterator itr;
        for(itr=m_vecRecorder.begin(); itr<m_vecRecorder.end(); itr++)
        {
            (*itr)->WriteHeader(severity);
        }
    };

    void WriteMessage(const TCHAR* tszMessage)
    {
        vector<CLogRecorder*>::iterator itr;
        for(itr=m_vecRecorder.begin(); itr<m_vecRecorder.end(); itr++)
        {
            (*itr)->WriteMessage(tszMessage);
        }
    };

private:
    UINT m_nLockCount;
    vector<CLogRecorder*> m_vecRecorder;
    TCHAR m_tszEnd[16];
    TCHAR m_tszSeperator[16];
    LOGSTATUS m_logstatus;
    SEVERITY m_severity;
    volatile bool m_bCanWriteSererity;
    CCriticalSection m_criticalSection;
};

class CFileLogRecorder : public CLogRecorder
{
public:
    CFileLogRecorder(SEVERITY severity, const TCHAR* tszFileName,
        const TCHAR* tszFormat = TEXT("%h | %t | %e | "))
        :    CLogRecorder(severity, tszFormat)
    {
        m_hFile = ::CreateFile(tszFileName, GENERIC_WRITE, FILE_SHARE_READ,
            0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
#ifdef UNICODE
        if (INVALID_HANDLE_VALUE != m_hFile)
        {
            DWORD dw = 0;
            BYTE bytes[2] = {0xFF, 0xFE};
            ::WriteFile(m_hFile, bytes, sizeof(bytes), &dw, NULL);
        }
#endif
    };

    virtual ~CFileLogRecorder() 
    {
        if (m_hFile != INVALID_HANDLE_VALUE)
        {
            CloseHandle(m_hFile);
        }
    };

private:
    virtual void Write(const TCHAR* tszMessage)
    {
        if (m_hFile != INVALID_HANDLE_VALUE)
        {
            DWORD dw = 0;
            UINT uiMessageSize = 0;
            StringCchLength(tszMessage, 1024, &uiMessageSize);
            ::WriteFile(m_hFile, tszMessage, (DWORD)sizeof(TCHAR)*uiMessageSize, &dw, 0);
        }
    };

private:
    HANDLE m_hFile;
};

class CFilesLogRecorder : public CLogRecorder
{
public:
    CFilesLogRecorder(SEVERITY severity, const TCHAR* tszFileName, UINT uiMaxLineCount,
        const TCHAR* tszFormat = TEXT("%h | %d %t | %e | "))
        :    CLogRecorder(severity, tszFormat), 
             m_uiMaxLineCount(uiMaxLineCount),
             m_uiLineCount(0), m_uiLogCount(1)
    {
        StringCchCopy(m_tszFileName, MAX_PATH, tszFileName);
        // fix the file name to be "xxxx[1].log" as the first log file
        _OpenLogFileWithFixedName();

#ifdef UNICODE
        if (INVALID_HANDLE_VALUE != m_hFile)
        {
            DWORD dw = 0;
            BYTE bytes[2] = {0xFF, 0xFE};
            ::WriteFile(m_hFile, bytes, sizeof(bytes), &dw, NULL);
        }
#endif
    };

    virtual ~CFilesLogRecorder() 
    {
        if (m_hFile != INVALID_HANDLE_VALUE)
        {
            CloseHandle(m_hFile);
        }
    };

private:
    virtual void Write(const TCHAR* tszMessage)
    {
        if (m_hFile != INVALID_HANDLE_VALUE)
        {
            DWORD dw = 0;
            UINT uiMessageSize = 0;
            StringCchLength(tszMessage, 1024, &uiMessageSize);
            ::WriteFile(m_hFile, tszMessage, (DWORD)sizeof(TCHAR)*uiMessageSize, &dw, 0);

            if (NULL != _tcschr(tszMessage, TEXT('\n')))
            {
                ++m_uiLineCount;
            }
            if (m_uiLineCount >= m_uiMaxLineCount)
            {
                CloseHandle(m_hFile);
                ++m_uiLogCount;
                _OpenLogFileWithFixedName();
                m_uiLineCount = 0;
            }
        }
    };

    void _OpenLogFileWithFixedName()
    {
        TCHAR tszNewName[MAX_PATH];
        tszNewName[0] = TEXT('\0');
        TCHAR* tszDot = _tcsrchr(m_tszFileName, TEXT('.'));
        TCHAR* tszSlash = _tcsrchr(m_tszFileName, TEXT('\\'));
        if (tszSlash > tszDot || NULL == tszDot)
        {
            StringCchPrintf(tszNewName, MAX_PATH, TEXT("%s[%d]"), m_tszFileName, m_uiLogCount);
        }
        else
        {
            TCHAR tszSubfix[16] = {0};
            StringCchPrintf(tszSubfix, 16, TEXT("[%d]"), m_uiLogCount);
            StringCchCatN(tszNewName, MAX_PATH, m_tszFileName, tszDot - m_tszFileName);
            StringCchCat(tszNewName, MAX_PATH, tszSubfix);
            StringCchCat(tszNewName, MAX_PATH, tszDot);
        }
        m_hFile = ::CreateFile(tszNewName, GENERIC_WRITE, FILE_SHARE_READ,
            0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
    };

private:
    HANDLE m_hFile;
    const UINT m_uiMaxLineCount;
    TCHAR m_tszFileName[MAX_PATH];
    UINT m_uiLineCount;
    UINT m_uiLogCount;
};

class CEventLogRecorder : public CLogRecorder
{
public:
    CEventLogRecorder(SEVERITY severity, const TCHAR* tszSource,
        const TCHAR* tszFormat = TEXT("%h | %d %t | %e | "))
        :    CLogRecorder(severity, tszFormat) 
    {
        m_hEventLogHandle = RegisterEventSource(NULL, tszSource);
    };

    virtual ~CEventLogRecorder()
    {
        if (m_hEventLogHandle) 
        {
            DeregisterEventSource(m_hEventLogHandle);
        }
    };

private:
    virtual void Write(const TCHAR* tszMessage)
    {
        if (m_hEventLogHandle) 
        {
            ReportEvent(
                m_hEventLogHandle,
                EVENTLOG_ERROR_TYPE,
                1500, 0, NULL, 1, 0, &tszMessage, NULL);
        }
    };

    HANDLE m_hEventLogHandle;
};

class CConsoleLogRecorder : public CLogRecorder
{
public:
    CConsoleLogRecorder(SEVERITY severity, 
        const TCHAR* tszFormat = TEXT("%h | %t | %e | "))
        :    CLogRecorder(severity, tszFormat) 
    {
        m_hStd = GetStdHandle(STD_OUTPUT_HANDLE);
        m_wColorBak = 0;
        CONSOLE_SCREEN_BUFFER_INFO csbi = {0};
        BOOL fRet = GetConsoleScreenBufferInfo(m_hStd, &csbi);
        if (fRet)
        {
            m_wColorBak = csbi.wAttributes & 0x000F;
        }
    };

    virtual ~CConsoleLogRecorder()
    {
        CloseHandle(m_hStd);
    };

private:
    virtual void Write(const TCHAR* tszMessage)
    {
        WORD wColor = 0;
        switch (CLog::Instance()->GetCurrentSeverity())
        {
        case SV_DEBUG:
            wColor = 8; // gray
            break;
        case SV_INFO:
            wColor = 15; // light white
            break;
        case SV_WARNING:
            wColor = 14; // light yellow
            break;
        case SV_ERROR:
            wColor = 12; // light red
            break;
        case SV_FATAL:
            wColor = 12; // light red
            break;
        default:
            wColor = 7; // normal white
        }
        SetConsoleTextAttribute(m_hStd, wColor);
        _tprintf(tszMessage);
        SetConsoleTextAttribute(m_hStd, m_wColorBak);
    };
    
private:
    HANDLE m_hStd;
    WORD m_wColorBak;
};

} // namespace LOG
} // namespace CS

#endif
