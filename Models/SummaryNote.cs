using MiBandNaramek.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models
{
    public class SummaryNote
    {

        public int Id { get; set; }

        public string Note { get; set; }

        [Column(TypeName="Date")]
        public DateTime Date { get; set; }

        // ID uživatele z AspNetUser - Spravuje Identity framework
        public string UserId { get; set; }

        public string DoctorId { get; set; }

        public DateTime UpdateDate { get; set; }

        [ForeignKey(nameof(UserId))]
        public MiBandNaramekUser User { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public MiBandNaramekUser Doctor { get; set; }
    }
}
