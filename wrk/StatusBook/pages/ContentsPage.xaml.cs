using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using StatusBook.pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Lights.Effects;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook.pages
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

			//　初期選択項目を指定
			var pMenuItems = this.Navigation.MenuItems;
			foreach (var pMenuItem in pMenuItems)
			{
				var iNavigationViewItem = pMenuItem as NavigationViewItem;
				if (pApp.m_pProfile.CurrentPage.Equals(iNavigationViewItem.Tag.ToString()) == true) {
					this.Navigation.SelectedItem = iNavigationViewItem;
					break;
				}
			}

			//　初期表示
			this.TransitFrame(pApp.m_pProfile.CurrentPage, null);

			//　イベントリスナを登録
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
			this.TransitFrame(pSelectedItemName, null);

			var pApp = Application.Current as App;
			pApp.m_pProfile.CurrentPage = pSelectedItemName;
		}

		//　コンテンツページのキャプションを設定
		public void SetCaption(string pCaption)
		{
			var pLoader = new ResourceLoader();
			this.Caption.Text = pLoader.GetString(pCaption);

			return;
		}

		//　フレームを差し替え
		public void	TransitFrame(string  pTargetPage, Object pParameters)
		{
			if (pTargetPage.Equals("DashBoardView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StartPage));
			}
			else if (pTargetPage.Equals("OrgUnitView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(OrgUnitsPage));
			}
			else if (pTargetPage.Equals("DevicesView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(DevicesPage));
			}
			else if (pTargetPage.Equals("StaffsView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StaffsPage));
			}
			else if (pTargetPage.Equals("StudentsView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StudentsPage));
			}
			else if (pTargetPage.Equals("LoaderView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(LoaderPage));
			}
			else if (pTargetPage.Equals("MediaView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(MediaPage));
			}
			else if (pTargetPage.Equals("HistoryView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(HistoryPage));
			}
			else if (pTargetPage.Equals("SettingsView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(SettingsPage));
			}
			else if (pTargetPage.Equals("StudentView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StudentPage), pParameters);
			}
			else if (pTargetPage.Equals("StaffView") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StaffPage), pParameters);
			}

			this.SetCaption(pTargetPage);
		}

	}
}
