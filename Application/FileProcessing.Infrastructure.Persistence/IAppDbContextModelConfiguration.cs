using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence
{
    public interface IAppDbContextModelConfiguration
    {
        void ConfigureModel(ModelBuilder modelBuilder);
    }
}
