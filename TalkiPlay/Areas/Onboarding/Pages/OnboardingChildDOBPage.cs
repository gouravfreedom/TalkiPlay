using System;
using ChilliSource.Mobile.UI;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class OnboardingChildDOBPage : SimpleBasePage<OnboardingChildDOBPageViewModel>
    {        
        public OnboardingChildDOBPage()
        {           
            BuildContent();           
        }
       
        void BuildContent()
        {
            var barHeight = DeviceInfo.StatusbarHeight;
            var navHeight = DeviceInfo.NavBarHeight;
            var totalHeight = barHeight + navHeight;
            var bottomOffset = DeviceInfo.GetSafeAreaInsets().Bottom;


            var navView = new NavigationView
            {
                BarTintColor = Color.Transparent,
                ShowLeftButton = true,
                
                ShowRightButton = false,
                Title = "",
                
                Padding = Dimensions.NavPadding(barHeight)
            };

            navView.SetBinding(NavigationView.IsVisibleProperty, nameof(ViewModel.ShowNavBar));
            navView.LeftButton.SetBinding(ImageButtonView.CommandProperty, nameof(ViewModel.BackCommand));            

           
            var label = new ExtendedLabel
            {                
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            label.SetBinding(Label.TextProperty, nameof(ViewModel.HeaderText));
                       
            var datePicker = new ExtendedDatePicker
            {
                BackgroundColor = Color.White,
                TextColor = Color.Black,
                MaximumDate = DateTime.Today,
                MinimumDate = DateTime.MinValue,                    
                HasBorder = false,
            };
            
            datePicker.Effects.Add(new BorderlessEffect());
            
            if (Device.RuntimePlatform == Device.Android)
            {
                datePicker.Margin = new Thickness(2,0,2,0);
            }
            else
            {
                datePicker.Margin = new Thickness(10,0,0,0);;
            }
            
            datePicker.SetBinding(DatePicker.DateProperty, nameof(ViewModel.DateOfBirthValue), mode: BindingMode.TwoWay);
            
            var dobLayout = BuildValidationLayout(datePicker, "Birthdate", nameof(ViewModel.DateOfBirthIsValid), nameof(ViewModel.DateOfBirthErrors));


            var nextButton = new Button
            {
                Style = Styles.PrimaryButtonStyle,
                Text = "Next",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(15,0),
            };
            nextButton.SetBinding(Button.CommandProperty, nameof(OnboardingChildDOBPageViewModel.NextCommand));

            var buttonLayout = new StackLayout
            {
                Children = { nextButton },
                Margin = new Thickness(0, 0, 0, 20 + bottomOffset)
            };

            var layout = new StackLayout
            {
                Spacing = 40,
                Margin = 20,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {label, dobLayout }
            };
            
            var grid = new Grid()
            {
                RowSpacing = 0,
            };
            grid.RowDefinitions.Add(new RowDefinition() { Height = totalHeight });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var gradient = Styles.BuildMainGradient();
            grid.Children.Add(gradient);
            Grid.SetRowSpan(gradient, 3);

            grid.Children.Add(navView);
            grid.Children.Add(layout, 0, 1);
            grid.Children.Add(buttonLayout, 0, 2);
            
            Content = grid;
        }

        StackLayout BuildValidationLayout(View view, string labelText, string validBinding, string errorsBinding)
        {
            var label = new ExtendedLabel
            {
                Text = labelText,
                CustomFont = Fonts.MetaLinkWhiteFont,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            var frame = new Frame
            {
                HeightRequest = 60,
                Padding = new Thickness(10,0,0,0),
                HasShadow = false,
                CornerRadius = 4,
                BorderColor = Color.Transparent,
                Content = view
            };
            
            var height = Dimensions.GetFormTextFieldHeight();
            var padding = Dimensions.GetFormTextFieldPadding();
            frame.HeightRequest = height;
            frame.Padding = padding;
            
            frame.SetBinding(Frame.BorderColorProperty, validBinding, converter: new IsValidToColorConverter());

            var validationView = new ValidationErrorsView
            {
                ValidationMessageFont = Fonts.MetaErrorFont
            };
            validationView.SetBinding(ValidationErrorsView.ItemsSourceProperty, errorsBinding);

            var layout = new StackLayout
            {
                Children = { label, frame, validationView }
            };

            return layout;
        }

    }
}

