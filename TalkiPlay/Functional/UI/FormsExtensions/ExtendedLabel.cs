#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace ChilliSource.Mobile.UI
{
    /// <summary>
    /// Xamarin Forms Label extension
    /// </summary>
	public class ExtendedLabel : Label//, IViewContainer<StyledTextPart>
    {
        /// <summary>
        /// Initializes a new instance of the <c>ExtendedLabel</c> class.
        /// </summary>
        public ExtendedLabel()
        {
            //Children = new List<StyledTextPart>();
        }

        /// <summary>
        /// Backing store for the <c>CustomFont</c> bindable property.
        /// </summary>
        public static readonly BindableProperty CustomFontProperty =
            BindableProperty.Create(nameof(CustomFont), typeof(ExtendedFont), typeof(ExtendedLabel), null);

        /// <summary>
        /// Gets or sets the custom font for the label's text. This is a bindable property.
        /// </summary>
        /// <value>A <see cref="ExtendedFont"/> value that represents the custom font for the content of the label. 
        /// The default value is <c>null</c>.</value>
        public ExtendedFont CustomFont
        {
            get { return (ExtendedFont)GetValue(CustomFontProperty); }
            set { SetValue(CustomFontProperty, value); }
        }
        
        /// <summary>
        /// Identifies the <c>AdjustFontSizeToFit</c> bindable property.
        /// </summary>
        public static readonly BindableProperty AdjustFontSizeToFitProperty =
            BindableProperty.Create(nameof(AdjustFontSizeToFit), typeof(bool), typeof(ExtendedLabel), false);

        /// <summary>
        /// Gets or sets a value indicating whether the font size of the content should be adjusted to fit inside the label. This is a bindable property.
        /// </summary>
        /// <value><c>true</c> if the font size should be adjusted to fit; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        public bool AdjustFontSizeToFit
        {
            get { return (bool)GetValue(AdjustFontSizeToFitProperty); }
            set { SetValue(AdjustFontSizeToFitProperty, value); }
        }


        /// <summary>
        /// Backing store for the <c>NumberOfLines</c> bindable property.
        /// </summary>
        public static readonly BindableProperty NumberOfLinesProperty =
            BindableProperty.Create(nameof(NumberOfLines), typeof(int), typeof(ExtendedLabel), 0);

        /// <summary>
        /// Gets or sets the number of lines for the label. This is a bindable property.
        /// </summary>
        /// <value>The number of lines in the label; the default is 0.</value>
        public int NumberOfLines
        {
            get { return (int)GetValue(NumberOfLinesProperty); }
            set { SetValue(NumberOfLinesProperty, value); }
        }
    }
    
}

