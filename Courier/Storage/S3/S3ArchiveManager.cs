using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Courier.Data.Helpers;
using Courier.Data.Models;
using Courier.Storage.S3.Internal;
using Microsoft.Extensions.Options;

namespace Courier.Storage.S3;

public class S3ArchiveManager : IArchiveManager
{
    private readonly IAmazonS3 _amazonS3;
    private readonly IOptions<S3ArchiveManagerOptions> _options;

    public S3ArchiveManager(IOptions<S3ArchiveManagerOptions> options)
    {
        _options = options;
        _amazonS3 = new AmazonS3Client(
            options.Value.AccessKey,
            options.Value.SecretKey,
            RegionEndpoint.GetBySystemName(options.Value.Region));
    }

    public async Task<string?> Upload(Stream stream, PackageVersion packageVersion)
    {
        var uniqueId = IdHelper.GenerateId();
        var normalizedVersionName = new string(packageVersion.VersionName
            .Where(c => char.IsLetterOrDigit(c) || c is '_' or '.')
            .ToArray());

        var objectKey = (packageVersion.Package?.Name ?? packageVersion.PackageId) +
                        $"/{normalizedVersionName}_{uniqueId}.tar.gz";
        if (!string.IsNullOrEmpty(_options.Value.ArchiveDirectory))
        {
            objectKey = Path.Combine(_options.Value.ArchiveDirectory, objectKey);
        }
            
        var uploadRequest = new TransferUtilityUploadRequest
        {
            Key = objectKey,
            InputStream = stream,
            BucketName = _options.Value.BucketName,
            CannedACL = S3CannedACL.Private,
            StorageClass = S3StorageClass.Standard,
            AutoCloseStream = false,
        };

        var fileTransferUtility = new TransferUtility(_amazonS3);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return new BlobObject(_options.Value.BucketName, objectKey).ToString();
    }

    public Task<string> GetDownloadUrl(string key)
    {
        var blobObject = BlobObject.Parse(key);
        var preSignedUrl = _amazonS3.GeneratePreSignedURL(blobObject.BucketName, blobObject.ObjectKey,
            DateTime.UtcNow.AddHours(1), new Dictionary<string, object>());
            
        return Task.FromResult(preSignedUrl);
    }

    public async Task<bool> RemoveArchive(string key)
    {
        if (!BlobObject.TryParse(key, out var blobObject))
        {
            return false;
        }

        var deleteRequest = new DeleteObjectRequest
        {
            Key = blobObject!.ObjectKey,
            BucketName = blobObject!.BucketName
        };
                
        await _amazonS3.DeleteObjectAsync(deleteRequest);
        return true;
    }
}