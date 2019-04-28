using DMS.Models;
using DMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DummyAuditRepository : IAuditRepository
    {
        public async Task<AuditChange> ReadAsync( string auditId )
        {
            return null;
        }


        public IQueryable<AuditChange> ReadAll()
        {
            return null;
        }


        public async Task<AuditChange> CreateAsync( AuditChange auditChange )
        {
            return auditChange;

        }

    } // class

} // namespace
