using Microsoft.UI.Xaml.Controls.Primitives;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

		//　PostgreSQL セッション変数を更新
		protected void SetSessionVariable(String VariableName, String VariableValue)
		{
			var pSQL = "SELECT SET_CONFIG(@VariableName, @VariableValue, false);";
			using (var pCommand = new NpgsqlCommand(pSQL, m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("VariableName", VariableName);
				pCommand.Parameters.AddWithValue("VariableValue", VariableValue);

				try
				{
					pCommand.ExecuteNonQuery();
				}
				catch (PostgresException e)
				{
					System.Diagnostics.Debug.WriteLine($"Mesage: {e.MessageText} Code: {e.ErrorCode}");
				}
			}

			return;
		}

		protected string GetSessionVariable(String VariableName)
		{
			string	VariableValue = null;

			var pSQL = "SELECT current_setting(@VariableName);";
			using (var pCommand = new NpgsqlCommand(pSQL, m_pConnection))
			{
				pCommand.Parameters.Clear();
				pCommand.Parameters.AddWithValue("VariableName", VariableName);

				try
				{
					using (var pReader = pCommand.ExecuteReader())
					{
						while (pReader.Read())
						{
							VariableValue = pReader.GetString(0);
						}
					}
				}
				catch (PostgresException e)
				{
					System.Diagnostics.Debug.WriteLine($"Mesage: {e.MessageText} Code: {e.ErrorCode}");
				}
			}

			return (VariableValue);
		}
	}

	public class ContextEx : Context
	{
		public ContextEx(string DatabaseServer, string DatabaseName, string SchemaName, string AccountName, string AccessToken) : base(DatabaseServer, DatabaseName, SchemaName)
		{
			SetSessionVariable("app.AccountName", AccountName);
			SetSessionVariable("app.AccessToken", AccessToken);
			GetSessionVariable("app.AccountName");
		}
	}
}
