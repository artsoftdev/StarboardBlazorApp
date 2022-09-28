using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarboardNFT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarboardNFT.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //This allows the audit fields to automatically be added, or updated.
        public override int SaveChanges()
        {

            var state = this.ChangeTracker.Entries().Select(x => x.State).ToList();

            state.ForEach(x => {

                switch(x)
                {
                    case EntityState.Added:
                        UpdateAddedEntity();
                        break;
                    case EntityState.Modified:
                        UpdateModifiedEntity();
                        break;
                }

            });

            return base.SaveChanges();
        }

        //Update Entity Pattern
        protected void UpdateAddedEntity()
        {
            var created = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(e => e.Entity).ToArray();

            foreach (var entity in created)
            {
                if (entity is AuditFields)
                {
                    var auditFields = entity as AuditFields;
                    auditFields.CreateDateTimeUtc = DateTime.UtcNow;
                    auditFields.ModifiedDateTimeUtc = DateTime.UtcNow;
                    auditFields.Active = true;
                }
            }
        }

        protected void UpdateModifiedEntity()
        {
            //Modified record changes
            var modified = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(e => e.Entity).ToArray();

            foreach (var entity in modified)
            {
                if (entity is AuditFields)
                {
                    var auditFields = entity as AuditFields;
                    auditFields.ModifiedDateTimeUtc = DateTime.UtcNow;
                }
            }
        }

        public DbSet<Auction> Auction { get; set; }
        public DbSet<AuctionBid> AuctionBid { get; set; }
        public DbSet<AuctionBidQueue> AuctionBidQueue { get; set; }
        public DbSet<NFT> NFT { get; set; }
        public DbSet<NFTData> NFTData { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserProfileHeader> UserProfileHeader { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<RecentViewNFT> RecentViewNFT { get; set; }
        public DbSet<NFTFavorites> NFTFavorites { get; set; }
        public DbSet<NFTLikes> NFTLikes { get; set; }
        public DbSet<Activity> Activity { get; set; }
        public DbSet<UserFollowing> UserFollowing { get; set; }
    }
}
