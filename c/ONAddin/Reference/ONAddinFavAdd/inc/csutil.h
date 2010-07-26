

///////////////////////////////////////////////////////////////////////////////
//
// CS Util classes library - Version 2.00
// --------------------------------------------------------
// Copyright (C) 2004, by Shu Chen (shuchen.net@gmail.net).
//
///////////////////////////////////////////////////////////////////////////////
#pragma once

#if !defined(_CSUTIL_)
#define _CSUTIL_

#include <process.h>
#include <queue>
#include <set>
using namespace std;
#include <math.h>

namespace CS
{
namespace UTIL
{

static TCHAR* GetLastErrorMessage(DWORD dwErr)
{
    static TCHAR tszErrMsg[512] = {0};
    if (!FormatMessage(FORMAT_MESSAGE_FROM_SYSTEM, 
        0, dwErr, 0, tszErrMsg,  511, NULL))
    {
        StringCchPrintf(tszErrMsg, sizeof(tszErrMsg)/sizeof(TCHAR), 
            TEXT("FormatMessage failed when translate the error: 0x%X"), dwErr);
    }
    return tszErrMsg;
}

static __int64 CompareSystemTime(SYSTEMTIME time1, SYSTEMTIME time2)
{
    FILETIME ft1 = {0};
    FILETIME ft2 = {0};
    SystemTimeToFileTime(&time1, &ft1);
    SystemTimeToFileTime(&time2, &ft2);

    __int64* piiFt1 = 0;
    __int64* piiFt2 = 0;
    piiFt1 = (__int64*)&ft1;
    piiFt2 = (__int64*)&ft2;

    return ((*piiFt1) - (*piiFt2)) / 10000 /* change 100-nanosecs to millisecs */;

    //int iRet = (int)(ft1.dwHighDateTime - ft2.dwHighDateTime);
    //if (0 == iRet)
    //{
    //    iRet = (int)(ft1.dwLowDateTime - ft2.dwLowDateTime);
    //}
    //return iRet;
}

static CHAR* BSTR2STR(BSTR bstr)
{
    CHAR* szRet = NULL;
    int iLen = WideCharToMultiByte(
        CP_ACP, 0, 
        bstr, ::SysStringLen(bstr) + 1, 
        NULL, 0, NULL, NULL);
    if (0 == iLen)
    {
        goto Exit;
    }
    szRet = new CHAR[iLen];
    if (NULL == szRet)
    {
        goto Exit;
    }
    ZeroMemory(szRet, iLen);
    iLen = WideCharToMultiByte(
                CP_ACP, 0, 
                bstr, ::SysStringLen(bstr) + 1,
                szRet, iLen, NULL, NULL);
    if (0 == iLen)
    {
        delete [] szRet;
        szRet = NULL;
    }

Exit:
    return szRet;
}

static bool TtoF(LPCTSTR tszData, double* pdRet)
{
    bool fRet = false;

    if (NULL == tszData || tszData[0] == 0)
    {
        goto Exit;
    }

    // take care of the sign/unsign stat
    int iSign = 1;
    int iStart = 0;
    if (_T('+') == tszData[0])
    {
        iSign = 1;
        iStart = 1;
    }
    else if (_T('-') == tszData[0])
    {
        iSign = -1;
        iStart = 1;
    }

    int iInteger = 0;
    double dFraction = 0.0;
    int iFracNum = 0;
    // start the status machine
    int iStatus = 0;
    size_t iLength = 0;
    StringCchLength(tszData, 1024, &iLength);
    for (size_t i=iStart; i<iLength; ++i)
    {
        TCHAR ch = tszData[i];
        switch (iStatus)
        {
        case 0: // integer part
            if (_T('.') == ch)
            {
                iStatus = 1;
            }
            else if (ch >= _T('0') && ch <= _T('9'))
            {
                iInteger = iInteger * 10 + ch - _T('0');
            }
            else
            {
                goto Exit;
            }
            break;
        case 1: // fraction part
            if (ch >= _T('0') && ch <= _T('9'))
            {
                dFraction += (ch - _T('0')) / pow(10.0, ++iFracNum);
            }
            else
            {
                goto Exit;
            }
            break;
        }
    }

    fRet = true;

Exit:
    if (fRet)
    {
        *pdRet = iSign * (iInteger + dFraction);
    }
    return fRet;
}

//
//TCHAR* StrReplace(const TCHAR* tszInput, const TCHAR* tszOld, const TCHAR* tszNew )
//{
//	// nSourceLen is in XCHARs
//	int nSourceLen = StringTraits::SafeStringLen( pszOld );
//	if( nSourceLen == 0 )
//		return( 0 );
//	// nReplacementLen is in XCHARs
//	int nReplacementLen = StringTraits::SafeStringLen( pszNew );
//
//	// loop once to figure out the size of the result string
//	int nCount = 0;
//	{
//		PCXSTR pszStart = GetString();
//		PCXSTR pszEnd = pszStart+GetLength();
//		while( pszStart < pszEnd )
//		{
//			PCXSTR pszTarget;
//			while( (pszTarget = StringTraits::StringFindString( pszStart, pszOld ) ) != NULL)
//			{
//				nCount++;
//				pszStart = pszTarget+nSourceLen;
//			}
//			pszStart += StringTraits::SafeStringLen( pszStart )+1;
//		}
//	}
//
//	// if any changes were made, make them
//	if( nCount > 0 )
//	{
//		// if the buffer is too small, just
//		//   allocate a new buffer (slow but sure)
//		int nOldLength = GetLength();
//		int nNewLength = nOldLength+(nReplacementLen-nSourceLen)*nCount;
//
//		PXSTR pszBuffer = GetBuffer( __max( nNewLength, nOldLength ) );
//
//		PXSTR pszStart = pszBuffer;
//		PXSTR pszEnd = pszStart+nOldLength;
//
//		// loop again to actually do the work
//		while( pszStart < pszEnd )
//		{
//			PXSTR pszTarget;
//			while( (pszTarget = StringTraits::StringFindString( pszStart, pszOld ) ) != NULL )
//			{
//				int nBalance = nOldLength-int(pszTarget-pszBuffer+nSourceLen);
//                Checked::memmove_s( pszTarget+nReplacementLen, nBalance*sizeof( XCHAR ), 
//					pszTarget+nSourceLen, nBalance*sizeof( XCHAR ) );
//				Checked::memcpy_s( pszTarget, nReplacementLen*sizeof( XCHAR ), 
//					pszNew, nReplacementLen*sizeof( XCHAR ) );
//				pszStart = pszTarget+nReplacementLen;
//				pszTarget[nReplacementLen+nBalance] = 0;
//				nOldLength += (nReplacementLen-nSourceLen);
//			}
//			pszStart += StringTraits::SafeStringLen( pszStart )+1;
//		}
//		ATLASSERT( pszBuffer[nNewLength] == 0 );
//		ReleaseBufferSetLength( nNewLength );
//	}
//
//	return( nCount );
//};

class CSystemInfo : public SYSTEM_INFO 
{
public:
    CSystemInfo() 
    { 
        ::GetSystemInfo(this);
    }
};

class CEvent 
{
public:
    CEvent(bool manualReset, bool initialState)
    {
        m_hEvent = ::CreateEvent(0, manualReset, initialState, 0);
    };

