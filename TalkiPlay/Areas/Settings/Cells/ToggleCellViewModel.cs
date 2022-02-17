using System;
using ReactiveUI;

namespace TalkiPlay.Shared
{
    public class ToggleItemViewModel 
    {
        bool _isOn;
        Action<bool> _callback;

        public ToggleItemViewModel(Action<bool> callback)
        {
            _callback = callback;
        }
        
        public string Title { get; set; }

        public bool IsOn
        {
            get => _isOn;
            set
            {
                _isOn = value;
                _callback?.Invoke(_isOn);                
            }
        }
            
    }
}
