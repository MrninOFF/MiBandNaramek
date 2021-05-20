using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.Helpers
{
    public class SummaryHelper
    {
        public DateTime DateTimeValue { get; set; }
        public double DoubleValue { get; set; }
        public int Steps { get; set; }
        public double Intensity { get; set; }
    }

    public class SummaryViewData
    {
        public List<DailyChartData> Charts { get; set; }
        public string Od { get; set; }
        public string Do { get; set; }
        public List<ActivityData> Activity { get; set; }
        public List<SummaryHeartRate> SummaryHeartRate { get; set; }
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
        public ChartJSCore.Models.Chart Chart { get; set; }
    }

}
