using MiBandNaramek.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    // Nová třída ActivityData - Tomáš Mrňák 20.4.2021

    public class ActivityData
    {
        [JsonIgnore]
        public int Id { get; set; }
        // ID uživatele z AspNetUser - Spravuje Identity framework
        [JsonIgnore]
        public string UserId { get; set; }

        // Atributy //
        [JsonPropertyName("timestamp_start")]
        public long TimestampStart { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime DateStart { get; set; }

        [JsonPropertyName("timestamp_end")]
        public long TimestampEnd { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime DateEnd { get; set; }

        [JsonPropertyName("activity_kind")]
        public int Kind { get; set; }

        [JsonPropertyName("latitude")]
        public decimal Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public decimal Longitude { get; set; }

        [JsonPropertyName("altitude")]
        public decimal Altitude { get; set; }

        [JsonPropertyName("gpx_track")]
        public string GpxTrack { get; set; }

        [JsonPropertyName("summary_data")]
        public string Summary { get; set; }

        // Kdy byly data nahrána na server
        [JsonIgnore]
        public long UploadDate { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public MiBandNaramekUser User { get; set; }
    }
}
