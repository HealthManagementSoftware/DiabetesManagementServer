using DMS.Data;
using DMS.Models;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbDeveloperRepository : IDeveloperRepository
    {
        private readonly ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbDeveloperRepository( ApplicationDbContext db,
            IAuditRepository auditRepository )
        {
            _db = db;

        } // constructor


        public async Task<Developer> ReadAsync(string userName)
        {
            return await _db.Developers
                .SingleOrDefaultAsync( o => o.UserName == userName );

        } // ReadAsync


        public IQueryable<Developer> ReadAll()
        {
            return _db.Developers;

        } // ReadAll


        public async Task<Developer> CreateAsync(Developer developer)
        {
            _db.Developers.Add(developer);
            await _db.SaveChangesAsync();

            if (Config.AuditingOn)
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail(AuditActionType.CREATE, developer.Id, new Doctor(), developer);
                await _auditRepo.CreateAsync(auditChange);

            } // if

            return developer;

        } // CreateAsync


        public async Task UpdateAsync(string userName, Developer developer)
        {
            var oldDeveloper = await ReadAsync(userName);
            if (oldDeveloper != null)
            {
                if (Config.AuditingOn)
                {
                    var auditChange = new AuditChange();
                    if (!auditChange.CreateAuditTrail(AuditActionType.UPDATE, developer.Id, oldDeveloper, developer))
                        await _auditRepo.CreateAsync(auditChange);

                } // if

                oldDeveloper.Title = developer.Title;

                _db.Entry(oldDeveloper).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync(string userName)
        {
            var developer = await ReadAsync(userName);
            if (developer != null)
            {
                if (Config.AuditingOn)
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail(AuditActionType.DELETE, developer.Id, developer, new Doctor());
                    await _auditRepo.CreateAsync(auditChange);

                } // if

                _db.Developers.Remove(developer);
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync

    } // class

} // namespace
