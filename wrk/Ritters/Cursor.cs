using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ritters
{
	public class Cursor
	{
		//　
		public bool Create(NpgsqlConnection pConnection, Object pObject)
		{
			String	pSQL = "";
			using (var pCommand = new NpgsqlCommand(pSQL, pConnection))
			{
				pConnection.Open();

				using (var pReader = pCommand.ExecuteReader())
				{
					while (pReader.Read())
					{
						System.Diagnostics.Debug.WriteLine("" + pReader.GetString(0));
					}
				}
			}
			return(true);
		}
	}
}
