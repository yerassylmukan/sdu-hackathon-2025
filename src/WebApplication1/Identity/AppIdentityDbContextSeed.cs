using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;

namespace WebApplication1.Identity;

public class AppIdentityDbContextSeed
{
    public static async Task SeedAsync(
        AppIdentityDbContext identityDbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if (identityDbContext.Database.IsNpgsql()) identityDbContext.Database.Migrate();

        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        var defaultUserEmail = "user@gmail.com";
        if (await userManager.FindByEmailAsync(defaultUserEmail) is null)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = defaultUserEmail,
                Email = defaultUserEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(defaultUser, AuthorizationConstants.DEFAULT_PASSWORD);
            await userManager.AddToRoleAsync(defaultUser, "User");
        }

        var adminEmail = "admin@gmail.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, AuthorizationConstants.DEFAULT_PASSWORD);
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}