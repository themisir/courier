using System.CommandLine;
using Courier.Data;
using Courier.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Courier;

public class Program
{
    private readonly ILogger<Program> _logger;
    private readonly string[] _args;
    private readonly IHost _host;
    
    public static Task<int> Main(string[] args)
    {
        return new Program(args).Run();
    }

    public Program(string[] args)
    {
        _args = args;
        _host = CreateHostBuilder().Build();
        _logger = _host.Services.GetRequiredService<ILogger<Program>>();
    }

    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder(_args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public async Task<int> Run()
    {
        var createUserCommand = new Command("add", description: "Create a new user account")
        {
            new Option(aliases: new[] { "--username", "-u" }, description: "Username")
            {
                IsRequired = true,
            },
            new Option(aliases: new[] { "--email", "-e" }, description: "Email")
            {
                IsRequired = true,
            },
            new Option(aliases: new[] { "--password", "-p" }, description: "Password"),
            new Option(aliases: new[] { "--fullname", "-n" }, description: "Full name"),
            new Option(aliases: new[] { "--password-stdin" }, description: "Read password from stdin"),
        };

        createUserCommand.SetHandler<string, string, string, string, bool>(CreateUser);

        var migrateDatabaseCommand = new Command("migrate", description: "Migrate database schema")
        {
            new Option(aliases: new[] { "--connection", "-c" }, description: "Connection string"),
        };

        migrateDatabaseCommand.SetHandler(MigrateDatabase);

        var rootCommand = new RootCommand(description: "Package server for dart and flutter")
        {
            new Option(aliases: new[] { "--migrate-database" }, description: "Migrates database before server startup"),
            new Command("users", description: "Manage users")
            {
                createUserCommand,
            },
            new Command("database", description: "Database utilities")
            {
                migrateDatabaseCommand,
            }
        };

        rootCommand.SetHandler(StartServer);

        return await rootCommand.InvokeAsync(_args);
    }

    public async Task StartServer()
    {
        await _host.RunAsync();
    }

    public async Task<int> CreateUser(
        string username,
        string email,
        string? password,
        string? fullname,
        bool passwordStdIn = false)
    {
        if (string.IsNullOrEmpty(password))
        {
            if (!passwordStdIn)
            {
                Console.WriteLine("You have to provide either --password or --password-stdin option.");
                return 1;
            }

            while (string.IsNullOrEmpty(password = Console.ReadLine()))
            {
            }
        }

        using var scope = _host.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var user = new AppUser
        {
            UserName = username,
            Email = email,
            FullName = fullname,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to create account because of following errors: {@Errors}", result.Errors);
            return 2;
        }

        _logger.LogInformation("User account created! id: {UserId}", user.Id);
        return 0;
    }

    public async Task MigrateDatabase()
    {
        using var scope = _host.Services.CreateScope();

        _logger.LogInformation("Migrating database...");

        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<CourierDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to migrate database");
        }
    }
}