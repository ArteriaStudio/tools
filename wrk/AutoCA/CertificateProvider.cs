using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Certificates;

namespace AutoCA
{
	//　証明書、署名要求のデータを作成する関数群（関数を跨いだ状態を持たない）
	public static class CertificateProvider
	{
		//　署名要求を生成
		public static CertificateRequest CreateSignRequest(ECDsaCng pKeys, OrgProfile pOrgProfile, string pCommonName)
		{
			string pSubject = $"C={pOrgProfile.CountryName},L={pOrgProfile.LocalityName},O={pOrgProfile.OrgName},CN={pCommonName},";

			//　署名要求を生成（der形式）
			CertificateRequest pRequest = new CertificateRequest(pSubject, pKeys, HashAlgorithmName.SHA256);

			//　CA制約：証明書が認証局であるか否かを指定する。
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
			pRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(pRequest.PublicKey, false));
			var pKeyUsageFlags = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.EncipherOnly;
			pRequest.CertificateExtensions.Add(new X509KeyUsageExtension(pKeyUsageFlags, false));
			OidCollection pEnhancedKeyUsageFlags = new OidCollection
			{
				// https://oidref.com/1.3.6.1.5.5.7.3.2
				new Oid("1.3.6.1.5.5.7.3.1"),   //　serverAuth
				new Oid("1.3.6.1.5.5.7.3.3"),	//　codeSigning
				new Oid("1.3.6.1.5.5.7.3.4"),	//　emailProtection
			};
			//pRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(pEnhancedKeyUsageFlags, false));

			/*
			var pTextOfKey = pKey.ExportPkcs8PrivateKey();
			var pText = Convert.ToBase64String(pTextOfKey);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText);

			var pBytesOfKey = pKey.ExportECPrivateKey();
			File.WriteAllBytes("D:/tmp/256.txt", pBytesOfKey);
					
			var pText2 = Convert.ToBase64String(pBytesOfKey);
			Debug.WriteLine("PrivateKey():" + pText2);
			*/

			/*
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
			var pBytes = pRequest.CreateSigningRequest();
			var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
			*/

			return (pRequest);
		}

