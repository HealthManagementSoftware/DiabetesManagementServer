using DMS.Data;
using DMS.Models;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/*********************************************/
//  Created by J.T. Blevins
//  Modified by Heather Harvey with advice from Natash Ince and J.T. Blevins
/*********************************************/
namespace DMS.Services
{
    public class DbExerciseEntryRepository : IExerciseEntryRepository
    {
        private readonly ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbExerciseEntryRepository( ApplicationDbContext db,
                                    IAuditRepository auditRepo )
        {
            _db = db;
            _auditRepo = auditRepo;

        } // Injection Constructor


        public async Task<ExerciseEntry> ReadAsync( Guid id )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.Id == id );

        } // ReadAsync


        public IQueryable<ExerciseEntry> ReadAll()
        {
            return _db.ExerciseEntries;

        } // ReadAll


        public async Task<ExerciseEntry> CreateAsync( ExerciseEntry exerciseentry )
        {
            _db.ExerciseEntries.Add( exerciseentry );
            await _db.SaveChangesAsync();

            if( Config.AuditingOn )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.CREATE, exerciseentry.Id.ToString(), new ExerciseEntry(), exerciseentry );
                await _auditRepo.CreateAsync( auditChange );

            } // if

            return exerciseentry;

        } // Create


        public async Task UpdateAsync( Guid id, ExerciseEntry exerciseEntry )
        {
            var oldExerciseEntry = await ReadAsync( id );
            if( oldExerciseEntry != null )
            {
                if( Config.AuditingOn )
                {
                    var auditChange = new AuditChange();
                    if( !auditChange.CreateAuditTrail( AuditActionType.UPDATE, exerciseEntry.Id.ToString(), oldExerciseEntry, exerciseEntry ) )
                        await _auditRepo.CreateAsync( auditChange );

                } // if

                oldExerciseEntry.UserName = exerciseEntry.UserName;
    			oldExerciseEntry.Patient = exerciseEntry.Patient;
    			oldExerciseEntry.Name = exerciseEntry.Name;
    			oldExerciseEntry.Minutes = exerciseEntry.Minutes;
    			oldExerciseEntry.CreatedAt = exerciseEntry.CreatedAt;
                oldExerciseEntry.UpdatedAt = exerciseEntry.UpdatedAt;
                oldExerciseEntry.Timestamp = exerciseEntry.Timestamp;
                _db.Entry( oldExerciseEntry ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( Guid id )
        {
            var exerciseentry = await ReadAsync( id );
            if( exerciseentry != null )
            {
                if( Config.AuditingOn )
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail( AuditActionType.DELETE, id.ToString(), exerciseentry, new ExerciseEntry() );
                    await _auditRepo.CreateAsync( auditChange );

                } // if

                _db.ExerciseEntries.Remove( exerciseentry );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync

        public ExerciseEntry Create(ExerciseEntry exerciseEntry)
        {
            _db.ExerciseEntries.Add(exerciseEntry);
            _db.SaveChanges();

            if( Config.AuditingOn )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.CREATE, exerciseEntry.Id.ToString(), new ExerciseEntry(), exerciseEntry );
                _auditRepo.CreateAsync( auditChange );

            } // if

            return exerciseEntry;
        }// ExerciseEntry Create

        public ExerciseEntry Read(Guid exerciseEntryId)
        {
            return _db.ExerciseEntries.FirstOrDefault(e => e.Id == exerciseEntryId);
        }// ExerciseEntry Read
        

        public async Task CreateOrUpdateEntries( ICollection<ExerciseEntry> exerciseEntries )
        {
            foreach ( ExerciseEntry exerciseEntry in exerciseEntries )
            {
                ExerciseEntry dbExerciseEntry = await ReadAsync( exerciseEntry.Id );
                if ( dbExerciseEntry == null )                                  // If meal entry doesn't exist
                {
                    // Create in the database
                    await CreateAsync( exerciseEntry );

                }
                else if ( dbExerciseEntry.UpdatedAt < exerciseEntry.UpdatedAt ) // Otherwise...
                {
                    // Update in the database
                    await UpdateAsync( exerciseEntry.Id, exerciseEntry );

                }

            } // foreach MealEntry

            return;

        } // CreateOrUpdateEntries

        public bool Exists( Guid id )
        {
            return _db.ExerciseEntries.Any( o => o.Id == id );
        }

    } // Class

} // Namespace