using Arteria_s.DB.Base;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.Windows.ApplicationModel.Resources;
using Npgsql;
using Ritters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class OrgUnitCSV
	{
		[Index(0)]
		public int Year { get; set; }
		[Index(1)]
		public string OrgUnitCode { get; set; }
		[Index(2)]
		public string OrgUnitName { get; set; }
		[Index(3)]
		public string ContainerCode { get; set; }

		public bool CheckSelf()
		{
			if (Year < 2000)
			{
				return(false);
			}
			return(true);
		}
		public void Dump()
		{
			System.Diagnostics.Debug.WriteLine($"年度: {Year}");
			System.Diagnostics.Debug.WriteLine($"組織単位コード: {OrgUnitCode}");
			System.Diagnostics.Debug.WriteLine($"組織単位名: {OrgUnitName}");
			System.Diagnostics.Debug.WriteLine($"上位組織コード: {ContainerCode}");
		}
	}

	public interface OrgUnitsLoaderEventListener
	{
		public abstract void OnCheckedOrgUnits(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<OrgUnitCSV> pItems);
	}

	public class OrgUnitsLoader : Loader
	{
		OrgUnitsLoaderEventListener 	m_pListener = null;
		public OrgUnitsLoader(OrgUnitsLoaderEventListener pListener)
		{
			m_pListener = pListener;
		}
		~OrgUnitsLoader()
		{
			m_pListener = null;
		}
		public override bool Load(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				while (pFetcher.Read())
				{
					var pRecords = pFetcher.GetRecord<OrgUnitCSV>();
					if (pRecords.CheckSelf() == true)
					{
						OrgUnit 	pOrgUnit = new OrgUnit();

						pOrgUnit.Year        = pRecords.Year;
						pOrgUnit.OrgUnitCode = pRecords.OrgUnitCode;
						pOrgUnit.OrgUnitName = pRecords.OrgUnitName;

						//　データベースに登録
						OrgUnitsCursor.Insert(pContext, pRecords.ContainerCode, pOrgUnit);
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
					}
				}
			}

			return(true);
		}

		public override bool Check(string pPath, string pCodeSet, SQLContext pContext)
		{
			// https://code-maze.com/csharp-read-data-from-csv-file/
			var pConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = true,
			};
			using (var pReader = new StreamReader(pPath, Encoding.GetEncoding(pCodeSet)))
			using (var pFetcher = new CsvReader(pReader, pConfiguration))
			{
				var nItems = 0;
				var nError = 0;
				var pItems = new List<OrgUnitCSV>();
				while (pFetcher.Read())
				{
					var pRecoords = pFetcher.GetRecord<OrgUnitCSV>();
					if (pRecoords.CheckSelf() == true)
					{
						pItems.Add(pRecoords);
						nItems ++;
					}
					else
					{
						//　字句エラー
						System.Diagnostics.Debug.WriteLine("Found Error in input record self check.");
						nError ++;
					}
				}
				m_pListener.OnCheckedOrgUnits(pPath, pCodeSet, pContext, nItems, nError, pItems);
			}

			return (true);
		}
	}
}
