using System.Threading.Tasks;
using Android.OS;
using Xamarin.Essentials;

namespace TalkiPlay
{
    public partial class FileStorage
    {
        public async Task<bool> HasStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            
            return status == PermissionStatus.Granted;
        }

        public async Task<bool> RequestStoragePermission()
        {
            if ((int)Build.VERSION.SdkInt < 23)
            {
                return true;
            }

            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                _logger.Information("Does not have storage permission granted, requesting.");

                var result = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (result != PermissionStatus.Granted)
                {
                    _logger.Information("Storage permission Denied.");
                    return false;
                }
            }

            return status == PermissionStatus.Granted;
        }
    }
}