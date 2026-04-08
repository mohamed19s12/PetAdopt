using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<AdoptionRequest> AdoptionRequests { get; set; }
        public DbSet<PetImage> PetImages { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.TargetUser)
                .WithMany()
                .HasForeignKey(r => r.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Pet)
                .WithMany()
                .HasForeignKey(f => f.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdoptionRequest>()
                .HasOne(a => a.Adopter)
                .WithMany()
                .HasForeignKey(a => a.AdoprerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
