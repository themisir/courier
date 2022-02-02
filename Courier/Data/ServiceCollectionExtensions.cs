using Microsoft.EntityFrameworkCore;

namespace Courier.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCourierDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CourierDbContext>(options =>
        {
            var provider = configuration.GetValue("Provider", "postgres").ToLowerInvariant();
            switch (provider)
            {
                case "postgres":
                    options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"),
                        x => x.MigrationsAssembly("Courier.PostgresMigrations"));
                    break;
                
                case "sqlite":
                    options.UseSqlite(configuration.GetConnectionString("SqliteConnection"),
                        x => x.MigrationsAssembly("Courier.SqliteMigrations"));
                    break;
                
                default:
                    throw new NotSupportedException($"Db engine type {provider} is not supported");
            }
        });
        
        return services;
    }
}