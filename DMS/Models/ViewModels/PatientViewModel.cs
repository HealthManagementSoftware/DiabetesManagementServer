using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ViewModels
{
    public class PatientViewModel
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<GlucoseEntry> GlucoseEntries { get; set; }
        public List<ExerciseEntry> ExerciseEntries { get; set; }
        public List<MealEntry> MealEntries { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip1 { get; set; }
        public int Zip2 { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        [Display(Name = "Doctor")]
        public string DoctorUserName { get; set; }
        public Doctor Doctor { get; set; }

        public PatientViewModel()
        {
                
        }


        public async Task<Patient> GetNewPatient( IDoctorRepository doctorRepository )
        {
            var doc = await doctorRepository.ReadAsync( DoctorUserName );
            return new Patient
            {
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                State = State,
                Zip1 = Zip1,
                Zip2 = Zip2,
                PhoneNumber = PhoneNumber,
                Email = Email,
                GlucoseEntries = GlucoseEntries,
                ExerciseEntries = ExerciseEntries,
                MealEntries = MealEntries,
                Doctor = doc
            };

        } // GetNewPatient

    } // class

} // namespace
