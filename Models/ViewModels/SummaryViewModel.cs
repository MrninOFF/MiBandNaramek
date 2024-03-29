﻿using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.Helpers
{
    public class SummaryHelper
    {
        [JsonPropertyName("date_time")]
        public DateTime DateTimeValue { get; set; }
        [JsonPropertyName("heart_rate")]
        public double DoubleValue { get; set; }
        [JsonPropertyName("steps")]
        public int Steps { get; set; }
        [JsonPropertyName("intensity")]
        public double Intensity { get; set; }
        [JsonPropertyName("kind")]
        public int Kind { get; set; }
    }

    public class SummaryViewModel
    {
        public ChartJSCore.Models.Chart SummaryChart { get; set; }
        public List<DailyChartData> DailyCharts { get; set; }
        public string Od { get; set; }
        public string Do { get; set; } 
        public string UserId { get; set; }
        public int GroupByMin { get; set; }
        public MiBandNaramekUser User { get; set; }
    }

    public class SummaryHeartRate
    {
        public string Description { get; set; }
        public int Count { get; set; }
    }

    public class DailyChartData
    {
        public string Name { get; set; }
        public string VariableName { get; set; }
        public string VariablePieName { get; set; }
        public ChartJSCore.Models.Chart Chart { get; set; }
        public ChartJSCore.Models.Chart PieChart { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public List<(string Name, int Steps, DateTime Od, DateTime Do)> Activity { get; set; }
    }

}
