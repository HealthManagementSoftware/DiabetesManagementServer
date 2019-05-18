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
                .SingleOrDefaultAsync( o => o.PatientUserName == PatientUserName && o.NoticeId == noticeid );

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


        public async Task UpdateAsync( string patientUserName, Guid noticeid, SignHIPAANoticeViewModel hipaaNoticeVM )
        {
            var oldPatientSignedHIPAANotice = await ReadAsync( patientUserName, noticeid );
            if( oldPatientSignedHIPAANotice != null )
            {
    			oldPatientSignedHIPAANotice.PatientUserName = hipaaNoticeVM.PatientUserName;
    			oldPatientSignedHIPAANotice.Patient = hipaaNoticeVM.Patient;
                oldPatientSignedHIPAANotice.NoticeId = hipaaNoticeVM.HIPAAPrivacyNotice.Id;
    			oldPatientSignedHIPAANotice.HIPAAPrivacyNotice = hipaaNoticeVM.HIPAAPrivacyNotice;
    			oldPatientSignedHIPAANotice.Signed = hipaaNoticeVM.Signed;
    			oldPatientSignedHIPAANotice.SignedAt = hipaaNoticeVM.SignedAt;
    			oldPatientSignedHIPAANotice.UpdatedAt = hipaaNoticeVM.UpdatedAt;

                _db.Entry( oldPatientSignedHIPAANotice.Patient ).State = EntityState.Unchanged;
                _db.Entry( oldPatientSignedHIPAANotice.HIPAAPrivacyNotice ).State = EntityState.Unchanged;
                _db.Entry( oldPatientSignedHIPAANotice ).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync(string patientUserName, Guid noticeid )
        {
            var patientsignedhipaanotice = await ReadAsync(patientUserName, noticeid);
            if( patientsignedhipaanotice != null )
            {
                _db.PatientSignedHIPAANotices.Remove( patientsignedhipaanotice );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync

    } // Class

} // Namespace