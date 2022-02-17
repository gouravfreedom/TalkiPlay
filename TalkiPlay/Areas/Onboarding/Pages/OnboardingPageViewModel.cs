using System;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Linq;

namespace TalkiPlay.Shared
{
    public class OnboardingPageViewModel : SimpleBasePageModel
    {
        readonly IUserSettings _userSettings;
        readonly int _totalCount = 0;
        int _position;

        static readonly string[] _onboardingHeaderTexts = new[]
        {
            "Welcome to\n",
            "Use TalkiPlay for\n active learning\nwith your young\nones",
            "Personalize and\ntrack your\nlearning",
            "Use real objects\n to enhance the learning\nexperience",
            ""
        };

        static readonly string[] _onboardingSubHeaderTexts = new[]
        {
            "We're excited\n you've joined\n\n Let's Talk & Play!",
            "",
            "",
            "",
            ""
        };
        private static readonly string[] _onboardingImages = new[]
        {
            Images.LogoWhite,
            Images.ObActiveLearn,
            Images.ObPersonal,
            Images.ObRealObjects,
            Images.ObTalkiGuyWelcome,
            Images.ObShakeMeTapMe,
            Images.ObTagMe,
            Images.SvgResources + ".roomselection.svg",
            Images.SvgResources + ".selectagame.svg",
            Images.SvgResources + ".letsplay.svg",
        };

        public OnboardingPageViewModel(
            IUserSettings userSettings = null)
        {
            _userSettings = userSettings ?? Locator.Current.GetService<IUserSettings>();

            SetupCommands();

            Items = new List<OnboardingItemViewModel>();

            var startIndex = _userSettings.HasTalkiPlayerDevice ? 0 : 7;
            _totalCount = _onboardingHeaderTexts.Length - startIndex;
            var scalemap = new Dictionary<int, double> { {1, 1.3 }, { 3, 1.3} };

            for (var i = startIndex; i < _onboardingHeaderTexts.Length; i++)
            {
                double resScale = 1d;
                if (!scalemap.TryGetValue(i, out resScale))
                {
                    resScale = 1;
                }

                var itemModel = new OnboardingItemViewModel()
                {
                    Heading = _onboardingHeaderTexts[i],
                    SubHeading = _onboardingSubHeaderTexts[i],
                    ResourceScale = resScale
                };
                itemModel.Resource = _onboardingImages[i];
                Items.Add(itemModel);
            }
        }

        public Action<int> ScrollCarouselViewToPosition;

        public string Title => "";

        public List<OnboardingItemViewModel> Items { get; private set; }

        public int Position
        {
            get => _position;
            set
            {
                _position = value;
            }
        }

        public ICommand ItemChangeCommand { get; private set; }

        public ICommand BackCommand { get; private set; }


        void SetupCommands()
        {
            BackCommand = new Command(() => SimpleNavigationService.PopModalAsync().Forget());
        }

        public async Task GotoNextState()
        {
            var userSettings = Locator.Current.GetService<IUserSettings>();
            var navigatorHelper = Locator.Current.GetService<ITalkiPlayNavigator>();
            if (userSettings.IsOnboarded)
            {
                navigatorHelper.NavigateToTabbedPage();
            }
            else
            {
                var childService = Locator.Current.GetService<IChildrenRepository>();
                var children = await childService.GetChildren();
                var firstChild = children.FirstOrDefault();
                if (firstChild != null)
                {
                    if (userSettings.CurrentChild == null)
                    {
                        userSettings.CurrentChild = firstChild;
                    }

                    var deviceMgr = Locator.Current.GetService<ITalkiPlayerManager>();
                    if (deviceMgr.Current != null)
                    {
                        navigatorHelper.NavigateToTabbedPage();
                    }
                    else
                    {
                        SimpleNavigationService.PushAsync(new DeviceSetupPageViewModel(DeviceSetupSource.Onboard)).Forget();
                    }
                }
                else
                {
                    SimpleNavigationService.PushAsync(new AddEditChildPageViewModel(false)).Forget();
                }
            }
        }
    }
}