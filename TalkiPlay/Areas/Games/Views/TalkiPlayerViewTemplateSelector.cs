using System;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
	public class TalkiPlayerTemplateSelector : DataTemplateSelector
	{
		private readonly DataTemplate _talkiplayerViewTemplate;
		private readonly DataTemplate _selecttalkiPlayerViewTemplate;

		public TalkiPlayerTemplateSelector()
		{
			_talkiplayerViewTemplate = new DataTemplate(() => new TalkiPlayerView());
			_selecttalkiPlayerViewTemplate = new DataTemplate(() => new SelectTalkiPlayerView());
		}

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			if (item is TalkiPlayerViewModel)
			{
				return _talkiplayerViewTemplate;
			}

			if (item is SelectTalkiPlayerViewModel)
			{
				return _selecttalkiPlayerViewTemplate;
			}

			throw new NotSupportedException();
		}

	}
}