    CEvent(bool manualReset, bool initialState, const TCHAR* tszName)
    {
        m_hEvent = ::CreateEvent(0, manualReset, initialState, tszName);
    };

    virtual ~CEvent()
    {
        if (NULL != m_hEvent &&
            INVALID_HANDLE_VALUE != m_hEvent) 
        {
            ::CloseHandle(m_hEvent);
        }
    };
    
    HANDLE GetEvent() const { return m_hEvent; };
    
    bool Wait() const
    {
        return Wait(INFINITE);
    };
    
    bool Wait(DWORD timeoutMillis) const
    {
        return WAIT_OBJECT_0 == ::WaitForSingleObject(m_hEvent, timeoutMillis);
    };
    
    bool Reset()
    {
        return FALSE != ::ResetEvent(m_hEvent);
    };
    
    bool Set()
    {
        return FALSE != ::SetEvent(m_hEvent);
    };
    
    bool Pulse()
    {
        return FALSE != ::PulseEvent(m_hEvent);
    };
    
private:
    HANDLE m_hEvent;
};

class CAutoEvent : public CEvent
{
public:
    explicit CAutoEvent(bool initialState = false)
        : CEvent(false, initialState) {};
    explicit CAutoEvent(const TCHAR* tszName, bool initialState = false)
        : CEvent(false, initialState, tszName) {};
};

class CManualEvent : public CEvent
{
public:
    explicit CManualEvent(bool initialState = false)
        : CEvent(true, initialState) {};
    explicit CManualEvent(const TCHAR* tszName, bool initialState = false)
        : CEvent(true, initialState, tszName) {};
};

class CCriticalSection 
{
public:
    class CSynchronizer
    {
    public:
        CSynchronizer(
           CCriticalSection &criticalsection)
           : m_criticalsection(criticalsection)
        {
            m_criticalsection.Enter();
        };

        ~CSynchronizer()
        {
            m_criticalsection.Leave();
        };

    private :
        CCriticalSection &m_criticalsection;
    };

    CCriticalSection() { ::InitializeCriticalSection(&m_criticalsection); };
    ~CCriticalSection() { ::DeleteCriticalSection(&m_criticalsection); };

#if(_WIN32_WINNT >= 0x0400)
    bool TryEnter()
    {
        return FALSE != ::TryEnterCriticalSection(&m_criticalsection);
    };
#endif

    void Enter()
    {
        ::EnterCriticalSection(&m_criticalsection);
    };

    void Leave()
    {
        ::LeaveCriticalSection(&m_criticalsection);
    };

private:
    CRITICAL_SECTION m_criticalsection;
};

template <class _Derived>
class CReferencedObject
{
public:
    void AddRef() { ::InterlockedIncrement(&m_lRef); };
    void Release() 
    {
        LONG lRef = ::InterlockedDecrement(&m_lRef);
        if (0 == lRef) 
        {
            static_cast<_Derived*>(this)->OnNoneRef();
        }
    };

protected:
    CReferencedObject() : m_lRef(1) {};
    ~CReferencedObject() {};

protected:
    LONG m_lRef;
};

class CThread 
{
protected:
    HANDLE m_hThread;
    UINT m_nThreadID;
    LONG m_lRunningFlag;
    const bool m_fDeleteWhileExit;

private:
    virtual int Run() = 0;
    static unsigned int __stdcall ThreadFunction(void *pV)
    {
        int nResult = 0;
        CThread* pThis = (CThread*)pV;
        if (pThis)
        {
            InterlockedExchange(&pThis->m_lRunningFlag, 1);
            nResult = pThis->Run();
            InterlockedExchange(&pThis->m_lRunningFlag, 0);
            if (pThis->m_fDeleteWhileExit)
            {
                delete pThis;
            }
        }
        return nResult;
    };

public :
    CThread(bool fDeleteWhileExit = false)
        : m_hThread(INVALID_HANDLE_VALUE), m_nThreadID(0), 
          m_lRunningFlag(0), m_fDeleteWhileExit(fDeleteWhileExit)
    {};

    virtual ~CThread() 
    {
        if (m_hThread != INVALID_HANDLE_VALUE) 
        {
            ::CloseHandle(m_hThread);
        }
    };

    inline HANDLE GetHandle() const { return m_hThread; };

    inline DWORD GetThreadId() const { return m_nThreadID; };

