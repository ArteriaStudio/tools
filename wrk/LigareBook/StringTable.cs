using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class StringTable
	{
		public static int TransformStaffTypeNameToNumber(string pStaffTypeName)
		{
			var pTable = new Dictionary<string, int>()
			{
//				{ "教育職員", 1 },
				{ "事務職員", 2 },
				{ "技術職員", 3 },
			};

			int iValue = 0;
			try
			{
				iValue = pTable[pStaffTypeName];
			}
			catch(Exception)
			{
				iValue = 0;
			}

			return (iValue);
		}
		public static int TransformContractTypeNameToNumber(string pContractTypeName)
		{
			var pTable = new Dictionary<string, int>()
			{
				{ "専任", 1 },
				{ "常勤", 2 },
				{ "非常勤", 3 },
				{ "嘱託", 4 },
				{ "派遣", 5 },
			};

			int iValue = 0;
			try
			{
				iValue = pTable[pContractTypeName];
			}
			catch (Exception)
			{
				iValue = 0;
			}

			return (iValue);
		}
	}
}
