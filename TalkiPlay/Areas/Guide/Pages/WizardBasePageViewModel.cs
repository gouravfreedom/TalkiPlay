using System;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class WizardBasePageViewModel : SimpleBasePageModel
    {

        public WizardBasePageViewModel(GuideStep step, GuideState state)
        {
            Step = step;
            State = state;
            HeaderText = GuideHelper.GetHeaderTextForStep(step);
            HeaderTextFontSize = GuideHelper.GetHeaderTextFontSizeForStep(step);
            
            SetupCommands();
        }
        
        public string Title => "";

        [Reactive]
        public bool ShowNavBar { get; set; } = true;

        //public Action<bool> Callback { get; }
        public GuideStep Step { get; }
        public GuideState State { get; }
        
        public string HeaderText { get; set; }
        
        public int HeaderTextFontSize { get; }
        
        public bool ShowNextButton { get; set; } = true;
        public string NextButtonText { get; set; } = "Next";
        
        
        public ICommand NextCommand { get; set; }        
        public ICommand BackCommand { get; set; }

        void SetupCommands()
        {
            SyncNavView();

            NextCommand = new Command(() =>
            {
                SyncNavView();
                if (GuideHelper.IsLastStep(Step))
                {
                    GuideHelper.EndGuide(State);
                }
                else
                {
                    var nextStep = GuideHelper.GetNextStep(Step);
                    var vm = GuideHelper.GetStepViewModel(nextStep, State);
                    SimpleNavigationService.PushAsync(vm).Forget();      
                }
            });
            
            BackCommand = new Command(() =>
            {
                SyncNavView();

                if (GuideHelper.IsFirstStep(State, Step) && State.IsModal)
                {
                    SimpleNavigationService.PopModalAsync().Forget();
                }
                else
                {
                    SimpleNavigationService.PopAsync().Forget();    
                }
                
            });
        }

        private void SyncNavView()
        {
            ShowNavBar = !GuideHelper.IsFirstStep(State, Step) || State.EnableBackAtStart;
        }
    }
}