    bool Wait()
    {
        return Wait(INFINITE);
    };

    bool Wait(DWORD timeoutMillis)
    {
        bool fRet = true;
        if (INVALID_HANDLE_VALUE != m_hThread)
        {
            fRet = (WAIT_OBJECT_0 == ::WaitForSingleObject(m_hThread, timeoutMillis));
            if (fRet)
            {
                ::CloseHandle(m_hThread);
                m_hThread = INVALID_HANDLE_VALUE;
            }
        }
        return fRet;
    };

    bool Start()
    {
        if (INVALID_HANDLE_VALUE == m_hThread)
        {
            m_hThread = (HANDLE)::_beginthreadex(
                0, 0, CThread::ThreadFunction, (void*)this, 0, &m_nThreadID);
        }
        return (INVALID_HANDLE_VALUE != m_hThread);
    };

    bool Terminate(DWORD exitCode = 0)
    {
        bool fRet = false;
        if (INVALID_HANDLE_VALUE != m_hThread)
        {
            if (::TerminateThread(m_hThread, exitCode))
            {
                InterlockedExchange(&m_lRunningFlag, 0);
                fRet = true;
            }
            ::CloseHandle(m_hThread);
            m_hThread = INVALID_HANDLE_VALUE;
        }
        return fRet;
    };

    bool IsRunning()
    {
        LONG lFlag = InterlockedExchange(&m_lRunningFlag, m_lRunningFlag);
        return 0 != lFlag;
    };
};

class CRWLock
{
public:
    CRWLock() 
        : m_uiSharedLockCount(0), 
          m_uiExclusiveLockCount(0), 
          m_uiOwnerThreadId(0)
    {
    };

    void ExclusiveLock()
    {
        m_cs.Enter();
        while (true)
        {
            if (m_uiSharedLockCount > 0 ||
                (m_uiExclusiveLockCount > 0 && 
                GetCurrentThreadId() != m_uiOwnerThreadId))
            {
                m_event.Reset();
                m_cs.Leave();
                m_event.Wait();
                m_cs.Enter();
            }
            else
            {
                break;
            }
        }
        ++m_uiExclusiveLockCount;
        m_uiOwnerThreadId = GetCurrentThreadId();
        m_cs.Leave();
    };

    void SharedLock()
    {
        m_cs.Enter();
        while (true)
        {
            if (m_uiExclusiveLockCount > 0 && 
                GetCurrentThreadId() != m_uiOwnerThreadId)
            {
                m_event.Reset();
                m_cs.Leave();
                m_event.Wait();
                m_cs.Enter();
            }
            else
            {
                break;
            }
        }
        ++m_uiSharedLockCount;
        m_cs.Leave();
    };

    void Unlock()
    {
        m_cs.Enter();
        if (m_uiSharedLockCount > 0)
        {
            --m_uiSharedLockCount;
        }
        else if (m_uiExclusiveLockCount > 0)
        {
            --m_uiExclusiveLockCount;
        }
        if (0 == m_uiSharedLockCount && 
            0 == m_uiExclusiveLockCount)
        {
            m_uiOwnerThreadId = 0;
            m_event.Set();
        }
        m_cs.Leave();
    };

private:
    CManualEvent m_event;
    CCriticalSection m_cs;
    UINT m_uiExclusiveLockCount;
    UINT m_uiSharedLockCount;
    UINT m_uiOwnerThreadId;
};

class CThreadGroup
{
protected:
    HANDLE* m_phThread;
    UINT* m_puiThreadID;
    UINT m_uiThreadCount;
    const UINT m_uiMaxThreadCount;
    const UINT m_uiMinThreadCount;
    CRWLock m_lock;

private:
    virtual int Run() = 0;
    static unsigned int __stdcall ThreadFunction(void *pV)
    {
        int iResult = 0;
        CThreadGroup* pThis = (CThreadGroup*)pV;
        if (pThis)
        {
            iResult = pThis->Run();
        }
        return iResult;
    };

public:
    CThreadGroup(UINT uiThreadCount, 
        const UINT uiMaxThreadCount, const UINT uiMinThreadCount = 1) 
        : m_phThread(NULL), m_puiThreadID(NULL),
          m_uiThreadCount(uiThreadCount), 
          m_uiMaxThreadCount(uiMaxThreadCount),
          m_uiMinThreadCount(uiMinThreadCount)
    {
        _ASSERTE(m_uiThreadCount <= m_uiMaxThreadCount &&
               m_uiThreadCount >= m_uiMinThreadCount);
        m_phThread = new HANDLE[m_uiMaxThreadCount];
        _ASSERTE(m_phThread);
        if (NULL != m_phThread)
        {
            for (UINT i=0; i<m_uiMaxThreadCount; ++i)
            {
                m_phThread[i] = INVALID_HANDLE_VALUE;
            }
        }
        m_puiThreadID = new UINT[m_uiMaxThreadCount];
        _ASSERTE(m_puiThreadID);
        if (NULL != m_puiThreadID)
        {
            for (UINT i=0; i<m_uiMaxThreadCount; ++i)
            {
                m_puiThreadID[i] = 0;
            }
        }
    };

    virtual ~CThreadGroup()
    {
        m_lock.ExclusiveLock();
        if (NULL != m_phThread)
        {
            for (UINT i=0; i<m_uiThreadCount; ++i)
            {
                if (m_phThread[i] != INVALID_HANDLE_VALUE) 
                {
                    ::CloseHandle(m_phThread[i]);
                }
            }
            delete [] m_phThread;
        }
        if (NULL != m_puiThreadID)
        {
            delete [] m_puiThreadID;
        }
        m_lock.Unlock();
    };

    inline UINT GetThreadCount() { return (UINT)InterlockedExchange((LONG*)&m_uiThreadCount, (LONG)m_uiThreadCount); };
    inline const UINT GetMaxThreadCount() { return m_uiMaxThreadCount; };

