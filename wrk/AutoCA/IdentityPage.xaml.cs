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

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class IdentityPage : Page
	{
		private Identity m_pIdentity;
		private OrgProfile m_pOrgProfile;

		public IdentityPage()
		{
			this.InitializeComponent();
			var pApp = App.Current as AutoCA.App;
			m_pIdentity   = pApp.m_pIdentity;
			m_pOrgProfile = pApp.m_pOrgProfile;
			/*
			var pProfile = pApp.m_pProfile;
			var pCertsStock = pApp.m_pCertsStock;
			*/

			//m_pProfile = pProfile.m_pOrgProfile;
		}

		//　TODO: データオブジェクト側に検査処理を寄せること
		private bool IsNotNull(string pText)
		{
			if (pText == null)
			{
				return (false);
			}
			if (pText.Length <= 0)
			{
				return (false);
			}
			return (true);
		}

		private bool Validate()
		{
			if (IsNotNull(AuthorityName.Text) == false)
			{
				return (false);
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
			if (Validate() == false)
			{
				return;
			}

			var pApp = App.Current as AutoCA.App;
			pApp.SaveIdentity();
			pApp.SaveOrgProfile();
			//pApp.m_pOrgProfile.Save();


			//var pCertsStock = pApp.m_pCertsStock;
			//pCertsStock.Save()


			//var pProfile = pApp.m_pProfile;
			//pProfile.Save();
			Save.IsEnabled = false;
		}


		private void Settings_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Validate() == false)
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
