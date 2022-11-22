// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DevicesPage : Page
	{
		public DevicesPage()
		{
			this.InitializeComponent();

			//　一覧データを入力
			var pApp = Application.Current as App;
			var pProfile = pApp.m_pProfile;
			var pContext = pApp.m_pContext;
			var pStatusSheet = StatusSheet.GetInstance();
			//pDevices = pStatusSheet.ListupStudents(pContext);
		}

		ObservableCollection<Device> pDevices= new ObservableCollection<Device>();
	}
}
