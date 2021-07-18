using DatingApp.API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DatingApp.API.Data
{
    public class SeedData
    {
        public static async Task SeedUsers(ILogger<Program> logger,
                                           UserManager<AppUser> userManager,
                                           RoleManager<AppRole> roleManager)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }

            try
            {
                string userData = await File.ReadAllTextAsync(@"Data\UserSeedData.json");
                List<AppUser> users = JsonSerializer.Deserialize<List<AppUser>>(userData);

                if (users == null)
                {
                    return;
                }

                List<AppRole> roles = new() 
                {
                    new AppRole {Name = "Member"},
                    new AppRole {Name = "Admin"},
                    new AppRole {Name = "Moderator"}
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }

                foreach (AppUser user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    await userManager.CreateAsync(user, "Passw0rd123!!");
                    await userManager.AddToRoleAsync(user, "Member");
                }

                AppUser admin = new()
                {
                    UserName = "admin"
                };

                await userManager.CreateAsync(admin, "Passw0rd123!!");
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during database seeding.");
            }
        }
    }
}
