namespace TalkiPlay.Shared
{
    public interface ITalkiPlayNavigator
    {
        void NavigateToLoginPage();
        void NavigateToTabbedPage(TabItemType defaultTab = TabItemType.Games);
    }
}