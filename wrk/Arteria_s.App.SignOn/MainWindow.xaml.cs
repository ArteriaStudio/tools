// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
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

namespace Arteria_s.App.SignOn
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void Signin_Click(object sender, RoutedEventArgs e)
		{
			GetByApiKey();
		}

		private async void GetByApiKey()
		{
			// Create the service.
			var service = new DiscoveryService(new BaseClientService.Initializer
			{
				ApplicationName = "C# Application",
				ApiKey = "AIzaSyDs2zzreZWTfgjBd4Q9q3BgONLqMOvfBtY",
			});

			// Run the request.
			Console.WriteLine("Executing a list request...");
			var result = await service.Apis.List().ExecuteAsync();

			// Display the results.
			if (result.Items != null)
			{
				foreach (DirectoryList.ItemsData api in result.Items)
				{
					Console.WriteLine(api.Id + " - " + api.Title);
				}
			}
			return;
		}
	}
}
