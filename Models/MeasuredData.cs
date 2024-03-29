﻿using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    // Nová třída HeartRate - Tomáš Mrňák 15.2.2021
    public class MeasuredData
    {
        [JsonIgnore]
        public int Id { get; set; }
        // ID uživatele z AspNetUser - Spravuje Identity framework
        [JsonIgnore]
        public string UserId { get; set; }

        // Atributy //

        // Čas UNIX Timestamp in Seconds !!!
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        // Tepová frekvence
        [JsonPropertyName("intensity")]
        public int Intensity { get; set; }

        // Tepová frekvence
        [JsonPropertyName("heart_rate")]
        public int HeartRate { get; set; }

        // Tepová frekvence
        [JsonPropertyName("kind")]
        public int Kind { get; set; }
        
        // Tepová frekvence
        [JsonPropertyName("steps")]
        public int Steps { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Kdy byly data nahrána na server
        [JsonIgnore]
        public long UploadDate { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public MiBandNaramekUser User { get; set; }

    }
}
