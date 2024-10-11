using BussinessObjects;
using Services;
using Microsoft.EntityFrameworkCore;
using PRN___Final_Project.MiddleWare;
using DataAccessObjects;
using Microsoft.AspNetCore.Identity;
using System;

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

            });
            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.ConfigureService(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
