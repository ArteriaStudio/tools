using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AutoCA
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		public List<Parameter> pParameters;
		public SettingsPage()
		{
			this.InitializeComponent();
			pParameters = new List<Parameter>();
			pParameters.Add(new Parameter() { ItemName = "���ږ��P", ItemValue = "v" });
			pParameters.Add(new Parameter() { ItemName = "���ږ��Q", ItemValue = "v" });
			pParameters.Add(new Parameter() { ItemName = "���ږ��R", ItemValue = "v" });
			pParameters.Add(new Parameter() { ItemName = "���ږ��S", ItemValue = "v" });
			pParameters.Add(new Parameter() { ItemName = "���ږ��T", ItemValue = "v" });
		}
	}
}
