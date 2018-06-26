using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobTheBot
{
    public class AppDbContext : DbContext
    {
        #region public DbSets

        public DbSet<SearchKey> SearchKeys { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        #endregion


        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            SearchKeysModel(builder);
            RecipientsModel(builder);
            base.OnModelCreating(builder);
        }

        private void SearchKeysModel(ModelBuilder builder)
        {
            builder.Entity<SearchKey>(entity =>
            {
                entity.HasKey(p => p.Id);


                entity.Property(p => p.Word)
                    .HasMaxLength(128)
                    .IsRequired();

            });
        }

        private void RecipientsModel(ModelBuilder builder)
        {
            builder.Entity<Recipient>(entity =>
            {
                entity.HasKey(p => p.Id);

                entity.Property(p => p.Name)
                    .HasMaxLength(128)
                    .IsRequired();

                entity.Property(p => p.Email)
                    .HasMaxLength(128)
                    .IsRequired();
            });
        }
    }
}
