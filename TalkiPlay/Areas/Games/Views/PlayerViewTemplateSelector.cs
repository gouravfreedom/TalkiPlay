using System;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class PlayerViewTemplateSelector : DataTemplateSelector
	{
		private readonly DataTemplate _playerViewTemplate;
		private readonly DataTemplate _selectPlayerViewTemplate;

		public PlayerViewTemplateSelector()
		{
			_playerViewTemplate = new DataTemplate(() => new PlayerView());
			_selectPlayerViewTemplate = new DataTemplate(() => new SelectPlayerView());
		}

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is ChildPlayerViewModel)
			{
				return _playerViewTemplate;
			}

			if (item is SelectPlayerViewModel)
			{
				return _selectPlayerViewTemplate;
			}

			throw new NotSupportedException();
		}
   
    }
}
