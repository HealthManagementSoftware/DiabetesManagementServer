using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IHIPAANoticeRepository
    {
        HIPAAPrivacyNotice ReadNewest();
        string ReadNewestVersion();
        Task<HIPAAPrivacyNotice> ReadAsync( Guid id );
        IQueryable<HIPAAPrivacyNotice> ReadAll();
        Task<HIPAAPrivacyNotice> CreateAsync( HIPAAPrivacyNotice privacyNotice );
        Task UpdateAsync( Guid id, HIPAAPrivacyNotice privacyNotice );
        Task DeleteAsync( Guid id );

    } // interface

} // namespace
