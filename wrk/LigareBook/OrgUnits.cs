using Arteria_s.DB.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class OrgUnit
	{
		public Guid OrgUnitID { get; set; }
		public String Code { get; set; }
		public String Name { get; set; }
		public Guid ContainerID { get; set; }

		public bool IsExpanded { get; set; } = false;
		public ObservableCollection<OrgUnit> Children { get; set; } = new ObservableCollection<OrgUnit>();
	}

	public class OrgUnitsCursor
	{
		public ObservableCollection<OrgUnit> Listup(Context pContext)
		{
			var pItems = new ObservableCollection<OrgUnit>();
			var pSQL = "SELECT OrgUnitID, Code, Name, ContainerID FROM VOrgUnits;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						OrgUnit pOrgUnit = new OrgUnit();

						pOrgUnit.OrgUnitID = pReader.GetGuid(0);
						pOrgUnit.Code = pReader.GetString(1);
						pOrgUnit.Name = pReader.GetString(2);
						pOrgUnit.ContainerID = pReader.GetGuid(3);

						pItems.Add(pOrgUnit);
					}
				}
			}

			return (pItems);
		}
	}
}
