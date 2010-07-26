#pragma once

class CAddinEventHandler : public CThread
{
public:
    CAddinEventHandler(BSTR bstrParameter);
    ~CAddinEventHandler(void);

private:
    BSTR m_bstrParameter;

private:
    int Run();
};
