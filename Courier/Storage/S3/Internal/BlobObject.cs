namespace Courier.Storage.S3.Internal;

public class BlobObject
{
    public string BucketName { get; }
    public string ObjectKey { get; }

    public BlobObject(string bucketName, string objectKey)
    {
        BucketName = bucketName;
        ObjectKey = objectKey;
    }

    public static BlobObject Parse(string url)
    {
        var uri = new Uri(url);
        if (uri.Scheme.ToLower() != "s3")
        {
            throw new NotSupportedException("Invalid S3 object url supplied.");
        }

        return new BlobObject(uri.Host, uri.AbsolutePath[1..]);
    }

    public static bool TryParse(string url, out BlobObject? result)
    {
        var uri = new Uri(url);
        if (uri.Scheme.ToLower() != "s3")
        {
            result = null;
            return false;
        }

        result = new BlobObject(uri.Host, uri.AbsolutePath[1..]);
        return true;
    }
            
    public override string ToString()
    {
        return Path.Combine($"s3://{BucketName}", ObjectKey);
    }
}