    bool CreateThreads(UINT uiNewThreadCount)
    {
        bool fRet = false;
        m_lock.ExclusiveLock();

        if (m_uiThreadCount + uiNewThreadCount > 
            m_uiMaxThreadCount)
        {
            goto Exit;
        }

        UINT uiThreadId = 0;
        for (UINT i=0; i<uiNewThreadCount; ++i)
        {
            m_phThread[i + m_uiThreadCount] = (HANDLE)::_beginthreadex(
                0, 0, CThreadGroup::ThreadFunction, (void*)this, 0, &uiThreadId);
            if (INVALID_HANDLE_VALUE == m_phThread[i + m_uiThreadCount])
            {
                goto Exit;
            }
            m_puiThreadID[i + m_uiThreadCount] = uiThreadId;
        }

        m_uiThreadCount += uiNewThreadCount;
        fRet = true;

    Exit:
        m_lock.Unlock();
        return fRet;
    };

    bool KillSelfThread()
    {
        bool fRet = false;
        m_lock.ExclusiveLock();

        if (m_uiThreadCount <= m_uiMinThreadCount)
        {
            goto Exit;
        }

        UINT uiThreadId = GetCurrentThreadId();
        for (UINT i=0; i<m_uiThreadCount; ++i)
        {
            if (uiThreadId == m_puiThreadID[i])
            { // found it!
                CloseHandle(m_phThread[i]);
                // trim all handle after hThread
                for (UINT j=i; j<m_uiThreadCount - 1; ++j)
                {
                    m_phThread[j] = m_phThread[j+1];
                    m_puiThreadID[j] = m_puiThreadID[j+1];
                }
                m_phThread[m_uiThreadCount - 1] = INVALID_HANDLE_VALUE;
                m_puiThreadID[m_uiThreadCount - 1] = 0;
                --m_uiThreadCount;
                fRet = true;
                break;
            }
        }

    Exit:
        m_lock.Unlock();
        return fRet;
    };

    inline bool Wait()
    {
        return Wait(INFINITE);
    };

    bool Wait(DWORD timeoutMillis)
    {
        bool fRet = false;
        if (NULL == m_phThread)
        {
            goto Exit;
        }
        m_lock.SharedLock();
        HANDLE* phThread = new HANDLE[m_uiThreadCount];
        if (NULL == phThread)
        {
            goto Exit;
        }
        CopyMemory(phThread, m_phThread, m_uiThreadCount * sizeof(HANDLE));
        m_lock.Unlock();

        fRet = (WAIT_TIMEOUT != ::WaitForMultipleObjects(m_uiThreadCount, phThread, TRUE, timeoutMillis));

        delete [] phThread;
        phThread = NULL;
        if (!fRet)
        {
            goto Exit;
        }

        m_lock.ExclusiveLock();
        for (UINT i=0; i<m_uiThreadCount; ++i)
        {
            if (m_phThread[i] != INVALID_HANDLE_VALUE) 
            {
                ::CloseHandle(m_phThread[i]);
                m_phThread[i] = INVALID_HANDLE_VALUE;
            }
        }
        m_lock.Unlock();

    Exit:
        return fRet;
    };

    bool Start()
    {
        bool fRet = true;
        UINT uiThreadId = 0;
        m_lock.ExclusiveLock();
        for (UINT i=0; i<m_uiThreadCount; ++i)
        {
            if (INVALID_HANDLE_VALUE == m_phThread[i])
            {
                m_phThread[i] = (HANDLE)::_beginthreadex(
                    0, 0, CThreadGroup::ThreadFunction, (void*)this, 0, &uiThreadId);
                if (INVALID_HANDLE_VALUE == m_phThread[i])
                {
                    fRet = false;
                    break;
                }
                m_puiThreadID[i] = uiThreadId;
            }
        }
        m_lock.Unlock();
        return fRet;
    };

    bool Terminate(DWORD exitCode = 0)
    {
        bool fRet = false;
        m_lock.ExclusiveLock();
        for (UINT i=0; i<m_uiThreadCount; ++i)
        {
            if (m_phThread[i] != INVALID_HANDLE_VALUE) 
            {
                if (::TerminateThread(m_phThread[i], exitCode))
                {
                    fRet = true;
                    m_phThread[i] = INVALID_HANDLE_VALUE;
                }
            }
        }
        m_lock.Unlock();
        return fRet;
    };
};

template <class T>
class CSingleton
{
public:
	static T* Instance()
	{
		static T instance;
		return &instance;
	}

protected:
	CSingleton(){};
	virtual ~CSingleton(){};

private:
	//! Copy constructor.
    CSingleton(const CSingleton& source);
	CSingleton& operator =(const CSingleton&);
};

template <typename _T>
class CFixedSet
{
public:
    CFixedSet(UINT uiSize) : m_uiSize(uiSize)
    {
    };

    bool Exists(_T& key)
    {
        return m_set.end() != m_set.find(key);
    };

    bool Insert(_T& key)
    {
        _ASSERTE(m_queue.size() == m_set.size());

        pair<set<_T>::iterator, bool> pr = m_set.insert(key);
        bool fRet = pr.second;
        if (fRet)
        {
            m_queue.push(key);

            while (m_queue.size() >= m_uiSize)
            {
                _T& old = m_queue.front();
                m_set.erase(old);
                m_queue.pop();
            }
        }

        return fRet;
    };

private:
    set<_T> m_set;
    UINT m_uiSize;
    queue<_T> m_queue;
};

template <typename _T>
class CSafeQueue
{
public:
    CSafeQueue(const UINT uiMaxSize) 
        : m_uiMaxSize(uiMaxSize), m_event(false), m_lShutdownFlag(0)
    {
        _ASSERTE(m_uiMaxSize > 1);
    };

