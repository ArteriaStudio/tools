using Arteria_s.DB.Base;
using Microsoft.Data.Sqlite;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCA
{
	//　証明書データをコレクションするクラス
	public class CertsStock
	{
		private SQLContext	m_pSQLContext;

		public CertsStock() { }

		//　
		public bool Initialize(PrepareFlags pFlags, DbParams pDbParams)
		{
			if (pFlags.bExistDbParams == false)
			{
				//　準正常：接続情報が未登録のため、ここでの処理なし
				return (true);
			}
			//　データベースインスタンスに接続
			m_pSQLContext = new SQLContext(pDbParams.HostName, pDbParams.InstanceName, pDbParams.SchemaName, pDbParams.ClientKey, pDbParams.ClientCrt, pDbParams.TrustCrt);
			
			//　認証局の主体者情報を入力
			var pOrgProfile = FetchIdentity(m_pSQLContext, 0) as OrgProfile;

			//　ルート認証局の証明書データを入力
			var pTrustCAName = pOrgProfile.CaName + "-RCA";
			var pTrustCAItem = FetchCertificate(m_pSQLContext, pTrustCAName);
			
			//　発行認証局の証明書データを入力
			var pIssueCAName = pOrgProfile.CaName + "-ICA";
			var pIssueCAItem = FetchCertificate(m_pSQLContext, pIssueCAName);




			return (true);
		}

		protected OrgProfile FetchIdentity(SQLContext pSQLContext, int iUserIdentity)
		{
			var pOrgProfile = new OrgProfile();



			return(pOrgProfile);
		}

		protected CertificateItem FetchCertificate(SQLContext pSQLContext, string pCN)
		{
			var pCertificateItem = new CertificateItem();

			var pSQL = "SELECT SequenceNumber, SerialNumber, CommonName, Revoked, PemData FROM IssuedCerts WHERE SequenceNumber == @SequenceNumber;";
			var pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection);
			pCommand.Parameters.Clear();
			pCommand.Parameters.AddWithValue("SequenceNumber", 0);
			var pReader = pCommand.ExecuteReader();
			while (pReader.Read())
			{
				pCertificateItem.SequenceNumber = pReader.GetInt32(0);
				pCertificateItem.SerialNumber   = pReader.GetString(1);
				pCertificateItem.CommonName     = pReader.GetString(2);
				pCertificateItem.Revoked        = pReader.GetInt32(3);
				pCertificateItem.PemData        = pReader.GetString(4);
			}

			return (pCertificateItem);
		}

		public void Initialize2(OrgProfile pOrgProfile)
		{
			//m_pCertItems = new List<CertficateItem>();

			//　ルート証明書をロード
			var pCertificate = Profile.GetCertificate(0);
			if (pCertificate == null )
			{
				if ((pOrgProfile.CaName != null) && (pOrgProfile.CaName.Length > 0))
				{
					//　準正常：初回起動かルート証明書がなく、主体情報の入力は済んでいるため再作成
					string pCommonName = pOrgProfile.CaName + "-RCA";
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

		

		//public List<CertficateItem> m_pCertItems;   //　
	}
}
