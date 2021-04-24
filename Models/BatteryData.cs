using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    // Nová třída BatteryData - Tomáš Mrňák 20.4.2021
    public class BatteryData
    {
        [JsonIgnore]
        public int Id { get; set; }
        // ID uživatele z AspNetUser - Spravuje Identity framework
        [JsonIgnore]
        public string UserId { get; set; }

        // Atributy //

        // Úroveň baterie
        [JsonPropertyName("level")]
        public int Level { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [JsonIgnore]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Datum
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        // Kdy byly data nahrána na server
        [JsonIgnore]
        public long UploadDate { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public MiBandNaramekUser User { get; set; }
    }
}
