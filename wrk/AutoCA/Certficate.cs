﻿using Arteria_s.DB.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.OnlineId;
using Windows.Security.Cryptography.Certificates;
using Windows.System;

namespace AutoCA
{
	public enum CertificateType
	{
		Unknown,
		CA,
		Server,
		Client,
	}

	public class Certificate : IEquatable<Certificate>
	{
		public Certificate()
		{
			SequenceNumber = -1;
			SerialNumber   = "";
			SubjectName    = "";
			CommonName     = "";
			TypeOf         = 0;
			Revoked        = false;
			PemData        = "";
			KeyData        = "";
		}

		public long 				SequenceNumber { get; set; }
		public string				SerialNumber { get; set; }
		public string				SubjectName { get; set; }
		public string				CommonName { get; set; }
		public CertificateType		TypeOf { get; set; }
		public bool 				Revoked { get; set; }
		public DateTime 			LaunchAt { get; set; }
		public DateTime 			ExpireAt { get; set; }
		public string				PemData { get; set; }
		public string				KeyData { get; set; }
		public X509Certificate2		m_pCertificate { get; set; }

		//　共通名が一致する証明書を入力
		public bool Load(SQLContext pSQLContext, string pCommonName)
		{
			var pSQL = "SELECT SequenceNumber, SerialNumber, SubjectName, CommonName, TypeOf, Revoked, LaunchAt, ExpireAt, PemData, KeyData FROM TIssuedCerts WHERE CommonName = @CommonName AND Revoked = FALSE AND LaunchAt <= now() AND now() < ExpireAt;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("CommonName", pCommonName);
				using (var pReader = pCommand.ExecuteReader())
				{
					int iCount = 0;
					while (pReader.Read())
					{
						SequenceNumber = pReader.GetInt64(0);
						SerialNumber   = pReader.GetString(1);
						SubjectName    = pReader.GetString(2);
						CommonName     = pReader.GetString(3);
						TypeOf         = (CertificateType)pReader.GetInt32(4);
						Revoked        = pReader.GetBoolean(5);
						LaunchAt       = pReader.GetDateTime(6);
						ExpireAt       = pReader.GetDateTime(7);
						PemData        = pReader.GetString(8);
						KeyData		   = pReader.GetString(9);

						if ((KeyData != null) && (KeyData.Length > 0))
						{
							m_pCertificate = X509Certificate2.CreateFromPem(PemData, KeyData);
						}
						else
						{
							m_pCertificate = X509Certificate2.CreateFromPem(PemData);
						}
						iCount++;
					}
					if (iCount == 0)
					{
						return(false);
					}
				}
			}

			return (true);
		}

		//　
		public void Prepare()
		{
			if ((KeyData != null) && (KeyData.Length > 0))
			{
				m_pCertificate = X509Certificate2.CreateFromPem(PemData, KeyData);
			}
			else
			{
				m_pCertificate = X509Certificate2.CreateFromPem(PemData);
			}

			return;
		}

		//　
		public void LoadBridge(NpgsqlDataReader pReader)
		{
			SequenceNumber = pReader.GetInt64(0);
			SerialNumber   = pReader.GetString(1);
			SubjectName    = pReader.GetString(2);
			CommonName     = pReader.GetString(3);
			TypeOf         = (CertificateType)pReader.GetInt32(4);
			Revoked        = pReader.GetBoolean(5);
			LaunchAt       = pReader.GetDateTime(6);
			ExpireAt       = pReader.GetDateTime(7);
			PemData        = pReader.GetString(8);
			KeyData        = pReader.GetString(9);

			if ((KeyData != null) && (KeyData.Length > 0))
			{
				m_pCertificate = X509Certificate2.CreateFromPem(PemData, KeyData);
			}
			else
			{
				m_pCertificate = X509Certificate2.CreateFromPem(PemData);
			}

			return;
		}

