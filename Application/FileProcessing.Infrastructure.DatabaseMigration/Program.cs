using Azure;
using FileProcessing.Infrastructure.DatabaseMigration;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.Seeding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Seeding;
using System.Diagnostics;
using UserManagement.PL.Seeding;

namespace FileProcessing.Infrastructure.DatabaseMigration
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await MigrationConsole.RunAsync(args);
        }
    }
    internal static class MigrationConsole
    {
        internal static async Task<int> RunAsync(string[] args)
        {
            string DefaultConnectionString = "Data Source = localhost; Integrated Security = True; Persist Security Info = False; Server = MONTU-KHARB-DES; Encrypt = True; TrustServerCertificate = True; Initial Catalog = FileProcessing_Web_APP";

            if (args.Length is 0)
            {
                return 1;
            }
            var options = MigrationCommandLine.Parse(args);


            if (options.ShowHelp || !options.HasActions)
            {
                Console.WriteLine(MigrationCommandLine.Usage);
                return options.ShowHelp ? 0 : 1;
            }

            try
            {
                if (options.CreateMigration)
                {
                    Console.WriteLine("Creating migration...");
                    await Operation.CreateMigration(options);
                }

                if (options.ApplyMigrations)
                {
                    Console.WriteLine("Applying migration...");
                    await Operation.ApplyMigration(args);
                }

                if (options.SeedData)
                {
                    Console.WriteLine("Seeding data...");
                    await Operation.SeedData(DefaultConnectionString, args);
                }

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }

            return 0;
        }
    }
}

internal sealed class Operation
{
    internal static async Task CreateMigration(MigrationCommandLine options)
    {
        var process = new ProcessStartInfo("dotnet")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = Directory.GetCurrentDirectory()
        };

        process.ArgumentList.Add("ef");

        process.ArgumentList.Add("migrations");
        process.ArgumentList.Add("add");
        process.ArgumentList.Add(options.MigrationName!);
        process.ArgumentList.Add("--project");
        process.ArgumentList.Add(MigrationCommandLine.ProjectPath!);
        process.ArgumentList.Add("--startup-project");
        process.ArgumentList.Add(MigrationCommandLine.ProjectPath!);
        process.ArgumentList.Add("--context");
        process.ArgumentList.Add(typeof(AppDbContext).FullName!);
        process.ArgumentList.Add("--output-dir");
        process.ArgumentList.Add("Migrations");

        using var migrationProcess = Process.Start(process);
        var output = await migrationProcess!.StandardOutput.ReadToEndAsync();
        var error = await migrationProcess.StandardError.ReadToEndAsync();
        await migrationProcess!.WaitForExitAsync();

        Console.WriteLine(output);
        Console.Error.WriteLine(error);
    }

    internal static async Task ApplyMigration(string[] args)
    {
        var factory = new AppDbContextDesignTimeFactory();

        var dbContext = factory.CreateDbContext(args: args);
        await dbContext.Database.MigrateAsync();
    }

    internal static async Task SeedData(string defaultConnectionString, string[] args)
    {

        var service = new ServiceCollection();

        //service schema di register here
        service.AddSingleton<IAppDbContextSeeder, UserSeeding>();


        service.AddScoped<AppDbContext>(_ =>
        {
            var factory = new AppDbContextDesignTimeFactory();

            return factory.CreateDbContext(args);
        });


        var serviceProvider = service.BuildServiceProvider();

        //seeding
        await serviceProvider.SeedAppDbContextAsync();
    }


}




internal sealed class MigrationCommandLine
{
    public const string Usage = """
        Migration commands:
          -c, --create [name]       Create a new EF migration. Uses the current branch name when name is omitted on non-main/dev/test branches.
          -a, --apply               Apply pending migrations to the database.
          -s, --seeding             Run registered seeders.

        Options:
          --connection <string>     Override the database connection string.
          --connection-string <s>   Override the database connection string.
          -n, --name <name>         Migration name used with -c.
          -h, --help                Show this help.

        Examples:
          dotnet run -- -c
          dotnet run -- -a -s
          dotnet run -- -c -n MigrationName --connection-string "Data Source=MyServer;Initial Catalog=MyDatabase;Integrated Security=True"
        """;

    public bool ShowHelp { get; private set; }

    public bool CreateMigration { get; private set; }
    public bool ApplyMigrations { get; private set; }
    public bool SeedData { get; private set; }
    public string? MigrationName { get; private set; }
    public static string? ProjectName { get; private set; }
    public static string? ProjectPath { get; private set; }
    public string? ConnectionString { get; private set; }

    public bool HasActions => CreateMigration || ApplyMigrations || SeedData;

    public static void ExceptionCheck(int index, string[] args)
    {
        if (index >= args.Length)
        {
            throw new Exception("Migration name not found in arguments");
        }
    }
    public static string Migration_Name(string[] args, string FindArgument, ref int Index)
    {
        ExceptionCheck(Index, args);

        if (FindArgument is "--name" or "-n")
        {
            ++Index;
        }
        var DirectoryPath = Directory.GetCurrentDirectory();
        var ProjectName = Directory.GetParent(DirectoryPath)?.Name;
        MigrationCommandLine.ProjectName = ProjectName;
        MigrationCommandLine.ProjectPath = DirectoryPath;

        var migrationName = args[Index] + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ProjectName;

        return migrationName;
    }

    public static string Connection_String(string[] args, string FindArgument, ref int Index)
    {
        ExceptionCheck(Index, args);

        if (FindArgument is "--connection" or "--connection-string" or "-connection" or "-connectionString")
        {
            ++Index;
        }

        var connectionString = args[Index];

        return connectionString;
    }


    public static MigrationCommandLine Parse(string[] args)
    {
        var options = new MigrationCommandLine();

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];

            switch (argument)
            {
                case "-h":
                case "--help":
                    options.ShowHelp = true;
                    break;

                case "-c":
                case "--create":
                    options.CreateMigration = true;
                    break;

                case "-a":
                case "--apply":
                    options.ApplyMigrations = true;
                    break;

                case "-s":
                case "--seed":
                case "--seeding":
                    options.SeedData = true;
                    break;

                case "-n":
                case "--name":
                    options.MigrationName = Migration_Name(args, FindArgument: argument, Index: ref index);
                    break;

                case "--connection":
                case "--connection-string":
                    options.ConnectionString = Connection_String(args, FindArgument: argument, Index: ref index);
                    break;

                default:
                    throw new InvalidOperationException("Unknown argument");
            }


        }

        return options;
    }
}