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
	public class Staff
	{
		public Guid AccountID { get; set; }
		public String Email { get; set; }
		public String Name { get; set; }
		public String Read { get; set; }
		public String OrgUnitID { get; set; }
		public String Status { get; set; }
		public String ExpireAt { get; set; }
		public String UpdateAt { get; set; }
		public String DeleteAt { get; set; }
		public String StaffNumber { get; set; }
		public String StaffType { get; set; }
		public String ContractType { get; set; }
	}

	public class StaffsCursor : Loader
	{
		public ObservableCollection<Staff> Listup(Context pContext)
		{
			var pItems = new ObservableCollection<Staff>();
			var pSQL = "SELECT AccountID, Email, Name, StudentNumber, Year, School, Grade, Sets, Numbers FROM VStudents;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						Staff pStaff = new Staff();
						pStaff.AccountID = pReader.GetGuid(0);
						pStaff.Email = pReader.GetString(1);
						pStaff.Name = pReader.GetString(2);
						pStaff.Read = pReader.GetString(3);
						pStaff.OrgUnitID = pReader.GetString(4);
						pStaff.Status = pReader.GetString(5);
						pStaff.ExpireAt = pReader.GetString(6);
						pStaff.UpdateAt = pReader.GetString(7);
						pStaff.DeleteAt = pReader.GetString(8);
						pStaff.StaffNumber = pReader.GetString(9);
						pStaff.StaffType = pReader.GetString(10);
						pStaff.ContractType = pReader.GetString(11);
						pItems.Add(pStaff);
					}
				}
			}

			return (pItems);
		}
	}
}
