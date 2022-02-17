using System;
using Lottie.Forms;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class ExtendedAnimationView : AnimationView
    {
        public ExtendedAnimationView()
        {
        }

        public static BindableProperty JsonSourceProperty = BindableProperty.Create(nameof(JsonSource), 
            typeof(string), typeof(ExtendedAnimationView));
        public string JsonSource
        {
            get { return (string)GetValue(JsonSourceProperty); }
            set { SetValue(JsonSourceProperty, value); }
        }
    }
}