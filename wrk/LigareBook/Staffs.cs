using Arteria_s.DB.Base;
using CsvHelper.Configuration.Attributes;
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

	public class StaffCSV
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
				return (false);
			}
			if (Lexical.IsSchool(School) == false)
			{
				return (false);
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

	public interface StaffsCursorEventListener
	{
		public abstract void OnCheckedStaffs(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<StaffCSV> pItems);
	}

	public class StaffsCursor : Loader
	{
		StaffsCursorEventListener m_pListener = null;
		public StaffsCursor(StaffsCursorEventListener pListener)
		{
			m_pListener = pListener;
		}

		~StaffsCursor()
		{
			m_pListener = null;
		}

		public ObservableCollection<Staff> Listup(SQLContext pContext)
		{
			var pItems = new ObservableCollection<Staff>();
			var pSQL = "SELECT AccountID, Email, Name, Read, OrgUnitID, Status, ExpireAt, UpdateAt, DeleteAt, StaffNumber, StaffType, ContractType FROM VEmploys;";

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
