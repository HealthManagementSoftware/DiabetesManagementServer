using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace DMS.Models
{
    public class GlucoseEntry
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        //public string UserId { get; set; }
        public Patient Patient { get; set; }
        public float Measurement { get; set; }
        
        public int BeforeAfter { get; set; }
        public int WhichMeal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long Timestamp { get; set; }

        //public override string ToString()
        //{
        //    return "GLUCOSE ENTRY:"
        //        + "\nId: " + Id
        //        + "\nPatient: " + UserName
        //        //+ "\nUserId: " + UserId
        //        + "\nMeasurement: " + Measurement
        //        + "\nBefore/After: " + BeforeAfter.ToString()
        //        + "\nWhich meal: " + WhichMeal.ToString()
        //        + "\nCreatedAt: " + CreatedAt
        //        + "\nUpdatedAt: " + UpdatedAt
        //        + "\nTimestamp: " + Timestamp;

        //} // ToString


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // ToString

    } // class

} // namespace