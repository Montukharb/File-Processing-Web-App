using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Core.DomainLibrary.DomainModel
{
    public interface IAuditEntity
    {
        public Guid Id { get; set; }
        public long? CreatedBy_UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long? UpdatedBy_UserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? IpAddress { get; set; }
        string App_Version { get; set; }
    }
}
