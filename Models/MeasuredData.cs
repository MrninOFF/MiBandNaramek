using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    // Nová třída HeartRate - Tomáš Mrňák 15.2.2021
    public class MeasuredData
    {
        public int Id { get; set; }
        // ID uživatele z AspNetUser - Spravuje Identity framework
        public string UserId { get; set; }

        // Atributy //

        // Tepová frekvence
        public int Intensity { get; set; }

        // Tepová frekvence
        public int HeartRate { get; set; }

        // Tepová frekvence
        public int Kind { get; set; }

        // Tepová frekvence
        public int Steps { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // Kdy byly data nahrána na server
        public long UploadDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public MiBandNaramekUser User { get; set; }

    }
}
