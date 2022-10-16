using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public interface IEventListener
	{
		public void OnInsert(object pObject);
		public void OnUpdate(object  pObject);
	}

	public class TransitionContext
	{
		public IEventListener pListener = null;
		public Object	pArgments = null;
	}
}
