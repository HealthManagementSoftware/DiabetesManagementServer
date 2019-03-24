using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbAuditRepository : IAuditRepository
    {
        public AuditDbContext _db { get; set; }

        public DbAuditRepository( AuditDbContext dbContext )
        {
            _db = dbContext;

        } // constructor 


        public async Task<AuditChange> ReadAsync( string auditId )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.Id == auditId );

        } // ReadAsync


        public IQueryable<AuditChange> ReadAll()
        {
            return _db.AuditChanges.Include( o => o.Changes );

        } // ReadAll


        public async Task<AuditChange> CreateAsync( AuditChange auditChange )
        {
            _db.AuditChanges.Add( auditChange );
            await _db.SaveChangesAsync();
            return auditChange;

        } // CreateAsync


    } // class

}  // namespace
