using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UserManagement.AM.Composition
{
    public sealed record UserManagementAssemblyProvider
    {
        public Assembly AssemblyName = typeof(UserManagementAssemblyProvider).Assembly;
    }
}
