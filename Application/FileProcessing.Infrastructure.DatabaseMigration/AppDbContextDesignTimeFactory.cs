using FileProcessing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FileProcessing.Infrastructure.DatabaseMigration
{
    public class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            string defaultConnectionString = "Data Source = localhost; Integrated Security = True; Persist Security Info = False; Server = MONTU-KHARB-DES; Encrypt = True; TrustServerCertificate = True; Initial Catalog = FileProcessing_Web_APP";
            var connectionString = GetConnectionString(args) ?? Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? defaultConnectionString;

            //DbContextOptionBuilder
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString, options =>
            {
                options.MigrationsAssembly(typeof(AppDbContextDesignTimeFactory).Assembly.GetName().Name);
            }).Options;

            return new AppDbContext(optionBuilder, [] );
        }

        //GetConnectionString
        public string? GetConnectionString(string[] args)
        {
            for (int i = 0; i < args.Length-1; i++)
            {
                if (args is null || args.Length == 0)
                {
                    return null;
                }
                if (args[i] is "--connectionString" or "-connection")
                {
                    return args[i + 1];
                }
            } 
            return null;

        }
    }
}
