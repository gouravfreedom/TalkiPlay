using System;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using TalkiPlay;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public enum NavBarLeftButtonType
    {
        None,
        Back,
        Close
    }

    public enum NavBarRightButtonType
    {
        None,
        Add,
        Guide,
    }

    public class SimpleNavBar : Grid
    {
        ImageButtonView _leftImageButton;
        ImageButtonView _rightImageButton;
        ExtendedLabel _titleLabel;

        public SimpleNavBar()
        {
            BuildContent();
        }

        #region Properties

        public static readonly BindableProperty LeftButtonCommandProperty =
          BindableProperty.Create(nameof(LeftButtonCommand), typeof(ICommand), typeof(SimpleNavBar), default(ICommand));

        public ICommand LeftButtonCommand
        {
            get => (ICommand)GetValue(LeftButtonCommandProperty);
            set => SetValue(LeftButtonCommandProperty, value);
        }
                
        public static BindableProperty LeftButtonTypeProperty = BindableProperty.Create(nameof(LeftButtonType), typeof(NavBarLeftButtonType), typeof(SimpleNavBar), NavBarLeftButtonType.None, propertyChanged:OnLeftButtonTypePropertyChanged);

        private static void OnLeftButtonTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var parent = ((SimpleNavBar)bindable);
            var imageButton = parent._leftImageButton;
            if (imageButton != null && newValue != null)
            {
                parent.SetLeftButtonType(imageButton, (NavBarLeftButtonType)newValue);
            }
        }

        public NavBarLeftButtonType LeftButtonType
        {
            get { return (NavBarLeftButtonType)GetValue(LeftButtonTypeProperty); }
            set { SetValue(LeftButtonTypeProperty, value); }
        }

        public static readonly BindableProperty RightButtonCommandProperty =
          BindableProperty.Create(nameof(RightButtonCommand), typeof(ICommand), typeof(SimpleNavBar), default(ICommand));

        public ICommand RightButtonCommand
        {
            get => (ICommand)GetValue(RightButtonCommandProperty);
            set => SetValue(RightButtonCommandProperty, value);
        }

        public static BindableProperty RightButtonTypeProperty = BindableProperty.Create(nameof(RightButtonType), typeof(NavBarRightButtonType), typeof(SimpleNavBar), NavBarRightButtonType.None, propertyChanged: OnRightButtonTypePropertyChanged);

        private static void OnRightButtonTypePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var parent = ((SimpleNavBar)bindable);            
            if (newValue != null)
            {
                parent.SetRightButtonType((NavBarRightButtonType)newValue);
            }
        }

        public NavBarRightButtonType RightButtonType
        {
            get { return (NavBarRightButtonType)GetValue(RightButtonTypeProperty); }
            set { SetValue(RightButtonTypeProperty, value); }
        }



        public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(SimpleNavBar), default(string), propertyChanged: OnTitlePropertyChanged);

        private static void OnTitlePropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var label = ((SimpleNavBar)bindable)._titleLabel;
            if (label != null)
            {
                label.Text = newvalue as string;
            }
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        void SetLeftButtonType(ImageButtonView imageButton, NavBarLeftButtonType type)
        {
            switch (type)
            {
                case NavBarLeftButtonType.Back:
                    {
                        imageButton.DefaultSource = Images.ArrowBackIcon;
                        break;
                    }
                case NavBarLeftButtonType.Close:
                    {
                        imageButton.DefaultSource = Images.ArrowBackIcon;
                        break;
                    }
                case NavBarLeftButtonType.None:
                    {
                        imageButton.DefaultSource = null;
                        break;
                    }
            }
        }

        void SetRightButtonType(NavBarRightButtonType type)
        {
            switch (type)
            {
                case NavBarRightButtonType.Add:
                {
                    _rightImageButton.DefaultSource = Images.AddIcon;
                        break;
                    }
                case NavBarRightButtonType.Guide:
                    {
                        _rightImageButton.DefaultSource = Images.LogoButtonIcon;
                        break;
                    }
                case NavBarRightButtonType.None:
                    {
                        break;
                    }
            }
        }

        void BuildContent()
        {            
            Padding = new Thickness(Dimensions.DefaultHorizontalMargin, 0);

            ColumnDefinitions.Add(new ColumnDefinition {Width = 50});
            ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Star});
            ColumnDefinitions.Add(new ColumnDefinition {Width = 50});
            RowDefinitions.Add(new RowDefinition {Height = Dimensions.DefaultNavBarHeight});

            _leftImageButton = new ImageButtonView
            {
                HorizontalOptions = LayoutOptions.Start,
            };

            _leftImageButton.OnButtonTapped += (sender, args) => LeftButtonCommand?.Execute(null);

            Children.Add(_leftImageButton, 0, 0);
                                              
            _titleLabel = new ExtendedLabel
            {                            
                CustomFont = Fonts.NavigationTitleFont,
                LineBreakMode = LineBreakMode.TailTruncation,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(10,0),                
            };
                                    
            Children.Add(_titleLabel,1,0);
            
            _rightImageButton = new ImageButtonView
            {
                HorizontalOptions = LayoutOptions.End,                
                IsVisible = false
            };

            _rightImageButton.OnButtonTapped += (sender, args) => RightButtonCommand?.Execute(null);
            
            Children.Add(_rightImageButton, 2, 0);
        }
    }
}
