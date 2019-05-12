using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DMS.Data;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using DMS.Services.Interfaces;
using DMS.Models.ViewModels;

namespace DMS.Controllers
{
    [Authorize(Roles = Roles.DEVELOPER)]
    public class HIPAAPrivacyNoticesController : Controller
    {
        private readonly IHIPAANoticeRepository _hipaaRepo;

        public HIPAAPrivacyNoticesController(IHIPAANoticeRepository hIPAANotice )
        {
            _hipaaRepo = hIPAANotice;

        } // constructor


        // GET: HIPAAPrivacyNotices
        public async Task<IActionResult> Index()
        {
            return View( await _hipaaRepo.ReadAll().ToListAsync() );

        } // Index


        // GET: HIPAAPrivacyNotices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hIPAAPrivacyNotice = await _hipaaRepo.ReadAsync( (Guid) id );

            if (hIPAAPrivacyNotice == null)
            {
                return NotFound();
            }

            return View(hIPAAPrivacyNotice);

        } // Details


        // GET: HIPAAPrivacyNotices/Create
        public IActionResult Create()
        {
            return View();

        } // Create


        // POST: HIPAAPrivacyNotices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( HIPAAPrivacyNoticeViewModel hIPAAPrivacyNoticeVM )
        {
            if (ModelState.IsValid)
            {
                await _hipaaRepo.CreateAsync( hIPAAPrivacyNoticeVM.GetNewHIPAAPrivacyNotice() );
                return RedirectToAction(nameof(Index));
            }
            return View( hIPAAPrivacyNoticeVM );

        } // Create


        // GET: HIPAAPrivacyNotices/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var hIPAAPrivacyNotice = await _hipaaRepo.ReadAsync( (Guid) id );

            if (hIPAAPrivacyNotice == null)
                return NotFound();

            return View(hIPAAPrivacyNotice);

        } // Edit


        // POST: HIPAAPrivacyNotices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, HIPAAPrivacyNoticeViewModel hIPAAPrivacyNoticeVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _hipaaRepo.UpdateAsync( id, hIPAAPrivacyNoticeVM.GetNewHIPAAPrivacyNotice() );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( _hipaaRepo.ReadAsync(id) == null )
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hIPAAPrivacyNoticeVM);

        } // Edit


        // GET: HIPAAPrivacyNotices/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hIPAAPrivacyNotice = await _hipaaRepo.ReadAsync( (Guid) id );

            if (hIPAAPrivacyNotice == null)
            {
                return NotFound();
            }

            return View(hIPAAPrivacyNotice);

        } // Delete


        // POST: HIPAAPrivacyNotices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _hipaaRepo.DeleteAsync( (Guid) id );
            return RedirectToAction(nameof(Index));

        } // DeleteConfirmed

    } // class

} // namespace
