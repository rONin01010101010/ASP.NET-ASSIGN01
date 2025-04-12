using COMP2139_assign01.Models;
using Microsoft.AspNetCore.Identity;

namespace COMP2139_assign01.Areas.User;

public class RoleInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
            string[] roleNames = { "Admin", "User" };
                
            // Create roles if they don't exist
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
                
            // Create admin user if it doesn't exist
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
                
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "System Administrator"
                };
                    
                await userManager.CreateAsync(adminUser, "Admin@123"); // Change to a secure password in production
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}