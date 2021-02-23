using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiBandNaramek.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MiBandNaramek.Areas.Identity.Data
{
    public class MiBandNaramekIdentityDbContext : IdentityDbContext<MiBandNaramekUser>
    {
        public MiBandNaramekIdentityDbContext(DbContextOptions<MiBandNaramekIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
