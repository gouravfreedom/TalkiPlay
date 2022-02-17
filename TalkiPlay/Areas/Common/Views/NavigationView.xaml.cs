using ChilliSource.Mobile.UI;
using System.Windows.Input;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class NavigationView : Grid
    {
        public NavigationView()
        {
            InitializeComponent();

			BarTintColor = Shared.Colors.NavColor1;
			TitleFont = Fonts.NavigationTitleFont;
			LeftButtonIcon = Shared.Images.ArrowBackIcon;
		}

		public Color BarTintColor
		{
			get => this.BackgroundColor;
			set => this.BackgroundColor = value;
		}

		public static BindableProperty TitleProperty =
			BindableProperty.Create(nameof(Title), typeof(string), typeof(NavigationView), propertyChanged:OnTitleChanged);

		private static void OnTitleChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (bindable is NavigationView parent)
			{
				parent.NavTitle.Text = (string)newvalue;
			}
		}

		public string Title
		{
			get { return (string) GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		
		
		public ExtendedFont TitleFont
		{
			get => this.NavTitle.CustomFont;
			set => NavTitle.CustomFont = value;
		}

		public ImageButtonView LeftButton => LeftNavButton;
		public ImageButtonView RightButton => RightNavButton;

		public bool ShowLeftButton
		{
			get => (bool) LeftNavButton.IsVisible;
			set => LeftNavButton.IsVisible = value;
		}

		public bool ShowRightButton
		{
			get => (bool) RightNavButton.IsVisible;
			set => RightNavButton.IsVisible = value;
		}

		//[TypeConverter(typeof (ImageSourceConverter))]
		public string LeftButtonIcon
		{
			get => this.LeftNavButton.DefaultSource;
			set => LeftNavButton.DefaultSource = value;
		}

		

		//[TypeConverter(typeof (ImageSourceConverter))]
		public string RightButtonIcon
		{
			get => RightNavButton.DefaultSource;
			set => RightNavButton.DefaultSource = value;
		}


		public ICommand LeftButtonCommand
		{
			get { return (ICommand)GetValue(LeftButtonCommandProperty); }
			set { SetValue(LeftButtonCommandProperty, value); }
		}
		public static BindableProperty LeftButtonCommandProperty =
			BindableProperty.Create(nameof(LeftButtonCommand), typeof(ICommand), typeof(NavigationView), propertyChanged: OnLeftButtonCommandChanged);

		private static void OnLeftButtonCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (bindable is NavigationView view)
			{
				view.LeftNavButton.Command = (ICommand)newvalue;
			}
		}

		public ICommand RightButtonCommand
		{
			get { return (ICommand)GetValue(RightButtonCommandProperty); }
			set { SetValue(RightButtonCommandProperty, value); }
		}
		public static BindableProperty RightButtonCommandProperty =
			BindableProperty.Create(nameof(RightButtonCommand), typeof(ICommand), typeof(NavigationView), propertyChanged: OnRightButtonCommandChanged);

		private static void OnRightButtonCommandChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			if (bindable is NavigationView view)
			{
				view.RightNavButton.Command = (ICommand)newvalue;
			}
		}
	}

}
