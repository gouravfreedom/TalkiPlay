using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuideChildSelectionPageViewModel : WizardBasePageViewModel
    {
        //private IList<IChild> _children;
        public GuideChildSelectionPageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            LoadData().Forget();
        }
        
        //public List<GuideChildViewModel> Items { get; private set; }
        
        public List<List<GuideChildViewModel>> Items { get; private set; }

        async Task LoadData()
        {
            Dialogs.ShowLoading();
        
            try
            {
                var repository = Locator.Current.GetService<IChildrenRepository>();
                var children = await repository.GetChildren();
                Items = new List<List<GuideChildViewModel>>();

                var colCount = 2;
                if (children != null && children.Count > 0)
                {
                    int rowCount = (int)Math.Ceiling(children.Count / (double)colCount);
                    for (int i = 0; i < rowCount; i++)
                    {
                        var list = new List<GuideChildViewModel>();
                        for (int j = 0; j < Math.Min(colCount, children.Count - i*colCount); ++j)
                        {
                            var index = j + i * colCount;
                            list.Add(new GuideChildViewModel(children[index], HandleChildSelection));
                        }
                        Items.Add(list);
                    }
                }
            
                RaisePropertyChanged(nameof(Items));
                Dialogs.HideLoading();
            }
            catch (Exception e)
            {
                Dialogs.HideLoading();
                e.ShowExceptionDialog();
            }
        }

        //TODO: re-enable once https://github.com/xamarin/Xamarin.Forms/issues/9125 is fixed
        // async Task LoadData()
        // {
        //     Dialogs.ShowLoading();
        //
        //     try
        //     {
        //         var repository = Locator.Current.GetService<IChildrenRepository>();
        //         var children = await repository.GetChildren();
        //         Items = new List<GuideChildViewModel>();
        //
        //         if (children != null)
        //         {
        //             foreach (var child in children)
        //             {
        //                 Items.Add(new GuideChildViewModel(child, HandleChildSelection));
        //             }
        //         }
        //     
        //         RaisePropertyChanged(nameof(Items));
        //         Dialogs.HideLoading();
        //     }
        //     catch (Exception e)
        //     {
        //         Dialogs.HideLoading();
        //         e.ShowExceptionDialog();
        //     }
        // }
        
        void HandleChildSelection(IChild child)
        {
            State.SelectedChild = child;
            Locator.Current.GetService<IUserSettings>().CurrentChild = child;
            NextCommand.Execute(null);
        }
    }
}