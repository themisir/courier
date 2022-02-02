using Amazon.S3;

namespace Courier.Storage.S3;

public class S3ArchiveManagerOptions
{
    public string AccessKey { get; set; } = default!;
    public string SecretKey { get; set; } = default!;
    public string Region { get; set; } = default!;

    public string BucketName { get; set; } = default!;
    public S3StorageClass StorageClass { get; set; } = S3StorageClass.Standard;
    public string? ArchiveDirectory { get; set; }
}