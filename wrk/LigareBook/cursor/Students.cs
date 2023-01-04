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
		public string Email { get; set; }
		public string Name { get; set; }
		public string Read { get; set; }
		public Guid OrgUnitID { get; set; }
		public string OrgUnitName { get; set; }
		public int Status { get; set; }
		public DateTime ExpireAt { get; set; }
		public DateTime UpdateAt { get; set; }
		public DateTime DeleteAt { get; set; }
		public string Gender { get; set; }
		public DateTime BirthAt { get; set; }
		public string StudentNumber { get; set; }
		public DateTime EnterAt { get; set; }
		public DateTime LeaveAt { get; set; }
		public int Year { get; set; }
		public string School { get; set; }
		public string Grade { get; set; }
		public string Sets { get; set; }
		public string Numbers { get; set; }

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

	public class StudentsCursor
	{
		public StudentsCursor()
		{
			;
		}

		public ObservableCollection<Student> Listup(SQLContext pContext)
		{
			var pItems = new ObservableCollection<Student>();
			var pSQL = "SELECT AccountID, Email, Name, Read, OrgUnitID, OrgUnitName, Status, ExpireAt, UpdateAt, DeleteAt, Gender, BirthAt, StudentNumber, EnterAt, LeaveAt, Year, School, Grade, Sets, Numbers FROM VStudents;";

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
						pStudent.Read          = pReader.GetString(3);
						pStudent.OrgUnitID     = pReader.GetGuid(4);
						pStudent.OrgUnitName   = pReader.GetString(5);
						pStudent.Status        = pReader.GetInt32(6);
						pStudent.ExpireAt      = SafeGetDateTime(pReader, 7);
						pStudent.UpdateAt      = SafeGetDateTime(pReader, 8);
						pStudent.DeleteAt      = SafeGetDateTime(pReader, 9);
						pStudent.Gender        = pReader.GetString(10);
						pStudent.BirthAt       = SafeGetDateTime(pReader, 11);
						pStudent.StudentNumber = pReader.GetString(12);
						pStudent.EnterAt       = SafeGetDateTime(pReader, 13);
						pStudent.LeaveAt       = SafeGetDateTime(pReader, 14);
						pStudent.Year          = pReader.GetInt32(15);
						pStudent.School        = pReader.GetString(16);
						pStudent.Grade         = pReader.GetString(17);
						pStudent.Sets          = pReader.GetString(18);
						pStudent.Numbers       = pReader.GetString(19);

						pItems.Add(pStudent);
					}
				}
			}

			return(pItems);
		}

		protected DateTime	SafeGetDateTime(NpgsqlDataReader pReader, int  iColumn)
		{
			if (pReader.IsDBNull(iColumn) == false)
			{
				return(pReader.GetDateTime(iColumn));
			}
			return (new DateTime());
		}

		public void Insert(SQLContext pContext, List<StudentCSV> pStudentsCSV)
		{
			var pTransaction = pContext.m_pConnection.BeginTransaction();

			var pSQL = "CALL UpsertStudentAccount(@OrgUnitCode, @Email, @Name, @Read, @Gender, @BirthAt, @EnterAt, @Year, @StudentNumber, @School, @Grade, @Sets, @Numbers);";
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				foreach (var pStudentCSV in pStudentsCSV)
				{
					pCommand.Parameters.Clear();

					var pOrgUnitCode = "00000000";
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

		public static void Insert(SQLContext pContext, List<Student> pStudents)
		{
			var pTransaction = pContext.m_pConnection.BeginTransaction();

			var pSQL = "CALL UpsertStudentAccount(@OrgUnitCode, @Email, @Name, @Read, @Gender, @BirthAt, @EnterAt, @Year, @StudentNumber, @School, @Grade, @Sets, @Numbers);";
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				foreach (var pStudent in pStudents)
				{
					pCommand.Parameters.Clear();

					var pOrgUnitCode = "00000000";
					var pEmail = $"{pStudent.StudentNumber}@class.bunri-s.ed.jp";
					var pEnterAt = new DateTime(2022, 4, 1);

					pCommand.Parameters.AddWithValue("OrgUnitCode", pOrgUnitCode);
					pCommand.Parameters.AddWithValue("Email", pEmail);
					pCommand.Parameters.AddWithValue("Name", pStudent.Name);
					pCommand.Parameters.AddWithValue("Read", pStudent.Read);
					pCommand.Parameters.AddWithValue("Gender", pStudent.Gender);
					pCommand.Parameters.AddWithValue("BirthAt", pStudent.BirthAt);
					pCommand.Parameters.AddWithValue("EnterAt", pEnterAt);
					pCommand.Parameters.AddWithValue("Year", pStudent.Year);
					pCommand.Parameters.AddWithValue("StudentNumber", pStudent.StudentNumber);
					if (pStudent.School.Equals("中学校") == true)
					{
						pCommand.Parameters.AddWithValue("School", "中学");
					}
					else
					{
						pCommand.Parameters.AddWithValue("School", pStudent.School);
					}
					pCommand.Parameters.AddWithValue("Grade", pStudent.Grade);
					pCommand.Parameters.AddWithValue("Sets", pStudent.Sets);
					pCommand.Parameters.AddWithValue("Numbers", pStudent.Numbers);

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
	}
}
