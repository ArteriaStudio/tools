using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public static class Lexical
	{
		public static bool IsSchool(string  pValue)
		{
			if (pValue.Equals("中学校") == false)
			{
				if (pValue.Equals("高校") == false)
				{
					return (false);
				}
			}
			return(true);
		}

		public static bool IsGrade(string pValue)
		{
			var iValue = int.Parse(pValue);
			if ((iValue < 1) || (iValue > 3))
			{
				return(false);
			}
			return(true);
		}

		public static bool IsName(string pValue)
		{
			var iDelimit = pValue.IndexOf("　");
			if (iDelimit == -1)
			{
				//　全角空白での区切り文字なし
				return(false);
			}

			return(true);
		}

		public static bool IsRead(string pValue)
		{
			var iDelimit = pValue.IndexOf("　");
			if (iDelimit == -1)
			{
				//　全角空白での区切り文字なし
				return (false);
			}

			return (true);
		}

		public static bool IsNumber(string pValue)
		{
			foreach (var iValue in pValue)
			{
				if (char.IsDigit(iValue) == false)
				{
					return(false);
				}
			}
			return (true);
		}

		public static bool IsGender(string pValue)
		{
			if (pValue.Equals("男") == false)
			{
				if (pValue.Equals("女") == false)
				{
					return (false);
				}
			}
			return (true);
		}

		public static bool IsDateTime(DateTime pValue)
		{
			/*
			if (pValue.Equals("") == false)
			{
				return(false);
			}
			*/
			return (true);
		}



	}
}
