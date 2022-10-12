using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 1, Label = pResLoader.GetString("TeachersList") });
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 2, Label = pResLoader.GetString("StaffList") });
			pLoadOptions.Add(new LoadOptionItem() { iNumber = 3, Label = pResLoader.GetString("DevicesList") });
		}

		private void LoadOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("[Enter]LoadOptions_SelectionChanged");
			foreach (LoadOptionItem pOption in e.AddedItems)
			{
				if (pOption != null)
				{
					System.Diagnostics.Debug.WriteLine("[Add]pOption.iNumber=" + pOption.iNumber);
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

		private async void Filepath_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Contains(StandardDataFormats.StorageItems))
			{
				var pItems = await e.DataView.GetStorageItemsAsync();
				for (int i = 0; i < pItems.Count; i++)
				{
					// なぜか落ちる（2022/10/12:debug）
					var pStorageFile = pItems[i] as StorageFile;
					System.Diagnostics.Debug.WriteLine("[Trace]Filepath: " + pStorageFile.Path);
					this.Filepath.Text = pStorageFile.Path;
				}
				/*
				foreach (var pItem in pItems)
				{
					var pStorageFile = pItem as StorageFile;
					System.Diagnostics.Debug.WriteLine("[Trace]Filepath: " + pStorageFile.Path);
					this.Filepath.Text = pStorageFile.Path;
				}
				*/
			}

			ValidateMe(sender);
		}

		private void Filepath_Tapped(object sender, TappedRoutedEventArgs e)
		{
			//　コモンダイアログを使う道は、忘れるか諦めるのが好ましい（2022/10/12）
			;
			//　最適解：エクスプローラでファイルをドロップしろと利用者に伝える
		}

		private void Filepath_DragOver(object sender, DragEventArgs e)
		{
			e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
			;
		}
	}
}
