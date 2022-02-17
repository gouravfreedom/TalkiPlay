using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using TalkiPlay.Shared;
using Android.Provider;
using Uri = Android.Net.Uri;

namespace TalkiPlay.Droid
{
    public class DownloadManagerExtended : IDownloadManagerExtended, IDisposable
    {
        private readonly Context _context;
        private  const string DOWNLOAD_MANAGER_PACKAGE_NAME = "com.android.providers.downloads";
        public DownloadManagerExtended(Context context)
        {
            _context = context;
        }
        
        public  bool IsDownloadManagerEnabled()
        {
            return ResolveEnable();
        }

        public void OpenSettingsToEnableDownloadManager()
        {
            try {
                //Open the specific App Info page:
                var intent = new Intent(Settings.ActionApplicationDetailsSettings);
                intent.SetData(Uri.Parse($"package:{DOWNLOAD_MANAGER_PACKAGE_NAME}"));
                _context.StartActivity(intent);
            } catch (ActivityNotFoundException) {
                //Open the generic Apps page:
                var intent = new Intent(Settings.ActionManageApplicationsSettings);
                _context.StartActivity(intent);
            }
        }
        
        private bool ResolveEnable() {
            var state = _context.PackageManager
                .GetApplicationEnabledSetting(DOWNLOAD_MANAGER_PACKAGE_NAME);

            if ((int) Build.VERSION.SdkInt > 18) {
                return !(state == ComponentEnabledState.Disabled ||
                         state == ComponentEnabledState.DisabledUser
                         || state == ComponentEnabledState.DisabledUntilUsed);
            }

            return !(state == ComponentEnabledState.DisabledUser ||
                     state == ComponentEnabledState.DisabledUntilUsed);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}