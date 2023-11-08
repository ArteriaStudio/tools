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
//　リスナ型のデザインパタンにすると複雑化するので合理的でない。
	public interface ContentDialogEventListener
	{
		abstract void OnAccept(object pContext);
	}

	//　対話処理の多様性を削減してロジックの合理的様態を育てる。
	public abstract class NormalizeDialog
	{
		public async void DisplayDialog(object pContext, XamlRoot pXamlRoot, string pTitle, string pContent, ContentDialogEventListener pListener)
		{
			ContentDialog pContentDialog = new ContentDialog
			{
				Title = pTitle,
				Content = pContent,
				CloseButtonText = "いいえ",
				PrimaryButtonText = "はい",
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

			//　DBコネクションにおいてトランザクジョンを開始
			// ※ このトランザクションクラスは、リスナパタンでかなり使いやすい設計。
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//　選択された証明書データを獲得
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

				//　有効期限を延長した証明書を発行する。
				pAuthority.Update(pSQLContext, pCertificate);

				//　証明書を失効する。
				pAuthority.Revoke(pSQLContext, pCertificate);

				//　メモリ上のデータを更新する。
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
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);

			//　ダウンロードフォルダにファイルを出力する。
			var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
			var pExportFilepath = pCertificate.Export(pExportFolder);

			pWindow.AddMessage(new Message(AppFacility.Complete, "証明書をファイルに出力しました。", pCertificate.CommonName, pExportFilepath));
		}

		//　選択した証明書をファイルに出力する。
		private void ExportButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//　選択項目がなければ処理なし
				return;
			}

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			DisplayExportDialog(pCommonName, pSerialNumber);

			return;
		}

		//　
		private async void DisplayUpdateDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pDialog = new ContentDialog
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
				var eResult = await pDialog.ShowAsync();
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
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//　DBコネクションにおいてトランザクジョンを開始
			// ※ このトランザクションクラスは、リスナパタンでかなり使いやすい設計。
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//　選択された証明書データを獲得
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);
				if (pCertificate.Revoked == true)
				{
					//　失効した証明書を選択した。
					throw (new AppException(AppError.InvalidCertificate, AppFacility.Error, AppFlow.CreateCertificateForUpdate, pCertificate.SerialNumber));
				}

				//　証明書を失効する。
				pAuthority.Revoke(pSQLContext, pCertificate);

				//　有効期限を延長した証明書を発行する。
				pAuthority.Update(pSQLContext, pCertificate);

				//　メモリ上のデータを更新する。
				foreach (var pCert in m_pCertificates)
				{
					if (pCert.SerialNumber == pSerialNumber)
					{
						pCert.Revoked = true;
						break;
					}
				}
				pTransaction.Commit();
				pWindow.AddMessage(new Message(AppFacility.Complete, "証明書の有効期限を延長しました。", pCertificate.CommonName));
			}
			catch (AppException pException)
			{
				pTransaction.Rollback();
				pWindow.AddMessage(new Message(pException.m_eFacility, pException.GetText(), pException.GetParameter()));
			}

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

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			//　ダイアログを開始
			DisplayUpdateDialog(pCommonName, pSerialNumber);

			return;
		}

		private async void DisplayRevokeDialog(string pCommonName, string pSerialNumber)
		{
			ContentDialog pDialog = new ContentDialog
			{
				Title = "証明書を失効",
				Content = "証明書（" + pCommonName + "：" + pSerialNumber + "）を失効させます。\nよろしいですか？",
				CloseButtonText = "いいえ",
				PrimaryButtonText = "はい",
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

		//　
		private void OnChoiceRevokeDialog(ContentDialogResult eResult, string pSerialNumber)
		{
			if (eResult != ContentDialogResult.Primary)
			{
				return;
			}

			//　
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//　DBコネクションにおいてトランザクジョンを開始
			// ※ このトランザクションクラスは、リスナパタンでかなり使いやすい設計。
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//　選択された証明書データを獲得する。
				var pCertificate = pAuthority.Fetch(pSQLContext, pSerialNumber);
				if (pCertificate.Revoked == true)
				{
					//　失効した証明書を選択した。
					throw (new AppException(AppError.InvalidCertificate, AppFacility.Error, AppFlow.Revoke, pCertificate.SerialNumber));
				}

				//　証明書を失効する。
				pAuthority.Revoke(pSQLContext, pCertificate);

				//　メモリ上のデータを更新する。
				foreach (var pCert in m_pCertificates)
				{
					if (pCert.SerialNumber == pSerialNumber)
					{
						pCert.Revoked = true;
						break;
					}
				}
				pTransaction.Commit();
				pWindow.AddMessage(new Message(AppFacility.Complete, "証明書を失効しました。", pCertificate.CommonName));
			}
			catch (Exception)
			{
				pTransaction.Rollback();
			}
			
			return;
		}


		//　指定された証明書を失効させる。
		private void RevokeButton_Click(object sender, RoutedEventArgs e)
		{
			if (CertsList.SelectedIndex == -1)
			{
				//　選択項目がなければ処理なし
				return;
			}

			var pCommonName   = m_pCertificates[CertsList.SelectedIndex].CommonName;
			var pSerialNumber = m_pCertificates[CertsList.SelectedIndex].SerialNumber;

			//　ダイアログを開始
			DisplayRevokeDialog(pCommonName, pSerialNumber);

			return;
		}

		//　CRL を最新に更新
		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			//　
			var pApp = App.Current as AutoCA.App;
			var pWindow = pApp.m_pWindow as MainWindow;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;

			//　DBコネクションにおいてトランザクジョンを開始
			// ※ このトランザクションクラスは、リスナパタンでかなり使いやすい設計。
			var pTransaction = pSQLContext.BeginTransaction();
			try
			{
				//　失効リストを生成する。
				var iCrlDays = 128;
				var pBytes = pAuthority.GenerateCRL(pSQLContext, iCrlDays);

				//　ダウンロードフォルダにファイルを出力する。
				var pExportFolder = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Downloads";
				var pExportFilepath = pAuthority.ExportCRL(pExportFolder, pBytes);

				pTransaction.Commit();

				var pText = "次のパスにファイルを出力しました。";
				var pMessage = new Message(AppFacility.Info, pText, pExportFilepath);
				pWindow.AddMessage(pMessage);
			}
			catch (Exception)
			{
				pTransaction.Rollback();
			}

			return;
		}

		//　証明書一覧を再表示
		private void ReloadButton_Click(object sender, RoutedEventArgs e)
		{
			var pApp = App.Current as AutoCA.App;
			var pSQLContext = pApp.GetSQLContext();
			var pAuthority = Authority.Instance;
			var pNewCertificates = pAuthority.Listup(pSQLContext);

			//　カーソルとキャッシュの差分を探索して、差分となった要素を更新
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
