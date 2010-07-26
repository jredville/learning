#include "StdAfx.h"
#include ".\ntrwlock.h"

// Function names exported from ntdll.dll.
LPCSTR CNtRWLock::s_szInitName = "RtlInitializeResource";
LPCSTR CNtRWLock::s_szDeleteName = "RtlDeleteResource";
LPCSTR CNtRWLock::s_szGetExclusiveName = "RtlAcquireResourceExclusive";
LPCSTR CNtRWLock::s_szGetSharedName = "RtlAcquireResourceShared";
LPCSTR CNtRWLock::s_szReleaseName = "RtlReleaseResource";
LPCSTR CNtRWLock::s_szDumpName = "RtlDumpResource";

// Function pointers into ntdll.dll.
CNtRWLock::RtlInitializeResource CNtRWLock::s_pInitProc;
CNtRWLock::RtlDeleteResource CNtRWLock::s_pDelProc;
CNtRWLock::RtlAcqResourceExclusive CNtRWLock::s_pGetExclusiveProc;
CNtRWLock::RtlAcqResourceShared CNtRWLock::s_pGetSharedProc;
CNtRWLock::RtlReleaseResource CNtRWLock::s_pReleaseProc;
CNtRWLock::RtlDumpResource CNtRWLock::s_pDumpProc;

CNtRWLock::CNtRWLock()
{
    // If this is the first object constructed, fill in the statics.
    static HINSTANCE hNtdll = NULL;
    if (hNtdll == NULL)
    {
        hNtdll = LoadLibraryA("ntdll.dll");
        s_pInitProc = (RtlInitializeResource) GetProcAddress(hNtdll, s_szInitName);
        s_pDelProc = (RtlDeleteResource) GetProcAddress(hNtdll, s_szDeleteName);
        s_pGetExclusiveProc = (RtlAcqResourceExclusive) GetProcAddress(hNtdll, s_szGetExclusiveName);
        s_pGetSharedProc = (RtlAcqResourceShared) GetProcAddress(hNtdll, s_szGetSharedName);
        s_pReleaseProc = (RtlReleaseResource) GetProcAddress(hNtdll, s_szReleaseName);
        s_pDumpProc = (RtlDumpResource) GetProcAddress(hNtdll, s_szDumpName);
    }
    s_pInitProc(&m_lock);
};

CNtRWLock::~CNtRWLock()
{
    s_pDelProc(&m_lock);
};

// Public members to lock and
// release the MRSW resource.
void CNtRWLock::ExclusiveLock()
{
    s_pGetExclusiveProc(&m_lock, 1);
};
void CNtRWLock::SharedLock()
{
    s_pGetSharedProc(&m_lock, 1);
};
void CNtRWLock::Unlock()
{
    s_pReleaseProc(&m_lock);
};
