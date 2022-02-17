using System;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class CollectionViewEx : CollectionView
    {
        public CollectionViewEx()
        {
        }

        public static BindableProperty ShouldDisableScrollProperty =
            BindableProperty.Create(nameof(ShouldDisableScroll), typeof(bool), typeof(CollectionViewEx), false);

        public bool ShouldDisableScroll
        {
            get => (bool)GetValue(ShouldDisableScrollProperty);
            set => SetValue(ShouldDisableScrollProperty, value);
        }
    }
}
