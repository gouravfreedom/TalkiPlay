using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuideImagePageViewModel : WizardBasePageViewModel
    {
        public GuideImagePageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            ImageSource = GuideHelper.GetImageSourceForStep(step);
            NextCommand = new Command(() => GoNext().Forget());
        }
        public string ImageSource { get; }
        
        async Task GoNext()
        {
            var nextStep = GuideHelper.GetNextStep(Step);
            
            if (Step == GuideStep.Personalisation)
            {
                Dialogs.ShowLoading();

                try
                {
                    var repository = Locator.Current.GetService<IChildrenRepository>();
                    var children = await repository.GetChildren();

                    Dialogs.HideLoading();
                
                    if (children.Count == 1)
                    {
                        State.SelectedChild = children.First();
                        nextStep = GuideHelper.GetNextStep(nextStep);
                    }
                }
                catch (Exception e)
                {
                    Dialogs.HideLoading();
                    e.ShowExceptionDialog();
                }
            }
            
            var vm = GuideHelper.GetStepViewModel(nextStep, State);
            SimpleNavigationService.PushAsync(vm).Forget();
        }
        
    }
}