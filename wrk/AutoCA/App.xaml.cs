using Arteria_s.DB;
using Arteria_s.DB.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.Security.Authentication.OnlineId;
using Windows.Storage;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	public class PrepareFlags
	{
		public bool bExistDbParams;		//　データベースとの接続情報が登録されている。
		public bool	bExistIdentity;		//　認証局の主体情報が登録されている。
		public bool bExistOrgProfile;	//　
		public bool	bExistAuthority;	//　有効な認証局証明書が存在する。

		public PrepareFlags()
		{
			bExistDbParams   = false;
			bExistIdentity   = false;
			bExistOrgProfile = false;
			bExistAuthority  = false;
		}

		//　環境の前提条件の状態を検査
		public void Check(SQLContext pSQLContext, Profile pProfile, Authority pAuthority)
		{
			//　データベース接続情報が登録されているか？
			if (pProfile.m_pDbParams == null)
			{
				bExistDbParams = false;
			}
			else
			{
				bExistDbParams = pProfile.m_pDbParams.Validate();
			}

			//　認証局の主体情報が登録されているか？
			if ((pAuthority == null) || (pAuthority.m_pIdentity == null))

            {
				bExistIdentity = false;
			}
			else
			{
				bExistIdentity = pAuthority.m_pIdentity.Validate();
			}
            if ((pAuthority == null) || (pAuthority.m_pOrgProfile == null))
			{
				bExistOrgProfile = false;
			}
			else
			{
				bExistOrgProfile = pAuthority.m_pOrgProfile.Validate();
			}

			//　有効な認証局証明書が存在するか？
			if ((pAuthority == null) || (pAuthority.Validate(pSQLContext) == false))
			{
				bExistAuthority = false;
			}
			else
			{
				bExistAuthority = true;
			}

			return;
		}
	}



	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application
	{
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			//　WinUI3かつUnpackagedのアプリケーションは、動的に表示言語を変更する機能はないと思われる。（2023/11/13：said stack overflow...）
			//Debug.WriteLine(Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride);
			//Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "ja-JP";
			//Windows.ApplicationModel.Resources.Core.ResourceContext.SetGlobalQualifierValue("Language", "de-DE");
			//ApplicationLanguages.PrimaryLanguageOverride
			//Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "ja";

			this.InitializeComponent();
			/*
			//
			var resourceManager = new Microsoft.Windows.ApplicationModel.Resources.ResourceManager();
			var resourceContext = resourceManager.CreateResourceContext();
			var iCount = resourceContext.QualifierValues.Count;

			var pMap = resourceManager.MainResourceMap;
			//pMap.GetSubtree();


			resourceContext.QualifierValues["Language"] = "en-US";
			var resourceMap = resourceManager.MainResourceMap.GetSubtree("Resources");
			//resourceMap.


			var pText = resourceMap.GetValue("ExportTag.Header", resourceContext).ValueAsString;
			*/
			//　
			//InspectEnviroment();
			Debug.WriteLine(CultureInfo.CurrentUICulture.Name);


			//
			// https://learn.microsoft.com/ja-jp/windows/apps/winui/winui3/localize-winui3-app
			// https://learn.microsoft.com/ja-jp/windows/uwp/app-resources/localize-strings-ui-manifest#localize-the-string-resources
			// https://nicksnettravels.builttoroam.com/mrtcore-unpackaged/
		}

		/// <summary>
		/// Invoked when the application is launched.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			m_pProfile = new Profile();
			m_pProfile.Load();

			//　データベースインスタンスに接続
			if (m_pProfile.m_pDbParams.Validate() == true)
			{
				m_pSQLContext = new SQLContext(m_pProfile.m_pDbParams.HostName, m_pProfile.m_pDbParams.InstanceName, m_pProfile.m_pDbParams.SchemaName, m_pProfile.m_pDbParams.ClientKey, m_pProfile.m_pDbParams.ClientCrt, m_pProfile.m_pDbParams.TrustCrt);

				m_pCertsStock = Authority.Instance;
				m_pCertsStock.Load(m_pSQLContext);
			}
			m_pPrepareFlags = new PrepareFlags();
			m_pPrepareFlags.Check(m_pSQLContext, m_pProfile, m_pCertsStock);

			m_pWindow = new MainWindow();
			m_pWindow.Title = "EasyCA";
			m_pWindow.Activate();
		}

		//　
		public void SaveIdentity()
		{
			if (m_pSQLContext != null)
			{
				m_pCertsStock.m_pIdentity.Save(m_pSQLContext);
			}
		}

		//　組織プロファイルを保存
		public void SaveOrgProfile()
		{
			if (m_pSQLContext != null)
			{
				m_pCertsStock.m_pOrgProfile.Save(m_pSQLContext);
			}
		}

		public SQLContext	GetSQLContext()
		{
			return (m_pSQLContext);
		}

		protected SQLContext m_pSQLContext;

		public Window m_pWindow;
		public Profile m_pProfile;
		public PrepareFlags m_pPrepareFlags;		//　前提条件検査結果（）
		public Authority m_pCertsStock;
	}
}
