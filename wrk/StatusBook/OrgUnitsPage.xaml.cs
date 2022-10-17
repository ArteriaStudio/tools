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
using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class OrgUnitsPage : Page, IEventListener
	{
		public OrgUnitsPage()
		{
			this.InitializeComponent();

//			Guid pRoot = new Guid("{00000000-0000-0000-0000-000000000001}");
			Guid pRoot = new Guid("{00000000-0000-0000-0000-000000000000}");

			m_pOrgUnits.Clear();
			LoadItems(m_pOrgUnits, pRoot);

			var pOrgUnit = new OrgUnit();
			pOrgUnit.OrgUnitID = pRoot;
			pOrgUnit.ContainerID = Guid.Empty;

			TransitionContext pTransitoin = new TransitionContext();
			pTransitoin.pListener = this;
			pTransitoin.pArgments = pOrgUnit;

			FormFrame.Navigate(typeof(OrgUnitPage), pTransitoin);
		}

		private ObservableCollection<OrgUnit> m_pOrgUnits = new ObservableCollection<OrgUnit>();

		//　順番
		private void LoadItems_bug()
		{
			m_pOrgUnits.Clear();

			Guid pRoot = new Guid("{00000000-0000-0000-0000-000000000001}");

			var pApp = Application.Current as App;
			var pContext = pApp.m_pContext;

			OrgUnitsCursor pCursor = new OrgUnitsCursor();
			var pItems = pCursor.Listup(pContext);
			foreach (var pItem in pItems)
			{
				OrgUnit pOrgUnit = new OrgUnit();
				pOrgUnit.IsExpanded = true;
				pOrgUnit.Children = new ObservableCollection<OrgUnit>();
				pOrgUnit.OrgUnitCode = pItem.OrgUnitCode;
				pOrgUnit.OrgUnitName = pItem.OrgUnitName;
				pOrgUnit.OrgUnitID = pItem.OrgUnitID;
				pOrgUnit.ContainerID = pItem.ContainerID;

				if (pOrgUnit.OrgUnitID.CompareTo(pRoot) == 0)
				{
					//　ルート項目を登録するルート
					m_pOrgUnits.Add(pOrgUnit);
				}
				else
				{
					//　子要素を登録するルート
					AppendOrgUnit(m_pOrgUnits, pOrgUnit);
				}
			}
		}

		//　順番
		private void LoadItems(ObservableCollection<OrgUnit> pOrgUnits, Guid  pContainerID)
		{
			var pApp = Application.Current as App;
			var pContext = pApp.m_pContext;

			OrgUnitsCursor pCursor = new OrgUnitsCursor();
			var pItems = pCursor.Listup(pContext, pContainerID);
			foreach (var pItem in pItems)
			{
				OrgUnit pOrgUnit = new OrgUnit();

				pOrgUnit.IsExpanded = true;
				pOrgUnit.Children = new ObservableCollection<OrgUnit>();
				pOrgUnit.OrgUnitCode = pItem.OrgUnitCode;
				pOrgUnit.OrgUnitName = pItem.OrgUnitName;
				pOrgUnit.OrgUnitID = pItem.OrgUnitID;
				pOrgUnit.ContainerID = pItem.ContainerID;

				pOrgUnits.Add(pOrgUnit);

				LoadItems(pOrgUnit.Children, pOrgUnit.OrgUnitID);
			}
		}

		//　保存先コンテナから登録するコンテナを探し、そこに指定要素を追加
		private bool AppendOrgUnit(ObservableCollection<OrgUnit> pOrgUnits, OrgUnit pAppendOrgUnit)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			if (pOrgUnits.Count > 0)
			{
				System.Diagnostics.Trace.WriteLine("pOrgUnits.OrgUnitID=" + pOrgUnits[0].OrgUnitID.ToString());
				System.Diagnostics.Trace.WriteLine("pOrgUnits.OrgUnitName=" + pOrgUnits[0].OrgUnitName.ToString());
			}

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

		private bool InsertOrgUnit(ObservableCollection<OrgUnit> pOrgUnits, OrgUnit pInsertOrgUnit)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			foreach (var pOrgUnit in pOrgUnits)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Compare_0=" + pOrgUnit.ContainerID.ToString());
				System.Diagnostics.Trace.WriteLine("Compare_1=" + pInsertOrgUnit.ContainerID.ToString());
#endif
				if (pOrgUnit.OrgUnitID.Equals(pInsertOrgUnit.ContainerID) == true)
				{
					pOrgUnit.Children.Add(pInsertOrgUnit);
					return(true);
				}
				else
				{
					if (InsertOrgUnit(pOrgUnit.Children, pInsertOrgUnit) == true)
					{
						return(true);
					}
				}
			}

			return(false);
		}

		private bool UpdateOrgUnit(ObservableCollection<OrgUnit> pOrgUnits, OrgUnit pUpdateOrgUnit)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			foreach (var pOrgUnit in pOrgUnits)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Compare_0=" + pOrgUnit.ContainerID.ToString());
				System.Diagnostics.Trace.WriteLine("Compare_1=" + pUpdateOrgUnit.ContainerID.ToString());
