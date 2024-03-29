﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace PropertyManagerWeb.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Landlord> Landlords { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<ProprtyType> PropertyTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<PriceRange> PriceRanges { get; set; }
        public DbSet<VoteLog> VoteLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Bath> Baths { get; set; }
        public DbSet<Toilet> Toilets { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<PropertyCategory> PropertyCategories { get; set; }
        public DbSet<ParkingLot> ParkingLot { get; set; }
        public DbSet<SiteMessage> SiteMessages { get; set; }

    }
}