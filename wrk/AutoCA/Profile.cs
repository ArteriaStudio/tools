using Arteria_s.OS.Base;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	public class Profile : ProfileBase
	{
//		private static String m_pProfilePath = null;
		private static readonly string m_pCompanyName = "Arteria";
		private static readonly string m_pAppName = "AutoCA";
		private const long LAYOUT_VERSION = 5;
		private static SqliteConnection m_pConnection;
		public OrgProfile m_pOrgProfile;

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

			//　組織プロファイル（基礎情報）を入力
			m_pOrgProfile = LoadOrgProfile(m_pConnection);
			if (m_pOrgProfile == null)
			{
				m_pOrgProfile = new OrgProfile();
			}

			return (true);
		}

		public bool Save()
		{
			m_pConnection = Open(LAYOUT_VERSION);
			if (m_pConnection == null)
			{
				return (false);
			}
			return(SaveOrgProfile(m_pConnection, m_pOrgProfile));
		}

		//　テーブルレイアウトを最新に更新
		protected override bool DoUpgradeLayout(SqliteCommand pCommand)
		{
			List<string> pSQLs = new List<string>();

			pSQLs.Add(@"DROP TABLE IF EXISTS LayoutVersion;");
			pSQLs.Add(@"CREATE TABLE IF NOT EXISTS LayoutVersion (Revision INTEGER);");
			pSQLs.Add(@$"INSERT INTO LayoutVersion VALUES ({LAYOUT_VERSION});");
			pSQLs.Add("DROP TABLE OrgProfile;");
			pSQLs.Add("CREATE TABLE OrgProfile (OrgKey INTEGER NOT NULL, CaName TEXT NOT NULL, OrgName TEXT NOT NULL, OrgUnitName TEXT NOT NULL, localityName TEXT NULL, ProvinceName NOT NULL, countryName NOT NULL, PRIMARY KEY (OrgKey))");
			pSQLs.Add("DROP TABLE IF EXISTS IssuedCerts;");
			pSQLs.Add("CREATE TABLE IssuedCerts (SerialNumber INTEGER NOT NULL, CommonName TEXT NOT NULL, Revoked INTEGER NOT NULL,  PemData TEXT NOT NULL, PRIMARY KEY (SerialNumber))");

			foreach (var pSQL in pSQLs)
			{
				pCommand.CommandText = pSQL;
				pCommand.ExecuteNonQuery();
			}

			return(true);
		}

		//　
		protected OrgProfile LoadOrgProfile(SqliteConnection pConnection)
		{
			OrgProfile pOrgProfile = new OrgProfile();
			pOrgProfile.OrgKey = -1;

			try
			{
				pConnection.Open();
				var pSQL = "SELECT OrgKey, CaName, OrgName, OrgUnitName, LocalityName, ProvinceName, CountryName FROM OrgProfile WHERE OrgKey == 0";
				var pCommand = new SqliteCommand(pSQL ,pConnection);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pOrgProfile.OrgKey       = pReader.GetInt64(0);
						pOrgProfile.CaName       = pReader.GetString(1);
						pOrgProfile.OrgName      = pReader.GetString(2);
						pOrgProfile.OrgUnitName  = pReader.GetString(3);
						pOrgProfile.LocalityName = pReader.GetString(4);
						pOrgProfile.ProvinceName = pReader.GetString(5);
						pOrgProfile.CountryName  = pReader.GetString(6);
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

		public bool SaveOrgProfile(SqliteConnection pConnection, OrgProfile pOrgProfile)
		{
			try
			{
				pConnection.Open();
				var pSQL = "INSERT INTO OrgProfile VALUES (@OrgKey, @CaName, @OrgName, @OrgUnitName, @LocalityName, @ProvinceName, @CountryName)";
				pSQL += " ON CONFLICT (OrgKey) DO UPDATE SET CaName = @CaName, OrgName = @OrgName, OrgUnitName = @OrgUnitName, LocalityName = @LocalityName, ProvinceName = @ProvinceName, CountryName = @CountryName";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("CaName", pOrgProfile.CaName));
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

		public bool SaveCertificate(SqliteConnection pConnection, CertficateItem pCertficateItem)
		{
			try
			{
				pConnection.Open();
				var pSQL = "INSERT INTO IssuedCerts VALUES (@SerialNumber, @CommonName, false, @PemData)";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
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

		public static CertficateItem GetCertificate(int pSerialNumber)
		{
			return(GetCertificate(m_pConnection, pSerialNumber));
		}

		//　指定したシリアル番号の証明書データを取得
		protected static CertficateItem GetCertificate(SqliteConnection pConnection, int iSerialNumber)
		{
			CertficateItem pCertificateItem = new CertficateItem();

			try
			{
				pConnection.Open();
				var pSQL = "SELECT SerialNumber, CommonName, Revoked, PemData FROM IssuedCerts WHERE SerialNumber == @SerialNumber;";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
				pCommand.Parameters.Add(new SqliteParameter("SerialNumber", iSerialNumber));
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pCertificateItem.SerialNumber = pReader.GetInt32(0);
						pCertificateItem.CommonName   = pReader.GetString(1);
						pCertificateItem.Revoked      = pReader.GetInt32(2);
						pCertificateItem.PemData      = pReader.GetString(3);
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
			if (pCertificateItem.SerialNumber == -1)
			{
				return (null);
			}

			return (pCertificateItem);
		}

	}
}