		//　署名要求（メール証明書）を生成
		public static CertificateRequest CreateSignRequestForClient(ECDsaCng pKeys, OrgProfile pOrgProfile, string pCommonName, string pEmail)
		{
			string pSubject = $"C={pOrgProfile.CountryName},L={pOrgProfile.LocalityName},O={pOrgProfile.OrgName},CN={pCommonName},";
			
			//　署名要求を生成（der形式）
			CertificateRequest pRequest = new CertificateRequest(pSubject, pKeys, HashAlgorithmName.SHA256);

			//　CA制約：証明書が認証局であるか否かを指定する。
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
			pRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(pRequest.PublicKey, false));
			var pKeyUsageFlags = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.EncipherOnly;
			pRequest.CertificateExtensions.Add(new X509KeyUsageExtension(pKeyUsageFlags, false));
			OidCollection pEnhancedKeyUsageFlags = new OidCollection
			{
				// https://oidref.com/1.3.6.1.5.5.7.3.2
				new Oid("1.3.6.1.5.5.7.3.4"),	//　emailProtection
			};
			pRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(pEnhancedKeyUsageFlags, false));

			var pBuilder = new SubjectAlternativeNameBuilder();
			pBuilder.AddEmailAddress(pEmail);
			var pExtBuilt = pBuilder.Build(true);
			pRequest.CertificateExtensions.Add(new X509SubjectAlternativeNameExtension(pExtBuilt.RawData));

			/*
			var pTextOfKey = pKey.ExportPkcs8PrivateKey();
			var pText = Convert.ToBase64String(pTextOfKey);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText);

			var pBytesOfKey = pKey.ExportECPrivateKey();
			File.WriteAllBytes("D:/tmp/256.txt", pBytesOfKey);
					
			var pText2 = Convert.ToBase64String(pBytesOfKey);
			Debug.WriteLine("PrivateKey():" + pText2);
			*/

			/*
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
			var pBytes = pRequest.CreateSigningRequest();
			var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
			*/

			return (pRequest);
		}

		//　署名要求（サーバ証明書）を生成
		public static CertificateRequest CreateSignRequestForServer(ECDsaCng pKeys, OrgProfile pOrgProfile, string pCommonName, string pDnsName)
		{
			string pSubject = $"C={pOrgProfile.CountryName},L={pOrgProfile.LocalityName},O={pOrgProfile.OrgName},CN={pCommonName},";

			//　署名要求を生成（der形式）
			CertificateRequest pRequest = new CertificateRequest(pSubject, pKeys, HashAlgorithmName.SHA256);

			//　CA制約：証明書が認証局であるか否かを指定する。
			pRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 2, true));
			pRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(pRequest.PublicKey, false));
			var pKeyUsageFlags = X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.EncipherOnly;
			pRequest.CertificateExtensions.Add(new X509KeyUsageExtension(pKeyUsageFlags, false));
			OidCollection pEnhancedKeyUsageFlags = new OidCollection
			{
				// https://oidref.com/1.3.6.1.5.5.7.3.2
				new Oid("1.3.6.1.5.5.7.3.1"),   //　serverAuth
			};
			pRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(pEnhancedKeyUsageFlags, false));

			var pBuilder = new SubjectAlternativeNameBuilder();
			pBuilder.AddDnsName(pDnsName);
			var pExtBuilt = pBuilder.Build(true);
			pRequest.CertificateExtensions.Add(new X509SubjectAlternativeNameExtension(pExtBuilt.RawData));

			/*
			var pTextOfKey = pKey.ExportPkcs8PrivateKey();
			var pText = Convert.ToBase64String(pTextOfKey);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText);

			var pBytesOfKey = pKey.ExportECPrivateKey();
			File.WriteAllBytes("D:/tmp/256.txt", pBytesOfKey);
					
			var pText2 = Convert.ToBase64String(pBytesOfKey);
			Debug.WriteLine("PrivateKey():" + pText2);
			*/

			/*
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(365);
			var pBytes = pRequest.CreateSigningRequest();
			var pRootCert = pRequest.CreateSelfSigned(pNotBefore, pNotAfter);
			*/

			return (pRequest);
		}

		//　自己署名証明書を作成
		public static X509Certificate2	CreateSelfCertificate(CertificateRequest pRequest, string CommonName, int iLifeDays, string pServerName)
		{
			//　機関情報アクセスを署名要求に追加
			List<string> pOCSP = new List<string>();
			List<string> pICAs = new List<string>();
			pICAs.Add("http://" + pServerName + "/" + CommonName + ".crt");
			pRequest.CertificateExtensions.Add(new X509AuthorityInformationAccessExtension(pOCSP, pICAs, false));

			//　失効リスト配布ポイントを署名要求に追加
			List<string> pCDP = new List<string>();
			pCDP.Add("http://" + pServerName + "/" + CommonName + ".crl");
			var pCDPExtension = CertificateRevocationListBuilder.BuildCrlDistributionPointExtension(pCDP, false);
			pRequest.CertificateExtensions.Add(pCDPExtension);


			//　認証局識別子を署名要求に追加。発行認証局の「鍵識別子（SKI）」
			X509SubjectKeyIdentifierExtension	pSKI = null;
			foreach (var pExtension in pRequest.CertificateExtensions)
			{
				//　拡張情報からSKIを探索し、そのキーをAKIに転写
				if (pExtension.Oid.Value == "2.5.29.14")
				{
					pSKI = (X509SubjectKeyIdentifierExtension)pExtension;
					break;
				}
			}
			if (pSKI != null)
			{
				var pAKI = X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(pSKI);
				pRequest.CertificateExtensions.Add(pAKI);
			}

			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(iLifeDays);
			return(pRequest.CreateSelfSigned(pNotBefore, pNotAfter));
		}

