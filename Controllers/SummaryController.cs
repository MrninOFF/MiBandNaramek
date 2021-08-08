using ChartJSCore.Helpers;
using ChartJSCore.Models;
using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Data;
using MiBandNaramek.Models.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;
using MiBandNaramek.Models;
using MiBandNaramek.Services;
using System.Security.Claims;
using AspNetCoreHero.ToastNotification.Abstractions;
using MiBandNaramek.Constants;

namespace MiBandNaramek.Controllers
{
    [AllowAnonymous]
    public class SummaryController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<MiBandNaramekUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly IToastifyService _notifyService;

        // Časy pro načtení dat
        private DateTime DateTimeStart;
        private DateTime DateTimeEnd;

        // Načtená data pro další úpravu
        private List<SummaryHelper> LoadedSummaryHelperData;

        // Přihlášený uživatel
        private string UserId;

        public SummaryController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager, INotyfService notyfService, IToastifyService notifyService)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _notyfService = notyfService;
            _notifyService = notifyService;
        }

        public IActionResult Index(string User)
        {
            // Nastavím defaultní časy

            if (String.IsNullOrEmpty(UserId))
                UserId = User;

            DateTimeStart = DateTime.Now.Date.AddDays(-3).AddHours(6);
            DateTimeEnd = DateTime.Now.Date.AddHours(6);

            this.LoadSummaryHelperData();

            return View(new SummaryViewModel() {
                User = _userManager.FindByIdAsync(UserId).Result,
                SummaryChart = SummaryService.LoadSummaryChart(LoadedSummaryHelperData, DateTimeStart, DateTimeEnd),
                DailyCharts = SummaryService.LoadSummaryDailyCharts(_applicationDbContext, LoadedSummaryHelperData, UserId, DateTimeStart, DateTimeEnd, 1),
                /* Activity = LoadActivityData(DateTimeStart, DateTimeEnd), */
                Do = DateTimeEnd.ToString("dd.MM.yyyy 06:00", CultureInfo.InvariantCulture),
                Od = DateTimeStart.ToString("dd.MM.yyyy 06:00", CultureInfo.InvariantCulture),
                UserId = UserId,
                GroupByMin = 1
            });
        }

        [HttpPost]
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

            if (String.IsNullOrEmpty(UserId))
                UserId = Model.UserId;

            DateTimeStart = DateTime.Parse(Model.Od);
            DateTimeEnd = DateTime.Parse(Model.Do).AddDays(1);

            this.LoadSummaryHelperData();

            // _notifyService.Success($"Načteno");

            return View("Index", new SummaryViewModel() {
                User = _userManager.FindByIdAsync(UserId).Result,
                SummaryChart = SummaryService.LoadSummaryChart(LoadedSummaryHelperData, DateTimeStart, DateTimeEnd),
                DailyCharts = SummaryService.LoadSummaryDailyCharts(_applicationDbContext, LoadedSummaryHelperData, UserId, DateTimeStart, DateTimeEnd, Model.GroupByMin),
                /* Activity = LoadActivityData(DateTimeStart, DateTimeEnd), */
                Do = Model.Do,
                Od = Model.Od,
                UserId = UserId,
                GroupByMin = Model.GroupByMin
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUserNote(string note, string userId, string date)
        {
            try
            {
                // Ten kdo právě upravuje
                var doctor = await _userManager.FindByNameAsync(User.Identity.Name);
                var user = await _userManager.FindByIdAsync(userId);
                DateTime dateTime = DateTime.Parse(date);

                SummaryNote summaryNote = new SummaryNote()
                {
                    Id = _applicationDbContext.SummaryNote.Where(where => where.UserId == user.Id && where.Date == dateTime.Date).Select(select => select.Id).FirstOrDefault(),
                    Date = dateTime.Date,
                    DoctorId = doctor.Id,
                    UserId = user.Id,
                    Note = note,
                    UpdateDate = DateTime.Now
                };

                _ = _applicationDbContext.SummaryNote.Update(summaryNote);
                _ = _applicationDbContext.SaveChanges();

                // _notifyService.Success($"Poznámka k měření {date} uložena");

                // return Content("<h1>Super</h1>");
                return Content($"Poznámka k {dateTime.ToString("dd.MM.yyyy")} uložena");
                //return View("Index", model);
            }
            catch (System.Exception er)
            {
                return Json(new { success = false, responseText = "Chyba při odesílání" });
                // return NotFound();
            }
        }

        // Funkce pro načtení RAW dat ve vybraném období
        private void LoadSummaryHelperData()
        {
            LoadedSummaryHelperData = MeasuredDataService.LoadSummaryHelperData(_applicationDbContext, UserId, DateTimeStart, DateTimeEnd);
        }
    }
}
