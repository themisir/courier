using Microsoft.Extensions.Options;

namespace Courier.Storage.Local;

public static class EndpointRouteBuilderExtensions
{
    public static void MapLocalArchives(this IEndpointRouteBuilder endpoints)
    {
        var settings = endpoints.ServiceProvider.GetRequiredService<IOptions<LocalArchiveManagerOptions>>().Value;

        endpoints.MapGet(settings.UriPrefix + "/{name}", async context =>
        {
            try
            {
                if (context.Request.RouteValues["name"] is not string name)
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                if (!name.All(c => char.IsLetterOrDigit(c) || c is '_' or '.') || name.Contains(".."))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                var filePath = Path.Combine(settings.Directory, Path.GetFileName(name));
                if (File.Exists(filePath))
                {
                    context.Response.ContentType = "application/tar+gzip";
                    await context.Response.SendFileAsync(filePath);
                    return;
                }

                context.Response.StatusCode = 404;
            }
            finally
            {
                await context.Response.CompleteAsync();
            }
        });
    }
}