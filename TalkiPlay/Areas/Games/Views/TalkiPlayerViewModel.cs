using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Plugin.BluetoothLE;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public abstract class TalkiPlayerBaseViewModel : ReactiveObject
    {
        public abstract string Name { get; set; }
    }
    
    public class TalkiPlayerViewModel : TalkiPlayerBaseViewModel
    {
        public TalkiPlayerViewModel(ITalkiPlayerData player,
            ReactiveCommand<ITalkiPlayerData, Unit> removeCommand)
        {
            Name = player?.Name;
            RemoveCommand = ReactiveCommand.CreateFromObservable(() => removeCommand.Execute(player));
            RemoveCommand.ThrownExceptions.SubscribeAndLogException();
        }
        
        [Reactive] public override string Name { get; set; }
        
        public ReactiveCommand<Unit, Unit> RemoveCommand { get; }


        
    }
    
    public class SelectTalkiPlayerViewModel : TalkiPlayerBaseViewModel
    {
       public SelectTalkiPlayerViewModel(
            ReactiveCommand<Unit, Unit> selectCommand
        )
        {
            var canExecute = this.WhenAnyValue(m => m.ChildrenCount).Select(count => count > 1)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Select(m => m);

            SelectCommand = ReactiveCommand.CreateFromObservable(() => selectCommand.Execute(), canExecute);
            SelectCommand.ThrownExceptions.SubscribeAndLogException();
        }
        
        public ReactiveCommand<Unit, Unit> SelectCommand { get; }

        public override string Name { get; set; }
        
        [Reactive]
        public int ChildrenCount { get; set; }
        
    }
}