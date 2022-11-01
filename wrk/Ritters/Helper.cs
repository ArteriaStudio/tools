using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.UI.Helper
{
	public static class Functions
	{
		public static void EnableAllControls(Panel pPanel, bool bEnabled)
		{
			foreach (var pElement in pPanel.Children)
			{
				if (pElement is Control)
				{
					Control pControl = pElement as Control;
					pControl.IsEnabled = bEnabled;
					System.Diagnostics.Trace.WriteLine("Name: " + pControl.Name);
				}
				else if (pElement is Panel)
				{
					Panel pChild = pElement as Panel;
					EnableAllControls(pChild, bEnabled);
				}
			}
		}
	}
}
