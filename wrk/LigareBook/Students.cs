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
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
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

		public void Dump()
		{
			System.Diagnostics.Debug.WriteLine(AccountID);
			System.Diagnostics.Debug.WriteLine(Email);
			System.Diagnostics.Debug.WriteLine(Name);
			System.Diagnostics.Debug.WriteLine(Read);
			System.Diagnostics.Debug.WriteLine(OrgUnitID);
			System.Diagnostics.Debug.WriteLine(Status);
			System.Diagnostics.Debug.WriteLine(ExpireAt);
			System.Diagnostics.Debug.WriteLine(UpdateAt);
			System.Diagnostics.Debug.WriteLine(DeleteAt);
			System.Diagnostics.Debug.WriteLine(StudentNumber);
			System.Diagnostics.Debug.WriteLine(EnterAt);
			System.Diagnostics.Debug.WriteLine(LeaveAt);
			System.Diagnostics.Debug.WriteLine($"{Year}");
			System.Diagnostics.Debug.WriteLine(School);
			System.Diagnostics.Debug.WriteLine(Grade);
			System.Diagnostics.Debug.WriteLine(Sets);
			System.Diagnostics.Debug.WriteLine(Numbers);
		}
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
		public DateTime BirthAt { get; set; }
		[Index(9)]
		public String StudentNumber { get; set; }
		
		public bool CheckSelf()
		{
			if (Year < 2000)
			{
				return(false);
			}
			if (Lexical.IsSchool(School) == false)
			{
				return(false);
			}
			if (Lexical.IsGrade(Grade) == false)
			{
				return (false);
			}
			if (Lexical.IsName(Name) == false)
			{
				return (false);
			}
			if (Lexical.IsRead(Read) == false)
			{
				return (false);
			}
			/*
			if (Lexical.IsNumber(Sets) == false)
			{
				return (false);
			}
			*/
			if (Lexical.IsNumber(Numbers) == false)
			{
				return (false);
			}
			if (Lexical.IsGender(Gender) == false)
			{
				return (false);
			}
			if (Lexical.IsDateTime(BirthAt) == false)
			{
				return (false);
			}
			if (Lexical.IsNumber(StudentNumber) == false)
			{
				return (false);
			}

			return (true);
		}

		public void Dump()
		{
			System.Diagnostics.Debug.WriteLine($"年度: {Year}");
			System.Diagnostics.Debug.WriteLine("学校区分: " + School);
			System.Diagnostics.Debug.WriteLine("学年: " + Grade);
			System.Diagnostics.Debug.WriteLine("名前: " + Name);
			System.Diagnostics.Debug.WriteLine("ふりがな: " + Read);
			System.Diagnostics.Debug.WriteLine("クラス: " + Sets);
			System.Diagnostics.Debug.WriteLine("出席番号: " + Numbers);
			System.Diagnostics.Debug.WriteLine("性別: " + Gender);
			System.Diagnostics.Debug.WriteLine("生年月日: " + BirthAt);
			System.Diagnostics.Debug.WriteLine("学籍番号: " + StudentNumber);
		}
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

		public void Insert(Context pContext, List<StudentCSV> pStudentsCSV)
		{
			var pTransaction = pContext.m_pConnection.BeginTransaction();

			var pSQL = "CALL UpsertStudentAccount(@OrgUnitCode, @Email, @Name, @Read, @Gender, @BirthAt, @EnterAt, @Year, @StudentNumber, @School, @Grade, @Sets, @Numbers);";
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				foreach (var pStudentCSV in pStudentsCSV)
				{
					pCommand.Parameters.Clear();

					var pOrgUnitCode = "000000";
					var pEmail = $"{pStudentCSV.StudentNumber}@class.bunri-s.ed.jp";
					var pEnterAt = new DateTime(2022, 4, 1);

					pCommand.Parameters.AddWithValue("OrgUnitCode", pOrgUnitCode);
					pCommand.Parameters.AddWithValue("Email", pEmail);
					pCommand.Parameters.AddWithValue("Name", pStudentCSV.Name);
					pCommand.Parameters.AddWithValue("Read", pStudentCSV.Read);
					pCommand.Parameters.AddWithValue("Gender", pStudentCSV.Gender);
					pCommand.Parameters.AddWithValue("BirthAt", pStudentCSV.BirthAt);
					pCommand.Parameters.AddWithValue("EnterAt", pEnterAt);
					pCommand.Parameters.AddWithValue("Year", pStudentCSV.Year);
					pCommand.Parameters.AddWithValue("StudentNumber", pStudentCSV.StudentNumber);
					if (pStudentCSV.School.Equals("中学校") == true)
					{
						pCommand.Parameters.AddWithValue("School", "中学");
					}
					else
					{
						pCommand.Parameters.AddWithValue("School", pStudentCSV.School);
					}
					pCommand.Parameters.AddWithValue("Grade", pStudentCSV.Grade);
					pCommand.Parameters.AddWithValue("Sets", pStudentCSV.Sets);
					pCommand.Parameters.AddWithValue("Numbers", pStudentCSV.Numbers);

					try
					{
						pCommand.ExecuteNonQuery();
					}
					catch (PostgresException e)
					{
						System.Diagnostics.Debug.WriteLine($"Mesage: {e.MessageText} Code: {e.ErrorCode}");
					}
				}
			}

			pTransaction.Commit();

			return;
		}

		public override bool Load(string pPath, string pCodeSet, Context pContext, LoaderEventListener pListener)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var pItems = new List<StudentCSV>();
				while (pFetcher.Read())
				{
					var pRecoords = pFetcher.GetRecord<StudentCSV>();
					if (pRecoords.CheckSelf() == true)
					{
						pItems.Add(pRecoords);
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("");
					}
				}
				Insert(pContext, pItems);
			}

			return(true);
		}
	}
}
