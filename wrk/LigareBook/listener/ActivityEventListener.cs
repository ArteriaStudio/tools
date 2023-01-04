using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LigareBook
{
	public interface ActivityEventListener
	{
		public abstract void OnItemClick(object sender, ItemClickEventArgs e);
	}
}
