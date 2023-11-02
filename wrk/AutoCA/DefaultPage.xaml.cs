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

		//　
		private void CertsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateFormState();
		}

		//　
		private async void DisplayExportDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pExportDialog = new ContentDialog
			{
				Title = "証明書をエクスポート",
				Content = "証明書（" + pCommonName + "：" + pSerialNumber + "）をファイルに出力します。\nよろしいですか？",
				CloseButtonText = "いいえ",
				PrimaryButtonText = "はい",
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

		//　証明書をエクスポートする処理（利用者の同意を得た契機）
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

			//　ダウンロードフォルダにファイルを出力する。
			var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
			pCertificate.Export(pExportFolder);
		}

		//　選択した証明書をファイルに出力する。
		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//　選択項目がなければ処理なし
				return;
			}

			var pCommonName   = pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayExportDialog(pCommonName, pSerialNumber);

			return;
		}

		//　有効期限を更新した証明書を発行
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//　選択項目がなければ処理なし
				return;
			}

			var pCommonName   = pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayUpdateDialog(pCommonName, pSerialNumber);

			return;
		}

		//　
		private async void DisplayUpdateDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pExportDialog = new ContentDialog
			{
				Title = "証明書を更新",
				Content = "証明書（" + pCommonName + "：" + pSerialNumber + "）の有効期限を延長した証明書を発行します。\nよろしいですか？",
				CloseButtonText = "いいえ",
				PrimaryButtonText = "はい",
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

		//　証明書の有効期限を延長した証明書を発行する処理（利用者の同意を得た契機）
		private void OnChoiceUpdateDialog(ContentDialogResult eResult, string pSerialNumber)
		{
			if (eResult != ContentDialogResult.Primary)
			{
				return;
			}

			//　
			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

			//　更新した証明書を発行する。
			pAuthority.Update(pSQLContext, pCertificate);

			//　証明書を失効する。
			pAuthority.Revoke(pSQLContext, pCertificate);

			//　
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
