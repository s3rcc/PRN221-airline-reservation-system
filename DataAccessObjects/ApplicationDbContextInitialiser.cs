using BussinessObjects;
using BussinessObjects.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataAccessObjects.SeedData
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly Fall2024DbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ClassTypesConfig _classTypesConfig;
        private readonly PaymentStatusConfig _paymentStatusConfig;
        private readonly TicketTypesConfig _ticketTypesConfig;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            Fall2024DbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager,IOptions<ClassTypesConfig> classTypesConfig,
            IOptions<PaymentStatusConfig> paymentStatusConfig,
            IOptions<TicketTypesConfig> ticketTypesConfig)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _classTypesConfig = classTypesConfig.Value;
            _paymentStatusConfig = paymentStatusConfig.Value;
            _ticketTypesConfig = ticketTypesConfig.Value;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    if (!_context.Database.CanConnect())
                    {
                        await _context.Database.EnsureDeletedAsync();
                        await _context.Database.MigrateAsync();
                    }
                    else
                    {
                        await _context.Database.MigrateAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            _logger.LogInformation("Starting to seed data...");
            await SeedRolesAsync();
            await SeedLocationsAsync();
            await SeedTiersAsync();
            await SeedPilotsAsync();
            await SeedPlanesAsync();
            await SeedFlightsAsync();
            await SeedUsersAsync();
            await SeedBookingsAsync();
            await SeedTicketsAsync();
            await SeedPaymentsAsync();

            _logger.LogInformation("Data seeding completed.");
        }

        #region Roles
        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                new IdentityRole { Name = "Admin" },
                new IdentityRole { Name = "Staff" },
                new IdentityRole { Name = "Member" }
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    await _roleManager.CreateAsync(role);
                    _logger.LogInformation($"Role {role.Name} created successfully.");
                }
            }
        }
        #endregion Roles

        #region Locations
        private async Task SeedLocationsAsync()
        {
            var locations = new List<Location>
{
    new Location { LocationName = "Hà Nội" },
    new Location { LocationName = "TP. Hồ Chí Minh" },
    new Location { LocationName = "Đà Nẵng" },
    new Location { LocationName = "Hải Phòng" },
    new Location { LocationName = "Cần Thơ" },
    new Location { LocationName = "An Giang" },
    new Location { LocationName = "Bà Rịa - Vũng Tàu" },
    new Location { LocationName = "Bạc Liêu" },
    new Location { LocationName = "Bắc Kạn" },
    new Location { LocationName = "Bắc Giang" },
    new Location { LocationName = "Bắc Ninh" },
    new Location { LocationName = "Bến Tre" },
    new Location { LocationName = "Bình Dương" },
    new Location { LocationName = "Bình Định" },
    new Location { LocationName = "Bình Phước" },
    new Location { LocationName = "Bình Thuận" },
    new Location { LocationName = "Cà Mau" },
    new Location { LocationName = "Cao Bằng" },
    new Location { LocationName = "Đắk Lắk" },
    new Location { LocationName = "Đắk Nông" },
    new Location {  LocationName = "Điện Biên" },
    new Location { LocationName = "Đồng Nai" },
    new Location {  LocationName = "Đồng Tháp" },
    new Location { LocationName = "Gia Lai" },
    new Location { LocationName = "Hà Giang" },
    new Location { LocationName = "Hà Nam" },
    new Location { LocationName = "Hà Tĩnh" },
    new Location { LocationName = "Hải Dương" },
    new Location { LocationName = "Hậu Giang" },
    new Location { LocationName = "Hòa Bình" },
    new Location { LocationName = "Hưng Yên" },
    new Location { LocationName = "Khánh Hòa" },
    new Location { LocationName = "Kiên Giang" },
    new Location { LocationName = "Kon Tum" },
    new Location { LocationName = "Lai Châu" },
    new Location { LocationName = "Lâm Đồng" },
    new Location { LocationName = "Lạng Sơn" },
    new Location { LocationName = "Lào Cai" },
    new Location { LocationName = "Long An" },
    new Location { LocationName = "Nam Định" },
    new Location { LocationName = "Nghệ An" },
    new Location { LocationName = "Ninh Bình" },
    new Location { LocationName = "Ninh Thuận" },
    new Location { LocationName = "Phú Thọ" },
    new Location { LocationName = "Phú Yên" },
    new Location { LocationName = "Quảng Bình" },
    new Location { LocationName = "Quảng Nam" },
    new Location { LocationName = "Quảng Ngãi" },
    new Location { LocationName = "Quảng Ninh" },
    new Location { LocationName = "Quảng Trị" },
    new Location { LocationName = "Sóc Trăng" },
    new Location { LocationName = "Sơn La" },
    new Location { LocationName = "Tây Ninh" },
    new Location { LocationName = "Thái Bình" },
    new Location { LocationName = "Thái Nguyên" },
    new Location { LocationName = "Thanh Hóa" }
};


            if (!_context.Locations.Any())
            {
                await _context.Locations.AddRangeAsync(locations);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Locations seeded successfully.");
            }
        }
        #endregion Locations

        #region Pilots
        private async Task SeedPilotsAsync()
        {

            var pilots = new List<Pilot>
        {
            new Pilot { PilotName = "Nguyen Van A", Status = true },
            new Pilot { PilotName = "Le Van B", Status = true },
            new Pilot { PilotName = "Tran Thi C", Status = true },
            new Pilot { PilotName = "Pham Van D", Status = true },
            new Pilot { PilotName = "Hoang Van E", Status = true },
            new Pilot { PilotName = "Vu Thi F", Status = false },
            new Pilot { PilotName = "Ngo Van G", Status = true },
            new Pilot { PilotName = "Đo Thi H", Status = true },
            new Pilot { PilotName = "Phan Van I", Status = true },
        new Pilot { PilotName = "Duong Thi K", Status = false },
        new Pilot { PilotName = "Dinh Van L", Status = true },
        new Pilot { PilotName = "Truong Thi M", Status = true },
        new Pilot { PilotName = "Nguyen Van N", Status = true },
        new Pilot { PilotName = "Ly Van O", Status = false },
        new Pilot { PilotName = "Tran Thi P", Status = true },
        new Pilot { PilotName = "Mai Van Q", Status = true }
        };
            if (!_context.Pilots.Any())
            {
                await _context.Pilots.AddRangeAsync(pilots);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pilots seeded successfully.");
            }
        }
        #endregion Pilots

        #region Planes
        private async Task SeedPlanesAsync()
        {

            var planes = new List<AirPlane>
        {
            new AirPlane { PlaneName = "Boeing 737", VipSeatNumber = 20, NormalSeatNumber = 150 },
            new AirPlane { PlaneName = "Airbus A320", VipSeatNumber = 15, NormalSeatNumber = 160 },
            new AirPlane { PlaneName = "Boeing 747", VipSeatNumber = 25, NormalSeatNumber = 300 },
            new AirPlane { PlaneName = "Airbus A380", VipSeatNumber = 30, NormalSeatNumber = 400 },
            new AirPlane { PlaneName = "Boeing 787", VipSeatNumber = 20, NormalSeatNumber = 250 },
            new AirPlane { PlaneName = "Airbus A321", VipSeatNumber = 10, NormalSeatNumber = 190 },
            new AirPlane { PlaneName = "Boeing 777", VipSeatNumber = 25, NormalSeatNumber = 300 }
        };
            if (!_context.AirPlanes.Any())
            {
                await _context.AirPlanes.AddRangeAsync(planes);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Planes seeded successfully.");
            }
        }
        #endregion Planes

        #region Tiers
        private async Task SeedTiersAsync()
        {
            var tiers = new List<Tier>
            {
                new Tier { TierName = "Kim Cương", PriorityLevel = 1, Discount = 0.2m, Description = "Ưu tiên cao nhất và giảm giá lớn nhất" },
                new Tier { TierName = "Vàng", PriorityLevel = 2, Discount = 0.15m, Description = "Ưu tiên cao và giảm giá tốt" },
                new Tier { TierName = "Bạc", PriorityLevel = 3, Discount = 0.1m, Description = "Ưu tiên trung bình và giảm giá hợp lý" },
                new Tier { TierName = "Đồng", PriorityLevel = 4, Discount = 0.05m, Description = "Ưu tiên cơ bản và giảm giá nhỏ" }
            };

            if (!_context.Tiers.Any())
            {
                await _context.Tiers.AddRangeAsync(tiers);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tiers seeded successfully.");
            }
        }
        #endregion Tiers

        #region Flights
        private async Task SeedFlightsAsync()
        {
            if (_context.Flights.Any())
            {
                _logger.LogInformation("Flights already exist.");
                return;
            }

            var random = new Random();
            DateTime flightDate1 = new DateTime(2024, 11, 18, 10, 0, 0);
            DateTime flightArrival1 = flightDate1.AddHours(2); 

            DateTime flightDate2 = new DateTime(2024, 11, 20, 10, 0, 0);
            DateTime flightArrival2 = flightDate2.AddHours(2);
            var origin1 = _context.Locations.FirstOrDefault(l => l.LocationID == 1);
            var destination1 = _context.Locations.FirstOrDefault(l => l.LocationID == 2);
            var pilot1 = _context.Pilots.FirstOrDefault();
            var plane1 = _context.AirPlanes.FirstOrDefault();

            if (pilot1 != null && plane1 != null)
            {
                var flight1 = new Flight
                {
                    FlightNumber = $"VN{random.Next(100, 999)}",
                    PlaneId = plane1.PlaneId,
                    PilotId = pilot1.PilotId,
                    OriginID = origin1.LocationID,
                    DestinationID = destination1.LocationID,
                    DepartureDateTime = flightDate1,
                    ArrivalDateTime = flightArrival1,
                    BasePrice = random.Next(100, 500) + random.Next(0, 99) / 100m,
                    Status = true,
                    AvailableNormalSeat = plane1.NormalSeatNumber,
                    AvailableVipSeat = plane1.VipSeatNumber,
                };
                _context.Flights.Add(flight1);
            }
            else
            {
                _logger.LogWarning("No available pilot or plane for flight 1.");
            }
            var pilot2 = pilot1; 
            var plane2 = plane1; 

            if (pilot2 != null && plane2 != null)
            {
                var flight2 = new Flight
                {
                    FlightNumber = $"VN{random.Next(100, 999)}",
                    PlaneId = plane2.PlaneId,
                    PilotId = pilot2.PilotId,
                    OriginID = destination1.LocationID,
                    DestinationID = origin1.LocationID,
                    DepartureDateTime = flightDate2,
                    ArrivalDateTime = flightArrival2,
                    BasePrice = random.Next(100, 500) + random.Next(0, 99) / 100m,
                    Status = true,
                    AvailableNormalSeat = plane2.NormalSeatNumber,
                    AvailableVipSeat = plane2.VipSeatNumber,
                };
                _context.Flights.Add(flight2);
            }
            else
            {
                _logger.LogWarning("No available pilot or plane for return flight.");
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation("Specific flights seeded successfully.");
        }




        #endregion Flights

        #region User
        private async Task SeedUsersAsync()
        {
            var users = new List<(User User, string Role)>
    {
        (new User
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                Gender = "Male",
                DoB = new DateTime(1980, 1, 1),
                CCCD = "123451116789",
                TierId = 1,
                EmailConfirmed = true,
            }, "Admin"),
        (new User
            {
                UserName = "staff@example.com",
                Email = "staff@example.com",
                Gender = "Female",
                DoB = new DateTime(1990, 2, 1),
                CCCD = "987654111321",
                TierId = 2,
                EmailConfirmed= true,
            }, "Staff"),
        (new User
        {
                UserName = "staff2@example.com",
                Email = "staff2@example.com",
                Gender = "Female",
                DoB = new DateTime(1990, 2, 1),
                CCCD = "987654111321",
                TierId = 2,
                EmailConfirmed= true,
        },"Staff"),
        (new User
            {
                UserName = "member@example.com",
                Email = "member@example.com",
                Gender = "Male",
                DoB = new DateTime(2000, 3, 1),
                CCCD = "456711189123",
                TierId = 3,
                EmailConfirmed= true,
            }, "Member"),
                (new User
            {
                UserName = "member2@example.com",
                Email = "member2@example.com",
                Gender = "Male",
                DoB = new DateTime(2000, 3, 1),
                CCCD = "456711189123",
                TierId = 3,
                  EmailConfirmed= true,
            }, "Member")
    };

            foreach (var (user, role) in users)
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser is null)
                {
                    var result = await _userManager.CreateAsync(user, "123456");
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                        _logger.LogInformation($"User {user.Email} created and assigned to role {role}.");
                    }
                    else
                    {
                        _logger.LogError($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                    }

                }
            }
        }
        #endregion User

        #region Booking
        private async Task SeedBookingsAsync()
        {
            var users = await _context.Users.Take(5).ToListAsync();
            var flights = await _context.Flights.ToListAsync();
            var random = new Random();
            var booking = new List<Booking>();
            for (var i = 0; i < 20; i++)
            {
                var user = users[random.Next(users.Count)];
                var flight = flights[random.Next(flights.Count)];
                Flight returnFlight;
                do
                {
                    returnFlight = flights[random.Next(flights.Count)];
                } while (returnFlight.OriginID == flight.DestinationID);
                int adultNum = random.Next(1, 3);
                int childNum = random.Next(0, 2);
                int babyNum = random.Next(0, 1);
                string paymentStatus = random.Next(0, 2) == 0 ? _paymentStatusConfig.Paid : _paymentStatusConfig.Unpaid;
                string classType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business;
                booking.Add(new Booking
                {
                    UserId = user.Id,
                    FlightId = flight.FlightId,
                    BookingDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    PaymentStatus = random.Next(0, 2) == 0 ? _paymentStatusConfig.Paid : _paymentStatusConfig.Unpaid,
                    ClassType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business,
                    AdultNum = random.Next(1, 3),
                    ChildNum = random.Next(0, 2),
                    BabyNum = random.Next(0, 1),
                    Status = true,
                    TotalPrice = flight.BasePrice * (1 + random.Next(0, 2) * 0.5m),
                    ReturnClassType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business
                }
                );
                booking.Add(new Booking
                {
                    UserId = user.Id,
                    FlightId = flight.FlightId,
                    ReturnFlightId = returnFlight.FlightId,
                    BookingDate = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                    PaymentStatus = random.Next(0, 2) == 0 ? _paymentStatusConfig.Paid : _paymentStatusConfig.Unpaid,
                    ClassType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business,
                    AdultNum = random.Next(1, 3),
                    ChildNum = random.Next(0, 2),
                    BabyNum = random.Next(0, 1),
                    Status = true,
                    TotalPrice = flight.BasePrice * (1 + random.Next(0, 2) * 0.5m),
                    ReturnClassType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business
                }
          );
                if(paymentStatus == _paymentStatusConfig.Paid)
                {
                    var totalPeople = adultNum + childNum + babyNum;
                    if(classType == _classTypesConfig.Economy && flight.AvailableNormalSeat >= totalPeople)
                    {
                        flight.AvailableNormalSeat -= totalPeople;
                    }
                    if (classType == _classTypesConfig.Business && flight.AvailableVipSeat >= totalPeople)
                    {
                        flight.AvailableVipSeat -= totalPeople;
                    }
                }
            }
            if (!_context.Bookings.Any())
            {
                await _context.Bookings.AddRangeAsync(booking);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Bookings seeded successfully.");
            }
        }
        #endregion Booking

        #region Tickets
        private async Task SeedTicketsAsync()
        {
            var random = new Random();
            var bookings = await _context.Bookings.ToListAsync(); // Get all bookings from database

            var tickets = new List<Ticket>();

            foreach (var booking in bookings)
            {
                tickets.Add(new Ticket
                {
                    SeatNumber = $"A{random.Next(1, 30)}", // Random seat number
                    TicketType = random.Next(0, 2) == 0 ? _ticketTypesConfig.OutBoundFlight : _ticketTypesConfig.ReturnFlight, // Random ticket type
                    IssuedDate = DateTime.UtcNow.AddDays(-random.Next(1, 15)), // Issued date within the last two weeks
                    Carryluggage = random.Next(5, 10), // Random carry luggage weight in kg
                    Baggage = random.Next(15, 30), // Random baggage weight in kg
                    ClassType = random.Next(0, 2) == 0 ? _classTypesConfig.Economy : _classTypesConfig.Business, // Random class type
                    BookingId = booking.BookingId // Link ticket to a booking
                });
            }

            if (!_context.Tickets.Any())
            {
                await _context.Tickets.AddRangeAsync(tickets);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Tickets seeded successfully.");
            }
        }
        #endregion Tickets
        #region Payments
        private async Task SeedPaymentsAsync()
        {
            // Retrieve all bookings with an Unpaid status
            var unpaidBookings = await _context.Bookings
                .Where(b => b.PaymentStatus == _paymentStatusConfig.Unpaid)
                .ToListAsync();

            var payments = new List<Payment>();
            var random = new Random();

            foreach (var booking in unpaidBookings)
            {
                // Create a Payment record for each unpaid booking
                payments.Add(new Payment
                {
                    BookingId = booking.BookingId,
                    UserId = booking.UserId,
                    Amount = booking.TotalPrice,
                    PaymentDate = DateTime.UtcNow,
                });

                // Update the booking status to Paid
                booking.PaymentStatus = _paymentStatusConfig.Paid;
            }

            if (payments.Any())
            {
                // Add all new payments to the Payments table
                await _context.Payments.AddRangeAsync(payments);

                // Save changes to apply the new payments and updated booking statuses
                await _context.SaveChangesAsync();

                _logger.LogInformation("Payments seeded and booking statuses updated to Paid successfully.");
            }
        }
        #endregion Payments
    }
}