#pragma		once

class CFrameWnd
{
private:
	LPWSTR	m_pCaptionText;
	LPWSTR	m_pMessageText;

protected:
	HWND		m_hWnd;

	HPEN		m_hPen;
	HBRUSH		m_hBrushCaption;
	HBRUSH		m_hBrushMessage;
	HFONT		m_hFont;

	COLORREF	m_clrCaptionText;
	COLORREF	m_clrCaptionBack;
	COLORREF	m_clrMessageText;
	COLORREF	m_clrMessageBack;

	void	WriteText(HDC hDC, RECT pClient, COLORREF clrText, COLORREF clrBack, HBRUSH hBackBrush, HFONT hTextFont, UINT uFormat, int nText, LPWSTR pText);

public:
			 CFrameWnd();
	virtual ~CFrameWnd();

	ATOM	Register(HINSTANCE  hInstance);
	void	SetMessageText(LPWSTR pCaptionText, LPWSTR pMessageText);
	bool	Create(LPCWSTR pClassName, LPCWSTR pWindowName, DWORD dwStyle, int X, int Y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lParam);
	bool	Show(int nShow);
	bool	Update(void);
	void	OnCreate(HWND hWnd);
	void	OnDestroy(void);
	void	OnDraw(HWND hWnd, HDC hDC);
};

