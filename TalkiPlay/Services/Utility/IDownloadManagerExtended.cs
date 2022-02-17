using System.Threading.Tasks;

namespace TalkiPlay.Shared
{
    public interface IDownloadManagerExtended
    {
       bool IsDownloadManagerEnabled();
       void OpenSettingsToEnableDownloadManager();
    }
}