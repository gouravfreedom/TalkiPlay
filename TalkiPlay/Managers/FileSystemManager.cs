#region License

/*
Licensed to Blue Chilli Technology Pty Ltd and the contributors under the MIT License (the "License").
You may not use this file except in compliance with the License.
See the LICENSE file in the project root for more information.
*/

#endregion

using System;
using System.IO;

namespace ChilliSource.Mobile.Core
{

    /// <summary>
    /// Provides methods to work with the platform's file system
    /// </summary>
    public static class FileSystemManager
    {
        /// <summary>
        /// Returns the path to the platform's default documents folder
        /// </summary>
        /// <returns>The documents path.</returns>
        public static string GetDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        /// <summary>
        /// Creates a subfolder in the platform's default documents folder
        /// </summary>
        /// <returns>The created subfolder path.</returns>
        /// <param name="subFolderName">Subfolder name.</param>
        public static string CreateDocumentsSubfolder(string subFolderName)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var destinationFolder = Path.Combine(documents, subFolderName);

            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            return destinationFolder;
        }
    }
}

