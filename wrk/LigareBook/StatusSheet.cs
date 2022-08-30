using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		public ObservableCollection<Person> 	Listup()
		{
			var pItems = new ObservableCollection<Person>();

			var pItem = new Person();
			pItem.FirstName = "FirstName";
			pItem.FamilyName = "FamilyName";
			pItem.CompositionName = "CompositionName";

			pItems.Add(pItem);

			return (pItems);
		}
	}
}
