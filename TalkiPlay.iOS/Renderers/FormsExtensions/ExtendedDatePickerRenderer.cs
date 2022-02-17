using System;
using System.ComponentModel;
using System.Drawing;
using ChilliSource.Mobile.UI;
using TalkiPlay;
using TalkiPlay.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedDatePicker), typeof(ExtendedDatePickerRenderer))]


namespace TalkiPlay
{
    public class ExtendedDatePickerRenderer : DatePickerRenderer
    {
        public ExtendedDatePickerRenderer()
        {
        }

        ExtendedDatePicker ExtendedDatePicker => Element as ExtendedDatePicker;

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
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

                textField.HorizontalContentPadding = ExtendedDatePicker.HorizontalContentPadding;
                textField.BorderStyle = UITextBorderStyle.RoundedRect;
                textField.ClipsToBounds = true;                
            }

            SetBorder();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (ExtendedDatePicker == null)
            {
                return;
            }

            if (e.PropertyName == ExtendedDatePicker.HasBorderProperty.PropertyName ||
                e.PropertyName == ExtendedDatePicker.BorderColorProperty.PropertyName)
            {
                SetBorder();
            }
        }
       

        void SetBorder()
        {
            if (!ExtendedDatePicker.HasBorder)
            {
                Control.BorderStyle = UITextBorderStyle.None;
            }
            else
            {

                Control.Layer.BorderColor = ExtendedDatePicker.BorderColor.ToCGColor();
                Control.Layer.BorderWidth = 0.5f;
                Control.Layer.CornerRadius = 4;
            }
        }

    }
}
