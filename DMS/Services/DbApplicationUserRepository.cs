using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbApplicationUserRepository : IApplicationUserRepository
    {
        private ApplicationDbContext _db;
        private IAuditRepository _auditRepo;


        private UserManager<ApplicationUser> _userManager;

        public DbApplicationUserRepository( ApplicationDbContext db,
                                            IAuditRepository auditRepo,
                                            UserManager<ApplicationUser> userManager )
        {
            _db = db;
            _auditRepo = auditRepo;
            _userManager = userManager;

        } // constructor


        public bool UserExists( string userName )
        {
            return _db.Users.Any( o => o.UserName == userName );

        } // UserExists


        public async Task<bool> AssignRole( string email, string roleName )
        {
            var user = await ReadAsync(email);

            if ( user != null )
            {
                if ( !user.HasRole( roleName ) )
                {
                    Debug.WriteLine( "User doesn't have role '" + roleName + "'. Adding..." );
                    await _userManager.AddToRoleAsync( user, roleName );//.Wait();
                    return true;
                }
                else
                    Debug.WriteLine( "User has role '" + roleName + "'." );
            }
            return false;

        } // AssignRole


        public ApplicationUser ReadUser( string email )
        {
            ApplicationUser appUser = _db.Users
                .Include( r => r.Roles )
                .FirstOrDefault( u => u.Email == email );

            return appUser;

        }

        public async Task<ApplicationUser> ReadAsync( string username )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.UserName == username );

        } // ReadAsync


        public IQueryable<ApplicationUser> ReadAll()
        {
            return _db.Users
                .Include( r => r.Roles );

        } // ReadAll


        public async Task<ApplicationUser> CreateAsync( ApplicationUser applicationUser )
        {
            _db.Users.Add( applicationUser );
            await _db.SaveChangesAsync();

            if (Config.AuditingOn)
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail(AuditActionType.CREATE, applicationUser.Id, new ApplicationUser(), applicationUser);
                await _auditRepo.CreateAsync(auditChange);
            }
            return applicationUser;

        } // CreateAsync


        public async Task UpdateAsync( string username, ApplicationUser applicationUser )
        {
            var dbUser = await ReadAsync( username );
            if ( dbUser != null )
            {

                if (Config.AuditingOn)
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail(AuditActionType.UPDATE, applicationUser.Id, dbUser, applicationUser);
                    await _auditRepo.CreateAsync(auditChange);
                }

                dbUser.Address1 = applicationUser.Address1;
                dbUser.Address2 = applicationUser.Address2;
                dbUser.City = applicationUser.City;
                if ( !string.IsNullOrEmpty( applicationUser.Email ) )
                    dbUser.Email = applicationUser.Email;
                dbUser.LastName = applicationUser.LastName;
                dbUser.PhoneNumber = applicationUser.PhoneNumber;
                dbUser.State = applicationUser.State;
                dbUser.Zip1 = applicationUser.Zip1;
                dbUser.Zip2 = applicationUser.Zip2;

                _db.Entry( dbUser ).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                return;

            } // if

        } // UpdateAsync


        public async Task DeleteAsync( string username )
        {
            var user = await ReadAsync( username );
            if ( user != null )
            {
                if (Config.AuditingOn)
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail(AuditActionType.DELETE, user.Id, user, new ApplicationUser());
                    await _auditRepo.CreateAsync(auditChange);
                }

                _db.Users.Remove( user );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync


        public List<ApplicationRole> ReadAllRoles()
        {
            return _db.Roles.ToList();

        } // ReadAllRoles


        //public bool HasRole(string roleName)
        //{
        //    string role = Read(roleName).ToString();
        //    if(roleName == role)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public ApplicationRole Read(string role)
        //{
        //    return _db.Roles.FirstOrDefault(r => r.Role == role);
        //}
    }

}
