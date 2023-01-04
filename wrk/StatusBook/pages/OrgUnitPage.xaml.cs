using LigareBook;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using Npgsql;
using Ritters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace StatusBook.pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class OrgUnitPage : Page
	{
		public OrgUnitPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			if (e.Parameter is TransitionContext)
			{
				TransitionContext	pTransition = (TransitionContext)e.Parameter;
				m_pListener = pTransition.pListener;
				OrgUnit pOrgUnit = (OrgUnit)pTransition.pArgments;

				var pApp = Application.Current as App;
				var pContext = pApp.m_pContext;

				OrgUnitsCursor pCursor = new OrgUnitsCursor();
				m_pOrgUnit = pCursor.Fetch(pContext, pOrgUnit.OrgUnitID);
				if (m_pOrgUnit != null)
				{
					this.OrgUnitID.Text = m_pOrgUnit.OrgUnitID.ToString();
					this.OrgUnitName.Text = m_pOrgUnit.OrgUnitName;
					this.OrgUnitCode.Text = m_pOrgUnit.OrgUnitCode;
					this.ContainerID.Text = m_pOrgUnit.ContainerID.ToString();
				}
				else
				{
					this.OrgUnitID.Text = pOrgUnit.OrgUnitID.ToString();
					this.OrgUnitName.Text = pOrgUnit.OrgUnitName;
					this.OrgUnitCode.Text = pOrgUnit.OrgUnitCode;
					this.ContainerID.Text = pOrgUnit.ContainerID.ToString();
				}
			}
		}

		private OrgUnit	m_pOrgUnit = new OrgUnit();
		private IEventListener m_pListener = null;

		private void Apply_Click(object sender, RoutedEventArgs eargs)
		{
			OrgUnit pOrgUnit = new OrgUnit();
			pOrgUnit.OrgUnitID = new Guid(OrgUnitID.Text);
			pOrgUnit.OrgUnitName = this.OrgUnitName.Text;
			pOrgUnit.OrgUnitCode = this.OrgUnitCode.Text;
			pOrgUnit.ContainerID = new Guid(ContainerID.Text);

			if (pOrgUnit.OrgUnitID.Equals(Guid.Empty) == true)
			{
				try
				{
					//　新規登録
					var pApp = Application.Current as App;
					var pContext = pApp.m_pContext;

					var pOrgUnitsCursor = new OrgUnitsCursor();
					OrgUnitsCursor.Insert(pContext, pOrgUnit.ContainerID, pOrgUnit);
					pOrgUnit.OrgUnitID = pOrgUnitsCursor.FetchID(pContext, pOrgUnit.OrgUnitCode);

					m_pListener.OnInsert(pOrgUnit);
				}
				catch (PostgresException e)
				{
					System.Diagnostics.Debug.WriteLine(e.Message);

					if (e.SqlState.Equals("23505") == true)
					{
						DialogHelper	pDialog = new DialogHelper(this.Content.XamlRoot);
						pDialog.DisplayDialogByID("Message_Violation_UniqueKey");
					}
				}
			}
			else
			{
				//　更新
				var pApp = Application.Current as App;
				var pContext = pApp.m_pContext;

				var pOrgUnitsCursor = new OrgUnitsCursor();
				pOrgUnitsCursor.Update(pContext, pOrgUnit);
				pOrgUnit.OrgUnitID = pOrgUnitsCursor.FetchID(pContext, pOrgUnit.OrgUnitCode);

				m_pListener.OnUpdate(pOrgUnit);
			}
		}

	}
}
