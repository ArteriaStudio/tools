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
using System.Xml.Linq;
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
		private OrgProfile	m_pProfile;

		public SettingsPage()
		{
			this.InitializeComponent();
			var pApp = App.Current as AutoCA.App;
			var pProfile = pApp.m_pProfile;
			m_pProfile = pProfile.m_pOrgProfile;
		}

		private bool IsNotNull(string pText)
		{
			if (pText == null)
			{
				return(false);
			}
			if (pText.Length <= 0)
			{
				return (false);
			}
			return (true);
		}

		private bool Varidate()
		{
			if (IsNotNull(CaName.Text) == false)
			{
				return(false);
			}
			if (IsNotNull(OrgName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(OrgUnitName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(ProvinceName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(LocalityName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(CountryName.Text) == false)
			{
				return (false);
			}

			return (true);
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (Varidate() == false)
			{
				return;
			}

			var pApp = App.Current as AutoCA.App;
			var pProfile = pApp.m_pProfile;
			pProfile.Save();
			Save.IsEnabled = false;
		}


		private void Settings_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Varidate() == false)
			{
				Save.IsEnabled = false;
			}
			else
			{
				Save.IsEnabled = true;
			}

			return;
		}
	}
}
