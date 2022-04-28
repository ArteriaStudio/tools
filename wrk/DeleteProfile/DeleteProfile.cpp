//　ユーザープロファイルを削除するコマンド
#include	<stdlib.h>
#include	<Windows.h>
#include	<winbase.h>
#include	<tchar.h>
#include	<userenv.h>
#include	<sddl.h>

#pragma comment (lib, "Userenv.lib")

//　メモリ解放用SIDクラス
class	CSID
{
protected:
	PSID *		m_pSID;

public:
	 CSID();
	~CSID();
};

CSID::CSID()
{
	m_pSID = nullptr;
}

CSID::~CSID()
{
	if (m_pSID) {
		::LocalFree(m_pSID);
		m_pSID = nullptr;
	}
}


//　アカウント名からSID に変換
//（引数）
//　pAccountName：（入力）アカウント名、ドメイン名やホスト名で修飾する必要なし
//　ppSID：（出力）SIDを格納したメモリブロックを指すポインタ
bool
LookupAccount(LPCTSTR  pAccountName, PSID *  ppSID)
{
	DWORD			nSID = 0;
	DWORD			nDomainName = 0;
	SID_NAME_USE	eUSE;

	if (::LookupAccountName(nullptr, pAccountName, nullptr, &nSID, nullptr, &nDomainName, &eUSE) == FALSE) {
		int		status = ::GetLastError();
		if (status != ERROR_INSUFFICIENT_BUFFER) {
			_ftprintf_s(stdout, L"LookupAccountName() is Failed. (%d) \n", ::GetLastError());
			return(false);
		}
	}

	PSID		pSID = new BYTE[nSID];
	TCHAR * 	pDomainName = new TCHAR[nDomainName + 1];

	if (::LookupAccountName(nullptr, pAccountName, pSID, &nSID, pDomainName, &nDomainName, &eUSE) == FALSE) {
		_ftprintf_s(stdout, L"LookupAccountName() is Failed. (%d) \n", ::GetLastError());
		return(false);
	}
	(*ppSID) = pSID;

	return(true);
}

//（引数）
//　pSID：（入力）削除対象とするユーザーのSIDを文字列表現にした文字列
bool
DeleteUserProfile(LPCTSTR  pSID)
{
	_ftprintf_s(stdout, L"Try DeleteProfile() Waiting for. SID:%s\n", pSID);
	if (::DeleteProfile(pSID, nullptr, nullptr) == FALSE) {
		_ftprintf_s(stdout, L"DeleteProfile() is Failed. SID:%s(%d)\n", pSID,::GetLastError());
		return(false);
	}

	return(true);
}

//　指定されたユーザーのユーザープロファイルを削除
//（引数）
//　pUser：（引数）起動引数で指定された削除するユーザープロファイルを指し示すユーザープリンシパル
bool
DeleteUserProfileEx(wchar_t *  pUser)
{
	PSID		pSID = nullptr;
	LPCTSTR 	pAccountName = pUser;
	LPTSTR		pSIDText = nullptr;

	//　ユーザー名に対応するSIDを検索
	if (LookupAccount(pAccountName, &pSID) == false) {
		_ftprintf_s(stdout, L"LookupAccount() is Failed. AccountName:%s\n", pAccountName);
		return(false);
	}
	//　SIDを文字列表現に変換
	if (::ConvertSidToStringSid(pSID, &pSIDText) == FALSE) {
		_ftprintf_s(stdout, L"ConvertSidToStringSid(). Failed. AccountName:%s (%d)\n", pAccountName, GetLastError());
		return(false);
	} else {
		//　ユーザープロファイルを削除
		if (DeleteUserProfile(pSIDText) == false) {
			_ftprintf_s(stdout, L"DeleteUserProfile(). Failed. AccountName:%s, SID:%s (%d)\n", pAccountName, pSIDText, GetLastError());
			return(false);
		}
	}
	_ftprintf_s(stdout, L"DeleteUserProfile(). Success. AccountName:%s\n", pAccountName);

	return(true);
}

//　
int
wmain(int  nC, wchar_t *  pV[])
{
	if (nC >= 2) {
		for (int  i = 1; i < nC; i ++) {
			DeleteUserProfileEx(pV[i]);
		}
	} else {
		_ftprintf_s(stdout, L"Usage: dp.exe $(AccountName) ... \n");
	}

	return(EXIT_SUCCESS);
}
