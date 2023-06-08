//　Windows Management Infrastructure Helper
#include	<atlbase.h>
#include	<comdef.h>
#include	<Wbemidl.h>
#include	"string"
#include	"format"
#include	"UxWMI.h"



#pragma comment(lib, "wbemuuid.lib")


CUxLocator::CUxLocator()
{
	m_pLocator = nullptr;
	m_pServices = nullptr;
}

CUxLocator::~CUxLocator()
{
	m_pServices = nullptr;
	m_pLocator = nullptr;
}

//　ロケータオブジェクトを生成
bool
CUxLocator::Create()
{
	// Obtain the initial locator to Windows Management on a particular host computer.
	HRESULT		hResult = ::CoCreateInstance(CLSID_WbemLocator, 0, CLSCTX_INPROC_SERVER, IID_IWbemLocator, (LPVOID*)&m_pLocator);
	if (FAILED(hResult)) {
		return(false);
	}

	//　
	hResult = m_pLocator->ConnectServer(_bstr_t(L"ROOT\\CIMV2"), nullptr, nullptr, 0, 0, 0, 0, &m_pServices);
	if (FAILED(hResult)) {
		return(false);
	}
	// Set the IWbemServices proxy so that impersonation of the user (client) occurs.
	hResult = ::CoSetProxyBlanket(m_pServices, RPC_C_AUTHN_WINNT, RPC_C_AUTHZ_NONE, nullptr, RPC_C_AUTHN_LEVEL_CALL, RPC_C_IMP_LEVEL_IMPERSONATE, nullptr, EOAC_NONE);
	if (FAILED(hResult)) {
		return(false);
	}

	return(true);
}

//　ロケータオブジェクトを削除
void
CUxLocator::Delete()
{
	m_pServices = nullptr;
	m_pLocator = nullptr;

	return;
}

//　クエリーを実行
bool
CUxLocator::Query(LPCWSTR pQuery, CUxEnumerator & pUxEnumerator)
{
	IEnumWbemClassObject *	pEnumator = nullptr;

	HRESULT 	hResult = m_pServices->ExecQuery(bstr_t("WQL"), bstr_t(pQuery), WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY, nullptr, &pEnumator);
	if (FAILED(hResult)) {
		return(false);
	}
	pUxEnumerator.SetHandle(pEnumator);

	return(true);
}

CUxEnumerator::CUxEnumerator()
{
	pEnumerator = nullptr;
}

CUxEnumerator::~CUxEnumerator()
{
	pEnumerator = nullptr;
}

//　
void
CUxEnumerator::SetHandle(IEnumWbemClassObject * pHandle)
{
	pEnumerator = pHandle;

	return;
}

//　
bool
CUxEnumerator::Next(CUxClassObject & pUxObject)
{
	IWbemClassObject *	pObject = nullptr;
	ULONG	uReturn = 0;

	HRESULT		hResult = pEnumerator->Next(WBEM_INFINITE, 1, &pObject, &uReturn);
	if (uReturn == 0) {
		return(false);
	}
	pUxObject.SetHandle(pObject);

	return(true);
}

//　
void
CUxEnumerator::Relase()
{
	pEnumerator = nullptr;

	return;
}




CUxClassObject::CUxClassObject()
{
	m_pObject = nullptr;
}

CUxClassObject::~CUxClassObject()
{
	m_pObject = nullptr;
}

//　
void
CUxClassObject::SetHandle(IWbemClassObject * pHandle)
{
	m_pObject = pHandle;

	return;
}

void
CUxClassObject::Dump(const wchar_t * pItemName)
{
	VARIANT 	vtProp = {};

	// Get the value of the Name property
	HRESULT		hResult = m_pObject->Get(pItemName, 0, &vtProp, 0, 0);
	if (vtProp.pbstrVal != nullptr) {
		std::wstring	pValue = vtProp.bstrVal;
//		std::wstring	pText = std::format(L"{s}", pValue);
		::OutputDebugStringW(pValue.c_str());
//		wcout << pItemName << " : " << vtProp.bstrVal << endl;
	}
	::VariantClear(&vtProp);

	return;
}

void
CUxClassObject::Relase()
{
	m_pObject = nullptr;
}
