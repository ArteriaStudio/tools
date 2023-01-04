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
	public sealed partial class StartPage : Page
	{
		public StartPage()
		{
			this.InitializeComponent();

			this.Loaded += StartPage_Loaded;
			
			for (int i = 0; i < 3; i ++)
			{
				Person	pPerson = new Person();
				pPerson.Name = "数字: " + i + "x";
				pItems.Add(pPerson);
			}
		}

		private void StartPage_Loaded(object sender, RoutedEventArgs e)
		{
			/*
			var pWindow = Window.Current;
			var pContent = pWindow.Content;
			//　この時点（Loaded）で参照するとWindow.Contentがnullptrとなっておりインスタンスが存在しない
			Console.Write("Window.Width = " + pContent.ActualSize); 
			*/

#if ENABLE_TRACE
			//　このパタンだと見かけ上のウィンドウサイズと異なる寸法が返却される。（メインウィンドウの上に仮想ウィンドウがおかれており、それを参照している気配）
			var pApp = Application.Current as App;
			Console.Write("App.MainWindow.Width = " + pApp.m_window.Content.ActualSize);
//			Console.Write("StartPage.Width = " + this.Width); // NaN
			Console.Write("StartPage.Width = " + this.ActualSize); // NaN
#endif
		}

		ObservableCollection<Person> 	pItems = new ObservableCollection<Person>();
	}
}
