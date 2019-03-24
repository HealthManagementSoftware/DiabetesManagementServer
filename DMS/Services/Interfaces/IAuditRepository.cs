using DMS.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IAuditRepository
    {
        Task<AuditChange> ReadAsync( string auditId );
        IQueryable<AuditChange> ReadAll();
        Task<AuditChange> CreateAsync( AuditChange auditChange );
        // No update or delete needed, due to need for permanence

    } // interface

} // namespace
