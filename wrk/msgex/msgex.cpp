// msgex.cpp : アプリケーションのエントリ ポイントを定義します。
//

#include	"pch.h"
#include	"msgex.h"
#include	"FrameWnd.h"

HINSTANCE	hInst;							// 現在のインターフェイス
WCHAR		szTitle[MAX_LOADSTRING];		// タイトル バーのテキスト
WCHAR		szWindowClass[MAX_LOADSTRING];	// メイン ウィンドウ クラス名
CFrameWnd	pFrameWnd;

// このコード モジュールに含まれる関数の宣言を転送します:
//ATOM	RegisterWindowClass(HINSTANCE hInstance);

WCHAR	pText[1024] = {TEXT("\n最終帰宅事項まで３０分を切りました。\n\n仕事を整理して帰宅しましょう。\n\n\n管理職より")};

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
		pFrameWnd.SetMessageText(pA[0], pText);
	}

	//　グローバル文字列を初期化する
	LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadStringW(hInstance, IDC_MSGEX, szWindowClass, MAX_LOADSTRING);
	pFrameWnd.Register(hInstance);
//	pFrameWnd.SetMessageText(pA[0], pA[1]);

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
