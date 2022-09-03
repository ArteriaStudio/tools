using Microsoft.UI.Xaml;
using Npgsql;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class Person
	{
		public string Name { get; set; }
		public string Number { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string FamilyName { get; set; }
		public string CompositionName { get; set; }
		public string Memo { get; set; }
	}

	public class StatusSheet
	{
		static StatusSheet	pInstance = new StatusSheet();

		StatusSheet()
		{
			;
		}

		public static StatusSheet GetInstance()
		{
			return(pInstance);
		}

		//　一覧を取得
		public ObservableCollection<Person> 	Listup(ProfileData  pProfileData)
		{
			var pItems = new ObservableCollection<Person>();

			{
				var pItem = new Person();
				pItem.FirstName = "FirstName";
				pItem.FamilyName = "FamilyName";
				pItem.CompositionName = "CompositionName";

				pItems.Add(pItem);
			}

			//　データベースサーバからデータを入力
			var pConnectionString = String.Format("Server={0};Port=5432;Database={1};Username={2};sslmode=disable", pProfileData.DatabaseServer, pProfileData.DatabaseName, pProfileData.SchemaName);

			var pSQL = "SELECT StateID, Name FROM MStates;";

			System.Diagnostics.Debug.WriteLine("pConnectionString: " + pConnectionString);
			using (var pConnection = new NpgsqlConnection(pConnectionString))
			using (var pCommand = new NpgsqlCommand(pSQL, pConnection))
			{
				pConnection.Open();

				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						System.Diagnostics.Debug.WriteLine("StateID: " + pReader.GetString(0));
						System.Diagnostics.Debug.WriteLine("Name: " + pReader.GetString(1));

						var pItem = new Person();
						pItem.Number = pReader.GetString(0);
						pItem.CompositionName = pReader.GetString(1);

						pItems.Add(pItem);
					}
				}
			}

			return (pItems);
		}
	}
}
