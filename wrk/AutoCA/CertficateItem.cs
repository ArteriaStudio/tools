using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	public class CertficateItem
	{
		public CertficateItem()
		{
			SerialNumber = -1;
			CommonName = "";
			Revoked = 0;
			PemData = "";
		}
		public int SerialNumber { get; set; }
		public string CommonName { get; set; }
		public int Revoked { get; set; }
		public string PemData { get; set; }
	}
}
