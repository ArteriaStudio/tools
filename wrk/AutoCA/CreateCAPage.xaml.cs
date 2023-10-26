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
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CreateCAPage : Page
	{
		public CreateCAPage()
		{
			this.InitializeComponent();
			pCommonName = "";
		}

		private void Create_Click(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine(pCommonName);
			var pApp = App.Current as AutoCA.App;
			


			//CertificateProvider.CreateRootCA(pApp.m_pProfile.m_pOrgProfile, pCommonName);

		}
		private string pCommonName; //Å@îFèÿã«ÇÃã§í ñº

		private void CN_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (CN.Text.Length <= 0)
			{
				Create.IsEnabled = false;
			}
			else
			{
				Create.IsEnabled = true;
			}
		}
	}
}
