using Arteria_s.DB.Base;
using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
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
		public Guid OrgUnitID { get; set; }
		public string OrgUnitName { get; set; }
		public string Status { get; set; }
		public String ExpireAt { get; set; }
		public String UpdateAt { get; set; }
		public String DeleteAt { get; set; }
		public String StaffNumber { get; set; }
		public int StaffType { get; set; }
		public String StaffTypeName { get; set; }
		public int ContractType { get; set; }
		public String ContractTypeName { get; set; }

		public String WindowsID { get; set; }
		public String GmailID { get; set; }
		public String MicrosoftID { get; set; }
		public String SequenceNumber { get; set; }
		public String MembershipCode01 { get; set; }
		public String MembershipCode02 { get; set; }
	}

	public class StaffCSV
	{
		[Index(0)]
		public int Year { get; set; }
		[Index(1)]
		public String StaffNumber { get; set; }
		[Index(2)]
		public String Name { get; set; }
		[Index(3)]
		public String Read { get; set; }
		[Index(4)]
		public String StaffTypeName { get; set; }
		[Index(5)]
		public String ContractTypeName { get; set; }
		[Index(6)]
		public String Email { get; set; }
		[Index(7)]
		public String WindowsID { get; set; }
		[Index(8)]
		public String GmailID { get; set; }
		[Index(9)]
		public String MicrosoftID { get; set; }
		[Index(10)]
		public String SequenceNumber { get; set; }
		[Index(11)]
		public String MembershipCode01 { get; set; }
		[Index(12)]
		public String MembershipCode02 { get; set; }

		public bool CheckSelf()
		{
			if (Year < 2000)
			{
				return (false);
			}
			if (Lexical.IsStaffNumber(StaffNumber) == false)
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
			if (Lexical.IsStaffTypeName(StaffTypeName) == false)
			{
				return (false);
			}
			if (Lexical.IsContractTypeName(ContractTypeName) == false)
			{
				return (false);
			}
			if (Lexical.IsEmail(Email) == false)
			{
				return (false);
			}
			if (Lexical.IsAlnum(WindowsID) == false)
			{
				return (false);
			}
			if (Lexical.IsEmail(GmailID) == false)
			{
				return (false);
			}
			if (Lexical.IsEmail(MicrosoftID) == false)
			{
				return (false);
			}
			if (Lexical.IsNumber(SequenceNumber) == false)
			{
				return (false);
			}
			if (Lexical.IsNumber(MembershipCode01, 8) == false)
			{
				return (false);
			}
			if (Lexical.IsNumber(MembershipCode02, 8) == false)
			{
				return (false);
			}

			return (true);
		}

		public void Dump()
		{
			System.Diagnostics.Debug.WriteLine($"年度: {Year}");
			System.Diagnostics.Debug.WriteLine($"職員コード: {StaffNumber}");
			System.Diagnostics.Debug.WriteLine($"氏名: {Name}");
			System.Diagnostics.Debug.WriteLine($"ふりがな: {Read}");
			System.Diagnostics.Debug.WriteLine($"職員種類: {StaffTypeName}");
			System.Diagnostics.Debug.WriteLine($"契約種類: {ContractTypeName}");
			System.Diagnostics.Debug.WriteLine($"e-Mail: {Email}");
			System.Diagnostics.Debug.WriteLine($"Windows-ID: {WindowsID}");
			System.Diagnostics.Debug.WriteLine($"Google-ID: {GmailID}");
			System.Diagnostics.Debug.WriteLine($"Microsoft-ID: {MicrosoftID}");
			System.Diagnostics.Debug.WriteLine($"表示優先度: {SequenceNumber}");
			System.Diagnostics.Debug.WriteLine($"法人会員コード（SSN）: {MembershipCode01}");
			System.Diagnostics.Debug.WriteLine($"法人会員コード（SSE）: {MembershipCode02}");
		}
	}

	public interface StaffsLoaderEventListener
	{
		public abstract void OnCheckedStaffs(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<StaffCSV> pItems);
	}

	public class StaffsLoader : Loader
	{
		StaffsLoaderEventListener m_pListener = null;
		public StaffsLoader(StaffsLoaderEventListener pListener)
		{
			m_pListener = pListener;
		}
		~StaffsLoader()
		{
			m_pListener = null;
		}
		public override bool Load(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var pItems = new List<Staff>();
				while (pFetcher.Read())
				{
					var pRecords = pFetcher.GetRecord<StaffCSV>();
					if (pRecords.CheckSelf() == true)
					{
						Staff	pStaff = new Staff();

//						pStaff.AccountID = pRecords.AccountID;
						pStaff.Email = pRecords.Email;
						pStaff.Name = pRecords.Name;
						pStaff.Read = pRecords.Read;
//						pStaff.OrgUnitName = pRecords.OrgUnitName;
//						pStaff.Status = pRecords.Status;
//						pStaff.ExpireAt = pRecords.ExpireAt;
//						pStaff.UpdateAt = pRecords.UpdateAt;
//						pStaff.DeleteAt = pRecords.DeleteAt;
						pStaff.StaffNumber = pRecords.StaffNumber;
						pStaff.StaffTypeName = pRecords.StaffTypeName;
						pStaff.ContractTypeName = pRecords.ContractTypeName;

						pStaff.WindowsID = pRecords.WindowsID;
						pStaff.GmailID = pRecords.GmailID;
						pStaff.MicrosoftID = pRecords.MicrosoftID;
						pStaff.SequenceNumber = pRecords.SequenceNumber;
						pStaff.MembershipCode01 = pRecords.MembershipCode01;
						pStaff.MembershipCode02 = pRecords.MembershipCode02;

						pItems.Add(pStaff);
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
					}
				}

				//　データベースに登録
				StaffsCursor.Insert(pContext, pItems);
			}

			return (true);
		}
		public override bool Check(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var nItems = 0;
				var nError = 0;
				var pItems = new List<StaffCSV>();
				while (pFetcher.Read())
				{
					var pRecoords = pFetcher.GetRecord<StaffCSV>();
					if (pRecoords.CheckSelf() == true)
					{
						pItems.Add(pRecoords);
						nItems++;
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
						nError++;
					}
				}
				m_pListener.OnCheckedStaffs(pPath, pCodeSet, pContext, nItems, nError, pItems);
			}

			return (true);
		}
	}

	public class StaffsCursor
	{
		public ObservableCollection<Staff> Listup(SQLContext pContext)
		{
			var pItems = new ObservableCollection<Staff>();
			var pSQL = "SELECT AccountID, Email, Name, Read, OrgUnitID, OrgUnitName, Status, ExpireAt, UpdateAt, DeleteAt, StaffNumber, StaffType, StaffTypeName, ContractType, ContractTypeName FROM VEmploys;";

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
						pStaff.OrgUnitID = pReader.GetGuid(4);
						pStaff.OrgUnitName = pReader.GetString(5);
						pStaff.Status = pReader.GetInt32(6).ToString();
						if (pReader.IsDBNull(7) == false)
						{
							pStaff.ExpireAt = pReader.GetString(7);
						}
						else
						{
							pStaff.ExpireAt = "";
						}
						if (pReader.IsDBNull(8) == false)
						{
							pStaff.UpdateAt = pReader.GetString(8);
						}
						else
						{
							pStaff.UpdateAt = "";
						}
						if (pReader.IsDBNull(9) == false)
						{
							pStaff.DeleteAt = pReader.GetString(9);
						}
						else
						{
							pStaff.DeleteAt = "";
						}
						pStaff.StaffNumber = pReader.GetString(10);
						pStaff.StaffType = pReader.GetInt32(11);
						pStaff.StaffTypeName = pReader.GetString(12);
						pStaff.ContractType = pReader.GetInt32(13);
						pStaff.ContractTypeName = pReader.GetString(14);
						pItems.Add(pStaff);
					}
				}
			}

			return (pItems);
		}

		public static void Insert(SQLContext pContext, List<StaffCSV> pStaffsCSV)
		{
			var pTransaction = pContext.m_pConnection.BeginTransaction();
			var pSQL = "CALL UpsertEmployAccount(@OrgUnitCode, @Email, @Name, @Read, @StaffNumber, @StaffType, @ContractType, @License, @WindowsID, @GmailID, @MicrosoftID, @Sequence, @SSNCode, @SSECode);";
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				foreach (var pStaffCSV in pStaffsCSV)
				{
					pCommand.Parameters.Clear();

					var pOrgUnitCode = "00000000";

					pCommand.Parameters.AddWithValue("OrgUnitCode", pOrgUnitCode);
					pCommand.Parameters.AddWithValue("Email", pStaffCSV.Email);
					pCommand.Parameters.AddWithValue("Name", pStaffCSV.Name);
					pCommand.Parameters.AddWithValue("Read", pStaffCSV.Read);
					pCommand.Parameters.AddWithValue("StaffNumber", pStaffCSV.StaffNumber);

					var pStaffTypeID = StringTable.TransformStaffTypeNameToNumber(pStaffCSV.StaffTypeName);
					pCommand.Parameters.AddWithValue("StaffType", pStaffTypeID);

					var pContractTypeID = StringTable.TransformContractTypeNameToNumber(pStaffCSV.ContractTypeName);
					pCommand.Parameters.AddWithValue("ContractType", pContractTypeID);
					pCommand.Parameters.AddWithValue("License", 0);
					pCommand.Parameters.AddWithValue("WindowsID", pStaffCSV.WindowsID);
					pCommand.Parameters.AddWithValue("GmailID", pStaffCSV.GmailID);
					pCommand.Parameters.AddWithValue("MicrosoftID", pStaffCSV.MicrosoftID);
					pCommand.Parameters.AddWithValue("Sequence", pStaffCSV.SequenceNumber);
					pCommand.Parameters.AddWithValue("SSNCode", pStaffCSV.MembershipCode01);
					pCommand.Parameters.AddWithValue("SSECode", pStaffCSV.MembershipCode02);

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

		public static void Insert(SQLContext pContext, List<Staff> pStaffs)
		{
			var pTransaction = pContext.m_pConnection.BeginTransaction();
			var pSQL = "CALL UpsertEmployAccountByName(@OrgUnitCode, @Email, @Name, @Read, @StaffNumber, @StaffTypeName, @ContractTypeName, @License, @WindowsID, @GmailID, @MicrosoftID, @Sequence, @SSNCode, @SSECode);";
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				foreach (var pStaff in pStaffs)
				{
					pCommand.Parameters.Clear();

					var pOrgUnitCode = "00000000";

					pCommand.Parameters.AddWithValue("OrgUnitCode", pOrgUnitCode);
					pCommand.Parameters.AddWithValue("Email", pStaff.Email);
					pCommand.Parameters.AddWithValue("Name", pStaff.Name);
					pCommand.Parameters.AddWithValue("Read", pStaff.Read);
					pCommand.Parameters.AddWithValue("StaffNumber", pStaff.StaffNumber);
					/*
					var pStaffTypeID = StringTable.TransformStaffTypeNameToNumber(pStaff.StaffTypeName);
					pCommand.Parameters.AddWithValue("StaffType", pStaffTypeID);

					var pContractID = StringTable.TransformContractTypeNameToNumber(pStaff.ContractTypeName);
					pCommand.Parameters.AddWithValue("ContractType", pContractID);
					*/
					pCommand.Parameters.AddWithValue("StaffTypeName", pStaff.StaffTypeName);
					pCommand.Parameters.AddWithValue("ContractTypeName", pStaff.ContractTypeName);
					pCommand.Parameters.AddWithValue("License", 0);
					pCommand.Parameters.AddWithValue("WindowsID", pStaff.WindowsID);
					pCommand.Parameters.AddWithValue("GmailID", pStaff.GmailID);
					pCommand.Parameters.AddWithValue("MicrosoftID", pStaff.MicrosoftID);
					pCommand.Parameters.AddWithValue("Sequence", pStaff.SequenceNumber);
					pCommand.Parameters.AddWithValue("SSNCode", pStaff.MembershipCode01);
					pCommand.Parameters.AddWithValue("SSECode", pStaff.MembershipCode02);

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
