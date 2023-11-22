using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Storage;
using Arteria_s.UI.Base;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class BasicParametersPage : Page
	{
		private DbParams m_pDbParams;
		private bool m_bIsDirty;

		public BasicParametersPage()
		{
			this.InitializeComponent();
			var pApp = App.Current as AutoCA.App;
			var pProfile = pApp.m_pProfile;
			m_pDbParams = pProfile.m_pDbParams;
			m_bIsDirty = false;
		}

		private bool IsNotNull(string pText)
		{
			if (pText == null)
			{
				return(false);
			}
			if (pText.Length <= 0)
			{
				return (false);
			}
			return (true);
		}

		private bool Varidate()
		{
			if (IsNotNull(DatabaseServerName.Text) == false)
			{
				return(false);
			}
			if (IsNotNull(InstanceName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(SchemaName.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(ClientKey.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(ClientCrt.Text) == false)
			{
				return (false);
			}
			if (IsNotNull(RootCACrt.Text) == false)
			{
				return (false);
			}

			return (true);
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (Varidate() == false)
			{
				return;
			}

			var pApp = App.Current as AutoCA.App;
			var bResult = pApp.m_pProfile.Save();
			//Save.IsEnabled = false;
		}


		private void Settings_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (Varidate() == false)
			{
				//Save.IsEnabled = false;
			}
			else
			{
				if (m_bIsDirty == true)
				{
					//Save.IsEnabled = true;
				}
			}
			m_bIsDirty = true;

			return;
		}

		private async void BrowseClienCrt_Click(object sender, RoutedEventArgs e)
		{
			//�@�R�����_�C�A���O���g�����́A�Y��邩���߂�̂��D�܂����i2022/10/12�j
			//�@�œK���F�G�N�X�v���[���Ńt�@�C�����h���b�v����Ɨ��p�҂ɓ`����
			//�@���݉^�p�@�\�ɗ���i2022/10/15�j
			var pApp = Application.Current as App;
			var pOpenPicker = PickerHelper.NewFileOpenPicker(pApp.m_pWindow);

			pOpenPicker.ViewMode = PickerViewMode.List;
			pOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			pOpenPicker.FileTypeFilter.Add(".csv");
			pOpenPicker.FileTypeFilter.Add("*");
			//�@FileTypeFilter�̃��X�g���珉���\���Ɏg���t�B���^�[��I��������@�͖������i2023/01/22�j

			// 2023/01/22�@ChatGPT Suggestion is follow.
			// pOpenPicker.SuggestedFileType = ".csv";
			// pOpenPicker.SuggestedFileType = pOpenPicker.FileTypeFilter[1];
			//�@���◝�z�I�Ȏ����͂��������ǁA����iSuggestedFileType�j����������Ă��Ȃ��̂ł���c

			StorageFile file = await pOpenPicker.PickSingleFileAsync();
			if (file != null)
			{
				this.ClientCrt.Text = file.Path;
			}
			else
			{
				;
			}
		}

		private async void BrowseRootCACrt_Click(object sender, RoutedEventArgs e)
		{
			//�@�R�����_�C�A���O���g�����́A�Y��邩���߂�̂��D�܂����i2022/10/12�j
			//�@�œK���F�G�N�X�v���[���Ńt�@�C�����h���b�v����Ɨ��p�҂ɓ`����
			//�@���݉^�p�@�\�ɗ���i2022/10/15�j
			var pApp = Application.Current as App;
			var pOpenPicker = PickerHelper.NewFileOpenPicker(pApp.m_pWindow);

			pOpenPicker.ViewMode = PickerViewMode.List;
			pOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			pOpenPicker.FileTypeFilter.Add(".csv");
			pOpenPicker.FileTypeFilter.Add("*");
			//�@FileTypeFilter�̃��X�g���珉���\���Ɏg���t�B���^�[��I��������@�͖������i2023/01/22�j

			// 2023/01/22�@ChatGPT Suggestion is follow.
			// pOpenPicker.SuggestedFileType = ".csv";
			// pOpenPicker.SuggestedFileType = pOpenPicker.FileTypeFilter[1];
			//�@���◝�z�I�Ȏ����͂��������ǁA����iSuggestedFileType�j����������Ă��Ȃ��̂ł���c

			StorageFile file = await pOpenPicker.PickSingleFileAsync();
			if (file != null)
			{
				this.RootCACrt.Text = file.Path;
			}
			else
			{
				;
			}

		}

		private async void BrowseClientKey_Click(object sender, RoutedEventArgs e)
		{
			//�@�R�����_�C�A���O���g�����́A�Y��邩���߂�̂��D�܂����i2022/10/12�j
			//�@�œK���F�G�N�X�v���[���Ńt�@�C�����h���b�v����Ɨ��p�҂ɓ`����
			//�@���݉^�p�@�\�ɗ���i2022/10/15�j
			var pApp = Application.Current as App;
			var pOpenPicker = PickerHelper.NewFileOpenPicker(pApp.m_pWindow);

			pOpenPicker.ViewMode = PickerViewMode.List;
			pOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			pOpenPicker.FileTypeFilter.Add(".csv");
			pOpenPicker.FileTypeFilter.Add("*");
			//�@FileTypeFilter�̃��X�g���珉���\���Ɏg���t�B���^�[��I��������@�͖������i2023/01/22�j

			// 2023/01/22�@ChatGPT Suggestion is follow.
			// pOpenPicker.SuggestedFileType = ".csv";
			// pOpenPicker.SuggestedFileType = pOpenPicker.FileTypeFilter[1];
			//�@���◝�z�I�Ȏ����͂��������ǁA����iSuggestedFileType�j����������Ă��Ȃ��̂ł���c

			StorageFile file = await pOpenPicker.PickSingleFileAsync();
			if (file != null)
			{
				this.ClientKey.Text = file.Path;
			}
			else
			{
				;
			}

		}

		private void BasicParameters_LostFocus(object sender, RoutedEventArgs e)
		{
			if (m_bWriteable == false)
			{
				return;
			}
			if (Varidate() == false)
			{
				return;
			}
			var pApp = App.Current as AutoCA.App;
			var bResult = pApp.m_pProfile.Save();
		}

		public void IsWriteable(bool? bWriteable)
		{
			if (bWriteable == null)
			{
				return;
			}
			DatabaseServerName.IsReadOnly = !bWriteable.Value;
			InstanceName.IsReadOnly       = !bWriteable.Value;
			SchemaName.IsReadOnly         = !bWriteable.Value;
			ClientKey.IsReadOnly          = !bWriteable.Value;
			ClientCrt.IsReadOnly          = !bWriteable.Value;
			RootCACrt.IsReadOnly          = !bWriteable.Value;

			BrowseClientKey.IsEnabled = bWriteable.Value;
			BrowseClientCrt.IsEnabled = bWriteable.Value;
			BrowseRootCACrt.IsEnabled = bWriteable.Value;

			m_bWriteable = bWriteable.Value;
		}
		private bool m_bWriteable = false;
	}
}
