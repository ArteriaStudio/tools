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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			var pApp = App.Current as AutoCA.App;
			if (pApp.m_pPrepareFlags.bExistDbParams == false)
			{
				//　設定情報（接続情報入力画面）に遷移
				this.ContentFrame.Navigate(typeof(SettingsPage));
			}
			else if (pApp.m_pPrepareFlags.bExistIdentity == false)
			{
				//　設定情報（認証局主体者情報入力画面）に遷移
				this.ContentFrame.Navigate(typeof(SettingsPage));
			}
			else
			{
				//　既定の初期画面に遷移
				this.ContentFrame.Navigate(typeof(DefaultPage));
			}
			this.MessageFrame.Navigate(typeof(MessagesPage));
		}

		private void DumpCertificate(string pFilepath)
		{
			X509Certificate pCert = new X509Certificate(pFilepath);
			string resultsTrue = pCert.ToString(true);
			Console.WriteLine(resultsTrue);
			string resultsFalse = pCert.ToString(false);
			Console.WriteLine(resultsFalse);
		}

		private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var clickedItem = args.SelectedItem;
			var clickedItemContainer = args.SelectedItemContainer;

			var pItem = clickedItem as NavigationViewItem;
			Debug.WriteLine("Tag=" + pItem.Tag);

			switch (pItem.Tag.ToString()) {
			case "ListupCertificate":
				ContentFrame.Navigate(typeof(DefaultPage));
				break;
			case "CreateCertificate":
				ContentFrame.Navigate(typeof(IssuePage));
				break;
			case "Request":
				ContentFrame.Navigate(typeof(RequestPage));
				break;
			case "Signing":
				ContentFrame.Navigate(typeof(SigningPage));
				break;
			case "Settings":
				ContentFrame.Navigate(typeof(SettingsPage));
				break;
			case "Author":
				ContentFrame.Navigate(typeof(VersionPage));
				break;
			}
		}

		public void AddMessage(Message pMessage)
		{
			var pMessagesPage = MessageFrame.Content as MessagesPage;
			pMessagesPage.AddMessage(pMessage);
		}
	}
}
