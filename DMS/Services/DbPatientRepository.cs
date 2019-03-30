using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbPatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbPatientRepository( ApplicationDbContext db,
                                    IAuditRepository auditRepo )
        {
            _db = db;
            _auditRepo = auditRepo;

        } // Injection Constructor


        public async Task<Patient> ReadAsync( string username )
        {
            return await ReadAll()
                .Include( d => d.Doctor )
                .Include( g => g.GlucoseEntries )
                .Include( e => e.ExerciseEntries )
                .Include( m => m.MealEntries )
                .SingleOrDefaultAsync( o => o.UserName == username );

        } // ReadAsync


        public IQueryable<Patient> ReadAll()
        {
            return _db.Patients
                .Include( d => d.Doctor )
                .Include( g => g.GlucoseEntries )
                .Include( e => e.ExerciseEntries )
                .Include( m => m.MealEntries )
                    .ThenInclude( mi => mi.MealItems );

        } // ReadAll


        public async Task<Patient> CreateAsync( Patient patient )
        {
            _db.Patients.Add( patient );
            await _db.SaveChangesAsync();

            var auditChange = new AuditChange();
            auditChange.CreateAuditTrail( AuditActionType.CREATE, patient.Id, new Patient(), patient );
            await _auditRepo.CreateAsync( auditChange );

            return patient;

        } // Create


        public async Task UpdateAsync( string username, Patient patient )
        {
            var oldPatient = await ReadAsync( username );
            if( oldPatient != null )
            {
                var auditChange = new AuditChange();
                if( !auditChange.CreateAuditTrail( AuditActionType.UPDATE, patient.Id, oldPatient, patient ) )
                    await _auditRepo.CreateAsync( auditChange );

                oldPatient.FirstName = patient.FirstName;
                oldPatient.LastName = patient.LastName;
                oldPatient.Address1 = patient.Address1;
                oldPatient.Address2 = patient.Address2;
                oldPatient.City = patient.City;
                oldPatient.State = patient.State;
                oldPatient.Zip1 = patient.Zip1;
                oldPatient.Zip2 = patient.Zip2;
                oldPatient.PhoneNumber = patient.PhoneNumber;
                oldPatient.Email = patient.Email;
                oldPatient.CreatedAt = patient.CreatedAt;
                oldPatient.UpdatedAt = patient.UpdatedAt;
                oldPatient.RemoteLoginToken = patient.RemoteLoginToken; // In case it has changed
                oldPatient.Height = patient.Height;
                oldPatient.Weight = patient.Weight;
                if( !string.IsNullOrEmpty( patient.DoctorId ) && oldPatient.DoctorId != patient.DoctorId )
                    oldPatient.DoctorId = patient.DoctorId;
                if( !string.IsNullOrEmpty( patient.DoctorUserName ) && oldPatient.DoctorUserName != patient.DoctorUserName )
                    oldPatient.DoctorUserName = patient.DoctorUserName;
                if( patient.Doctor != null )
                    oldPatient.Doctor = oldPatient.Doctor;
                if ( oldPatient.Doctor != null && patient.Doctor != null
                    && oldPatient.Doctor.Id == patient.Doctor.Id )
                    _db.Entry( patient.Doctor ).State = EntityState.Unchanged;

                _db.Entry( oldPatient ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( string username )
        {
            var patient = await ReadAsync( username );
            if( patient != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.DELETE, patient.Id, patient, new Patient() );
                await _auditRepo.CreateAsync( auditChange );

                _db.Patients.Remove( patient );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public ApplicationUser ReadPatient( string email )
        {
            return _db.Users.FirstOrDefault( u => u.Email == email );
        }


        public bool Exists(string username)
        {
            return _db.Patients.Any( p => p.UserName == username);
        }

    } // Class

} // Namespace