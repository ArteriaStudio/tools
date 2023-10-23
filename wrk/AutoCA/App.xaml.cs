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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	public class PrepareFlags
	{
		public bool	bIsIdentity;	//　認証局の主体情報が登録されている。
		public bool	bIsExistCA; 	//　有効な認証局証明書が存在する。

		public PrepareFlags()
		{
			bIsIdentity = false;
			bIsExistCA = false;
		}

		//　環境の前提条件の状態を検査
		public void Check()
		{
			bIsIdentity = false;
			bIsExistCA = false;
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
			m_pPrepareFlags = new PrepareFlags();
			m_pPrepareFlags.Check();
			m_window = new MainWindow();
			m_window.Activate();
		}

		private Window m_window;
		public Profile m_pProfile;
		public PrepareFlags m_pPrepareFlags;	//　前提条件検査結果（）
	}
}
