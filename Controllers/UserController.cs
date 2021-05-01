using MiBandNaramek.Areas.Identity.Data;
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
        private readonly UserManager<MiBandNaramekUser> _userManager;
        public UserController(UserManager<MiBandNaramekUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var allUsersExceptCurrentUser = _userManager.Users.Where(a => a.Id != currentUser.Id).ToList();
            return View(allUsersExceptCurrentUser);
        }
    }
}
