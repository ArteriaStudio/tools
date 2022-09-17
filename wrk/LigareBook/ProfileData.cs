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
		public string	DatabaseName { get; set; }
		public string	SchemaName { get; set; }
		public string	CurrentPage { get; set; }

		public ProfileData()
		{
			//　データベースサーバー名
			DatabaseServer = "stagesv.bunri-hjs.ac.jp";
			DatabaseName = "sbook";
			SchemaName = "aploper";
			CurrentPage = "DashboardView";
		}
	}
}
