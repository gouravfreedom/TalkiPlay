using System;
using Akavache;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;
using ReactiveUI;
using Splat;
using TalkiPlay.Shared;
using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay.Droid
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
         
        }

        public override void OnCreate()
        {
            //RaygunClient.Attach(Config.RaygunKey);
            //Images.ImagesFolder = "";
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            RegisterDependencies();
          
            //A great place to initialize Xamarin.Insights and Dependency Services!
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
            Shutdown();
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityDestroyed(Activity activity)
        {
            if(activity is MainActivity)
            {
                var service = Locator.Current.GetService<IApplicationService>();
                service?.OnTerminated();
                Shutdown();
            }
        }

        public void OnActivityPaused(Activity activity)
        {
            if(activity is MainActivity)
            {
                Flush();
            }
        }

        public void OnActivityResumed(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
        }

        public void OnActivityStarted(Activity activity)
        {
            CrossCurrentActivity.Current.Activity = activity;
        }

        public void OnActivityStopped(Activity activity)
        {
            //Shutdown();
        }
        
           
        private void RegisterDependencies()
        {
            //Locator.CurrentMutable.RegisterLazySingleton(() => new MessageHandlerFactory(), typeof(IMessageHandlerFactory));
            Locator.CurrentMutable.RegisterLazySingleton(() => new AudioPlayerFactory(this), typeof(IAudioPlayerFactory));
            //Locator.CurrentMutable.RegisterLazySingleton(() => new TimezoneProvider(), typeof(ITimezone));
        }
        
        private void Shutdown()
        {
            Observable.FromAsync(async () => await BlobCache.Shutdown(), RxApp.TaskpoolScheduler)
                .OnErrorResumeNext(Observable.Return(Unit.Default))
                .SubscribeSafe();
        }
        
        private void Flush()
        {
            Observable.FromAsync(async () => await BlobCache.LocalMachine.Flush(), RxApp.TaskpoolScheduler)
                .OnErrorResumeNext(Observable.Return(Unit.Default))
                .SubscribeSafe();
        }
    }
}