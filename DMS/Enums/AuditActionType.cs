using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS
{
    public class AuditActionType
    {
        public const int CREATE = 0;
        public const int READ = 1;
        public const int UPDATE = 2;
        public const int DELETE = 3;
        
        public static string GetTypeName( int type )
        {
            switch( type )
            {
                case READ:
                    return "Read";

                case CREATE:
                    return "Create";

                case UPDATE:
                    return "Update";

                case DELETE:
                    return "Delete";
            }

            return null;

        } // GetAuditActionNameType

    } // class

} // namespace
