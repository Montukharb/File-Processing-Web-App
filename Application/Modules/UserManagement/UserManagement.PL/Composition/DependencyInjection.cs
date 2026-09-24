using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.Seeding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using UserManagement.PL.ContextConfiguration;
using UserManagement.PL.Seeding;

namespace UserManagement.PL.Composition
{
    public static class DependencyInjection
    {
        public static IServiceCollection UserManagementDependencyInjection(this IServiceCollection service)
        {
            service.TryAddEnumerable(ServiceDescriptor.Singleton<IAppDbContextModelConfiguration, UserManagementBuilderConfiguration>());
            service.TryAddEnumerable(ServiceDescriptor.Singleton<IAppDbContextSeeder, UserSeeding>());
            return service;
        }
    }
}
