namespace DataAccessLibrary.Data;

public class SeedData
{
    public static async Task SeedUsersAsync(ILogger logger,
                                            UserManager<AppUser> userManager,
                                            RoleManager<AppRole> roleManager)
    {
        if (await userManager.Users.AnyAsync())
        {
            return;
        }

        try
        {
            string execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string userData = await File.ReadAllTextAsync(@$"{execPath}\Data\UserSeedData.json");
            List<AppUser> users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null)
            {
                return;
            }

            List<AppRole> roles = new() 
            {
                new AppRole {Name = "Member"},
                new AppRole {Name = "Administrator"},
                new AppRole {Name = "Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            foreach (AppUser user in users)
            {
                user.UserName = user.UserName;
                await userManager.CreateAsync(user, "Passw0rd123!!");
                await userManager.AddToRoleAsync(user, "Member");
            }

            AppUser admin = new()
            {
                UserName = "Brian",
                Gender = "male",
                DateOfBirth = DateTime.Parse("1984-09-13"),
                KnownAs = "Brian",
                Created = DateTime.Now,
                LastActive = DateTime.Now,
                Introduction = "Leave me alone, please.",
                LookingFor = "Judy, Piper and Horsie.",
                Interests = "C# + ASP.NET Core Blazor",
                City = "Center Moriches",
                State = "New York"
            };

            await userManager.CreateAsync(admin, "Passw0rd123!!");
            await userManager.AddToRolesAsync(admin, new[] { "Administrator", "Moderator", "Member" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during database user seeding.");
        }
    }

    public static async Task SeedUserLikesAndMessages(ILogger logger, DataContext db)
    {
        try
        {
            List<AppUser> users = await db.Users.ToListAsync();

            foreach (AppUser user in users)
            {
                Random random = new();
                int numLikes = random.Next(0, users.Count);

                for (int i = 0; i < numLikes; i++)
                {
                    int skipUsers = random.Next(0, users.Count);
                    AppUser userToLike = db.Users.OrderBy(r => Guid.NewGuid()).Skip(skipUsers).Take(1).FirstOrDefault();

                    if (userToLike is not null 
                        && userToLike.Id != user.Id
                        && userToLike.Gender != user.Gender
                        && db.Likes.Any(l => l.SourceUserId == user.Id && l.LikedUserId == userToLike.Id) == false)
                    {
                        db.Likes.Add(new UserLike 
                        { 
                            LikedUser = userToLike, 
                            LikedUserId = userToLike.Id, 
                            SourceUser = user, 
                            SourceUserId = user.Id 
                        });

                        db.Messages.Add(new Message
                        {
                            SenderId = user.Id,
                            SenderUsername = user.UserName,
                            Sender = user,
                            RecipientId = userToLike.Id,
                            RecipientUsername = userToLike.UserName,
                            Recipient = userToLike,
                            Content = "Hey gorgeous..."
                        });

                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured during database like seeding.");
        }
    }
}
