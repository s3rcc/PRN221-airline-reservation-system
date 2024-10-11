using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class Fall2024DbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public Fall2024DbContext(DbContextOptions<Fall2024DbContext> options) : base(options) 
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<AirPlane> AirPlanes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Pilot> Pilots { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Tier> Tiers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the relationship between Flight and Location (Origin and Destination)
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Origin)
                .WithMany(l => l.OriginFlights) // Use OriginFlights collection for origin relationships
                .HasForeignKey(f => f.OriginID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Flight_Origin_Location");

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Destination)
                .WithMany(l => l.DestinationFlights) // Use DestinationFlights collection for destination relationships
                .HasForeignKey(f => f.DestinationID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Flight_Destination_Location");

            // Configure other relationships here as needed
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Plane)
                .WithMany(p => p.Flights)
                .HasForeignKey(f => f.PlaneId);

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Pilot)
                .WithMany(p => p.Flights)
                .HasForeignKey(f => f.PilotId);

            base.OnModelCreating(modelBuilder);
            var admin = new IdentityRole("admin");
            admin.NormalizedName = "admin" ;
            var staff = new IdentityRole("staff");
            staff.NormalizedName = "staff";
            var member = new IdentityRole("member");
            member.NormalizedName = "member";
            modelBuilder.Entity<IdentityRole>().HasData(admin,staff,member);

        }
    }
}
 