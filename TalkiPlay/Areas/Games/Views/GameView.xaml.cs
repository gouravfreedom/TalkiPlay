using System;
using System.Reactive.Disposables;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.XamForms;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public partial class GameView : ReactiveContentView<GameViewModel>
    {
        //readonly TapGestureRecognizer _tapGesture;

        public GameView()
        {
            InitializeComponent();

            var tapGesture = new TapGestureRecognizer()
            {
                Command = new Command(() =>
                {
                    if (ViewModel.IsLocked)
                    {
                        //Dialogs.Alert("This game requires a subscription.", "Games");
                        SimpleNavigationService.PushModalAsync(new SubscriptionListPageViewModel()).Forget();
                    }
                    else
                    {
                        this.ViewModel?.SelectCommand?.Execute().SubscribeSafe();
                    }
                    //AnimationView.Play();
                })
            };
            //SubLayout.GestureRecognizers.Add(tapGesture);
            
           BackgroundImage.HeightRequest = Device.Idiom == TargetIdiom.Tablet ? 500 : 200;
           BackgroundImage.GestureRecognizers.Add(tapGesture);
            
            
            // var recognizer = AddTouch.GetRecognizer(SubLayout);
        
           //recognizer.TouchEnd += (sender, e) =>
           //{
           //     if (Device.RuntimePlatform == Device.Android)
           //     {
           //         this.ViewModel?.SelectCommand?.Execute().SubscribeSafe();
           //     }
           // };

           // if (Device.RuntimePlatform == Device.iOS)
           // {
           //     _tapGesture = new TapGestureRecognizer();
           //     MainLayout.GestureRecognizers.Add(_tapGesture);
           // }

            // this.WhenActivated(d =>
            // {
            //     this.OneWayBind(ViewModel, v => v.Title, view => view.Title.Text).DisposeWith(d);
            //     this.OneWayBind(ViewModel, v => v.ShortDescription, view => view.ShortDescription.Text).DisposeWith(d);
            //     this.OneWayBind(ViewModel, v => v.GameType, view => view.GameType.Text).DisposeWith(d);
            //     this.OneWayBind(ViewModel, v => v.Description, view => view.Description.Text).DisposeWith(d);
            //     //this.BindCommand(ViewModel, v => v.SelectCommand, view => view._tapGesture).DisposeWith(d);
            //
            // });
        }
    }
}
