using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbDoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbDoctorRepository( ApplicationDbContext db,
                                    IAuditRepository auditRepo )
        {
            _db = db;
            _auditRepo = auditRepo;

        } // Injection Constructor


        public IQueryable<Doctor> ReadAll()
        {
            return _db.Doctors;

        } // ReadAll


        public async Task<Doctor> ReadAsync( string username )
        {
            return await ReadAll()
                .Include( p => p.Patients )
                .SingleOrDefaultAsync( o => o.UserName == username );

        } // ReadAsync


        public async Task<Doctor> CreateAsync( Doctor doctor )
        {
            _db.Doctors.Add( doctor );
            await _db.SaveChangesAsync();

            var auditChange = new AuditChange();
            auditChange.CreateAuditTrail( AuditActionType.CREATE, doctor.Id, new Doctor(), doctor );
            await _auditRepo.CreateAsync( auditChange );

            return doctor;

        } // Create


        public async Task UpdateAsync( string userName, Doctor doctor )
        {
            var oldDoctor = await ReadAsync( userName );
            if( oldDoctor != null )
            {
                var auditChange = new AuditChange();
                if( !auditChange.CreateAuditTrail( AuditActionType.UPDATE, doctor.Id, oldDoctor, doctor ) )
                    await _auditRepo.CreateAsync( auditChange );

                oldDoctor.DegreeAbbreviation = doctor.DegreeAbbreviation;

                if( doctor.Patients != null && doctor.Patients.Count > 0 )
                    oldDoctor.Patients = doctor.Patients;

                _db.Entry( oldDoctor ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( string usernameid )
        {
            var doctor = await ReadAsync( usernameid );
            if( doctor != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.DELETE, doctor.Id, doctor, new Doctor() );
                await _auditRepo.CreateAsync( auditChange );

                _db.Doctors.Remove( doctor );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync

        public ApplicationUser ReadDoctor( string email )
        {
            return _db.Users.FirstOrDefault( u => u.Email == email );
        }

        public bool Exists( string firstName )
        {
            return _db.Doctors.Any( fn => fn.FirstName == firstName );
        }

    } // Class

} // Namespace