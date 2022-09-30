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
	public sealed partial class PersonsPage : Page
	{
		public PersonsPage()
		{
			this.InitializeComponent();

			//　一覧データを入力
			var pApp = Application.Current as App;
			var pProfileData = pApp.m_pProfile as ProfileData;
			var pStatusSheet = StatusSheet.GetInstance();
			pItems = pStatusSheet.Listup(pProfileData);

			pStudents = pStatusSheet.ListupStudents(pProfileData);
		}

		ObservableCollection<Person> pItems = new ObservableCollection<Person>();
		ObservableCollection<Student> pStudents = new ObservableCollection<Student>();
	}
}
