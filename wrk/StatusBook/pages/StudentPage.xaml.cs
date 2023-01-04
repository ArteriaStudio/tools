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

namespace StatusBook.pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StudentPage : Page
	{
		public StudentPage()
		{
			this.InitializeComponent();
		}
		private Student m_pStudent = null;
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter is Student)
			{
				m_pStudent = e.Parameter as Student;
			}
			base.OnNavigatedTo(e);
		}
	}
}
