using System;
using FormsControls.Base;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class SimpleBasePage : ContentPage, IAnimationPage
    {
        private static ImageSource _backSource;
        public static ImageSource BackImageSource
        {
            get
            {
                if (_backSource == null)
                {
                    //_backSource = ImageSource.FromFile(Images.HuntBg);
                    _backSource = ImageResizer.ResizeImageToFitDevice(Images.PngName + ".hunt_bg.png");
                }
                return _backSource;
            }
        }

        private static ImageSource _gameBackSource;
        public static ImageSource GameBackImageSource
        {
            get
            {
                if (_gameBackSource == null)
                {
                    //_backSource = ImageSource.FromFile(Images.HuntBg);
                    _gameBackSource = ImageResizer.ResizeImageToFitDevice(Images.PngName + ".game_bg.png");
                }
                return _gameBackSource;
            }
        }

        public SimpleBasePage()
        {
            BackgroundImageSource = BackImageSource;
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
        }

        public delegate void PageEventDelegate();
        public event PageEventDelegate ViewDidLoad;

        public void RaiseViewDidLoadEvent()
        {
            ViewDidLoad?.Invoke();
        }

        protected override bool OnBackButtonPressed()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
        
        public void OnAnimationStarted(bool isPopAnimation)
        {

        }

        public void OnAnimationFinished(bool isPopAnimation)
        {

        }

        public virtual IPageAnimation PageAnimation => new SlidePageAnimation()
        {
            BounceEffect = false,
            Subtype = AnimationSubtype.FromRight,
            Duration = AnimationDuration.Short
        };
    }

    public class SimpleBasePage<T> : SimpleBasePage
    {
        protected static T ViewModelType { get; }
        public T ViewModel => (T)BindingContext;
    }
}
