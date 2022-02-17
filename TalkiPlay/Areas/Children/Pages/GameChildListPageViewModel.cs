// using System;
// using System.Linq;
// using System.Reactive;
// using ChilliSource.Mobile.UI.ReactiveUI;
// using ReactiveUI;
// using Splat;
//
// namespace TalkiPlay.Shared
// {
//     public class GameChildListPageViewModel : ChildListPageViewModel, IModalViewModel
//     {
//         private readonly IGameMediator _gameMediator;
//
//         public GameChildListPageViewModel(IGameMediator gameMediator = null) : base()
//         {
//             _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
//             ShowLeftMenuItem = true;
//             SetupCommands();
//         }
//
//         void SetupCommands()
//         {
//             SelectCommand = ReactiveCommand.Create<ChildViewModel, Unit>(m =>
//             {
//                 _gameMediator.Children.Edit(item =>
//                 {
//                     var c = item.FirstOrDefault(a => a.Id == m.Child.Id);
//                     if (c == null)
//                     {
//                         item.Add(m.Child);
//                     }
//                 });
//
//                 SimpleNavigationService.PopModalAsync().Forget();
//                 return Unit.Default;
//             });
//             
//             // SelectCommand = ReactiveCommand.CreateFromObservable<ChildViewModel, Unit>(m =>
//             // {
//             //     _gameMediator.Children.Edit(item =>
//             //     {
//             //         var c = item.FirstOrDefault(a => a.Id == m.Child.Id);
//             //         if (c == null)
//             //         {
//             //             item.Add(m.Child);
//             //         }
//             //     });
//             //
//             //     return Navigator.PopModal();
//             // });
//
//             SelectCommand.ThrownExceptions.SubscribeAndLogException();
//
//             BackCommand = ReactiveCommand.Create(() => SimpleNavigationService.PopModalAsync().Forget());
//             BackCommand.ThrownExceptions.SubscribeAndLogException();
//
//         }
//     }
// }
