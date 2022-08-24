﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
	public sealed partial class StatusPage : Page
	{
		public StatusPage()
		{
			this.InitializeComponent();

			for (int i=0; i < 13; i ++)
			{
				Person pPerson = new Person();
				pPerson.Name = "名前";
				pPerson.Number = "学籍番号";
				pPerson.FamilyName = "行";
				pPerson.FirstName = "秋";
				pPerson.CompositionName = pPerson.FamilyName + pPerson.FirstName;
				pItems.Add(pPerson);
			}
		}
		private void myButton_Click(object sender, RoutedEventArgs e)
		{
			this.Frame.Navigate(typeof(StartPage), null, new DrillInNavigationTransitionInfo());
		}
		ObservableCollection<Person>	pItems = new ObservableCollection<Person>();
	}
}
