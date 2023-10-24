using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCA
{
	public class CertsStock
	{
		public CertsStock() { }
		public void Initialize(OrgProfile pOrgProfile)
		{
			m_pCertItems = new List<CertficateItem>();

			//　ルート証明祖をロード
			var pCertificate = Profile.GetCertificate(0);
			if (pCertificate == null )
			{
				string pCommonName = pOrgProfile.CaName + "-RCA";
				CertificateProvider.CreateRootCA(pOrgProfile, pCommonName);
				//m_pCertItems.Add();
			}
			return;
		}
		public List<CertficateItem> m_pCertItems;   //　
	}
}
