using Bloggi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

namespace Bloggi.Data.Configurations
{
    public class PostConfigurations:IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            
            builder.Property(p=>p.Content).IsRequired();
            builder.Property(p=>p.Title).IsRequired().HasMaxLength(100);
            builder.Property(p=>p.Brief).HasMaxLength(150).IsRequired();
            builder.Property(p=>p.ReadingTime).IsRequired();
            builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETDATE()");
            builder.Property(p=>p.Image).IsRequired();
            builder.Property(p => p.ImageUrl).IsRequired();
        }
    }
}
