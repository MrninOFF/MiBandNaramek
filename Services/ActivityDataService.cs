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
        public static int CountStepsForActivity(ApplicationDbContext applicationDbContext, int ActivityId, int steps)
        {
            if (steps > 0)
                return steps;

            ActivityData activityData = applicationDbContext.ActivityData.Where(where => where.Id == ActivityId).FirstOrDefault();
            if(activityData != null)
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
