using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ViewModels
{
    public class GlucoseEntryViewModel
    {
        public string PatientUsername { get; set; }
        public Patient Patient { get; set; }
        public float Measurement { get; set; }
        public int BeforeAfter { get; set; }
        public int WhichMeal { get; set; }
        public DateTime Date { get; set; }
        public long Timestamp { get; set; }

		public GlucoseEntry GetNewGlucoseEntries()
		{
			return new GlucoseEntry
			{
    			UserName = PatientUsername,
    			Patient = Patient,
    			Measurement = Measurement,
    			BeforeAfter = BeforeAfter,
    			WhichMeal = WhichMeal,
    			CreatedAt = Date,
    			Timestamp = Timestamp
			};

		} // GetNewGlucoseEntries

    } // Class

} // Namespace