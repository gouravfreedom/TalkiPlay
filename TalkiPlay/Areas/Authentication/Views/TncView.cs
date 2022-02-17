using System;
using ChilliSource.Mobile.UI;
using TalkiPlay.Managers;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class TncView : StackLayout
    {        
        Image _checkButton;
        public TncView()
        {
            Spacing = 2;
            BuildContent();
        }

        public static BindableProperty HasAgreedProperty = BindableProperty.Create(nameof(HasAgreed), typeof(bool), typeof(TncView));
        public bool HasAgreed
        {
            get { return (bool)GetValue(HasAgreedProperty); }
            set { SetValue(HasAgreedProperty, value); }
        }

        void BuildContent()
        {

            // _checkButton = new ExtendedIconLabel
            // {
            //     Icon = Icons.SquareIcon,
            //     VerticalOptions = LayoutOptions.Center,                
            // };
            _checkButton = new Image
            {
                Source = Icons.SquareIcon,
                VerticalOptions = LayoutOptions.Center,                
            };
            _checkButton.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(ToggleSelection)
            });

            var tncLabel = new ExtendedLabel
            {
                Text = "I have read and agree to the",
                CustomFont = new ExtendedFont(Fonts.FontFamilyRegular, size: 16, color: Colors.WhiteColor),
                VerticalOptions = LayoutOptions.Center
            };

            var subLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 10,
                Children = { _checkButton, tncLabel }
            };


            var tncLinkLabel = new ExtendedLabel
            {
                Text = "Terms of Use",
                CustomFont = Fonts.LinkWhiteFont,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            tncLinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    WebpageHelper.OpenUrl(Config.TermsUrl, "Terms of Use");
                })
            });

            var andLabel = new ExtendedLabel()
            {
                Text = " and ",
                CustomFont = new ExtendedFont(Fonts.FontFamilyRegular, size: 16, color: Colors.WhiteColor),
            };
            
            var privacyPolicyLinkLabel = new ExtendedLabel
            {
                Text = "Privacy Policy.",
                CustomFont = Fonts.LinkWhiteFont,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            privacyPolicyLinkLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    WebpageHelper.OpenUrl(Config.PrivacyPolicyUrl, "Privacy Policy");
                })
            });

            var linksLayout = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 0,
                Children = {tncLinkLabel, andLabel, privacyPolicyLinkLabel}
            };
            
            Children.Add(subLayout);
            Children.Add(linksLayout);            

        }

        void ToggleSelection()
        {
            HasAgreed = !HasAgreed;
            if (HasAgreed)
            {
                _checkButton.Source = Icons.CheckedSquareIcon;
                //_checkButton.Icon = Icons.CheckedSquareIcon;
            }
            else
            {
                _checkButton.Source = Icons.SquareIcon;
                //_checkButton.Icon = Icons.SquareIcon;
            }

        }
    }

    
}
