using Arteria_s.DB;
using Arteria_s.OS.Base;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Security.Authentication.OnlineId;

namespace AutoCA
{
	public class DbParams : Data
	{
		public DbParams()
		{
			UserIdentity = -1;
			HostName     = "";
			InstanceName = "";
			SchemaName   = "";
			ClientKey    = "";
			ClientCrt    = "";
			TrustCrt     = "";
		}

		public int UserIdentity { get; set; }
		public string HostName { get; set; }
		public string InstanceName { get; set; }
		public string SchemaName { get; set; }
		public string ClientKey { get; set; }
		public string ClientCrt { get; set; }
		public string TrustCrt { get; set; }

		public override bool Validate()
		{
			if (IsNull(HostName) == true)
			{
				return(false);
			}
			if (IsNull(InstanceName) == true)
			{
				return (false);
			}
			if (IsNull(SchemaName) == true)
			{
				return (false);
			}
			if (IsNull(ClientKey) == true)
			{
				return (false);
			}
			if (IsNull(ClientCrt) == true)
			{
				return (false);
			}
			if (IsNull(TrustCrt) == true)
			{
				return (false);
			}
			return (true);
		}
	}

	public class Profile : ProfileBase
	{
//		private static String m_pProfilePath = null;
		private static readonly string m_pCompanyName = "Arteria";
		private static readonly string m_pAppName = "AutoCA";
		private const long LAYOUT_VERSION = 11;
		private static SqliteConnection m_pConnection;
		//public OrgProfile m_pOrgProfile;
		public DbParams m_pDbParams;

		public Profile() : base(m_pCompanyName, m_pAppName)
		{
		}
	
		public bool Load()
		{
			m_pConnection = Open(LAYOUT_VERSION);
			if (m_pConnection == null)
			{
				return(false);
			}

			//　データベース接続情報を入力
			m_pDbParams = LoadDbParams(m_pConnection);
			if (m_pDbParams == null)
			{
				var pAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				var pFolderPath = pAppDataFolder + "\\postgresql";

				m_pDbParams = new DbParams();
				m_pDbParams.UserIdentity = 0;
				m_pDbParams.HostName     = "";
				m_pDbParams.InstanceName = m_pAppName;
				m_pDbParams.SchemaName   = "aploper";
				m_pDbParams.ClientKey    = pFolderPath + "\\postgresql.key";
				m_pDbParams.ClientCrt    = pFolderPath + "\\postgresql.crt";
				m_pDbParams.TrustCrt     = pFolderPath + "\\root.crt";
			}

			/*
			//　組織プロファイル（基礎情報）を入力
			m_pOrgProfile = LoadOrgProfile(m_pConnection);
			if (m_pOrgProfile == null)
			{
				m_pOrgProfile = new OrgProfile();
			}
			*/

			return (true);
		}

		public bool Save()
		{
			m_pConnection = Open(LAYOUT_VERSION);
			if (m_pConnection == null)
			{
				return (false);
			}
			return(SaveDbParams(m_pConnection, m_pDbParams));

			//return(SaveOrgProfile(m_pConnection, m_pOrgProfile));
		}

