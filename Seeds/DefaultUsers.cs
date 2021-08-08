using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Constants;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedAdminAsync(UserManager<MiBandNaramekUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new MiBandNaramekUser
            {
                UserName = "Admin@Admin",
                Email = "Admin@Admin",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Test_123");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
        }
        public static async Task SeedUserAsync(UserManager<MiBandNaramekUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            for (int i = 0; i < 100; i++)
            {
                var defaultUser = new MiBandNaramekUser
                {
                    UserName = $"User{i}@User{i}",
                    Email = $"User{i}@User{i}",
                    EmailConfirmed = true,
                    PhoneNumber = new Random().Next(111111111, 999999999).ToString(),
                    Height = new Random().Next(150, 190),
                    Wight = new Random().Next(40, 120)
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "Test_123");
                        await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                    }

                    user = await userManager.FindByEmailAsync(defaultUser.Email);

                    if (user != null && i%5 == 0)
                        await SeedMiBandData(user, dbContext);
                }
            }
        }

        public static async Task SeedMiBandData(MiBandNaramekUser user, ApplicationDbContext dbContext)
        {
            if(!dbContext.MeasuredData.Where(u => u.UserId == user.Id).Any())
            { 
                int beginTimestamp = new Random().Next(1625000400, 1625120400);
                int endTimestamp = new Random().Next(1625695199, 1625735199);
                int help = 0;
                for (int i = beginTimestamp; i <= endTimestamp; i += 60)
                {
                    var heartRateData = new MeasuredData
                    {
                        Timestamp = i,
                        Date = DateTimeOffset.FromUnixTimeSeconds(i).DateTime,
                        HeartRate = new Random().Next(50, 160),
                        Intensity = new Random().Next(0, 200),
                        Kind = 0,
                        Steps = new Random().Next(0, 15),
                        UploadDate = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        User = user
                    };
                    help += 1;
                    await dbContext.MeasuredData.AddAsync(heartRateData);
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
