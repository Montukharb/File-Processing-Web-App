using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserSession> Session { get; set; } = new List<UserSession>();
    }
}
