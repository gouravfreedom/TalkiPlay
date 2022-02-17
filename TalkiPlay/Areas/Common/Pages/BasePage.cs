using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using ChilliSource.Mobile.UI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;

namespace TalkiPlay
{
    // public enum ToolbarButtonPosition
    // {
    //     Left = -1,
    //     Right = 0
    // }

    public interface IBasePageController
    {
        void OnBackButtonTapped();
        void OnLoaded();
        void OnAppeared();
        void OnDisposing();
        void OnWillDisappear();
        Color NavigationBarBackgroundColor { get; }
        Color TintColor { get; }
        //ExtendedFont NavigationTitleFont { get; }
        //bool RightToolbarItemVisible { get; }
        //bool LeftToolbarItemVisible { get; }
        //ExtendedFont RightToolbarItemFont { get; }
        //ExtendedFont LeftToolbarItemFont { get; }
        //bool IsTransparentNavBar { get; }
        //bool ShowNavigationBar { get; }
        //void AdjustSafeAreaForToolbar(double safeAreaTop);
        bool IsAppearing { get; }
    }
    
    public abstract class BasePage<T> : ReactiveContentPage<T>, IBasePageController where T : class 
    {
        private bool _isAppearing;
        protected BasePage()
        {
            this.On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(false);
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
            //NavigationPage.SetBackButtonTitle(this, "Back");
            //BackgroundImageSource = Images.GetEmbeddedPngImage(Images.HuntBg);
            BackgroundImageSource = SimpleBasePage.BackImageSource;
        }
        
        public ICommand BackButtonPressed { get; set; }

        public void OnBackButtonTapped()
        {
            var canExecute = BackButtonPressed?.CanExecute(null) ?? false;
            if (canExecute)
            {
                BackButtonPressed?.Execute(null);
            } 
        }

         public event EventHandler ViewDidLoad;
         public event EventHandler ViewDidAppear;
         public event EventHandler ViewIsDisposing;
         public event EventHandler ViewWillDisappear;

//
          public Color NavigationBarBackgroundColor { get; set; } = Color.Transparent;
          public Color TintColor { get; set; } = Color.White;
          public ExtendedFont NavigationTitleFont { get; set;  }
//
//         private bool _isTransparentNavBar;
//
//         /// <summary>
//         /// Determines whether the navigation bar is transparent.
//         /// </summary>
//         public bool IsTransparentNavBar
//         {
//             get => _isTransparentNavBar;
//             set
//             {
//                 _isTransparentNavBar = value;
// #if __IOS__
//                 ShowNavigationBar = false;
// #endif
//             }
//         }
//
//         private bool _showNavigationBar = true;
//         public bool ShowNavigationBar
//         {
//             get => _showNavigationBar;
//             set => NavigationPage.SetHasNavigationBar(this, value);
//         }
//
//         /// <summary>
//         /// Backing store for the <see cref="RightToolbarItemFont"/> bindable property.
//         /// </summary>
//         public static BindableProperty RightToolbarItemFontProperty =
//             BindableProperty.Create(nameof(RightToolbarItemFont), typeof(ExtendedFont), typeof(ContentPage));
//
//         /// <summary>
//         /// Gets or sets the font for the right toolbar item. This is a bindable property.
//         /// </summary>
//         public ExtendedFont RightToolbarItemFont
//         {
//             get => (ExtendedFont)GetValue(RightToolbarItemFontProperty);
//             set => SetValue(RightToolbarItemFontProperty, value);
//         }
//
//         /// <summary>
//         /// Identifies the <see cref="RightToolbarItemVisible"/> bindable property.
//         /// </summary>
//         public static BindableProperty RightToolbarItemVisibleProperty =
//             BindableProperty.Create(nameof(RightToolbarItemVisible), typeof(bool), typeof(ContentPage),  true);
//
//         /// <summary>
//         /// Gets or sets a value indicating whether the right toolbar item is visible. This is a bindable property.
//         /// </summary>
//         public bool RightToolbarItemVisible
//         {
//             get => (bool)GetValue(RightToolbarItemVisibleProperty);
//             set => SetValue(RightToolbarItemVisibleProperty, value);
//         }
//
//         /// <summary>
//         /// Backing store for the <see cref="LeftToolbarItemFont"/> bindable property.
//         /// </summary>
//         public static BindableProperty LeftToolbarItemFontProperty =
//             BindableProperty.Create(nameof(LeftToolbarItemFont), typeof(ExtendedFont), typeof(ContentPage));
//
//         /// <summary>
//         /// Gets or sets the font for the left toolbar item. This is a bindable property.
//         /// </summary>
//         public ExtendedFont LeftToolbarItemFont
//         {
//             get => (ExtendedFont)GetValue(LeftToolbarItemFontProperty);
//             set => SetValue(LeftToolbarItemFontProperty, value);
//         }
//
//         /// <summary>
//         ///  Identifies the <see cref="LeftToolbarItemVisible"/> bindable property.
//         /// </summary>
//         public static BindableProperty LeftToolbarItemVisibleProperty =
//             BindableProperty.Create(nameof(LeftToolbarItemVisible), typeof(bool), typeof(ContentPage), true);
//
//         /// <summary>
//         /// Gets or sets a value indicating whether the left toolbar item is visible. This is a bindable property.
//         /// </summary>
//         public bool LeftToolbarItemVisible
//         {
//             get => (bool)GetValue(LeftToolbarItemVisibleProperty);
//             set => SetValue(LeftToolbarItemVisibleProperty, value);
//         }

       

         public virtual void OnLoaded()
         {
             ViewDidLoad?.Invoke(this, new EventArgs());
            OnAppeared();
         }
        
         public virtual void OnAppeared()
         {
             ViewDidAppear?.Invoke(this, new EventArgs());
         }
        
         public virtual void OnDisposing()
         {
             ViewIsDisposing?.Invoke(this, new EventArgs());
         }
        
         public virtual void OnWillDisappear()
         {
             ViewWillDisappear?.Invoke(this, new EventArgs());
         }
        
         public virtual void AdjustSafeAreaForToolbar(double safeAreaTop)
         {
             
         }
        
         public bool IsAppearing => _isAppearing;
        
        
         protected override void OnAppearing()
         {
             _isAppearing = true;
             base.OnAppearing();
         }
        
         protected override void OnDisappearing()
         {
             _isAppearing = false;
             base.OnDisappearing();
         }
    }
}
