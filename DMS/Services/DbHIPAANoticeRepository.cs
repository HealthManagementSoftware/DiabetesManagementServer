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
    public class DbHIPAANoticeRepository : IHIPAANoticeRepository
    {
        private ApplicationDbContext _db;
        private IAuditRepository _auditRepo;

        public DbHIPAANoticeRepository( ApplicationDbContext db, IAuditRepository auditRepo )
        {
            _db = db;
            _auditRepo = auditRepo;

        } // constructor


        public async Task<HIPAAPrivacyNotice> ReadAsync( Guid id )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( n => n.Id == id );

        } // ReadAsync


        public IQueryable<HIPAAPrivacyNotice> ReadAll()
        {
            return _db.HIPAAPrivacyNotices;

        } // ReadAll


        public async Task<HIPAAPrivacyNotice> CreateAsync( HIPAAPrivacyNotice privacyNotice )
        {
            await _db.HIPAAPrivacyNotices.AddAsync( privacyNotice );
            await _db.SaveChangesAsync();

            if( Config.AuditingOn )
            {
                var auditChange = new AuditChange();
                auditChange.CreateAuditTrail( AuditActionType.CREATE, privacyNotice.Id.ToString(), new HIPAAPrivacyNotice(), privacyNotice );
                await _auditRepo.CreateAsync( auditChange );

            } // if

            return privacyNotice;

        } // CreateAsync


        public async Task UpdateAsync( Guid id, HIPAAPrivacyNotice privacyNotice )
        {
            var dbNotice = await ReadAsync( id );
            if( dbNotice != null )
            {
                if( Config.AuditingOn )
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail( AuditActionType.UPDATE, privacyNotice.Id.ToString(), dbNotice, privacyNotice );
                    await _auditRepo.CreateAsync( auditChange );
                    
                } // if

                dbNotice.CreatedAt = privacyNotice.CreatedAt;
                dbNotice.NoticeText = privacyNotice.NoticeText;
                dbNotice.Title = privacyNotice.Title;
                dbNotice.UpdatedAt = privacyNotice.UpdatedAt;
                dbNotice.Version = privacyNotice.Version;

                _db.Entry( dbNotice ).State = EntityState.Modified;
                await _db.SaveChangesAsync();

            } // if

            return;

        } // UpdateAsync

        public async Task DeleteAsync( Guid id )
        {
            var notice = await ReadAsync( id );
            if( notice != null )
            {
                if( Config.AuditingOn )
                {
                    var auditChange = new AuditChange();
                    auditChange.CreateAuditTrail( AuditActionType.DELETE, notice.Id.ToString(), notice, new HIPAAPrivacyNotice() );
                    await _auditRepo.CreateAsync( auditChange );

                } // if

                _db.HIPAAPrivacyNotices.Remove( notice );
                await _db.SaveChangesAsync();
            }
            return;

        }

    } // class

} // namespace
