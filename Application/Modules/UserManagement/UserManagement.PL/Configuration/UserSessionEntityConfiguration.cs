using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagement.PL.Configuration
{
    public class UserSessionEntityConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSession", "Identity");
            builder.Property(x => x.UserId).IsRequired();
        }
    }
}
