#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ChilliSource.Mobile.UI;
using System;
using System.Drawing;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]

namespace ChilliSource.Mobile.UI
{
    public class ExtendedEntryRenderer : ViewRenderer<Entry, ExtendedUITextField>
    {
        #region Lifecycle

        public ExtendedEntryRenderer()
        {
            Frame = new RectangleF(0, 20, 320, 40);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Control != null)
                {
                    Control.ShouldEndEditing -= ShouldEndEditing;

                    Control.EditingDidBegin -= OnEditingBegan;
                    Control.EditingChanged -= OnEditingChanged;
                    Control.EditingDidEnd -= OnEditingEnded;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        ExtendedEntry ExtendedEntry => Element as ExtendedEntry;
        IElementController ElementController => Element as IElementController;

        #endregion


        #region Event Handlers

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
            {
                return;
            }

            if (Control == null)
            {
                var textField = new ExtendedUITextField(RectangleF.Empty);
                SetNativeControl(textField);

                textField.HorizontalContentPadding = ExtendedEntry.HorizontalContentPadding;
                textField.BorderStyle = UITextBorderStyle.RoundedRect;
                textField.ClipsToBounds = true;


                textField.ShouldReturn = OnShouldReturn;
                textField.EditingDidBegin += OnEditingBegan;
                textField.EditingChanged += OnEditingChanged;
                textField.EditingDidEnd += OnEditingEnded;
            }

            SetInitialStyle();
            SetBorder();
            SetIsPassword();
            SetIsValid();
            SetText();
            SetPlaceholder();
            SetKeyboard();

            if (e.OldElement != null)
            {
                Control.ShouldEndEditing -= ShouldEndEditing;
            }

            if (e.NewElement != null)
            {
                Control.ShouldEndEditing += ShouldEndEditing;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (ExtendedEntry == null)
            {
                return;
            }

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
            {
                SetIsPassword();
            }
            else if (e.PropertyName == Xamarin.Forms.InputView.KeyboardProperty.PropertyName)
            {
                SetKeyboard();
            }
            else if (e.PropertyName == ExtendedEntry.HasBorderProperty.PropertyName ||
                     e.PropertyName == ExtendedEntry.BorderColorProperty.PropertyName)
            {
                SetBorder();
            }
            else if (e.PropertyName == Entry.TextProperty.PropertyName ||
                e.PropertyName == Entry.PlaceholderProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.IsValidProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomFontProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomPlaceholderFontProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomErrorFontProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomPlaceholderErrorFontProperty.PropertyName)
            {
                SetInitialStyle();
                SetIsValid();
                SetText();
                SetPlaceholder();
            }
        }


        void OnEditingBegan(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, true);
        }

        void OnEditingChanged(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);

        }

        void OnEditingEnded(object sender, EventArgs e)
        {
            // Typing aid changes don't always raise EditingChanged event
            if (Control.Text != Element.Text)
            {
                ElementController.SetValueFromRenderer(Entry.TextProperty, Control.Text);
            }

            ElementController.SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
        }

        protected virtual bool OnShouldReturn(UITextField view)
        {
            Control.ResignFirstResponder();
            ((IEntryController)Element).SendCompleted();
            return false;
        }

        bool ShouldEndEditing(UITextField textField)
        {
            if (Element == null)
            {
                return true;
            }

            return ExtendedEntry.ShouldCancelEditing;
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            //with borderStyle set to RoundedRect, iOS always returns a height of 30
            //https://stackoverflow.com/a/36569247/1063783
            //we get the current value, and restor it, to allow custom renderers to change the border style
            var borderStyle = Control.BorderStyle;
            Control.BorderStyle = UITextBorderStyle.None;
            var size = Control.GetSizeRequest(widthConstraint, double.PositiveInfinity);
            Control.BorderStyle = borderStyle;
            return size;
        }

        #endregion


        #region Property Setters

