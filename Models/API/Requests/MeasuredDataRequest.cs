using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.API.Requests
{
    public class MeasuredDataRequest
    {
        [Required]
        public List<MeasuredDataFormat> MeasuredData { get; set; }
    }

    public class MeasuredDataFormat
    {
        [Required]
        public long Timestamp { get; set; }

        [Required]
        public int Kind { get; set; }

        [Required]
        public int Intensity { get; set; }

        [Required]
        public int Steps { get; set; }

        [Required]
        public int HeartRate { get; set; }
    }
}
