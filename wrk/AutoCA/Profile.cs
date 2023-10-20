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
		private static readonly String m_pCompanyName = "Arteria";
		private static readonly String m_pAppName = "AutoCA";
		private const long LAYOUT_VERSION = 2;
		private SqliteConnection m_pConnection;
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
			pSQLs.Add("CREATE TABLE OrgProfile (OrgKey INTEGER NOT NULL, OrgName TEXT NOT NULL, OrgUnitName TEXT NOT NULL, localityName TEXT NULL, ProvinceName NOT NULL, countryName NOT NULL, PRIMARY KEY (OrgKey))");

			foreach (var pSQL in pSQLs)
			{
				pCommand.CommandText = pSQL;
				pCommand.ExecuteNonQuery();
			}

			return(true);
		}

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
						pOrgProfile.OrgKey = pReader.GetInt64(0);
						pOrgProfile.OrgName = pReader.GetString(1);
						pOrgProfile.OrgUnitName = pReader.GetString(2);
						pOrgProfile.LocalityName = pReader.GetString(3);
						pOrgProfile.ProvinceName = pReader.GetString(4);
						pOrgProfile.CountryName = pReader.GetString(5);
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
				var pSQL = "INSERT INTO OrgProfile VALUES (@OrgKey, @OrgName, @OrgUnitName, @LocalityName, @ProvinceName, @CountryName)";
				pSQL += " ON CONFLICT (OrgKey) DO UPDATE SET OrgName = @OrgName, OrgUnitName = @OrgUnitName, LocalityName = @LocalityName, ProvinceName = @ProvinceName, CountryName = @CountryName";
				var pCommand = new SqliteCommand(pSQL, pConnection);
				pCommand.Parameters.Clear();
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
	}
}
