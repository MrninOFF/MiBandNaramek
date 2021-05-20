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

        // Časy pro načtení dat
        private long TimestampStart;
        private long TimestampEnd;

        // Načtená data pro další úpravu
        private List<SummaryHelper> LoadedSummaryHelperData;

        public SummaryController(ApplicationDbContext applicationDbContext, UserManager<MiBandNaramekUser> userManager)
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            // Nastavím defaultní časy
            TimestampStart = DateTimeOffset.Now.ToUnixTimeSeconds();
            TimestampEnd = DateTimeOffset.Now.AddDays(-3).ToUnixTimeSeconds();

            this.LoadSummaryHelperData();

            List<DailyChartData> Charts = new List<DailyChartData>();
            Charts.Add(new DailyChartData { Chart = GenerateHeartRateChartAsync().Result, Name = "Test" });

            return View(new SummaryViewData() {
                SummaryHeartRate = LoadSummaryHeartRate(TimestampStart, TimestampEnd),
                Charts = Charts,
                Activity = LoadActivityData(TimestampStart, TimestampEnd),
                Do = DateTimeOffset.FromUnixTimeSeconds(TimestampEnd).LocalDateTime.ToString("dd/MM/yyyy 11:59 PM", CultureInfo.InvariantCulture),
                Od = DateTimeOffset.FromUnixTimeSeconds(TimestampStart).LocalDateTime.ToString("dd/MM/yyyy 12:01 AM", CultureInfo.InvariantCulture)
            }); 
        }

        [HttpGet]
        public ActionResult ChangeDate(string Od, string Do)
        {
            // Nastavím časy dle požadavku od uživatele
            TimestampStart = DateTimeOffset.Parse(Od).ToUnixTimeSeconds();
            TimestampEnd = DateTimeOffset.Parse(Do).ToUnixTimeSeconds();

            this.LoadSummaryHelperData();

            List<DailyChartData> Charts = new List<DailyChartData>();
            Charts.Add(new DailyChartData { Chart = GenerateHeartRateChartAsync().Result, Name = "Test" });
            Charts.Add(new DailyChartData { Chart = GenerateHeartRateChartAsync().Result, Name = "Retest" });

            return View("Index", new SummaryViewData() {
                SummaryHeartRate = LoadSummaryHeartRate(TimestampStart, TimestampEnd),
                Charts = Charts,
                Activity = LoadActivityData(TimestampStart, TimestampEnd),
                Do = DateTimeOffset.FromUnixTimeSeconds(TimestampEnd).LocalDateTime.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture),
                Od = DateTimeOffset.FromUnixTimeSeconds(TimestampStart).LocalDateTime.ToString("dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture)
            });
        }

        private async Task<Chart> GenerateHeartRateChartAsync()
        {
            // Generovat Graf
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();
            data.Labels = new List<string>();

            //    var result = hearRateData.GroupBy(groupBy => groupBy.DateTimeValue).Select(select => new SummaryHelper { DoubleValue = select.Average(s => s.DoubleValue), DateTimeValue = select.Key });

            List<double?> hearRateDataForGraph = new List<double?>();
            List<double?> StepsDataForGraph = new List<double?>();
            List<double?> IntesityDataForGraph = new List<double?>();
            double stepsTotal = 0;
            foreach (var hearRate in this.LoadMeasuredData(TimestampStart, TimestampEnd))
            {
                if (hearRate.DoubleValue >= 0 && hearRate.DoubleValue <= 255)
                {
                    data.Labels.Add(hearRate.DateTimeValue.ToString());
                    hearRateDataForGraph.Add(hearRate.DoubleValue);
                    stepsTotal += Convert.ToDouble(hearRate.Steps);
                    StepsDataForGraph.Add(stepsTotal);
                    IntesityDataForGraph.Add(hearRate.Intensity);
                }
            }


            // Vytvoření datasets
            LineDataset dataset = new LineDataset()
            {
                Label = "Heart Data",
                Data = hearRateDataForGraph,
                Fill = "false",
                LineTension = 0,
                BackgroundColor = ChartColor.FromRgba(75, 192, 192, 0.4),
                BorderColor = ChartColor.FromRgb(75, 192, 192),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                BorderWidth = 1,
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> {  },
                PointHoverRadius = new List<int> {  },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(75, 192, 192) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> {  },
                PointRadius = new List<int> {  },
                PointHitRadius = new List<int> {  },
                
            };

            LineDataset dataset2 = new LineDataset()
            {
                Label = "Heart Data",
                Data = StepsDataForGraph,
                Fill = "false",
                LineTension = 0,
                BackgroundColor = ChartColor.FromRgba(175, 80, 140, 0.4),
                BorderColor = ChartColor.FromRgb(175, 80, 140),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                BorderWidth = 1,
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(175, 80, 140) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { },
                PointHoverRadius = new List<int> { },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(175, 80, 140) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { },
                PointRadius = new List<int> { },
                PointHitRadius = new List<int> { },
                YAxisID = "xxx-1",

            };

            LineDataset dataset3 = new LineDataset()
            {
                Label = "Heart Data",
                Data = IntesityDataForGraph,
                Fill = "false",
                LineTension = 0,
                BackgroundColor = ChartColor.FromRgba(10, 80, 140, 0.4),
                BorderColor = ChartColor.FromRgb(10, 80, 140),
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                BorderWidth = 1,
                PointBorderColor = new List<ChartColor> { ChartColor.FromRgb(10, 80, 140) },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { },
                PointHoverRadius = new List<int> { },
                PointHoverBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(10, 80, 140) },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { },
                PointRadius = new List<int> { },
                PointHitRadius = new List<int> { },
                YAxisID = "xxx-2",

            };

            data.Datasets = new List<Dataset>();
            data.Datasets.Add(dataset);
            data.Datasets.Add(dataset2);
            data.Datasets.Add(dataset3);

            chart.Data = data;

            // Vytvoření separátních YAXES pro graf

            // První Osa
            CartesianScale heartRateScale = new CartesianScale()
            {
                Id = "xxx-1",
                Type = "linear",
                Position = "right",
                Stacked = true,
                ScaleLabel = new ScaleLabel() { Display = true },
                Ticks = new CartesianLinearTick()
                {
                    Max = 500,
                    Display = true,
                    BeginAtZero = true,
                    FontSize = 50,
                    Padding = 10,
                    Callback = "function (tick, index, ticks) { return numeral(tick).format('$ 0,0');}"
                }
            };

            CartesianScale IntensityScale = new CartesianScale()
            {
                Id = "xxx-2",
                Type = "linear",
                Position = "right",
                Stacked = true,
                ScaleLabel = new ScaleLabel() { Display = true },
                Ticks = new CartesianLinearTick()
                {
                    Max = 200,
                    Display = true,
                    BeginAtZero = true,
                    Min = 0,
                    FontSize = 50,
                    Padding = 10,
                    Callback = "function (tick, index, ticks) { return numeral(tick).format('$ 0,0');}"
                }
            };

            chart.Options.Scales = new Scales { YAxes = new List<Scale>() };
            chart.Options.Scales.YAxes.Add(heartRateScale);
            chart.Options.Scales.YAxes.Add(IntensityScale);

            return chart;
        }

        // Funkce pro načtení RAW dat ve vybraném období
        private void LoadSummaryHelperData()
        {
            LoadedSummaryHelperData = new List<SummaryHelper>();
            LoadedSummaryHelperData = _applicationDbContext.MeasuredData
                                        .Where(option => option.Timestamp >= TimestampStart && option.Timestamp <= TimestampEnd)
                                        .Select(select => new SummaryHelper { DoubleValue = Convert.ToDouble(select.HeartRate), DateTimeValue = select.Date, Steps = select.Steps, Intensity =  select.Intensity })
                                        .OrderBy(orderBy => orderBy.DateTimeValue)
                                        .ToList();
        }

        private List<SummaryHelper> LoadMeasuredData (long Od, long Do)
        {
            // Zjistím si, jak musím data na grafu rozčlenit
            // Defaultní rozdělení - Plná šířka grafu = 12h v minutách = 720 hodnot
            int timeMergreLimit = Convert.ToInt32((double)((TimestampEnd - TimestampStart) / (12 * 60 * 60)));


            if (timeMergreLimit > 1)
            {
                // Pokud je limit více jak 1, pak potřebuji upravit načtené hodnoty

                // Proveď přetočítání času podle timeMergeLimit
                LoadedSummaryHelperData.ForEach(select => select.DateTimeValue = RoundUp(select.DateTimeValue, TimeSpan.FromMinutes(timeMergreLimit)));

                // Provedu PRŮBĚRYZACI TEPŮ V JEDEN ČAS pomocí GROUP BY
                // Tady zamezím, aby se mi nezobrazovali blbosti: 255 a 70 tep v jednu jednotu času (naramek byl sundaný, nesejmul hodnotu) => Musí vrátit 70
                var measuredData = LoadedSummaryHelperData
                                                .Where(where => where.DoubleValue > 0 && where.DoubleValue < 255)
                                                .GroupBy(groupBy => groupBy.DateTimeValue)
                                                .Select(select => new SummaryHelper { DoubleValue = select.Average(a => a.DoubleValue), DateTimeValue = select.Key, Steps = select.Sum(a => a.Steps), Intensity = select.Average(a => a.Intensity) }).ToList();

                // Provedu to samé pro vadné hodnoty
                var measuredDataInvalidData = LoadedSummaryHelperData
                                                            .Where(where => where.DoubleValue <= 0 || where.DoubleValue >= 255)
                                                            .GroupBy(groupBy => groupBy.DateTimeValue)
                                                            .Select(select => new SummaryHelper { DoubleValue = 0, DateTimeValue = select.Key, Steps = select.Sum(a => a.Steps), Intensity = select.Average(a => a.Intensity) }).ToList();

                // Vytvořím union z těchto dvou tabulek + seřadím pro finální výpis
                var measuredDataFinal = measuredDataInvalidData.Union(measuredData, new SummaryComparer()).OrderBy(orderBy => orderBy.DateTimeValue).ToList();
                return measuredDataFinal;
            }
            else
            {
                var measuredDataFinal = LoadedSummaryHelperData.ToList();
                return measuredDataFinal;
            }
        }

        private List<ActivityData> LoadActivityData (long Od, long Do)
        {
            List<ActivityData> activityData = _applicationDbContext.ActivityData.Select(select => new ActivityData() { Id = select.Id, DateStart = select.DateStart, DateEnd = select.DateEnd, Steps = select.Steps, Kind = select.Kind }).ToList();
            // activityData.ForEach(select => select.Steps = ActivityDataService.CountStepsForActivity(_applicationDbContext, select.Id, select.Steps));
            return activityData;
        }

        private List<SummaryHeartRate> LoadSummaryHeartRate (long Od, long Do)
        {
            List<SummaryHeartRate> summaryHeartRate = new List<SummaryHeartRate>();
            return summaryHeartRate;
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
    }
}
