using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    /// <summary>
    /// Each instance of this class represents an action included in the AuditActionType class
    ///     (Create, Read, Update, or Delete). Each change will have a set of deltas, which are 
    ///     found using the CompareNETObjects nuget package.
    /// </summary>
    public class AuditChange
    {
        public const int MAX_DIFFS = 99;

        public int ID { get; set; }
        public string KeyFieldID { get; set; }
        public string DateTimeStamp { get; set; }
        public string DataModel { get; set; }
        public int Action { get; set; }
        public string AuditActionTypeName { get; set; }
        public List<AuditDelta> Changes { get; set; }

        public AuditChange()
        {
            DateTimeStamp = DateTime.Now.ToString();
            Changes = new List<AuditDelta>();

        } // constructor

        public void CreateAuditTrail( int action, string keyFieldID, Object oldObject, Object newObject )
        {
            // get the differance  
            CompareLogic compObjects = new CompareLogic();
            compObjects.Config.MaxDifferences = MAX_DIFFS;

            // This generates the deltas:
            ComparisonResult compResult = compObjects.Compare(oldObject, newObject);

            // remove "." in front of field/property names and add deltas to our list:
            foreach( var change in compResult.Differences )
            {
                AuditDelta delta = new AuditDelta();
                if( change.PropertyName.Substring( 0, 1 ) == "." )
                    delta.FieldName = change.PropertyName.Substring( 1, change.PropertyName.Length - 1 );
                delta.ValueBefore = change.Object1Value;
                delta.ValueAfter = change.Object2Value;
                Changes.Add( delta );
            }

            // Set values to save:
            Action = action;
            AuditActionTypeName = AuditActionType.GetTypeName( action );
            KeyFieldID = keyFieldID;

            // Save to an external database:
            // TODO

        } // CreateAuditTrail


    } // class

} // namespace
