using Arteria_s.Common.LigareBook;
using Arteria_s.DB.Base;
using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			this.InitializeComponent();

			//　Unicode 以外のコードページをサポートするエンコーディングプロバイダーのインスタンスを登録
			System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			//　前提条件検査
			PrepareEnvironments();

			//　フォーカス
			//this.FocusVisualKind = FocusVisualKind.Reveal;

			//　ユーザー別プロファイルを入力
			//　ウィンドウを作成する前にプロファイルを入力する契機はここになるが、
			//　アプリケーションのインスタンスが削除される契機はフレームワークが提供していない。
			m_pProfile = ProfileProvider.Load();

			//　データベースと接続
			try
			{
				m_pContext = new SQLContextEx(m_pProfile.DatabaseServer, m_pProfile.DatabaseName, m_pProfile.SchemaName, "", "");
			}
			catch (NpgsqlException e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message + "\n" + e.StackTrace);
				m_pContext = null;
			}
			catch (SocketException)
			{
				m_pContext = null;
			}
		}

		public Profile m_pProfile = null;
		public SQLContextEx m_pContext = null;

		/// <summary>
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			/*
			Frame rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null) {
				rootFrame = new Frame();
				rootFrame.NavigationFailed += RootFrame_NavigationFailed;

				/*
				ApplicationExecutionState	eState = args.PreviousExecutionState;
				if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//
				}
				*
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				rootFrame.Navigate(typeof(StartPage), args.Arguments);
			}

			Window.Current.Activate();
			*/

			m_pWindow = new MainWindow();
			m_pWindow.Activate();
		}

		protected void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new NotImplementedException();
		}

		public Window m_pWindow;

		public void TransitView(String pTargetView)
		{
			var pMainWindow = Window.Current as MainWindow;
			var pMainFrame = pMainWindow.Content;

			var pUIElement = pMainWindow.Content;
			//pUIElement;

			//var pItems = this.GetXmlnsDefinitions();

			//pMainWindow;


			/*
			if (pTargetView.Equals("DashBoard") == true)
			{

				this.FragmentsFrame.Navigate(typeof(StartPage));
			}
			else if (pTargetView.Equals("Persons") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StatusPage));
			}
			else if (pTargetView.Equals("Media") == true)
			{
				this.FragmentsFrame.Navigate(typeof(MediaPage));
			}
			*/
		}

		private void PrepareEnvironments()
		{
			var pEncordings = Encoding.GetEncodings();
			foreach (var pEncodeing in pEncordings)
			{
				System.Diagnostics.Trace.WriteLine("Encoding: Name=" + pEncodeing.Name + ", DisplayName=" + pEncodeing.DisplayName);
			}
		}

	}
}