		//　テーブルレイアウトを最新に更新
		protected override bool DoUpgradeLayout(SqliteCommand pCommand)
		{
			List<string> pSQLs = new List<string>();

			pSQLs.Add(@"DROP TABLE IF EXISTS LayoutVersion;");
			pSQLs.Add(@"CREATE TABLE IF NOT EXISTS LayoutVersion (Revision INTEGER);");
			pSQLs.Add(@$"INSERT INTO LayoutVersion VALUES ({LAYOUT_VERSION});");
			pSQLs.Add("DROP TABLE IF EXISTS DbParams;");
			pSQLs.Add("CREATE TABLE DbParams (UserIdentity INTEGER NOT NULL, HostName TEXT NOT NULL, InstanceName TEXT NOT NULL, SchemaName TEXT NOT NULL, ClientKey TEXT NOT NULL, ClientCrt TEXT NOT NULL, TrustCrt TEXT NOT NULL, PRIMARY KEY (UserIdentity))");
			pSQLs.Add("DROP TABLE OrgProfile;");
			//pSQLs.Add("CREATE TABLE OrgProfile (OrgKey INTEGER NOT NULL, CaName TEXT NOT NULL, OrgName TEXT NOT NULL, OrgUnitName TEXT NOT NULL, localityName TEXT NULL, ProvinceName NOT NULL, countryName NOT NULL, PRIMARY KEY (OrgKey))");
			pSQLs.Add("DROP TABLE IF EXISTS IssuedCerts;");
			//pSQLs.Add("CREATE TABLE IssuedCerts (SequenceNumber INTEGER NOT NULL, SerialNumber TEXT NOT NULL, CommonName TEXT NOT NULL, Revoked INTEGER NOT NULL,  PemData TEXT NOT NULL, PRIMARY KEY (SequenceNumber))");

			foreach (var pSQL in pSQLs)
			{
				pCommand.CommandText = pSQL;
				pCommand.ExecuteNonQuery();
			}

			return(true);
		}

		//　
		protected DbParams LoadDbParams(SqliteConnection pConnection)
		{
			var pDbParams = new DbParams();

			try
			{
				pConnection.Open();
				var pSQL = "SELECT UserIdentity, HostName, InstanceName, SchemaName, ClientKey, ClientCrt, TrustCrt FROM DbParams WHERE UserIdentity == 0";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pDbParams.UserIdentity = pReader.GetInt32(0);
						pDbParams.HostName     = pReader.GetString(1);
						pDbParams.InstanceName = pReader.GetString(2);
						pDbParams.SchemaName   = pReader.GetString(3);
						pDbParams.ClientKey    = pReader.GetString(4);
						pDbParams.ClientCrt	   = pReader.GetString(5);
						pDbParams.TrustCrt     = pReader.GetString(6);
					}
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (null);
				}
			}
			if (pDbParams.UserIdentity == -1)
			{
				return (null);
			}

