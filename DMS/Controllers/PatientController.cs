using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DMS.Models;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DMS.Controllers
{
    [Authorize( Roles = Roles.PATIENT + ", " + Roles.DOCTOR )]
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
            if (patient.PatientSignedHIPAANotice != null && !patient.PatientSignedHIPAANotice.Signed)
                return RedirectToAction( nameof(SignHIPAANotice) );
            return View( patient );

        } // Index


        public async Task<IActionResult> SignHIPAANotice()
        {
            SignHIPAANoticeViewModel vm = new SignHIPAANoticeViewModel();
            vm.HIPAAPrivacyNotice = _hipaa.ReadNewest();
            vm.Patient = await _pat.ReadAsync(User.Identity.Name);
            if (vm.Patient == null)
                return RedirectToAction(nameof(Index));

            return View( vm );

        } // SignHIPAANotice


        [HttpPost]
        public async Task<IActionResult> SignHIPAANotice( SignHIPAANoticeViewModel vm )
        {
            if ( !ModelState.IsValid )
                return NotFound();

            // If patient hasn't agreed to privacy notice, we need to stay here:
            if (!vm.IAgree || vm.HIPAAPrivacyNotice == null)
                return View(vm);

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
            signedNotice = await _patientSignedHIPAA.CreateAsync(signedNotice);

            // Attach the signed notice to the patient signing it:
            Patient patient = await _pat.ReadAsync(User.Identity.Name);
            patient.PatientSignedHIPAANotice = signedNotice;
            await _pat.UpdateAsync(patient.UserName, patient);

            // Once signed, proceed to the main index:
            return RedirectToAction(nameof(Index));

        } // SignHIPAANotice


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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create( Patient patient )
        {
            if( ModelState.IsValid )
            {
                await _pat.CreateAsync( patient );
                return RedirectToAction( "Index" );
            }
            return View( patient );

        } // Create


        //[Authorize( Roles = Roles.DOCTOR + ", " + Roles.PATIENT )]
        public async Task<IActionResult> Details( Patient patient )
        {
            var pat = await _pat.ReadAsync( patient.UserName );

            if( User.Identity.Name != pat.UserName && User.Identity.Name != pat.Doctor.UserName )
                return new UnauthorizedResult();
            return View( await _pat.ReadAsync( patient.UserName ) );

        } // Details


    } // class

} // namespace