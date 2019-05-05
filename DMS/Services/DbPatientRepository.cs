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

        public DbPatientRepository( ApplicationDbContext db )
        {
            _db = db;

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


            return patient;

        } // Create


        public async Task UpdateAsync( string username, Patient patient )
        {
            var dbPatient = await ReadAsync( username );
            if( dbPatient != null )
            {

                dbPatient.FirstName = patient.FirstName;
                dbPatient.LastName = patient.LastName;
                dbPatient.Address1 = patient.Address1;
                dbPatient.Address2 = patient.Address2;
                dbPatient.City = patient.City;
                dbPatient.State = patient.State;
                dbPatient.Zip1 = patient.Zip1;
                dbPatient.Zip2 = patient.Zip2;
                dbPatient.PhoneNumber = patient.PhoneNumber;
                dbPatient.Email = patient.Email;
                dbPatient.CreatedAt = patient.CreatedAt;
                dbPatient.UpdatedAt = patient.UpdatedAt;
                dbPatient.RemoteLoginToken = patient.RemoteLoginToken; // In case it has changed
                dbPatient.Height = patient.Height;
                dbPatient.Weight = patient.Weight;

                _db.Entry( patient.Doctor ).State = EntityState.Unchanged;

                if( dbPatient?.Doctor?.UserName == patient?.Doctor?.UserName )
                {
                }
                else
                {
                    //if( !string.IsNullOrEmpty( patient.DoctorId ) && oldPatient.DoctorId != patient.DoctorId )
                    //    oldPatient.DoctorId = patient.DoctorId;
                    //if( !string.IsNullOrEmpty( patient.DrUserName ) && dbPatient.DrUserName != patient.DrUserName )
                    //    dbPatient.DrUserName = patient.DrUserName;
                    //if( patient.Doctor != null )
                        //dbPatient.Doctor = patient.Doctor;
                }

                _db.Entry( dbPatient ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( string username )
        {
            var patient = await ReadAsync( username );
            if( patient != null )
            {

                _db.Patients.Remove( patient );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public ApplicationUser ReadPatient( string email )
        {
            return _db.Users.FirstOrDefault( u => u.Email == email );
        }


        public bool Exists( string username )
        {
            return _db.Patients.Any( p => p.UserName == username );
        }

    } // Class

} // Namespace