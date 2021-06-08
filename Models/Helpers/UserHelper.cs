using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.Helpers
{
    public class UserViewData
    {
        public MiBandNaramekUser User { get; set; }
        public BatteryData BatteryData { get; set; }
        public string LastSync { get; set; }
    }
}
