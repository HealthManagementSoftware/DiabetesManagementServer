using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class PatientSignedHIPAANotice
    {
        public Guid Id { get; set; }

        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public Guid HIPAAPrivacyNoticeId { get; set; }
        public HIPAAPrivacyNotice HIPAAPrivacyNotice { get; set; }
        public bool Signed { get; set; }

        public DateTime SignedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public PatientSignedHIPAANotice()
        {
            Signed = false;

        } // constructor

    } // class

} // namespace
