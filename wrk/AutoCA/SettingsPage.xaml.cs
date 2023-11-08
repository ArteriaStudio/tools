using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
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

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		public SettingsPage()
		{
			this.InitializeComponent();

			ConnectionTab.Navigate(typeof(BasicParametersPage));
			AuthorityTag.Navigate(typeof(IdentityPage));
			//ExportTag.Navigate(typeof());
		}

		private void ParametersTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			switch (this.ParametersTab.SelectedIndex)
			{
			case 0:
				//TabContents.Navigate(typeof(BasicParametersPage));
				break;
			case 1:
				//TabContents.Navigate(typeof(IdentityPage));
				break;
			case 2:
				break;
			}
		}

		private void Lock_Click(object sender, RoutedEventArgs e)
		{
			var pBasicParametersPage = ConnectionTab.Content as BasicParametersPage;
			var pIdentityPage = AuthorityTag.Content as IdentityPage;

			pBasicParametersPage.IsWriteable(Lock.IsChecked);
			pIdentityPage.IsWriteable(Lock.IsChecked);
		}
	}
}
