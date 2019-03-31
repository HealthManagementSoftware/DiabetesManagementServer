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
    public class DbMealEntryRepository : IMealEntryRepository
    {
        private readonly ApplicationDbContext _db;
        private IMealItemRepository _mealItemRepository;
        private IAuditRepository _auditRepo;

        public DbMealEntryRepository( ApplicationDbContext db,
                                    IMealItemRepository mealItemRepository,
                                    IAuditRepository auditRepo )
        {
            _db = db;
            _mealItemRepository = mealItemRepository;
            _auditRepo = auditRepo;

        } // Injection Constructor


        public async Task<MealEntry> ReadAsync( Guid id )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.Id == id );

        } // ReadAsync


        public IQueryable<MealEntry> ReadAll()
        {
            return _db.MealEntries;
            //.Include( o => o.MealItems );

        } // ReadAll


        public async Task<MealEntry> CreateAsync( MealEntry mealentry )
        {
            _db.MealEntries.Add( mealentry );
            await _db.SaveChangesAsync();

            var auditChange = new AuditChange();
            auditChange.CreateAuditTrail( AuditActionType.CREATE, mealentry.Id.ToString(), new MealEntry(), mealentry );
            await _auditRepo.CreateAsync( auditChange );

            return mealentry;

        } // Create


        public async Task UpdateAsync( Guid id, MealEntry mealEntry )
        {
            var dbMealEntry = await ReadAsync( id );
            if( dbMealEntry != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.UPDATE, mealEntry.Id.ToString(), dbMealEntry, mealEntry );
                await _auditRepo.CreateAsync( auditChange );

                dbMealEntry.UserName = mealEntry.UserName;
                dbMealEntry.Patient = mealEntry.Patient;
                dbMealEntry.TotalCarbs = mealEntry.TotalCarbs;
                dbMealEntry.CreatedAt = mealEntry.CreatedAt;
                dbMealEntry.UpdatedAt = mealEntry.UpdatedAt;
                dbMealEntry.Timestamp = mealEntry.Timestamp;
                //oldMealEntry.MealItems = mealEntry.MealItems;

                _db.Entry( mealEntry.MealItems ).State = EntityState.Unchanged;
                _db.Entry( dbMealEntry.MealItems ).State = EntityState.Unchanged;

                foreach( var mealItem in mealEntry.MealItems )
                    _db.Entry( mealItem ).State = EntityState.Unchanged;

                foreach( var mealItem in dbMealEntry.MealItems )
                    _db.Entry( mealItem ).State = EntityState.Unchanged;

                _db.Entry( dbMealEntry ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( Guid id )
        {
            var mealEntry = await ReadAsync( id );
            if( mealEntry != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.DELETE, mealEntry.Id.ToString(), mealEntry, new MealEntry() );
                await _auditRepo.CreateAsync( auditChange );

                _db.MealEntries.Remove( mealEntry );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public async Task CreateOrUpdateEntries( ICollection<MealEntry> mealEntries )
        {
            foreach( MealEntry mealEntry in mealEntries )
            {
                MealEntry dbMealEntry = await ReadAsync( mealEntry.Id );
                if( dbMealEntry == null )                  // If meal entry doesn't exist
                {
                    // Create in the database
                    await CreateAsync( mealEntry );

                }
                else if( dbMealEntry.UpdatedAt < mealEntry.UpdatedAt )
                {
                    // Update in the database
                    await UpdateAsync( mealEntry.Id, mealEntry );

                }

                // Check whether meal items need to be created/updated:
                await _mealItemRepository.CreateOrUpdateEntries( mealEntry.MealItems );

            } // foreach MealEntry

            return;

        } // CreateOrUpdateEntries

    } // Class

} // Namespace