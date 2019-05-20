using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DMS.Models;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace DMS.Controllers
{
    [Authorize( Roles = Roles.PATIENT + ", " + Roles.DOCTOR + ", " + Roles.DEVELOPER )]
    public class PatientController : Controller
    {
        private IPatientRepository _pat;
        private IPatientSignedHIPAANoticeRepository _patientSignedHIPAA;
        private IDoctorRepository _doc;
        private IHIPAANoticeRepository _hipaa;

        public PatientController( IPatientRepository pat,
                                 IDoctorRepository doc,
                                 IHIPAANoticeRepository hipaa,
                                 IPatientSignedHIPAANoticeRepository patientSignedHIPAA )
        {
            _pat = pat;
            _patientSignedHIPAA = patientSignedHIPAA;
            _doc = doc;
            _hipaa = hipaa;

        } // Constructor


        public async Task<IActionResult> Index()
        {
            var patient = await _pat.ReadAsync(User.Identity.Name);
            if ( patient.PatientSignedHIPAANotice != null && !patient.PatientSignedHIPAANotice.Signed )
                return RedirectToAction( nameof( SignHIPAANotice ) );
            return View( patient );

        } // Index


        public async Task<IActionResult> SignHIPAANotice()
        {
            SignHIPAANoticeViewModel vm = new SignHIPAANoticeViewModel();
            vm.HIPAAPrivacyNotice = _hipaa.ReadNewest();
            vm.Patient = await _pat.ReadAsync( User.Identity.Name );
            if ( vm.Patient == null )
                return RedirectToAction( nameof( Index ) );

            return View( vm );

        } // SignHIPAANotice


        [HttpPost]
        public async Task<IActionResult> SignHIPAANotice( SignHIPAANoticeViewModel vm )
        {
            if ( !ModelState.IsValid )
                return NotFound();

            // If patient hasn't agreed to privacy notice, we need to stay here:
            if ( !vm.IAgree || vm.HIPAAPrivacyNotice == null )
                return View( vm );

            var notice = _hipaa.ReadNewest();

            // Create the record of the signed notice in the sytem:
            var signedNotice = new PatientSignedHIPAANotice //vm.GetNewPatientSignedHIPAANotice();
            {
                HIPAAPrivacyNoticeId = notice.Id,
                HIPAAPrivacyNotice = notice,
                PatientId = User.Identity.Name,
                Patient = await _pat.ReadAsync( User.Identity.Name ),
                Signed = true,
                SignedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            signedNotice = await _patientSignedHIPAA.CreateAsync( signedNotice );

            // Attach the signed notice to the patient signing it:
            Patient patient = await _pat.ReadAsync(User.Identity.Name);
            patient.PatientSignedHIPAANotice = signedNotice;
            await _pat.UpdateAsync( patient.UserName, patient );

            // Once signed, proceed to the main index:
            return RedirectToAction( nameof( Index ) );

        } // SignHIPAANotice


        [Authorize( Roles = Roles.DEVELOPER + ", " + Roles.DOCTOR )]
        public IActionResult PatientList()
        {
            var model = _pat.ReadAll()
            .Select(p => new PatientViewModel
            {
                UserName = p.UserName,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Address1 = p.Address1,
                Address2 = p.Address2,
                City = p.City,
                State = p.State,
                Zip1 = p.Zip1,
                Zip2 = p.Zip2,
                PhoneNumber = p.PhoneNumber,
                Email = p.Email
            });
            return View( model );
        }


        [Authorize( Roles = Roles.DEVELOPER + "," + Roles.DOCTOR )]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize( Roles = Roles.DEVELOPER + "," + Roles.DOCTOR )]
        [HttpPost]
        public async Task<IActionResult> Create( Patient patient )
        {
            if ( ModelState.IsValid )
            {
                await _pat.CreateAsync( patient );
                if ( User.IsInRole( Roles.DEVELOPER ) )
                    return RedirectToAction( nameof( PatientList ) );
                else if( User.IsInRole( Roles.PATIENT ) )
                    return RedirectToAction( nameof( Index ) );
                else if ( User.IsInRole( Roles.DOCTOR ) )
                    return RedirectToAction( nameof( Index ), Roles.DOCTOR );
                 
            }
            return View( patient );

        } // Create


        [Authorize( Roles = Roles.DEVELOPER + "," + Roles.DOCTOR )]
        public async Task<IActionResult> Edit( Patient patient )
        {
           // var patient = await _pat.ReadAll()
           //     .SingleOrDefaultAsync( u => u.UserName == UserName );
            if ( patient == null )
                return NotFound();
            patient = await _pat.ReadAsync( patient.UserName );
            var doc = await _doc.ReadAsync( patient.Doctor.UserName );
            var patientVM = new PatientViewModel
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Address1 = patient.Address1,
                Address2 = patient.Address2,
                City = patient.City,
                State = patient.State,
                Zip1 = patient.Zip1,
                Zip2 = patient.Zip2,
                Email = patient.Email,
                DoctorUserName = doc?.UserName,
                PhoneNumber = patient.PhoneNumber
            };
            var AllDoctors = _doc.ReadAll()
                .OrderBy( n => n.UserName )
                .ToList();
            ViewData[ "doctors" ] = AllDoctors;
            return View( patientVM );
        }


        [Authorize( Roles = Roles.DEVELOPER + "," + Roles.DOCTOR )]
        [HttpPost]
        public async Task<IActionResult> Edit( string UserName, PatientViewModel patientVM )
        {
            if ( ModelState.IsValid )
            {
                var editedPat = await patientVM.GetNewPatient( _doc );
                //editedPat.Doctor = await _doc.ReadAsync( patientVM.DoctorUserName );
                //editedPat.DoctorUserName = patientVM.DoctorUserName;
                await _pat.UpdateAsync( patientVM.UserName, editedPat );
                if ( User.IsInRole( Roles.DEVELOPER ) )
                    return RedirectToAction( nameof( PatientList ) );
                else if( User.IsInRole( Roles.PATIENT ) )
                    return RedirectToAction( nameof( Index ) );
                else if ( User.IsInRole( Roles.DOCTOR ) )
                    return RedirectToAction( nameof( Index ), Roles.DOCTOR );
            }
            return View( patientVM );

        } // Create


        [Authorize( Roles = Roles.DEVELOPER )]
        // GET: Patient/Delete/5
        public async Task<IActionResult> Delete( string userName )
        {
            if ( userName == null )
            {
                return NotFound();
            }

            var patient = await _pat.ReadAsync( userName );

            if ( patient == null )
            {
                return NotFound();
            }

            return View( patient );

        } // Delete


        // POST: Patient/Delete/5
        [Authorize( Roles = Roles.DEVELOPER )]
        [HttpPost, ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( string userName )
        {
            await _pat.DeleteAsync( userName );
            return RedirectToAction( nameof( PatientList ) );

        } // DeleteConfirmed


        //[Authorize( Roles = Roles.DOCTOR + ", " + Roles.PATIENT )]
        public async Task<IActionResult> Details( Patient patient )
        {
            //var pat = await _pat.ReadAsync( patient.UserName );
            //if ( User.Identity.Name != patient.UserName || !User.IsInRole( Roles.DEVELOPER ) || !User.IsInRole( Roles.DOCTOR ) )
            //    return new UnauthorizedResult();
            return View( await _pat.ReadAsync( patient.UserName ) );

        } // Details


    } // class

} // namespace