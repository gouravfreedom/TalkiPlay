using System;
using TalkiPlay;
using TalkiPlay.Shared;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(SimpleTabPage), typeof(SimpleTabPageRenderer))]

namespace TalkiPlay
{
	public class SimpleTabPageRenderer : TabbedRenderer
    {
        SimpleTabPage TabPage => Element as SimpleTabPage;
        
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            
            TabBar.TintColor = Colors.TabBarSelectedColor.ToUIColor();
            TabBar.Translucent = false;

            if (TabBar.Items != null)
            {
                SetTabBarItem(0, Images.GameTabIcon, Images.GameTabSelectedIcon);
                SetTabBarItem(1, Images.KidsTabIcon, Images.KidsTabSelectedIcon);
                SetTabBarItem(2, Images.RewardsTabIcon, Images.RewardsTabSelectedIcon);
                SetTabBarItem(3, Images.ItemsTabIcon, Images.ItemsTabSelectedIcon);
                SetTabBarItem(4, Images.SettingsTabIcon, Images.SettingsTabSelectedIcon);
            }
        }

        void SetTabBarItem(int index, string image, string selectedImage)
        {
            var image1 = UIImage.FromBundle(image);
            if (image1 != null)
            {
                TabBar.Items[index].Image = image1.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }

            var image2 = UIImage.FromBundle(selectedImage);
            if (image2 != null)
            {
                TabBar.Items[index].SelectedImage = image2.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
        }

    }
}
