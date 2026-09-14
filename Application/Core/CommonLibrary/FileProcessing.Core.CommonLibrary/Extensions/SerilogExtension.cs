using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Sinks.MSSqlServer;
using Serilog;

namespace FileProcessing.Core.CommonLibrary.Extensions
{
    public static class SerilogExtension
    {
        public static void ConfigureSerilog(this IHostBuilder host, IConfiguration configurations)
        {
            var sink_Options = new MSSqlServerSinkOptions()
            {
                TableName = "Logs",
                AutoCreateSqlTable = true,
                SchemaName = "FileProcessingLogs",
            };

            var LogConnectionString = configurations["LogConnectionString"];

            host.UseSerilog((context, config) =>
            {
                if (context.HostingEnvironment.IsDevelopment())
                {
                    config.WriteTo.Console();
                    config.WriteTo.File("Logs/log.txt",
                        shared: false,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 50);
                }
                else
                {
                    config.WriteTo.MSSqlServer(connectionString: LogConnectionString, sinkOptions: sink_Options);
                }
            });
        }
    }
}