using System;
using System.Windows.Input;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class SettingsViewTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _deviceInfoDataTemplate;
        private readonly DataTemplate _settingsCellTemplate;
        private readonly DataTemplate _toggleCellTemplate;
        public ICommand SelectCommand { get; set; }

        public SettingsViewTemplateSelector()
        {
            _settingsCellTemplate = new DataTemplate(() => new SettingsMenuItemView() {
                TapCommand = SelectCommand
            });
            _deviceInfoDataTemplate = new DataTemplate(() => new DeviceInfoItemView());
            _toggleCellTemplate = new DataTemplate(() => new ToggleItemView());
        }

        public void SetSelectCommand(ICommand command)
        {
            SelectCommand = command;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is DeviceInfoViewModel)
            {
                return _deviceInfoDataTemplate;
            }

            if (item is SettingsItemViewModel)
            {
                return _settingsCellTemplate;
            }

            if (item is ToggleItemViewModel)
            {
                return _toggleCellTemplate;
            }

            throw new NotSupportedException();
        }
    }
}
