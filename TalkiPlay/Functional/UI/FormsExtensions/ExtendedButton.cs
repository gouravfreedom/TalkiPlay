#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System.Windows.Input;
using Xamarin.Forms;

namespace ChilliSource.Mobile.UI
{
    /// <summary>
    /// Specifies the horizontal content alignment of the button.
    /// </summary>
	public enum ButtonHorizontalContentAlignment
    {
        Left,
        Right,
        Center
    }

    /// <summary>
    /// Specifies the horizontal content alignment of the button.
    /// </summary>
    public enum ButtonVerticalContentAlignment
    {
        Top,
        Bottom,
        Center
    }
    
    /// <summary>
    /// Xamarin Forms Button extension
    /// </summary>
	public class ExtendedButton : Button
    {
        /// <summary>
        /// Backing store for the <c>PressedBackgroundColor</c> bindable property.
        /// </summary>
        public static readonly BindableProperty PressedBackgroundColorProperty =
            BindableProperty.Create(nameof(PressedBackgroundColor), typeof(Color), typeof(ExtendedButton), Color.Default);

        /// <summary>
        /// Gets or sets the background color of the button when it is pressed . This is a bindable property.
        /// </summary>
        /// <value>A <see cref="Color"/> value that represents the bachground color of the button whenever is pressed. 
        /// The default value is <see cref="Color.Default"/>.</value>
        public Color PressedBackgroundColor
        {
            get { return (Color)GetValue(PressedBackgroundColorProperty); }
            set { SetValue(PressedBackgroundColorProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>DisabledBackgroundColor</c> bindable property.
        /// </summary>
        public static readonly BindableProperty DisabledBackgroundColorProperty =
            BindableProperty.Create(nameof(DisabledBackgroundColor), typeof(Color), typeof(ExtendedButton), Color.Default);

        /// <summary>
        /// Gets or sets the background color of the button when it is disabled. This is a bindable property.
        /// </summary>
        /// <value>A <see cref="Color"/> value that represents the bachground color of the button when is disabled. 
        /// The default value is <see cref="Color.Default"/>.</value>
        public Color DisabledBackgroundColor
        {
            get { return (Color)GetValue(DisabledBackgroundColorProperty); }
            set { SetValue(DisabledBackgroundColorProperty, value); }
        }
               
        /// <summary>
        /// Identifies the <c>IsCustomButton</c> bindable Property.
        /// </summary>
        public static readonly BindableProperty IsCustomButtonProperty =
                BindableProperty.Create(nameof(IsCustomButton), typeof(bool), typeof(ExtendedButton), false);

        /// <summary>
        /// Gets or sets a value indicating whether the button is custom type for iOS platform. This is a bindable property.
        /// </summary>
        /// <value><c>true</c> if the button is custom type; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        public bool IsCustomButton
        {
            get { return (bool)GetValue(IsCustomButtonProperty); }
            set { SetValue(IsCustomButtonProperty, value); }
        }

      
        public static readonly BindableProperty AllowTextWrappingProperty =
            BindableProperty.Create(nameof(AllowTextWrapping), typeof(bool), typeof(ExtendedButton), false);

        public bool AllowTextWrapping
        {
            get { return (bool)GetValue(AllowTextWrappingProperty); }
            set { SetValue(AllowTextWrappingProperty, value); }
        }
        
        /// <summary>
        /// Backing store for the <c>CustomFont</c> bindable property.
        /// </summary>
        public static readonly BindableProperty CustomFontProperty =
            BindableProperty.Create(nameof(CustomFont), typeof(ExtendedFont), typeof(ExtendedButton), null);

        /// <summary>
        /// Gets or sets the custom font for the text of the button. This is a bindable property.
        /// </summary>
        /// <value>A <see cref="ExtendedFont"/> value that represents the custom font for the button's text. 
        /// The default value is <c>null</c>.</value>
		public ExtendedFont CustomFont
        {
            get { return (ExtendedFont)GetValue(CustomFontProperty); }
            set { SetValue(CustomFontProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>PressedCustomFont</c> bindable property.
        /// </summary>
        public static readonly BindableProperty PressedCustomFontProperty =
            BindableProperty.Create(nameof(PressedCustomFont), typeof(ExtendedFont), typeof(ExtendedButton), null);

        /// <summary>
        /// Gets or sets the custom font for the button's text whenever the button is pressed.
        /// </summary>
        /// <value>A <see cref="ExtendedFont"/> value that represents the custom font for the button.
        ///  The default value is <c>null</c>.</value>
        public ExtendedFont PressedCustomFont
        {
            get { return (ExtendedFont)GetValue(PressedCustomFontProperty); }
            set { SetValue(PressedCustomFontProperty, value); }
        }

        /// <summary>
        /// Identifies the <c>ImageRightAligned</c> bindable property.
        /// </summary>
        public static readonly BindableProperty ImageRightAlignedProperty =
            BindableProperty.Create(nameof(ImageRightAligned), typeof(bool), typeof(ExtendedButton), false);

        /// <summary>
        /// Gets or sets a value indicating whether the image of the button is right aligned. This is a bindable property.
        /// </summary>
        /// <value><c>true</c> if image is right aligned; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
		public bool ImageRightAligned
        {
            get { return (bool)GetValue(ImageRightAlignedProperty); }
            set { SetValue(ImageRightAlignedProperty, value); }
        }

        /// <summary>
        /// Identifies the <c>ImageVisible</c> bindable property.
        /// </summary>
        public static readonly BindableProperty ImageVisibleProperty =
            BindableProperty.Create(nameof(ImageVisible), typeof(bool), typeof(ExtendedButton), true);

        /// <summary>
        /// Gets or sets a value indicating whether the button's image is visible. This is a bindable property.
        /// </summary>
        /// <value><c>true</c> if the image is visible; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
		public bool ImageVisible
        {
            get { return (bool)GetValue(ImageVisibleProperty); }
            set { SetValue(ImageVisibleProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>VerticalContentAlignment</c> bindable property.
        /// </summary>
        public static readonly BindableProperty VerticalContentAlignmentProperty =
            BindableProperty.Create(nameof(VerticalContentAlignment), typeof(ButtonVerticalContentAlignment), typeof(ExtendedButton), ButtonVerticalContentAlignment.Center);

        /// <summary>
        /// Gets or sets the vertcial alignment of the button's content. This is a bindable property.
        /// </summary>
        /// <value>A <c>ButtonVerticalContentAlignment</c> value that represents the vertical content alignment of the button. 
        /// The default value is <c>ButtonVerticalContentAlignment.Center</c>.</value>
        public ButtonVerticalContentAlignment VerticalContentAlignment
        {
            get { return (ButtonVerticalContentAlignment)GetValue(VerticalContentAlignmentProperty); }
            set { SetValue(VerticalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>HorizontalContentAlignment</c> bindable property.
        /// </summary>
        public static readonly BindableProperty HorizontalContentAlignmentProperty =
            BindableProperty.Create(nameof(HorizontalContentAlignment), typeof(ButtonHorizontalContentAlignment), typeof(ExtendedButton), ButtonHorizontalContentAlignment.Center);

        /// <summary>
        /// Gets or sets the horizontal alignment of the button's content. This is a bindable property.
        /// </summary>
        /// <value>A <c>ButtonHorizontalContentAlignment</c> value that represents the horizontal content alignment of the button. 
        /// The default value is <c>ButtonHorizontalContentAlignment.Center</c>.</value>
        public ButtonHorizontalContentAlignment HorizontalContentAlignment
        {
            get { return (ButtonHorizontalContentAlignment)GetValue(HorizontalContentAlignmentProperty); }
            set { SetValue(HorizontalContentAlignmentProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>ContentPadding</c> bindable property.
        /// </summary>
        public static readonly BindableProperty ContentPaddingProperty =
            BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(ExtendedButton), new Thickness());

        /// <summary>
        /// Gets or sets the padding of the button's content. This is a bindable property.
        /// </summary>
        /// <value>A <c>Thickness</c> value that represents the padding of buttons content; the default is a Thickness of 0. 
        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        /// <summary>
        /// Backing store for the <c>ImageHorizontalOffset</c> bindable property.
        /// </summary>
        public static readonly BindableProperty ImageHorizontalOffsetProperty =
            BindableProperty.Create(nameof(ImageHorizontalOffset), typeof(int), typeof(ExtendedButton), 5);

        /// <summary>
        /// Gets or sets the horizontal offset of the button's image. This is a bindable property.
        /// </summary>
        /// <value>The horizontal offset of the button's image; the default is 5.</value>
		public int ImageHorizontalOffset
        {
            get { return (int)GetValue(ImageHorizontalOffsetProperty); }
            set { SetValue(ImageHorizontalOffsetProperty, value); }
        }


       

  //       /// <summary>
  //       /// Delegate for the <c>OnTouchesMoved</c> event.
  //       /// </summary>
  //       /// <param name = "forceTouchPercentage" > Represents the amount of force the user applied
		// /// through the touch and is expressed as percentage relative to the maximum force allowed.</param>
		// public delegate void TouchedMovedEventHandler(float forceTouchPercentage);
  //
  //       /// <summary>
  //       /// Event that is raised in response to the iOS <see cref="MonoTouch.UIKit.UIResponder.TouchesMoved"/> event.
  //       /// </summary>
  //       public event TouchedMovedEventHandler OnTouchesMoved;
  //
  //       /// <summary>
  //       /// Method that raises the <c>OnTouchesMoved</c> event.
  //       /// </summary>
  //       /// <param name="forceTouchPercentage">Represents the amount of force the user applied 
  //       /// through the touch and is expressed as percentage relative to the maximum force allowed.</param>
  //       public void InvokeTouchesMovedEvent(float forceTouchPercentage)
  //       {
  //           OnTouchesMoved?.Invoke(forceTouchPercentage);
  //       }
  //
  //       /// <summary>
  //       /// Delegate for the <c>OnTouchesBegan</c> event.
  //       /// </summary>
  //       public delegate void TouchesBeganEventHandler();
  //
  //       /// <summary>
  //       /// Event that is raised in response to the iOS <see cref="UIKit.UIResponder.TouchesBegan(Foundation.NSSet, UIKit.UIEvent)"/> event.
  //       /// </summary>
  //       public event TouchesBeganEventHandler OnTouchesBegan;
  //
  //       /// <summary>
  //       /// Method that raises the <c>OnTouchesBegan</c> event.
  //       /// </summary>
  //       public void InvokeTouchesBeganEvent()
  //       {
  //           OnTouchesBegan?.Invoke();
  //       }
  //
  //       /// <summary>
  //       /// Delegate for the <c>OnTouchesEnded</c> event.
  //       /// </summary>
  //       public delegate void TouchesEndedEventHandler(bool invokedFromForceTouch);
  //
  //       /// <summary>
  //       /// Event that is raised in response to the iOS <see cref="UIKit.UIResponder.TouchesEnded(Foundation.NSSet, UIKit.UIEvent)"/> event.
  //       /// </summary>
  //       public event TouchesEndedEventHandler OnTouchesEnded;
  //
  //       /// <summary>
  //       /// Method that raises the <c>OnTouchesEnded</c> event.
  //       /// </summary>
  //       /// <param name="invokedFromForceTouch">If set to <c>true</c> the method is invoked from force touch.</param>
  //       public void InvokeTouchesEndedEvent(bool invokedFromForceTouch)
  //       {
  //           OnTouchesEnded?.Invoke(invokedFromForceTouch);
  //       }
  //
  //       /// <summary>
  //       /// Delegate for the <c>OnTouchesCancelled</c> event.
  //       /// </summary>
  //       public delegate void TouchesCancelledEventHandler();
  //
  //       /// <summary>
  //       /// Event that is raised in response to the iOS <see cref=UIKit.UIResponder.TouchesCancelled(Foundation.NSSet, UIKit.UIEvent)"/> event.
  //       /// </summary>
  //       public event TouchesCancelledEventHandler OnTouchesCancelled;
  //
  //       /// <summary>
  //       /// Method that raises when the <c>OnTouchesCancelled</c> event occurs.
  //       /// </summary>
  //       public void InvokeTouchesCancelledEvent()
  //       {
  //           OnTouchesCancelled?.Invoke();
  //       }
  //
  //       /// <summary>
  //       /// Delegate for the <c>OnDidLayoutSubviews</c> event.
  //       /// </summary>
  //       public delegate void ControlDidLayoutSubviewsEventHandler();
  //
  //       /// <summary>
  //       /// Event that is raised in response to the iOS <see cref="UIKit.UIView.LayoutSubviews"/> event.
  //       /// </summary>
  //       public event ControlDidLayoutSubviewsEventHandler OnDidLayoutSubviews;

        // /// <summary>
        // /// Method that raises the <c>OnDidLayoutSubviews</c> event.
        // /// </summary>
        // public void InvokeSubviewsDidLayoutEvent()
        // {
        //     OnDidLayoutSubviews?.Invoke();
        // }
    }

    public class ExtendedButtonEx : ExtendedButton
    {
        public ExtendedButtonEx()
        {
            HeightRequest = 60; // Device.Idiom == TargetIdiom.Phone ? 60 : 100;
            CustomFont = TalkiPlay.Fonts.ButtonTextFontBold; //  Device.Idiom == TargetIdiom.Phone ? TalkiPlay.Fonts.ButtonTextFontBold : TalkiPlay.Fonts.ButtonTextFontBoldBig;
            CornerRadius = 0;
            BorderWidth = 1d;
            BorderColor = Color.White;            
            BackgroundColor = TalkiPlay.Shared.Colors.Blue5Color;
        }
    }
}

