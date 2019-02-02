using DMS.Models;
using DMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IGlucoseEntryRepository
    {
        Task<GlucoseEntry> ReadAsync( Guid id );
        IQueryable<GlucoseEntry> ReadAll();
        Task<GlucoseEntry> CreateAsync( GlucoseEntry glucoseEntry );
        Task UpdateAsync( Guid id, GlucoseEntry glucoseEntry );
        Task DeleteAsync( Guid id );
        Task CreateOrUpdateEntries( ICollection<GlucoseEntry> glucoseEntries );

    } // Interface

} // Namespace