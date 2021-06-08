using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using MiBandNaramek.Models.Helpers;
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


        public UserController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
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
    }
}
