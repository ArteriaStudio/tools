#pragma		once
#include	"FrameWnd.h"

#define		MAX_LOADSTRING	(100)

// グローバル変数:
extern HINSTANCE	hInst;							// 現在のインターフェイス
extern WCHAR		szTitle[MAX_LOADSTRING];		// タイトル バーのテキスト
extern WCHAR		szWindowClass[MAX_LOADSTRING];	// メイン ウィンドウ クラス名
extern CFrameWnd	pFrameWnd;


BOOL	InitInstance(HINSTANCE, int);
