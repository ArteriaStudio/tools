using Npgsql;
using Ritters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace Arteria_s.DB.Base
{
	public class SqlCommand
	{
		private NpgsqlCommand	m_pCommand = null;

		public SqlCommand(string pSQL, SQLContext pSQLContext)
		{
			m_pCommand = new NpgsqlCommand(pSQL, pSQLContext.m_pConnection);
		}

		~SqlCommand()
		{
			m_pCommand = null;
		}
	}
}
