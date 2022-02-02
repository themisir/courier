using Courier.Storage.Local;
using Courier.Storage.S3;

namespace Courier.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddArchiveManager(this IServiceCollection services, IConfiguration configuration)
    {
        var options = new ArchiveManagerOptions();
        configuration.Bind(options);
        services.Configure<ArchiveManagerOptions>(configuration);
        
        switch (options.Type)
        {
            case ArchiveManagerType.S3:
                services.AddSingleton<IArchiveManager, S3ArchiveManager>();
                services.Configure<S3ArchiveManagerOptions>(configuration.GetSection("S3"));
                break;
            
            case ArchiveManagerType.Local:
                services.AddSingleton<IArchiveManager, LocalArchiveManager>();
                services.Configure<LocalArchiveManagerOptions>(configuration.GetSection("Local"));
                break;
            
            default:
                throw new NotSupportedException($"Storage type '{options.Type}' is not supported");
        }

        return services;
    }
}