using Arteria_s.DB.Base;
using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class Student
	{
		public Guid	AccountID { get; set; }
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

	public class StudentsCursor
	{
		public StudentsCursor()
		{
			;
		}

		~StudentsCursor()
		{
			;
		}

		public ObservableCollection<Student> Listup(Context pContext)
		{
			var pItems = new ObservableCollection<Student>();
			var pSQL = "SELECT AccountID, Email, Name, StudentNumber, Year, School, Grade, Sets, Numbers FROM VStudents;";

			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						Student pStudent = new Student();
						pStudent.AccountID     = pReader.GetGuid(0);
						pStudent.Email         = pReader.GetString(1);
						pStudent.Name          = pReader.GetString(2);
						pStudent.StudentNumber = pReader.GetString(3);
						pStudent.Year          = pReader.GetInt32(4);
						pStudent.School        = pReader.GetString(5);
						pStudent.Grade         = pReader.GetString(6);
						pStudent.Sets          = pReader.GetString(7);
						pStudent.Numbers       = pReader.GetString(8);
						pItems.Add(pStudent);
					}
				}
			}

			return(pItems);
		}


	}
}
