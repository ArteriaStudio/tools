using Ritters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class ProfileData
	{
		//　プロパティ
		public string	DatabaseServer { get; set; }
		public string	CampusNote { get; set; }

		public ProfileData()
		{
			//　データベースサーバー名
			DatabaseServer = "staging";
			CampusNote = "1";
		}
	}
}
