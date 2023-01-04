using Arteria_s.DB.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public class Loader
	{
		public Loader()
		{
			;
		}
		~Loader()
		{
			;
		}

		//　CSV ファイルを入力
		public virtual bool Load(string pPath, string pCodeSet, SQLContext pContext)
		{
			return(false);
		}
		//　CSV ファイルを検査
		public virtual bool Check(string pPath, string pCodeSet, SQLContext pContext)
		{
			return(false);
		}
	}
	//public delegate bool LoadPointers(string pPath);
}
