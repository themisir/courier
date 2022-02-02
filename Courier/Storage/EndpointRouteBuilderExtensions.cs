using Courier.Storage.Local;
using Microsoft.Extensions.Options;

namespace Courier.Storage;

public static class EndpointRouteBuilderExtensions
{
    public static void MapArchiveStorage(this IEndpointRouteBuilder endpoints)
    {
        var options = endpoints.ServiceProvider.GetRequiredService<IOptions<ArchiveManagerOptions>>();
        if (options.Value.Type == ArchiveManagerType.Local)
        {
            endpoints.MapLocalArchives();
        }
    }
}