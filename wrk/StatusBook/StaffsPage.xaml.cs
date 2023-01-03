// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

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
	public sealed partial class StaffsPage : Page
	{
		public StaffsPage()
		{
			this.InitializeComponent();

			//�@�ꗗ�f�[�^�����
			var pApp = Application.Current as App;
			var pProfile = pApp.m_pProfile;
			var pContext = pApp.m_pContext;
			var pStatusSheet = StatusSheet.GetInstance();
			StaffsList.pStaffs = pStatusSheet.ListupStaffs(pContext);
		}
	}
}
