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

			this.m_pNavigationView = this.Navigation;

			this.FragmentsFrame.Navigate(typeof(ListupPage));
//			this.FragmentsFrame.Navigate(typeof(StartPage));
//			this.FragmentsFrame.Navigate(typeof(SettingsPage));

//			this.SizeChanged += MainWindow_SizeChanged;
		}

		private void MainWindow_SizeChanged(object sender, WindowSizeChangedEventArgs args)
		{
			System.Diagnostics.Debug.WriteLine("this.Bounds.Width=" + this.Bounds.Width);
			System.Diagnostics.Debug.WriteLine("this.Bounds.Height=" + this.Bounds.Height);
		}

		private void myButton_Click(object sender, RoutedEventArgs e)
		{
			this.FragmentsFrame.Navigate(typeof(StatusPage));
			/*
			var pFrame = new Frame();
//			pFrame.Navigate(typeof(StatusPage));
			pFrame.Navigate(typeof(StatusPage), null, new DrillInNavigationTransitionInfo());
			*/
		}

		public NavigationView	m_pNavigationView = null;
	}
}
