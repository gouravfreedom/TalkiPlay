
namespace TalkiPlay.Shared
{
    public class GuideInfoPageViewModel : WizardBasePageViewModel
    {
        public GuideInfoPageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            ImageSource = GuideHelper.GetImageSourceForStep(step);
            BodyText = GuideHelper.GetBodyTextForStep(step, state);
            
            BodyTextFontSize = GuideHelper.GetBodyTextFontSizeForStep(step);
            
            if (step == GuideStep.Conclusion)
            {
                NextButtonText = "Let's play!";
                ShowNextButton = false;
                ShowImageButton = true;
            }
        }

        public bool ShowImageButton { get; set; }
        
        public int BodyTextFontSize { get; }
        
        public string ImageSource { get; }

        public bool HasImage => !string.IsNullOrEmpty(ImageSource);
        
        public string BodyText { get; set; }
    }
}