// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Discovery.v1;
using Google.Apis.Discovery.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
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
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Google.Apis.CloudIdentity.v1;
using Google.Apis.Oauth2.v2;
using System.Net.Http;
using Windows.Media.Protection.PlayReady;
using Windows.Storage.Streams;

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
			//LookupYoutubeByKeyword();
			//SigninByOAuth2();
			SigninCalenderByAccount();
		}

		private void Signout_Click(object sender, RoutedEventArgs e)
		{
			SignoutCalenderByAccount();
		}

		private void Post_Click(object sender, RoutedEventArgs e)
		{
			PostCalendarByAccount();
		}

		private void Query_Click(object sender, RoutedEventArgs e)
		{
			GetUserinfo2();
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

		private async void SigninByOAuth2()
		{
			UserCredential	pCredential;

			var pDirectory = Directory.GetCurrentDirectory();
			System.Diagnostics.Debug.WriteLine("Current Directory: " + pDirectory);
			using (var pStream = new FileStream("D:\\home\\rink\\projects\\tools\\wrk\\Arteria_s.App.SignOn\\client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				pCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(pStream).Secrets,
					new[] { BooksService.Scope.Books },
					"user", CancellationToken.None, new FileDataStore("Books.ListMyLibrary"));
			}

			// Create the service.
			var service = new BooksService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = pCredential,
				ApplicationName = "Books API Sample",
			});

			var bookshelves = await service.Mylibrary.Bookshelves.List().ExecuteAsync();

			;
		}

		private async void SigninCalenderByAccount()
		{
			UserCredential pCredential;

			var pDirectory = Directory.GetCurrentDirectory();
			System.Diagnostics.Debug.WriteLine("Current Directory: " + pDirectory);
			var pFileDataStore = new FileDataStore("arteria-s/lumine");

			using (var pStream = new FileStream("D:\\home\\rink\\projects\\tools\\wrk\\Arteria_s.App.SignOn\\client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				pCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(pStream).Secrets,
					new[] { CalendarService.Scope.Calendar },
					"user", CancellationToken.None, pFileDataStore);
			}

			// Create the service.
			var pService = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = pCredential,
				ApplicationName = "Lumine.arteria-s.net.",
			});

			var pCalendar = await pService.CalendarList.List().ExecuteAsync();

			foreach (var pItem in pCalendar.Items)
			{
				System.Diagnostics.Debug.WriteLine("CalendarId: " + pItem.Id);
				//System.Diagnostics.Debug.WriteLine("CalendarDescription: " + pItem.Description);
				EnumCalendarEvents(pService, pItem.Id);
			}
		}

		private void EnumCalendarEvents(CalendarService pCalendar, string pCalendarId)
		{
			var pRequest = pCalendar.Events.List(pCalendarId);
			pRequest.TimeMin = DateTime.Now;
			pRequest.ShowDeleted = false;
			pRequest.SingleEvents = true;
			pRequest.MaxResults = 10;
			pRequest.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
			var pEvents = pRequest.Execute();
			foreach (var pItem in pEvents.Items)
			{
				System.Diagnostics.Debug.WriteLine("Summary: " + pItem.Summary);
				System.Diagnostics.Debug.WriteLine("Description: " + pItem.Description);
			}
		}

		private async void SignoutCalenderByAccount()
		{
			var pFileDataStore = new FileDataStore("arteria-s/lumine");
			await pFileDataStore.ClearAsync();
		}

		private async void PostCalendarByAccount()
		{
			UserCredential pCredential;

			var pDirectory = Directory.GetCurrentDirectory();
			System.Diagnostics.Debug.WriteLine("Current Directory: " + pDirectory);
			var pFileDataStore = new FileDataStore("arteria-s/lumine");

			using (var pStream = new FileStream("D:\\home\\rink\\projects\\tools\\wrk\\Arteria_s.App.SignOn\\client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				pCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(pStream).Secrets,
					new[] { CalendarService.Scope.Calendar },
					"user", CancellationToken.None, pFileDataStore);
			}

			// Create the service.
			var pService = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = pCredential,
				ApplicationName = "Lumine.arteria-s.net.",
			});

			Event pNewEvent = new Event();
			pNewEvent.Summary = "";
			pNewEvent.Description = "";
			pNewEvent.Start = new EventDateTime();
			pNewEvent.End = new EventDateTime();
			pNewEvent.Start.DateTime = DateTime.Now;
			pNewEvent.End.DateTime = DateTime.Now;


			string pCalendarId = "rink@arteria-s.net";
			var p = await pService.Events.Insert(pNewEvent, pCalendarId).ExecuteAsync();

			//var pCalendar = new Calendar();
			//var pCalendars = await pService.Calendars.Insert(pCalendar);
			/*
			foreach (var pItem in pCalendars.Items)
			{
				System.Diagnostics.Debug.WriteLine("CalendarId: " + pItem.Id);
				System.Diagnostics.Debug.WriteLine("CalendarDescription: " + pItem.Description);
			}
			*/
		}

		private async void GetUserinfo()
		{
			UserCredential pCredential;

			var pDirectory = Directory.GetCurrentDirectory();
			System.Diagnostics.Debug.WriteLine("Current Directory: " + pDirectory);
			var pFileDataStore = new FileDataStore("arteria-s/lumine");

			using (var pStream = new FileStream("D:\\home\\rink\\projects\\tools\\wrk\\Arteria_s.App.SignOn\\client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				pCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(pStream).Secrets,
					new[] { CloudIdentityService.Scope.CloudIdentityDevicesReadonly },
					"user", CancellationToken.None, pFileDataStore);
			}

			// Create the service.
			var pService = new CloudIdentityService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = pCredential,
				ApplicationName = "Lumine.arteria-s.net.",
			});



			//AccountPicture.Source
			//AccountPicture.ProfilePicture.Source;

			/*
			var pCalendar = await pService..List().ExecuteAsync();

			foreach (var pItem in pCalendar.Items)
			{
				System.Diagnostics.Debug.WriteLine("CalendarId: " + pItem.Id);
				//System.Diagnostics.Debug.WriteLine("CalendarDescription: " + pItem.Description);
				EnumCalendarEvents(pService, pItem.Id);
			}
			*/
			;
		}

		private async void GetUserinfo2()
		{
			UserCredential pCredential;

			var pDirectory = Directory.GetCurrentDirectory();
			System.Diagnostics.Debug.WriteLine("Current Directory: " + pDirectory);
			var pFileDataStore = new FileDataStore("arteria-s/lumine");

			using (var pStream = new FileStream("D:\\home\\rink\\projects\\tools\\wrk\\Arteria_s.App.SignOn\\client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				pCredential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(pStream).Secrets,
					new[] { Oauth2Service.Scope.UserinfoProfile, Oauth2Service.Scope.UserinfoEmail },
					"user", CancellationToken.None, pFileDataStore);
			}

			// Create the service.
			var pService = new Oauth2Service(new BaseClientService.Initializer()
			{
				HttpClientInitializer = pCredential,
				ApplicationName = "Lumine.arteria-s.net.",
			});

			var pUserinfo = await pService.Userinfo.Get().ExecuteAsync();

			System.Diagnostics.Debug.WriteLine("Email: " + pUserinfo.Email);
			System.Diagnostics.Debug.WriteLine("Name: " + pUserinfo.Name);
			System.Diagnostics.Debug.WriteLine("FamilyName: " + pUserinfo.FamilyName);
			System.Diagnostics.Debug.WriteLine("GivenName: " + pUserinfo.GivenName);
			System.Diagnostics.Debug.WriteLine("Picture: " + pUserinfo.Picture);

			//　イメージをダウンロードして画像を表示
			var pClient = new HttpClient();
			try
			{
				using HttpResponseMessage pResponse = await pClient.GetAsync(pUserinfo.Picture);
				pResponse.EnsureSuccessStatusCode();
				var pStream = await pResponse.Content.ReadAsStreamAsync();
				IRandomAccessStream pImageStream = WindowsRuntimeStreamExtensions.AsRandomAccessStream(pStream);
				AccountPicture.SetSource(pImageStream);
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);
			}
		}
	}
}
