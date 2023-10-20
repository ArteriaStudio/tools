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

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (m_pProfile.OrgName == null) {
				return;
			}
			if (m_pProfile.OrgUnitName == null)
			{
				return;
			}
			if (m_pProfile.CountryName == null)
			{
				return;
			}
			if (m_pProfile.ProvinceName == null)
			{
				return;
			}
			if (m_pProfile.LocalityName == null)
			{
				return;
			}

			var pApp = App.Current as AutoCA.App;
			var pProfile = pApp.m_pProfile;
			pProfile.Save();
		}
	}
}
