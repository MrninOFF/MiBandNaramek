using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.API.Requests
{
    public class MeasuredDataRequest
    {
        // Stračka atribut //
        [JsonPropertyName("verification_data")]
        public DeviceDataFormat DeviceData { get; set; }

        [JsonPropertyName("measured_data")]
        public List<MeasuredData> MeasuredData { get; set; }

        [JsonPropertyName("battery_data")]
        public List<BatteryData> BatteryData { get; set; }

        [JsonPropertyName("activity_data")]
        public List<ActivityData> ActivityData { get; set; }
    }

    public class DeviceDataFormat 
    {
        [Required]
        [JsonPropertyName("wristband_mac")]
        public string MiBandMacAddress { get; set; }

        [Required]
        [JsonPropertyName("device_id")]
        public string MobileDeviceId { get; set; }
    }
}
