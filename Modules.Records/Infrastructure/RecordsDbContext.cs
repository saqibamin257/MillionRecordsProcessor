using Microsoft.EntityFrameworkCore;
using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Records.Infrastructure
{
    public class RecordsDbContext: DbContext
    {
        //public DbSet<Record> Records { get; set; }
        public DbSet<Record> Records => Set<Record>();
        public RecordsDbContext(DbContextOptions<RecordsDbContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Record>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Status)
                    .HasMaxLength(50);
            });
        }
    }
}
