using Arteria_s.DB.Base;
using Microsoft.Data.Sqlite;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Authentication.OnlineId;
using Windows.System;

namespace AutoCA
{
	//　証明書データをコレクションするクラス
	//　認証局クラス
	public class Authority
	{
		private Authority()
		{
			;
		}

		public static Authority Instance { get; set; } = new Authority();

		public Certificate	pTrustCAItem;
		public Certificate	pIssueCAItem;

		public bool Validate()
		{
			if ((pTrustCAItem == null)|| (pIssueCAItem == null))
			{
				return (false);
			}
			if (pTrustCAItem.Validate() == false)
			{
				return(false);
			}
			if (pIssueCAItem.Validate() == false)
			{
				return (false);
			}
			return (true);
		}

		public Identity 	m_pIdentity;
		public OrgProfile	m_pOrgProfile;

		//　認証局の証明書と鍵を入力
		public bool Load(SQLContext pSQLContext)
		{
			var iUserIdentity = 0;
			m_pIdentity = new Identity();
			m_pIdentity.Load(pSQLContext, iUserIdentity);
			m_pOrgProfile = new OrgProfile();
			m_pOrgProfile.Load(pSQLContext, iUserIdentity);


			/*
			if (pDbParams.Validate() == false)
			{
				//　準正常：接続情報が未登録のため、ここでの処理なし
				return (true);
			}
			*/
			/*
			//　データベースインスタンスに接続
			m_pSQLContext = new SQLContext(pDbParams.HostName, pDbParams.InstanceName, pDbParams.SchemaName, pDbParams.ClientKey, pDbParams.ClientCrt, pDbParams.TrustCrt);
			*/

			//　認証局の主体者情報を入力
			/*
			var iUserIdentity = 0;
			var pIdentity = FetchIdentity(pSQLContext, iUserIdentity);
			m_pOrgProfile = FetchOrgProfile(pSQLContext, iUserIdentity);
			*/
			if (m_pIdentity.Validate() == true)
			{
				if (m_pOrgProfile.Validate() == true)
				{
					//　ルート認証局の証明書データを入力
					pTrustCAItem = new Certificate();
					var pTrustCAName = m_pIdentity.AuthorityName + "-RCA";
					if (pTrustCAItem.Load(pSQLContext, pTrustCAName) == false)
					{
						//　ルート認証局の証明書を作成
						if (pTrustCAItem.CreateCA(m_pOrgProfile, pTrustCAName, null) == false)
						{
							//　異常系：証明書の作成に失敗
							return(false);
						}
						if (pTrustCAItem.Save(pSQLContext) == false)
						{
							return (false);
						}
					}

					//　発行認証局の証明書データを入力
					pIssueCAItem = new Certificate();
					var pIssueCAName = m_pIdentity.AuthorityName + "-ICA";
					if (pIssueCAItem.Load(pSQLContext, pIssueCAName) == false)
					{
						if (pIssueCAItem.CreateCA(m_pOrgProfile, pIssueCAName, pTrustCAItem) == false)
						{
							//　異常系：証明書の作成に失敗
							return (false);
						}
						if (pIssueCAItem.Save(pSQLContext) == false)
						{
							return (false);
						}
					}
				}
			}

			return (true);
		}

		//　サーバ証明書を生成する。
		public bool CreateForServer(SQLContext pSQLContext, string pCommonName, string pFQDN)
		{
			var pCertificate = new Certificate();
			if (pCertificate.CreateForServer(m_pOrgProfile, pCommonName, pFQDN, pIssueCAItem) == false)
			{
				return(false);
			}
			if (pCertificate.Save(pSQLContext) == false)
			{
				return (false);
			}

			return (true);
		}

		//　メール証明書を生成する。
		public bool CreateForClient(SQLContext pSQLContext, string pCommonName, string pMailAddress)
		{
			var pCertificate = new Certificate();
			if (pCertificate.CreateForClient(m_pOrgProfile, pCommonName, pMailAddress, pIssueCAItem) == false)
			{
				return (false);
			}
			if (pCertificate.Save(pSQLContext) == false)
			{
				return (false);
			}

			return (true);
		}

