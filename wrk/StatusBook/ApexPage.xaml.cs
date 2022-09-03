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
	public sealed partial class ApexPage : Page
	{
		public ApexPage()
		{
			this.InitializeComponent();

			//　初期データ入力
			//this.BreadcrumbBar.ItemsSource = new string[] { "/", "dashboard", "status" };
			
			//　表示状態の変更
			//this.Caption.Visibility = Visibility.Visible;
		}
		//　画面名称（プロパティ）
		public String	ApexCaption { get { return(this.Caption.Text); } set { this.Caption.Text = value; } }
}
}
