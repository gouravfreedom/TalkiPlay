#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using System.ComponentModel;
using ChilliSource.Mobile.UI;

[assembly: ExportRenderer(typeof(ExtendedButton), typeof(ExtendedButtonRenderer))]

namespace ChilliSource.Mobile.UI
{
    public class ExtendedButtonRenderer : ButtonRenderer
    {
        ExtendedButton _extendedButton;        

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            if (Element == null)
            {
                return;
            }

            _extendedButton = Element as ExtendedButton;

            if (_extendedButton.IsCustomButton)
            {
                SetNativeControl(UIButton.FromType(UIButtonType.Custom));

                base.Control.TouchUpInside += (sender, _) =>
                {
                    ((IButtonController)Element)?.SendClicked();
                };
                base.OnElementChanged(e);

            }
            else
            {
                base.OnElementChanged(e);

            }

            SetStyle();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            _extendedButton = Element as ExtendedButton;

            if (_extendedButton != null)
            {
                if (e.PropertyName == Button.TextProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.CustomFontProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.PressedCustomFontProperty.PropertyName ||
                    e.PropertyName == ExtendedLabel.CustomFontProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.ImageVisibleProperty.PropertyName || 
                    e.PropertyName == ExtendedButton.AllowTextWrappingProperty.PropertyName)
                {
                    SetFont();
                }

                if (e.PropertyName == ExtendedButton.HorizontalContentAlignmentProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.VerticalContentAlignmentProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.ContentPaddingProperty.PropertyName)
                {
                    SetContentAlignment();
                }

                if (e.PropertyName == Button.ImageSourceProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.ImageRightAlignedProperty.PropertyName ||
                    e.PropertyName == ExtendedButton.ImageHorizontalOffsetProperty.PropertyName)
                {
                    SetImage();
                }

                if (e.PropertyName == ExtendedButton.PressedBackgroundColorProperty.PropertyName)
                {
                    SetPressedBackgroundColor(_extendedButton);
                }

                if (e.PropertyName == ExtendedButton.DisabledBackgroundColorProperty.PropertyName)
                {
                    SetDisabledBackgroundColor(_extendedButton);
                }
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            //_extendedButton.InvokeSubviewsDidLayoutEvent();
        }

        private void SetFont()
        {
            if (_extendedButton.CustomFont != null)
            {
                var normalString = _extendedButton.CustomFont.BuildAttributedString(_extendedButton.Text);

                PerformWithoutAnimation(() =>
                {
                    this.Control.SetAttributedTitle(normalString, UIControlState.Normal);
                    this.Control.LayoutIfNeeded();
                });

                this.Control.SetTitleColor(_extendedButton.CustomFont.Color.ToUIColor(), UIControlState.Normal);
                this.Control.SetTitleColor(
                    _extendedButton.PressedCustomFont != null
                        ? _extendedButton.PressedCustomFont.Color.ToUIColor()
                        : _extendedButton.CustomFont.Color.ToUIColor(), UIControlState.Highlighted);
            }

            if (_extendedButton.PressedCustomFont != null)
            {
                var pressedString = _extendedButton.PressedCustomFont.BuildAttributedString(_extendedButton.Text);
                this.Control.SetAttributedTitle(pressedString, UIControlState.Highlighted);
            }

            if (_extendedButton.AllowTextWrapping)
            {
                this.Control.TitleLabel.AdjustsFontSizeToFitWidth = false;
                this.Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                this.Control.TitleLabel.TextAlignment = UITextAlignment.Center;
            }
            else
            {
                this.Control.TitleLabel.LineBreakMode = UILineBreakMode.TailTruncation;
                this.Control.TitleLabel.AdjustsFontSizeToFitWidth = true;
                this.Control.TitleLabel.MinimumFontSize = 14.0f;    
            }
            
        }

        void SetContentAlignment()
        {
            switch (_extendedButton.HorizontalContentAlignment)
            {
                case ButtonHorizontalContentAlignment.Left:
                    this.Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
                    break;

                case ButtonHorizontalContentAlignment.Right:
                    this.Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Right;
                    break;

                case ButtonHorizontalContentAlignment.Center:
                    this.Control.HorizontalAlignment = UIControlContentHorizontalAlignment.Center;
                    break;
            }

            switch (_extendedButton.VerticalContentAlignment)
            {
                case ButtonVerticalContentAlignment.Top:
                    this.Control.VerticalAlignment = UIControlContentVerticalAlignment.Top;
                    break;

                case ButtonVerticalContentAlignment.Bottom:
                    this.Control.VerticalAlignment = UIControlContentVerticalAlignment.Bottom;
                    break;

                case ButtonVerticalContentAlignment.Center:
                    this.Control.VerticalAlignment = UIControlContentVerticalAlignment.Center;
                    break;
            }

            Control.ContentEdgeInsets = _extendedButton.ContentPadding.ToUIEdgeInsets();
        }

        async void SetImage()
        {
            var image = await _extendedButton.ImageSource.ToUIImage();
            if (image != null && _extendedButton.ImageVisible)
            {
                Control.SetImage(image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);

                if (_extendedButton.ImageRightAligned)
                {
                    Control.TitleEdgeInsets = new UIEdgeInsets(0, -Control.ImageView.Frame.Size.Width, 0, Control.ImageView.Frame.Size.Width);
                    Control.ImageEdgeInsets = new UIEdgeInsets(0, Control.TitleLabel.Frame.Size.Width + _extendedButton.ImageHorizontalOffset, 0, -Control.TitleLabel.Frame.Size.Width);
                    Control.SizeToFit();
                }
                else
                {
                    Control.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, image.Size.Width + _extendedButton.ImageHorizontalOffset);
                }
            }
            else if (!_extendedButton.ImageVisible)
            {
                Control.SetImage(null, UIControlState.Normal);
            }
        }

        private void SetDisabledBackgroundColor(ExtendedButton extendedButton)
        {
            if (extendedButton.DisabledBackgroundColor != Color.Default)
            {
                var disabled = UIColorExtensions.GetImageFromColor(extendedButton.DisabledBackgroundColor.ToUIColor());
                if (extendedButton.BackgroundColor != Color.Transparent)
                {
                    this.Control.Layer.BorderColor = UIColor.Clear.CGColor;
                }
                this.Control.SetBackgroundImage(disabled, UIControlState.Disabled);
            }
        }

        private void SetPressedBackgroundColor(ExtendedButton extendedButton)
        {
            if (extendedButton.PressedBackgroundColor != Color.Default)
            {
                var pressed = UIColorExtensions.GetImageFromColor(extendedButton.PressedBackgroundColor.ToUIColor());
                this.Control.SetBackgroundImage(pressed, UIControlState.Highlighted);
                this.Control.SetBackgroundImage(pressed, UIControlState.Selected);
                this.Control.ClipsToBounds = true;
            }
        }

        private void SetStyle()
        {
            if (_extendedButton.Text != null)
            {
                SetFont();
                SetContentAlignment();
            }

            if (_extendedButton.ImageSource != null)
            {
                SetImage();
            }

            SetPressedBackgroundColor(_extendedButton);
            SetDisabledBackgroundColor(_extendedButton);

            if (Control != null && Control.Layer != null && _extendedButton != null)
            {
                Control.Layer.BorderColor = _extendedButton.BorderColor.ToCGColor();
                Control.Layer.BorderWidth = (float)_extendedButton.BorderWidth;
            }
        }
    }
}



