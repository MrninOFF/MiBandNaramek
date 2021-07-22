using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MiBandNaramek.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MiBandNaramekUser class
    public class MiBandNaramekUser : IdentityUser
    {
        // V�ha
        [PersonalData]
        [DisplayName("V�ha")]
        [DataType(DataType.Currency, ErrorMessage = "Uve�te ��slo.")]
        [Range(0, 1000, ErrorMessage = "Uve�te v�hu v kg.")]
        public double Wight { get; set; }
        // V��ka
        [PersonalData]
        [DisplayName("V��ka")]
        [DataType(DataType.Currency, ErrorMessage = "Uve�te ��slo.")]
        [Range(5, 300, ErrorMessage = "Uve�te v��ku v cm.")]
        public double Height { get; set; }

    }
}
