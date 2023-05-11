#include	"HttpClient.h"
//　HTTPクライアント（WinHTTPを利用する実装）


static const wchar_t *	USER_AGENT=L"WinHTTP Variant/1.0";

CUxHttpClient::CUxHttpClient()
{
	m_hConnect = nullptr;
}

CUxHttpClient::~CUxHttpClient()
{
	m_hConnect = nullptr;
}

void
CUxHttpClient::SetHandle(HINTERNET hConnect)
{
	m_hConnect = hConnect;

	return;
}

bool
CUxHttpClient::Get(LPCWSTR pURI)
{

	return(true);
}

bool
CUxHttpClient::Post(LPCWSTR pURI)
{

	return(true);
}

bool
CUxHttpClient::Put(LPCWSTR pURI)
{

	return(true);
}

bool
CUxHttpClient::Delete(LPCWSTR pURI)
{

	return(true);
}








CUxHttpFactory::CUxHttpFactory()
{
	m_hSession = nullptr;
}

CUxHttpFactory::~CUxHttpFactory()
{
	m_hSession = nullptr;
}

CUxHttpFactory *
CUxHttpFactory::GetInstance(void)
{
static CUxHttpFactory	pInstance;

	return(&pInstance);
}

//　セッションを作成
bool
CUxHttpFactory::Initialize()
{
	m_hSession = ::WinHttpOpen( L"WinHTTP Example/1.0", WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY, WINHTTP_NO_PROXY_NAME, WINHTTP_NO_PROXY_BYPASS, 0);
	if (m_hSession == nullptr) {
		return(false);
	}

	return(true);
}

//　セッションを削除
void
CUxHttpFactory::Finalize()
{
	if (m_hSession) {
		::WinHttpCloseHandle(m_hSession);
		m_hSession = nullptr;
	}

	return;
}

//　HTTPクライアント
bool
CUxHttpFactory::CreateClient(LPCWSTR pHost, CUxHttpClient & pClient)
{
	HINTERNET	hConnect = nullptr;

	hConnect = ::WinHttpConnect(m_hSession, pHost, INTERNET_DEFAULT_HTTPS_PORT, 0);
	if (hConnect == nullptr) {
		return(false);
	}
	pClient.SetHandle(hConnect);

	return(true);
}
