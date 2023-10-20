using Arteria_s.OS.Base;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	public abstract class ProfileBase
	{
		private static string m_pProfilePath = null;

		public ProfileBase(string pCompanyName, string pAppName)
		{
			var pAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.Diagnostics.Debug.WriteLine("AppData: " + pAppDataFolder);

			//　ユーザープロファイルのフォルダ名を生成
			var pFolderPath = pAppDataFolder + "/" + pCompanyName + "/" + pAppName;

			DirectoryHelper.CreateDirectorySafe(pFolderPath);

			//　ユーザープロファイルのパス名を生成
			m_pProfilePath = pFolderPath + "/" + pAppName + ".db";
		}

		protected virtual bool IsUpgradeLayout(SqliteCommand pCommand, long lRequireRevision)
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
			catch (SqliteException e)
			{
				//　Brunch: レイアウトバージョンのテーブルが存在しない
				System.Diagnostics.Debug.Write("" + e.ToString());
				if (e.SqliteErrorCode == 1)
				{
					return (true);
				}
			}

			return (false);
		}

		protected abstract bool DoUpgradeLayout(SqliteCommand pCommand);

		//　データベースを開く
		//　lVersion: レイアウトバージョン番号
		protected SqliteConnection Open(long lVersion)
		{
			var m_pConnectionString = "Data Source=" + m_pProfilePath;

			var pConnection = new SqliteConnection(m_pConnectionString);
			{
				pConnection.Open();

				using (var pCommand = pConnection.CreateCommand())
				{
					if (IsUpgradeLayout(pCommand, lVersion) == true)
					{
						DoUpgradeLayout(pCommand);
					}
					else
					{
						;
					}
				}

				pConnection.Close();
			}

			return (pConnection);
		}
	}
}
