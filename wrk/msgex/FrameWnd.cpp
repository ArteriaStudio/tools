#include	"pch.h"
#include	"msgex.h"
#include	"FrameWnd.h"


LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);


CFrameWnd::CFrameWnd()
{
	m_hWnd = nullptr;
	m_hPen = m_hShadowPen = nullptr;
	m_hBrushCaption = m_hBrushMessage = nullptr;
	m_hFont = nullptr;
	m_pCaptionText = m_pMessageText = nullptr;
	m_clrCaptionText = m_clrCaptionBack = m_clrMessageText = m_clrMessageBack = 0;

}

CFrameWnd::~CFrameWnd()
{
	;
}

ATOM
CFrameWnd::Register(HINSTANCE  hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);
	wcex.style = CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = ::LoadIcon(hInstance, MAKEINTRESOURCE(IDI_MSGEX));
	wcex.hCursor = ::LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(COLOR_WINDOW + 1);
	wcex.lpszMenuName = nullptr;
	wcex.lpszClassName = szWindowClass;
	wcex.hIconSm = ::LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return(::RegisterClassExW(&wcex));
}

void
CFrameWnd::SetMessageText(LPWSTR pCaptionText, LPWSTR pMessageText)
{
	m_pCaptionText = pCaptionText;
	m_pMessageText = pMessageText;

	return;
}

bool
CFrameWnd::Create(LPCWSTR pClassName, LPCWSTR pWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lParam)
{
	m_hWnd = ::CreateWindowEx(0, pClassName, pWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, lParam);
	if (m_hWnd == nullptr) {
		return(false);
	}

	return(true);
}

bool
CFrameWnd::Show(int nShow)
{
	if (::ShowWindow(m_hWnd, nShow) == FALSE) {
		return(false);
	}

	return(true);
}

bool
CFrameWnd::Update(void)
{
	if (::UpdateWindow(m_hWnd) == FALSE) {
		return(false);
	}

	return(true);
}

void
CFrameWnd::OnCreate(HWND hWnd)
{
	m_clrCaptionText = RGB(0, 180, 0);
	m_clrCaptionBack = RGB(220, 220, 220);
	m_clrMessageText = RGB(0, 0, 0);
	m_clrMessageBack = RGB(230, 230, 230);

	m_hPen   = ::CreatePen(PS_SOLID, 1, RGB(200, 0, 0));
	m_hShadowPen = ::CreatePen(PS_SOLID, 2, RGB(210, 210, 210));
	m_hBrushCaption = ::CreateSolidBrush(m_clrCaptionBack);
	m_hBrushMessage = ::CreateSolidBrush(m_clrMessageBack);

	HDC		hDC = ::GetWindowDC(hWnd);


	LOGFONT 	pFont;

	pFont.lfHeight = MulDiv(36, ::GetDeviceCaps(hDC, LOGPIXELSY), 72);
	pFont.lfWidth = 0;
	pFont.lfEscapement = 0;
	pFont.lfOrientation = 0;
	pFont.lfWeight = 0;
	pFont.lfItalic = 0;
	pFont.lfUnderline = 0;
	pFont.lfStrikeOut = 0;
	pFont.lfCharSet = 0;
	pFont.lfOutPrecision = 0;
	pFont.lfClipPrecision = 0;
	pFont.lfQuality = 0;
	pFont.lfPitchAndFamily = 0;
	wcsncpy_s(pFont.lfFaceName, TEXT("HGｺﾞｼｯｸE"), LF_FACESIZE);

	m_hFont  = ::CreateFontIndirect(&pFont);

	::ReleaseDC(hWnd, hDC);

	return;
}

void
CFrameWnd::OnDestroy(void)
{
	::DeleteObject(m_hShadowPen);
	::DeleteObject(m_hFont);
	::DeleteObject(m_hBrushMessage);
	::DeleteObject(m_hBrushCaption);
	::DeleteObject(m_hPen);

	return;
}


