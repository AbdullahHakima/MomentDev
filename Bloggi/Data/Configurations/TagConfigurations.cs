using Bloggi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bloggi.Data.Configurations;

public class TagConfigurations:IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(t=>t.Name).IsRequired().HasMaxLength(50);
        builder.Property(t => t.Description).IsRequired();
    }
}
