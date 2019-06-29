using DMS.Models;
using DMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IDoctorRepository
    {
        Task<Doctor> ReadAsync( string username );
        IQueryable<Doctor> ReadAll();
        Task<Doctor> CreateAsync( Doctor doctor );
        Task UpdateAsync( string username, Doctor doctor );
        Task DeleteAsync( string username );
        bool Exists( string id );


    } // Interface

} // Namespace