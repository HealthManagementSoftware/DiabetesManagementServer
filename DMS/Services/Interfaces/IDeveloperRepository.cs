using DMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services.Interfaces
{
    public interface IDeveloperRepository
    {
        Task<Developer> ReadAsync(string userName);
        IQueryable<Developer> ReadAll();
        Task<Developer> CreateAsync(Developer developer);
        Task UpdateAsync(string userName, Developer developer);
        Task DeleteAsync(string userName);

    }
}
