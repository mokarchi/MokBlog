﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mok.Settings
{
    /// <summary>
    /// AppSettings section in appsettings.json.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The folder/container name of uploaded media files: "media" (default).
        /// </summary>
        /// <remarks>
        /// For FileSystem, it's a folder created in wwwroot, a typical url from file sys
        /// https://yoursite.com/media/2017/11/file-name.ext
        /// For AzureBlob, it's a container created in your Azure storage account, a typical url from blob
        /// https://your-blob-acct-name.blob.core.windows.net/media/2017/11/file-name.ext
        /// </remarks>
        public string MediaContainerName { get; set; } = "media";
    }
}