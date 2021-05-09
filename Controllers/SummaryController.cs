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

namespace MiBandNaramek.Controllers
{
    [AllowAnonymous]
    public class SummaryController : Controller
    {

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly UserManager<MiBandNaramekUser> _userManager;

        private long TimestampStart;
        private long TimestampEnd;

        public SummaryController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View(new SummaryViewData() {
                Chart = GenerateHeartRateChartAsync().Result,
                Activity = LoadActivityData(DateTimeOffset.Now.AddDays(-3).ToUnixTimeSeconds(), DateTimeOffset.Now.ToUnixTimeSeconds()),
                Do = DateTime.Now.ToString("dd/MM/yyyy 11:59 PM", CultureInfo.InvariantCulture),
                Od = DateTime.Now.AddDays(-3).ToString("dd/MM/yyyy 01:00 AM", CultureInfo.InvariantCulture) }); 
        }

        [HttpGet]
        public ActionResult ChangeDate(string Od, string Do)
        {
            TimestampStart = DateTimeOffset.Parse(Od).ToUnixTimeSeconds();
            TimestampEnd = DateTimeOffset.Parse(Do).ToUnixTimeSeconds();
            return View("Index", new SummaryViewData() {
                Chart = GenerateHeartRateChartAsync().Result,
                Activity = LoadActivityData(TimestampStart, TimestampEnd),
                Do = DateTimeOffset.FromUnixTimeSeconds(TimestampEnd).LocalDateTime.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                Od = DateTimeOffset.FromUnixTimeSeconds(TimestampStart).LocalDateTime.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture)
            });
        }

        private async Task<Chart> GenerateHeartRateChartAsync()
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Labels = new List<string>();

            var hearRateData = _applicationDbContext.MeasuredData
                                .Where(option => option.Timestamp >= TimestampStart && option.Timestamp <= TimestampEnd)
                                .Select(select => new SummaryHelper { DoubleValue = Convert.ToDouble(select.HeartRate), DateTimeValue = select.Date })
                                .OrderBy(orderBy => orderBy.DateTimeValue)
                                .ToList();

            var heartRateDateInListNew = hearRateData.ToList();
            var heartRateDateInListNewNotValuated = hearRateData.ToList();
            var heartRateDateInListFinal = hearRateData.ToList();
            if (TimestampEnd - TimestampStart > 24 * 60 * 60)
            {
                int NumDays = Convert.ToInt32((double)((TimestampEnd - TimestampStart) / (24 * 60 * 60)));
                hearRateData.ForEach(select => select.DateTimeValue = RoundUp(select.DateTimeValue, TimeSpan.FromMinutes(5 * NumDays)));

                heartRateDateInListNew = hearRateData.Where(where => where.DoubleValue > 0 && where.DoubleValue < 255).GroupBy(groupBy => groupBy.DateTimeValue).Select(select => new SummaryHelper { DoubleValue = select.Average(a => a.DoubleValue), DateTimeValue = select.Key }).ToList();
                heartRateDateInListNewNotValuated = hearRateData.Where(where => where.DoubleValue <= 0 || where.DoubleValue >= 255).GroupBy(groupBy => groupBy.DateTimeValue).Select(select => new SummaryHelper { DoubleValue = select.Average(a => a.DoubleValue), DateTimeValue = select.Key }).ToList();
                heartRateDateInListFinal = heartRateDateInListNewNotValuated.Union(heartRateDateInListNew, new SummaryComparer()).OrderBy(orderBy => orderBy.DateTimeValue).ToList();
            }

            //    var result = hearRateData.GroupBy(groupBy => groupBy.DateTimeValue).Select(select => new SummaryHelper { DoubleValue = select.Average(s => s.DoubleValue), DateTimeValue = select.Key });

            List<double?> hearRateDataForGraph = new List<double?>();
            foreach (var hearRate in heartRateDateInListFinal)
            {
                if (hearRate.DoubleValue >= 0 && hearRate.DoubleValue <= 255)
                {
                    data.Labels.Add(hearRate.DateTimeValue.ToString());
                    hearRateDataForGraph.Add(hearRate.DoubleValue);
                }
            }

            LineDataset dataset = new LineDataset()
            {
                Label = "Heart Data",
                Data = hearRateDataForGraph,
                Fill = "false",
                LineTension = 0.1,
                BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                BorderColor = ChartColor.FromRgb(75, 192, 192),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { 1 },
                PointHoverRadius = new List<int> { 1 },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { 1 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 1 },
                SpanGaps = false,
                
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            return chart;
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }

        class SummaryComparer : IEqualityComparer<SummaryHelper>
        {
            bool IEqualityComparer<SummaryHelper>.Equals(SummaryHelper x, SummaryHelper y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.DateTimeValue == y.DateTimeValue;
            }

            int IEqualityComparer<SummaryHelper>.GetHashCode(SummaryHelper obj)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(obj, null)) return 0;

                //Get hash code for the Name field if it is not null.
                int hashDateTimeValue = obj.DateTimeValue.Year < 2000 ? 0 : obj.DateTimeValue.GetHashCode();

                //Calculate the hash code for the product.
                return hashDateTimeValue;
            }
        }

        public List<ActivityData> LoadActivityData (long Od, long Do)
        {
            List<ActivityData> ActivityData = _applicationDbContext.ActivityData.Select(select => new ActivityData() { Id = select.Id, DateStart = select.DateStart, DateEnd = select.DateEnd, Steps = select.Steps, Kind = select.Kind }).ToList();
            ActivityData.ForEach(select => select.Steps = ActivityDataService.CountStepsForActivity(_applicationDbContext, select.Id, select.Steps));
            return ActivityData;
        }
    }
}
