#include	"pch.h"
#include	"Instance.h"

//
//   �֐�: InitInstance(HINSTANCE, int)
//
//   �ړI: �C���X�^���X �n���h����ۑ����āA���C�� �E�B���h�E���쐬���܂�
//
//   �R�����g:
//
//        ���̊֐��ŁA�O���[�o���ϐ��ŃC���X�^���X �n���h����ۑ����A
//        ���C�� �v���O���� �E�B���h�E���쐬����ѕ\�����܂��B
//
BOOL
InitInstance(HINSTANCE hInstance, int nCmdShow)
{
	hInst = hInstance; // �O���[�o���ϐ��ɃC���X�^���X �n���h�����i�[����

	//�@�V�X�e���p�����[�^�����
	//�@1�j��ʂ̐��@���l��
	int		iScreenW = ::GetSystemMetrics(SM_CXSCREEN);
	int		iScreenH = ::GetSystemMetrics(SM_CYSCREEN);
	//�@2�j�E�B���h�E���@���v�Z
	int		iW = iScreenW / 3 * 2;
	int		iH = iScreenH / 3 * 2;
	//�@3�j�E�B���h�E�z�u���W���v�Z
	int		iX = iW / 4;
	int		iY = iH / 4;

	if (pFrameWnd.Create(szWindowClass, szTitle, WS_OVERLAPPED, iX, iY, iW, iH, nullptr, nullptr, hInstance, nullptr) == false) {
		return(FALSE);
	}
	pFrameWnd.Show(nCmdShow);
	pFrameWnd.Update();

	return(TRUE);
}
