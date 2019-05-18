using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ViewModels
{
    public class DoctorViewModel
    {
        public List<Patient> Patients { get; set; }
        [DisplayName( "First Name" )]
        public string FirstName { get; set; }
        [DisplayName( "Last Name" )]
        public string LastName { get; set; }
        [DisplayName( "Address 1" )]
        public string Address1 { get; set; }
        [DisplayName( "Address 2" )]
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [DisplayName( "Zip Code" )]
        public int Zip1 { get; set; }
        [DisplayName( "Zip 2" )]
        public int Zip2 { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }    // Provide for lookup
        [DisplayName( "Degree Abbreviation" )]
        public string DegreeAbbreviation { get; set; }

        public DoctorViewModel()
        {
            Patients = new List<Patient>();

        } // Constructor


        public Doctor GetNewDoctor()
        {
            return new Doctor
            {
                Email = Email,
                UserName = UserName,
                FirstName = FirstName,
                LastName = LastName,
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                State = State,
                Zip1 = Zip1,
                Zip2 = Zip2,
                Height = Height,
                Weight = Weight,
                DegreeAbbreviation = DegreeAbbreviation,
                Patients = Patients
            };

        } // GetNewDoctor

    } // class

} // namespace
