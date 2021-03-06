#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using ChilliSource.Mobile.UI;
using Android.Content;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]

namespace ChilliSource.Mobile.UI
{
	public class ExtendedButtonRenderer : ButtonRenderer
	{
	    public ExtendedButtonRenderer(Context context) : base(context)
	    {

	    }

		protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
		{
			base.OnElementChanged(e);

			if (Element == null)
			{
				return;
			}

			SetStyle();
            SetPadding();
        }

		void SetStyle()
		{
			var styledButton = (ExtendedButton)this.Element;

			switch (styledButton.HorizontalContentAlignment)
			{
				case ButtonHorizontalContentAlignment.Left:
					this.Control.Gravity = GravityFlags.Left;
					break;

				case ButtonHorizontalContentAlignment.Right:
					this.Control.Gravity = GravityFlags.Right;
					break;

				case ButtonHorizontalContentAlignment.Center:
					this.Control.Gravity = GravityFlags.Center;
					break;
			}
			if (!String.IsNullOrWhiteSpace(styledButton.Text) && styledButton.CustomFont != null)
			{
				styledButton.CustomFont.ApplyTo(Control);
			}

			if (styledButton.AllowTextWrapping)
			{
				this.Control.SetSingleLine(false);
				this.Control.SetMaxLines(3);
			}
			else
			{
				this.Control.SetSingleLine(true);
				this.Control.SetMaxLines(1);
			}
		}

        void SetPadding()
        {
	        Control?.SetPadding(0, Control.PaddingTop, 0, Control.PaddingBottom);
        }
    }
}

