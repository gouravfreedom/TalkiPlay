using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using ChilliSource.Mobile.Core;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public enum GuideStep
    {
        Welcome,
        Personalisation,
        ChildSelection,
        CommunicationQuestion,
        RequestResponseQuestion,
        PrimaryLanguageQuestion,
        // MotorSkillsQuestion,
        ChildLedLearningInfo,
        //InterestsQuestion,
        PackQuestion,
        Recommendation,
        TapToPlay,
        Conclusion
    }
    
    public enum GuideInterests
    {
        Animals,
        Vehicles,
        Food,
        HouseItems,
        Rainbows,
        Dinosaurs,
        Blocks,
        Planets
    }

    public static class GuideHelper
    {
        public static string GetHeaderTextForStep(GuideStep step)
        {
            switch (step)
            {
                case GuideStep.Welcome: return "Hi, I am your learning guide";
                case GuideStep.Personalisation:
                    return
                        "I will ask a few questions to help me personalise TalkiPlay for your child";
                case GuideStep.ChildSelection: return "Which child is playing?";
                case GuideStep.CommunicationQuestion:
                    return "Generally, how does your child communicate?";
                case GuideStep.RequestResponseQuestion:
                    return
                        "How would your child respond to the question \"Please sit at the table\"?";
                 case GuideStep.PrimaryLanguageQuestion:
                     return "English is my child's primary language";
                //case GuideStep.MotorSkillsQuestion: return "How would you describe your child's motor skills?";
                case GuideStep.ChildLedLearningInfo: return "TalkiPlay encourages child led learning";
                case GuideStep.PackQuestion: return "What is your child interested in this week?"; 
                case GuideStep.Recommendation:
                    return "We recommend playing the following games at least 3 times a week";
                case GuideStep.TapToPlay:
                    return "";
                default: return "";
            }
        }

        public static int GetHeaderTextFontSizeForStep(GuideStep step)
        {
            switch (step)
            {
                case GuideStep.Personalisation: return 28;
                case GuideStep.Recommendation: return 30;
                default: return 34;
            }
        }
        
        public static int GetBodyTextFontSizeForStep(GuideStep step)
        {
            switch (step)
            {
                case GuideStep.ChildLedLearningInfo: return 28;
                default: return 34;
            }
        }
        
        public static string GetImageSourceForStep(GuideStep step)
        {
            switch (step)
            {
                case GuideStep.Welcome: return Images.GetSvgImage("ob_01_thisisyourtalkiplayer");
                case GuideStep.Personalisation: return Images.GetPngImage("guide_personalisation");
                case GuideStep.ChildLedLearningInfo: return Images.GetPngImage("guide_child_led_play");
                case GuideStep.TapToPlay: return Images.GetPngImage("logo");
                default: return "";
            }
        }

        public static string GetBodyTextForStep(GuideStep step, GuideState state)
        {
            switch (step)
            {
                
                case GuideStep.ChildLedLearningInfo:
                    return "Children are happier and retain more knowledge when they learn about things that interest them!";
                case GuideStep.TapToPlay:
                    return $"Find this icon to\nupdate {state.SelectedChild.Name}'s\nprogress\n \n \n ";
                case GuideStep.Conclusion:
                    return "Play is a powerful way for children to learn.";
                default: return "";
            }
        }

        public static Type GetAnswersForStep(GuideStep step)
        {
            switch (step)
            {
                case GuideStep.CommunicationQuestion: return typeof(ChildCommunicationLevel);
                case GuideStep.RequestResponseQuestion: return typeof(ChildResponseLevel);
                case GuideStep.PrimaryLanguageQuestion: return typeof(ChildLanguageLevel);
                default: return null;
            }
        }

        public static GuideStep GetNextStep(GuideStep currentStep)
        {
            int step = (int) currentStep + 1;
            return (GuideStep) step;
        }

        public static WizardBasePageViewModel GetStepViewModel(GuideStep currentStep, GuideState state)
        {
            
            switch (currentStep)
            {
                case GuideStep.Welcome:
                    return new GuideImagePageViewModel(currentStep, state);
                case GuideStep.Personalisation:
                    return new GuideImagePageViewModel(currentStep, state);
                case GuideStep.ChildSelection:
                {
                    //if children.count > 1
                    return new GuideChildSelectionPageViewModel(currentStep, state);
                }
                case GuideStep.CommunicationQuestion:
                    return new GuideQuestionPageViewModel(currentStep, state);
                case GuideStep.RequestResponseQuestion:
                    return new GuideQuestionPageViewModel(currentStep, state);
                case GuideStep.PrimaryLanguageQuestion:
                    return new GuideQuestionPageViewModel(currentStep, state);
                case GuideStep.ChildLedLearningInfo:
                    return new GuideInfoPageViewModel(currentStep, state);
                // case GuideStep.InterestsQuestion:
                //     return new GuideInterestsPageViewModel(currentStep, state);
                 case GuideStep.PackQuestion:
                     return new GuidePackSelectionPageViewModel(currentStep, state);
                case GuideStep.Recommendation:
                    return new GuideRecommendationPageViewModel(currentStep, state);
                case GuideStep.TapToPlay:
                    return new GuideInfoPageViewModel(currentStep, state);
                case GuideStep.Conclusion:
                    return new GuideInfoPageViewModel(currentStep, state);
                default: return null;
            }
        }
        
        public static bool IsLastStep(GuideStep currentStep)
        {
            return currentStep == GuideStep.Conclusion;
        }
        
        public static bool IsFirstStep(GuideState state, GuideStep currentStep)
        {
            return currentStep == state.StartStep;
        }
        
        public static void StartGuide()
        {
           var state = new GuideState() { IsModal = true };
           var vm = GetStepViewModel(state.StartStep, state);
           SimpleNavigationService.PushModalAsync(vm).Forget();
        }

        public static async void EndGuide(GuideState state)
        {
            var settings = Locator.Current.GetService<IUserSettings>();
            try
            {
                var repository = Locator.Current.GetService<IChildrenRepository>();
                var result = await repository.AddOrUpdateChild(state.SelectedChild);
                settings.IsGuideCompleted = true;
            }
            catch (Exception e)
            {
                e.ShowExceptionDialog();
            }
           
            MessageBus.Current.SendMessage(new GameRecommendationChangedMessage());
            if (state.ShouldRlaceMainPage)
            {
                settings.CurrentChild = state.SelectedChild;
                Locator.Current.GetService<ITalkiPlayNavigator>().NavigateToTabbedPage();
            }
            else if (state.IsModal)
            {
                SimpleNavigationService.PopModalAsync().Forget();
            }
            else
            {
                SimpleNavigationService.PopToRootAsync().Forget();
            }
        }
    }
}