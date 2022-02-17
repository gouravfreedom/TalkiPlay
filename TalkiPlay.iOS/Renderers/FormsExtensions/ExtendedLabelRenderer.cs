#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System.ComponentModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ChilliSource.Mobile.UI;

[assembly: ExportRenderer(typeof(ExtendedLabel), typeof(ExtendedLabelRenderer))]

namespace ChilliSource.Mobile.UI
{
	public class ExtendedLabelRenderer : LabelRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Element == null)
			{
				return;
			}

			SetStyle();
			UpdateLineFit();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (ExtendedLabel == null)
			{
				return;
			}

			if (e.PropertyName == Label.TextProperty.PropertyName ||
				e.PropertyName == Label.FormattedTextProperty.PropertyName ||
				e.PropertyName == ExtendedLabel.CustomFontProperty.PropertyName || 
				e.PropertyName == ExtendedLabel.IsVisibleProperty.PropertyName) 
			{
				SetStyle();
			}
			else if (e.PropertyName == ExtendedLabel.NumberOfLinesProperty.PropertyName)
			{
				UpdateLineFit();
			}
		}
		
        public ExtendedLabel ExtendedLabel => Element as ExtendedLabel;
		
		private void SetStyle()
		{
			if (ExtendedLabel.Text != null)
			{
				 var attributedString = ExtendedLabel.CustomFont.BuildAttributedString(ExtendedLabel.Text, this.Control.TextAlignment);
				 Control.AttributedText = attributedString;
			}
		}

		void UpdateLineFit()
		{
			if (ExtendedLabel.AdjustFontSizeToFit)
			{
				Control.AdjustsFontSizeToFitWidth = true;
				Control.Lines = 1;
			}
			else if (ExtendedLabel.NumberOfLines > 0)
			{
				Control.Lines = ExtendedLabel.NumberOfLines;
			}			
		}
	}
}