    ~CSafeQueue()
    {
        this->Shutdown();
    };

    bool Push(const _T& val, DWORD dwTimeout = INFINITE, bool jumpHead = false)
    {
        return this->Push(&val, 1, NULL, dwTimeout, jumpHead);
    };

    bool Push(const _T* arrVals, UINT uiSize, UINT* puiPushed = NULL, DWORD dwTimeout = INFINITE, bool fHead = false)
    {
        bool fRet = false;

        if (NULL == arrVals || uiSize > m_uiMaxSize)
        {
            goto Exit;
        }

        LONG lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
        if (0 != lShutdownFlag)
        {
            goto Exit;
        }

        m_cs.Enter();

        while (m_queue.size() + uiSize > m_uiMaxSize)
        { // if cannot push all, then wait
            m_event.Reset();
            m_cs.Leave();

            fRet = m_event.Wait(dwTimeout);
            if (!fRet)
            { // if wait timeout, then decide if force push
                m_cs.Enter();
                if (NULL != puiPushed)
                {
                    // get the size need to force push
                    if (m_queue.size() + uiSize > m_uiMaxSize)
                    {
                        uiSize = m_uiMaxSize - m_queue.size();
                    }
                    // if size > 0, that means has something to push
                    // so, just force push it and dont forget to set the event to unblock pop
                    // else, function return false
                    if (uiSize > 0)
                    {
                        for (UINT i=0; i<uiSize; ++i)
                        {
                            if (fHead)
                            {
                                m_queue.push_front(arrVals[i]);
                            }
                            else
                            {
                                m_queue.push_back(arrVals[i]);
                            }
                            ++(*puiPushed);
                        }
                        m_event.Set();
                        fRet = true;
                    }
                }
                m_cs.Leave();
                goto Exit;
            }

            lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
            if (0 != lShutdownFlag)
            {
                fRet = false;
                goto Exit;
            }

            // re-enter the cs to check the size to see if can do push now
            m_cs.Enter();
        }

        for (UINT i=0; i<uiSize; ++i)
        {
            if (fHead)
            {
                m_queue.push_front(arrVals[i]);
            }
            else
            {
                m_queue.push_back(arrVals[i]);
            }
            if (NULL != puiPushed)
            {
                ++(*puiPushed);
            }
        }
        // dont forget to set the event to unblock pop
        m_event.Set();

        m_cs.Leave();

        fRet = true;

    Exit:
        return fRet;
    };

    _T& Front()
    {
        CCriticalSection::CSynchronizer lock(m_cs);
        return m_queue.front();
    };

    _T& Back()
    {
        CCriticalSection::CSynchronizer lock(m_cs);
        return m_queue.back();
    };

    bool Pop(DWORD dwTimeout = INFINITE, bool fTail = false)
    {
        return this->Pop(1, NULL, dwTimeout, fTail);
    };

    bool Pop(_T* pVal, DWORD dwTimeout = INFINITE, bool fTail = false)
    {
        bool fRet = false;

        if (NULL == pVal)
        {
            goto Exit;
        }

        LONG lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
        if (0 != lShutdownFlag)
        {
            goto Exit;
        }

        m_cs.Enter();

        while (m_queue.size() == 0)
        { // if cannot pop, then wait
            m_event.Reset();
            m_cs.Leave();

            fRet = m_event.Wait(dwTimeout);
            if (!fRet)
            {
                goto Exit;
            }

            lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
            if (0 != lShutdownFlag)
            {
                fRet = false;
                goto Exit;
            }

            m_cs.Enter();
        }

        if (fTail)
        {
            MoveMemory(pVal, &m_queue.back(), sizeof(_T));
            m_queue.pop_back();
        }
        else
        {
            MoveMemory(pVal, &m_queue.front(), sizeof(_T));
            m_queue.pop_front();
        }
        m_event.Set();

        m_cs.Leave();

        fRet = true;

    Exit:
        return fRet;
    };

    bool Pop(UINT uiCount, UINT* puiPopped = NULL, DWORD dwTimeout = INFINITE, bool fTail = false)
    {
        bool fRet = false;

        if (0 == uiCount || m_uiMaxSize < uiCount)
        {
            goto Exit;
        }

        LONG lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
        if (0 != lShutdownFlag)
        {
            goto Exit;
        }

        m_cs.Enter();

        while (m_queue.size() < uiCount)
        { // if cannot pop all, then wait
            m_event.Reset();
            m_cs.Leave();

            fRet = m_event.Wait(dwTimeout);
            if (!fRet)
            {
                m_cs.Enter();
                if (NULL != puiPopped)
                {
                    // get the size need to force pop
                    if (m_queue.size() < uiCount)
                    {
                        uiCount = m_queue.size();
                    }
                    // if count > 0, that means has something to pop
                    // so, just force pop it and dont forget to set the event to unblock push
                    // else, function return false
                    if (uiCount > 0)
                    {
                        for (UINT i=0; i<uiCount; ++i)
                        {
                            if (fTail)
                            {
                                m_queue.pop_back();
                            }
                            else
                            {
                                m_queue.pop_front();
                            }
                            ++(*puiPopped);
                        }
                        m_event.Set();
                        fRet = true;
                    }
                }
                m_cs.Leave();
                goto Exit;
            }

            lShutdownFlag = InterlockedExchange(&m_lShutdownFlag, m_lShutdownFlag);
            if (0 != lShutdownFlag)
            {
                fRet = false;
                goto Exit;
            }

            m_cs.Enter();
        }

        for (UINT i=0; i<uiCount; ++i)
        {
            if (fTail)
            {
                m_queue.pop_back();
            }
            else
            {
                m_queue.pop_front();
            }
            if (NULL != puiPopped)
            {
                ++(*puiPopped);
            }
        }
        m_event.Set();

        m_cs.Leave();

        fRet = true;

    Exit:
        return fRet;
    };

