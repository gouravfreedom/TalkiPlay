using System.Threading.Tasks;

namespace TalkiPlay
{
    public partial class FileStorage
    {
        public Task<bool> HasStoragePermission()
        {
            return Task.FromResult(true);
        }

        public Task<bool> RequestStoragePermission()
        {
            return Task.FromResult(true);
        }
    }
}