using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace TalkiPlay.Functional.UI.FormsExtensions
{
	public class ExtendedPicker : Picker
	{
		public static readonly BindableProperty ImageProperty =
			BindableProperty.Create(nameof(Image), typeof(string), typeof(ExtendedPicker), string.Empty);

		public string Image
		{
			get { return (string)GetValue(ImageProperty); }
			set { SetValue(ImageProperty, value); }
		}
	}
}
