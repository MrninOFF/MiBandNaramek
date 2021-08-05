using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.Helpers
{
    public class UserViewData
    {
        public MiBandNaramekUser User { get; set; }
        public BatteryData BatteryData { get; set; }
        public string LastSync { get; set; }
    }

    public class UserMeasuredDataToJSON
    {
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }
        [JsonPropertyName("user_height")]
        public double UserHeight { get; set; }
        [JsonPropertyName("user_weight")]
        public double UserWeight { get; set; }
        [JsonPropertyName("measured_data")]
        public List<SummaryHelper> SummaryData { get; set; }
    }

}
