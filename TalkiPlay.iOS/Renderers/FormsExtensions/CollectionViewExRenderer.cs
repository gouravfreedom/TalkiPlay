using System;
using System.ComponentModel;
using ChilliSource.Mobile.UI;
using TalkiPlay;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CollectionViewEx), typeof(CollectionViewExRenderer))]

namespace ChilliSource.Mobile.UI
{
    public class CollectionViewExRenderer : CollectionViewRenderer
    {
        public CollectionViewExRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<GroupableItemsView> e)
        {
            base.OnElementChanged(e);
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
            if (Control == null || Control.Subviews == null || Control.Subviews.Length <= 0)
            {
                return;
            }

            var view = Element as CollectionViewEx;
            var ctl = Control.Subviews[0] as UIScrollView;

            if (ctl != null && view != null)
            {
                ctl.ScrollEnabled = !view.ShouldDisableScroll;
            }
        }
    }
}
