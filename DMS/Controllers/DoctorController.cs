using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DMS.Models.ViewModels;
using DMS.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DMS.Models;

namespace DMS.Controllers
{
    public class DoctorController : Controller
    {
        private IDoctorRepository _doctorRepository;
        private IPatientRepository _patientRepository;
        private IApplicationUserRepository _userRepository;
        private UserManager<ApplicationUser> _userManager;

        public DoctorController( IPatientRepository patientRepository,
            IDoctorRepository doctorRepository,
            IApplicationUserRepository userRepo,
            UserManager<ApplicationUser> userManager)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _userRepository = userRepo;
            _userManager = userManager;

        } // Constructor


        [Authorize( Roles = Roles.DOCTOR )]
        public async Task<IActionResult> Index()
        {
            var doctor = await _doctorRepository.ReadAsync( User.Identity.Name );
            if( doctor != null && doctor.Patients != null )
                doctor.Patients = doctor.Patients
                    .OrderBy( l => l.LastName )
                    .ThenBy( f => f.FirstName )
                    .ToList();
            return View( doctor );

        } // Index


        [Authorize( Roles = Roles.DOCTOR )]
        public IActionResult PatientNames()
        {
            var model = _patientRepository.ReadAll()
                .OrderBy( l => l.LastName )
                .ThenBy( f => f.FirstName )
            .Select(p => new PatientViewModel
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                PhoneNumber = p.PhoneNumber
            });
            return View( model );

        } // PatientNames


        //[HttpPost]
        [Authorize( Roles = Roles.DOCTOR )]
        public IActionResult PatientDetails()
        {

            //var patient = _pat.ReadAsync(userName);

            var model = _patientRepository.ReadAll()
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

        } // PatientDetails


        [Authorize( Roles = Roles.DEVELOPER )]
        public async Task<IActionResult> List()
        {
            return View( await _doctorRepository.ReadAll().ToListAsync() );

        } // List


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
        public async Task<IActionResult> Create( DoctorViewModel doctorVM )
        {
            if ( ModelState.IsValid )
            {
                var doc = doctorVM.GetNewDoctor();
                doc.UserName = doc.Email;
                doc.NormalizedEmail = doc.Email.ToUpper();
                doc.NormalizedUserName = doc.UserName.ToUpper();
                doc = await _doctorRepository.CreateAsync( doc );
                await _userManager.AddToRoleAsync( doc, Roles.DOCTOR );
                return RedirectToAction( nameof( List ) );
            }
            return View( doctorVM );

        } // Create


        [Authorize( Roles = Roles.DEVELOPER )]
        public async Task<IActionResult> Edit( string UserName )
        {
            var doc = await _doctorRepository.ReadAsync( UserName );

            if ( doc == null )
                return NotFound();
            
            var docVM = new DoctorViewModel
            {
                Address1 = doc.Address1,
                Address2 = doc.Address2,
                City = doc.City,
                DegreeAbbreviation = doc.DegreeAbbreviation,
                FirstName = doc.FirstName,
                LastName = doc.LastName,
                Patients = doc.Patients,
                State = doc.State,
                UserName = UserName,
                Zip1 = doc.Zip1
            };
            return View( doc );

        } // Edit


        [Authorize( Roles = Roles.DEVELOPER )]
        [HttpPost]
        public async Task<IActionResult> Edit( string UserName, DoctorViewModel doctorVM )
        {
            if ( doctorVM == null )
                return NotFound();

            var doctor = doctorVM.GetNewDoctor();
            await _doctorRepository.UpdateAsync( UserName, doctor );
            await _userRepository.UpdateAsync( doctorVM.UserName, doctor );

            return RedirectToAction(nameof(List));

        } // Edit


        [Authorize( Roles = Roles.DEVELOPER )]
        // GET: HIPAAPrivacyNotices/Delete/5
        public async Task<IActionResult> Delete( string userName )
        {
            if ( userName == null )
            {
                return NotFound();
            }

            var doctor = await _doctorRepository.ReadAsync( userName );

            if ( doctor == null )
            {
                return NotFound();
            }

            return View( doctor );

        } // Delete


        // POST: HIPAAPrivacyNotices/Delete/5
        [Authorize( Roles = Roles.DEVELOPER )]
        [HttpPost, ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed( string userName )
        {
            await _doctorRepository.DeleteAsync( userName );
            return RedirectToAction( nameof( List ) );

        } // DeleteConfirmed


        // GET: HIPAAPrivacyNotices/Details/5
        public async Task<IActionResult> Details( string userName )
        {
            if ( userName == null )
            {
                return NotFound();
            }

            var doctor = await _doctorRepository.ReadAsync( userName );

            if ( doctor == null )
            {
                return NotFound();
            }

            return View( doctor );

        } // Details


    } // class

} // namespace