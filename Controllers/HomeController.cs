using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using MiBandNaramek.Models.Helpers;
using MiBandNaramek.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MiBandNaramek.Controllers
{
  
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<MiBandNaramekUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public HomeController(ILogger<HomeController> logger, IAuthorizationService authorizationService, UserManager<MiBandNaramekUser> userManager, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _authorizationService = authorizationService;
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }


        [Authorize]
        public async Task<IActionResult> IndexAsync()
        {
            if ((await _authorizationService.AuthorizeAsync(User, "IsAllowedToUseApp")).Succeeded)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                DateTime dateTimeStart = DateTime.Now.Date.AddDays(-3).AddHours(6);
                DateTime dateTimeEnd = DateTime.Now.Date.AddHours(6);

                var loadedSummaryHelperData = this.LoadSummaryHelperData(userId, dateTimeStart, dateTimeEnd);

                return View("Summary", new SummaryViewModel()
                {
                    User = _userManager.FindByIdAsync(userId).Result,
                    SummaryChart = SummaryService.LoadSummaryChart(loadedSummaryHelperData, dateTimeStart, dateTimeEnd),
                    DailyCharts = SummaryService.LoadSummaryDailyCharts(_applicationDbContext, loadedSummaryHelperData, userId, dateTimeStart, dateTimeEnd, 1),
                    /* Activity = LoadActivityData(DateTimeStart, DateTimeEnd), */
                    Do = dateTimeEnd.ToString("dd.MM.yyyy 06:00", CultureInfo.InvariantCulture),
                    Od = dateTimeStart.ToString("dd.MM.yyyy 06:00", CultureInfo.InvariantCulture),
                    UserId = userId,
                    GroupByMin = 1
                });
            }
            return View();
        }


        [HttpPost]
        [Authorize(Policy = "IsAllowedToUseApp")]
        public ActionResult ChangeDate(SummaryViewModel Model)
        {
            // Nastavím časy dle požadavku od uživatele

            //  Vyberu od 12.5. do 14.5. - čas 6:00
            //
            //       12.5. od 6:00 do 13.5. do 6:00
            //       13.5. od 6:00 do 14.5. do 6:00
            //       14.5. od 6:00 do 15.5. do 6:00
            //
            //  K poslednímu datu musím přidat den

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            DateTime dateTimeStart = DateTime.Parse(Model.Od);
            DateTime dateTimeEnd = DateTime.Parse(Model.Do).AddDays(1);

            var loadedSummaryHelperData = this.LoadSummaryHelperData(userId, dateTimeStart, dateTimeEnd);

            return View("Summary", new SummaryViewModel()
            {
                User = _userManager.FindByIdAsync(userId).Result,
                SummaryChart = SummaryService.LoadSummaryChart(loadedSummaryHelperData, dateTimeStart, dateTimeEnd),
                DailyCharts = SummaryService.LoadSummaryDailyCharts(_applicationDbContext, loadedSummaryHelperData, userId, dateTimeStart, dateTimeEnd, Model.GroupByMin),
                /* Activity = LoadActivityData(DateTimeStart, DateTimeEnd), */
                Do = Model.Do,
                Od = Model.Od,
                UserId = userId,
                GroupByMin = Model.GroupByMin
            });
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

        private List<SummaryHelper> LoadSummaryHelperData(string userId, DateTime dateTimeStart, DateTime dateTimeEnd)
        {
            return MeasuredDataService.LoadSummaryHelperData(_applicationDbContext, userId, dateTimeStart, dateTimeEnd);
        }
    }
}
