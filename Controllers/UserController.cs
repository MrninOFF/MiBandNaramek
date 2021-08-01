using AspNetCoreHero.ToastNotification.Abstractions;
using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using MiBandNaramek.Models.Helpers;
using MiBandNaramek.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<MiBandNaramekUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INotyfService _notyfService;


        public UserController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager, RoleManager<IdentityRole> roleManager, INotyfService notyfService)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _notyfService = notyfService;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = _userManager.Users
                                            .Where(a => a.Id != currentUser.Id)
                                            .Select(select => new UserViewData() { User = new MiBandNaramekUser() { Id = select.Id, Email = select.Email, UserName = select.UserName, PhoneNumber = select.PhoneNumber }, 
                                                                                 BatteryData = _applicationDbContext.BatteryData.Where(where => where.UserId == select.Id)
                                                                                                                                 .OrderByDescending(order => order.Timestamp)
                                                                                                                                 .FirstOrDefault(),
                                                                                 LastSync = _applicationDbContext.MeasuredData.Where(where => where.UserId == select.Id)
                                                                                                                              .OrderByDescending(order => order.Timestamp)
                                                                                                                              .Select(select => select.Date)
                                                                                                                              .FirstOrDefault().ToString()})
                                            .ToList();
            return View(allUsersExceptCurrentUser);
        }

        public async Task<IActionResult> Update(string User)
        {
            var selectedUser = await _userManager.FindByIdAsync(User);
            return View(new UserUpdateViewModel(){
                User = selectedUser,
                UserRoles = _roleManager.Roles.ToList(),
                UserSelectedRole = (await _userManager.GetRolesAsync(selectedUser)).FirstOrDefault()
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(string userId, UserUpdateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var myUser = _userManager.FindByIdAsync(userId).Result;

                myUser.Height = model.User.Height;
                myUser.Wight = model.User.Wight;
                myUser.PhoneNumber = model.User.PhoneNumber;

                await _userManager.UpdateAsync(myUser);

                var currentRole = (await _userManager.GetRolesAsync(myUser)).FirstOrDefault();

                if (currentRole != model.UserSelectedRole)
                {
                    if (currentRole != null)
                        await _userManager.RemoveFromRoleAsync(myUser, currentRole);
                    await _userManager.AddToRoleAsync(myUser, (await _roleManager.FindByIdAsync(model.UserSelectedRole)).Name);
                }

                _notyfService.Success($"Uživatel {myUser.UserName} upraven");

                return RedirectToAction("Index");
            }
            model.User.Id = userId;
            model.UserSelectedRole = (await _userManager.GetRolesAsync(model.User)).FirstOrDefault();
            model.UserRoles = _roleManager.Roles.ToList();
            return View("Update", model);
        }
    }
}
