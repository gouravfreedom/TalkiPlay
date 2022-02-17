﻿using System;
namespace TalkiPlay.Shared
{
    public enum QRCodeOnboardingStep
    {
        Welcome,
        Intro,        
        QRCodeSheets,
        AttachTalkies,
        SelectGame,
        SelectChild,
        ToggleItems,
        ExploreGame,
        HuntGame,        
        //Signup,
        ChildName,
        ChildDOB,
        ChildAvatar,
    }

    public class QRCodeOnboardingState
    {
        public QRCodeOnboardingState()
        {
        }
        public ChildDto Child { get; set; }
    }

    public static class QRCodeOnboardingHelper
    {
        public static string GetHeaderTextForStep(QRCodeOnboardingStep step)
        {
            switch(step)
            {
                case QRCodeOnboardingStep.Welcome: return "Welcome to TalkiPlay";
                case QRCodeOnboardingStep.Intro: return "TalkiPlay brings real objects to life to talk, sing and play games";                
                case QRCodeOnboardingStep.QRCodeSheets: return "Download & print talkie sheets";
                case QRCodeOnboardingStep.AttachTalkies: return "Cut out & attach to real objects";
                case QRCodeOnboardingStep.SelectGame: return "Select a game to play";
                case QRCodeOnboardingStep.SelectChild: return "In a game select a child";
                case QRCodeOnboardingStep.ToggleItems: return "Turn off objects you don’t have";
                case QRCodeOnboardingStep.ExploreGame: return "Practice words! With Explore games";
                case QRCodeOnboardingStep.HuntGame: return "Practice following instructions! With Hunt games";
                case QRCodeOnboardingStep.ChildName: return "Enter your child’s name";
                case QRCodeOnboardingStep.ChildDOB: return "When was {0} born?";
                case QRCodeOnboardingStep.ChildAvatar: return "Choose an avatar";                
                default:return "";

            }
        }

        public static string GetImageForStep(QRCodeOnboardingStep step)
        {
            switch (step)
            {
                case QRCodeOnboardingStep.Welcome: return Images.GetSvgImage("ob_01_thisisyourtalkiplayer");
                case QRCodeOnboardingStep.Intro: return Images.GetSvgImage("ob_02_talking_rainbow");
                case QRCodeOnboardingStep.QRCodeSheets: return Images.GetSvgImage("ob_03_download");
                case QRCodeOnboardingStep.AttachTalkies: return Images.GetSvgImage("ob_04_cut_out");
                case QRCodeOnboardingStep.SelectGame: return Images.GetSvgImage("ob_05_phone_game");
                case QRCodeOnboardingStep.SelectChild: return Images.GetSvgImage("ob_06_add_child");
                case QRCodeOnboardingStep.ToggleItems: return Images.GetSvgImage("ob_07_toggle_items");
                case QRCodeOnboardingStep.ExploreGame: return Images.GetSvgImage("ob_08_explore_game");
                case QRCodeOnboardingStep.HuntGame: return Images.GetSvgImage("ob_09_hunt_game");                
                default: return "";

            }
        }

        public static QRCodeOnboardingStep GetNextStep(QRCodeOnboardingStep currentStep)
        {            
            int step = (int)currentStep + 1;
            return (QRCodeOnboardingStep)step;            
        }

        public static QRCodeOnboardingStep GetPreviousStep(QRCodeOnboardingStep currentStep)
        {
            int step = (int)currentStep - 1;
            return (QRCodeOnboardingStep)step;
        }

        public static bool IsFirstStep(QRCodeOnboardingStep currentStep)
        {
            return currentStep == QRCodeOnboardingStep.Welcome;
        }

        public static bool IsLastStep(QRCodeOnboardingStep currentStep)
        {
            return currentStep == QRCodeOnboardingStep.ChildAvatar;
        }

        public static object GetNextOnboardingViewModel(QRCodeOnboardingStep currentStep, QRCodeOnboardingState state = null)
        {                        
            var nextStep = GetNextStep(currentStep);
            return GetOnboardingVMForStep(nextStep, state);
        }

        public static object GetOnboardingVMForStep(QRCodeOnboardingStep step, QRCodeOnboardingState state = null)
        {
            switch (step)
            {
                case QRCodeOnboardingStep.Welcome: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.Intro: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.QRCodeSheets: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.AttachTalkies: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.SelectGame: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.SelectChild: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.ToggleItems: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.ExploreGame: return new OnboardingImagePageViewModel(step, state);
                case QRCodeOnboardingStep.HuntGame: return new OnboardingImagePageViewModel(step, state); //return new SignupPageViewModel(SignupNavigationSource.ModeSelection);
                //case QRCodeOnboardingStep.Signup: return new OnboardingChildNamePageViewModel(nextStep);
                case QRCodeOnboardingStep.ChildName: return new OnboardingChildNamePageViewModel(step, state);
                case QRCodeOnboardingStep.ChildDOB: return new OnboardingChildDOBPageViewModel(step, state);
                case QRCodeOnboardingStep.ChildAvatar: return new OnboardingChildAvatarPageViewModel(step, state);
            }
            return null;
        }
    }
}
