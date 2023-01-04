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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook.pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			InitializeComponent();

			var pApp = Application.Current as App;

			DatabaseServer.Text = pApp.m_pProfile.DatabaseServer;
			DatabaseName.Text = pApp.m_pProfile.DatabaseName;
			SchemaName.Text = pApp.m_pProfile.SchemaName;
		}

		//　コンテンツ幅の計算処理をフック
		protected override Size MeasureOverride(Size  size)
		{
			var pApp = Application.Current as App;
			System.Diagnostics.Debug.WriteLine("pApp.m_window.Content.ActualSize=" + pApp.m_pWindow.Content.ActualSize);
			System.Diagnostics.Debug.WriteLine("pApp.m_window.Content.DesiredSize=" + pApp.m_pWindow.Content.DesiredSize);
			System.Diagnostics.Debug.WriteLine("pApp.m_window.Bounds.Width=" + pApp.m_pWindow.Bounds.Width);
			System.Diagnostics.Debug.WriteLine("pApp.m_window.Bounds.Height=" + pApp.m_pWindow.Bounds.Height);

			System.Diagnostics.Debug.WriteLine("this.Parent.ToString()=" + this.Parent.ToString());
			
			var pFrame = this.Parent as Frame;
			System.Diagnostics.Debug.WriteLine("pFrame.ActualSize=" + pFrame.ActualSize);
			System.Diagnostics.Debug.WriteLine("pFrame.DesiredSize=" + pFrame.DesiredSize);
			//System.Diagnostics.Debug.WriteLine("pFrame.Width=" + pFrame.Width);

			System.Diagnostics.Debug.WriteLine("this.ActualSize=" + this.ActualSize);
			System.Diagnostics.Debug.WriteLine("this.DesiredSize=" + this.DesiredSize);

			var pWindow = pApp.m_pWindow as MainWindow;
			/*
//			var pNavigationView = pWindow.m_pNavigationView;
			System.Diagnostics.Debug.WriteLine("pNavigationView.ActualSize=" + pNavigationView.ActualSize);
			System.Diagnostics.Debug.WriteLine("pNavigationView.CompactModeThresholdWidth=" + pNavigationView.CompactModeThresholdWidth);
			*/

			

			//　パネルをスタックから相対に変更することで期待通りの挙動を示す（2022/08/22 Rink）
//			size.Width = pApp.m_window.Bounds.Width - pNavigationView.CompactModeThresholdWidth;

			return (base.MeasureOverride(size));
		}

		ObservableCollection<Person>	pItems = new ObservableCollection<Person>();

		private void Apply_Click(object sender, RoutedEventArgs e)
		{
			var pApp = Application.Current as App;
			pApp.m_pProfile.DatabaseServer = DatabaseServer.Text;
			pApp.m_pProfile.DatabaseName = DatabaseName.Text;
			pApp.m_pProfile.SchemaName = SchemaName.Text;
		}
	}
}
