using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class ApplicationRole : IdentityRole
    {
        [Key]
        //[JsonProperty(PropertyName = "Id")]
        public override string Id { get; set; }

        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Discriminator { get; internal set; }

        //public List<ApplicationUser> Users { get; set; }


        public ApplicationRole()
        {
            //Users = new List<ApplicationUser>();

        } // constructor


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);

        } // ToString

    } // class

} // namespace
