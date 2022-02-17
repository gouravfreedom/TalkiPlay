using ChilliSource.Mobile.UI;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class EmptyStateView : ContentView
    {
        private ExtendedLabel _label;
        
        public EmptyStateView()
        {
            BuildContent();
        }

        // public static BindableProperty ButtonTextProperty = BindableProperty.Create(nameof(ButtonText), typeof(string), typeof(EmptyStateView), propertyChanged:OnButtonTextChanged);
        //
        // private static void OnButtonTextChanged(BindableObject bindable, object oldValue, object newValue)
        // {
        //     if (bindable is EmptyStateView parent && newValue != null)
        //     {
        //         parent._button.Text = (string)newValue;
        //     }
        // }

        // public string ButtonText
        // {
        //     get { return (string)GetValue(ButtonTextProperty); }
        //     set { SetValue(ButtonTextProperty, value); }
        // }

        
        public static BindableProperty EmptyStateTextProperty =
            BindableProperty.Create(nameof(EmptyStateText), typeof(string), typeof(EmptyStateView), 
                propertyChanged:OnEmptyStateTextPropertyChanged);

        private static void OnEmptyStateTextPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is EmptyStateView parent && newvalue != null)
            {
                parent._label.Text = (string)newvalue;
            }
        }

        public string EmptyStateText    
        {
            get => (string) GetValue(EmptyStateTextProperty);
            set => SetValue(EmptyStateTextProperty, value);
        }
        
        
        void BuildContent()
        {
            _label = new ExtendedLabel
            {
                Margin = 40,
                Text = "",
                CustomFont = Fonts.BodyWhiteFont,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };
            
            var emptyView = new StackLayout
            {
                Margin = 40,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.Center,
                Children = { _label}
            };

            Content = emptyView;
        }
    }
}