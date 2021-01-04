using KnowledgeSpace.BackendServer.Data.Entities;
using KnowledgeSpace.BackendServer.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KnowledgeSpace.BackendServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<EntityEntry> modified = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);
            foreach (EntityEntry item in modified)
            {
                if (item.Entity is IDateTracking changedOrAddedItem)
                {
                    if (item.State == EntityState.Added)
                    {
                        changedOrAddedItem.CreateDate = DateTime.Now;
                    }
                    else
                    {
                        changedOrAddedItem.LastModifiedDate = DateTime.Now;
                    }
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);
            builder.Entity<User>().Property(x => x.Id).HasMaxLength(50).IsUnicode(false);

            builder.Entity<LabelInKnowledgeBase>()
                        .HasKey(c => new { c.LabelId, c.KnowledgeBaseId });

            builder.Entity<Permission>()
                       .HasKey(c => new { c.RoleId, c.FunctionId, c.CommandId });

            builder.Entity<Vote>()
                        .HasKey(c => new { c.KnowledgeBaseId, c.UserId });

            builder.Entity<CommandInFunction>()
                       .HasKey(c => new { c.CommandId, c.FunctionId });

            builder.HasSequence("KnowledgeBaseSequence");
        }

        public DbSet<Command> Commands { set; get; }
        public DbSet<CommandInFunction> CommandInFunctions { set; get; }

        public DbSet<ActivityLog> ActivityLogs { set; get; }
        public DbSet<Category> Categories { set; get; }
        public DbSet<Comment> Comments { set; get; }
        public DbSet<Function> Functions { set; get; }
        public DbSet<KnowledgeBase> KnowledgeBases { set; get; }
        public DbSet<Label> Labels { set; get; }
        public DbSet<LabelInKnowledgeBase> LabelInKnowledgeBases { set; get; }
        public DbSet<Permission> Permissions { set; get; }
        public DbSet<Report> Reports { set; get; }
        public DbSet<Vote> Votes { set; get; }

        public DbSet<Attachment> Attachments { get; set; }
    }
}