﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
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
		public static X509Certificate2	CreateSelfCertificate(CertificateRequest pCertificateRequest, int iLifeDays)
		{
			var pNotBefore = DateTimeOffset.UtcNow;
			var pNotAfter = DateTimeOffset.UtcNow.AddDays(iLifeDays);
			return(pCertificateRequest.CreateSelfSigned(pNotBefore, pNotAfter));
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
		public static X509Certificate2 CreateCertificate(CertificateRequest pRequest, Certificate pTrustCrt, Int64 uSerialNumber, int iLifeDays)
		{
			var pNotBefore    = DateTimeOffset.UtcNow;
			var pNotAfter     = DateTimeOffset.UtcNow.AddDays(iLifeDays);
			var pSerialNumber = BitConverter.GetBytes(uSerialNumber);
			return (pRequest.Create(pTrustCrt.m_pCertificate, pNotBefore, pNotAfter, pSerialNumber));
		}
	}
}
