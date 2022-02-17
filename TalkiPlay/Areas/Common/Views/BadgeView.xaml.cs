using ChilliSource.Mobile.UI;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class BadgeView : Grid
    {
        public static BindableProperty BadgeTextFontProperty = BindableProperty.Create(nameof(BadgeTextFont),
            typeof(ExtendedFont), typeof(BadgeView), Fonts.BadgeTextFont, propertyChanged: (bindable, oldVal, newVal) =>
            {
                var view = (BadgeView) bindable;
                view.BadgeLabel.CustomFont = (ExtendedFont) newVal;
            });

        public static BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(BadgeView), "0", propertyChanged: (bindable, oldVal, newVal) =>
            {
                var view = (BadgeView) bindable;
                view.BadgeLabel.Text = (string) newVal;
            });

        public static BindableProperty BadgeColorProperty = BindableProperty.Create(nameof(BadgeColor), typeof(Color),
            typeof(BadgeView), Color.Black, propertyChanged: (bindable, oldVal, newVal) =>
            {
                var view = (BadgeView) bindable;
                view.BadgeCircle.BackgroundColor = (Color) newVal;
            });

        public BadgeView()
        {
            InitializeComponent();
            BadgeLabel.Text = Text;
            BadgeLabel.CustomFont = BadgeTextFont;
            BadgeCircle.BackgroundColor = BadgeColor;
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color BadgeColor
        {
            get => (Color) GetValue(BadgeColorProperty);
            set => SetValue(BadgeColorProperty, value);
        }

        public ExtendedFont BadgeTextFont
        {
            get => (ExtendedFont) GetValue(BadgeTextFontProperty);
            set => SetValue(BadgeTextFontProperty, value);
        }
    }
}