using System;
using ChilliSource.Mobile.UI;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class OnboardingChildNamePage : SimpleBasePage<OnboardingChildNamePageViewModel>
    {
        public OnboardingChildNamePage()
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

            var headerLabel = new ExtendedLabel
            {
                CustomFont = Fonts.Header1Font,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            headerLabel.SetBinding(Label.TextProperty, nameof(ViewModel.HeaderText));


            var firstNameEntry = new ExtendedEntry
            {
                CustomFont = Fonts.BodyFont,
                CustomPlaceholderFont = Fonts.BodyPlaceHolderFont,
                Placeholder = "Child's first name...",
                Keyboard = Keyboard.Text,
                ReturnType = ReturnType.Next,
                KeyboardReturnType = KeyboardReturnKeyType.Next,
                HorizontalContentPadding = 10,
                ErrorBackgroundColor = Color.White,
                HasBorder = false,
            };
            firstNameEntry.Effects.Add(new BorderlessEffect());
            
            firstNameEntry.SetBinding(Entry.ReturnCommandProperty, nameof(ViewModel.NextCommand));

            firstNameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.FirstNameText), BindingMode.TwoWay);
            firstNameEntry.SetBinding(ExtendedEntry.IsValidProperty, nameof(ViewModel.FirstNameIsValid));

            var firstNameLayout = BuildEntryLayout(firstNameEntry, "First name", nameof(ViewModel.FirstNameIsValid), nameof(ViewModel.FirstNameErrors));
            
            var mainLayout = new StackLayout
            {
                Margin = 15,
                Spacing = 40,
                VerticalOptions = LayoutOptions.Center,
                Children = { headerLabel, firstNameLayout }
            };

            var scrollView = new ScrollView
            {
                Content = mainLayout
            };

            var nextButton = new Button
            {
                Style = Styles.PrimaryButtonStyle,
                Text = "Next",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(15, 0),
            };
            nextButton.SetBinding(Button.CommandProperty, nameof(OnboardingImagePageViewModel.NextCommand));
            var buttonLayout = new StackLayout
            {
                Children = { nextButton },
                Margin = new Thickness(0, 0, 0, 20 + bottomOffset)
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
            grid.Children.Add(scrollView, 0, 1);
            grid.Children.Add(buttonLayout, 0, 2);

            Content = grid;

        }

        StackLayout BuildEntryLayout(ExtendedEntry entry, string labelText, string validBinding, string errorsBinding)
        {
            var label = new ExtendedLabel
            {
                Text = labelText,
                CustomFont = Fonts.MetaLinkWhiteFont,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
           
            var frame = new Frame
            {
                HasShadow = false,
                CornerRadius = 4,
                BorderColor = Color.Transparent,
                Content = entry
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
                Children = {label, frame, validationView}
            };

            return layout;
        }

    }
}