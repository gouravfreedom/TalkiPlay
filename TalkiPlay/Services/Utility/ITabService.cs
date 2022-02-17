using System;
using System.Reactive;

namespace TalkiPlay.Shared
{
    public interface ITabService
    {
        IObservable<Unit> ChangeTab(TabItemType tabItemType);
        IObservable<Unit> ShowBadge(string badge, TabItemType tabItemType);
    }
}