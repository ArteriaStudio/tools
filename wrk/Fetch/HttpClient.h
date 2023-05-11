#pragma 	once
//　HTTPクライアント（WinHTTPを利用する実装）
#include	"framework.h"
#include	"Fetch.h"

class	CUxHttpClient
{
protected:
	HINTERNET	m_hConnect;

public:
	 CUxHttpClient();
	~CUxHttpClient();

	bool	Get(LPCWSTR pURI);
	bool	Post(LPCWSTR pURI);
	bool	Put(LPCWSTR pURI);
	bool	Delete(LPCWSTR pURI);
	void	SetHandle(HINTERNET hConnect);
};

//　HTTPセッションファクトリ
class	CUxHttpFactory
{
private:
	 CUxHttpFactory();
	~CUxHttpFactory();

protected:
	HINTERNET	m_hSession;

public:
	CUxHttpFactory *	GetInstance(void);

	bool	Initialize();
	void	Finalize();

	bool	CreateClient(LPCWSTR pHost, CUxHttpClient & pClient);

};
