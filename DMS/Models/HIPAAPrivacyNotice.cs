using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class HIPAAPrivacyNotice
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string NoticeText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
}
