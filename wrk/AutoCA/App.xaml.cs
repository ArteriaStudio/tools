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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Authentication.OnlineId;

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
		public void Check(Profile pProfile, Identity pIdentity, OrgProfile pOrgProfile)
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
			if (pIdentity == null)
			{
				bExistIdentity = false;
			}
			else
			{
				bExistIdentity = pIdentity.Validate();
			}
			if (pOrgProfile == null)
			{
				bExistOrgProfile = false;
			}
			else
			{
				bExistOrgProfile = pOrgProfile.Validate();
			}

			//　有効な認証局証明書が存在するか？
			bExistAuthority = false;
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
			this.InitializeComponent();
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

				var iUserIdentity = 0;
				m_pIdentity = new Identity();
				m_pIdentity.Load(m_pSQLContext, iUserIdentity);
				m_pOrgProfile = new OrgProfile();
				m_pOrgProfile.Load(m_pSQLContext, iUserIdentity);
				m_pCertsStock = new CertsStock();
				m_pCertsStock.Load(m_pSQLContext, m_pIdentity, m_pOrgProfile);
			}
			m_pPrepareFlags = new PrepareFlags();
			m_pPrepareFlags.Check(m_pProfile, m_pIdentity, m_pOrgProfile);

			m_pWindow = new MainWindow();
			m_pWindow.Activate();
		}

		//　
		public void SaveIdentity()
		{
			if (m_pSQLContext != null)
			{
				m_pIdentity.Save(m_pSQLContext);
			}
		}

		//　組織プロファイルを保存
		public void SaveOrgProfile()
		{
			if (m_pSQLContext != null)
			{
				m_pOrgProfile.Save(m_pSQLContext);
			}
		}

		protected SQLContext m_pSQLContext;

		public Window m_pWindow;
		public Profile m_pProfile;
		public Identity m_pIdentity;
		public OrgProfile m_pOrgProfile;
		public PrepareFlags m_pPrepareFlags;		//　前提条件検査結果（）
		public CertsStock m_pCertsStock;
	}
}
