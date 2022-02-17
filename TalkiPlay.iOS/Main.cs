using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Svg.Forms;
using Foundation;
using UIKit;

namespace TalkiPlay.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            //RaygunClient.Attach(Config.RaygunKey);

			// if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
			var ignore = typeof(SvgCachedImage);
            UIApplication.Main(args, null, "AppDelegate");
        }
    }
}