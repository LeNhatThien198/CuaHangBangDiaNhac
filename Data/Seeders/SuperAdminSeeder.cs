using CuaHangBangDiaNhac.Models;
using Microsoft.AspNetCore.Identity;

namespace CuaHangBangDiaNhac.Data.Seeders 
{
    public class SuperAdminSeeder
    {
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            string superAdminEmail = config["Admin:Email"]!;
            string superAdminPassword = config["Admin:Password"]!;
            string superAdminFullname = config["Admin:FullName"]!;

            var adminUser = await userManager.FindByEmailAsync(superAdminEmail);
            if (adminUser == null)
            {
                adminUser = new User()
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true,
                    FullName = superAdminFullname,
                };

                var result = await userManager.CreateAsync(adminUser, superAdminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create super admin:\n" +
                        string.Join("\n", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}