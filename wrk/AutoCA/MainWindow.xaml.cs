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
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			var pApp = App.Current as App;
			if (pApp.m_pPrepareFlags.bExistDbParams == false)
			{
				//�@�ݒ���i�ڑ������͉�ʁj�ɑJ��
				this.ContentFrame.Navigate(typeof(SettingsPage));
			}
			else if (pApp.m_pPrepareFlags.bExistIdentity == false)
			{
				//�@�ݒ���i�F�؋ǎ�̎ҏ����͉�ʁj�ɑJ��
				this.ContentFrame.Navigate(typeof(IdentityPage));
			}
			else if (pApp.m_pPrepareFlags.bExistAuthority == false)
			{
				//�@�F�؋Ǐؖ������s�ɑJ��
				this.ContentFrame.Navigate(typeof(CreateCAPage));
			}
			else
			{
				//�@����̏�����ʂɑJ��
				this.ContentFrame.Navigate(typeof(DefaultPage));
			}
		}

		private void createRootCert_Click(object sender, RoutedEventArgs e)
		{
			//�@�����v���𐶐��ider�`���j
			string pSubject = "cn=Arteria-RCA";
			ECDsaCng pKey = new ECDsaCng();
			CertificateRequest pRequest = new CertificateRequest(pSubject, pKey, HashAlgorithmName.SHA256);
			//�@CA����F�ؖ������F�؋ǂł��邩�ۂ����w�肷��B
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
			pRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(pRequest.PublicKey, false));
			var pKeyUsageFlags = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.EncipherOnly;
			pRequest.CertificateExtensions.Add(new X509KeyUsageExtension(pKeyUsageFlags, false));
			OidCollection pEnhancedKeyUsageFlags = new OidCollection
			{
				// https://oidref.com/1.3.6.1.5.5.7.3.2
				new Oid("1.3.6.1.5.5.7.3.1"),   //�@serverAuth
				new Oid("1.3.6.1.5.5.7.3.3")    //�@codeSigning
			};
			pRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(pEnhancedKeyUsageFlags, false));

			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
			var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
			var pBytes = pRootCert.Export(X509ContentType.Cert);

			File.WriteAllBytes("D:/tmp/root.crt", pBytes);
		}

		private void DumpCertificate(string pFilepath)
		{
			X509Certificate pCert = new X509Certificate(pFilepath);
			string resultsTrue = pCert.ToString(true);
			Console.WriteLine(resultsTrue);
			string resultsFalse = pCert.ToString(false);
			Console.WriteLine(resultsFalse);
		}

		private void createCertificate_Click(object sender, RoutedEventArgs e)
		{

		}

		private void createRequest_Click(object sender, RoutedEventArgs e)
		{
			//�@�����v���𐶐��ider�`���j
			string pSubject = "cn=���{��ŋ��ʖ�";
			ECDsaCng pKey = new ECDsaCng();
			CertificateRequest	pRequest = new CertificateRequest(pSubject, pKey, HashAlgorithmName.SHA256);
			//�@CA����F�ؖ������F�؋ǂł��邩�ۂ����w�肷��B
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, true, 2, true));

			var pBytes = pRequest.CreateSigningRequest();

			File.WriteAllBytes("D:/tmp/certificate.req", pBytes);
		}

		private void MenuItemView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (var pAddItem in e.AddedItems)
			{
				var pItem = pAddItem as ListViewItem;
				Debug.WriteLine($"{pItem} " + "Tag=" + pItem.Tag);
			}
		}

		private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
		{
			var clickedItem = args.SelectedItem;
			var clickedItemContainer = args.SelectedItemContainer;

			var pItem = clickedItem as NavigationViewItem;
			Debug.WriteLine("Tag=" + pItem.Tag);

			switch (pItem.Tag.ToString()) {
			case "ListupCertificate":
				ContentFrame.Navigate(typeof(DefaultPage));
				break;
			case "CreateCertificate":
				ContentFrame.Navigate(typeof(IssuePage));
				break;
			case "Request":
				ContentFrame.Navigate(typeof(RequestPage));
				break;
			case "Signing":
				ContentFrame.Navigate(typeof(SigningPage));
				break;
			case "CreateCA":
				ContentFrame.Navigate(typeof(CreateCAPage));
				break;
			case "Identity":
				ContentFrame.Navigate(typeof(IdentityPage));
				break;
			case "Settings":
				ContentFrame.Navigate(typeof(SettingsPage));
				break;
			}
		}
	}
}