        void SetInitialStyle()
        {
           
            if (ExtendedEntry.TextIndentation > 0)
            {
                Control.Layer.SublayerTransform = CATransform3D.MakeTranslation(ExtendedEntry.TextIndentation, 0, 0);
            }
        }

        void SetBorder()
        {
            if (!ExtendedEntry.HasBorder)
            {
                Control.BorderStyle = UITextBorderStyle.None;
            }
            else
            {
                Control.BorderStyle = UITextBorderStyle.RoundedRect;
                Control.Layer.BorderColor = ExtendedEntry.BorderColor.ToCGColor();
                Control.Layer.BorderWidth = 0.5f;
                Control.Layer.CornerRadius = 4;
            }
        }

        void SetIsValid()
        {
            if (ExtendedEntry.IsValid)
            {
                Control.BackgroundColor = ExtendedEntry.NormalBackgroundColor.ToUIColor();
            }
            else
            {
                Control.BackgroundColor = ExtendedEntry.ErrorBackgroundColor.ToUIColor();

                if (ExtendedEntry.ShouldFocusWhenInvalid && !ExtendedEntry.IsFocused)
                {
                    ExtendedEntry.Focus();
                }
            }
        }

      
        void SetPlaceholder()
        {
            if (string.IsNullOrEmpty(ExtendedEntry.Text) && ExtendedEntry.Placeholder != null)

            {
                var fontToApply = ExtendedEntry.IsValid ? ExtendedEntry.CustomPlaceholderFont : ExtendedEntry.CustomPlaceholderErrorFont;

                if (fontToApply != null)
                {
                    ExtendedEntry.PlaceholderColor = fontToApply.Color;
                    Control.AttributedPlaceholder = fontToApply.BuildAttributedString(ExtendedEntry.Placeholder, Control.TextAlignment);
                }
                else
                {

                    Control.Placeholder = ExtendedEntry.Placeholder;
                }

            }
        }

        void SetText()
        {
            if (ExtendedEntry.Text != null)
            {
                
                var cursorLocation = Control.SelectedTextRange;

                var fontToApply = ExtendedEntry.CustomFont;

                if (!ExtendedEntry.IsValid && ExtendedEntry.CustomErrorFont != null)
                {
                    fontToApply = ExtendedEntry.CustomErrorFont;
                }

                if (fontToApply != null)
                {
                    Control.TextColor = fontToApply.Color.ToUIColor();
                    Control.TintColor = fontToApply.Color.ToUIColor();

                    var attributedString = fontToApply.BuildAttributedString(ExtendedEntry.Text);
                    Control.AttributedText = attributedString;
                }
                else
                {

                    Control.Placeholder = ExtendedEntry.Placeholder;
                }

                Control.SelectedTextRange = cursorLocation;


            }
        }
        
        void SetKeyboard()
        {
            switch (ExtendedEntry.KeyboardReturnType)
            {
                case KeyboardReturnKeyType.Done:
                    {
                        this.Control.ReturnKeyType = UIKit.UIReturnKeyType.Done;
                        break;
                    }
                case KeyboardReturnKeyType.Go:
                    {
                        this.Control.ReturnKeyType = UIKit.UIReturnKeyType.Go;
                        break;
                    }
                case KeyboardReturnKeyType.Next:
                    {
                        this.Control.ReturnKeyType = UIKit.UIReturnKeyType.Next;
                        break;
                    }
            }
            
            Control.ApplyKeyboard(Element.Keyboard);
            Control.ReloadInputViews();

        }

        void SetIsPassword()
        {
            if (Element.IsPassword && Control.IsFirstResponder)
            {
                Control.Enabled = false;
                Control.SecureTextEntry = true;
                Control.Enabled = Element.IsEnabled;
                Control.BecomeFirstResponder();
            }
            else
            {
                Control.SecureTextEntry = Element.IsPassword;
            }
        }
        #endregion
    }
}

