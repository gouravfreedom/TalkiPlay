using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TalkiPlay.Functional.UI.FormsExtensions;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using TalkiPlay.Renderers.FormsExtensions;

[assembly: ExportRenderer(typeof(ExtendedPicker), typeof(ExtendedPickerRenderer))]

namespace TalkiPlay.Renderers.FormsExtensions
{
	public class ExtendedPickerRenderer : PickerRenderer
	{
		ExtendedPicker element;

		public ExtendedPickerRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
		{
			base.OnElementChanged(e);

			element = (ExtendedPicker)this.Element;

			if (Control != null && this.Element != null && !string.IsNullOrEmpty(element.Image))
            {
				Control.Background = AddPickerStyles(element.Image);
				Control.Gravity = GravityFlags.Center;
			}
		}

		public LayerDrawable AddPickerStyles(string imagePath)
		{
			Drawable[] layers = {GetDrawable(imagePath) };
			LayerDrawable layerDrawable = new LayerDrawable(layers);
			layerDrawable.SetLayerInset(0, 0, 0, 10, 0);
			return layerDrawable;
		}

		private BitmapDrawable GetDrawable(string imagePath)
		{
			int resID = Resources.GetIdentifier(imagePath, "drawable", this.Context.PackageName);
			var drawable = ContextCompat.GetDrawable(this.Context, resID);
			var bitmap = ((BitmapDrawable)drawable).Bitmap;

			var result = new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, 70, 70, true));
			result.Gravity = GravityFlags.Right;

			return result;
		}

	}
}