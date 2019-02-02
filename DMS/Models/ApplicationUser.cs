﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DMS.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        //public ICollection<IdentityRole> Roles { get; set; }
        public List<ApplicationRole> Roles { get; set; }
        //public IdentityUser User { get;  set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid RemoteLoginToken { get; set; }
        public long RemoteLoginExpiration { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int Zip1 { get; set; }
        public int Zip2 { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser()
        {
            Roles = new List<ApplicationRole>();

        } // constructor

        public bool HasRole(string roleName)
        {
            return Roles.Any( r => r.Name == roleName );
            //Role != null;
            //roleName = Role.ToString();
            //return Role;
        }


        public void CopyFrom( ApplicationUser user )
        {
            Roles = user.Roles;
            FirstName = user.FirstName;
            LastName = user.LastName;
            RemoteLoginToken = user.RemoteLoginToken;
            RemoteLoginExpiration = user.RemoteLoginExpiration;
            Address1 = user.Address1;
            Address2 = user.Address2;
            City = user.City;
            State = user.City;
            Zip1 = user.Zip1;
            Zip2 = user.Zip2;
            Height = user.Height;
            Weight = user.Weight;
            CreatedAt = user.CreatedAt;
            UpdatedAt = user.UpdatedAt;

        } // CopyFrom

    } // class

} // namespace
