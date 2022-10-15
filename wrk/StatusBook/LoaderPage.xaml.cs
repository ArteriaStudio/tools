using Arteria_s.UI.Base;
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
	public class LoadOptionItem
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
	public sealed partial class LoaderPage : Page
	{
		List<LoadOptionItem> pLoadOptions;

		public LoaderPage()
		{
			this.InitializeComponent();

			var pResLoader = new ResourceLoader();
			pLoadOptions = new List<LoadOptionItem>();
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 0, Label = pResLoader.GetString("StudentsList") });
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 1, Label = pResLoader.GetString("StaffList") });
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 2, Label = pResLoader.GetString("DevicesList") });

			this.ListFrame.Navigate(typeof(StudentListPage));
		}

		private void LoadOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("[Enter]LoadOptions_SelectionChanged");
			foreach (LoadOptionItem pOption in e.AddedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Add]pOption.iNumber=" + pOption.iNumber);
					switch (pOption.iNumber)
					{
					case	0:
						this.ListFrame.Navigate(typeof(StudentListPage));
						break;
					case	1:
						this.ListFrame.Navigate(typeof(StaffListPage));
						break;
					case	2:
						this.ListFrame.Navigate(typeof(DeviceListPage));
						break;
					}
				}
			}

			foreach (LoadOptionItem pOption in e.RemovedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Del]pOption.iNumber=" + pOption.iNumber);
				}
			}
			System.Diagnostics.Debug.WriteLine("[Leave]LoadOptions_SelectionChanged");
			
			ValidateMe(sender);
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

			this.Upsert.IsEnabled = fEnabled;
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

		private async void Filepath_Tapped(object sender, TappedRoutedEventArgs e)
		{
			//　コモンダイアログを使う道は、忘れるか諦めるのが好ましい（2022/10/12）
			//　最適解：エクスプローラでファイルをドロップしろと利用者に伝える
			//　相互運用機能に頼る（2022/10/15）
			var pApp = Application.Current as App;
			var pOpenPicker = PickerHelper.NewFileOpenPicker(pApp.m_pWindow);

			pOpenPicker.ViewMode = PickerViewMode.Thumbnail;
			pOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			pOpenPicker.FileTypeFilter.Add(".csv");

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

	}
}
