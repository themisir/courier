using System.IO.Compression;
using System.Text;
using Courier.Models;
using ICSharpCode.SharpZipLib.Tar;
using YamlDotNet.Serialization;

namespace Courier.Helpers;

public static class ArchiveHelper
{
    private const int BufferSize = 3 * 1024 * 1024; // 3 MB
    private static readonly Deserializer YamlDeserializer = new();

    public static async Task<PackageContents> ReadPackageArchive(Stream stream)
    {
        await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress, true);
        await using var tarIn = new TarInputStream(gzipStream, Encoding.ASCII);
        await using var bufferStream = new MemoryStream(BufferSize);

        var contents = new PackageContents();

        TarEntry tarEntry;
        while ((tarEntry = tarIn.GetNextEntry()) != null)
        {
            if (tarEntry.IsDirectory) continue;

            var directory = Path.GetDirectoryName(tarEntry.Name);
            if (!string.IsNullOrEmpty(directory))
            {
                continue;
            }

            var basename = Path.GetFileNameWithoutExtension(tarEntry.Name.ToLowerInvariant());
            if (basename is not ("pubspec" or "readme" or "changelog"))
            {
                continue;
            }

            var buffer = new byte[tarEntry.Size];

            bufferStream.Seek(0, SeekOrigin.Begin);
            tarIn.CopyEntryContents(bufferStream);
            bufferStream.Seek(0, SeekOrigin.Begin);
            bufferStream.Read(buffer, 0, buffer.Length);

            var decodedText = Encoding.UTF8.GetString(buffer);

            switch (basename)
            {
                case "pubspec":
                    contents.Pubspec = YamlDeserializer.Deserialize<Dictionary<string, object>>(decodedText);
                    break;

                case "readme":
                    contents.Readme = decodedText;
                    break;

                case "changelog":
                    contents.Changelog = decodedText;
                    break;
            }

            // Check if all fields are filled, if then
            if (contents.Pubspec != null && contents.Changelog != null && contents.Readme != null)
            {
                break;
            }
        }

        return contents;
    }
}