// using System;
// using TalkiPlay.Functional.UI.FormsExtensions;
// using TalkiPlay.Renderers.FormsExtensions;
// using WebKit;
// using Xamarin.Forms;
// using Xamarin.Forms.Platform.iOS;
//
// [assembly: ExportRenderer(typeof(ExtendedWebView), typeof(ExtendedWebViewRenderer))]
//
// namespace TalkiPlay.Renderers.FormsExtensions
// {
//     public class ExtendedWebViewRenderer : WkWebViewRenderer
//     {
//         protected override void OnElementChanged(VisualElementChangedEventArgs e)
//         {
//             base.OnElementChanged(e);
//
//             this.UIDelegate = new MyWKUIDelegate();
//             this.NavigationDelegate = new MyWKNavigationDelegate();
//         }
//     }
//     
//     public class MyWKUIDelegate : WKUIDelegate
//     {
//         public override WKWebView CreateWebView(WKWebView webView, WKWebViewConfiguration configuration, WKNavigationAction navigationAction, WKWindowFeatures windowFeatures)
//         {
//             var url = navigationAction.Request.Url;
//             if (navigationAction.TargetFrame == null)
//             {
//                 webView.LoadRequest(navigationAction.Request);
//             }
//
//             if (!navigationAction.TargetFrame.MainFrame)
//             {
//                 webView.LoadRequest(navigationAction.Request);
//             }
//
//             return webView;
//         }
//     }
//
//     public class MyWKNavigationDelegate : WKNavigationDelegate
//     {
//         public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
//         {
//             base.DecidePolicy(webView, navigationAction, decisionHandler);
//         }
//
//         public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction,
//             WKWebpagePreferences preferences, Action<WKNavigationActionPolicy, WKWebpagePreferences> decisionHandler)
//         {
//             if (decisionHandler != null)
//             {
//                 decisionHandler(WKNavigationActionPolicy.Allow, preferences);
//             }
//         }
//
//         public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse,
//             Action<WKNavigationResponsePolicy> decisionHandler)
//         {
//             decisionHandler(WKNavigationResponsePolicy.Allow);
//         }
//     }
// }