using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	public class CertificateItem
	{
		public CertificateItem()
		{
			SequenceNumber = -1;
			SerialNumber = "";
			CommonName = "";
			Revoked = 0;
			PemData = "";
		}

		public int SequenceNumber { get; set; }
		public string SerialNumber { get; set; }
		public string CommonName { get; set; }
		public int Revoked { get; set; }
		public string PemData { get; set; }
	}
}