#endif
				if (pOrgUnit.OrgUnitID.Equals(pUpdateOrgUnit.OrgUnitID) == true)
				{
					/*
					pOrgUnit.OrgUnitCode = pUpdateOrgUnit.OrgUnitCode;
					pOrgUnit.OrgUnitName = pUpdateOrgUnit.OrgUnitName;
					*/

					var pItem = pOrgUnit;
					pItem.IsExpanded = true;
					pItem.OrgUnitCode = pUpdateOrgUnit.OrgUnitCode;
					pItem.OrgUnitName = pUpdateOrgUnit.OrgUnitName;
					pOrgUnits.Remove(pOrgUnit);
					pOrgUnits.Add(pItem);
					return (true);
				}
				else
				{
					if (UpdateOrgUnit(pOrgUnit.Children, pUpdateOrgUnit) == true)
					{
						return (true);
					}
				}
			}

			return (false);
		}

		//　保存先コンテナから登録するコンテナを探し、そこに指定要素を追加
		private bool DeleteOrgUnit(ObservableCollection<OrgUnit> pOrgUnits, OrgUnit pDeleteOrgUnit)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			foreach (var pOrgUnit in pOrgUnits)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Compare_0=" + pOrgUnit.ContainerID.ToString());
				System.Diagnostics.Trace.WriteLine("Compare_1=" + pDeleteOrgUnit.OrgUnitID.ToString());