		//　
		public bool Save(SQLContext pSQLContext)
		{
			var pSQL = "INSERT INTO TIssuedCerts (SequenceNumber, SerialNumber, SubjectName, CommonName, TypeOf, Revoked, LaunchAt, ExpireAt, PemData, KeyData)";
			pSQL += " VALUES (NEXTVAL('SQ_CERTS'), @SerialNumber, @SubjectName, @CommonName, @TypeOf, @Revoked, @LaunchAt, @ExpireAt, @PemData, @KeyData)";
			pSQL += " ON CONFLICT ON CONSTRAINT tissuedcerts_pkey DO UPDATE SET";
			pSQL += " SerialNumber = @SerialNumber, SubjectName = @SubjectName, CommonName = @CommonName, TypeOf = @TypeOf, Revoked = @Revoked,";
			pSQL += " LaunchAt = @LaunchAt, ExpireAt = @ExpireAt, PemData = @PemData, KeyData = @KeyData;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SequenceNumber", SequenceNumber);
				pCommand.Parameters.AddWithValue("SerialNumber", SerialNumber);
				pCommand.Parameters.AddWithValue("SubjectName", SubjectName);
				pCommand.Parameters.AddWithValue("CommonName", CommonName);
				pCommand.Parameters.AddWithValue("TypeOf", (int)TypeOf);
				pCommand.Parameters.AddWithValue("Revoked", Revoked);
				pCommand.Parameters.AddWithValue("LaunchAt", LaunchAt);
				pCommand.Parameters.AddWithValue("ExpireAt", ExpireAt);
				pCommand.Parameters.AddWithValue("PemData", PemData);
				pCommand.Parameters.AddWithValue("KeyData", KeyData);
				pCommand.ExecuteNonQuery();
			}

			return (true);
		}

		//　認証局証明書を生成
		//　
		//　pCommonName：
		//　pCACertificate：署名する認証局の証明書データ
		public bool CreateForAuthority(OrgProfile pOrgProfile, string pCommonName, Certificate pCACertificate)
		{
			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKeys = new ECDsaCng();

			if (pCACertificate == null)
			{
				//　ルート認証局の署名要求を生成
				var pRequest = CertificateProvider.CreateSignRequest(pKeys, pOrgProfile, pCommonName);
				if (pRequest == null)
				{
					return (false);
				}
				//　デバッグ用：署名要求のバイト列を生成
				//var pBytes = pRequest.CreateSigningRequest();

				var iLifeDays = 365 * 10;
				var pCertificate = CertificateProvider.CreateSelfCertificate(pRequest, pCommonName, iLifeDays, pOrgProfile.ServerName);
				if (pCertificate == null)
				{
					return (false);
				}
				//　自己署名証明書を生成した時は、秘密鍵が証明書オブジェクトに含まれている。

				//　証明書記載情報の主要なものをキャッシュ
				FetchProperties(pCertificate, pKeys, pCommonName, CertificateType.CA);
			}
			else
			{
				//　発行認証局の署名要求を生成
				var pRequest = CertificateProvider.CreateSignRequest(pKeys, pOrgProfile, pCommonName);
				if (pRequest == null)
				{
					return(false);
				}

				//　ルート認証局の証明書データで署名した発行認証局の証明書を生成
				var iLifeDays = 365 * 5;
				var pCertificate = CertificateProvider.CreateCertificate(pRequest, pCACertificate, iLifeDays, pOrgProfile.ServerName);
				if (pCertificate == null)
				{
					return (false);
				}
				//　通常のシーケンスだと証明書オブジェクトに鍵ペアは含まれない。

				//　証明書記載情報の主要なものをキャッシュ
				FetchProperties(pCertificate, pKeys, pCommonName, CertificateType.CA);
			}

			return (true);
		}

		//　サーバ証明書を生成
		public bool CreateForServer(OrgProfile pOrgProfile, string pCommonName, string pFQDN, Certificate pCACertificate)
		{
			//　認証局の証明書データが指定されていない時はエラー
			if (pCACertificate == null)
			{
				return (false);
			}

			//　証明書のサブジェクト名を生成
			string pSubjectName = GenerateSubjectName(pOrgProfile, pCommonName);

			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKeys = new ECDsaCng();

			//　サーバ証明書用の署名要求を生成
			var pRequest = CertificateProvider.CreateSignRequestForServer(pKeys, pOrgProfile, pSubjectName, pCommonName, pFQDN);
			if (pRequest == null)
			{
				return (false);
			}
			//　指定された認証局の証明書で署名
			var iLifeDays = 365 * 1;
			var pCertificate = CertificateProvider.CreateCertificate(pRequest, pCACertificate, iLifeDays, pOrgProfile.ServerName);
			if (pCertificate == null)
			{
				return (false);
			}

			//　証明書記載情報の主要なものをキャッシュ
			FetchProperties(pCertificate, pKeys, pCommonName, CertificateType.Server);

			return (true);
		}

		//　メール証明書を生成
		public bool CreateForClient(OrgProfile pOrgProfile, string pCommonName, string pMailAddress, Certificate pCACertificate)
		{
			//　認証局の証明書データが指定されていない時はエラー
			if (pCACertificate == null)
			{
				return (false);
			}

			//　証明書のサブジェクト名を生成
			string pSubjectName = GenerateSubjectName(pOrgProfile, pCommonName);

			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKeys = new ECDsaCng();

			//　メール証明書用の署名要求を生成
			var pRequest = CertificateProvider.CreateSignRequestForClient(pKeys, pOrgProfile, pSubjectName, pCommonName, pMailAddress);
			if (pRequest == null)
			{
				return (false);
			}
			//　指定された認証局の証明書で署名
			var iLifeDays = 365 * 1;
			var pCertificate = CertificateProvider.CreateCertificate(pRequest, pCACertificate, iLifeDays, pOrgProfile.ServerName);
			if (pCertificate == null)
			{
				return (false);
			}

			//　証明書記載情報の主要なものをキャッシュ
			FetchProperties(pCertificate, pKeys, pCommonName, CertificateType.Client);

			return (true);
		}

		//　指定された証明書データに基づいて有効期限を延長した証明書を作成する。
		//　for Updateとあるが、Oracleの行ロックとは関係ない。
		//　pBaseCertificate：転記元の証明書
		public bool CreateForUpdate(OrgProfile pOrgProfile, Certificate pBaseCertificate, Certificate pCACertificate)
		{
			//　認証局の証明書データが指定されていない時はエラー
			if ((pCACertificate == null) || (pCACertificate.m_pCertificate == null))
			{
				return (false);
			}
			//　継承元の証明書が指定されていない時もエラー
			if ((pBaseCertificate == null) || (pBaseCertificate.m_pCertificate == null))
			{
				return (false);
			}

			//　秘密鍵を生成（楕円曲線方式）
			ECDsaCng pKeys = new ECDsaCng();

			//　メール証明書用の署名要求を生成
			var pRequest = CertificateProvider.CreateSignRequestForUpdate(pKeys, pOrgProfile, pBaseCertificate.m_pCertificate);
			if (pRequest == null)
			{
				return (false);
			}
			//　指定された認証局の証明書で署名
			var iLifeDays = 365 * 1;
			var pCertificate = CertificateProvider.CreateCertificate(pRequest, pCACertificate, iLifeDays, pOrgProfile.ServerName);
			if (pCertificate == null)
			{
				return (false);
			}

			//　証明書記載情報の主要なものをキャッシュ
			FetchProperties(pCertificate, pKeys, pBaseCertificate.CommonName, pBaseCertificate.TypeOf);

			return (true);
		}

		//　証明書を失効
		public bool Revoke(SQLContext pSQLContext)
		{
			var pSQL = "UPDATE TIssuedCerts SET Revoked = @Revoked, RevokeAt = now() WHERE SerialNumber = @SerialNumber";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				Revoked = true;

				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SerialNumber", SerialNumber);
				pCommand.Parameters.AddWithValue("Revoked", Revoked);
				pCommand.ExecuteNonQuery();
			}

			return (true);
		}

		//　証明書データの有効性を検査
		public bool	Validate(SQLContext pSQLContext)
		{
			if (SequenceNumber == -1)
			{
				return(false);
			}
			//　認証局が発行した有効な証明書の中にサブジェクト名の重複がないことを検査
			if (IsExistSubject(pSQLContext, SerialNumber, SubjectName) == true)
			{
				return (false);
			}

			return(true);
		}

		//　X509証明書データとECDsa鍵データをオブジェクトに入力
		private void FetchProperties(X509Certificate2 pCertificate, ECDsaCng pKeys, string pCommonName, CertificateType eTypeOf)
		{
			SequenceNumber = 0;
			SerialNumber   = pCertificate.SerialNumber;
			SubjectName    = pCertificate.SubjectName.Name;
			CommonName     = pCommonName;
			TypeOf         = eTypeOf;
			Revoked        = false;
			LaunchAt       = pCertificate.NotBefore;
			ExpireAt       = pCertificate.NotAfter;
			PemData        = pCertificate.ExportCertificatePem();
			KeyData        = pKeys.ExportECPrivateKeyPem();

			m_pCertificate = X509Certificate2.CreateFromPem(PemData, KeyData);
		}

		//　指定したフォルダに証明書と鍵をファイル形式で出力
		public string Export(string pExportFolder)
		{
			var pBytesOfKey = m_pCertificate.GetECDsaPrivateKey().ExportECPrivateKey();
			var pExportFilepathOfKey = pExportFolder + "\\" + CommonName + ".key";
			File.WriteAllBytes(pExportFilepathOfKey, pBytesOfKey);

			var pBytesOfCrt = m_pCertificate.Export(X509ContentType.Cert);
			var pExportFilepathOfCrt = pExportFolder + "\\" + CommonName + ".crt";
			File.WriteAllBytes(pExportFilepathOfCrt, pBytesOfCrt);

			return (pExportFilepathOfCrt);
		}

		//　秘密鍵と証明書をファイルに出力
		protected void DumpToFile(ECDsaCng pKeys)
		{
			var pBytesOfKey = pKeys.ExportECPrivateKey();
			File.WriteAllBytes("D:/tmp/" + CommonName + ".key", pBytesOfKey);
			var pBytesOfCrt = m_pCertificate.Export(X509ContentType.Cert);
			File.WriteAllBytes("D:/tmp/" + CommonName + ".crt", pBytesOfCrt);
		}

		//　証明書データに収められた秘密鍵を比較（動作検証用）
		protected void Dump()
		{
			var pKey = m_pCertificate.GetECDsaPrivateKey();
			if (pKey == null)
			{
				;
			}
			else
			{
				var pRoot = pKey.ExportECPrivateKeyPem();
				if (pRoot == KeyData)
				{
					//　ルート証明書を自己署名した証明書データは、このケースに来る。
					//　つまり、X509オブジェクトのインスタンスに何故か秘密鍵が含まれている。
					Debug.WriteLine("Match");
				}
				else
				{
					Debug.WriteLine("UnMatch");
				}
			}
		}

		//　サブジェクト名を生成
		protected string GenerateSubjectName(OrgProfile pOrgProfile, string pCommonName)
		{
			return($"C={pOrgProfile.CountryName},L={pOrgProfile.LocalityName},O={pOrgProfile.OrgName},CN={pCommonName}");
		}

		//　有効な証明祖の中に同一のサブジェクト名を持つ要素が存在するか検査
		protected bool IsExistSubject(SQLContext pSQLContext, string pSerialNumber, string pSubjectName)
		{
			var pSQL = "SELECT SequenceNumber, SerialNumber, SubjectName FROM TIssuedCerts WHERE SerialNumber <> @SerialNumber AND SubjectName = @SubjectName AND Revoked = FALSE AND LaunchAt <= now() AND now() < ExpireAt;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SerialNumber", pSerialNumber);
				pCommand.Parameters.AddWithValue("SubjectName", pSubjectName);
				using (var pReader = pCommand.ExecuteReader())
				{
					int iCount = 0;
					while (pReader.Read())
					{
						iCount++;
					}
					if (iCount == 0)
					{
						return(false);
					}
				}
			}

			return (true);
		}

		//　鍵をダンプ（デバッグ用）
		protected void Dump(ECDsaCng pKeys)
		{
			//　秘密鍵をテキストに変換
			var pTextOfKey1 = pKeys.ExportPkcs8PrivateKey();
			var pText1 = Convert.ToBase64String(pTextOfKey1);
			Debug.WriteLine("PrivateKey(PKCS#8):" + pText1);
		}

		public bool Equals(Certificate pOther)
		{
			if (pOther.SerialNumber != SerialNumber)
			{
				return false;
			}

			return(true);
		}
	}
}
