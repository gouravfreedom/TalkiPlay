using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalkiPlay.Functional.UI.FormsExtensions;
using TalkiPlay.Renderers.FormsExtensions;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedPicker), typeof(ExtendedPickerRenderer))]

namespace TalkiPlay.Renderers.FormsExtensions
{
	public class ExtendedPickerRenderer : PickerRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			var element = (ExtendedPicker)this.Element;

			if (this.Control != null && this.Element != null && !string.IsNullOrEmpty(element.Image))
			{
				var downarrow = UIImage.FromBundle(element.Image);
				Control.RightViewMode = UITextFieldViewMode.Always;
				Control.RightView = new UIImageView(downarrow);
				Control.TextAlignment = UITextAlignment.Center;
			}
		}
	}
}