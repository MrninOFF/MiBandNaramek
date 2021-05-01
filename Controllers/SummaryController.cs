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
            ViewData["Chart"] = GenerateHeartRateChartAsync().Result;
            return View();
        }

        [HttpPost]
        public string ChangeDate(string newStartDate, string newEndDate)
        {
            TimestampStart = DateTimeOffset.Parse(newStartDate).ToUnixTimeSeconds();
            TimestampEnd = DateTimeOffset.Parse(newEndDate).ToUnixTimeSeconds();
            //  ViewData["Chart"] = GenerateHeartRateChartAsync().Result;
            var data = GenerateHeartRateChartAsync().Result;
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            return JsonSerializer.Serialize(data.Data, serializeOptions);
        }

        private async Task<Chart> GenerateHeartRateChartAsync()
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Labels = new List<string>();

            var hearRateData = _applicationDbContext.MeasuredData
                                .Where(option => option.Timestamp >= TimestampStart && option.Timestamp <= TimestampEnd)
                                .Select(select => new SummaryHelper { DoubleValue = Convert.ToDouble(select.HeartRate), DateTimeValue = select.Date });
            List<double?> hearRateDataForGraph = new List<double?>(); 
            foreach (var hearRate in hearRateData)
            {
                if (hearRate.DoubleValue > 0 && hearRate.DoubleValue < 255)
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
                PointHoverRadius = new List<int> { 5 },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { 2 },
                PointRadius = new List<int> { 1 },
                PointHitRadius = new List<int> { 10 },
                SpanGaps = false
            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);

            chart.Data = data;

            return chart;
        }
    }
}
