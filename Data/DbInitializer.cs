using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MiniCartMvc.Data;
using MiniCartMvc.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task Seed(IServiceProvider serviceProvider)
    {
        // Rol ve kullanıcı servislerini al
        var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


        // Roller
        if (!await roleManager.RoleExistsAsync("admin"))
        {
            var adminRole = new ApplicationRole("admin", "Admin role with full permissions");
            var roleResult = await roleManager.CreateAsync(adminRole);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    Console.WriteLine($"Error creating role: {error.Description}");
                }
            }
        }

        if (!await roleManager.RoleExistsAsync("user"))
        {
            var userRole = new ApplicationRole("user", "Standard user role");
            var roleResult = await roleManager.CreateAsync(userRole);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    Console.WriteLine($"Error creating role: {error.Description}");
                }
            }
        }

        if (await userManager.FindByNameAsync("kadirmergen") == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = "kadirmergen",
                Email = "kadir@example.com",
                Name = "kadir",
                Surname = "mergen",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, "Kadir.123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
                await userManager.AddToRoleAsync(adminUser, "user");
            }
        }

        if (await userManager.FindByNameAsync("cinarturan") == null)
        {
            var normalUser = new ApplicationUser
            {
                UserName = "cinarturan",
                Email = "cinar@example.com",
                Name = "Cinar",
                Surname = "Turan",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(normalUser, "Cinar.123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "user");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error creating user 'cinarturan': {error.Description}");
                }
            }
        }
    }
}
