using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arteria_s.UI.Base
{
	public class FrameWindow : Window
	{
		public FrameWindow()
		{
			var pLoader = new ResourceLoader();
			this.Title = pLoader.GetString("ApplicationName");

			/*
			//　既定のタイトルバーを隠す
			this.ExtendsContentIntoTitleBar = true;
			*/
		}
	}
}
