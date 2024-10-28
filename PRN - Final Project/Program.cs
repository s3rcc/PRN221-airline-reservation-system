using BussinessObjects;
using Services;
using Microsoft.EntityFrameworkCore;
using PRN___Final_Project.MiddleWare;
using DataAccessObjects;
using Microsoft.AspNetCore.Identity;
using System;
using DataAccessObjects.SeedData;

namespace PRN___Final_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddIdentity<User, IdentityRole>()
         .AddEntityFrameworkStores<Fall2024DbContext>()
         .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Thi?t l?p v? Password
                options.Password.RequireDigit = false; // Kh?ng b?t ph?i c? s?
                options.Password.RequireLowercase = false; // Kh?ng b?t ph?i c? ch? th??ng
                options.Password.RequireNonAlphanumeric = false; // Kh?ng b?t k? t? ??c bi?t
                options.Password.RequireUppercase = false; // Kh?ng b?t bu?c ch? in
                options.Password.RequiredLength = 6; // S? k? t? t?i thi?u c?a password
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;

            });
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Events.OnSigningIn = c =>
                {
                    if (c.Properties.IsPersistent)
                    {
                        var issued = c.Properties.IssuedUtc ?? DateTimeOffset.UtcNow;
                        c.Properties.ExpiresUtc = issued.AddDays(16);
                    }
                    return Task.FromResult(0);
                };

            });
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.ConfigureService(builder.Configuration);

            var app = builder.Build();
 
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
                    initialiser.InitialiseAsync().Wait();
                    initialiser.SeedAsync().Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseExceptionHandler("/Errors/500");
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStatusCodePagesWithReExecute("/Errors/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
