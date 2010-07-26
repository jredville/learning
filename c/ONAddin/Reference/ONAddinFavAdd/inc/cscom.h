
///////////////////////////////////////////////////////////////////////////////
//
// CS COM classes library - Version 1.00
// --------------------------------------------------------
// Copyright (C) 2007, by Shu Chen (shuchen.net@gmail.com).
//
///////////////////////////////////////////////////////////////////////////////

#if !defined(_CSCOM_)
#define _CSCOM_

#pragma once

#include <objbase.h>
#include <strsafe.h>

#define CLASSOFFSET(Base, Derived) ((DWORD_PTR)(static_cast<Base*>((Derived*)8))-8)

#define BEGIN_COM_MAP(x) public: \
    typedef x _ComClass; \
    static const ComIntMapEntry* WINAPI _GetEntries() { \
    static const ComIntMapEntry _entries[] = {{&__uuidof(IUnknown), 0},

#define COM_INTERFACE_ENTRY(x) {&__uuidof(x), CLASSOFFSET(x, _ComClass)},

#define END_COM_MAP {NULL, 0}}; return &_entries[0];}

#define BEGIN_COCLASS_MAP public: \
    static const CoClassEntry* WINAPI _GetEntries() { \
    static const CoClassEntry _entries[] = {

#define COCLASS_ENTRY(clsid, x) {&clsid, new CClassFactory<x>},

#define END_COCLASS_MAP {NULL, NULL}}; return &_entries[0];}

namespace CS
{
namespace COM
{

struct ComIntMapEntry
{
    const IID* pIID;
    DWORD_PTR offset;
};

class CSingleThreadedModel
{
public:
    static STDMETHODIMP_(ULONG) Increment(long &lRef)
    {
        lRef ++;
        return lRef;
    }

    static STDMETHODIMP_(ULONG) Decrement(long &lRef)
    {
        lRef --; 
        return lRef; 
    }
};

class CMultiThreadedModel
{
public:
    static STDMETHODIMP_(ULONG) Increment(long &lRef) 
    { 
        ::InterlockedIncrement(&lRef); 
        return lRef; 
    }

    static STDMETHODIMP_(ULONG) Decrement(long &lRef) 
    {
        ::InterlockedDecrement(&lRef); 
        return lRef; 
    }
};

template < class ComClass > class CComObject;

template<class ComClass>
class CSingleCreator
{
public:
    static CComObject<ComClass>* m_pObj;
    static CComObject<ComClass>* CreateObject()
    {
        if(!m_pObj)
        {
            m_pObj = new CComObject<ComClass>;
        }
        else
        {
            m_pObj->AddRef();
        }
        return m_pObj;
    }
};
//template<class ComClass> CComObject<ComClass>* CSingleCreator::m_pObj = NULL;

template<class ComClass>
class CMultiCreator
{
public:
    static CComObject<ComClass>* CreateObject()
    {
        return new CComObject<ComClass>;
    }
};

template < class ThreadModel, class CreateClass >
class CComBase
{
public:
    typedef ThreadModel _ThreadModel;
    typedef CreateClass _Creator;

    virtual ~CComBase() {};
};

template < class ComClass >
class CComObject : public ComClass
{
protected:
    long m_lRef;

public:
    CComObject() : m_lRef(0) { };
    virtual ~CComObject() {};

public:
    STDMETHOD(QueryInterface)(REFIID riid, LPVOID *ppv)
    {
        printf("CComObject::QueryInterface\n");
        *ppv = NULL;
        const ComIntMapEntry* pEntries = _GetEntries();
        while (pEntries->pIID)
        {
            if(IsEqualIID(riid, *(pEntries->pIID)))
            {
                *ppv = (LPBYTE)(this) + pEntries->offset;
                AddRef();
                return S_OK;
            }
            ++pEntries;
        }
        return E_NOINTERFACE;
    };
    STDMETHODIMP_(ULONG) AddRef()
    {
        printf("CComObject::AddRef, ref=%d\n", m_lRef + 1);
        return _ThreadModel::Increment(m_lRef);
    };
    STDMETHODIMP_(ULONG) Release()
    {
        long Value = _ThreadModel::Decrement(m_lRef);
        printf("CComObject::Release, ref=%d\n", m_lRef);
        if(!m_lRef) delete this;
        return Value;
    };

    static HRESULT WINAPI Create(CComObject<ComClass>** pp);
};

template <class ComClass>
HRESULT WINAPI CComObject<ComClass>::Create(CComObject<ComClass>** pp) throw()
{
	if (pp == NULL)
        return E_POINTER;
	*pp = NULL;

	HRESULT hRes = E_OUTOFMEMORY;
	CComObject<ComClass>* p = _Creator::CreateObject();
	if (p != NULL)
	{
	    *pp = p;
        hRes = S_OK;
	}
	return hRes;
}

template <class ComClass>
class CClassFactory : public IClassFactory
{
private:
    ULONG m_lRef;

public:
    CClassFactory() : m_lRef(0) { };
    virtual ~CClassFactory() {};

    STDMETHOD(QueryInterface)(REFIID riid, LPVOID *ppv)
    {
        printf("CClassFactory::QueryInterface\n");
        *ppv = NULL;
        if (riid != __uuidof(IUnknown) &&
            riid != __uuidof(IClassFactory))
        {
            return E_NOINTERFACE;
        }
        *ppv = this;
        AddRef();
        return S_OK;
    }

    STDMETHODIMP_(ULONG) AddRef()
    {
        printf("CClassFactory::AddRef, ref=%d\n", m_lRef + 1);
        return (++m_lRef);
    }

    STDMETHODIMP_(ULONG) Release()
    {
        long lValue = --m_lRef;
        printf("CClassFactory::Release, ref=%d\n", m_lRef);
        if(!m_lRef) delete this;
        return lValue;
    }

    STDMETHODIMP CreateInstance(LPUNKNOWN pUnkOuter, REFIID riid, LPVOID *ppvObj)
    {
        *ppvObj = NULL;
        if (pUnkOuter)
        {
            return CLASS_E_NOAGGREGATION;
        }
        CComObject<ComClass>* pObj = NULL;
        HRESULT hr = CComObject<ComClass>::Create(&pObj);
        if (hr != S_OK)
        {
            return hr;
        }

        hr = pObj->QueryInterface(riid, ppvObj);
        if(hr != S_OK)
        {
            delete pObj;
        }

        return hr;
    }

    STDMETHODIMP LockServer(BOOL fLock)
    {
        if (fLock)
            _ComModule.Lock();
        else
            _ComModule.Unlock();
        return S_OK;
    }
};

struct CoClassEntry
{
    const CLSID* pCLSID;
    LPVOID pcf;
};

template < class MyDllModule >
class CComDllModule
{
private:
    HANDLE m_hModule;
    int m_iLockCount;
    TCHAR m_tszLibId[ 64 ];
    TCHAR m_tszClassId[ 64 ];

public:
    CComDllModule(LPCTSTR tszLibId, LPCTSTR tszClassId) : m_hModule(NULL), m_iLockCount(0)
    {
        StringCchCopy(m_tszLibId, sizeof(m_tszLibId)/sizeof(TCHAR), tszLibId);
        StringCchCopy(m_tszClassId, sizeof(m_tszLibId)/sizeof(TCHAR), tszClassId);
    }

    inline HMODULE GetModuleHandle() { return (HMODULE) m_hModule; }

    const static CoClassEntry* WINAPI _GetEntries();

    BOOL DllMain(HANDLE hModule, DWORD dwReason, LPVOID lpReserved)
    {
        switch (dwReason)
	    {
		    case DLL_PROCESS_ATTACH: 
			    m_hModule = hModule;
			    break;
		    case DLL_THREAD_ATTACH:
		    case DLL_THREAD_DETACH:
		    case DLL_PROCESS_DETACH:
			    break;
        }
        return TRUE;
    }

    HRESULT DllGetClassObject(REFCLSID rclsid, REFIID riid, LPVOID *ppvOut)
    {
        *ppvOut = NULL;
        const CoClassEntry* pEntries = MyDllModule::_GetEntries();
        while (pEntries->pCLSID)
        {
            if (IsEqualCLSID(rclsid, *(pEntries->pCLSID)))
            {
                IClassFactory* pcf = static_cast<IClassFactory*>(pEntries->pcf);
                return pcf->QueryInterface(riid, ppvOut);
            }
            ++pEntries;
        }
        return CLASS_E_CLASSNOTAVAILABLE;
    }
    
    HRESULT DllCanUnloadNow(void)
    {
        return m_iLockCount == 0 ? S_OK : S_FALSE;
    }

    HRESULT DllRegisterServer(void)
    {
    	TCHAR tszPath [ MAX_PATH ];
	    GetModuleFileName(_ComModule.GetModuleHandle(), tszPath, MAX_PATH);
	    return CDllRegistrar().RegisterObject(CLSID_ONFavAddin, m_tszLibId, m_tszClassId, tszPath) ? S_OK : S_FALSE;
    }

    HRESULT DllUnregisterServer(void)
    {
	    return CDllRegistrar().UnRegisterObject(CLSID_ONFavAddin, m_tszLibId, m_tszClassId) ? S_OK : S_FALSE;
    }

    void Lock()
    {
        m_iLockCount++;
    }

    void Unlock()
    {
        if (m_iLockCount) m_iLockCount--;
    }
};

}
}

#endif
