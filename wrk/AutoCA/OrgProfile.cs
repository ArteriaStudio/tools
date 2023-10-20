using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutoCA
{
	//　組織プロファイル
	public class OrgProfile
	{
		public OrgProfile() { }
		public long OrgKey { get; set; }
		public string OrgName { get; set; }
		public string OrgUnitName { get; set; }
		public string LocalityName { get; set; }
		public string ProvinceName { get; set; }
		public string CountryName { get; set; }
	}
}
