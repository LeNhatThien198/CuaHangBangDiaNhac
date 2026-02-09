using CuaHangBangDiaNhac.Data;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Identity;

namespace CuaHangBangDiaNhac.ServicesRegisters;

public static class AuthService
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services, IConfiguration configuration
    )
    {
        services.AddIdentity<User, IdentityRole>(options => {
            options.SignIn.RequireConfirmedAccount = false;
            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            });

        services.ConfigureApplicationCookie(options => {
            options.LoginPath = "/Account/Login";        
            options.AccessDeniedPath = "/Account/AccessDenied"; 
        });

        return services;
    }
}