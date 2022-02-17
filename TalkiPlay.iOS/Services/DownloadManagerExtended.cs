using System.Threading.Tasks;
using TalkiPlay.Shared;

namespace TalkiPlay.iOS
{
    public class DownloadManagerExtended : IDownloadManagerExtended
    {
        public bool IsDownloadManagerEnabled()
        {
            return true;
        }

        public void OpenSettingsToEnableDownloadManager()
        {
           
        }
    }
}