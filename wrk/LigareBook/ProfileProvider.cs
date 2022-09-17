using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace LigareBook
{
	//　ユーザープロファイルは、SQLiteデータベースファイルに保存
	public class ProfileProvider
	{
		private static String m_pProfilePath = null;
		private static readonly String	m_pCompanyName = "Arteria";
		private static readonly String	m_pAppName = "StatusBook";

		static ProfileProvider()
		{
			var pAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.Diagnostics.Debug.WriteLine("AppData: " + pAppDataFolder);

			//　ユーザープロファイルのパス名を生成
			m_pProfilePath = pAppDataFolder + "/" + m_pCompanyName + "/" + m_pAppName + "/" + m_pAppName + ".db";

			//　データベースを利用する前提条件を整える（なければ作る）
			PrepareDatabase();
		}

		protected static void PrepareDatabase()
		{
			var m_pConnectionString = "Data Source=" + m_pProfilePath;

			using (var pConnection = new SqliteConnection(m_pConnectionString))
			using (var pCommand = pConnection.CreateCommand())
			{
				pConnection.Open();

				if (IsUpgradeLayout(pCommand, 1) == true)
				{
					DoUpgradeLayout(pCommand);
				}
				else
				{
					;
				}
			}
		}

		//　レイアウトのリビジョン番号でレイアウト更新の要否を判定
		protected static bool IsUpgradeLayout(SqliteCommand pCommand, long  lRequireRevision)
		{
			try
			{
				pCommand.CommandText = "SELECT Revision FROM LayoutVersion;";
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						var lRevision = pReader.GetInt64(0);
						if (lRequireRevision > lRevision)
						{
							return (true);
						}
					}
				}
			}
			catch (SqliteException  e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return(true);
				}
			}

			return(false);
		}

		//　テーブルレイアウトを最新に更新
		protected static bool DoUpgradeLayout(SqliteCommand pCommand)
		{
			pCommand.CommandText = @"DROP TABLE IF EXISTS LayoutVersion;";
			pCommand.ExecuteNonQuery();

			pCommand.CommandText = @"DROP TABLE IF EXISTS UserParameters;";
			pCommand.ExecuteNonQuery();

			pCommand.CommandText = @"CREATE TABLE IF NOT EXISTS LayoutVersion (Revision INTEGER);";
			pCommand.ExecuteNonQuery();

			pCommand.CommandText = @"INSERT INTO LayoutVersion VALUES (1);";
			pCommand.ExecuteNonQuery();

			pCommand.CommandText = @"CREATE TABLE IF NOT EXISTS UserParameters (Item TEXT UNIQUE, Value TEXT);";
			pCommand.ExecuteNonQuery();

			return (true);
		}

		// こちらもとてもスッキリしたコーディング。みんなニッコリ（2022/08/31 Rink）
		public static bool Save(ProfileData pProfileData)
		{
			var pProperties = pProfileData.GetType().GetProperties();

			var m_pConnectionString = "Data Source=" + m_pProfilePath;

			using (var pConnection = new SqliteConnection(m_pConnectionString))
			using (var pCommand = pConnection.CreateCommand())
			{
				pConnection.Open();

				pCommand.CommandText = "INSERT INTO UserParameters (Item, Value) VALUES ($Item, $Value) ON CONFLICT (Item) DO UPDATE SET Value = $Value";

				foreach (var pProperty in pProperties)
				{
					System.Diagnostics.Debug.WriteLine(pProperty.Name);
					System.Diagnostics.Debug.WriteLine(pProperty.GetValue(pProfileData));

					var pItem  = pProperty.Name;
					var pValue = pProperty.GetValue(pProfileData);

					pCommand.Parameters.Clear();
					pCommand.Parameters.AddWithValue("$Item", pItem);
					pCommand.Parameters.AddWithValue("$Value", pValue);
					pCommand.ExecuteNonQuery();
				}
			}

			return (true);
		}

		//　とてもスッキリしたコーディング。みんなニッコリ（2022/08/31 Rink）
		public static ProfileData Load()
		{
			ProfileData pProfileData = new ProfileData();
			var pProperties = pProfileData.GetType().GetProperties();

			var m_pConnectionString = "Data Source=" + m_pProfilePath;

			using (var pConnection = new SqliteConnection(m_pConnectionString))
			using (var pCommand = pConnection.CreateCommand())
			{
				pConnection.Open();

				pCommand.CommandText = "SELECT Item, Value FROM UserParameters;";
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						var pItem  = pReader.GetString(0);
						var pValue = pReader.GetString(1);

						foreach(var pProperty in pProperties)
						{
							System.Diagnostics.Debug.WriteLine(pProperty.Name);
							if (pItem.Equals(pProperty.Name) == true)
							{
								//　第一引数は、設定対象となるクラスのインスタンスを指定
								pProperty.SetValue(pProfileData, pValue);
							}
						}
					}
				}
			}

			return (pProfileData);
		}

	}
}
