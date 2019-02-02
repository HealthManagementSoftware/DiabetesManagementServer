﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        //public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        //public string Name { get; set; }
        //public string RoleId { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
