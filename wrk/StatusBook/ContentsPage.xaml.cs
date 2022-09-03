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

			//　初期表示
//			this.FragmentsFrame.Navigate(typeof(StartPage));
			this.FragmentsFrame.Navigate(typeof(StatusPage));
//			this.FragmentsFrame.Navigate(typeof(ListupPage));
//			this.FragmentsFrame.Navigate(typeof(SettingsPage));

			//　
			this.Navigation.SelectionChanged += Navigation_SelectionChanged;
		}

		private void Navigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var pSelectedItem = args.SelectedItem as NavigationViewItem;
			System.Diagnostics.Debug.WriteLine("Args.SelectedItem = " + pSelectedItem.Tag);
			
			var pSelectedItemName = pSelectedItem.Tag.ToString();
			System.Diagnostics.Debug.WriteLine("Args.pSelectedItemName = " + pSelectedItemName);


			this.TransitFrame(pSelectedItemName);


/*
			if (pSelectedItem.Tag.Equals("DashBoard") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StartPage));
			}
			else if (pSelectedItem.Tag.Equals("Persons") == true)
			{
				this.FragmentsFrame.Navigate(typeof(StatusPage));
			}
			else if (pSelectedItem.Tag.Equals("Media") == true)
			{
				this.FragmentsFrame.Navigate(typeof(MediaPage));
			}
*/
		}

		public void	TransitFrame(String  pTargetPage)
		{
			var pApp = Application.Current as App;
			var pMainWindow = pApp.m_window as MainWindow;


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
			pMainWindow.SetCaption(pTargetPage);


		}

	}
}
