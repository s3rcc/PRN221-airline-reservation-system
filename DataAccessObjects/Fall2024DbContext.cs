using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Origin)
                .WithMany(l => l.OriginFlights)
                .HasForeignKey(f => f.OriginID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Flight_Origin_Location");

            modelBuilder.Entity<Flight>()
                .HasOne(f => f.Destination)
                .WithMany(l => l.DestinationFlights)
                .HasForeignKey(f => f.DestinationID)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Flight_Destination_Location");

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
            modelBuilder.Entity<Location>().HasData(
            new Location { LocationID = 1, LocationName = "Hà Nội" },
            new Location { LocationID = 2, LocationName = "Tp.Hồ Chí Minh" },
            new Location { LocationID = 3, LocationName = "Đà Nẵng" },
            new Location { LocationID = 4, LocationName = "Phú Quốc" },
            new Location { LocationID = 5, LocationName = "Nha Trang" },
            new Location { LocationID = 6, LocationName = "Buôn Ma Thuột" },
            new Location { LocationID = 7, LocationName = "Cà Mau" },
            new Location { LocationID = 8, LocationName = "Cần Thơ" },
            new Location { LocationID = 9, LocationName = "Chu Lai" },
            new Location { LocationID = 10, LocationName = "Côn Đảo" },
            new Location { LocationID = 11, LocationName = "Đà Lạt" },
            new Location { LocationID = 12, LocationName = "Điện Biên" },
            new Location { LocationID = 13, LocationName = "Đồng Hới" },
            new Location { LocationID = 14, LocationName = "Hải Phòng" },
            new Location { LocationID = 15, LocationName = "Huế" },
            new Location { LocationID = 16, LocationName = "Pleiku" },
            new Location { LocationID = 17, LocationName = "Quy Nhơn" },
            new Location { LocationID = 18, LocationName = "Rạch Giá" },
            new Location { LocationID = 19, LocationName = "Thanh Hóa" },
            new Location { LocationID = 20, LocationName = "Tuy Hòa" },
            new Location { LocationID = 21, LocationName = "Vân Đồn" },
            new Location { LocationID = 22, LocationName = "Vinh" }
        );
        }
    }
}
 