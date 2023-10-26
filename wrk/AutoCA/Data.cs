using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.DB
{
	public abstract class Data
	{
		protected bool IsNotNull(string pText)
		{
			if (pText == null)
			{
				return (false);
			}
			if (pText.Length <= 0)
			{
				return (false);
			}
			return (true);
		}

		protected bool IsNull(string pValue)
		{
			if (pValue == null)
			{
				return (true);
			}
			pValue = pValue.Trim();
			if (pValue.Length <= 0)
			{
				return (true);
			}
			return (false);
		}

		public abstract bool Validate();
	}
}