    void Clear()
    {
        CCriticalSection::CSynchronizer lock(m_cs);
        if (m_queue.size() == m_uiMaxSize)
        {
            m_event.Set();
        }
        m_queue.clear();
    };

    UINT Size()
    {
        CCriticalSection::CSynchronizer lock(m_cs);
        return (UINT) m_queue.size();
    };

    inline const UINT MaxSize()
    {
        return m_uiMaxSize;
    };

    void Shutdown()
    {
        InterlockedExchange(&m_lShutdownFlag, 1);
        m_event.Set();
    };

    void Reset()
    {
        InterlockedExchange(&m_lShutdownFlag, 0);
    };

private:
    list<_T> m_queue;
    CCriticalSection m_cs;
    CManualEvent m_event;

    LONG m_lShutdownFlag;

    const UINT m_uiMaxSize;
};

class CStrTok2
{
private:
    char* m_szData;
    char m_szDelimit[32];
public:
    CStrTok2(char* szData, const char* szDelimit)
    {
        m_szData = szData;
        for (int i=0; i<32; ++i) m_szDelimit[i] = 0;
        char ch = 0;
        while ((ch = *(szDelimit++)) != '\0')
        {
            m_szDelimit[ch >> 3] |= 1 << (ch & 7);
        }
    }
    ~CStrTok2()
    {
    }

    int IsDelimit(char ch)
    {
        return m_szDelimit[ch >> 3] & 1 << (ch & 7);
    }

    char* GetNext()
    {
        if (!m_szData) return NULL;

        char* szStart = m_szData;

        char* szTmp = m_szData;
        int iDecodeCount = 0;
        while (true)
        {
            *szTmp = *(szTmp + iDecodeCount);
            char ch = *szTmp;
            if (ch == '\0')
            {
                m_szData = NULL;
                break;
            }

            if (ch == '\\')
            {
                ++iDecodeCount;
                *szTmp = *(szTmp + iDecodeCount);
            }
            else if (IsDelimit(ch))
            {
                *szTmp = '\0';
                m_szData = szTmp + iDecodeCount + 1;
                break;
            }
            ++szTmp; // if (ch == '\\') then skip the char after '\\'
        }

        return szStart;
    }
};

class CStrTok
{
public:
    CStrTok(TCHAR* tszData, const TCHAR tchDelimit)
        : m_tszData(tszData), m_tchDelimit(tchDelimit)
    {
    };

    CStrTok() : m_tszData(NULL), m_tchDelimit(0)
    {
    };

    void Initialize(TCHAR* tszData, const TCHAR tchDelimit)
    {
        m_tszData = tszData;
        m_tchDelimit = tchDelimit;
    };

    TCHAR* GetNext()
    {
        TCHAR* tszRet = m_tszData;
        if (NULL != tszRet)
        {
            TCHAR* tszTmp = _tcschr(m_tszData, m_tchDelimit);
            if (NULL == tszTmp)
            {
                m_tszData = NULL;
            }
            else
            {
                *tszTmp = TEXT('\0');
                m_tszData = tszTmp + 1;
            }
        }
        return tszRet;
    };

    TCHAR* GetNextEncoded()
    {
        TCHAR* tszRet = GetNext();
        if (NULL != tszRet)
        {
            TCHAR* tszTmp = tszRet;
            while (NULL != tszTmp)
            {
                // $TODO: Remove the hard code - 1024
                UINT uiSize = 0;
                StringCchLength(tszTmp, 1024, &uiSize);
                if (TEXT('\\') == tszTmp[uiSize - 1])
                {
                    tszTmp[uiSize] = m_tchDelimit;
                    m_tszData = tszTmp + uiSize + 1;
                    tszTmp = GetNext();
                }
                else
                {
                    break;
                }
            }
        }
        return tszRet;
    };

#ifndef UNICODE
    static void DecodeString(CHAR* szData, const CHAR* szDelimits)
    {
        if (NULL == szDelimits || NULL == szData)
        {
            return;
        }
        
        BYTE map[32];
        ZeroMemory(map, sizeof(map));
        do
        {
            map[(BYTE)*szDelimits >> 3] |= (1 << ((BYTE)*szDelimits & 7));
        } while (*(++szDelimits));

        // Decoding: just replace all '\X' with 'X'
        CHAR* szTmp = szData;
        int iDecCount = 0;
        while (*szTmp != '\0')
        {
            BYTE by = (BYTE)*(szTmp + iDecCount + 1);
            // if need decode, remove the '\\' here
            if ('\\' == *szTmp && 
                (map[by >> 3] & (1 << (by & 7))) )
            {
                ++iDecCount;
            }
            else
            {
                ++szTmp;
            }
            *szTmp = *(szTmp + iDecCount);
        }
    };

