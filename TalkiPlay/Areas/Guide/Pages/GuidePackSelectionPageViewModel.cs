using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class GuidePackSelectionPageViewModel : WizardBasePageViewModel
    {
        private IList<PackDto> _packs;
        public GuidePackSelectionPageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            LoadData().Forget();
        }
        
        //public List<GuidePackViewModel> Items { get; private set; }
        
        public List<List<GuidePackViewModel>> Items { get; private set; }

        async Task LoadData()
        {
            Dialogs.ShowLoading();
        
            try
            {
                var repository = Locator.Current.GetService<IAssetRepository>();
                var games = await Locator.Current.GetService<IGameService>().GetGames();
                var packs = await repository.GetPacks();
                _packs = packs?.Where(p => !p.Name.StartsWith("Smart") && games.Any(a => a.PackId == p.Id))?.ToList();
                Items = new List<List<GuidePackViewModel>>();

                var colCount = 2;
                if (_packs != null && _packs.Count > 0)
                {
                    int rowCount = (int)Math.Ceiling(_packs.Count / (double)colCount);
                    for (int i = 0; i < rowCount; i++)
                    {
                        var list = new List<GuidePackViewModel>();
                        for (int j = 0; j < Math.Min(colCount, _packs.Count - i*colCount); ++j)
                        {
                            var index = j + i * colCount;
                            var pack = _packs[index];
                            list.Add(new GuidePackViewModel(pack.Id, pack.Name, pack.ImagePath, HandlePackSelection));
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
        //         var repository = Locator.Current.GetService<IAssetRepository>();
        //         _packs = await repository.GetPacks();
        //         Items = new List<GuidePackViewModel>();
        //
        //         if (_packs != null)
        //         {
        //             foreach (var pack in _packs.Where(p => !p.Name.StartsWith("Smart")))
        //             {
        //                 Items.Add(new GuidePackViewModel(pack.Id, pack.Name, pack.ImagePath,
        //                     HandlePackSelection));
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

        void HandlePackSelection(int id)
        {
            State.SelectedPack = _packs.FirstOrDefault(p => p.Id == id);
            State.SelectedChild.FavouritePackId = State.SelectedPack?.Id ?? 0;
            NextCommand.Execute(null);
        }
    }
}