using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public string Name { get; set; }
        [JsonProperty(PropertyName = "RoleId")]
        public override string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
