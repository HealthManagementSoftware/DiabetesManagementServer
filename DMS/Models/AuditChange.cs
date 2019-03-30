using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;
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

        public string Id { get; set; }
        public string KeyFieldID { get; set; }
        public string DateTimeStamp { get; set; }
        public string DataModel { get; set; }
        public int Action { get; set; }
        public string AuditActionTypeName { get; set; }
        public string Deltas { get; set; }

        public AuditChange()
        {
            Id = Guid.NewGuid().ToString();
            DateTimeStamp = DateTime.Now.ToString();

        } // constructor


        /// <summary>
        /// Create a new Audit, comparing two objects of any class type.
        /// </summary>
        /// <param name="auditActionType"></param>
        /// <param name="keyFieldID"></param>
        /// <param name="oldObject"></param>
        /// <param name="newObject"></param>
        public bool CreateAuditTrail( int auditActionType, string keyFieldID, Object oldObject, Object newObject, bool compareChildren = false )
        {
            // get the differance  
            var compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = Int32.MaxValue;
            compareLogic.Config.CompareChildren = compareChildren;

            // This generates the deltas:
            ComparisonResult compResult = compareLogic.Compare(oldObject, newObject);

            var changes = new List<AuditDelta>();
            // remove "." in front of field/property names and add deltas to our list:
            foreach( var change in compResult.Differences )
            {
                var delta = new AuditDelta();
                delta.FieldName = change.PropertyName.Substring( 0, 1 ) != "." 
                    ? change.PropertyName 
                    : change.PropertyName.Substring( 1, change.PropertyName.Length - 1 );
                delta.ValueBefore = change.Object1Value;
                delta.ValueAfter = change.Object2Value;
                changes.Add( delta );
            }

            // Set values to save:
            Action = auditActionType;
            AuditActionTypeName = AuditActionType.GetTypeName( auditActionType );
            KeyFieldID = keyFieldID;
            var split = oldObject.GetType().ToString().Split('.');
            DataModel = split[ split.Length - 1 ];
            Deltas = JsonConvert.SerializeObject( changes );//compResult.DifferencesString;//

            // Save to database after creating the audit trail

            return compResult.AreEqual;

        } // CreateAuditTrail


    } // class

} // namespace
