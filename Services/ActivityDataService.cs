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

            switch (AktivityKind)
            {
                case 0x01:
                case 0x10:
                case 0x11:
                case 0x21:
                case 0x31:
                case 0x41:
                case 0x51:
                    // WALK
                    return ChartColor.FromRgb(50, 50, 200);
                case 0x06:
                case 0x53:
                case 0x63:
                case 0x73:
                    // Charging, Nenošen
                    return ChartColor.FromRgb(200, 50, 50);
                case 0x12:
                case 0x22:
                case 0x32:
                case 0x42:
                case 0x52:
                case 0x62:
                case 0x5a:
                    // RUN
                    return ChartColor.FromRgb(50, 200, 50);
                case 0x19:
                case 0x6c:
                case 0x1c:
                case 0x7c:
                case 0x79:
                case 0x70:
                case 0x1a:
                case 240:
                    // POSTEL
                    return ChartColor.FromRgb(200, 200, 50);
                case 0x13:
                    return ChartColor.CreateRandomChartColor(false);
                case 0x5c:
                case 0x59:
                case 0x50:
                    // SIT 
                    return ChartColor.FromRgb(200, 50, 200);
                case 0x60:
                case 0x6a:
                    // STAND
                    return ChartColor.FromRgb(50, 200, 200);
                default:
                    return ChartColor.CreateRandomChartColor(false);
            }
        }
    }
}
