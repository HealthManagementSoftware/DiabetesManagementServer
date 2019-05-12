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
        private ApplicationDbContext _db;
        public IAuditRepository _auditRepository { get; set; }
        private RoleManager<ApplicationRole> _roleManager;

        public DatabaseSeeder(
           ApplicationDbContext context,
           IAuditRepository auditRepository,
           RoleManager<ApplicationRole> roleManager )
        {
            _db = context;
            _auditRepository = auditRepository;
            _roleManager = roleManager;
        }


        public void SeedRoles()
        {
            _db.Database.EnsureCreated();
            AuditChange change;
            ApplicationRole role;

            if ( !_db.Roles.Any( r => r.Name == Roles.DOCTOR ) )
            {
                Debug.WriteLine( "Creating Doctor role..." );
                role = new ApplicationRole
                {
                    Name = Roles.DOCTOR,
                    Description = "A role allowing doctors to view their patients' statistics.",
                    CreatedDate = DateTime.Now
                };
                _roleManager.CreateAsync( role );
                if (Config.AuditingOn)
                {
                    change = new AuditChange();
                    change.CreateAuditTrail(AuditActionType.CREATE, role.Id, new ApplicationRole(), role);
                    _auditRepository.CreateAsync(change);
                }
            }

            if ( !_db.Roles.Any( r => r.Name == Roles.PATIENT ) )
            {
                Debug.WriteLine( "Creating Patient role..." );
                role = new ApplicationRole
                {
                    Name = Roles.PATIENT,
                    Description = "A patient who is registered to a doctor.",
                    CreatedDate = DateTime.Now
                };
                _roleManager.CreateAsync( role );
                if (Config.AuditingOn)
                {
                    change = new AuditChange();
                    change.CreateAuditTrail(AuditActionType.CREATE, role.Id, new ApplicationRole(), role);
                    _auditRepository.CreateAsync(change);
                }
            }

            if (!_db.Roles.Any(r => r.Name == Roles.DEVELOPER))
            {
                Debug.WriteLine("Creating Developer role...");
                role = new ApplicationRole
                {
                    Name = Roles.DEVELOPER,
                    Description = "A developer role allowing heightened permissions.",
                    CreatedDate = DateTime.Now
                };
                _roleManager.CreateAsync(role);
                if (Config.AuditingOn)
                {
                    change = new AuditChange();
                    change.CreateAuditTrail(AuditActionType.CREATE, role.Id, new ApplicationRole(), role);
                    _auditRepository.CreateAsync(change);
                }
            }

        } // SeedRoles

    } // class

} // namespace
