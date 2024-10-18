using DataAccessObjects;
using DataAccessObjects.SeedData;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Interface;
using Services.Interfaces;
using Services.Services;


namespace Services
{
    public static class ServiceConfigure
    {
        public static IServiceCollection ConfigureService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<Fall2024DbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Dependencies injection
            // Unit of work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Services
            services.AddScoped<IAirPlaneService, AirPlaneService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IFlightService, FlightService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPilotService, PilotService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ITierService, TierService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ApplicationDbContextInitialiser>();

            //services.AddOptions();                                        
            var mailsettings = configuration.GetSection("MailSettings");  
            services.Configure<MailSettings>(mailsettings);               

            services.AddTransient<IEmailSender, SendMailService>();
            return services;
        }
    }
}
