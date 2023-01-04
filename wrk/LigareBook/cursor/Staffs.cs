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
