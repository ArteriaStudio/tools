using Arteria_s.DB.Base;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.OnlineId;

namespace AutoCA
{
	public class CertificateItem
	{
		public CertificateItem()
		{
			SequenceNumber = -1;
			SerialNumber = "";
			CommonName = "";
			CA = false;
			Revoked = false;
			PemData = "";
		}

		public long SequenceNumber { get; set; }
		public string SerialNumber { get; set; }
		public string CommonName { get; set; }
		public bool CA { get; set; }
		public bool Revoked { get; set; }
		public DateTime	LaunchAt { get; set; }
		public DateTime ExpireAt { get; set; }
		public string PemData { get; set; }

		//　共通名が一致する証明書を入力
		public bool Load(SQLContext pSQLContext, string pCommonName)
		{
			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, CA, Revoked, LaunchAt, ExpireAt, PemData FROM TIssuedCerts WHERE CommonName = @CommonName AND Revoked = FALSE AND LaunchAt <= now() AND now() < ExpireAt;";
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
						iCount ++;
					}
					if (iCount == 0)
					{
						return(false);
					}
				}
			}

			return (true);
		}

		public bool CreateCA(OrgProfile pOrgProfile, string pCommonName, CertificateItem pCACertificate)
		{
			if (pCACertificate == null)
			{
				CertificateProvider.CreateRootCA(pOrgProfile, pCommonName);
			}

			return (true);
		}
	}
}
