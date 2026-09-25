using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence
{
    public class PersistenceAssemblyName
    {
        public Assembly assemblyName { get; set; } = typeof(PersistenceAssemblyName).Assembly;
    };
    
}
