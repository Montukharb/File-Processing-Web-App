using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.Seeding;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.PL.Seeding
{
    public class UserSeeding : IAppDbContextSeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
        {
            var roleEntity = dbContext.Roles();

            foreach (var seed in RoleSeedData.Create())
            {

                var eachSectionData = await roleEntity.FindAsync([seed.Id]);

                if (eachSectionData is null)
                {
                    roleEntity.Add(seed);
                    continue;
                }
                eachSectionData.Id = seed.Id;
                eachSectionData.RoleName = seed.RoleName;
            }
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
