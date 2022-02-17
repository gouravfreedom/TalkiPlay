using System;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI;
using EasyLayout.Forms;
using Lottie.Forms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
	public class AnimatedButton : ContentView
	{
		protected RelativeLayout _layout;
		protected ExtendedButton _button;
		protected AnimationView _indicator;
		protected bool _isExecuted;
		protected Rectangle _orignalSize;

		public AnimatedButton()
		{
			BuildContent();
		}

		private void BuildContent()
		{
			_layout = new RelativeLayout(); try
			{
				_button = new ExtendedButton()
				{
					CornerRadius = CornerRadius,
					CustomFont = ButtonFont,
					BackgroundColor = ButtonColor,
					DisabledBackgroundColor = Colors.WarmGrey,
					PressedBackgroundColor = Colors.TealColor,
					IsCustomButton = false,
					Text = " ",
					TextColor = Color.White,
					BorderWidth = this.BorderWidth,
				};
			}
			catch(Exception exc)
            {
				throw exc;
            }

			_indicator = new AnimationView
			{
				IsPlaying = true,
				Loop = true,
				Opacity = 0,
				IsVisible = false,
				WidthRequest = AnimationWidthRequest,
				HeightRequest = AnimationHeightRequest
			};

			_layout.ConstrainLayout(() =>
				_button.CenterX() == _layout.CenterX() &&
				_button.Width() == _layout.Width()
			);

			_layout.ConstrainLayout(() =>
				_indicator.CenterX() == _layout.CenterX() &&
				_indicator.CenterY() == _layout.CenterY()
			);

			Content = _layout;
		}


		public readonly static BindableProperty ContentPaddingProperty = BindableProperty.Create(nameof(ContentPadding), typeof(Thickness),
			typeof(AnimatedButton), null, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.ContentPadding = (Thickness)newValue;
				}

			});

		public Thickness ContentPadding
		{
			get => (Thickness)GetValue(ContentPaddingProperty);
			set => SetValue(ContentPaddingProperty, value);
		}

		public readonly static BindableProperty VerticalContentAlignmentProperty = BindableProperty.Create(nameof(VerticalContentAlignment), typeof(ButtonVerticalContentAlignment),
            typeof(AnimatedButton), ButtonVerticalContentAlignment.Center, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.VerticalContentAlignment = (ButtonVerticalContentAlignment)newValue;
				}

			});

		public ButtonVerticalContentAlignment VerticalContentAlignment
		{
			get => (ButtonVerticalContentAlignment)GetValue(VerticalContentAlignmentProperty);
			set => SetValue(VerticalContentAlignmentProperty, value);
		}



		public readonly static BindableProperty ButtonFontProperty = BindableProperty.Create(nameof(ButtonFont), typeof(ExtendedFont), typeof(AnimatedButton), Fonts.ButtonTextFont, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.CustomFont = (ExtendedFont)newValue;
				}

			});

		public ExtendedFont ButtonFont
		{
			get => (ExtendedFont)GetValue(ButtonFontProperty);
			set => SetValue(ButtonFontProperty, value);
		}


		public readonly static BindableProperty ButtonColorProperty = BindableProperty.Create(nameof(ButtonColor), typeof(Color), typeof(AnimatedButton), Colors.WarmGrey, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.BackgroundColor = (Color)newValue;
				}

			});

		public Color ButtonColor
		{
			get => (Color)GetValue(ButtonColorProperty);
			set => SetValue(ButtonColorProperty, value);
		}

		public readonly static BindableProperty BusyButtonColorProperty = BindableProperty.Create(nameof(BusyButtonColor), typeof(Color), typeof(AnimatedButton), Colors.WhiteColor);

		public Color BusyButtonColor
		{
			get => (Color)GetValue(BusyButtonColorProperty);
			set => SetValue(BusyButtonColorProperty, value);
		}

		public readonly static BindableProperty BorderWidthProperty = BindableProperty.Create(nameof(BorderWidth), typeof(double), typeof(AnimatedButton), defaultValue: 0d, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.BorderWidth = (double)newValue;
				}

			});

		public double BorderWidth
		{
			get => (double)GetValue(BorderWidthProperty);
			set => SetValue(BorderWidthProperty, value);
		}

		public readonly static BindableProperty BorderColorProperty = BindableProperty.Create(nameof(BorderColor), typeof(Color), typeof(AnimatedButton), defaultValue: Color.Transparent, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.BorderColor = (Color)newValue;
				}

			});

		public Color BorderColor
		{
			get => (Color)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public readonly static BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(AnimatedButton), 28, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.CornerRadius = (int)newValue;
				}

			});

		public int CornerRadius
		{
			get => (int)GetValue(CornerRadiusProperty);
			set => SetValue(CornerRadiusProperty, value);
		}

		public readonly static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(AnimatedButton), defaultValue:" ", propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._button.Text = (string)newValue ?? " ";
				}

			});

		public string Text
		{
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public readonly static BindableProperty AnimationProperty = BindableProperty.Create(nameof(Animation), typeof(string), typeof(AnimatedButton), propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._indicator.Animation = (string)newValue;
				}

			});

		public string Animation
		{
			get => (string)GetValue(AnimationProperty);
			set => SetValue(AnimationProperty, value);
		}

		public readonly static BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool), typeof(AnimatedButton), false, propertyChanged:
			async (bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					var isBusy = (bool)newValue;

					if (isBusy)
					{
						await view.StartAnimation();
					}
					else
					{
						await view.StopAnimation();
					}
				}

			});

		public bool IsBusy
		{
			get => (bool)GetValue(IsBusyProperty);
			set => SetValue(IsBusyProperty, value);
		}

		public double ButtonWidthRequest
		{
			get => _button.WidthRequest;
			set => _button.WidthRequest = value;
		}

		public double ButtonHeightRequest
		{
			get => _button.HeightRequest;
			set => _button.HeightRequest = value;
		}

		public readonly static BindableProperty AnimationWidthRequestProperty = BindableProperty.Create(nameof(AnimationWidthRequest), typeof(double), typeof(AnimatedButton), -1.0, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._indicator.WidthRequest = (double)newValue;
				}

			});


		public double AnimationWidthRequest
		{
			get => (double)GetValue(AnimationWidthRequestProperty);
			set => SetValue(AnimationWidthRequestProperty, value);
		}

		public readonly static BindableProperty AnimationHeightRequestProperty = BindableProperty.Create(nameof(AnimationHeightRequest), typeof(double), typeof(AnimatedButton), -1.0, propertyChanged:
			(bindable, value, newValue) =>
			{
				if (bindable is AnimatedButton view)
				{
					view._indicator.HeightRequest = (double)newValue;
				}

			});

		public double AnimationHeightRequest
		{
			get => (double)GetValue(AnimationHeightRequestProperty);
			set => SetValue(AnimationHeightRequestProperty, value);
		}

		public ExtendedButton Button => _button;

		public Color PressedBackgroundColor
		{
			get => _button.PressedBackgroundColor;
			set => _button.PressedBackgroundColor = value;
		}

		public Color DisabledBackgroundColor
		{
			get => _button.DisabledBackgroundColor;
			set => _button.DisabledBackgroundColor = value;
		}

		public bool IsCustomButton
		{
			get => _button.IsCustomButton;
			set => _button.IsCustomButton = value;
		}

		public bool ShouldFadeToButton { get; set; }

		private async Task StartAnimation()
		{
			_orignalSize = _button.Bounds;
			this._indicator.IsVisible = true;
			this._button.Text = "";
			_isExecuted = true;
			await Task.WhenAll(
				this._button.ChangeColorAnimated(ButtonColor, BusyButtonColor,
					color => { _button.BackgroundColor = color; }, 250, Easing.Linear),
				this._button.LayoutTo(
					new Rectangle(_layout.Bounds.X + (this._layout.Width / 2) - (60 / 2),
						this._button.Bounds.Y,
						60, 60
					),
					250,
					Easing.Linear
				),
				ShouldFadeToButton ? this._button.FadeTo(0.0, 350, Easing.CubicOut) : Task.Delay(0),
				this._indicator.FadeTo(1.0, 250, Easing.Linear)
			);
		}

		private async Task StopAnimation()
		{
			if (_isExecuted)
			{
				_isExecuted = false;
				await Task.WhenAll(
					_button.ChangeColorAnimated(BusyButtonColor, ButtonColor, color => { _button.BackgroundColor = color; }, 250, Easing.Linear),
					_button.LayoutTo(
						new Rectangle(_orignalSize.X,
							_orignalSize.Y,
							_orignalSize.Width,
							_orignalSize.Height
						),
						250,
						Easing.Linear
					),
					ShouldFadeToButton ? this._button.FadeTo(1.0, 350, Easing.CubicOut) : Task.Delay(0),
					this._indicator.FadeTo(0.0, 250, Easing.Linear)
				);

				this._button.Text = Text;
				this._indicator.IsVisible = false;
			}

		}
	}

	public class AnimatedButtonEx : AnimatedButton
    {
		public AnimatedButtonEx()
        {
			ButtonHeightRequest = 60; // Device.Idiom == TargetIdiom.Phone ? 60 : 100;
			ButtonFont = Device.Idiom == TargetIdiom.Phone ? TalkiPlay.Fonts.ButtonTextFontBold : TalkiPlay.Fonts.ButtonTextFontBoldBig;
			BorderWidth = 1; _button.BorderWidth = 1;
			BorderColor = Color.White; ; _button.BorderColor = Color.White;
			CornerRadius = 15; _button.CornerRadius = 15;
			ButtonColor = Colors.TealBlueColor; _button.BackgroundColor = Colors.TealBlueColor;

			
			AnimationWidthRequest = 80; _indicator.WidthRequest = 80;
			AnimationHeightRequest = 80; _indicator.HeightRequest = 80;
			Animation = Images.LoadingAnimation;
			BusyButtonColor = Colors.WarmGrey;
		}
    }
}

