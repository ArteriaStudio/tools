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

namespace StatusBook
{
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
		/// Invoked when the application is launched normally by the end user.  Other entry points
		/// will be used such as when the application is launched to open a specific file.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
		{
			/*
			Frame rootFrame = Window.Current.Content as Frame;
			if (rootFrame == null) {
				rootFrame = new Frame();
				rootFrame.NavigationFailed += RootFrame_NavigationFailed;

				/*
				ApplicationExecutionState	eState = args.PreviousExecutionState;
				if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//
				}
				*
				Window.Current.Content = rootFrame;
			}

			if (rootFrame.Content == null)
			{
				rootFrame.Navigate(typeof(StartPage), args.Arguments);
			}

			Window.Current.Activate();
			*/

			m_window = new MainWindow();
			m_window.Activate();
		}

		protected void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new NotImplementedException();
		}

		public Window m_window;
	}
}
