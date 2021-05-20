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
    }
}
