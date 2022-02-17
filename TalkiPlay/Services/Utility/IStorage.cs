using System.IO;
using System.Threading.Tasks;

namespace TalkiPlay.Shared
{
    public interface IStorage
    {
        string GetRootPath();
        Task<string> WriteAsync(Stream stream, string fullPath);
        Task<Stream> ReadAsync(string fullPaths);
        Task<bool> HasStoragePermission();

        Task<bool> RequestStoragePermission();
    }
}