		//　
		protected Identity FetchIdentity(SQLContext pSQLContext, int iAuthorityKey)
		{
			var pIdentity = new Identity();

			var pSQL = "SELECT AuthorityKey, AuthorityName FROM TAuthority WHERE AuthorityKey = @AuthorityKey;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("AuthorityKey", iAuthorityKey);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pIdentity.AuthorityKey  = pReader.GetInt64(0);
						pIdentity.AuthorityName = pReader.GetString(1);
					}
				}
			}

			return (pIdentity);
		}

		//　
		protected OrgProfile FetchOrgProfile(SQLContext pSQLContext, int iOrgKey)
		{
			var pOrgProfile = new OrgProfile();

			var pSQL = "SELECT OrgKey, OrgName, OrgUnitName, LocalityName, ProvinceName, CountryName, UpdateAt FROM TOrgProfile WHERE OrgKey = @OrgKey;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("OrgKey", iOrgKey);
				using (var pReader = pCommand.ExecuteReader()) {
					while (pReader.Read())
					{
						pOrgProfile.OrgKey       = pReader.GetInt64(0);
						pOrgProfile.OrgName      = pReader.GetString(1);
						pOrgProfile.OrgUnitName  = pReader.GetString(2);
						pOrgProfile.LocalityName = pReader.GetString(3);
						pOrgProfile.ProvinceName = pReader.GetString(4);
						pOrgProfile.CountryName  = pReader.GetString(5);
						pOrgProfile.UpdataAt     = pReader.GetDateTime(6);
					}
				}
			}

			return (pOrgProfile);
		}

		//　
		public List<Certificate>	Listup(SQLContext pSQLContext)
		{
			var pCertificates = new List<Certificate>();

			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, TypeOf, Revoked, LaunchAt, ExpireAt, PemData, KeyData FROM TIssuedCerts WHERE Revoked = FALSE AND LaunchAt <= now() AND now() < ExpireAt;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				//pCommand.Parameters.AddWithValue("CommonName", pCommonName);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						var pCertificateItem = new Certificate();
						pCertificateItem.SequenceNumber = pReader.GetInt64(0);
						pCertificateItem.SerialNumber   = pReader.GetString(1);
						pCertificateItem.CommonName     = pReader.GetString(2);
						pCertificateItem.TypeOf         = (CertificateType)pReader.GetInt32(3);
						pCertificateItem.Revoked        = pReader.GetBoolean(4);
						pCertificateItem.LaunchAt       = pReader.GetDateTime(5);
						pCertificateItem.ExpireAt       = pReader.GetDateTime(6);
						pCertificateItem.PemData        = pReader.GetString(7);
						pCertificateItem.KeyData        = pReader.GetString(8);
						pCertificateItem.Prepare();

						pCertificates.Add(pCertificateItem);
					}
					if (pCertificates.Count == 0)
					{
						return (null);
					}
				}
			}

			return (pCertificates);
		}

		//　シリアル番号で証明書を取得
		public Certificate Fetch(SQLContext pSQLContext, string pSerialNumber)
		{
			var pCertificate = new Certificate();

			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, TypeOf, Revoked, LaunchAt, ExpireAt, PemData, KeyData FROM TIssuedCerts WHERE SerialNumber = @SerialNumber;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SerialNumber", pSerialNumber);
				using (var pReader = pCommand.ExecuteReader())
				{
					var iCount = 0;
					while (pReader.Read())
					{
						pCertificate.SequenceNumber = pReader.GetInt64(0);
						pCertificate.SerialNumber   = pReader.GetString(1);
						pCertificate.CommonName     = pReader.GetString(2);
						pCertificate.TypeOf         = (CertificateType)pReader.GetInt32(3);
						pCertificate.Revoked        = pReader.GetBoolean(4);
						pCertificate.LaunchAt       = pReader.GetDateTime(5);
						pCertificate.ExpireAt       = pReader.GetDateTime(6);
						pCertificate.PemData        = pReader.GetString(7);
						pCertificate.KeyData        = pReader.GetString(8);
						pCertificate.Prepare();
						iCount ++;
					}
					if (iCount == 0)
					{
						return (null);
					}
				}
			}

			return (pCertificate);
		}

		/*
		//　
		protected Certificate FetchCertificate(SQLContext pSQLContext, string pCN)
		{
			var pCertificateItem = new Certificate();

			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, Revoked, PemData FROM TIssuedCerts WHERE SequenceNumber = @SequenceNumber;";
			using (var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("SequenceNumber", 0);
				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						pCertificateItem.SequenceNumber = pReader.GetInt32(0);
						pCertificateItem.SerialNumber   = pReader.GetString(1);
						pCertificateItem.CommonName     = pReader.GetString(2);
						pCertificateItem.Revoked        = pReader.GetBoolean(3);
						pCertificateItem.PemData        = pReader.GetString(4);
					}
				}
			}

			return (pCertificateItem);
		}
		*/
		/*
		//　
		public void Initialize2(Identity pIdentity, OrgProfile pOrgProfile)
		{
			//m_pCertItems = new List<CertficateItem>();

			//　ルート証明書をロード
			var pCertificate = Profile.GetCertificate(0);
			if (pCertificate == null )
			{
				if ((pIdentity.Validate() == true) && (pOrgProfile.Validate() == true))
				{
					//　準正常：初回起動かルート証明書がなく、主体情報の入力は済んでいるため再作成
					string pCommonName = pIdentity.AuthorityName + "-RCA";
					var pCertificateItem = CertificateProvider.CreateRootCA(pOrgProfile, pCommonName);
					
					//m_pCertItems.Add(pCertificateItem);
				}
				else
				{
					//　準正常：初回起動かルート証明書がなく、主体情報が未入力
					//　この時点での処理はない（ルート証明書を作成できないので、主体情報の入力を促す）
				}
			}

			return;
		}
		*/


		//public List<CertficateItem> m_pCertItems;   //　

		//　
		public void SaveIdentity(SQLContext pSQLContext)
		{
			m_pIdentity.Save(pSQLContext);
		}

		//　
		public void SaveOrgProfile(SQLContext pSQLContext)
		{
			m_pOrgProfile.Save(pSQLContext);
		}
	}
}
