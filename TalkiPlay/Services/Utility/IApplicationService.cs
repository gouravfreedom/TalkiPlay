﻿using System;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public interface IApplicationService
    {
        double StatusbarHeight { get; }
        double NavBarHeight { get; }
        Size ScreenSize { get; }      
        
        string Timezone { get; }
        
        void OpenSettings();       
        Thickness GetSafeAreaInsets(bool includeStatusBar = false);

        void OnTerminated();
        event EventHandler Terminated;
    }
}