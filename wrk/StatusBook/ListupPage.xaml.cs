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
	public sealed partial class ListupPage : Page
	{
		public ListupPage()
		{
			this.InitializeComponent();

			for (int i = 0; i < 31; i++)
			{
				Person pPerson = new Person();
				pPerson.Name = "項目: " + i + "[y]";
				pItems.Add(pPerson);
			}
		}
		ObservableCollection<Person> pItems = new ObservableCollection<Person>();
	}
}
