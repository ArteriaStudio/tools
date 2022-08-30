using Ritters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class ProfileData : CompanyData
	{
		public readonly string	m_pProfilePath = null;
		public readonly string	m_pDatabaseServer = null;
		public static readonly string	m_pAppName = "StatusBook";

		public ProfileData()
		{
			var pAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.Diagnostics.Debug.WriteLine("AppData: " + pAppDataFolder);

			//　ユーザープロファイルのパス名を生成
			m_pProfilePath += pAppDataFolder + "/" + m_pCompanyName + "/" + m_pAppName + "/" + m_pAppName + ".conf";

			//　データベースサーバー名
			m_pDatabaseServer = "stagesv";
		}
	}
}
