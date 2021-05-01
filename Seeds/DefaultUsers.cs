using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Constants;
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
        public static async Task SeedUserAsync(UserManager<MiBandNaramekUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultUser = new MiBandNaramekUser
            {
                UserName = "User@User",
                Email = "User@User",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Test_123");
                    await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                }
            }
        }
    }
}
