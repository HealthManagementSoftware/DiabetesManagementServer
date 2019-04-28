using DMS.Data;
using DMS.Models;
using DMS.Models.Entities;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class DbHIPAAPrivacyNoticeRepository : IHIPAAPrivacyNoticeRepository
    {
        private readonly ApplicationDbContext _db;

        public DbHIPAAPrivacyNoticeRepository( ApplicationDbContext db )
        {
            _db = db;

        } // Injection Constructor


        public async Task<HIPAAPrivacyNotice> ReadAsync( Guid id )
        {
            return await ReadAll()
                .SingleOrDefaultAsync( o => o.Id == id );

        } // ReadAsync


        public IQueryable<HIPAAPrivacyNotice> ReadAll()
        {
            return _db.HIPAAPrivacyNotice;

        } // ReadAll


        public async Task<HIPAAPrivacyNotice> CreateAsync( HIPAAPrivacyNotice hipaaprivacynotice )
        {
            _db.HIPAAPrivacyNotice.Add( hipaaprivacynotice );
            await _db.SaveChangesAsync();
            return hipaaprivacynotice;

        } // Create


        public async Task UpdateAsync( Guid id, HIPAAPrivacyNoticeViewModel hipaaprivacynoticeVM )
        {
            var oldHIPAAPrivacyNotice = await ReadAsync( id );
            if( oldHIPAAPrivacyNotice != null )
            {
    			oldHIPAAPrivacyNotice.Title = hipaaprivacynoticeVM.Title;
    			oldHIPAAPrivacyNotice.NoticeText = hipaaprivacynoticeVM.NoticeText;
    			oldHIPAAPrivacyNotice.Version = hipaaprivacynoticeVM.Version;
    			oldHIPAAPrivacyNotice.CreatedAt = hipaaprivacynoticeVM.CreatedAt;
    			oldHIPAAPrivacyNotice.UpdatedAt = hipaaprivacynoticeVM.UpdatedAt;
                _db.Entry( oldHIPAAPrivacyNotice ).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return;
            }

        } // UpdateAsync


        public async Task DeleteAsync( Guid id )
        {
            var hipaaprivacynotice = await ReadAsync( id );
            if( hipaaprivacynotice != null )
            {
                _db.HIPAAPrivacyNotice.Remove( hipaaprivacynotice );
                await _db.SaveChangesAsync();
            }
            return;

        } // DeleteAsync

    } // Class

} // Namespace