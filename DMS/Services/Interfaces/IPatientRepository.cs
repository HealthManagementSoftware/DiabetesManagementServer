using DMS.Models;
using DMS.Models.ViewModels
;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IPatientRepository
    {
        Task<Patient> ReadAsync( string username );
        IQueryable<Patient> ReadAll();
        Task<Patient> CreateAsync(Patient patient);
        Task UpdateAsync( string username, Patient project );
        Task DeleteAsync( string username );
        ApplicationUser ReadPatient(string email);

        bool Exists(string firstName/*, string lastName*/);


    } // Interface

} // Namespace