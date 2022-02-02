using Courier.Data.Configuration;
using Courier.Data.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Courier.Data;

public class CourierDbContext : IdentityDbContext<AppUser>, IDataProtectionKeyContext
{
    public DbSet<Package> Packages { get; set; } = null!;
    public DbSet<PackageUser> PackageUsers { get; set; } = null!;
    public DbSet<PackageVersion> PackageVersions { get; set; } = null!;
    public DbSet<PackageAuditLog> PackageAuditLogs { get; set; } = null!;
    public DbSet<UserPersonalToken> UserPersonalTokens { get; set; } = null!;
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

    public CourierDbContext(DbContextOptions<CourierDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Allow user defined IDs
        builder.UseIdentityByDefaultColumns();

        // Set DateTime kind to UTC
        builder.UseValueConverterForType<DateTime>(new ValueConverter<DateTime, DateTime>(
            v => v,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));

        // Serialize dictionaries as JSON string
        #region Json Properties

        var jsonValueComparer = JsonPropertyConfiguration.CreateDictionaryValueComparer<string, object>();

        builder.ApplyConfiguration(JsonPropertyConfiguration.For<PackageVersion>()
            .Property(v => v.Metadata)
            .WithComparer(jsonValueComparer));

        builder.ApplyConfiguration(JsonPropertyConfiguration.For<PackageAuditLog>()
            .Property(v => v.Meta)
            .WithComparer(jsonValueComparer));

        #endregion
    }
}