#endif
				if (pOrgUnit.OrgUnitID.Equals(pDeleteOrgUnit.OrgUnitID) == true)
				{
					pOrgUnits.Remove(pOrgUnit);
					return(true);
				}
				else
				{
					if (DeleteOrgUnit(pOrgUnit.Children, pDeleteOrgUnit) == true)
					{
						return(true);
					}
				}
			}
			return(false);
		}

		//　保存先コンテナから登録するコンテナを探す。（false：該当なし）
		private bool LookupOrgUnitHasChild(ObservableCollection<OrgUnit> pOrgUnits, Guid pContainerID)
		{
			System.Diagnostics.Trace.WriteLine("pOrgUnits.Count=" + pOrgUnits.Count);
			foreach (var pOrgUnit in pOrgUnits)
			{
#if DEBUG
				System.Diagnostics.Trace.WriteLine("Compare_0=" + pOrgUnit.ContainerID.ToString());
				System.Diagnostics.Trace.WriteLine("Compare_1=" + pContainerID.ToString());
#endif
				if (pOrgUnit.OrgUnitID.Equals(pContainerID) == true)
				{
					if (pOrgUnit.Children.Count() > 0)
					{
						return(true);
					}
					else
					{
						return(false);
					}
				}
				else
				{
					if (LookupOrgUnitHasChild(pOrgUnit.Children, pContainerID) == true)
					{
						return (true);
					}
				}
			}
			return (false);
		}

		private void add_Click(object sender, RoutedEventArgs e)
		{
			Button pButton = sender as Button;
			System.Diagnostics.Trace.WriteLine("Tag: " + pButton.Tag);

			Guid	pContainerID = new Guid(pButton.Tag.ToString());

			var pApp = Application.Current as App;
			var pContext = pApp.m_pContext;

			var pOrgUnit = new OrgUnit();
			pOrgUnit.OrgUnitID = Guid.Empty;
			pOrgUnit.OrgUnitCode = "";
			pOrgUnit.OrgUnitName = "";
			pOrgUnit.ContainerID = pContainerID;
/*
			var pOrgUnitsCursor = new OrgUnitsCursor();
			pOrgUnitsCursor.Insert(pContext, pContainerID, pOrgUnit);
			pOrgUnit.OrgUnitID = pOrgUnitsCursor.FetchID(pContext, pOrgUnit.Code);
*/
			InsertOrgUnit(m_pOrgUnits, pOrgUnit);
		}

		private void remove_Click(object sender, RoutedEventArgs e)
		{
			Button pButton = sender as Button;
			System.Diagnostics.Trace.WriteLine("Tag: " + pButton.Tag);

			Guid pContainerID = new Guid(pButton.Tag.ToString());

			if (LookupOrgUnitHasChild(m_pOrgUnits, pContainerID) == false)
			{
				var pApp = Application.Current as App;
				var pContext = pApp.m_pContext;

				var pOrgUnit = new OrgUnit();
				pOrgUnit.OrgUnitID = pContainerID;
				pOrgUnit.OrgUnitCode = "";
				pOrgUnit.OrgUnitName = "";

				var pOrgUnitsCursor = new OrgUnitsCursor();
				pOrgUnitsCursor.Delete(pContext, pContainerID);

				DeleteOrgUnit(m_pOrgUnits, pOrgUnit);
			}

		}

		private void more_Click(object sender, RoutedEventArgs e)
		{
			Button	pButton = sender as Button;
			System.Diagnostics.Trace.WriteLine("Tag: " + pButton.Tag);

		}

		private void TreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			var pOrgUnit = args.InvokedItem as OrgUnit;

			TransitionContext pTransitoin = new TransitionContext();
			pTransitoin.pListener = this;
			pTransitoin.pArgments = pOrgUnit;

			FormFrame.Navigate(typeof(OrgUnitPage), pTransitoin);
		}

		public void OnInsert(object  pData)
		{
			OrgUnit pOrgUnit = pData as OrgUnit;

			AppendOrgUnit(m_pOrgUnits, pOrgUnit);
		}

		public void OnUpdate(object pData)
		{
			OrgUnit pOrgUnit = pData as OrgUnit;

			UpdateOrgUnit(m_pOrgUnits, pOrgUnit);
			//　ItemSourceに紐付くコレクションは更新済み
			//　しかし、再描画メソッドが見当たらない…（2022/10/16）
			//　コレクションに削除、追加すれば部分的に更新できるが、置換メソッドが見当たらない…
			
			//this.OrgUnits.ItemsSource = null;
			//this.OrgUnits.ItemsSource = m_pOrgUnits;
		}

		private void OrgUnits_DragItemsCompleted(TreeView sender, TreeViewDragItemsCompletedEventArgs args)
		{
			if (args.DropResult == DataPackageOperation.Move)
			{
				var pContainer = (OrgUnit)args.NewParentItem;


				foreach (var pItem in args.Items)
				{
					var pOrgUnit = (OrgUnit)pItem;
					
					//MoveContainer
					;
					
				}
				System.Diagnostics.Debug.WriteLine("Current: " + args.Items.ToString());
				System.Diagnostics.Debug.WriteLine("Parent : " + args.NewParentItem.ToString());
			}
		}
	}
}
