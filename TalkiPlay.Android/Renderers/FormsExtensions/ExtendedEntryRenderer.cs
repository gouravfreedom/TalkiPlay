#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Views.InputMethods;
using Android.Text;
using Android.Graphics.Drawables;
using ChilliSource.Mobile.UI;
using Android.Content;
using Android.Support.V4.Content;
using Android.Graphics;

[assembly: ExportRenderer(typeof(ExtendedEntry), typeof(ExtendedEntryRenderer))]

namespace ChilliSource.Mobile.UI
{
    public class ExtendedEntryRenderer : EntryRenderer
    {
        public ExtendedEntryRenderer(Context context) : base(context)
        {

        }

        ExtendedEntry ExtendedEntry => Element as ExtendedEntry;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
            {
                return;
            }
            

            if (string.IsNullOrEmpty(ExtendedEntry.Text))
            {
                SetPlaceholder();
            }

            SetStyle();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            if (ExtendedEntry != null &&
                e.PropertyName == Entry.TextProperty.PropertyName ||
                e.PropertyName == Entry.PlaceholderProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.IsValidProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomFontProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.CustomPlaceholderFontProperty.PropertyName ||
                e.PropertyName == ExtendedEntry.ErrorMessageProperty.PropertyName)
            {
                SetStyle();

                if (string.IsNullOrEmpty(ExtendedEntry.Text))
                {
                    SetPlaceholder();
                }
            }
        }

        void SetPlaceholder()
        {            
            if (ExtendedEntry.Placeholder != null)
            {
                var fontToApply = ExtendedEntry.IsValid ? ExtendedEntry.CustomPlaceholderFont : ExtendedEntry.CustomPlaceholderErrorFont;

                if (fontToApply != null)
                {
                    // This will replace the Text property font not the placeholder. 
                    // fontToApply.ApplyTo(Control);

                    //This only sets the color of the placeholder.
                    Control.SetHintTextColor(fontToApply.Color.ToAndroid());
                }
            }
        }

        void SetBorder()
        {            
            if (ExtendedEntry.HasBorder)
            {
                var nativeEditText = (global::Android.Widget.EditText)Control;
                var shape = new ShapeDrawable(new Android.Graphics.Drawables.Shapes.RectShape());
                shape.Paint.Color = ExtendedEntry.BackgroundColor.ToAndroid();
                shape.Paint.SetStyle(Paint.Style.Stroke);
                nativeEditText.Background = shape;
            }
        }
        
        void SetStyle()
        {            
            var fontToApply = ExtendedEntry.IsValid ? ExtendedEntry.CustomFont : ExtendedEntry.CustomErrorFont;

            if (fontToApply != null)
            {
                fontToApply.ApplyTo(Control);
            }
        }
    }
}

