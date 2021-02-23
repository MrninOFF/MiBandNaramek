using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    // Nová třída HeartRate - Tomáš Mrňák 15.2.2021
    public class HeartRate
    {
        public int HeartRateId { get; set; }
        // ID uživatele z AspNetUser - Spravuje Identity framework
        public string OwnerID { get; set; }

        // Atributy //
        
        // Tepová frekvence
        public double HeartRateData { get; set; }

        // Datum ve formátu YYYY-MM-DD
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        // "Číslo" minuty ve dne od 0 do 1439
        public string Minute { get; set; }

        // Kdy byly data nahrána na server
        public long Timestamp { get; set; }

        // Zdroj dat AUTO nebo MANUAL
        public string MeasureType { get; set; }



    }
}
