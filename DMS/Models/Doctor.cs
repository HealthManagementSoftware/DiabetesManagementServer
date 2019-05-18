using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Doctor : ApplicationUser
    {
        [DisplayName( "Degree Abbreviation" )]
        public string DegreeAbbreviation { get; set; }
        public List<Patient> Patients { get; set; }

        public Doctor()
        {
            Patients = new List<Patient>();

        } // constructor


        public static Doctor Clone( Doctor oldDoctor )
        {
            return new Doctor()
            {
                DegreeAbbreviation = oldDoctor.DegreeAbbreviation,
                FirstName = oldDoctor.FirstName,
                LastName = oldDoctor.LastName,
                Address1 = oldDoctor.Address1,
                Address2 = oldDoctor.Address2,
                City = oldDoctor.City,
                State = oldDoctor.State,
                Zip1 = oldDoctor.Zip1,
                Zip2 = oldDoctor.Zip2,
                Email = oldDoctor.Email,
                PhoneNumber = oldDoctor.PhoneNumber,
                AccessFailedCount = oldDoctor.AccessFailedCount,
                ConcurrencyStamp = oldDoctor.ConcurrencyStamp,
                CreatedAt = oldDoctor.CreatedAt,
                UpdatedAt = oldDoctor.UpdatedAt,
                EmailConfirmed = oldDoctor.EmailConfirmed,
                Height = oldDoctor.Height,
                Weight = oldDoctor.Weight,
                Id = oldDoctor.Id,
                NormalizedEmail = oldDoctor.NormalizedEmail,
                NormalizedUserName = oldDoctor.NormalizedUserName,
                LockoutEnabled = oldDoctor.LockoutEnabled,
                LockoutEnd = oldDoctor.LockoutEnd,
                PhoneNumberConfirmed = oldDoctor.PhoneNumberConfirmed,
                PasswordHash = oldDoctor.PasswordHash,
                RemoteLoginExpiration = oldDoctor.RemoteLoginExpiration,
                RemoteLoginToken = oldDoctor.RemoteLoginToken,
                SecurityStamp = oldDoctor.SecurityStamp,
                TwoFactorEnabled = oldDoctor.TwoFactorEnabled,
                UserName = oldDoctor.UserName,
                Patients = oldDoctor.Patients
            };

        } // Clone

    } // class

} // namespace
