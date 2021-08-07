using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<MiBandNaramekUser> _userManager;

        public HomeController(ILogger<HomeController> logger, IAuthorizationService authorizationService, UserManager<MiBandNaramekUser> userManager)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }


        [AllowAnonymous]
        public async Task<IActionResult> IndexAsync()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "IsAllowedToUseApp")).Succeeded)
            {
                // return View("Summary", );
            }
            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
