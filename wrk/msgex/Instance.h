#pragma		once
#include	"FrameWnd.h"

#define		MAX_LOADSTRING	(100)

// �O���[�o���ϐ�:
extern HINSTANCE	hInst;							// ���݂̃C���^�[�t�F�C�X
extern WCHAR		szTitle[MAX_LOADSTRING];		// �^�C�g�� �o�[�̃e�L�X�g
extern WCHAR		szWindowClass[MAX_LOADSTRING];	// ���C�� �E�B���h�E �N���X��
extern CFrameWnd	pFrameWnd;


BOOL	InitInstance(HINSTANCE, int);
