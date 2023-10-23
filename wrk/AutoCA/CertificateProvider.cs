using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoCA
{
	public class CertificateProvider
	{
		//　署名要求を生成
		public static CertificateRequest CreateSignRequest(OrgProfile pOrgProfile, string pCommonName)
		{
			//　署名要求を生成（der形式）
			string pSubject = @"cn={pCommonName},";
			string pSubjectExm = "cn=Arteria-RCA";
			ECDsaCng pKey = new ECDsaCng();
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

			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
			/*
			var pBytes = pRequest.CreateSigningRequest();
			var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
*/
			return(pRequest);
		}

		//　自己署名証明書を作成
		public static X509Certificate2 CreateSelfCertificate(CertificateRequest pCertificateRequest, int iDays)
		{
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(iDays);
			return(pCertificateRequest.CreateSelfSigned(pNotBefore, pNotAfter));
		}


		//　ルート認証局の証明書を作成
		public static bool CreateRootCA(OrgProfile pOrgProfile, string pCommonName)
		{
			var pRequest = CreateSignRequest(pOrgProfile, pCommonName);
			if (pRequest == null)
			{
				return(false);
			}
			var iLifeDays = 365 * 10;
			var pCertificate = CreateSelfCertificate(pRequest, iLifeDays);
			if (pCertificate == null)
			{
				return (false);
			}

			return(true);
		}
	}
}
