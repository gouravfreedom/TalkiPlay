using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Splat;

namespace TalkiPlay.Shared
{
    public static class Dialogs
    {
        
        public static void ShowLoading(string text = "Loading ...", MaskType? maskType = null)
        {
            try
            {
                // var dialog =  Locator.Current.GetService<IUserDialogs>();
                // dialog.ShowLoading(text, maskType);
                UserDialogs.Instance.ShowLoading(text, maskType);
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("ShowLoading giving errors " + exc.Message );
                System.Diagnostics.Debug.WriteLine("Stack: " + exc.StackTrace);
                //throw exc;
            }
        }

        public static void Alert(string message, string title = null)
        {
            UserDialogs.Instance.Alert(message, title);
        }
        
        public static void Toast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }
        
        public static void Toast(ToastConfig config)
        {            
            UserDialogs.Instance.Toast(config);
        }
        
        public static void HideLoading()
        {
            try
            {
                UserDialogs.Instance.HideLoading();
            }
            catch(Exception exc)
            {
                System.Diagnostics.Debug.WriteLine("HideLoading giving errors " + exc.Message);
                System.Diagnostics.Debug.WriteLine("Stack: " + exc.StackTrace);
            }
        }

        public static async Task<bool> ConfirmAsync(ConfirmConfig config)
        {
            return await UserDialogs.Instance.ConfirmAsync(config);
        }

        public static IProgressDialog Progress(ProgressDialogConfig config)
        {
            return UserDialogs.Instance.Progress((config));
        }

        public static ToastConfig BuildErrorToast(string message, TimeSpan? dismissTimer = null)
        {
            return new ToastConfig(message)
            {
                Duration = dismissTimer ?? TimeSpan.FromSeconds(4),
                Position = ToastPosition.Bottom,
                BackgroundColor = Colors.Red,
                MessageTextColor = Colors.WhiteColor
            };
        }

        public static ToastConfig BuildSuccessToast(string message, TimeSpan? dismissTimer = null)
        {
            return new ToastConfig(message)
            {
                Duration = dismissTimer ?? TimeSpan.FromSeconds(4),
                Position = ToastPosition.Bottom,
                MessageTextColor = Colors.WhiteColor,
                BackgroundColor = Colors.BlueColor
            };
        }
    }
}
