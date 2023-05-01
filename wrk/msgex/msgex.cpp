// msgex.cpp : アプリケーションのエントリ ポイントを定義します。
//

#include	"pch.h"
#include	"msgex.h"
#include	"FrameWnd.h"

HINSTANCE	hInst;							// 現在のインターフェイス
WCHAR		szTitle[MAX_LOADSTRING];		// タイトル バーのテキスト
WCHAR		szWindowClass[MAX_LOADSTRING];	// メイン ウィンドウ クラス名
CFrameWnd	pFrameWnd;
WCHAR		pText[1024] = {};

int APIENTRY
wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	//　起動引数検査
	int			nA = 0;
	LPWSTR *	pA = ::CommandLineToArgvW(lpCmdLine, &nA);
	if (nA < 2) {
		return(EXIT_FAILURE);
	} else {
		//　ファイルから表示文字列を入力
		LPWSTR	pFilepath = pA[1];
		if (ReadFromFile(pFilepath, _countof(pText), pText) == false) {
			pText[0] = L'\0';
		}
		pFrameWnd.SetMessageText(pA[0], pText);
	}

	//　グローバル文字列を初期化する
	LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadStringW(hInstance, IDC_MSGEX, szWindowClass, MAX_LOADSTRING);
	pFrameWnd.Register(hInstance);

	//　アプリケーション初期化の実行
	if (!InitInstance(hInstance, nCmdShow))
	{
		return FALSE;
	}

	HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_MSGEX));

	MSG msg;

	// メイン メッセージ ループ:
	while (GetMessage(&msg, nullptr, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return((int)msg.wParam);
}

bool
ReadFromFile(LPWSTR pFilepath, size_t nText, WCHAR * pText)
{
	FILE *	pFile = nullptr;
	if (_wfopen_s(&pFile, pFilepath, L"rt,ccs=UTF-8")) {
		return(false);
	}
	if (pFile == nullptr) {
		return(false);
	}
	memset(pText, 0, nText * sizeof(WCHAR));
	for (size_t iText = 0; iText < (nText - 1); iText ++) {
		if (feof(pFile)) {
			break;
		}
		WCHAR	wChar = fgetwc(pFile);
		pText[iText] = wChar;
	}
	fclose(pFile);

	return(true);
}