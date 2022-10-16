using Arteria_s.DB.Base;
using Microsoft.Windows.ApplicationModel.Resources;
using Npgsql;
using Ritters;
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
		public String OrgUnitCode { get; set; }
		public String OrgUnitName { get; set; }
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
						pOrgUnit.OrgUnitCode = pReader.GetString(1);
						pOrgUnit.OrgUnitName = pReader.GetString(2);
						pOrgUnit.ContainerID = pReader.GetGuid(3);

						pItems.Add(pOrgUnit);
					}
				}
			}

			return (pItems);
		}
		public ObservableCollection<OrgUnit> Listup(Context pContext, Guid  pContainerID)
		{
			var pItems = new ObservableCollection<OrgUnit>();
			var pSQL = "SELECT OrgUnitID, Code, Name, ContainerID FROM VOrgUnits WHERE ContainerID = @ContainerID;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.AddWithValue("ContainerID", pContainerID);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						OrgUnit pOrgUnit = new OrgUnit();

						pOrgUnit.OrgUnitID = pReader.GetGuid(0);
						pOrgUnit.OrgUnitCode = pReader.GetString(1);
						pOrgUnit.OrgUnitName = pReader.GetString(2);
						pOrgUnit.ContainerID = pReader.GetGuid(3);

						pItems.Add(pOrgUnit);
					}
				}
			}

			return (pItems);
		}

		public OrgUnit	Fetch(Context pContext, Guid pOrgUnitID)
		{
			var pSQL = "SELECT OrgUnitID, Code, Name, ContainerID FROM VOrgUnits WHERE OrgUnitID = @OrgUnitID;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.AddWithValue("OrgUnitID", pOrgUnitID);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						OrgUnit pOrgUnit = new OrgUnit();

						pOrgUnit.OrgUnitID = pReader.GetGuid(0);
						pOrgUnit.OrgUnitCode = pReader.GetString(1);
						pOrgUnit.OrgUnitName = pReader.GetString(2);
						pOrgUnit.ContainerID = pReader.GetGuid(3);

						return(pOrgUnit);
					}
				}
			}

			return(null);
		}

		public Guid FetchID(Context pContext, String pCode)
		{
			Guid pOrgUnitID = Guid.Empty;
			var pSQL = "SELECT OrgUnitID FROM VOrgUnits WHERE Year = @Year AND Code = @Code;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.AddWithValue("Year", 2022);
				pCommand.Parameters.AddWithValue("Code", pCode);

				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pOrgUnitID = pReader.GetGuid(0);
						break;
					}
				}
			}

			return(pOrgUnitID);
		}

		public void Insert(Context pContext, Guid pContainerID, OrgUnit pOrgUnit)
		{
			var pSQL = "CALL AppendOrgUnit(@Year, @Code, @Name, @ContainerID)";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("Year", 2022);
				pCommand.Parameters.AddWithValue("Code", pOrgUnit.OrgUnitCode);
				pCommand.Parameters.AddWithValue("Name", pOrgUnit.OrgUnitName);
				pCommand.Parameters.AddWithValue("ContainerID", pContainerID);
				pCommand.ExecuteNonQuery();
			}
		}

		public void Update(Context pContext, OrgUnit pOrgUnit)
		{
			var pSQL = "CALL UpdateOrgUnit(@OrgUnitID, @Code, @Name)";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("OrgUnitID", pOrgUnit.OrgUnitID);
				pCommand.Parameters.AddWithValue("Code", pOrgUnit.OrgUnitCode);
				pCommand.Parameters.AddWithValue("Name", pOrgUnit.OrgUnitName);
				pCommand.ExecuteNonQuery();
			}
		}

		public void UpdateContainer(Context pContext, OrgUnit pOrgUnit)
		{
			var pSQL = "CALL UpdateOrgUnitContainer(@Year, @OrgUnitID, @ContainerID)";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("Year", 2022);
				pCommand.Parameters.AddWithValue("OrgUnitID", pOrgUnit.OrgUnitID);
				pCommand.Parameters.AddWithValue("ContainerID", pOrgUnit.ContainerID);
				pCommand.ExecuteNonQuery();
			}
		}

		public void Delete(Context pContext, Guid pOrgUnitID)
		{
			var pSQL = "DELETE FROM MOrgUnits WHERE OrgUnitID = @OrgUnitID";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("OrgUnitID", pOrgUnitID);
				pCommand.ExecuteNonQuery();
			}
		}
	}
}
