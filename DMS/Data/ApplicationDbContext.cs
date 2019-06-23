using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DMS.Models;


namespace DMS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
        //IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>,
        //IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public DbSet<ExerciseEntry> ExerciseEntries { get; set; }
        public DbSet<GlucoseEntry> GlucoseEntries { get; set; }
        public DbSet<MealEntry> MealEntries { get; set; }
        public DbSet<MealItem> MealItems { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<HIPAAPrivacyNotice> HIPAAPrivacyNotices { get; set; }
        public DbSet<PatientSignedHIPAANotice> PatientSignedHIPAANotices { get; set; }
        //public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<GlucoseEntry>().Property( x => x.BeforeAfter )
            //    .HasConversion<int>()
            //    .HasDefaultValue( BeforeAfter.BEFORE );

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<PatientSignedHIPAANotice>()
            //    .HasKey(k => new { k.PatientUserName, k.NoticeId });

            //builder.Entity<ApplicationUser>()
            //    .HasMany( o => o.Roles )
            //    .WithOne( o => o.User )
            //    .HasForeignKey( o => o.UserId );

            //builder.Entity<ApplicationRole>()
            //    .HasMany( o => o.Users )
            //    .WithOne( o => o.Role )
            //    .HasForeignKey( o => o.RoleId );

            builder.Entity<Patient>()
                .HasMany( o => o.GlucoseEntries )
                .WithOne( o => o.Patient )
                .HasForeignKey( o => o.UserName );

            builder.Entity<Patient>()
                .HasMany( o => o.MealEntries )
                .WithOne( o => o.Patient )
                .HasForeignKey( o => o.UserName );

            builder.Entity<Patient>()
                .HasMany( o => o.ExerciseEntries )
                .WithOne( o => o.Patient )
                .HasForeignKey( o => o.UserName );

            builder.Entity<Patient>()
                .HasOne( o => o.Doctor )
                .WithMany( o => o.Patients );

            builder.Entity<Patient>()
                .HasOne( o => o.PatientSignedHIPAANotice )
                .WithOne( o => o.Patient )
                .HasForeignKey<Patient>( o => o.PatientSignedHIPAANoticeId );

            builder.Entity<Doctor>()
                .HasMany( o => o.Patients )
                .WithOne( o => o.Doctor );

            builder.Entity<PatientSignedHIPAANotice>()
                .HasOne(o => o.Patient)
                .WithOne(o => o.PatientSignedHIPAANotice)
                .HasForeignKey<PatientSignedHIPAANotice>(o => o.PatientId);

            builder.Entity<PatientSignedHIPAANotice>()
                .HasOne( o => o.HIPAAPrivacyNotice )
                .WithMany( o => o.Signatures )
                .HasForeignKey( o => o.HIPAAPrivacyNoticeId );

            builder.Entity<HIPAAPrivacyNotice>()
                .HasMany(o => o.Signatures)
                .WithOne(o => o.HIPAAPrivacyNotice);

            builder.Entity<MealEntry>()
                .HasMany( o => o.MealItems )
                .WithOne( o => o.Meal );

        }

    } // class
    
} // namespace
