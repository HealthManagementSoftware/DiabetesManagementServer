using DMS.Models;
using DMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IPatientSignedHIPAANoticeRepository
    {
        Task<PatientSignedHIPAANotice> ReadAsync( string PatientUserName, Guid noticeid );
        IQueryable<PatientSignedHIPAANotice> ReadAll();
        Task<PatientSignedHIPAANotice> CreateAsync( PatientSignedHIPAANotice patientSignedHIPAANotice );
        Task UpdateAsync( string patientUserName, Guid noticeid, PatientSignedHIPAANotice patientSignedHIPAANotice );
        Task DeleteAsync(string patientUserName, Guid noticeid );
        Task CreateOrUpdateEntry( PatientSignedHIPAANotice patientSignedHIPAANotice );

    } // Interface

} // Namespace