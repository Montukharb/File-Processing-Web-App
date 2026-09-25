using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagement.PL.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {

            builder.ToTable("Users", "Identity");
            builder.HasKey(e => e.Id); //primary key
            builder.HasMany(applicationUser => applicationUser.Session)
                .WithOne(session => session.User)
                .HasForeignKey(session => session.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
