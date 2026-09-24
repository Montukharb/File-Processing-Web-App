using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.PL.Seeding
{
    public sealed class UserSeeding(IServiceScopeFactory ScopeFactory) : IAppDbContextSeeder
    {
        public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
        {
            using var scope = ScopeFactory.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "SuperAdmin", "Admin", "Manager", "Employee", "Customer", "Vendor", "Guest" };

            foreach (var role in roles)
            {

                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }

            }
            //await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}



//var roleEntity = dbContext.Roles();

//foreach (var seed in RoleSeedData.Create())
//{

//    var eachSectionData = await roleEntity.FindAsync([seed.Id]);

//    if (eachSectionData is null)
//    {
//        roleEntity.Add(seed);
//        continue;
//    }
//    eachSectionData.Id = seed.Id;
//    eachSectionData.RoleName = seed.RoleName;
//}
//await dbContext.SaveChangesAsync(cancellationToken);