    static CHAR* EncodeString(const CHAR* szData, const CHAR* szDelimits)
    {
        if (NULL == szDelimits || NULL == szData)
        {
            return NULL;
        }
        
        UINT uiSize = 0;
        StringCchLengthA(szData, 1024, &uiSize);
        CHAR* szRet = new CHAR[2*uiSize + 1];
        if (NULL == szRet)
        {
            return NULL;
        }
        ZeroMemory(szRet, 2*uiSize + 1);

        const CHAR* szTmp = szData;
        BYTE map[32];
        /* Clear control map */
        ZeroMemory(map, sizeof(map));
        /* Set bits in delimiter table */
        do
        {
            map[(BYTE)*szDelimits >> 3] |= (1 << ((BYTE)*szDelimits & 7));
        } while (*(++szDelimits));

        /* Find the end of the token. If it is not the end of the string, put a null there. */
        CHAR chars[3] = {0};
        chars[0] = '\\';
        for ( ; *szTmp ; szTmp++ )
        {
            if ( map[(BYTE)*szTmp >> 3] & (1 << ((BYTE)*szTmp & 7)) ) 
            {
                chars[1] = *szTmp;
                StringCchCatNA(szRet, 2*uiSize + 1, szData, szTmp - szData);
                StringCchCatA(szRet, 2*uiSize + 1, chars);
                szData = szTmp + 1;
            }
        }
        StringCchCatA(szRet, 2*uiSize + 1, szData);
        return szRet;
    };
#else
    static void DecodeString(WCHAR* wszData, const WCHAR* wszDelimits)
    {
        if (NULL == wszDelimits || NULL == wszData)
        {
            return;
        }

        // Decoding: just replace all '\X' with 'X'
        WCHAR* wszTmp = wszData;
        int iDecCount = 0;
        while (*wszTmp != L'\0')
        {
            BYTE by = (BYTE)*(wszTmp + iDecCount + 1);
            // if need decode, remove the '\\' here
            if (L'\\' == *wszTmp)
            {
                const WCHAR* wszTmp2 = wszDelimits;
                while (*wszTmp2 != L'\0')
                {
                    if (*wszTmp == *(wszTmp2++))
                    {
                        ++iDecCount;
                        break;
                    }
                }
            }
            else
            {
                ++wszTmp;
            }
            *wszTmp = *(wszTmp + iDecCount);
        }
    };

    static WCHAR* EncodeString(const WCHAR* wszData, const WCHAR* wszDelimits)
    {
        if (NULL == wszDelimits || NULL == wszData)
        {
            return NULL;
        }
    
        UINT uiSize = 0;
        StringCchLengthW(wszData, 1024, &uiSize);
        WCHAR* wszRet = new WCHAR[2*uiSize + 1];
        if (NULL == wszRet)
        {
            return NULL;
        }
        ZeroMemory(wszRet, sizeof(WCHAR) * (2*uiSize + 1));

        const WCHAR* wszTmp = wszData;
        /* Find the end of the token. If it is not the end of the string, put a null there. */
        WCHAR wchars[3] = {0};
        wchars[0] = L'\\';
        for ( ; *wszTmp ; ++wszTmp )
        {
            const WCHAR* wszTmp2 = wszDelimits;
            while (*wszTmp2 != L'\0')
            {
                if (*(wszTmp2++) == *wszTmp)
                {
                    wchars[1] = *wszTmp;
                    StringCchCatNW(wszRet, 2*uiSize + 1, wszData, wszTmp - wszData);
                    StringCchCatW(wszRet, 2*uiSize + 1, wchars);
                    wszData = wszTmp + 1;
                    break;
                }
            }
        }
        StringCchCatW(wszRet, 2*uiSize + 1, wszData);
        return wszRet;
    };
#endif

    TCHAR* GetNextDecoded(const TCHAR* tszDelimits = NULL)
    {
        TCHAR* tszRet = GetNextEncoded();
        if (NULL != tszRet)
        {
            if (NULL == tszDelimits)
            {
                TCHAR tszTemp[2] = {0};
                tszTemp[0] = m_tchDelimit;
                DecodeString(tszRet, tszTemp);
            }
            else
            {
                DecodeString(tszRet, tszDelimits);
            }
        }
        return tszRet;
    };

    TCHAR* GetRest()
    {
        return m_tszData;
    };

private:
    TCHAR* m_tszData;
    TCHAR m_tchDelimit;
};

static TCHAR* StrTokEnc(TCHAR* tszData, const TCHAR tchDelimit)
{
    TCHAR* tszContext = NULL;
    TCHAR tszDelimit[2] = {tchDelimit, TEXT('\0')};
    TCHAR* tszRet = _tcstok_s(tszData, tszDelimit, &tszContext);
    TCHAR* tszTmp = tszRet;
    while (NULL != tszTmp)
    {
        // $TODO: Remove the hard code - 1024
        UINT uiSize = 0;
        StringCchLength(tszTmp, 1024, &uiSize);
        if (TEXT('\\') == tszTmp[uiSize - 1])
        {
            tszTmp[uiSize] = tchDelimit;
            tszData = tszTmp + uiSize + 1;
            tszTmp = _tcstok_s(tszData, tszDelimit, &tszContext);
        }
        else
        {
            break;
        }
    }

    return tszRet;
};

template <typename _QueueNodeType>
class CThreadPool : public CThreadGroup
{
public:
    CThreadPool(UINT uiMaxQueueSize, const UINT uiQueueWaitTimeout,
        const UINT uiIdleTimeoutCount, const UINT uiFeedTimeout,
        UINT uiThreadCount, 
        const UINT uiMaxThreadCount, const UINT uiMinThreadCount = 1) 
        : CThreadGroup(uiThreadCount, uiMaxThreadCount, uiMinThreadCount), 
           m_inq(uiMaxQueueSize), m_uiQueueWaitTimeout(uiQueueWaitTimeout),
           m_uiIdleTimeoutCount(uiIdleTimeoutCount), m_uiFeedTimeout(uiFeedTimeout),
           /*m_lPoolFlag(0), */m_lShutDownFlag(0), m_lWaitThreadCount(0)
    {
    };

    virtual ~CThreadPool()
    {
        this->Shutdown();
    };

    void Shutdown()
    {
        if (0 == InterlockedExchange(&m_lShutDownFlag, 1))
        {
            m_inq.Shutdown();
            if (!this->Wait(3000))
            {
                this->Terminate();
            }
            this->_CleanQueue();
        }
    };

