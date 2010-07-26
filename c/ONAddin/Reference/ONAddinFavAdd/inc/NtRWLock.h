#pragma once

class CNtRWLock
{
private:
    // RTL_RWLOCK structure.  This is the
    // structure used by ntdll.dll to handle
    // the MRSW resource.
    struct RTL_RWLOCK
    {
        CRITICAL_SECTION    mCS;
        HANDLE              mSem1;
        DWORD               mSharedWaiters;
        HANDLE              mSem2;
        DWORD               mExclusiveWaiters;
        DWORD               mNumberActive;
        DWORD               mOwningThreadId;
        DWORD               mU2;
        PVOID               mDebugInfo;
    };

    typedef RTL_RWLOCK*     PRTL_RWLOCK;

    RTL_RWLOCK              m_lock;

    // Function prototypes for MRSW resources.
    typedef VOID (WINAPI *RtlInitializeResource)(PRTL_RWLOCK);
    typedef VOID (WINAPI *RtlDeleteResource)(PRTL_RWLOCK);
    typedef BYTE (WINAPI *RtlAcqResourceExclusive)(PRTL_RWLOCK, BYTE);
    typedef BYTE (WINAPI *RtlAcqResourceShared)(PRTL_RWLOCK, BYTE);
    typedef VOID (WINAPI *RtlReleaseResource)(PRTL_RWLOCK);
    typedef VOID (WINAPI *RtlDumpResource)(PRTL_RWLOCK);

    // Names for functions as exported from ntdll.dll.
    static LPCSTR s_szInitName;
    static LPCSTR s_szDeleteName;
    static LPCSTR s_szGetExclusiveName;
    static LPCSTR s_szGetSharedName;
    static LPCSTR s_szReleaseName;
    static LPCSTR s_szDumpName;

    // Static members containing function pointers.
    static RtlInitializeResource s_pInitProc;
    static RtlDeleteResource s_pDelProc;
    static RtlAcqResourceExclusive s_pGetExclusiveProc;
    static RtlAcqResourceShared s_pGetSharedProc;
    static RtlReleaseResource s_pReleaseProc;
    static RtlDumpResource s_pDumpProc;

public:
    CNtRWLock();
    ~CNtRWLock();
    // Public members to lock and
    // release the MRSW resource.
    void ExclusiveLock();
    void SharedLock();
    void Unlock();

    class CLocker
    {
    public:
        CLocker(CNtRWLock* pLock, bool fShared) : m_pLock(pLock)
        {
            if (fShared)
            {
                m_pLock->SharedLock();
            }
            else
            {
                m_pLock->ExclusiveLock();
            }
        };

        ~CLocker()
        {
            m_pLock->Unlock();
        };

    private:
        CNtRWLock* m_pLock;
    };
};