//
//  関数: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  目的: メイン ウィンドウのメッセージを処理します。
//
//  WM_COMMAND  - アプリケーション メニューの処理
//  WM_PAINT    - メイン ウィンドウを描画する
//  WM_DESTROY  - 中止メッセージを表示して戻る
//
LRESULT CALLBACK
WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case	WM_COMMAND:
	{
		int wmId = LOWORD(wParam);
		// 選択されたメニューの解析:
		switch (wmId)
		{
		case	IDM_EXIT:
			::DestroyWindow(hWnd);
			break;
		default:
			return(::DefWindowProc(hWnd, message, wParam, lParam));
		}
	}
	break;
	case	WM_LBUTTONDOWN:
		::PostMessage(hWnd, WM_CLOSE, 0, 0);
		break;
	case	WM_PAINT:
	{
		PAINTSTRUCT ps;
		HDC hdc = ::BeginPaint(hWnd, &ps);
		pFrameWnd.OnDraw(hWnd, hdc);
		::EndPaint(hWnd, &ps);
	}
	break;
	case WM_DESTROY:
		pFrameWnd.OnDestroy();
		::PostQuitMessage(0);
		break;
	case	WM_CREATE:
		pFrameWnd.OnCreate(hWnd);
		break;
	default:
		return(::DefWindowProc(hWnd, message, wParam, lParam));
	}
	return 0;
}

void
CFrameWnd::WriteText(HDC hDC, RECT pClient, COLORREF clrText, COLORREF clrBack, HBRUSH hBackBrush, HFONT hTextFont, UINT uFormat, int nText, LPWSTR pText)
{
	::FillRect(hDC, &pClient, hBackBrush);

	HBRUSH	hBrush = (HBRUSH)::SelectObject(hDC, hBackBrush);
	HFONT	hFont = (HFONT)::SelectObject(hDC, hTextFont);
	::SetTextColor(hDC, clrText);
	::SetBkColor(hDC, clrBack);
	::DrawTextEx(hDC, pText, nText, &pClient, uFormat, nullptr);
	::SelectObject(hDC, hFont);
	::SelectObject(hDC, hBrush);

	return;
}

void
CFrameWnd::WriteLine(HDC hDC, RECT pClient, HPEN hLinePen)
{
	HPEN	hPen = (HPEN)::SelectObject(hDC, hLinePen);
	::MoveToEx(hDC, pClient.left, pClient.bottom - 1, nullptr);
	::LineTo(hDC, pClient.right, pClient.bottom - 1);
	::SelectObject(hDC, hPen);
}

void
CFrameWnd::OnDraw(HWND hWnd, HDC hDC)
{
	RECT	pClient = {};

	if (::GetClientRect(hWnd, &pClient) == FALSE) {
		return;
	}
	LONG	w = pClient.right;
	LONG	h = pClient.bottom;

	RECT	pCaption = {};//683
	RECT	pContent = {};

	LONG	lCaptionH = h / 4;
	LONG	lContentH = h - lCaptionH;
	LONG	lContentY = lCaptionH;

	pCaption.right  = pContent.right = pClient.right;
	pCaption.bottom = lCaptionH;
	pContent.top    = lCaptionH;
	pContent.bottom = lContentH + lCaptionH;

//	HPEN	hPen   = (HPEN)::SelectObject(hDC, m_hPen);

	int		nText = 0;
	WCHAR	pText[1024];
	UINT	uFormat = 0;

	uFormat = DT_CENTER | DT_VCENTER | DT_SINGLELINE;
	nText = wsprintf(pText, TEXT("%s"), m_pCaptionText);
	WriteText(hDC, pCaption, m_clrCaptionText, m_clrCaptionBack, m_hBrushCaption, m_hFont, uFormat, nText, pText);
	WriteLine(hDC, pCaption, m_hShadowPen);

	uFormat = DT_WORDBREAK | DT_CENTER;
	nText = wsprintf(pText, TEXT("%s"), m_pMessageText);
	WriteText(hDC, pContent, m_clrMessageText, m_clrMessageBack, m_hBrushMessage, m_hFont, uFormat, nText, pText);

//	::SelectObject(hDC, hPen);

	return;
}
