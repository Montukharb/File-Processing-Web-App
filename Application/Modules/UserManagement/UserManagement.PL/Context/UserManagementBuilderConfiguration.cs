using FileProcessing.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.PL.EntityDbSet;

namespace UserManagement.PL.ContextConfiguration
{
    public sealed class UserManagementBuilderConfiguration : IAppDbContextModelConfiguration
    {
        public void ConfigureModel(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserManagementBuilderConfiguration).Assembly);

            var UserAssemblyName = new PersistenceAssemblyName();
            modelBuilder.ApplyConfigurationsFromAssembly(UserAssemblyName.assemblyName);
            modelBuilder.UserManagementModuleDbSet();
        }
    }
}
