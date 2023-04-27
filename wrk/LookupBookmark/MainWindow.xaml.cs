using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
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
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LookupBookmark
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();
			//TrySetSystemBackdrop();
		}

		public async void Button_Click(object sender, RoutedEventArgs e)
		{
			// Create the dialog
			var messageDialog = new MessageDialog("thumbnail.Name");
			messageDialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(DialogDismissedHandler)));

			MainWindow.ReferenceEquals(this, messageDialog);
			WinRT.Interop.InitializeWithWindow.Initialize(messageDialog, WinRT.Interop.WindowNative.GetWindowHandle(this));

			// Show the message dialog
			await messageDialog.ShowAsync();
		}

		private SpriteVisual _destinationSprite;
		private Compositor _compositor;
		private CompositionScopedBatch _scopeBatch;

		private void DialogDismissedHandler(IUICommand command)
		{
			// Start a scoped batch so we can register to completion event and hide the destination layer
			_scopeBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

			// Start the hide animation to fade out the destination effect
			ScalarKeyFrameAnimation hideAnimation = _compositor.CreateScalarKeyFrameAnimation();
			hideAnimation.InsertKeyFrame(0f, 1f);
			hideAnimation.InsertKeyFrame(1.0f, 0f);
			hideAnimation.Duration = TimeSpan.FromMilliseconds(1000);
			_destinationSprite.StartAnimation("Opacity", hideAnimation);

			// Use whichever effect is currently selected
					{
						CompositionSurfaceBrush brush = ((CompositionEffectBrush)_destinationSprite.Brush).GetSourceParameter("SecondSource") as CompositionSurfaceBrush;
						Vector2KeyFrameAnimation scaleAnimation = _compositor.CreateVector2KeyFrameAnimation();
						scaleAnimation.InsertKeyFrame(1f, new Vector2(2.0f, 2.0f));
						scaleAnimation.Duration = TimeSpan.FromMilliseconds(1000);
						brush.StartAnimation("Scale", scaleAnimation);
//						break;
					}
			/*
				case EffectTypes.VividLight:
					{
						CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
						ColorKeyFrameAnimation coloAnimation = _compositor.CreateColorKeyFrameAnimation();
						coloAnimation.InsertKeyFrame(1f, Color.FromArgb(255, 100, 100, 100));
						coloAnimation.Duration = TimeSpan.FromMilliseconds(1500);
						brush.StartAnimation("Base.Color", coloAnimation);
						break;
					}
				case EffectTypes.Hue:
					{
						CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
						ScalarKeyFrameAnimation rotateAnimation = _compositor.CreateScalarKeyFrameAnimation();
						rotateAnimation.InsertKeyFrame(1f, 0f);
						rotateAnimation.Duration = TimeSpan.FromMilliseconds(1500);
						brush.StartAnimation("Hue.Angle", rotateAnimation);
						break;
					}
				default:
					break;
			*/
		}
		/*
		//Scoped batch completed event
		_scopeBatch.Completed += ScopeBatch_Completed;
			_scopeBatch.End();
		}
	*/

		MicaController m_backdropController;

		protected bool TrySetSystemBackdrop()
		{
			if (MicaController.IsSupported() == true)
			{
				m_backdropController = new MicaController()
				{
					Kind = MicaKind.BaseAlt
				};
				//m_backdropController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
			}
			return(true);
		}
	}
}