			return (pDbParams);
		}

		//
		protected bool SaveDbParams(SqliteConnection pConnection, DbParams pDbParams)
		{
			try
			{
				pDbParams.UserIdentity = 0;
				pDbParams.InstanceName = pDbParams.InstanceName.ToLower();
				pConnection.Open();
				var pSQL = "INSERT INTO DbParams VALUES (@UserIdentity, @HostName, @InstanceName, @SchemaName, @ClientKey, @ClientCrt, @TrustCrt)";
				pSQL += " ON CONFLICT (UserIdentity) DO UPDATE SET HostName = @HostName, InstanceName = @InstanceName, SchemaName = @SchemaName, ClientKey = @ClientKey, ClientCrt = @ClientCrt, TrustCrt = @TrustCrt";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("UserIdentity", pDbParams.UserIdentity));
				pCommand.Parameters.Add(new SqliteParameter("HostName",     pDbParams.HostName));
				pCommand.Parameters.Add(new SqliteParameter("InstanceName", pDbParams.InstanceName));
				pCommand.Parameters.Add(new SqliteParameter("SchemaName",   pDbParams.SchemaName));
				pCommand.Parameters.Add(new SqliteParameter("ClientKey",    pDbParams.ClientKey));
				pCommand.Parameters.Add(new SqliteParameter("ClientCrt",    pDbParams.ClientCrt));
				pCommand.Parameters.Add(new SqliteParameter("TrustCrt",     pDbParams.TrustCrt));
				var nCount = pCommand.ExecuteNonQuery();
				if (nCount <= 0)
				{
					return (false);
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (false);
				}
			}

			return (true);
		}
		/*
		//　
		protected OrgProfile LoadOrgProfile(SqliteConnection pConnection)
		{
			OrgProfile pOrgProfile = new OrgProfile();
			pOrgProfile.OrgKey = -1;

			try
			{
				pConnection.Open();
				var pSQL = "SELECT OrgKey, OrgName, OrgUnitName, LocalityName, ProvinceName, CountryName FROM OrgProfile WHERE OrgKey == 0";
				var pCommand = new SqliteCommand(pSQL ,pConnection);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pOrgProfile.OrgKey       = pReader.GetInt64(0);
						pOrgProfile.OrgName      = pReader.GetString(1);
						pOrgProfile.OrgUnitName  = pReader.GetString(2);
						pOrgProfile.LocalityName = pReader.GetString(3);
						pOrgProfile.ProvinceName = pReader.GetString(4);
						pOrgProfile.CountryName  = pReader.GetString(5);
					}
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return(null);
				}
			}
			if (pOrgProfile.OrgKey == -1)
			{
				return(null);
			}

			return(pOrgProfile);
		}
		*/
		/*
		public bool SaveOrgProfile(SqliteConnection pConnection, OrgProfile pOrgProfile)
		{
			try
			{
				pConnection.Open();
				var pSQL = "INSERT INTO OrgProfile VALUES (@OrgKey, @CaName, @OrgName, @OrgUnitName, @LocalityName, @ProvinceName, @CountryName)";
				pSQL += " ON CONFLICT (OrgKey) DO UPDATE SET CaName = @CaName, OrgName = @OrgName, OrgUnitName = @OrgUnitName, LocalityName = @LocalityName, ProvinceName = @ProvinceName, CountryName = @CountryName";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("CaName", "N/A"));
				pCommand.Parameters.Add(new SqliteParameter("OrgKey", pOrgProfile.OrgKey));
				pCommand.Parameters.Add(new SqliteParameter("OrgName", pOrgProfile.OrgName));
				pCommand.Parameters.Add(new SqliteParameter("OrgUnitName", pOrgProfile.OrgUnitName));
				pCommand.Parameters.Add(new SqliteParameter("LocalityName", pOrgProfile.LocalityName));
				pCommand.Parameters.Add(new SqliteParameter("ProvinceName", pOrgProfile.ProvinceName));
				pCommand.Parameters.Add(new SqliteParameter("CountryName", pOrgProfile.CountryName));
				var nCount = pCommand.ExecuteNonQuery();
				if (nCount <= 0)
				{
					return(false);
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (false);
				}
			}

			return (true);
		}
		*/
		/*
		public bool SaveCertificate(SqliteConnection pConnection, Certificate pCertficateItem)
		{
			try
			{
				pConnection.Open();
				var pSQL = "INSERT INTO IssuedCerts VALUES (@SequenceNumber, @SerialNumber, @CommonName, false, @PemData)";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("SequenceNumber", pCertficateItem.SequenceNumber));
				pCommand.Parameters.Add(new SqliteParameter("SerialNumber", pCertficateItem.SerialNumber));
				pCommand.Parameters.Add(new SqliteParameter("CommonName", pCertficateItem.CommonName));
				pCommand.Parameters.Add(new SqliteParameter("PemData", pCertficateItem.PemData));
				var nCount = pCommand.ExecuteNonQuery();
				if (nCount <= 0)
				{
					return (false);
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (false);
				}
			}

			return (true);
		}
		*/
		/*
		public static Certificate GetCertificate(int pSerialNumber)
		{
			return(GetCertificate(m_pConnection, pSerialNumber));
		}
		*/
		/*
		//　指定したシリアル番号の証明書データを取得
		protected static Certificate GetCertificate(SqliteConnection pConnection, int iSequenceNumber)
		{
			Certificate pCertificateItem = new Certificate();

			try
			{
				pConnection.Open();
				var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, Revoked, PemData FROM IssuedCerts WHERE SequenceNumber == @SequenceNumber;";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("SequenceNumber", iSequenceNumber));
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pCertificateItem.SequenceNumber = pReader.GetInt32(0);
						pCertificateItem.SerialNumber   = pReader.GetString(1);
						pCertificateItem.CommonName     = pReader.GetString(2);
						pCertificateItem.Revoked        = pReader.GetBoolean(3);
						pCertificateItem.PemData        = pReader.GetString(4);
					}
				}
				pConnection.Close();
			}
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (null);
				}
			}
			if (pCertificateItem.SequenceNumber == -1)
			{
				return (null);
			}

			return (pCertificateItem);
		}
		*/
	}
}
