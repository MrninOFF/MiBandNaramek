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
        // Váha
        [PersonalData]
        [DisplayName("Váha")]
        [DataType(DataType.Currency, ErrorMessage = "Uveïte èíslo.")]
        [Range(0, 1000, ErrorMessage = "Uveïte váhu v kg.")]
        public double Wight { get; set; }
        // Výška
        [PersonalData]
        [DisplayName("Výška")]
        [DataType(DataType.Currency, ErrorMessage = "Uveïte èíslo.")]
        [Range(5, 300, ErrorMessage = "Uveïte výšku v cm.")]
        public double Height { get; set; }

    }
}