    bool Feed(_QueueNodeType& qnode)
    {
        // if no waiting thread, just create one
        if (0 == InterlockedExchange(&m_lWaitThreadCount, m_lWaitThreadCount))
        {
            // [BUG FIX] Dont return false while failed to CreateThreads
            // instead, if create threads failed, just push the node anyway!
            //// so that one of the threads in the thread pool will handle it later.
            //if (!this->CreateThreads(1))
            //{
            //    return false;
            //}
            this->CreateThreads(1);
        }
        return m_inq.Push(qnode, m_uiFeedTimeout);
    };

private:
    virtual void OnThreadStart() {};
    virtual void OnThreadStop() {};
    virtual void OnThreadTimeout(){};
    virtual void OnProcessQueueNode(_QueueNodeType& qnode) = 0;
    virtual void OnReleaseQueueNode(_QueueNodeType& qnode) = 0;

    void _CleanQueue()
    {
        m_inq.Reset();
        _QueueNodeType qnode;
        while (m_inq.Pop(&qnode, 0))
        {
            this->OnReleaseQueueNode(qnode);
        }
    };

    int Run()
    {
        this->OnThreadStart();

        UINT uiTimeoutCount = 0;
        //InterlockedExchange(&m_lPoolFlag, 0);
        InterlockedExchange(&m_lShutDownFlag, 0);
        while (true)
        {
            _QueueNodeType qnode;
            InterlockedIncrement(&m_lWaitThreadCount);
            bool fTimeout = !m_inq.Pop(&qnode, m_uiQueueWaitTimeout);
            InterlockedDecrement(&m_lWaitThreadCount);
            if (1 == InterlockedExchange(&m_lShutDownFlag, m_lShutDownFlag))
            {
                break;
            }
            if (!fTimeout)
            {
                //// check the queue size and
                //// adjust the pool size if necessary
                //// use m_lResendFlag as CCriticalSection::TryEnter(), cool! :-)
                //// ensure only one thread can enter here
                //if (1 == InterlockedIncrement(&m_lPoolFlag))
                //{
                //    UINT uiQSize = m_inq.Size();
                //    if (uiQSize > this->GetThreadCount())
                //    {
                //        UINT uiNewThreadCount = (uiQSize - this->GetThreadCount()) / 2;
                //        if (0 == uiNewThreadCount)
                //        {
                //            uiNewThreadCount = 1
                //        }
                //        this->CreateThreads(uiNewThreadCount);
                //    }
                //    InterlockedExchange(&m_lPoolFlag, 0);
                //}
                this->OnProcessQueueNode(qnode);
                uiTimeoutCount = 0;
            }
            else
            {
                this->OnThreadTimeout();
                ++uiTimeoutCount;
            }

            // if this thread is idle then try to kill itself
            if (uiTimeoutCount > m_uiIdleTimeoutCount)
            {
                // when every thread exiting, we should update the thread count
                // and adjust the handle array by calling KillSelfThread()
                if (this->KillSelfThread())
                {
                    //LOGGER << SV_DEBUG << TEXT("CThreadPool::Run - Self killed!") << ENDL;
                    break;
                }
            }
        }
        this->OnThreadStop();
        return 0;
    };

private:
    CSafeQueue<_QueueNodeType> m_inq;
    //LONG m_lPoolFlag;
    LONG m_lWaitThreadCount;
    LONG m_lShutDownFlag;
    const UINT m_uiQueueWaitTimeout;
    const UINT m_uiIdleTimeoutCount;
    const UINT m_uiFeedTimeout;
};

class CFilePrintf
{
public:
    CFilePrintf() : m_tszTemplate(NULL) {};
    CFilePrintf(const TCHAR* tszTemplate) : m_tszTemplate(tszTemplate) {};
    ~CFilePrintf()
    {
        while (m_listParams.size() > 0)
        {
            const TCHAR* tszParam = m_listParams.front();
            m_listParams.pop_front();
            delete [] tszParam;
        }
    };

    void AddParam(const TCHAR* tszParam)
    {
        size_t len = _tcslen(tszParam);
        if (len > 0)
        {
            TCHAR* tszNew = new TCHAR[len + 1];
            if (tszNew)
            {
                tszNew[len] = _T('\0');
                _tcsncpy_s(tszNew, len + 1, tszParam, len);
                m_listParams.push_back(tszNew);
            }
        }
        else
        {
            m_listParams.push_back(NULL);
        }
    };

    bool PrintfToFile(const TCHAR* tszFilePath)
    {
        bool fRet = false;

        FILE* f = NULL;
#ifdef UNICODE
        errno_t err = _tfopen_s(&f, tszFilePath, _T("w, ccs=UNICODE"));
#else
        errno_t err = _tfopen_s(&f, tszFilePath, _T("w, ccs=ANSI"));
#endif
        if (err || !f)
        {
            goto Exit;
        }

        size_t sizeWrite = 0;
        const TCHAR* s = m_tszTemplate;
        const TCHAR* p = NULL;
        while (p = _tcsstr(s, _T("%s")))
        {
            sizeWrite = fwrite(s, sizeof(TCHAR), p - s, f);
            if (sizeWrite != p - s)
            {
                goto Error;
            }
            if (m_listParams.size() == 0)
            {
                goto Error;
            }
            const TCHAR* tszParam = m_listParams.front();
            m_listParams.pop_front();
            if (tszParam)
            {
                sizeWrite = fwrite(tszParam, sizeof(TCHAR), _tcslen(tszParam), f);
                if (sizeWrite != _tcslen(tszParam))
                {
                    delete [] tszParam;
                    goto Error;
                }
                delete [] tszParam;
            }
            s = p + 2;
        }

        sizeWrite = fwrite(s, sizeof(TCHAR), _tcslen(s), f);
        if (sizeWrite != _tcslen(s))
        {
            goto Error;
        }

        fclose(f);
        fRet = true;

    Exit:
        return fRet;

    Error:
        fclose(f);
        goto Exit;
    };

private:
    const TCHAR* m_tszTemplate;
    list<const TCHAR*> m_listParams;
};

} // namespace UTIL
} // namespace CS

#endif
