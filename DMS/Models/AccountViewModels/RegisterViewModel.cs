using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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

        public bool DevCreationAllowed { get { return Config.AllowDevCreation; } }

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
            // Changed the way the user is registered. Using the UserManager to add patients/doctors
            // caused errors with CosmosDB, so have to create everything from scratch.
            var doctor = await doctorRepository.ReadAsync( DoctorUserName );
            var pat = new Patient
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                //DoctorId = doctor?.Id,
                Doctor = doctor,
                //DrUserName = DoctorUserName,
                UserName = Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString(),
                //HasSignedHIPAANotice = false
            };
            var passHash = new PasswordHasher<ApplicationUser>();
            var hash = passHash.HashPassword(pat, Password);
            pat.PasswordHash = hash;
            return pat;

        } // GetNewPatient


        public Doctor GetNewDoctor()
        {
            var doc = new Doctor
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                UserName = Email,
                DegreeAbbreviation = DegreeAbbreviation,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var passHash = new PasswordHasher<ApplicationUser>();
            var hash = passHash.HashPassword(doc, Password);
            doc.PasswordHash = hash;
            return doc;

        } // GetNewDoctor


        public Developer GetNewDeveloper()
        {
            var dev = new Developer
            {
                //Id = new Guid().ToString(),
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                UserName = Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var passHash = new PasswordHasher<ApplicationUser>();
            var hash = passHash.HashPassword(dev, Password);
            dev.PasswordHash = hash;
            return dev;

        } // GetNewDoctor

        public RegisterViewModel()
        {
            //Role = new ApplicationRole();
        }

    }

}
