using Arteria_s.DB.Base;
using Arteria_s.UI.Base;
using Arteria_s.UI.Helper;
using CsvHelper.Configuration.Attributes;
using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	public class OptionItem
	{
		public string Label { get; set; }
		public int iNumber { get; set; }
		public override string ToString()
		{
			return(Label);
		}

	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoaderPage : Page, StudentsCursorEventListener
	{
		List<OptionItem> pLoadOptions;
		List<OptionItem> pCodePages;

		public LoaderPage()
		{
			this.InitializeComponent();

			var pResLoader = new ResourceLoader();
			pLoadOptions = new List<OptionItem>();
			pLoadOptions.Add(new OptionItem() { iNumber = 0, Label = pResLoader.GetString("StudentsList") });
			pLoadOptions.Add(new OptionItem() { iNumber = 1, Label = pResLoader.GetString("StaffList") });
			pLoadOptions.Add(new OptionItem() { iNumber = 2, Label = pResLoader.GetString("DevicesList") });
			pCodePages = new List<OptionItem>();
			pCodePages.Add(new OptionItem() { iNumber = 0, Label = pResLoader.GetString("UTF8") });
			pCodePages.Add(new OptionItem() { iNumber = 1, Label = pResLoader.GetString("SJIS") });

			//this.ListFrame.Navigate(typeof(StudentListPage));
		}

		private void LoadOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("[Enter]LoadOptions_SelectionChanged");
			foreach (OptionItem pOption in e.AddedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Add]pOption.iNumber=" + pOption.iNumber);
					switch (pOption.iNumber)
					{
					case	0:
						//this.ListFrame.Navigate(typeof(StudentListPage));
						break;
					case	1:
						//this.ListFrame.Navigate(typeof(StaffListPage));
						break;
					case	2:
						//this.ListFrame.Navigate(typeof(DeviceListPage));
						break;
					}
				}
			}

			foreach (OptionItem pOption in e.RemovedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Del]pOption.iNumber=" + pOption.iNumber);
				}
			}
			System.Diagnostics.Debug.WriteLine("[Leave]LoadOptions_SelectionChanged");
			
			ValidateMe(sender);
		}

		private void CodePages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("[Enter]CodePages_SelectionChanged");
			foreach (OptionItem pOption in e.AddedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Add]pOption.iNumber=" + pOption.iNumber);
				}
			}

			foreach (OptionItem pOption in e.RemovedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Del]pOption.iNumber=" + pOption.iNumber);
				}
			}
			System.Diagnostics.Debug.WriteLine("[Leave]CodePages_SelectionChanged");

			ValidateMe(sender);
		}

		private async void Browse_Click(object sender, RoutedEventArgs e)
		{
			//　コモンダイアログを使う道は、忘れるか諦めるのが好ましい（2022/10/12）
			//　最適解：エクスプローラでファイルをドロップしろと利用者に伝える
			//　相互運用機能に頼る（2022/10/15）
			var pApp = Application.Current as App;
			var pOpenPicker = PickerHelper.NewFileOpenPicker(pApp.m_pWindow);

			pOpenPicker.ViewMode = PickerViewMode.List;
			pOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			pOpenPicker.FileTypeFilter.Add(".csv");
			pOpenPicker.FileTypeFilter.Add("*");

			StorageFile file = await pOpenPicker.PickSingleFileAsync();
			if (file != null)
			{
				this.Filepath.Text = file.Path;
			}
			else
			{
				;
			}
		}

		private void ValidateMe(object sender)
		{
			bool fEnabled = true;

			var pLoadOptions = this.LoadOptions;
			if (pLoadOptions.SelectedIndex == -1)
			{
				fEnabled = false;
			}
			System.Diagnostics.Debug.WriteLine("[Trace]pLoadOptions.SelectedIndex=" + pLoadOptions.SelectedIndex);

			var pCodePages = this.CodePages;
			if (pCodePages.SelectedIndex == -1)
			{
				fEnabled = false;
			}
			System.Diagnostics.Debug.WriteLine("[Trace]pLoadOptions.SelectedIndex=" + pLoadOptions.SelectedIndex);

			var pFilepath = this.Filepath;
			if (pFilepath.Text.Length <= 0)
			{
				fEnabled = false;
			}
			else
			{
				if (File.Exists(this.Filepath.Text) == false)
				{
					fEnabled = false;
					System.Diagnostics.Debug.WriteLine("Not Found: " + this.Filepath.Text);
				}
				else
				{
					System.Diagnostics.Debug.WriteLine("Exist: " + this.Filepath.Text);
				}
			}

			this.Check.IsEnabled = fEnabled;
			this.Upsert.IsEnabled = fEnabled;
		}

		private void Filepath_TextChanged(object sender, TextChangedEventArgs e)
		{
			ValidateMe(sender);
		}

		//　2022/10/14：ドラッグandドロップが正常に動作しないので実装を断念
		//　→　と思ったら「操作あの終了を同期的に確認できないので、Defferralを使えとサンプルにあった。
		//　→　でもやっぱり落ちる。（foreach inの二週目の契機）
		private async void Filepath_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				System.Diagnostics.Debug.WriteLine("[Trace]1");
				//　GetDeferral()を使っても非同期関数で獲得したオブジェクトを参照すると落ちる
				var def = e.GetDeferral();
				System.Diagnostics.Debug.WriteLine("[Trace]2");
				var pItems = await e.DataView.GetStorageItemsAsync();
				System.Diagnostics.Debug.WriteLine("[Trace]3");
				var pStorageFiles = pItems;
				System.Diagnostics.Debug.WriteLine("[Trace]4");
				//　↓この辺りで落ちる（2022/10/15）Windows App SDK 1.2辺りで問題が修正されるとかなんとか
				foreach (var pStorageFile in pStorageFiles)
				{
					System.Diagnostics.Debug.WriteLine("[Trace]4.1");
					this.Filepath.Text = pStorageFile.Path;
					System.Diagnostics.Debug.WriteLine("[Trace]4.2");
				}
				System.Diagnostics.Debug.WriteLine("[Trace]5");
				e.AcceptedOperation = DataPackageOperation.Copy;
				System.Diagnostics.Debug.WriteLine("[Trace]6");
				def.Complete();
			}
			else
			{
				e.AcceptedOperation = DataPackageOperation.None;
			}

			ValidateMe(sender);
		}

		private void Filepath_DragEnter(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
			}
			else
			{
				e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
			}
		}

		private void Filepath_DragOver(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
			}
			else
			{
				e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.None;
			}
		}

		//　ファイル入力前の事前検査
		private void Check_Click(object sender, RoutedEventArgs e)
		{
			Functions.EnableAllControls(this.LoaderPagePanel, false);
			this.Browse.IsEnabled = false;

			var pLoadOptions = this.LoadOptions;
			var iLoadOption = pLoadOptions.SelectedIndex;
			var pCodePages = this.CodePages;
			var iCodePage = pCodePages.SelectedIndex;
			var pLoadFilepath = this.Filepath.Text;

			System.Diagnostics.Debug.WriteLine("Selected LoadOptions Index=" + iLoadOption);
			System.Diagnostics.Debug.WriteLine("Selected CodePages Index=" + iCodePage);
			System.Diagnostics.Debug.WriteLine("LoadFilepath=" + pLoadFilepath);

			Loader pLoader = null;
			switch (iLoadOption)
			{
				case 0:
					pLoader = new StudentsCursor(this);
					break;
				case 1:
					pLoader = new StaffsCursor();
					break;
				case 2:
					pLoader = new DevicesCursor();
					break;
			}

			string pCodePage = "";
			switch (iCodePage)
			{
				case 0:
					pCodePage = "utf-8";
					break;
				case 1:
				default:
					pCodePage = "shift_jis";
					break;
			}
			if (pLoader != null)
			{
				var pApp = Application.Current as App;
				var pContext = pApp.m_pContext;
				pLoader.Check(pLoadFilepath, pCodePage, pContext);
			}

			Functions.EnableAllControls(this.LoaderPagePanel, true);
		}

		//　ロード開始
		private void Upsert_Click(object sender, RoutedEventArgs e)
		{
			Functions.EnableAllControls(this.LoaderPagePanel, false);
			this.Browse.IsEnabled = false;

			var pLoadOptions = this.LoadOptions;
			var iLoadOption = pLoadOptions.SelectedIndex;
			var pCodePages = this.CodePages;
			var iCodePage = pCodePages.SelectedIndex;
			var pLoadFilepath = this.Filepath.Text;

			System.Diagnostics.Debug.WriteLine("Selected LoadOptions Index=" + iLoadOption);
			System.Diagnostics.Debug.WriteLine("Selected CodePages Index=" + iCodePage);
			System.Diagnostics.Debug.WriteLine("LoadFilepath=" + pLoadFilepath);

			Loader pLoader = null;
			switch (iLoadOption)
			{
				case 0:
					pLoader = new StudentsCursor(this);
					break;
				case 1:
					pLoader = new StaffsCursor();
					break;
				case 2:
					pLoader = new DevicesCursor();
					break;
			}

			string pCodePage = "";
			switch (iCodePage)
			{
				case 0:
					pCodePage = "utf-8";
					break;
				case 1:
					pCodePage = "shift_jis";
					break;
			}
			if (pLoader != null)
			{
				var pApp = Application.Current as App;
				var pContext = pApp.m_pContext;
				pLoader.Load(pLoadFilepath, pCodePage, pContext);
			}

			Functions.EnableAllControls(this.LoaderPagePanel, true);
		}

		public void OnLoaded()
		{
			throw new NotImplementedException();
		}

		public void OnChecked(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<StudentCSV> pItems)
		{
			int nLine = 0;
			this.StudentListView.pStudents.Clear();
			foreach (var pItem in pItems)
			{
				Student pStudent = new Student();

//				pStudent.AccountID
//				pStudent.Email
				pStudent.Name = pItem.Name;
				pStudent.Read = pItem.Read;
//				pStudent.OrgUnitID
//				pStudent.Status
//				pStudent.ExpireAt
//				pStudent.UpdateAt
//				pStudent.DeleteAt
				pStudent.StudentNumber = pItem.StudentNumber;
//				pStudent.EnterAt
//				pStudent.LeaveAt
				pStudent.Year = pItem.Year;
				pStudent.School = pItem.School;
				pStudent.Grade = pItem.Grade;
				pStudent.Sets = pItem.Sets;
				pStudent.Numbers = pItem.Numbers;
/*
				pItem.Gender
				pItem.BirthAt
*/
				this.StudentListView.pStudents.Add(pStudent);
				nLine ++;
				if (nLine >= 3)
				{
					break;
				}
			}
			this.StudentListView.SetCount(nItems);
		}
	}
}
