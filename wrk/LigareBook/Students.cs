using Arteria_s.DB.Base;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.VisualBasic;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

	public class StudentCSV
	{
		[Index(0)]
		public int Year { get; set; }
		[Index(1)]
		public String School { get; set; }
		[Index(2)]
		public String Grade { get; set; }
		[Index(3)]
		public String Name { get; set; }
		[Index(4)]
		public String Read { get; set; }
		[Index(5)]
		public String Sets { get; set; }
		[Index(6)]
		public String Numbers { get; set; }
		[Index(7)]
		public String Gender { get; set; }
		[Index(8)]
		public String BirthAt { get; set; }
		[Index(9)]
		public String StudentNumber { get; set; }
	}



	public class StudentsCursor : Loader
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

		public override bool Load(string pPath)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
			};
			using (var pReader = new StreamReader(pPath))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				pFetcher.Read();
				var pRecoords = pFetcher.GetRecord<StudentCSV>();
			}


			return(true);
		}
	}
}
