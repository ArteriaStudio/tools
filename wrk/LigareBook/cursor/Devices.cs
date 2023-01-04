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
	public class Device
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
		public String StudentNumber { get; set; }
		public String EnterAt { get; set; }
		public String LeaveAt { get; set; }
		public int Year { get; set; }
		public String School { get; set; }
		public String Grade { get; set; }
		public String Sets { get; set; }
		public String Numbers { get; set; }
	}

	public class DevicesCursor : Loader
	{
		public ObservableCollection<Device> Listup(SQLContext pContext)
		{
			var pItems = new ObservableCollection<Device>();
			var pSQL = "SELECT AccountID, Email, Name, StudentNumber, Year, School, Grade, Sets, Numbers FROM VStudents;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						Device pDevice = new Device();
						pDevice.AccountID = pReader.GetGuid(0);
						pDevice.Email = pReader.GetString(1);
						pDevice.Name = pReader.GetString(2);
						pDevice.StudentNumber = pReader.GetString(3);
						pDevice.Year = pReader.GetInt32(4);
						pDevice.School = pReader.GetString(5);
						pDevice.Grade = pReader.GetString(6);
						pDevice.Sets = pReader.GetString(7);
						pDevice.Numbers = pReader.GetString(8);
						pItems.Add(pDevice);
					}
				}
			}

			return (pItems);
		}
	}
}
