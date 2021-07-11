using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(ILogger<Program> logger, UserManager<AppUser> userManager)
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

                foreach (AppUser user in users)
                {
                    user.UserName = user.UserName.ToLower();
                    await userManager.CreateAsync(user, "Passw0rd123!!");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occured during database seeding.");
            }
        }
    }
}
