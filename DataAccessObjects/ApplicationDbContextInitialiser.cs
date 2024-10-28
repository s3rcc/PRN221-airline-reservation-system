﻿using BussinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects.SeedData
{
    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly Fall2024DbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            Fall2024DbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
    new Location {  LocationName = "Quảng Bình" },
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
            new Pilot { PilotName = "Nguyễn Văn A", Status = true },
            new Pilot { PilotName = "Lê Văn B", Status = true },
            new Pilot { PilotName = "Trần Thị C", Status = true },
            new Pilot { PilotName = "Phạm Văn D", Status = true },
            new Pilot { PilotName = "Hoàng Văn E", Status = true },
            new Pilot { PilotName = "Vũ Thị F", Status = false },
            new Pilot { PilotName = "Ngô Văn G", Status = true },
            new Pilot { PilotName = "Đỗ Thị H", Status = true }
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
            // Kiểm tra xem có chuyến bay nào đã tồn tại chưa

                var flights = new List<Flight>();
                // Lấy ngày hiện tại
                DateTime currentDate = DateTime.Now;
                DateTime endDate = new DateTime(2024, 11, 20);

                // Random giá vé và trạng thái
                var random = new Random();
                var locations = _context.Locations.ToList(); // Lấy tất cả địa điểm từ database
                var planes = _context.AirPlanes.ToList(); // Lấy tất cả máy bay từ database
                var pilots = _context.Pilots.ToList(); // Lấy tất cả phi công từ database

                for (DateTime date = currentDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    for (int i = 0; i < 3; i++) // Tạo 3 chuyến bay mỗi ngày
                    {
                        // Random giờ khởi hành giữa 6h và 20h
                        DateTime departureTime = date.AddHours(6 + i * 4); // Các chuyến cách nhau 4 tiếng
                        DateTime arrivalTime = departureTime.AddHours(2 + random.Next(1, 3)); // Giả sử thời gian bay từ 2-4 giờ

                        // Tạo chuyến bay cho mỗi ngày
                        flights.Add(new Flight
                        {
                            FlightNumber = $"VN{random.Next(100, 999)}", // Tạo số chuyến bay ngẫu nhiên
                            PlaneId = planes[random.Next(planes.Count)].PlaneId,
                            PilotId = pilots[random.Next(pilots.Count)].PilotId,
                            OriginID = locations[random.Next(locations.Count)].LocationID, // Chọn ngẫu nhiên điểm khởi hành
                            DestinationID = locations[random.Next(locations.Count)].LocationID, // Chọn ngẫu nhiên điểm đến
                            DepartureDateTime = departureTime, // Giờ khởi hành ngẫu nhiên trong ngày
                            ArrivalDateTime = arrivalTime, // Giờ đến sau khoảng thời gian bay
                            BasePrice = random.Next(100, 500) + random.Next(0, 99) / 100m, // Giá vé ngẫu nhiên từ 1 triệu đến 5 triệu
                            Status = true
                        });
                    }
                }
            if (!_context.Flights.Any())
            {
                // Thêm chuyến bay vào database
                await _context.Flights.AddRangeAsync(flights);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Flights seeded successfully.");
            }
        }


        #endregion Flights



    }
}
