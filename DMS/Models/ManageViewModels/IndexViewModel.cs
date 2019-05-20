using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display( Name = "Phone number" )]
        public string PhoneNumber { get; set; }

        [Display( Name = "First Name" )]
        public string FirstName { get; set; }

        [Display( Name = "Last Name" )]
        public string LastName { get; set; }

        [Display( Name = "Address 1" )]
        public string Address1 { get; set; }

        [Display( Name = "Address 2" )]
        public string Address2 { get; set; }

        [Display( Name = "City" )]
        public string City { get; set; }

        [Display( Name = "State" )]
        public string State { get; set; }

        [Display( Name = "Zip Code 1" )]
        public int Zip1 { get; set; }

        [Display( Name = "Zip Code 2" )]
        public int Zip2 { get; set; }

        public string StatusMessage { get; set; }
        public string DegreeAbbreviation { get; set; }
        public string DoctorUserName { get; set; }
        [Display( Name = "Doctor" )]
        public string DoctorFullName { get; set; }
        public List<Doctor> AllDoctors { get; set; }
        public string Title { get; internal set; }
    }
}
