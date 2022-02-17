using System.ComponentModel;
using ChilliSource.Mobile.UI.ReactiveUI.iOS;
//using ChilliSource.Mobile.UI.ReactiveUI.iOS;
using TalkiPlay;
using TalkiPlay.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(IBasePageController), typeof(BasePageRenderer))]
namespace TalkiPlay.iOS
{
    public class BasePageRenderer : HackedPageRenderer
    {
        private IBasePageController BasePageController => Element is NavigationPage ? (Element as NavigationPage).RootPage as IBasePageController : Element as IBasePageController;
        private ContentPage Page => Element as ContentPage;
        //private UILabel _titleLabel;        

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            BasePageController?.OnLoaded();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                BasePageController?.OnDisposing();
            }

            base.Dispose(disposing);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            this.Page.PropertyChanged += PageOnPropertyChanged;

            //var transparent = BasePageController?.IsTransparentNavBar ?? false;
       
            //if (transparent)
            //{
            //    SetupTransparentNavigationBar(Page.ToolbarItems);
            //    return;
            //}

            //SetNavigationBarBackgroundColor();
            //ArrangeNavigationBarButtons();
        }
          
        private void PageOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            // if (propertyChangedEventArgs.PropertyName.Equals(nameof(Page.Title)))
            // {
            //     //if(_titleLabel != null)
            //     //{
            //     //    _titleLabel.Text = Page.Title;
            //     //}
            //     //else
            //     {
            //         Title = Page.Title;
            //     }
            // }
        }

        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            BasePageController?.OnWillDisappear();
            this.Page.PropertyChanged -= PageOnPropertyChanged;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            BasePageController?.OnAppeared();
        }

        #region Helpers

        // private void SetNavigationBarBackgroundColor()
        // {
        //     var navigationColor = BasePageController?.NavigationBarBackgroundColor ?? Color.Transparent;
        //     if (navigationColor == Color.Transparent) { return; }
        //     if (NavigationController?.NavigationBar == null) { return; }
        //     NavigationController.NavigationBar.BarTintColor = navigationColor.ToUIColor();
        //     NavigationController.NavigationBar.TintColor = BasePageController?.TintColor.ToUIColor();
        // }
        
        #endregion
    }
}
