using ChatApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        #region Constructor

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        #endregion

        #region Public Fields

        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Friends> Friends { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        #endregion

        #region Methods

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                   .HasOne(x => x.User)
                   .WithMany(x => x.Messages)
                   .HasForeignKey(x => x.UserId);

            builder.Entity<Message>()
                   .HasOne(x => x.Group)
                   .WithMany(x => x.Messages)
                   .HasForeignKey(x => x.GroupId);

            builder.Entity<Group>()
                   .HasOne(x => x.Owner)
                   .WithMany(x => x.OwnGroups)
                   .HasForeignKey(x => x.OwnerId);

            builder.Entity<UserGroup>()
                .HasKey(x => new { x.UserId, x.GroupId });

            builder.Entity<UserGroup>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserGroups)
                .HasForeignKey(x => x.UserId);

            builder.Entity<UserGroup>()
                .HasOne(x => x.Group)
                .WithMany(x => x.UserGroups)
                .HasForeignKey(x => x.GroupId);

            builder.Entity<User>()
                .HasOne(x => x.Settings)
                .WithOne(x => x.User)
                .HasForeignKey<Settings>(x => x.UserId);

            builder.Entity<User>()
                .HasOne(x => x.Image)
                .WithOne(x => x.User)
                .HasForeignKey<Image>(x => x.UserId);

            builder.Entity<Group>()
                .HasOne(x => x.Image)
                .WithOne(x => x.Group)
                .HasForeignKey<Image>(x => x.GroupId);

            builder.Entity<Notification>()
                .HasOne(x => x.ToUser)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ToUserId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        #endregion
    }
}
