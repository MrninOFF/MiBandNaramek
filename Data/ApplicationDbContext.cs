using MiBandNaramek.Areas.Identity.Data;
using MiBandNaramek.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiBandNaramek.Data
{
    public class ApplicationDbContext : IdentityDbContext<MiBandNaramekUser>
    {

        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<MeasuredData> MeasuredData { get; set; }
        public virtual DbSet<BatteryData> BatteryData { get; set; }
        public virtual DbSet<ActivityData> ActivityData { get; set; }
        public virtual DbSet<RequestData> RequestData { get; set; }

        public virtual DbSet<SummaryNote> SummaryNote { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Nastavení přesnosti pro datové typy spojené s GPS
            builder.Entity<ActivityData>().Property(e => e.Latitude).HasPrecision(38, 18);
            builder.Entity<ActivityData>().Property(e => e.Longitude).HasPrecision(38, 18);
            builder.Entity<ActivityData>().Property(e => e.Altitude).HasPrecision(38, 18);

            // Vytvoření Unique Klíčů z atributů
            builder.Entity<MeasuredData>()
                    .HasIndex(e => new { e.UserId, e.Timestamp }).IsUnique();

            builder.Entity<BatteryData>()
                    .HasIndex(e => new { e.UserId, e.Timestamp }).IsUnique();

            builder.Entity<ActivityData>()
                    .HasIndex(e => new { e.UserId, e.TimestampStart }).IsUnique();

            builder.Entity<SummaryNote>()
                    .HasIndex(e => new { e.UserId, e.Date }).IsUnique();

            // User data
            builder.Entity<MiBandNaramekUser>().Property(e => e.Height).HasPrecision(38, 18);
            builder.Entity<MiBandNaramekUser>().Property(e => e.Wight).HasPrecision(38, 18);

        }

    }
}
