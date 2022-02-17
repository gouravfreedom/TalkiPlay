using System;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ExtendedDatePicker : DatePicker
    {
        public ExtendedDatePicker()
        {
        }

        /// <summary>
        /// Identifies the <c>HasBorder</c> bindable property.
        /// </summary>
        public static readonly BindableProperty HasBorderProperty =
            BindableProperty.Create(nameof(HasBorder), typeof(bool), typeof(ExtendedDatePicker), true);

        /// <summary>
        /// Gets or sets a value indicating whether the entry has border. This is a bindable property.
        /// </summary>
        /// <value><c>true</c> if the entry has border; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
		public bool HasBorder
        {
            get { return (bool)GetValue(HasBorderProperty); }
            set { SetValue(HasBorderProperty, value); }
        }


        /// <summary>
        /// Backing store for the <c>BorderColor</c> bindable property.
        /// </summary>
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(ExtendedDatePicker), Color.FromHex("C7C7C8"));

        /// <summary>
        /// Gets or sets the color of the entry's border. This is a bindable property.<see cref="Color.FromHex(string)"/>
        /// </summary>
        /// <value>A <see cref="Color"/> value that represents the border color of the entry. The default value is <see cref="Color.FromHex(string)"/>.</value>
        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        /// <summary>
		///Backing store for the <c>HorizontalContentPadding</c> bindable property.
		/// </summary>
		public static readonly BindableProperty HorizontalContentPaddingProperty =
            BindableProperty.Create(nameof(HorizontalContentPadding), typeof(float), typeof(ExtendedDatePicker), 5f);

        /// <summary>
        /// Gets or sets the horizontal padding for the entry's content. This is a bindable property.
        /// </summary>		
        public float HorizontalContentPadding
        {
            get { return (float)GetValue(HorizontalContentPaddingProperty); }
            set { SetValue(HorizontalContentPaddingProperty, value); }
        }

    }
}
