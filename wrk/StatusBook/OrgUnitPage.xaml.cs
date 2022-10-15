using Arteria_s.DB.Base;
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
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class OrgUnitPage : Page
	{
		//　保存先コンテナから登録するコンテナを探し、そこに指定要素を追加
		private bool AppendOrgUnit(ObservableCollection<OrgUnit> pOrgUnits, OrgUnit pAppendOrgUnit)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			foreach (var pOrgUnit in pOrgUnits)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Compare_0=" + pOrgUnit.ContainerID.ToString());
				System.Diagnostics.Trace.WriteLine("Compare_1=" + pAppendOrgUnit.ContainerID.ToString());
#endif
				if (pOrgUnit.OrgUnitID.Equals(pAppendOrgUnit.ContainerID) == true)
				{
					pOrgUnit.Children.Add(pAppendOrgUnit);
					return(true);
				}
				else
				{
					if (AppendOrgUnit(pOrgUnit.Children, pAppendOrgUnit) == true)
					{
						return(true);
					}
				}
			}
			return(false);
		}

		public OrgUnitPage()
		{
			this.InitializeComponent();

			Guid pRoot = new Guid("{00000000-0000-0000-0000-000000000001}");

			var pApp = Application.Current as App;
			var pContext = pApp.m_pContext;

			OrgUnitsCursor	pCursor = new OrgUnitsCursor();
			var pItems = pCursor.Listup(pContext);
			foreach (var pItem in pItems)
			{
				OrgUnit pOrgUnit = new OrgUnit();
				pOrgUnit.IsExpanded = true;
				pOrgUnit.Children = new ObservableCollection<OrgUnit>();
				pOrgUnit.Code = pItem.Code;
				pOrgUnit.Name = pItem.Name;
				pOrgUnit.OrgUnitID = pItem.OrgUnitID;
				pOrgUnit.ContainerID = pItem.ContainerID;

				if (pOrgUnit.OrgUnitID.CompareTo(pRoot) == 0) {
					//　ルート項目を登録するルート
					pOrgUnits.Add(pOrgUnit);
				}
				else
				{
					//　子要素を登録するルート
					AppendOrgUnit(pOrgUnits, pOrgUnit);
				}

			}
//			pOrgUnits = pItems;


			/*
			OrgUnit	pOrgUnit = new OrgUnit();
			pOrgUnit.IsExpanded = true;
			pOrgUnit.Name = "root";
			OrgUnit pChild = new OrgUnit();
			pChild.Name = "child";
			pOrgUnit.Children.Add(pChild);
			pOrgUnits.Add(pOrgUnit);
			*/

		}

		private ObservableCollection<OrgUnit> pOrgUnits = new ObservableCollection<OrgUnit>();
		//ObservableCollection<TreeViewNode> pOrgUnits = new ObservableCollection<TreeViewNode>();
	}
}
