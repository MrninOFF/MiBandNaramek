using ChartJSCore.Helpers;
using MiBandNaramek.Data;
using MiBandNaramek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Services
{
    public static class ActivityDataService
    {
        public static int CountStepsForActivity(ApplicationDbContext applicationDbContext, int ActivityId, int steps, string userId)
        {
            if (steps > 0)
                return steps;

            ActivityData activityData = applicationDbContext.ActivityData.Where(where => where.Id == ActivityId).FirstOrDefault();
            DateTime LastUserUpdate = applicationDbContext.MeasuredData.Where(where => where.UserId == userId).OrderBy(order => order.Date).Select(select => select.Date).FirstOrDefault();
            if(activityData != null && activityData.DateEnd <= LastUserUpdate)
            {
                activityData.Steps =
                applicationDbContext.MeasuredData.Where(where => where.UserId == activityData.UserId && (where.Timestamp >= activityData.TimestampStart && where.Timestamp <= activityData.TimestampEnd)).Sum(sum => sum.Steps);
                applicationDbContext.ActivityData.Update(activityData);
                applicationDbContext.SaveChanges();
                return activityData.Steps;
            }

            return 0;
        }

        public static ChartColor GetChartColorByActivityKind(int AktivityKind)
        {
            if (AktivityKind < 50)
                return ChartColor.FromRgb(
                    Convert.ToByte(AktivityKind - (AktivityKind / 1)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 10)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 20)));
            if (AktivityKind < 100)
                return ChartColor.FromRgb(
                    Convert.ToByte(AktivityKind - (AktivityKind / 10)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 1)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 20)));
            if (AktivityKind < 150)
                return ChartColor.FromRgb(
                    Convert.ToByte(AktivityKind - (AktivityKind / 20)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 1)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 10)));
            if (AktivityKind < 200)
                return ChartColor.FromRgb(
                    Convert.ToByte(AktivityKind - (AktivityKind / 1)),
                    Convert.ToByte(AktivityKind - (AktivityKind / 20)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 10)));
            else
                return ChartColor.FromRgb(
                    Convert.ToByte(AktivityKind - (AktivityKind / 20)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 10)), 
                    Convert.ToByte(AktivityKind - (AktivityKind / 1)));
        }
    }
}
