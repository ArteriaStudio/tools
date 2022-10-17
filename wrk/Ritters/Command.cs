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

		public SqlCommand(string pSQL, Context pContext)
		{
			m_pCommand = new NpgsqlCommand(pSQL, pContext.m_pConnection);
		}

		~SqlCommand()
		{
			m_pCommand = null;
		}
	}
}
