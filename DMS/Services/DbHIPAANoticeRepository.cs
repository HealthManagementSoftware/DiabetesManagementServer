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

        public DbHIPAANoticeRepository(ApplicationDbContext db)
        {
            _db = db;

        } // constructor


        public async Task<HIPAAPrivacyNotice> ReadAsync(Guid id)
        {
            return await ReadAll()
                .SingleOrDefaultAsync(n => n.Id == id);

        } // ReadAsync


        public IQueryable<HIPAAPrivacyNotice> ReadAll()
        {
            return _db.HIPAAPrivacyNotices;

        } // ReadAll


        public async Task<HIPAAPrivacyNotice> CreateAsync(HIPAAPrivacyNotice privacyNotice)
        {
            await _db.HIPAAPrivacyNotices.AddAsync(privacyNotice);
            await _db.SaveChangesAsync();


            return privacyNotice;

        } // CreateAsync


        public async Task UpdateAsync(Guid id, HIPAAPrivacyNotice privacyNotice)
        {
            var dbNotice = await ReadAsync(id);
            if (dbNotice != null)
            {

                dbNotice.CreatedAt = privacyNotice.CreatedAt;
                dbNotice.NoticeText = privacyNotice.NoticeText;
                dbNotice.Title = privacyNotice.Title;
                dbNotice.UpdatedAt = privacyNotice.UpdatedAt;
                dbNotice.Version = privacyNotice.Version;

                _db.Entry(dbNotice).State = EntityState.Modified;
                await _db.SaveChangesAsync();

            } // if

            return;

        } // UpdateAsync

        public async Task DeleteAsync(Guid id)
        {
            var notice = await ReadAsync(id);
            if (notice != null)
            {

                _db.HIPAAPrivacyNotices.Remove(notice);
                await _db.SaveChangesAsync();
            }
            return;

        }

    } // class

} // namespace
