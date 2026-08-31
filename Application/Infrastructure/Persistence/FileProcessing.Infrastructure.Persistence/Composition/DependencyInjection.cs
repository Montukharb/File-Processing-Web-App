using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence.Composition
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAppDbContextDependencyInjection(this IServiceCollection service, Action<IServiceProvider, DbContextOptionsBuilder> configureOptions)
        {
            service.AddDbContext<AppDbContext>((serviceProvider, options) => 
            {
                configureOptions(serviceProvider, options); 
            });


            return service;
        }
    }
}
