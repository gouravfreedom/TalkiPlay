﻿using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Acr.UserDialogs;
using ChilliSource.Mobile.Api;
using ChilliSource.Mobile.Core;
using Refit;
using Splat;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public static class ObservableOperatorExtensions
    {
        public static IObservable<ServiceResult<T>> ToResult<T>(this IObservable<T> source)
        {
            return source
                .Select(m => ServiceResult<T>.AsSuccess(m))
                .Catch<ServiceResult<T>, Exception>(ex =>
                {
                    ex.LogException("ToResult");
                    ServiceResult<T> r;
                    if (ex is ApiException apiEx)
                    {
                        if (IsSessionExpired(apiEx))
                        {
                            HandleSessionExpiry(apiEx, Locator.Current.GetService<ApiExceptionHandlerConfig>());
                        }

                        r = ServiceResult<T>.AsFailure( new ApiHandledException(apiEx),
                            statusCode: (int) apiEx.StatusCode);
                    }
                    else
                    {
                        r = ServiceResult<T>.AsFailure(new ApiHandledException(ex));
                    }
                    return Observable.Return(r);
                });
        }
       
        public static IObservable<T> RetryWhen<T, U>(this IObservable<T> source,
            Func<IObservable<Exception>, IObservable<U>> handler)
        {
            var errorSignal = new Subject<Exception>();
            var retrySignal = handler(errorSignal);
            var sources = new BehaviorSubject<IObservable<T>>(source);

            return Observable.Using(
                () => retrySignal.Select(s => source).Subscribe(sources),
                r => sources.Select(src =>
                    src.Do(v => { }, e => errorSignal.OnNext(e), () => errorSignal.OnCompleted())
                        .OnErrorResumeNext(Observable.Empty<T>())
                ).Concat()
            );
        }
        
        public static IObservable<T> RetryAfter<T>(this IObservable<T> source, 
            int maxRetries,
            TimeSpan delay,
            IScheduler scheduler)
        {
            var retryCount = 0;

            return RetryWhen(source, o => o.SelectMany(ex =>
            {
                return ++retryCount < maxRetries ? Observable.Timer(delay, scheduler).Select(m => ex) 
                    : Observable.Throw<Exception>(ex);
            }));
        }

        // public static IObservable<T> ShowLoading<T>(this IObservable<T> source, string title)
        // {
        //     return source.Do(_ =>
        //     {
        //         var userDialog = Locator.Current.GetService<IUserDialogs>();
        //         userDialog.ShowLoading(title, MaskType.Black);
        //     });
        // }
        
        public static IObservable<T> HideLoading<T>(this IObservable<T> source)
        {
            return source.Do(_ =>
            {
                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.HideLoading();
            });
        }
        
        // public static IObservable<T> ShowToast<T>(this IObservable<T> source, 
        //     string message = null, 
        //     ToastConfig config = null, TimeSpan? dismissTimer = null)
        // {
        //     return source.Do(_ =>
        //     {
        //         var userDialog = Locator.Current.GetService<IUserDialogs>();
        //         userDialog.Toast(config ?? Dialogs.BuildSuccessToast(message, dismissTimer));
        //     });
        // }
        
        public static IObservable<T> ShowSuccessToast<T>(this IObservable<T> source, 
            string message = null)
        {
            return source.Do(_ =>
            {
                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.Toast(Dialogs.BuildSuccessToast(message));
            });
        }
        
        public static IObservable<T> ShowErrorToast<T>(this IObservable<T> source, string message, ToastConfig config = null, TimeSpan? dismissTimer = null)
        {
            return source.Do(ex =>
            {
                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.Toast(config ?? Dialogs.BuildErrorToast(message, dismissTimer));
            });
        }

        public static IObservable<Exception> ShowErrorToast(this IObservable<Exception> source, string message = null, ToastConfig config = null, TimeSpan? dismissTimer = null)
        {
            return source.Do(ex =>
            {
                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.Toast(config ?? Dialogs.BuildErrorToast(message ?? ex.Message, dismissTimer));
            });
        }

        public static IObservable<System.Reactive.Unit> StartShowLoading(string title, MaskType type = MaskType.Black)
        {
            return Observable.Start(() =>
            {
                var userDialog = Locator.Current.GetService<IUserDialogs>();
                userDialog.ShowLoading(title, MaskType.Black);
            });
        }
     
     
        static bool IsSessionExpired(ApiException ex)
        {
            if (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                return true;
            }

            return false;
        }
        
        static void HandleSessionExpiry(ApiException exception, ApiExceptionHandlerConfig config)
        {
            config.OnSessionExpired?.Invoke(ServiceResult.AsFailure("Session has timed out. Please log in again", (int)exception.StatusCode));
        }

    }
}