using Arteria_s.DB.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public interface LoaderEventListener
	{
		void OnLoaded();
	}

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

		public virtual bool Load(string pPath, string pCodeSet, Context pContext, LoaderEventListener pListener)
		{
			return(false);
		}
	}
	public delegate bool LoadPointers(string pPath);
}
