using DMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ViewModels
{
    public class SignHIPAANoticeViewModel
    {
        public string PatientId { get; set; }
        public Patient Patient { get; set; }
        public HIPAAPrivacyNotice HIPAAPrivacyNotice { get; set; }
        public bool Signed { get; set; }
        public DateTime SignedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public bool IAgree { get; set; }

        public PatientSignedHIPAANotice GetNewPatientSignedHIPAANotice()
		{
			return new PatientSignedHIPAANotice
			{
    			PatientId = PatientId,
    			Patient = Patient,
                HIPAAPrivacyNoticeId = HIPAAPrivacyNotice.Id,
    			HIPAAPrivacyNotice = HIPAAPrivacyNotice,
    			Signed = IAgree,
    			SignedAt = DateTime.Now,
    			UpdatedAt = SignedAt
			};

        } // GetNewPatientSignedHIPAANotice

    } // Class

} // Namespace