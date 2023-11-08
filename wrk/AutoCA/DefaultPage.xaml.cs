using Arteria_s.DB.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;
using Windows.UI;
using static AutoCA.AppException;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
#if (false)
//�@���X�i�^�̃f�U�C���p�^���ɂ���ƕ��G������̂ō����I�łȂ��B
	public interface ContentDialogEventListener
	{
		abstract void OnAccept(object pContext);
	}

	//�@�Θb�����̑��l�����팸���ă��W�b�N�̍����I�l�Ԃ���Ă�B
	public abstract class NormalizeDialog
	{
		public async void DisplayDialog(object pContext, XamlRoot pXamlRoot, string pTitle, string pContent, ContentDialogEventListener pListener)
		{
			ContentDialog pContentDialog = new ContentDialog
			{
				Title = pTitle,
				Content = pContent,
				CloseButtonText = "������",
				PrimaryButtonText = "�͂�",
				DefaultButton = ContentDialogButton.Primary,
				XamlRoot = pXamlRoot
			};

			try
			{
				var eResult = await pContentDialog.ShowAsync();
				if (eResult == ContentDialogResult.Primary)
				{
					pListener.OnAccept(pContext);
					return;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}

	public class UpdateDialog : NormalizeDialog, ContentDialogEventListener
	{
		public void OnAccept(object pContext)
		{
			var pSerialNumber = (string)pContext;
			//var m_pCertificates = ()pContext;

			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//�@DB�R�l�N�V�����ɂ����ăg�����U�N�W�������J�n
			// �� ���̃g�����U�N�V�����N���X�́A���X�i�p�^���ł��Ȃ�g���₷���݌v�B
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//�@�I�����ꂽ�ؖ����f�[�^���l��
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

				//�@�L�����������������ؖ����𔭍s����B
				pAuthority.Update(pSQLContext, pCertificate);

				//�@�ؖ�������������B
				pAuthority.Revoke(pSQLContext, pCertificate);

				//�@��������̃f�[�^���X�V����B
				foreach (var pCert in m_pCertificates)
				{
					if (pCert.SerialNumber == pSerialNumber)
					{
						pCert.Revoked = true;
						break;
					}
				}
				pTransaction.Commit();
			}
			catch (Exception)
			{
				pTransaction.Rollback();
			}
		}
	}

	public class RevokeDialog : NormalizeDialog, ContentDialogEventListener
	{
		public void OnAccept(object pContext)
		{
			;
		}
	}
#endif

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DefaultPage : Page
	{
		private ObservableCollection<Certificate> m_pCertificates;

		public DefaultPage()
		{
			this.InitializeComponent();

			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			m_pCertificates = pAuthority.Listup(pSQLContext);

			UpdateFormState();
		}

		private void UpdateFormState()
		{
			bool IsEnabled = true;
			if (CertsList.Items.Count <= 0)
			{
				IsEnabled = false;
			}
			else if (CertsList.SelectedIndex == -1)
			{
				IsEnabled = false;
			}
			RevokeButton.IsEnabled = IsEnabled;
			UpdateButton.IsEnabled = IsEnabled;
			ExportButton.IsEnabled = IsEnabled;
		}

		//�@
		private void CertsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateFormState();
		}

		//�@
		private async void DisplayExportDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pExportDialog = new ContentDialog
			{
				Title = "�ؖ������G�N�X�|�[�g",
				Content = "�ؖ����i" + pCommonName + "�F" + pSerialNumber + "�j���t�@�C���ɏo�͂��܂��B\n��낵���ł����H",
				CloseButtonText = "������",
				PrimaryButtonText = "�͂�",
				DefaultButton = ContentDialogButton.Primary,
				XamlRoot = this.Content.XamlRoot
			};

			try
			{
				var eResult = await pExportDialog.ShowAsync();
				this.OnChoiceExportDialog(eResult, pSerialNumber);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return;
		}

		//�@�ؖ������G�N�X�|�[�g���鏈���i���p�҂̓��ӂ𓾂��_�@�j
		private void OnChoiceExportDialog(ContentDialogResult eResult, string pSerialNumber)
		{
			if (eResult != ContentDialogResult.Primary)
			{
				return;
			}
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

			//�@�_�E�����[�h�t�H���_�Ƀt�@�C�����o�͂���B
			var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
			var pExportFilepath = pCertificate.Export(pExportFolder);

			pWindow.AddMessage(new Message(AppFacility.Complete, "�ؖ������t�@�C���ɏo�͂��܂����B", pCertificate.CommonName, pExportFilepath));
		}

		//�@�I�������ؖ������t�@�C���ɏo�͂���B
		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//�@�I�����ڂ��Ȃ���Ώ����Ȃ�
				return;
			}

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayExportDialog(pCommonName, pSerialNumber);

			return;
		}

		//�@
		private async void DisplayUpdateDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pDialog = new ContentDialog
			{
				Title = "�ؖ������X�V",
				Content = "�ؖ����i" + pCommonName + "�F" + pSerialNumber + "�j�̗L�����������������ؖ����𔭍s���܂��B\n��낵���ł����H",
				CloseButtonText = "������",
				PrimaryButtonText = "�͂�",
				DefaultButton = ContentDialogButton.Primary,
				XamlRoot = this.Content.XamlRoot
			};

			try
			{
				var eResult = await pDialog.ShowAsync();
				this.OnChoiceUpdateDialog(eResult, pSerialNumber);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return;
		}

		//�@�ؖ����̗L�����������������ؖ����𔭍s���鏈���i���p�҂̓��ӂ𓾂��_�@�j
		private void OnChoiceUpdateDialog(ContentDialogResult eResult, string pSerialNumber)
		{
			if (eResult != ContentDialogResult.Primary)
			{
				return;
			}

			//�@
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//�@DB�R�l�N�V�����ɂ����ăg�����U�N�W�������J�n
			// �� ���̃g�����U�N�V�����N���X�́A���X�i�p�^���ł��Ȃ�g���₷���݌v�B
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//�@�I�����ꂽ�ؖ����f�[�^���l��
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);
				if (pCertificate.Revoked == true)
				{
					//�@���������ؖ�����I�������B
					throw (new AppException(AppError.InvalidCertificate, AppFacility.Error, AppFlow.CreateCertificateForUpdate, pCertificate.SerialNumber));
				}

				//�@�ؖ�������������B
				pAuthority.Revoke(pSQLContext, pCertificate);

				//�@�L�����������������ؖ����𔭍s����B
				pAuthority.Update(pSQLContext, pCertificate);

				//�@��������̃f�[�^���X�V����B
				foreach (var pCert in m_pCertificates)
				{
					if (pCert.SerialNumber == pSerialNumber)
					{
						pCert.Revoked = true;
						break;
					}
				}
				pTransaction.Commit();
				pWindow.AddMessage(new Message(AppFacility.Complete, "�ؖ����̗L���������������܂����B", pCertificate.CommonName));
			}
			catch (AppException pException)
			{
				pTransaction.Rollback();
				pWindow.AddMessage(new Message(pException.m_eFacility, pException.GetText(), pException.GetParameter()));
			}

			return;
		}

		//�@�L���������X�V�����ؖ����𔭍s
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//�@�I�����ڂ��Ȃ���Ώ����Ȃ�
				return;
			}

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			//�@�_�C�A���O���J�n
			DisplayUpdateDialog(pCommonName, pSerialNumber);

			return;
		}

		private async void DisplayRevokeDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pDialog = new ContentDialog
			{
				Title = "�ؖ���������",
				Content = "�ؖ����i" + pCommonName + "�F" + pSerialNumber + "�j�����������܂��B\n��낵���ł����H",
				CloseButtonText = "������",
				PrimaryButtonText = "�͂�",
				DefaultButton = ContentDialogButton.Primary,
				XamlRoot = this.Content.XamlRoot
			};

			try
			{
				var eResult = await pDialog.ShowAsync();
				this.OnChoiceRevokeDialog(eResult, pSerialNumber);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return;
		}

		//�@
		private void OnChoiceRevokeDialog(ContentDialogResult eResult, string pSerialNumber)
		{
			if (eResult != ContentDialogResult.Primary)
			{
				return;
			}

			//�@
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//�@DB�R�l�N�V�����ɂ����ăg�����U�N�W�������J�n
			// �� ���̃g�����U�N�V�����N���X�́A���X�i�p�^���ł��Ȃ�g���₷���݌v�B
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//�@�I�����ꂽ�ؖ����f�[�^���l������B
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);
				if (pCertificate.Revoked == true)
				{
					//�@���������ؖ�����I�������B
					throw (new AppException(AppError.InvalidCertificate, AppFacility.Error, AppFlow.Revoke, pCertificate.SerialNumber));
				}

				//�@�ؖ�������������B
				pAuthority.Revoke(pSQLContext, pCertificate);

				//�@��������̃f�[�^���X�V����B
				foreach (var pCert in m_pCertificates)
				{
					if (pCert.SerialNumber == pSerialNumber)
					{
						pCert.Revoked = true;
						break;
					}
				}
				pTransaction.Commit();
				pWindow.AddMessage(new Message(AppFacility.Complete, "�ؖ������������܂����B", pCertificate.CommonName));
			}
			catch (Exception)
			{
				pTransaction.Rollback();
			}
			
			return;
		}


		//�@�w�肳�ꂽ�ؖ���������������B
		private void RevokeButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//�@�I�����ڂ��Ȃ���Ώ����Ȃ�
				return;
			}

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			//�@�_�C�A���O���J�n
			DisplayRevokeDialog(pCommonName, pSerialNumber);

			return;
		}

		//�@CRL ���ŐV�ɍX�V
		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			//�@
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//�@DB�R�l�N�V�����ɂ����ăg�����U�N�W�������J�n
			// �� ���̃g�����U�N�V�����N���X�́A���X�i�p�^���ł��Ȃ�g���₷���݌v�B
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//�@�������X�g�𐶐�����B
				var iCrlDays = 128;
				var pBytes = pAuthority.GenerateCRL(pSQLContext, iCrlDays);

				//�@�_�E�����[�h�t�H���_�Ƀt�@�C�����o�͂���B
				var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
				var pExportFilepath = pAuthority.ExportCRL(pExportFolder, pBytes);

				pTransaction.Commit();

				var pText = "���̃p�X�Ƀt�@�C�����o�͂��܂����B";
				var pMessage = new Message(AppFacility.Info, pText, pExportFilepath);
				pWindow.AddMessage(pMessage);
			}
			catch (Exception)
			{
				pTransaction.Rollback();
			}

			return;
		}

		//�@�ؖ����ꗗ���ĕ\��
		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pNewCertificates = pAuthority.Listup(pSQLContext);

			//�@�J�[�\���ƃL���b�V���̍�����T�����āA�����ƂȂ����v�f���X�V
			List<Certificate>	pDeleteList = new List<Certificate>();
			List<Certificate>	pAppendList = new List<Certificate>();

			foreach (var pNewCertificate in pNewCertificates)
			{
				if (m_pCertificates.Contains(pNewCertificate) == false)
				{
					pAppendList.Add(pNewCertificate);
				}
			}
			foreach (var pOldCertificate in m_pCertificates)
			{
				if (pNewCertificates.Contains(pOldCertificate) == false)
				{
					pDeleteList.Add(pOldCertificate);
				}
			}
			foreach (var pDeleteItem in  pDeleteList)
			{
				m_pCertificates.Remove(pDeleteItem);
			}
			foreach (var pAppendItem in pAppendList)
			{
				m_pCertificates.Add(pAppendItem);
			}
		}

		private void AppendButton_Click(object sender, RoutedEventArgs e)
		{
			var pItem = new Certificate();
			pItem.CommonName = "placeholder";
			m_pCertificates.Add(pItem);
		}

    }
}
