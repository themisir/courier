using Courier.Data.Helpers;
using Courier.Data.Models;
using Microsoft.Extensions.Options;

namespace Courier.Storage.Local;

public class LocalArchiveManager : IArchiveManager
{
    private readonly IOptions<LocalArchiveManagerOptions> _options;

    public LocalArchiveManager(IOptions<LocalArchiveManagerOptions> options)
    {
        _options = options;
    }

    public async Task<string?> Upload(Stream stream, PackageVersion packageVersion)
    {
        if (!Directory.Exists(_options.Value.Directory))
        {
            Directory.CreateDirectory(_options.Value.Directory);
        }
            
        var fileName = $"{packageVersion.Package?.Name ?? packageVersion.PackageId}_{packageVersion.VersionName}_{IdHelper.GenerateId()}.tar.gz";
        var filePath = Path.Combine(_options.Value.Directory,fileName);
            
        await using var fileStream = File.Create(filePath);
        await stream.CopyToAsync(fileStream);
        return fileName;
    }

    public Task<string> GetDownloadUrl(string data)
    {
        return Task.FromResult(_options.Value.Origin + _options.Value.UriPrefix + $"/{data}");
    }

    public Task<bool> RemoveArchive(string data)
    {
        if (!long.TryParse(data, out var id))
        {
            return Task.FromResult(false);
        }
            
        var filePath = Path.Combine(_options.Value.Directory, $"{id}.tar.gz");
        if (!File.Exists(filePath))
        {
            return Task.FromResult(false);
        }
            
        File.Delete(filePath);
        return Task.FromResult(true);
    }
}