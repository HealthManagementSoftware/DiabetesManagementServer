using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Diagnostics;
using System.Linq;

namespace DMS.Services
{
    public class DatabaseSeeder
    {
        private ApplicationDbContext _context;
        private RoleManager<ApplicationRole> _roleManager;

        public DatabaseSeeder(
           ApplicationDbContext context,
           RoleManager<ApplicationRole> roleManager )
        {
            _context = context;
            _roleManager = roleManager;
        }


        public void SeedRoles()
        {
            _context.Database.EnsureCreated();
            ApplicationRole role;

            if ( !_context.Roles.Any( r => r.Name == Roles.DOCTOR ) )
            {
                Debug.WriteLine( "Creating Doctor role..." );
                role = new ApplicationRole
                {
                    Name = Roles.DOCTOR,
                    Description = "A role allowing doctors to view their patients' statistics.",
                    CreatedDate = DateTime.Now
                };
                _roleManager.CreateAsync( role );
            }

            if ( !_context.Roles.Any( r => r.Name == Roles.PATIENT ) )
            {
                Debug.WriteLine( "Creating Patient role..." );
                role = new ApplicationRole
                {
                    Name = Roles.PATIENT,
                    Description = "A patient, registered to a doctor",
                    CreatedDate = DateTime.Now
                };
                _roleManager.CreateAsync( role );
            }

        } // SeedRoles

    } // class

} // namespace
