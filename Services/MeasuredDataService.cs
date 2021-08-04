using MiBandNaramek.Data;
using MiBandNaramek.Models.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Services
{
    public static class MeasuredDataService
    {
        public static List<SummaryHelper> LoadSummaryHelperData(ApplicationDbContext dbContext, string UserId, DateTime Od, DateTime Do)
        {
            var data = dbContext.MeasuredData
                            .Where(option => option.Date >= Od && option.Date <= Do && option.UserId == UserId)
                            .Select(select => new SummaryHelper { DoubleValue = Convert.ToDouble(select.HeartRate), DateTimeValue = select.Date, Steps = select.Steps, Intensity = Convert.ToDouble(select.Intensity), Kind = select.Kind })
                            .OrderBy(orderBy => orderBy.DateTimeValue)
                            .ToList();
            return data ?? new List<SummaryHelper>();
        }
    }
}
