using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ritters
{
	public class DialogHelper
	{
		public DialogHelper(XamlRoot pXamlRoot)
		{
			m_pXamlRoot = pXamlRoot;
		}
		private XamlRoot m_pXamlRoot = null;

		public async void DisplayDialog(String pMessage)
		{
			ContentDialog pDialog = new ContentDialog
			{
				Title = "Notice",
				Content = pMessage,
				CloseButtonText = "Ok"
			};

			pDialog.XamlRoot = m_pXamlRoot;
			ContentDialogResult result = await pDialog.ShowAsync(); ;
		}

		public void DisplayDialogByID(String  pStringID)
		{
			var pLoader = new ResourceLoader();
			var pMessage = pLoader.GetString(pStringID);
			DisplayDialog(pMessage);
		}

	}
}
