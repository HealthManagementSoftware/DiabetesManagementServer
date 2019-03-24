using DMS.Models;
using Microsoft.EntityFrameworkCore;

namespace DMS.Data
{
    public class AuditDbContext : DbContext
    {
        public DbSet<AuditChange> AuditChanges { get; set; }
        public DbSet<AuditDelta> AuditDeltas { get; set; }

        public AuditDbContext( DbContextOptions<AuditDbContext> options )
            : base( options )
        {
            // Constructor to pass options
        }


        protected override void OnModelCreating( ModelBuilder builder )
        {
            base.OnModelCreating( builder );
        }


    } // class

} // namespace
