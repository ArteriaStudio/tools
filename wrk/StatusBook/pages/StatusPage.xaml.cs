using Arteria_s.Common.LigareBook;
using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook.pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StatusPage : Page
	{
		public StatusPage()
		{
			InitializeComponent();

			//　一覧データを入力
			var pApp = Application.Current as App;
			var pProfile = pApp.m_pProfile as Profile;
			var pStatusSheet = StatusSheet.GetInstance();
			pItems = pStatusSheet.Listup(pProfile);
		}

		ObservableCollection<Person>	pItems = new ObservableCollection<Person>();
	}
}
