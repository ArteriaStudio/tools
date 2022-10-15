using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.Common.LigareBook
{
	public class Profile
	{
		//　プロパティ
		public string	DatabaseServer { get; set; }
		public string	DatabaseName { get; set; }
		public string	SchemaName { get; set; }
		public string	CurrentPage { get; set; }

		public Profile()
		{
			//　データベースサーバー名
			DatabaseServer = "stagesv.bunri-hjs.ac.jp";
			DatabaseName = "sbook";
			SchemaName = "aploper";
			CurrentPage = "DashboardView";
		}
	}
}
