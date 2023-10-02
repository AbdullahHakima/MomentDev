using Bloggi.Data.Configurations;
using Bloggi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Bloggi.Data;

public class ApplicationDbContext : IdentityDbContext<User,UserRole,int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        #region Define relation between Post & Tag models
        builder.Entity<Post>().HasMany(p => p.Tags).WithMany(t => t.Posts).UsingEntity<PostTag>(
            j => j.HasOne(pt => pt.Tag).WithMany(t => t.PostTags).HasForeignKey(pt => pt.TagId),
            j => j.HasOne(pt => pt.Post).WithMany(p => p.PostTags).HasForeignKey(p => p.PostId),
            j => j.HasKey(t => new { t.PostId, t.TagId })
            );
        #endregion

        new UserConfigurations().Configure(builder.Entity<User>());
        new PostConfigurations().Configure(builder.Entity<Post>());
        new TagConfigurations().Configure(builder.Entity<Tag>());

        base.OnModelCreating(builder);

    }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<User> users { get; set; }
    public DbSet<UserRole> roles { get; set; }
    public DbSet<Subscriber> subscribers { get; set; }
}
