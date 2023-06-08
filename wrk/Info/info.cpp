#define _WIN32_DCOM
#include	<iostream>
#include	<atlbase.h>
#include	<comdef.h>
#include	<Wbemidl.h>
#include	"UxWMI.h"



#pragma comment(lib, "wbemuuid.lib")
using namespace std;

void	DumpValue(IWbemClassObject* pObject, const wchar_t* pItemName);

int main(int argc, char** argv)
{
	HRESULT hres;

	// Initialize COM.
	hres = CoInitializeEx(0, COINIT_MULTITHREADED);
	if (FAILED(hres))
	{
		cout << "Failed to initialize COM library. "
			<< "Error code = 0x"
			<< hex << hres << endl;
		return 1;              // Program has failed.
	}

	// Initialize 
	hres = CoInitializeSecurity(
		NULL,
		-1,      // COM negotiates service                  
		NULL,    // Authentication services
		NULL,    // Reserved
		RPC_C_AUTHN_LEVEL_DEFAULT,    // authentication
		RPC_C_IMP_LEVEL_IMPERSONATE,  // Impersonation
		NULL,             // Authentication info 
		EOAC_NONE,        // Additional capabilities
		NULL              // Reserved
	);


	if (FAILED(hres))
	{
		cout << "Failed to initialize security. "
			<< "Error code = 0x"
			<< hex << hres << endl;
		CoUninitialize();
		return 1;          // Program has failed.
	}

	// Obtain the initial locator to Windows Management
	// on a particular host computer.
	IWbemLocator* pLoc = 0;

	hres = CoCreateInstance(
		CLSID_WbemLocator,
		0,
		CLSCTX_INPROC_SERVER,
		IID_IWbemLocator, (LPVOID*)&pLoc);

	if (FAILED(hres))
	{
		cout << "Failed to create IWbemLocator object. "
			<< "Error code = 0x"
			<< hex << hres << endl;
		CoUninitialize();
		return 1;       // Program has failed.
	}

	IWbemServices* pSvc = 0;

	// Connect to the root\cimv2 namespace with the
	// current user and obtain pointer pSvc
	// to make IWbemServices calls.

	hres = pLoc->ConnectServer(

		_bstr_t(L"ROOT\\CIMV2"), // WMI namespace
		NULL,                    // User name
		NULL,                    // User password
		0,                       // Locale
		NULL,                    // Security flags                 
		0,                       // Authority       
		0,                       // Context object
		&pSvc                    // IWbemServices proxy
	);

	if (FAILED(hres))
	{
		cout << "Could not connect. Error code = 0x"
			<< hex << hres << endl;
		pLoc->Release();
		CoUninitialize();
		return 1;                // Program has failed.
	}

	cout << "Connected to ROOT\\CIMV2 WMI namespace" << endl;

	// Set the IWbemServices proxy so that impersonation
	// of the user (client) occurs.
	hres = CoSetProxyBlanket(
		pSvc,                         // the proxy to set
		RPC_C_AUTHN_WINNT,            // authentication service
		RPC_C_AUTHZ_NONE,             // authorization service
		NULL,                         // Server principal name
		RPC_C_AUTHN_LEVEL_CALL,       // authentication level
		RPC_C_IMP_LEVEL_IMPERSONATE,  // impersonation level
		NULL,                         // client identity 
		EOAC_NONE                     // proxy capabilities     
	);

	if (FAILED(hres))
	{
		cout << "Could not set proxy blanket. Error code = 0x" << hex << hres << endl;
		pSvc->Release();
		pLoc->Release();
		CoUninitialize();
		return 1;               // Program has failed.
	}


	// Use the IWbemServices pointer to make requests of WMI. 
	// Make requests here:

//    bstr_t("SELECT * FROM Win32_Process"),


	// For example, query for all the running processes
	IEnumWbemClassObject* pEnumerator = NULL;
	hres = pSvc->ExecQuery(
		bstr_t("WQL"),
		bstr_t("SELECT * FROM Win32_BIOS"),
		WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
		NULL,
		&pEnumerator);

	if (FAILED(hres))
	{
		cout << "Query for processes failed. "
			<< "Error code = 0x"
			<< hex << hres << endl;
		pSvc->Release();
		pLoc->Release();
		CoUninitialize();
		return 1;               // Program has failed.
	}
	else
	{
		IWbemClassObject* pclsObj;
		ULONG uReturn = 0;

		while (pEnumerator)
		{
			hres = pEnumerator->Next(WBEM_INFINITE, 1, &pclsObj, &uReturn);

			if (0 == uReturn)
			{
				break;
			}

			DumpValue(pclsObj, L"SerialNumber");
			DumpValue(pclsObj, L"Manufacturer");
			DumpValue(pclsObj, L"Name");
			//DumpValue(pclsObj, L"BuildNumber");
			DumpValue(pclsObj, L"Caption");
			//DumpValue(pclsObj, L"CodeSet");
			DumpValue(pclsObj, L"CurrentLanguage");
			DumpValue(pclsObj, L"Description");
			//DumpValue(pclsObj, L"LanguageEdition");
			//DumpValue(pclsObj, L"OtherTargetOS");
			DumpValue(pclsObj, L"SMBIOSBIOSVersion");
			DumpValue(pclsObj, L"SoftwareElementID");
			DumpValue(pclsObj, L"Status");
			DumpValue(pclsObj, L"Version");

			pclsObj->Release();
			pclsObj = NULL;
		}

	}

	// For example, query for all the running processes
	pEnumerator->Release();
	pEnumerator = NULL;
	hres = pSvc->ExecQuery(
		bstr_t("WQL"),
		bstr_t("SELECT * FROM Win32_ComputerSystem"),
		WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY,
		NULL,
		&pEnumerator);

	if (FAILED(hres))
	{
		cout << "Query for processes failed. " << "Error code = 0x" << hex << hres << endl;
		pSvc->Release();
		pLoc->Release();
		CoUninitialize();
		return 1;               // Program has failed.
	}
	else
	{
		IWbemClassObject* pclsObj;
		ULONG uReturn = 0;

		while (pEnumerator)
		{
			hres = pEnumerator->Next(WBEM_INFINITE, 1, &pclsObj, &uReturn);

			if (0 == uReturn)
			{
				break;
			}

			DumpValue(pclsObj, L"Domain");
			DumpValue(pclsObj, L"Manufacturer");
			DumpValue(pclsObj, L"Model");
			DumpValue(pclsObj, L"Name");

			pclsObj->Release();
			pclsObj = NULL;
		}

	}

	CUxLocator	pLocator;

	pLocator.Create();

	CUxEnumerator	pUxEnumerator;
	if (pLocator.Query(L"SELECT * FROM Win32_ComputerSystem2", pUxEnumerator) == true) {
		CUxClassObject	pObject;
		while (pUxEnumerator.Next(pObject) == true) {
			pObject.Dump(L"Model");
			pObject.Dump(L"Domain");
			pObject.Dump(L"Manufacturer");
			pObject.Dump(L"Model");
			pObject.Dump(L"Name");
			pObject.Relase();
		}
		pUxEnumerator.Relase();
	}

	pLocator.Delete();


	// Cleanup
	// ========

	pSvc->Release();
	pLoc->Release();
	pEnumerator->Release();

	CoUninitialize();

	return 0;   // Program successfully completed.
}

void
DumpValue(IWbemClassObject * pObject, const wchar_t * pItemName)
{
	VARIANT 	vtProp = {};

	// Get the value of the Name property
	HRESULT		hResult = pObject->Get(pItemName, 0, &vtProp, 0, 0);
	if (vtProp.pbstrVal != nullptr) {
		wcout << pItemName << " : " << vtProp.bstrVal << endl;
	}
	::VariantClear(&vtProp);

	return;
}