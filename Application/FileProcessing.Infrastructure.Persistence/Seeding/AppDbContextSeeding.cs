using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.Seeding;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence.Seeding
{
    public static class AppDbContextSeeding
    {
        public static async Task SeedAppDbContextAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var seeders = scope.ServiceProvider.GetServices<IAppDbContextSeeder>();

            foreach (var seeder in seeders)
            {
                await seeder.SeedAsync(dbContext, cancellationToken);
            }
        }
    }
}

