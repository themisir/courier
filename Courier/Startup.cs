using Courier.Authentication;
using Courier.Data;
using Courier.Data.Models;
using Courier.Helpers;
using Courier.Models;
using Courier.Repositories;
using Courier.Services;
using Courier.Storage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;

namespace Courier;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        
        services.AddRazorPages(options =>
        {
            options.Conventions
                .AuthorizeFolder("/Account")
                .AuthorizeFolder("/SinglePackage");
        });
        
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCourierDbContext(Configuration);

        services.AddDataProtection()
            .SetApplicationName(Configuration["DataProtection:ApplicationName"] ?? "Courier")
            .PersistKeysToDbContext<CourierDbContext>();
        
        services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                    
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                    
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<CourierDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication()
            .AddBearerScheme<TokenRepository>();
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Bearer", builder => builder
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("Bearer"));
        });
        
        services.AddAutoMapper(options =>
        {
            options.AddProfile<AutoMapperProfile>();
        });
        
        services.Configure<AuthenticationOptions>(Configuration.GetSection("Authentication"));
            
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Login";
            options.LogoutPath = "/Logout";
        });
        
        services.Configure<ServerDetails>(Configuration.GetSection("Server"));
            
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.All;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddArchiveManager(Configuration.GetSection("Archives"));

        services.AddScoped<ITokenRepository, TokenRepository>()
            .AddScoped<IPackageRepository, PackageRepository>()
            .AddScoped<IMarkdownRenderer, MarkdownRenderer>()
            .AddScoped<IPermissionChecker, PermissionChecker>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseForwardedHeaders();

        app.UseRouting();
        
        app.UseStaticFiles();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapArchiveStorage();
            endpoints.MapControllers();
            endpoints.MapRazorPages();
        });
    }
}