using System;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class DeviceInfoCellTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _deviceInfoDataTemplate;
        private readonly DataTemplate _checkForUpdateTemplate;

        public DeviceInfoCellTemplateSelector()
        {
            _deviceInfoDataTemplate = new DataTemplate(() => new DeviceInfoItemViewCell());
            _checkForUpdateTemplate = new DataTemplate(() => new CheckForUpdateCell());
        }
        
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is DeviceInfoViewModel)
            {
                return _deviceInfoDataTemplate;
            }

            if (item is CheckForUpdateViewModel)
            {
                return _checkForUpdateTemplate;
            }
            
            throw new NotSupportedException();
        }
    }
}