using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IEnumerable<IAppDbContextModelConfiguration> _models;
        public AppDbContext(DbContextOptions<AppDbContext> options, IEnumerable<IAppDbContextModelConfiguration> models) : base(options)
        {
            _models = models.ToArray();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityPasskeyData>();
            foreach (var model in _models)
            {
                model.ConfigureModel(modelBuilder);
            }
        }
    }
}
