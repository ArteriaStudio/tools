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
		//　カーソルを作成
		public Cursor()
		{
			;
		}

		//　
		~Cursor()
		{
			;
		}

		public event EventHandler<NpgsqlDataReader> CursorEventHandler;

		public bool Open(Context pContext, String pSQL)
		{
			using (var pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection))
			{
				using (var pReader = pCommand.ExecuteReader())
				{
					if (CursorEventHandler != null)
					{
						CursorEventHandler(this, pReader);
					}
				}
			}
			return(true);
		}


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
