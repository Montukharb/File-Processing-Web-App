using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace UserManagement.PL.EntityDbSet
{
    public static class UserManagementDbSet
    {
        public static void UserManagementModuleDbSet(this ModelBuilder builder)
        {
            builder.Entity<UserSession>(); //ef core should be added userSession in db model.
        }
    }
}
