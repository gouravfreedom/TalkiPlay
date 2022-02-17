using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.Api;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public enum GameSteps
    {
        ScanHomeToStart,
        ScanHomeToEnd,
        ResultFromDeviceToServer,
        TapToCrackEgg,
        EggCracking,
        NextGameSuggestion,
        Failed
    }

    public class TestGamePageViewModel : BasePageViewModel, IActivatableViewModel
    {
        private readonly IApi<ITalkiPlayApi> _api;

        public TestGamePageViewModel()
        {
            //CurrentStep = GameSteps.NextGameSuggestion;

            _api = Locator.Current.GetService<IApi<ITalkiPlayApi>>();

            var service = Locator.Current.GetService<IApplicationService>();
            EggHeight = service.ScreenSize.Height * 0.35;

            GetGame().Forget();
        }

        public override string Title => "";
        public ViewModelActivator Activator { get; }
        public double EggHeight { get; set;}


        private GameSteps _currentStep;
        public GameSteps CurrentStep
        {
            get => _currentStep;
            set
            {
                switch (value)
                {
                    case GameSteps.ScanHomeToStart:
                        Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler)
                        .Do((m) => CurrentStep = GameSteps.ScanHomeToEnd)
                        .Subscribe();
                        break;
                    case GameSteps.ScanHomeToEnd:
                        Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler)
                            .Do((m) => CurrentStep = GameSteps.ResultFromDeviceToServer)
                            .Subscribe();
                        break;
                    case GameSteps.ResultFromDeviceToServer:
                        Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler)
                            .Do(async(m) => {
                                await GetGame();
                                CurrentStep = GameSteps.NextGameSuggestion;                                
                            }).Subscribe();
                        break;
                    case GameSteps.NextGameSuggestion:
                        Observable.Timer(TimeSpan.FromSeconds(5), RxApp.MainThreadScheduler)
                            .Do((m) => CurrentStep = GameSteps.ScanHomeToStart)
                            .Subscribe();
                        break;

                }
                this.RaiseAndSetIfChanged(ref _currentStep, value);
            }
        }

        [Reactive]
        public IGame NextGame { get; private set; }
        [Reactive]
        public string NextGameImagePath { get; private set; }

        private async Task GetGame()
        {
            var games = await _api.Client.GetGames();
            NextGame = games.FirstOrDefault();
        }
    }
}
