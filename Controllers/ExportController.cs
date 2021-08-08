using AspNetCoreHero.ToastNotification.Abstractions;
using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Constants;
using MiBandNaramek.Data;
using MiBandNaramek.Models.Helpers;
using MiBandNaramek.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiBandNaramek.Controllers
{
    [Authorize(Policy = "IsAllowedToManageApp")]
    public class ExportController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<MiBandNaramekUser> _userManager;
        private readonly INotyfService _notyfService;
        private readonly IToastifyService _notifyService;

        public ExportController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager, INotyfService notyfService, IToastifyService notifyService)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
            _notyfService = notyfService;
            _notifyService = notifyService;
        }


        public async Task<IActionResult> ExportUserMeasuredDataToExcel(string userId, string TextOd, string TextDo)
        {
            // var data = _applicationDbContext.MeasuredData.Any();

            DateTime Od = DateTime.Parse(TextOd);
            DateTime Do = DateTime.Parse(TextDo);

            var LoadedSummaryHelperData = MeasuredDataService.LoadSummaryHelperData(_applicationDbContext, userId, Od, Do);

            var stream = new MemoryStream();

            string nazev = $"{await _userManager.GetUserNameAsync(await _userManager.FindByIdAsync(userId))}___Od_{Od.ToString("dd.MM.yyyy")}___Do_{Do.ToString("dd.MM.yyyy")}";

            using (var xlPackage = new ExcelPackage(stream))
            {
                // Nastavíme property souboru
                xlPackage.Workbook.Properties.Title = nazev;
                xlPackage.Workbook.Properties.Author = User.Identity.Name;

                for (DateTime selectedDate = Od; selectedDate < Do; selectedDate = selectedDate.AddDays(1))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add(selectedDate.ToString("dd.MM.yyyy"));

                    worksheet.Cells["A1"].Value = "UserId";
                    worksheet.Cells["B1"].Value = "Datum a Čas";
                    worksheet.Cells["C1"].Value = "Tep";
                    worksheet.Cells["D1"].Value = "Kroky";
                    worksheet.Cells["E1"].Value = "Kroky Celkem";
                    worksheet.Cells["F1"].Value = "Intensity";
                    worksheet.Cells["G1"].Value = "Id Aktivity";
                    worksheet.Cells["H1"].Value = "Název Aktivity";

                    // První řádek
                    var row = 2;
                    double totalSteps = 0;
                    foreach (var measuredData in LoadedSummaryHelperData.Where(where => where.DateTimeValue >= selectedDate && where.DateTimeValue < selectedDate.AddDays(1)))
                    {
                        totalSteps += measuredData.Steps;

                        worksheet.Cells[row, 1].Value = userId;
                        worksheet.Cells[row, 2].Value = measuredData.DateTimeValue.ToString("dd.MM.yyyy HH:mm");
                        worksheet.Cells[row, 3].Value = measuredData.DoubleValue;
                        worksheet.Cells[row, 4].Value = measuredData.Steps;
                        worksheet.Cells[row, 5].Value = totalSteps;
                        worksheet.Cells[row, 6].Value = measuredData.Intensity;
                        worksheet.Cells[row, 7].Value = measuredData.Kind;
                        worksheet.Cells[row, 8].Value = ActivityKindConstants.GetConstantNameById(measuredData.Kind);
                        row++;
                    }
                }


                xlPackage.Save();

            }

            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{nazev}.xlsx");
        }

        public async Task<ContentResult> ExportUserMeasuredDataToJSON(string userId, string TextOd, string TextDo)
        {

            DateTime Od = DateTime.Parse(TextOd);
            DateTime Do = DateTime.Parse(TextDo);

            var user = await _userManager.FindByIdAsync(userId);

            UserMeasuredDataToJSON data = new UserMeasuredDataToJSON()
            {
                UserId = userId,
                UserWeight = user.Wight,
                UserHeight = user.Height,
                SummaryData = MeasuredDataService.LoadSummaryHelperData(_applicationDbContext, userId, Od, Do)
            };

            return Content(JsonSerializer.Serialize(data));
        }
    }
}
