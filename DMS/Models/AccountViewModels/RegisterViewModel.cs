using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        public List<ApplicationRole> AllRoles { get; set; }

        //public string SelectedUser { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        // API
        [Display( Name = "Doctor" )]
        public string DoctorUserName { get; set; }
        // Register:
        [Display( Name = "Degree Abbreviation" )]
        public string DegreeAbbreviation { get; set; }
        [Display( Name = "All Doctors" )]
        public List<Doctor> AllDoctors { get; set; }

        public async Task<Patient> GetNewPatient( IDoctorRepository doctorRepository)
        {
            var doctor = await doctorRepository.ReadAsync( DoctorUserName );
            return new Patient
            {
                //Id = new Guid().ToString(),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                //DoctorId = doctor?.Id,
                Doctor = doctor,
                //DrUserName = DoctorUserName,
                UserName = Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                //SecurityStamp = new Guid().ToString(),
                //HasSignedHIPAANotice = false
            };

        } // GetNewPatient

        public Doctor GetNewDoctor()
        {
            return new Doctor
            {
                //Id = new Guid().ToString(),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                UserName = Email,
                DegreeAbbreviation = DegreeAbbreviation,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now//,
                //SecurityStamp = new Guid().ToString()
            };

        } // GetNewPatient

        public RegisterViewModel()
        {
            //Role = new ApplicationRole();
        }

    }

}
