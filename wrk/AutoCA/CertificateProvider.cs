using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	//　証明書、署名要求のデータを作成する関数群（関数を跨いだ状態を持たない）
	public static class CertificateProvider
	{
		//　署名要求を生成
		public static CertificateRequest CreateSignRequest(OrgProfile pOrgProfile, string pCommonName)
		{
			string pSubject = $"cn={pCommonName},";
			//string pSubjectExm = "cn=Arteria-RCA";
			
			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKey = new ECDsaCng();
			var pTextOfKey1 = pKey.ExportPkcs8PrivateKey();
			var pText1 = Convert.ToBase64String(pTextOfKey1);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText1);

			//　署名要求を生成（der形式）
			CertificateRequest pRequest = new CertificateRequest(pSubject, pKey, HashAlgorithmName.SHA256);
			//　CA制約：証明書が認証局であるか否かを指定する。
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
			pRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(pRequest.PublicKey, false));
			var pKeyUsageFlags = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.EncipherOnly;
			pRequest.CertificateExtensions.Add(new X509KeyUsageExtension(pKeyUsageFlags, false));
			OidCollection pEnhancedKeyUsageFlags = new OidCollection
			{
				// https://oidref.com/1.3.6.1.5.5.7.3.2
				new Oid("1.3.6.1.5.5.7.3.1"),   //　serverAuth
				new Oid("1.3.6.1.5.5.7.3.3")    //　codeSigning
			};
			pRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(pEnhancedKeyUsageFlags, false));

			var pTextOfKey = pKey.ExportPkcs8PrivateKey();
			var pText = Convert.ToBase64String(pTextOfKey);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText);

			var pBytesOfKey = pKey.ExportECPrivateKey();
			File.WriteAllBytes("D:/tmp/256.txt", pBytesOfKey);
					
			var pText2 = Convert.ToBase64String(pBytesOfKey);
			Debug.WriteLine("PrivateKey():" + pText2);

			/*
				var pNotBefore = DateTimeOffset.UtcNow;
				var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
				var pBytes = pRequest.CreateSigningRequest();
				var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
			*/
			return (pRequest);
		}

		//　自己署名証明書を作成
		public static X509Certificate2 CreateSelfCertificate(CertificateRequest pCertificateRequest, int iDays)
		{
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(iDays);
			return(pCertificateRequest.CreateSelfSigned(pNotBefore, pNotAfter));
		}


		//　ルート認証局の証明書を作成
		public static CertificateItem CreateRootCA(OrgProfile pOrgProfile, string pCommonName)
		{
			var pRequest = CreateSignRequest(pOrgProfile, pCommonName);
			if (pRequest == null)
			{
				return(null);
			}

			//　デバッグ用：署名要求のバイト列を生成
			var pBytes = pRequest.CreateSigningRequest();
			Debug.WriteLine(pBytes);

			var iLifeDays = 365 * 10;
			var pCertificate = CreateSelfCertificate(pRequest, iLifeDays);
			if (pCertificate == null)
			{
				return (null);
			}
			var pPrivateKey = pCertificate.GetECDsaPrivateKey();
			var pPublicKey = pPrivateKey.ExportSubjectPublicKeyInfo();

			Debug.WriteLine("PrivateKey: "+Convert.ToBase64String(pPrivateKey.ExportECPrivateKey()));
			Debug.WriteLine("PublicKey: "+Convert.ToBase64String(pPublicKey));

			var pCertificateItem = new CertificateItem();
			pCertificateItem.SequenceNumber= 0;
			pCertificateItem.SerialNumber = pCertificate.SerialNumber;
			pCertificateItem.CommonName = pCertificate.SubjectName.Name;
			pCertificateItem.Revoked = 0;
			pCertificateItem.PemData = pCertificate.ExportCertificatePem();





			return (pCertificateItem);
		}
	}
}
