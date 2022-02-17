using System;
using System.Collections.Generic;
using System.Linq;

namespace TalkiPlay.Shared
{
    public class GuideQuestionPageViewModel : WizardBasePageViewModel
    {
        
        public GuideQuestionPageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            LoadData();
        }

        public List<ButtonViewModel> Items { get; private set; }

        void LoadData()
        {
            var type = GuideHelper.GetAnswersForStep(Step);
            
            if (type != null)
            {
                var values = Enum.GetValues(type).Cast<Enum>();

                Items = new List<ButtonViewModel>();
                foreach (var item in values)
                {
                    Items.Add(new ButtonViewModel(item, HandleCallback));
                }
            }
        }

        void HandleCallback(Enum item)
        {
            switch (Step)
            {
               
                case GuideStep.CommunicationQuestion:
                    State.SelectedChild.CommunicationLevel = (ChildCommunicationLevel) item;
                    break;
                case GuideStep.RequestResponseQuestion:
                    State.SelectedChild.ResponseLevel = (ChildResponseLevel) item;
                    break;
                case GuideStep.PrimaryLanguageQuestion:
                    State.SelectedChild.LanguageLevel = (ChildLanguageLevel) item;
                    break;
                default:
                    break;
            }
            
            NextCommand.Execute(null);
        }
    }
}