using Courier.Data.Models;

namespace Courier.Models;

public class PublishPackageResult
{
    public bool Succeeded { get; }
    public Package? Package { get; }
    public string[] Errors { get; }

    public PublishPackageResult(bool succeeded, Package? package, string[] errors)
    {
        Succeeded = succeeded;
        Package = package;
        Errors = errors;
    }

    public static PublishPackageResult Success(Package package) => new(true, package, Array.Empty<string>());

    public static PublishPackageResult Error(params string[] errors) => new(false, null, errors);
}