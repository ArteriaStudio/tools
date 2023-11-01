using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
    public class ClientCert : Certificate
	{
		public string MailAdrs { get; set; }

		public ClientCert() : base()
		{
			MailAdrs = "";
		}
		public void Create()
		{
			return;
		}
	}
}
