using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
    public class ServerCert : Certificate
	{
		public string FQDN { get; set; }

		public ServerCert() : base()
		{
			FQDN = "";
		}

		public void Create(OrgProfile pOrgProfile, string pCommonName, string pFQDN)
		{
			CommonName = pCommonName;
			FQDN = pFQDN;

			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKeys = new ECDsaCng();
			/*
			 * 秘密鍵をテキストに変換
			var pTextOfKey1 = pKey.ExportPkcs8PrivateKey();
			var pText1 = Convert.ToBase64String(pTextOfKey1);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText1);
			*/

			var pRequest = CertificateProvider.CreateSignRequestForServer(pKeys, pOrgProfile, pCommonName, pFQDN);


			return;
		}
	}
}
