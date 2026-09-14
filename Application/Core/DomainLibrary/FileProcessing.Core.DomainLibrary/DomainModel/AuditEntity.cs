
using System.ComponentModel.DataAnnotations;

namespace FileProcessing.Core.DomainLibrary.DomainModel
{
    public abstract class AuditEntity() : IAuditEntity
    {
        [Key]
        public abstract Guid Id { get; set; }
        public abstract long? CreatedBy_UserId { get; set; }
        public abstract DateTime CreatedAt { get; set; }
        public abstract long? UpdatedBy_UserId { get; set; }
        public abstract DateTime? UpdatedAt { get; set; }
        public abstract string? IpAddress { get; set; }
        public abstract string App_Version { get; set; }
    }
}
