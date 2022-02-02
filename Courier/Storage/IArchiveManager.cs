using Courier.Data.Models;

namespace Courier.Storage;

public interface IArchiveManager
{
    /// <summary>
    /// Uploads contents of <paramref name="stream"/> and returns the data
    /// required to retain saved binary.
    /// </summary>
    /// <param name="stream">File contents</param>
    /// <param name="packageVersion">Package version that will be linked to uploaded archive</param>
    /// <returns>Task that resolves to string that later might be used to retain saved data</returns>
    Task<string?> Upload(Stream stream, PackageVersion packageVersion);

    /// <summary>
    /// Converts <paramref name="key"/> into http(s) URI that might be used
    /// for downloading stored data.
    /// </summary>
    /// <param name="key">Data that references to stored item</param>
    /// <returns>Task that resolves to URL or null if archive is not exists</returns>
    Task<string> GetDownloadUrl(string key);

    /// <summary>
    /// Removes saved archive.
    /// </summary>
    /// <param name="key">Data that references to stored item</param>
    /// <returns>Task that resolves to bool that represents status of the operation</returns>
    Task<bool> RemoveArchive(string key);
}