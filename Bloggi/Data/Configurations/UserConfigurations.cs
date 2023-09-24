using Bloggi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bloggi.Data.Configurations
{
    public class UserConfigurations:IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> Builder)
        {
            Builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            Builder.Property(u=>u.LastName).IsRequired().HasMaxLength(50);
        }
    }
}
