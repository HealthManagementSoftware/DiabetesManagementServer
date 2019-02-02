using DMS.Models;
using DMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IMealEntryRepository
    {
        Task<MealEntry> ReadAsync( Guid id );
        IQueryable<MealEntry> ReadAll();
        Task<MealEntry> CreateAsync( MealEntry mealEntry );
        Task UpdateAsync( Guid id, MealEntry mealEntry );
        Task DeleteAsync( Guid id );
        Task CreateOrUpdateEntries( ICollection<MealEntry> mealEntries );

    } // Interface

} // Namespace