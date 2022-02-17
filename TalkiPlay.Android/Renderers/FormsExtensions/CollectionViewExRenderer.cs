using System;
using System.ComponentModel;
using Android.Content;
using ChilliSource.Mobile.UI;
using TalkiPlay;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CollectionViewEx), typeof(CollectionViewExRenderer))]

namespace ChilliSource.Mobile.UI
{
    public class CollectionViewExRenderer : CollectionViewRenderer
    {
        public CollectionViewExRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);
            UpdateScrollState();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
        {
            base.OnElementPropertyChanged(sender, changedProperty);

            if (changedProperty.PropertyName == CollectionViewEx.ShouldDisableScrollProperty.PropertyName)
            {
                UpdateScrollState();
            }
        }

        private void UpdateScrollState()
        {

            var view = Element as CollectionViewEx;

            if (ItemsView  != null && view != null)
            {
                ItemsView.VerticalScrollBarVisibility = view.ShouldDisableScroll ? ScrollBarVisibility.Default : ScrollBarVisibility.Never;
            }
        }
    }
}
