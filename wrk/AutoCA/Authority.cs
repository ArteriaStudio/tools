﻿using Arteria_s.DB.Base;
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
			//　組織プロファイルと認証局情報を入力
			var iUserIdentity = 0;
			m_pIdentity = new Identity();
			m_pIdentity.Load(pSQLContext, iUserIdentity);
			m_pOrgProfile = new OrgProfile();
			m_pOrgProfile.Load(pSQLContext, iUserIdentity);

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
						var pCertificate = new Certificate();
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
						pCertificates.Add(pCertificate);
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

		//　有効期限を延長した証明書を発行
		public void Update(SQLContext pSQLContext, Certificate pCertificate)
		{
			;
		}

		//　証明書を失効
		public void Revoke(SQLContext pSQLContext, Certificate pCertificate)
		{
			pCertificate.Revoke(pSQLContext);
		}

		//　認証局情報を保存
		public void SaveIdentity(SQLContext pSQLContext)
		{
			m_pIdentity.Save(pSQLContext);
		}

		//　組織プロファイルを保存
		public void SaveOrgProfile(SQLContext pSQLContext)
		{
			m_pOrgProfile.Save(pSQLContext);
		}
	}
}