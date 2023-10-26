using Arteria_s.DB.Base;
using Microsoft.Data.Sqlite;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Security.Authentication.OnlineId;

namespace AutoCA
{
	//　証明書データをコレクションするクラス
	public class CertsStock
	{
		//private SQLContext	m_pSQLContext;
		//public OrgProfile	m_pOrgProfile;

		public CertsStock() { }

		//　
		public bool Load(SQLContext pSQLContext, Identity pIdentity, OrgProfile pOrgProfile)
		{
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

			if (pIdentity.Validate() == true)
			{
				if (pOrgProfile.Validate() == true)
				{
					//　ルート認証局の証明書データを入力
					var pTrustCAItem = new CertificateItem();
					var pTrustCAName = pIdentity.AuthorityName + "-RCA";
					if (pTrustCAItem.Load(pSQLContext, pTrustCAName) == false)
					{
						//　ルート認証局の証明書を作成
						if (pTrustCAItem.CreateCA(pOrgProfile, pTrustCAName, null) == false)
						{
							//　異常系：証明書の作成に失敗
							return(false);
						}
					}
					//var pTrustCAItem = FetchCertificate(pSQLContext, pTrustCAName);

					//　発行認証局の証明書データを入力
					var pIssueCAItem = new CertificateItem();
					var pIssueCAName = pIdentity.AuthorityName + "-ICA";
					if (pIssueCAItem.Load(pSQLContext, pIssueCAName) == false)
					{
						if (pIssueCAItem.CreateCA(pOrgProfile, pIssueCAName, pTrustCAItem) == false)
						{
							//　異常系：証明書の作成に失敗
							return (false);
						}
					}
					//var pIssueCAItem = FetchCertificate(pSQLContext, pIssueCAName);
				}
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
		/*
		//　
		protected CertificateItem FetchCertificate(SQLContext pSQLContext, string pCN)
		{
			var pCertificateItem = new CertificateItem();

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
	}
}
