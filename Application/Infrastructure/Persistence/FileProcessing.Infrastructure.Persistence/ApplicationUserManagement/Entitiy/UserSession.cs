using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy
{
    public class UserSession
    {
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!;
        public string? DeviceName { get; set; }
        public string? DeviceType { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string RefreshTokenHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public bool IsRevoked { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
