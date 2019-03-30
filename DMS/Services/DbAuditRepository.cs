using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbAuditRepository : IAuditRepository
    {
        // EF Core is currently broken, so using direct approach until fixed in later versions,
        //      rather than being able to use a DbContext...
        //public AuditDbContext _db { get; set; }
        DocumentClient auditClient;
        public IConfiguration Configuration { get; }
        private const string databaseName = DbInfo.AUDIT_DB_NAME;
        private const string collectionName = DbInfo.AUDIT_COLLECTION_NAME;

        public DbAuditRepository( //AuditDbContext dbContext, 
            IConfiguration configuration)
        {
            //_db = dbContext;
            Configuration = configuration;
            IConfigurationSection auditConfigSection = Configuration.GetSection(DbInfo.KEY_AUDIT_DB_SECTION);

            try
            {
                auditClient = new DocumentClient(
                    new Uri(auditConfigSection[DbInfo.KEY_SERVICE_ENDPOINT]),
                    auditConfigSection[DbInfo.KEY_AUDIT_AUTH_KEY]
                    );
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }

        } // constructor 


        public async Task<AuditChange> ReadAsync( string auditId )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.Id == auditId );

        } // ReadAsync


        public IQueryable<AuditChange> ReadAll()
        {
            //return _db.AuditChanges.Include( o => o.Changes );
            return (IQueryable<AuditChange>) new List<AuditChange>();

        } // ReadAll


        public async Task<AuditChange> CreateAsync( AuditChange auditChange )
        {
            //_db.AuditChanges.Add( auditChange );
            //await _db.SaveChangesAsync();
            await auditClient.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), auditChange
                    );
            //if(auditChange.Deltas.Count > 0)
                //foreach(var delta in auditChange.Deltas)
                //    await auditClient.CreateDocumentAsync(
                //    UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), delta
                //    );
            return auditChange;

        } // CreateAsync


    } // class

}  // namespace
