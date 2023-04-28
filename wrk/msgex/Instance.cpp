#include	"pch.h"
#include	"Instance.h"

//
//   関数: InitInstance(HINSTANCE, int)
//
//   目的: インスタンス ハンドルを保存して、メイン ウィンドウを作成します
//
//   コメント:
//
//        この関数で、グローバル変数でインスタンス ハンドルを保存し、
//        メイン プログラム ウィンドウを作成および表示します。
//
BOOL
InitInstance(HINSTANCE hInstance, int nCmdShow)
{
	hInst = hInstance; // グローバル変数にインスタンス ハンドルを格納する

	//　システムパラメータを入力
	//　1）画面の寸法を獲得
	int		iScreenW = ::GetSystemMetrics(SM_CXSCREEN);
	int		iScreenH = ::GetSystemMetrics(SM_CYSCREEN);
	//　2）ウィンドウ寸法を計算
	int		iW = iScreenW / 3 * 2;
	int		iH = iScreenH / 3 * 2;
	//　3）ウィンドウ配置座標を計算
	int		iX = iW / 4;
	int		iY = iH / 4;

	if (pFrameWnd.Create(szWindowClass, szTitle, WS_OVERLAPPED, iX, iY, iW, iH, nullptr, nullptr, hInstance, nullptr) == false) {
		return(FALSE);
	}
	pFrameWnd.Show(nCmdShow);
	pFrameWnd.Update();

	return(TRUE);
}
