using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IHIPAAPrivacyNoticeRepository
    {
        Task<HIPAAPrivacyNotice> ReadAsync(Guid id);
        IQueryable<HIPAAPrivacyNotice> ReadAll();
        Task<HIPAAPrivacyNotice> CreateAsync(HIPAAPrivacyNotice hipaaprivacynotice);
        Task UpdateAsync(Guid id, HIPAAPrivacyNotice hipaaprivacynotice);

    }
}
