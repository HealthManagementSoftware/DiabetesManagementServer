using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models.ViewModels
{
    public class HIPAAPrivacyNoticeViewModel
    {
        public string Title { get; set; }
        public string NoticeText { get; set; }
        public string Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<PatientSignedHIPAANotice> Signatures { get; set; }

		public HIPAAPrivacyNotice GetNewHIPAAPrivacyNotice()
		{
			return new HIPAAPrivacyNotice
			{
    			Title = Title,
    			NoticeText = NoticeText,
    			Version = Version,
    			CreatedAt = DateTime.Now,
    			UpdatedAt = DateTime.Now
            };

		} // GetNewHIPAAPrivacyNotice

    } // Class

} // Namespace