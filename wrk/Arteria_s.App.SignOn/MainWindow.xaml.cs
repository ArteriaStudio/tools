// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
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
using System.Threading.Channels;
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
			//DiscoveryServiceByApiKey();
			LookupYoutubeByKeyword();
		}

		//　DiscoveryServiceに対するクラウド側API制限は「APIキーを制限しない」に設定しないと呼び出しがブロックされる
		private async void DiscoveryServiceByApiKey()
		{
			// Create the service.
			var service = new DiscoveryService(new BaseClientService.Initializer
			{
				ApplicationName = "C# Application",
				ApiKey = "AIzaSyDs2zzreZWTfgjBd4Q9q3BgONLqMOvfBtY",
			});

			// Run the request.
			System.Diagnostics.Debug.Write("Executing a list request...");
			var result = await service.Apis.List().ExecuteAsync();

			// Display the results.
			if (result.Items != null)
			{
				foreach (DirectoryList.ItemsData api in result.Items)
				{
					System.Diagnostics.Debug.Write(api.Id + " - " + api.Title + "\n");
				}
			}
			return;
		}

		private async void LookupYoutubeByKeyword()
		{
			YouTubeService	pYouTube = new YouTubeService(new BaseClientService.Initializer()
			{
				ApiKey = "AIzaSyDs2zzreZWTfgjBd4Q9q3BgONLqMOvfBtY"
			});
			SearchResource.ListRequest listRequest = pYouTube.Search.List("snippet");
			listRequest.Q = "impact";
			listRequest.Order = SearchResource.ListRequest.OrderEnum.Relevance;

			SearchListResponse searchResponse = await listRequest.ExecuteAsync();

			List<string> videos = new List<string>();
			List<string> channels = new List<string>();
			List<string> playlists = new List<string>();

			foreach (SearchResult searchResult in searchResponse.Items)
			{
				switch (searchResult.Id.Kind)
				{
					case "youtube#video":
						videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
						break;

					case "youtube#channel":
						channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
						break;

					case "youtube#playlist":
						playlists.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.PlaylistId));
						break;
				}
			}

			return;
		}
	}
}
