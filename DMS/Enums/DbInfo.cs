using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS
{
    public class DbInfo
    {
        public const string KEY_PRIMARY_DB_SECTION = "CosmosTestSettings";

        public const string KEY_SERVICE_ENDPOINT = "ServiceEndpoint";
        public const string KEY_AUTH_KEY = "AuthKey";
        public const string KEY_DB_NAME  ="DatabaseName";

        public const string PRIMARY_DB_NAME = "DiabetesDB";
        public const string COLLECTION_NAME = "ApplicationDbContext";

        // Values specific to Audit DB:

        public const string KEY_AUDIT_DB_SECTION = "AuditCosmosSettings";

        public const string KEY_AUDIT_AUTH_KEY = "AuditAuthKey";

        public const string AUDIT_DB_NAME = "DiabetesAuditDB";
        public const string AUDIT_COLLECTION_NAME = "AuditDbContext";

    }
}
