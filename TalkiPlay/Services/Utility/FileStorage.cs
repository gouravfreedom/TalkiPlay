using System;
using System.IO;
using System.Threading.Tasks;
using ChilliSource.Mobile.Core;
using Splat;
using TalkiPlay.Shared;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay
{
    public partial class FileStorage : IStorage
    {
        private readonly ILogger _logger;
        private readonly string _rootFolder;
    
        public FileStorage(string rootFolder, ILogger logger = null)
        {
            _logger = logger ?? Locator.Current.GetService<ILogger>();
            _rootFolder = FileSystemManager.CreateDocumentsSubfolder(rootFolder);

        }
        
        public string GetRootPath()
        {
            return _rootFolder;
        }

        public async Task<string> WriteAsync(Stream stream, string fullPath)
        {
            if (String.IsNullOrWhiteSpace(fullPath))
            {
                throw new ArgumentNullException(nameof(fullPath));    
            }
            
            using (var fileStream = File.Open(fullPath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
            }

            return fullPath;
        }

        public async Task<Stream> ReadAsync(string fullPaths)
        {
            if (String.IsNullOrWhiteSpace(fullPaths))
            {
                throw new ArgumentNullException(nameof(fullPaths));    
            }
            
            var memoryStream = new MemoryStream();
            
            using (var fileStream = File.OpenRead(fullPaths))
            {
                var reader = new StreamReader(fileStream);
                await reader.BaseStream.CopyToAsync(memoryStream);
                return memoryStream;
            }
        }

      
    }
}