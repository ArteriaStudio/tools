using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ContentsPage : Page
	{
		public ContentsPage()
		{
			this.InitializeComponent();

			var pApp = Application.Current as App;

			//　初期表示
			this.TransitFrame(pApp.m_pProfile.CurrentPage);

			//　
			this.Navigation.SelectionChanged += Navigation_SelectionChanged;
		}

		//　ナヴィゲーション項目を選択
		private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var pSelectedItem = args.SelectedItem as NavigationViewItem;
			System.Diagnostics.Debug.WriteLine("Args.SelectedItem = " + pSelectedItem.Tag);
			
			var pSelectedItemName = pSelectedItem.Tag.ToString();
			System.Diagnostics.Debug.WriteLine("Args.pSelectedItemName = " + pSelectedItemName);

			//　選択されたフレームに差し替え
			this.TransitFrame(pSelectedItemName);

			var pApp = Application.Current as App;
			pApp.m_pProfile.CurrentPage = pSelectedItemName;
		}

		//　コンテンツページのキャプションを設定
		public void SetCaption(String pCaption)
		{
			var pLoader = new ResourceLoader();
			this.Caption.Text = pLoader.GetString(pCaption);

			return;
		}

		//　フレームを差し替え
		public void	TransitFrame(String  pTargetPage)
		{
			if (pTargetPage.Equals("DashBoardView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StartPage));
			}
			else if (pTargetPage.Equals("PersonsView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StatusPage));
			}
			else if (pTargetPage.Equals("MediaView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(MediaPage));
			}

			this.SetCaption(pTargetPage);
		}

	}
}