#if (false)
		//　ルート認証局の証明書を作成
		public static X509Certificate2	CreateRootCA(ECDsaCng pKeys, OrgProfile pOrgProfile, string pCommonName, int iLifeDays)
		{
			//　署名要求を生成
			var pRequest = CreateSignRequest(pKeys, pOrgProfile, pCommonName);
			if (pRequest == null)
			{
				return(null);
			}
			//　デバッグ用：署名要求のバイト列を生成
			//var pBytes = pRequest.CreateSigningRequest();

			var pCertificate = CreateSelfCertificate(pRequest, iLifeDays);
			if (pCertificate == null)
			{
				return (null);
			}
			/*
			var pPrivateKey = pCertificate.GetECDsaPrivateKey();
			var pPublicKey = pPrivateKey.ExportSubjectPublicKeyInfo();

			Debug.WriteLine("PrivateKey: "+Convert.ToBase64String(pPrivateKey.ExportECPrivateKey()));
			Debug.WriteLine("PublicKey: "+Convert.ToBase64String(pPublicKey));
			*/
			/*
			//　
			var pCertificateItem = new Certificate();
			pCertificateItem.SequenceNumber = 0;
			pCertificateItem.SerialNumber   = pCertificate.SerialNumber;
			pCertificateItem.CommonName     = pCertificate.SubjectName.Name;
			pCertificateItem.Revoked        = false;
			pCertificateItem.PemData        = pCertificate.ExportCertificatePem();
			pCertificateItem.pCertificate   = pCertificate;
			*/

			return (pCertificate);
		}
#endif
		//　証明要求に署名を行い、証明書を作成する。
		public static X509Certificate2 CreateCertificate(CertificateRequest pRequest, Certificate pTrustCrt, int iLifeDays, string pServerName)
		{
			//　署名要求に認証局の機関情報アクセスと失効リストのURLを追加する。
			//pTrustCrt.m_pCertificate.Extensions;
			/*
			var pBuilder = new SubjectAlternativeNameBuilder();
			pBuilder.AddDnsName(pDnsName);
			var pExtBuilt = pBuilder.Build(true);
			*/
			//X509Extension	pExtension = new X509Extension()

			/*
			var pExtension = new X509AuthorityInformationAccessExtension();
			//pExtensions;
			pExtension.Oid.Value = "1.3.6.1.5.5.7.1.1";
			*/



			//　機関情報アクセスを署名要求に追加
			List<string> pOCSP = new List<string>();
			List<string> pICAs = new List<string>();
			pICAs.Add("http://" + pServerName + "/" + pTrustCrt.CommonName + ".crt");
			pRequest.CertificateExtensions.Add(new X509AuthorityInformationAccessExtension(pOCSP, pICAs, false));

			//　失効リスト配布ポイントを署名要求に追加
			List<string> pCDP = new List<string>();
			pCDP.Add("http://" + pServerName + "/" + pTrustCrt.CommonName + ".crl");
			var pCDPExtension = CertificateRevocationListBuilder.BuildCrlDistributionPointExtension(pCDP, false);
			pRequest.CertificateExtensions.Add(pCDPExtension);

			//　認証局識別子を署名要求に追加。発行認証局の「鍵識別子（SKI）」
			foreach (var pExtension in pTrustCrt.m_pCertificate.Extensions)
			{
				//　拡張情報からSKIを探索し、そのキーをAKIに転写
				if (pExtension.Oid.Value == "2.5.29.14")
				{
					var pSKI = (X509SubjectKeyIdentifierExtension)pExtension;
					var pAKI = X509AuthorityKeyIdentifierExtension.CreateFromSubjectKeyIdentifier(pSKI);
					pRequest.CertificateExtensions.Add(pAKI);
					break;
				}
			}

			var pNotBefore    = DateTimeOffset.UtcNow;
			var pNotAfter     = DateTimeOffset.UtcNow.AddDays(iLifeDays);
			//var pSerialNumber = BitConverter.GetBytes(uSerialNumber);

			//　※ 自己署名証明書の生成時に使っているロジック（ユニーク性は保証されていない）
			Span<byte> pSerialNumber = stackalloc byte[8];
			RandomNumberGenerator.Fill(pSerialNumber);

			return (pRequest.Create(pTrustCrt.m_pCertificate, pNotBefore, pNotAfter, pSerialNumber));
		}
	}
}
