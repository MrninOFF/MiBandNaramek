using MiBandNaramek.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiBandNaramek.Models.ViewModels
{
    public class UserUpdateViewModel
    {
        public MiBandNaramekUser User { get; set; }
        public List<IdentityRole> UserRoles { get; set; }
        public String UserSelectedRole { get; set; }
    }
}
