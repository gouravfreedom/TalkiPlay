﻿using System;
using System.Net;
using System.Reactive.Linq;
 using System.Threading.Tasks;
 using Acr.UserDialogs;
using ChilliSource.Mobile;
using ChilliSource.Mobile.Api;
using ChilliSource.Mobile.Core;
using ReactiveUI;
using Refit;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public static class ExceptionExtensions
    {
         public static IObservable<Exception> ShowExceptionDialog(this IObservable<Exception> source, 
            ToastConfig config = null,
            bool showNonApiException = true)
         {
           var userDialogs = Locator.Current.GetService<IUserDialogs>();
            return source
                .HandleApiException(TimeSpan.FromMilliseconds(300))
                .SelectMany(exception =>
                {
                   
                    if (!(exception is ApiHandledException apiEx))
                    {
                        if (showNonApiException)
                        {
                            userDialogs.Toast(config ?? Dialogs.BuildErrorToast(exception.Message));
                        }

                        return Observable.Return(exception);
                    }

                    if (apiEx.ApiException == null)
                    //if (!apiEx.ApiException.HasValue)
                    {
                        
                        userDialogs.Toast(config ?? Dialogs.BuildErrorToast(ErrorMessages.UnhandledMessage));
                        
                        return Observable.Return(exception);
                    }
                        
                    var error = apiEx.GetErrorResult(ApiConfiguration
                        .DefaultJsonSerializationSettingsFactory());

                    userDialogs.Toast(config ?? Dialogs.BuildErrorToast(error.ErrorMessages()));
                        
                    return Observable.Return(exception);

                });
        }
         
         public static Exception ShowExceptionDialog(this Exception exception, 
             ToastConfig config = null,
             bool showNonApiException = true)
         {

             if (exception is ApiException apiException)
             {
                 var exConfig = Locator.Current.GetService<ApiExceptionHandlerConfig>();
                 if (exConfig != null)
                 {
                     apiException.HandleApiException(exConfig);
                 }
             }
             
             if (!(exception is ApiHandledException apiEx))
             {
                 if (showNonApiException)
                 {
                     Dialogs.Toast(config ?? Dialogs.BuildErrorToast(exception.Message));
                 }
                 return exception;
             }

             if (apiEx.ApiException == null)
             //if (!apiEx.ApiException.HasValue)
             {
                 Dialogs.Toast(config ?? Dialogs.BuildErrorToast(ErrorMessages.UnhandledMessage));
                 return exception;
             }
                        
             var error = apiEx.GetErrorResult(ApiConfiguration
                 .DefaultJsonSerializationSettingsFactory());

             Dialogs.Toast(config ?? Dialogs.BuildErrorToast(error.ErrorMessages()));
                        
             return exception;
         }
        
         public static void LogException(this Exception exception, string message = "")
         {
             var logger = Locator.Current.GetService<ChilliSource.Mobile.Core.ILogger>();
             if (exception is ApiException apiEx)
             {
                 logger?.Error(apiEx , $"url:{apiEx.Uri.AbsolutePath}, code: {apiEx.StatusCode}, content: {apiEx.Content}");
             }
             else
             {
                 logger?.Error(exception, message);
             }
         }
         
         
         static async Task<Exception> HandleApiException(this Exception exception, TimeSpan delay)
         {

             await Task.Delay(delay);
             if (!(exception is ApiHandledException apiEx)) return exception;
             
             var config = Locator.Current.GetService<ApiExceptionHandlerConfig>();
             
             apiEx.ApiException.HandleApiException(config);

             return exception;
             
             // return source
             //     .ObserveOn(RxApp.MainThreadScheduler)
             //     .Delay(delay)
             //     .Do(exception =>
             //     {
             //         if (!(exception is ApiHandledException apiEx)) return;
             //         apiEx.ApiException.Match(ex =>
             //         {
             //             var config = Locator.Current.GetService<ApiExceptionHandlerConfig>();
             //             if (config != null)
             //             {
             //                 ex.HandleApiException(config);
             //             }
             //         }, () => { });
             //     });
         }
         
        static IObservable<Exception> HandleApiException(this IObservable<Exception> source, TimeSpan delay)
        {
            return source
                .ObserveOn(RxApp.MainThreadScheduler)
                .Delay(delay)
                .Do(exception =>
                {
                    if (!(exception is ApiHandledException apiEx)) return;
                    var config = Locator.Current.GetService<ApiExceptionHandlerConfig>();
                    apiEx.ApiException.HandleApiException(config);
                    
                    // apiEx.ApiException.Match(ex =>
                    // {
                    //     var config = Locator.Current.GetService<ApiExceptionHandlerConfig>();
                    //     if (config != null)
                    //     {
                    //         ex.HandleApiException(config);
                    //     }
                    // }, () => { });
                });
        }
        
        static void HandleApiException(this ApiException exception, ApiExceptionHandlerConfig config)
        {
            if (exception == null)
            {
                return;
            }
            
            if (IsSessionExpired(exception))
            {
                HandleSessionExpiry(exception, config);
            }
            else if (HasNoNetworkConnectivity(exception))
            {
                HandleNoNetworkConnectivity(exception, config);
            }
            else if (!exception.HasContent)
            {
                LogException(exception, "");
            }
        }

       

        static bool IsSessionExpired(ApiException ex)
        {
            return ex?.StatusCode == HttpStatusCode.Unauthorized;
        }

        static void HandleSessionExpiry(ApiException exception, ApiExceptionHandlerConfig config)
        {
            config.OnSessionExpired?.Invoke(ServiceResult.AsFailure(ErrorMessages.SessionTimedout, (int)exception.StatusCode));
        }

        static bool HasNoNetworkConnectivity(ApiException exception)
        {
            if (!exception.HasContent || exception.StatusCode != HttpStatusCode.RequestTimeout) return false;
            
            try
            {
                var result = exception.GetErrorResult(ApiConfiguration.DefaultJsonSerializationSettingsFactory());
                if (String.Equals(result.ErrorMessages(), ErrorMessages.NoNetWorkError, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            catch (Exception internalException)
            {
                LogException(internalException, "No network connectivity");
            }

            return false;
        }

        static void HandleNoNetworkConnectivity(ApiException exception, ApiExceptionHandlerConfig config)
        {
            var r = exception.GetErrorResult(ApiConfiguration.DefaultJsonSerializationSettingsFactory());
            config.OnNoNetworkConnectivity?.Invoke(ServiceResult.AsFailure(r.ErrorMessages(), ErrorMessages.NoNetworkErrorCode));
        }

        
        
    }
}