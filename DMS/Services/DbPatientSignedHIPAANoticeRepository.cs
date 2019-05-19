using DMS.Data;
using DMS.Models;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbPatientSignedHIPAANoticeRepository : IPatientSignedHIPAANoticeRepository
    {
        private readonly ApplicationDbContext _db;

        public DbPatientSignedHIPAANoticeRepository( ApplicationDbContext db )
        {
            _db = db;

        } // Injection Constructor


        public async Task<PatientSignedHIPAANotice> ReadAsync( string PatientUserName, Guid noticeid )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.PatientId == PatientUserName && o.HIPAAPrivacyNoticeId == noticeid );

        } // ReadAsync


        public IQueryable<PatientSignedHIPAANotice> ReadAll()
        {
            return _db.PatientSignedHIPAANotices;

        } // ReadAll


        public async Task<PatientSignedHIPAANotice> CreateAsync( PatientSignedHIPAANotice patientsignedhipaanotice )
        {
            _db.PatientSignedHIPAANotices.Add( patientsignedhipaanotice );
            await _db.SaveChangesAsync();
            return patientsignedhipaanotice;

        } // Create


        public async Task UpdateAsync( string patientUserName, Guid noticeid, PatientSignedHIPAANotice hipaaNotice )
        {
            var oldPatientSignedHIPAANotice = await ReadAsync( patientUserName, noticeid );
            if ( oldPatientSignedHIPAANotice != null )
            {
                oldPatientSignedHIPAANotice.PatientId = hipaaNotice.PatientId;
                oldPatientSignedHIPAANotice.Patient = hipaaNotice.Patient;
                oldPatientSignedHIPAANotice.HIPAAPrivacyNoticeId = hipaaNotice.HIPAAPrivacyNotice.Id;
                oldPatientSignedHIPAANotice.HIPAAPrivacyNotice = hipaaNotice.HIPAAPrivacyNotice;
                oldPatientSignedHIPAANotice.Signed = hipaaNotice.Signed;
                oldPatientSignedHIPAANotice.SignedAt = hipaaNotice.SignedAt;
                oldPatientSignedHIPAANotice.UpdatedAt = hipaaNotice.UpdatedAt;

                _db.Entry( oldPatientSignedHIPAANotice.Patient ).State = EntityState.Unchanged;
                _db.Entry( oldPatientSignedHIPAANotice.HIPAAPrivacyNotice ).State = EntityState.Unchanged;
                _db.Entry( oldPatientSignedHIPAANotice ).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( string patientUserName, Guid noticeid )
        {
            var patientsignedhipaanotice = await ReadAsync(patientUserName, noticeid);
            if ( patientsignedhipaanotice != null )
            {
                _db.PatientSignedHIPAANotices.Remove( patientsignedhipaanotice );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public async Task CreateOrUpdateEntry( PatientSignedHIPAANotice patientSignedHIPAANotice )
        {
            if ( patientSignedHIPAANotice == null )                  // If meal entry doesn't exist
            {
                // Create in the database
                await CreateAsync( patientSignedHIPAANotice );

            }
            else if ( patientSignedHIPAANotice.UpdatedAt < patientSignedHIPAANotice.UpdatedAt )
            {
                // Update in the database
                await UpdateAsync( patientSignedHIPAANotice.PatientId, 
                    patientSignedHIPAANotice.HIPAAPrivacyNoticeId, patientSignedHIPAANotice );

            }

            return;

        } // CreateOrUpdateEntries

    } // Class

} // Namespace