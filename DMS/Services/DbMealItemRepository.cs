using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbMealItemRepository : IMealItemRepository
    {
        private readonly ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbMealItemRepository( ApplicationDbContext db,
                                    IAuditRepository auditRepo )
        {
            _db = db;
            _auditRepo = auditRepo;

        } // Injection Constructor


        public async Task<MealItem> ReadAsync( Guid mealid )
        {
            return await ReadAll()
                .Include( o => o.Meal )
                .SingleOrDefaultAsync( o => o.Id == mealid );

        } // ReadAsync


        public IQueryable<MealItem> ReadAll()
        {
            return _db.MealItems
                .Include( o => o.Meal );

        } // ReadAll


        public async Task<MealItem> CreateAsync( Guid mealEntryId, MealItem mealItem )
        {
            var mealEntry = await _db.MealEntries
                .Include( o => o.MealItems )
                .SingleOrDefaultAsync( o => o.Id == mealEntryId );
            if ( mealEntry != null )
            {
                mealEntry.MealItems.Add( mealItem );    // Associate item with the entry
                mealItem.Meal = mealEntry;              // Associate the entry with the item
                await _db.SaveChangesAsync();

            }// End if mealEntry not null statement.

            var auditChange = new AuditChange();
            auditChange.CreateAuditTrail( AuditActionType.CREATE, mealItem.Id.ToString(), new MealItem(), mealItem );
            await _auditRepo.CreateAsync( auditChange );

            return mealItem;

        } // CreateAsync


        public async Task UpdateAsync( Guid id, MealItem mealItem )
        {
            var dbMealItem = await ReadAsync( id );
            if ( dbMealItem != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.UPDATE, mealItem.Id.ToString(), dbMealItem, mealItem );
                await _auditRepo.CreateAsync( auditChange );

                dbMealItem.Name = mealItem.Name;
                dbMealItem.Carbs = mealItem.Carbs;
                dbMealItem.Servings = mealItem.Servings;
                //oldMealItem.Meal = mealItem.Meal;
                //oldMealItem.MealId = mealItem.MealId;
                dbMealItem.UpdatedAt = mealItem.UpdatedAt;
                //oldMealItem.UpdatedAt = mealItem.UpdatedAt;
                _db.Entry( dbMealItem.Meal ).State = EntityState.Unchanged;
                _db.Entry( dbMealItem ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( Guid id )
        {
            var mealItem = await ReadAsync( id );
            if ( mealItem != null )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.DELETE, mealItem.Id.ToString(), mealItem, new MealItem() );
                await _auditRepo.CreateAsync( auditChange );

                _db.MealItems.Remove( mealItem );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync
        

        public async Task CreateOrUpdateEntries( ICollection<MealItem> mealItems )
        {
            foreach ( MealItem mealItem in mealItems )
            {
                MealItem dbMealItem = await ReadAsync( mealItem.Id );
                if ( dbMealItem == null )                  // If meal entry doesn't exist
                {
                    // Create in the database
                    await CreateAsync( mealItem.Id, mealItem );

                }
                else if ( dbMealItem.UpdatedAt < mealItem.UpdatedAt )
                {
                    // Update in the database
                    await UpdateAsync( mealItem.Id, mealItem );

                }

            } // foreach MealEntry

            return;

        } // CreateOrUpdateEntries

    } // Class

} // Namespace