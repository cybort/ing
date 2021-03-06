﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Services
{
    public interface IPackageProvider
    {
        string[] SupportedTargetFrameworks { get; set; }

        Task<IReadOnlyList<ModuleMetadata>> SearchPackagesAsync(int page);

        Task<string> GetLastestVersionAsync(string packageId);

        Task<ModuleMetadata> GetMetadataAsync(string id, string version);

        Task<Stream> FetchAsync(string id, string version);
    }
}
