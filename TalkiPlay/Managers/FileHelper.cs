using System;
using System.IO;
using System.Security.Cryptography;
using ChilliSource.Core.Extensions;
using Splat;
using ILogger = ChilliSource.Mobile.Core.ILogger;

namespace TalkiPlay.Shared
{
    public static class FileHelper
    {
        public static string GetFileName(string path)
        {
            if (!File.Exists(path))
            {
                return "";
            }

            var info = new FileInfo(path);

            return info.Name;

        }
        
        public static long GetFileSize(string path)
        {
            if (!File.Exists(path))
            {
                return 0;
            }

            var info = new FileInfo(path);

            return info.Length;

        }

        public static string GenerateCheckSum(string path)
        {
            var logger = Locator.Current.GetService<ILogger>();

            try
            {
                if (!File.Exists(path))
                {
                    return "";
                }

                var info = new FileInfo(path);

                

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        stream.Position = 0;
                        var hash = md5.ComputeHash(stream).ToHexString().ToLower();
                        return hash;
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex);
                return "";
            }


        }

        public static bool IsCheckSumMatch(string path, string checksum, long size)
        {
            var logger = Locator.Current.GetService<ILogger>();

            try
            {
                if (!File.Exists(path))
                {
                    return false;
                }

                var info = new FileInfo(path);

                if (info.Length != size)
                {
                    return false;
                }

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(path))
                    {
                        stream.Position = 0;
                        var hash = md5.ComputeHash(stream).ToHexString().ToLower();
                        logger?.Information($"CheckSum is {hash} and actual Checksum is {checksum}");
                        return hash == checksum;
                    }
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex);
                return false;
            }

        }
    }
}
