using Arteria_s.Common.LigareBook;
using Arteria_s.DB.Base;
using Microsoft.UI.Xaml;
using Npgsql;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
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


	//　形として不要なクラス
	public class StatusSheet : StudentsCursorEventListener
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

		//　生徒一覧を取得
		public ObservableCollection<Student>	ListupStudents(SQLContext pContext)
		{
//			var pContext = new Context(pProfileData.DatabaseServer, pProfileData.DatabaseName, pProfileData.SchemaName);
			var pCursor = new StudentsCursor(this);
			var pItems = pCursor.Listup(pContext);

			return(pItems);
		}

		//　一覧を取得
		public ObservableCollection<Person> 	Listup(Profile  pProfile)
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
			var pConnectionString = String.Format("Server={0};Port=5432;Database={1};Username={2};sslmode=disable", pProfile.DatabaseServer, pProfile.DatabaseName, pProfile.SchemaName);

			var pSQL = "SELECT StateID, Name FROM MStates;";

			System.Diagnostics.Debug.WriteLine("pConnectionString: " + pConnectionString);
			using (var pConnection = new NpgsqlConnection(pConnectionString))
			using (var pCommand = new NpgsqlCommand(pSQL, pConnection))
			{
				try
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
				catch (SocketException e)
				{
					System.Diagnostics.Debug.WriteLine("Exception: " + e.ToString());
					return(null);
				}
			}

			return (pItems);
		}

		public void OnChecked(string pPath, string pCodeSet, SQLContext pContext, int nItems, int nError, List<StudentCSV> pItems)
		{
			throw new NotImplementedException();
		}
	}
}
