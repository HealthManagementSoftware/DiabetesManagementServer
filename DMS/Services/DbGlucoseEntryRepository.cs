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
    public class DbGlucoseEntriesRepository : IGlucoseEntryRepository
    {
        private readonly ApplicationDbContext _db;

        public DbGlucoseEntriesRepository( ApplicationDbContext db )
        {
            _db = db;

        } // Injection Constructor


        public async Task<GlucoseEntry> ReadAsync( Guid id )
        {
            return await ReadAll()
                .Include( p => p.Patient )
                .SingleOrDefaultAsync( o => o.Id == id );

        } // ReadAsync


        public IQueryable<GlucoseEntry> ReadAll()
        {
            return _db.GlucoseEntries;

        } // ReadAll


        public async Task<GlucoseEntry> CreateAsync( GlucoseEntry glucoseEntry )
        {
            _db.GlucoseEntries.Add( glucoseEntry );
            await _db.SaveChangesAsync();


            return glucoseEntry;

        } // Create


        public async Task UpdateAsync( Guid id, GlucoseEntry glucoseEntry )
        {
            var dbGlucoseEntry = await ReadAsync( id );
            if( dbGlucoseEntry != null )
            {

                dbGlucoseEntry.UserName = glucoseEntry.UserName;
                dbGlucoseEntry.Patient = glucoseEntry.Patient;
    			dbGlucoseEntry.Measurement = glucoseEntry.Measurement;
    			dbGlucoseEntry.BeforeAfter = glucoseEntry.BeforeAfter;
    			dbGlucoseEntry.WhichMeal = glucoseEntry.WhichMeal;
    			dbGlucoseEntry.CreatedAt = glucoseEntry.CreatedAt;
                dbGlucoseEntry.UpdatedAt = glucoseEntry.UpdatedAt;
                dbGlucoseEntry.Timestamp = glucoseEntry.Timestamp;
                _db.Entry( dbGlucoseEntry ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( Guid id )
        {
            var glucoseEntry = await ReadAsync( id );
            if( glucoseEntry != null )
            {

                _db.GlucoseEntries.Remove( glucoseEntry );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public async Task CreateOrUpdateEntries( ICollection<GlucoseEntry> glucoseEntries )
        {
            foreach ( GlucoseEntry glucoseEntry in glucoseEntries )
            {
                GlucoseEntry dbGlucoseEntry = await ReadAsync( glucoseEntry.Id );
                if ( dbGlucoseEntry == null )                  // If meal entry doesn't exist
                {
                    // Create in the database
                    await CreateAsync( glucoseEntry );

                }
                else if ( dbGlucoseEntry.UpdatedAt < glucoseEntry.UpdatedAt )
                {
                    // Update in the database
                    await UpdateAsync( glucoseEntry.Id, glucoseEntry );

                }

            } // foreach MealEntry

            return;

        } // CreateOrUpdateEntries

    } // Class

} // Namespace