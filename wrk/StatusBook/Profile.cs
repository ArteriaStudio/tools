using Ritters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace StatusBook
{
	//　ユーザープロファイル
	public class Profile : Company
	{
		public static readonly string m_pAppName = "StatusBook";

		public Profile()
		{
			var pAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			System.Diagnostics.Debug.WriteLine("AppData: " + pAppDataFolder);
			
			//　ユーザープロファイルのパス名を生成
			m_pProfile += pAppDataFolder + "/" + m_pCompanyName + "/" + m_pAppName + "/" + m_pAppName  + ".conf";

			//　ユーザープロファイルを入力
			if (LoadProfile(m_pProfile) == false)
			{
				throw new FileNotFoundException();
			}

		}

		public readonly string	m_pProfile = null;

		protected bool	LoadProfile(string pProfile)
		{
			var pReader = new StreamReader(pProfile);

			using (pReader)
			{
				string	pLineBuffer = null;
				while ((pLineBuffer = pReader.ReadLine()) != null)
				{
					System.Diagnostics.Debug.WriteLine("LineBuffer: " + pLineBuffer);
					;
				}
			}



			return(false);
		}
	}
}
