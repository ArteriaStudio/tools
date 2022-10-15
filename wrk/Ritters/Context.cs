using Microsoft.UI.Xaml.Controls.Primitives;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.DB.Base
{
	//　データベースコンテキスト
	public class Context
	{
		public NpgsqlConnection m_pConnection = null;

		//　
		public Context(String DatabaseServer, String DatabaseName, String SchemaName)
		{
			var pConnectionString = String.Format("Server={0};Port=5432;Database={1};Username={2};sslmode=disable", DatabaseServer, DatabaseName, SchemaName);
			System.Diagnostics.Debug.WriteLine("pConnectionString: " + pConnectionString);
			m_pConnection = new NpgsqlConnection(pConnectionString);
			m_pConnection.Open();
		}

		//　
		~Context()
		{
			m_pConnection.Close();
			m_pConnection = null;
		}
	}
}
