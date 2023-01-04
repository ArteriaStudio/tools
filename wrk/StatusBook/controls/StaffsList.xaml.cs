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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	public sealed partial class StaffsList : UserControl
	{
		public StaffsList()
		{
			this.InitializeComponent();
		}
		public void SetCount(int nStaffs)
		{
			this.Count.Text = nStaffs.ToString();
		}
		ActivityEventListener m_pListener = null;
		public void SetActivityEventListener(ActivityEventListener pListener)
		{
			m_pListener = pListener;
		}

		public ObservableCollection<Staff> pStaffs = new ObservableCollection<Staff>();

		private void ListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			if (m_pListener != null)
			{
				m_pListener.OnItemClick(sender, e);
			}
		}
	}
}
