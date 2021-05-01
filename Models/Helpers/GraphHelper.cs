﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.Helpers
{
    public class SummaryHelper
    {
        public DateTime DateTimeValue { get; set; }
        public double DoubleValue { get; set; }
    }

    public class SummaryViewData
    {
        public ChartJSCore.Models.Chart Chart { get; set; }
    }

}