using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence.Seeding
{
    public interface IAppDbContextSeeder
    {
        public Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default);

    }
}
