using Arteria_s.DB.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.OnlineId;
using Windows.System;

namespace AutoCA
{
	public class CertificateItem
	{
		public CertificateItem()
		{
			SequenceNumber = -1;
			SerialNumber   = "";
			CommonName     = "";
			CA             = false;
			Revoked        = false;
			PemData        = "";
			KeyData        = "";
		}

		public long 				SequenceNumber { get; set; }
		public string				SerialNumber { get; set; }
		public string				CommonName { get; set; }
		public bool 				CA { get; set; }
		public bool 				Revoked { get; set; }
		public DateTime 			LaunchAt { get; set; }
		public DateTime 			ExpireAt { get; set; }
		public string				PemData { get; set; }
		public string				KeyData { get; set; }
		public X509Certificate2		m_pCertificate { get; set; }

		//　共通名が一致する証明書を入力
		public bool Load(SQLContext pSQLContext, string pCommonName)
		{
			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, CA, Revoked, LaunchAt, ExpireAt, PemData, KeyData FROM TIssuedCerts WHERE CommonName = @CommonName AND Revoked = FALSE AND LaunchAt <= now() AND now() < ExpireAt;";
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
						CommonName     = pReader.GetString(2);
						CA             = pReader.GetBoolean(3);
						Revoked        = pReader.GetBoolean(4);
						LaunchAt       = pReader.GetDateTime(5);
						ExpireAt       = pReader.GetDateTime(6);
						PemData        = pReader.GetString(7);
						KeyData		   = pReader.GetString(8);

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
		public bool Save(SQLContext pSQLContext)
		{
			var pSQL = "INSERT INTO TIssuedCerts VALUES (NEXTVAL('SQ_CERTS'), @SerialNumber, @CommonName, @CA, @Revoked, @LaunchAt, @ExpireAt, @PemData, @KeyData)";
			pSQL += " ON CONFLICT ON CONSTRAINT tissuedcerts_pkey DO UPDATE SET SerialNumber = @SerialNumber, CommonName = @CommonName, CA  = @CA, Revoked = @Revoked, LaunchAt = @LaunchAt , ExpireAt = @ExpireAt, PemData = @PemData, KeyData = @KeyData;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SequenceNumber", SequenceNumber);
				pCommand.Parameters.AddWithValue("SerialNumber", SerialNumber);
				pCommand.Parameters.AddWithValue("CommonName", CommonName);
				pCommand.Parameters.AddWithValue("CA", CA);
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
		public bool CreateCA(OrgProfile pOrgProfile, string pCommonName, CertificateItem pCACertificate)
		{
			if (pCACertificate == null)
			{
				//　ルート認証局の証明書と秘密鍵を生成
				var iLifeDays = 365 * 10;
				m_pCertificate = CertificateProvider.CreateRootCA(pOrgProfile, pCommonName, iLifeDays);
				if (m_pCertificate == null)
				{
					return(false);
				}
			}
			else
			{
				//　認証局の証明書をルート認証局が署名

				//　認証局から署名要求を生成
				var pRequest = CertificateProvider.CreateSignRequest(pOrgProfile, pCommonName);
				if (pRequest == null)
				{
					return(false);
				}
				var uSerialNumber = 1;
				var iLifeDays = 365 * 5;
				m_pCertificate = CertificateProvider.CreateCertificate(pRequest, pCACertificate, uSerialNumber, iLifeDays);
				if (m_pCertificate == null)
				{
					return (false);
				}
			}

			//　証明書記載情報の主要なものをキャッシュ
			SequenceNumber = 0;
			SerialNumber   = m_pCertificate.SerialNumber;
			CommonName     = pCommonName;
			//CommonName     = m_pCertificate.SubjectName.Name;//　これは証明書のサブジェクト
			CA             = true;
			Revoked        = false;
			LaunchAt       = m_pCertificate.NotBefore;
			ExpireAt       = m_pCertificate.NotAfter;
			PemData        = m_pCertificate.ExportCertificatePem();
			var pKey = m_pCertificate.GetECDsaPrivateKey();
			if (pKey == null)
			{
				KeyData = "";
			}
			else
			{
				KeyData = pKey.ExportECPrivateKeyPem();
			}

			return (true);
		}

		public bool	Validate()
		{
			if (SequenceNumber == -1)
			{
				return(false);
			}
			return(true);
		}
	}
}
