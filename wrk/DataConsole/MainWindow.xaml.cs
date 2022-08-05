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

namespace DataConsole
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
		}

		private void myButton_Click(object sender, RoutedEventArgs e)
		{
			//myButton.Content = "Clicked";
		}
		private void TwoPaneView_ModeChanged(Microsoft.UI.Xaml.Controls.TwoPaneView sender, object args)
		{
			/*
			// Remove details content from it's parent panel.
			((Panel)DetailsContent.Parent).Children.Remove(DetailsContent);
			// Set Normal visual state.
			Windows.UI.Xaml.VisualStateManager.GoToState(this, "Normal", true);

			// Single pane
			if (sender.Mode == Microsoft.UI.Xaml.Controls.TwoPaneViewMode.SinglePane)
			{
				// Add the details content to Pane1.
				Pane1StackPanel.Children.Add(DetailsContent);
			}
			// Dual pane.
			else
			{
				// Put details content in Pane2.
				Pane2Root.Children.Add(DetailsContent);

				// If also in Wide mode, set Wide visual state
				// to constrain the width of the image to 2*.
				if (sender.Mode == Microsoft.UI.Xaml.Controls.TwoPaneViewMode.Wide)
				{
					Windows.UI.Xaml.VisualStateManager.GoToState(this, "Wide", true);
				}
			}
			*/
		}
	}
}
