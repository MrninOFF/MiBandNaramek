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
                SummaryChart = LoadSummaryChart(DateTimeStart, DateTimeEnd),
                DailyCharts = LoadSummaryDailyCharts(DateTimeStart, DateTimeEnd, 1),
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
                SummaryChart = LoadSummaryChart(DateTimeStart, DateTimeEnd),
                DailyCharts = LoadSummaryDailyCharts(DateTimeStart, DateTimeEnd, Model.GroupByMin),
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

        /// <summary>
        /// Funkce slouží pro vytvoření denních grafů a tabulek
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>
        /// Funkce vrací List DailyChartData, který obsahuje tolik záznamů, kolik je vybraných dnů
        /// </returns>
        private List<DailyChartData> LoadSummaryDailyCharts(DateTime startDateTime, DateTime endDateTime, int groupBy)
        {
            // For cyklus projede všechny datumy ve vybraném rozsahu

            List<DailyChartData> Charts = new List<DailyChartData>();

            for (DateTime selectedDate = startDateTime; selectedDate < endDateTime; selectedDate = selectedDate.AddDays(1))
            {
                // Velký spojnicový graf
                Charts.Add(new DailyChartData()
                {
                    Chart = GenerateMainDailyChart(selectedDate, selectedDate.AddDays(1), groupBy),
                    PieChart = GeneratePieDailyChart(selectedDate, selectedDate.AddDays(1)),
                    Name = selectedDate.ToString("dd.MM.yyyy"),
                    VariableName = "DEN" + selectedDate.ToString("ddMMyyyy") + "GRAF",
                    VariablePieName = "DEN" + selectedDate.ToString("ddMMyyyy") + "PIEGRAF",
                    Activity = LoadActivityData(DateTimeStart, DateTimeEnd), 
                    Date = selectedDate.Date,
                    Note = _applicationDbContext.SummaryNote.Where(where => where.UserId == UserId && where.Date == selectedDate.Date).Select(select => select.Note).FirstOrDefault()
                }) ;
                // Koláčový graf aktivit

            }

            return Charts;
        }

        private Chart LoadSummaryChart(DateTime startDateTime, DateTime endDateTime)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Bar;

            chart.Data = new ChartJSCore.Models.Data();

            List<double?> lowHeartRate = new List<double?>();
            List<double?> mediumHeartRate = new List<double?>();
            List<double?> highHeartRate = new List<double?>();
            List<double?> steps = new List<double?>();

            List<string> labels = new List<string>();

            List<SummaryHelper> Summary = this.LoadMeasuredData(startDateTime, endDateTime, 1);

            for (DateTime selectedDate = startDateTime; selectedDate < endDateTime; selectedDate = selectedDate.AddDays(1))
            {
                labels.Add(selectedDate.ToString("dd.MM.yyyy"));
                steps.Add(Summary.Where(where => where.DateTimeValue >= selectedDate && selectedDate.AddDays(1) > where.DateTimeValue).Sum(sum => sum.Steps));
                lowHeartRate.Add(Summary.Where(where => where.DoubleValue > 0 && where.DoubleValue < 255).Where(where => where.DateTimeValue >= selectedDate && selectedDate.AddDays(1) > where.DateTimeValue).Where(where => where.DoubleValue <= Constants.HeartRateConstants.MediumHeartRate).Select(select => select.DoubleValue).Count());
                mediumHeartRate.Add(Summary.Where(where => where.DoubleValue > 0 && where.DoubleValue < 255).Where(where => where.DateTimeValue >= selectedDate && selectedDate.AddDays(1) > where.DateTimeValue).Where(where => where.DoubleValue > Constants.HeartRateConstants.MediumHeartRate && where.DoubleValue <= Constants.HeartRateConstants.HighHeartRate).Select(select => select.DoubleValue).Count());
                highHeartRate.Add(Summary.Where(where => where.DoubleValue > 0 && where.DoubleValue < 255).Where(where => where.DateTimeValue >= selectedDate && selectedDate.AddDays(1) > where.DateTimeValue).Where(where => where.DoubleValue > Constants.HeartRateConstants.HighHeartRate).Select(select => select.DoubleValue).Count());
            }

            chart.Data.Datasets = new List<Dataset>();
            chart.Options.Scales = new Scales { YAxes = new List<Scale>() };

            chart.Data.Labels = labels;

            chart.Data.Datasets.Add(new LineDataset() { Data = steps, Label = "Kroky", PointBackgroundColor = new List<ChartColor> { ChartColor.FromRgb(80, 10, 80) }, YAxisID = "Kroky" });

            chart.Data.Datasets.Add(new BarDataset() { Data = lowHeartRate, Label = "Nízký tep", BackgroundColor= new List<ChartColor> { ChartColor.FromRgb(10, 10, 220) } });
            chart.Data.Datasets.Add(new BarDataset() { Data = mediumHeartRate, Label = "Střední tep", BackgroundColor = new List<ChartColor> { ChartColor.FromRgb(10, 220, 10) } });
            chart.Data.Datasets.Add(new BarDataset() { Data = highHeartRate, Label = "Vysoký tep", BackgroundColor = new List<ChartColor> { ChartColor.FromRgb(220, 10, 10) } });

            CartesianScale intensityScale = new CartesianScale()
            {
                Id = "Kroky",
                Type = "linear",
                Position = "right",
                Stacked = true,
                ScaleLabel = new ScaleLabel() { Display = true },
                Ticks = new CartesianLinearTick()
                {
                    Display = true,
                    FontSize = 50,
                    Padding = 10,
                    Callback = "function (tick, index, ticks) { return numeral(tick).format('$ 0,0');}"
                }
            };

            chart.Options.Scales.YAxes.Add(intensityScale);

            return chart;
        }

        private Chart GeneratePieDailyChart(DateTime startDateTime, DateTime endDateTime)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Pie;

            chart.Data = new ChartJSCore.Models.Data();

            List<double?> Data = new List<double?>();

            List<string> labels = new List<string>();

            var Summary = this.LoadMeasuredData(startDateTime, endDateTime, 1)
                                                .GroupBy(groupBy => groupBy.Kind)
                                               // .Select(select => new SummaryHelper { DoubleValue = 0, Kind = select.Key, Steps = select.Count(a => a.Kind)}).ToList();
                                                .Select(g => new { Kind = g.Key, Count = g.Count() })
                                                .ToDictionary(k => k.Kind, i => i.Count);

            PieDataset dataset = new PieDataset()
            {
                Label = "Graf Aktivit",
                BackgroundColor = new List<ChartColor>() {},
                BorderWidth = 0,
                HoverBorderWidth = 0,
                HoverBackgroundColor = new List<ChartColor>() {}
            };

            foreach (var data in Summary)
            {
                if (data.Value > 20)
                {
                    labels.Add(ActivityKindConstants.GetConstantNameById(data.Key));
                    Data.Add(data.Value);
                    dataset.BackgroundColor.Add(ActivityDataService.GetChartColorByActivityKind(data.Key));
                    dataset.HoverBackgroundColor.Add(ActivityDataService.GetChartColorByActivityKind(data.Key));
                }
            }

            chart.Data.Labels = labels;

            dataset.Data = Data;

            chart.Data.Datasets = new List<Dataset>();
            chart.Data.Datasets.Add(dataset);

            return chart;
        }

        /// <summary>
        /// Funkce vygeneruje 3 Datasety, které obsahují HeartRate, Steps, Intensity, pro vstupní data
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns>
        /// Vrací Graf, který je připravený k použití
        /// </returns>
        private Chart GenerateMainDailyChart(DateTime startDateTime, DateTime endDateTime, int groupBy)
        {
            Chart chart = new Chart();

            chart.Type = Enums.ChartType.Line;

            ChartJSCore.Models.Data data = new ChartJSCore.Models.Data();

            data.Datasets = new List<Dataset>();

            chart.Options.Scales = new Scales { YAxes = new List<Scale>() };

            // Vytvoření Dataset pro následn
            prepareDataForDataset(startDateTime, endDateTime, groupBy, out List<string> Labels, out List<double?> hearRateDataForGraph, out List<double?> stepsDataForGraph, out List<double?> intesityDataForGraph);
            data.Labels = Labels;

            LineDataset lineDataSet;
            CartesianScale intensityScale;

            generateLineForChart("Steps", null, stepsDataForGraph, ChartColor.FromRgb(125, 55, 255), 0, out lineDataSet, out intensityScale);
            data.Datasets.Add(lineDataSet);

            generateLineForChart("Srdeční tep", "heart", hearRateDataForGraph, ChartColor.FromRgb(255, 40, 70), 0, out lineDataSet, out intensityScale);
            data.Datasets.Add(lineDataSet);
            chart.Options.Scales.YAxes.Add(intensityScale);

            generateLineForChart("Intenzita", "intensity", intesityDataForGraph, ChartColor.FromRgb(100, 255, 1), 0, out lineDataSet, out intensityScale);
            lineDataSet.Fill = "true";
            data.Datasets.Add(lineDataSet);
            chart.Options.Scales.YAxes.Add(intensityScale);

            chart.Data = data;

            return chart;

        }

        /// <summary>
        /// Funkce vytvoří Dataset pro jednitlivé dimenze do grafu
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="hearRateDataForGraph"></param>
        /// <param name="stepsDataForGraph"></param>
        /// <param name="intesityDataForGraph"></param>
        private void prepareDataForDataset(DateTime startDateTime, DateTime endDateTime, int groupBy, out List<string> labels, out List<double?> hearRateDataForGraph, out List<double?> stepsDataForGraph, out List<double?> intesityDataForGraph)
        {
            labels = new List<string>();
            hearRateDataForGraph = new List<double?>();
            stepsDataForGraph = new List<double?>();
            intesityDataForGraph = new List<double?>();
            double stepsTotal = 0;
            foreach (var measure in this.LoadMeasuredData(startDateTime, endDateTime, groupBy))
            {
                // TODO Možnost upgradu
                if (measure.DoubleValue >= 0 && measure.DoubleValue <= 255)
                {
                    // Odstranění 255 dat - Nahrazení normalní hodnotou
                    if (measure.DoubleValue >= 255 || measure.DoubleValue < 1)
                    {
                        if (hearRateDataForGraph.Count > 0)
                            measure.DoubleValue = hearRateDataForGraph.Last().Value;
                    }

                    labels.Add(measure.DateTimeValue.ToString());
                    hearRateDataForGraph.Add(measure.DoubleValue);
                    stepsTotal += Convert.ToDouble(measure.Steps);
                    stepsDataForGraph.Add(stepsTotal);
                    intesityDataForGraph.Add(measure.Intensity > 0 ? measure.Intensity : 0);
                }
            }
        }


        private void generateLineForChart(string LineTitle, string YAxisID, List<double?> dataForGraph, ChartColor chartColor, int maxValue, out LineDataset lineDataSet, out CartesianScale intensityScale)
        {
            lineDataSet = new LineDataset()
            {
                Label = LineTitle,
                Data = dataForGraph,
                Fill = "false",
                LineTension = 0,
                BackgroundColor = chartColor,
                BorderColor = chartColor,
                BorderCapStyle = "butt",
                BorderDash = new List<int> { },
                BorderDashOffset = 0.0,
                BorderJoinStyle = "miter",
                BorderWidth = 1,
                PointBorderColor = new List<ChartColor> { chartColor },
                PointBackgroundColor = new List<ChartColor> { ChartColor.FromHexString("#ffffff") },
                PointBorderWidth = new List<int> { },
                PointHoverRadius = new List<int> { },
                PointHoverBackgroundColor = new List<ChartColor> { chartColor },
                PointHoverBorderColor = new List<ChartColor> { ChartColor.FromRgb(220, 220, 220) },
                PointHoverBorderWidth = new List<int> { },
                PointRadius = new List<int> { },
                PointHitRadius = new List<int> { }
            };

            if (!String.IsNullOrEmpty(YAxisID))
            {
                lineDataSet.YAxisID = YAxisID;
                intensityScale = new CartesianScale()
                {
                    Id = YAxisID ,
                    Display = true,
                    Type = "linear",
                    Position = "right",
                    Stacked = true,
                    ScaleLabel = new ScaleLabel() { Display = true },
                    Ticks = new CartesianLinearTick()
                    {
                        Display = true,
                        FontSize = 50,
                        Padding = 10,
                        BeginAtZero = true,
                        SuggestedMax = 250,
                        SuggestedMin = 0,
                        Callback = "function (tick, index, ticks) { return numeral(tick).format('$ 0,0');}"
                    }
                };
            }
            else
            {
                intensityScale = new CartesianScale();
            }
        }

        // Funkce pro načtení RAW dat ve vybraném období
        private void LoadSummaryHelperData()
        {
            LoadedSummaryHelperData = MeasuredDataService.LoadSummaryHelperData(_applicationDbContext, UserId, DateTimeStart, DateTimeEnd);
        }

        private List<SummaryHelper> LoadMeasuredData (DateTime startDateTime, DateTime endDateTime, int groupBy)
        {
            // Zjistím si, jak musím data na grafu rozčlenit
            // Defaultní rozdělení - Plná šířka grafu = 12h v minutách = 720 hodnot
            int timeMergreLimit = Convert.ToInt32((double)((startDateTime - endDateTime).TotalSeconds / (4 * 60 * 60)));

            timeMergreLimit = groupBy;
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

                return measuredDataFinal.Where(option => option.DateTimeValue >= startDateTime && option.DateTimeValue <= endDateTime).ToList();
            }
            else
            {
                return LoadedSummaryHelperData.Where(option => option.DateTimeValue >= startDateTime && option.DateTimeValue <= endDateTime).ToList();
            }
        }

        private List<(string Name, int Steps, DateTime Od, DateTime Do)> LoadActivityData (DateTime startDateTime, DateTime endDateTime)
        {
            List<ActivityData> activityData = _applicationDbContext.ActivityData.Where(option => option.UserId == UserId).Select(select => new ActivityData() { Id = select.Id, DateStart = select.DateStart, DateEnd = select.DateEnd, Steps = select.Steps, Kind = select.Kind }).ToList();
            List<(string Name, int Steps, DateTime Od, DateTime Do)> Data = activityData.Select(select => (Name: ActivityConstant.GetConstantNameById(select.Kind), Steps: select.Steps, Od: select.DateStart, Do: select.DateEnd)).ToList();
            // activityData.ForEach(select => select.Steps = ActivityDataService.CountStepsForActivity(_applicationDbContext, select.Id, select.Steps));
            return Data.Take(3).ToList();
        }

        private List<SummaryHeartRate> LoadSummaryHeartRate (DateTime startDateTime, DateTime endDateTime)
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
