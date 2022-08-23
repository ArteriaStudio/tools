using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			//　初期表示
			this.ApexFrame.Navigate(typeof(ApexPage));
			this.ContentsFrame.Navigate(typeof(ContentsPage));
			this.FooterFrame.Navigate(typeof(FooterPage));

			this.SizeChanged += MainWindow_SizeChanged;
		}

		private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("this.Bounds.Width=" + this.Bounds.Width);
			System.Diagnostics.Debug.WriteLine("this.Bounds.Height=" + this.Bounds.Height);

			/*
			if (this.Bounds.Width < 640) {
				this.Navigation.PaneDisplayMode = NavigationViewPaneDisplayMode.LeftCompact;
			} else {
				this.Navigation.PaneDisplayMode = NavigationViewPaneDisplayMode.Auto;
			}
			*/
		}
	}
}
