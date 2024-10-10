using DataAccessObjects;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;
using Repositories.Interface;
using Services.Interfaces;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ITierService, TierService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddOptions();                                        
            var mailsettings = configuration.GetSection("MailSettings");  
            services.Configure<MailSettings>(mailsettings);               

            services.AddTransient<IEmailSender, SendMailService>();
            return services;
        }
    }
}
