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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DefaultPage : Page
	{
		private List<Certificate> pCertificates;

		public DefaultPage()
		{
			this.InitializeComponent();

			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			pCertificates = pAuthority.Listup(pSQLContext);
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
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

			//�@�_�E�����[�h�t�H���_�Ƀt�@�C�����o�͂���B
			var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
			pCertificate.Export(pExportFolder);
		}

		//�@�I�������ؖ������t�@�C���ɏo�͂���B
		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//�@�I�����ڂ��Ȃ���Ώ����Ȃ�
				return;
			}

			var pCommonName   = pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayExportDialog(pCommonName, pSerialNumber);

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

			var pCommonName   = pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayUpdateDialog(pCommonName, pSerialNumber);

			return;
		}

		//�@
		private async void DisplayUpdateDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pExportDialog = new ContentDialog
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
				var eResult = await pExportDialog.ShowAsync();
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
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

			//�@�X�V�����ؖ����𔭍s����B
			pAuthority.Update(pSQLContext, pCertificate);

			//�@�ؖ�������������B
			pAuthority.Revoke(pSQLContext, pCertificate);

			//�@
			foreach (var pCert in pCertificates)
			{
				if (pCert.SerialNumber == pSerialNumber)
				{
					pCert.Revoked = true;
					break;
				}
			}
		}

	}
}
