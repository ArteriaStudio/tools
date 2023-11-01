using Arteria_s.DB;
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
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	public class CertInputForm : Data
	{
		public string m_pCommonName;
		public string m_pHostName;
		public string m_pMailAddress;

		public CertInputForm()
		{
			m_pCommonName  = "";
			m_pHostName    = "";
			m_pMailAddress = "";
		}

		public override bool Validate()
		{
			if (IsNotNull(m_pCommonName) == false)
			{
				return (false);
			}
			if (IsNotNull(m_pHostName) == false)
			{
				return (false);
			}
			if (IsNotNull(m_pMailAddress) == false)
			{
				return (false);
			}

			return (true);
		}
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class IssuePage : Page
	{
		CertInputForm	m_pForm = new CertInputForm();

		public IssuePage()
		{
			this.InitializeComponent();

			UpdateFormState();
		}

		//　フォームの入力値を検査
		private bool IsValidForm()
		{
			bool IsEnabled = true;

			if (CertType.SelectedIndex == -1)
			{
				IsEnabled = false;
			}
			else if (Data.IsValidCommonName(CommonName.Text) == false)
			{
				IsEnabled = false;
			}
			else
			{
				//　サーバ証明書
				if (CertType.SelectedIndex == 0)
				{
					if (Data.IsValidFQDN(HostName.Text) == false)
					{
						IsEnabled = false;
					}
				}
				//　メール証明書
				else if (CertType.SelectedIndex == 1)
				{
					if (Data.IsValidMail(MailAddress.Text) == false)
					{
						IsEnabled = false;
					}
				}
				else
				{
					IsEnabled = false;
				}
			}

			return (IsEnabled);
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			IssueButton.IsEnabled = IsValidForm();
		}

		private void CertType_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Debug.WriteLine("Selected: " + CertType.SelectedIndex);

			IssueButton.IsEnabled = IsValidForm();
			UpdateFormState();
		}

		private void UpdateFormState()
		{
			switch (CertType.SelectedIndex)
			{
				case 0:
					//　サーバ証明書にメールアドレスは不要
					CommonName.IsEnabled = true;
					HostName.IsEnabled = true;
					MailAddress.IsEnabled = false;
					break;
				case 1:
					//　メール証明書にFQDNは不要
					CommonName.IsEnabled = true;
					HostName.IsEnabled = false;
					MailAddress.IsEnabled = true;
					break;
				case -1:
					CommonName.IsEnabled = false;
					HostName.IsEnabled = false;
					MailAddress.IsEnabled = false;
					break;
			}
		}

		//　証明書発行ボタンをクリック
		private void IssueButton_Click(object sender, RoutedEventArgs e)
		{
			if (IsValidForm() == false)
			{
				return;
			}

			switch (CertType.SelectedIndex)
			{
			case 0:
				//　サーバ証明書
				var pApp = App.Current as AutoCA.App;
				var pSQLContext = pApp.GetSQLContext();
				var pCertsStock = CertsStock.Instance;
				pCertsStock.CreateForServer(pSQLContext, m_pForm.m_pCommonName, m_pForm.m_pHostName);

				var pServerCertificate = new ServerCert();
				//pServerCertificate.Create(m_pFrom.m_pCommonName, m_pFrom.m_pHostName);
				break;
			case 1:
				//　メール証明書
				var pClientCertificate = new ClientCert();
				//pClientCertificate.Create(m_pFrom.m_pCommonName,m_pFrom.m_pMailAddress);
				break;
			}


		}
	}
}
