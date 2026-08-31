using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IEnumerable<IAppDbContextModelConfiguration> _models;
        public AppDbContext(DbContextOptions<AppDbContext> options, IEnumerable<IAppDbContextModelConfiguration> models) : base(options)
        {
            _models = models.ToArray();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var model in _models)
            {
                model.ConfigureModel(modelBuilder);
            }
        }
    }
}
