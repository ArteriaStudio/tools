//　Windows Management Infrastructure Helper
#pragma 	once


//　WMI クラスオブジェクト
class	CUxClassObject
{
protected:
	CComPtr<IWbemClassObject>	m_pObject;

public:
	 CUxClassObject();
	~CUxClassObject();

	void	SetHandle(IWbemClassObject * pHandle);
	void	Dump(const wchar_t * pItemName);
	void	Relase(void);
};

//　WMI 列挙オブジェクト
class	CUxEnumerator
{
protected:
	CComPtr<IEnumWbemClassObject>	pEnumerator;

public:
	 CUxEnumerator();
	~CUxEnumerator();

	void	SetHandle(IEnumWbemClassObject * pHandle);
	bool	Next(CUxClassObject & pObject);
	void	Relase(void);
};

//　WMI ロケータオブジェクト
class	CUxLocator
{
protected:
	CComPtr<IWbemLocator>	m_pLocator;
	CComPtr<IWbemServices>	m_pServices;

public:
	 CUxLocator();
	~CUxLocator();

	bool	Create();
	void	Delete();
	bool	Query(LPCWSTR pQuery, CUxEnumerator & pUxEnumerator);
};
