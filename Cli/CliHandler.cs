using CuaHangBangDiaNhac.Data.Seeders;
using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Identity;
namespace CuaHangBangDiaNhac.Cli;

public class CliHandler
{
    public static async Task Handle(string[] args, WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        switch (args[0].ToLower())
        {
            case "seed-admin":
                await SeedAdmin(services);
                break;

            case "seed-data":
                await SeedData(services);
                break;

            case "delete-user":
                if (args.Length < 2)
                {
                    Console.WriteLine("❌ Vui lòng nhập email user cần xóa. Ví dụ: dotnet run delete-user abc@gmail.com");
                }
                else
                {
                    await DeleteUser(services, args[1]);
                }
                break;

            case "delete-user-by-name":
                if (args.Length < 2) Console.WriteLine("❌ Thiếu username. Dùng: dotnet run delete-user-by-name [username]");
                else await DeleteUserByUsername(services, args[1]);
                break;

            default:
                Console.WriteLine("Unknown Command");
                break;
        }
    }

    public static async Task SeedAdmin(IServiceProvider services)
    {
        try
        {
            await SuperAdminSeeder.SeedSuperAdminAsync(services);
            Console.WriteLine("✅ Super Admin seeded successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error seeding Super Admin: " + ex.Message);
        }
    }

    
    public static async Task SeedData(IServiceProvider services)
    {
        try
        {
            await DataSeeder.SeedDataAsync(services);
            Console.WriteLine("✅ Data seeded successful.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error seeding data: " + ex.Message);
        }
    }

    public static async Task DeleteUser(IServiceProvider services, string email)
    {
        try
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                Console.WriteLine($"⚠️ Không tìm thấy user nào có email: {email}");
                return;
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                Console.WriteLine($"✅ Đã xóa thành công user: {email}");
            }
            else
            {
                Console.WriteLine($"❌ Xóa thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi hệ thống: {ex.Message}");
        }
    }

    public static async Task DeleteUserByUsername(IServiceProvider services, string username)
    {
        try
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                Console.WriteLine($"⚠️ Không tìm thấy username: {username}");
                return;
            }

            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                Console.WriteLine($"✅ Đã xóa thành công user: {username} (Email: {user.Email})");
            }
            else
            {
                Console.WriteLine($"❌ Xóa thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi hệ thống: {ex.Message}");
        }
    }
}