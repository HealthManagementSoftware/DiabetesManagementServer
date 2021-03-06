﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DMS.Models;
using DMS.Models.ManageViewModels;
using DMS.Services;
using DMS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DMS.Controllers
{
    [Authorize]
    [Route( "[controller]/[action]" )]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UrlEncoder _urlEncoder;
        private IApplicationUserRepository _userRepository;
        private IDoctorRepository _doctorRepository;
        private IPatientRepository _patientRepository;
        private IDeveloperRepository _devRepo;

        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private const string RecoveryCodesKey = nameof(RecoveryCodesKey);

        public ManageController(
          UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager,
          IEmailSender emailSender,
          ILogger<ManageController> logger,
          UrlEncoder urlEncoder,
          IApplicationUserRepository users,
          IDoctorRepository doctorRepo,
          IPatientRepository patientRepo,
          IDeveloperRepository devRepo )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _urlEncoder = urlEncoder;
            _userRepository = users;
            _doctorRepository = doctorRepo;
            _patientRepository = patientRepo;
            _devRepo = devRepo;
        }


        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            Patient patient = null;
            Doctor doctor = null;

            if ( User.IsInRole( Roles.PATIENT ) )
            {
                patient = await _patientRepository.ReadAsync( User.Identity.Name );
                doctor = _doctorRepository.ReadAll().Where( u => u.UserName == patient.Doctor.UserName ).FirstOrDefault();
            }
            else if ( User.IsInRole( Roles.DOCTOR ) )
            {
                doctor = await _doctorRepository.ReadAsync( User.Identity.Name );
            }

            // If a user is a patient, get all doctors for the list:
            List<Doctor> doctorList = User.IsInRole( Roles.PATIENT )
                ? _doctorRepository.ReadAll()
                .OrderBy( l => l.LastName )
                .Select( d => new Doctor
                {
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    UserName = d.UserName,
                    DegreeAbbreviation = d.DegreeAbbreviation
                } )
                .ToList()
                : new List<Doctor>();

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address1 = user.Address1,
                Address2 = user.Address2,
                City = user.City,
                State = user.State,
                Zip1 = user.Zip1,
                Zip2 = user.Zip2,
                StatusMessage = StatusMessage,
                DegreeAbbreviation = doctor?.DegreeAbbreviation,
                DoctorFullName = doctor?.FirstName + " " + doctor?.LastName + ", " + doctor?.DegreeAbbreviation,
                AllDoctors = doctorList,
                DoctorUserName = doctor?.UserName
            };

            return View( model );

        } // Index


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index( IndexViewModel model )
        {
            if ( !ModelState.IsValid )
                return View( model );

            var user = await _userManager.GetUserAsync( User );
            await UpdateProfileItems( model, user );

            if ( User.IsInRole( Roles.DOCTOR ) )
            {
                var doctor = await _doctorRepository.ReadAsync( User.Identity.Name );
                if ( doctor == null )
                    throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );

                var currentDoctor = Doctor.Clone(doctor);
                currentDoctor.DegreeAbbreviation = model.DegreeAbbreviation;
                currentDoctor.FirstName = model.FirstName;
                currentDoctor.LastName = model.LastName;
                currentDoctor.Address1 = model.Address1;
                currentDoctor.Address2 = model.Address2;
                currentDoctor.City = model.City;
                currentDoctor.State = model.State;
                currentDoctor.Zip1 = model.Zip1;

                currentDoctor.Zip2 = model.Zip2;
                currentDoctor.Email = model.Email;
                currentDoctor.PhoneNumber = model.PhoneNumber;

                _logger.LogDebug( "****************DOCTOR Degree abbrev.: " + model.DegreeAbbreviation + "****************" );

                // Update all common items among ApplicationUsers:
                //await UpdateProfileItems( model, currentDoctor );

                await _doctorRepository.UpdateAsync( User.Identity.Name, currentDoctor );

            }
            else if ( User.IsInRole( Roles.PATIENT ) )
            {
                var patient = await _patientRepository.ReadAsync( User.Identity.Name );
                if ( patient == null )
                    throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );

                _logger.LogDebug( "****************PATIENT****************" );

                var currentPatient = Patient.Clone( patient );
                currentPatient.FirstName = model.FirstName;
                currentPatient.LastName = model.LastName;
                currentPatient.Address1 = model.Address1;
                currentPatient.Address2 = model.Address2;
                currentPatient.City = model.City;
                currentPatient.State = model.State;
                currentPatient.Zip1 = model.Zip1;

                currentPatient.Zip2 = model.Zip2;
                currentPatient.Email = model.Email;
                currentPatient.PhoneNumber = model.PhoneNumber;
                //await UpdateProfileItems( model, currentPatient );

                //patient.CopyFrom( user );
                if ( model.DoctorUserName != null )
                {
                    currentPatient.Doctor = await _doctorRepository.ReadAsync( model.DoctorUserName );
                    //currentPatient.DrUserName = currentPatient.Doctor.UserName;
                    //currentPatient.DoctorId = currentPatient.Doctor.Id;

                } // if

                await _patientRepository.UpdateAsync( User.Identity.Name, currentPatient );

            } // if patient
            else if ( User.IsInRole( Roles.DEVELOPER ) )
            {
                var dev = new Developer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    State = model.State,
                    Zip1 = model.Zip1,
                    Zip2 = model.Zip2,
                    Title = model.Title
                };
                await _devRepo.UpdateAsync( User.Identity.Name, dev );

            }
            else
            {
                await _userRepository.UpdateAsync( user.UserName, user );
            }

            StatusMessage = "Your profile has been updated";
            return RedirectToAction( nameof( Index ) );

        } // Index


        private async Task UpdateProfileItems( IndexViewModel model, ApplicationUser user )
        {
            if ( user == null )
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );

            var email = user.Email;
            if ( model.Email != email )
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if ( !setEmailResult.Succeeded )
                    throw new ApplicationException( $"Unexpected error occurred setting email for user with ID '{user.Id}'." );

            } // if

            var phoneNumber = user.PhoneNumber;
            if ( model.PhoneNumber != phoneNumber )
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if ( !setPhoneResult.Succeeded )
                    throw new ApplicationException( $"Unexpected error occurred setting phone number for user with ID '{user.Id}'." );

            } // if

        } // UpdateProfileItems


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendVerificationEmail( IndexViewModel model )
        {
            if ( !ModelState.IsValid )
                return View( model );

            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
            var email = user.Email;
            await _emailSender.SendEmailConfirmationAsync( email, callbackUrl );

            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction( nameof( Index ) );

        } // SendVerificationEmail


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if ( !hasPassword )
                return RedirectToAction( nameof( SetPassword ) );

            var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
            return View( model );

        } // ChangePassword


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword( ChangePasswordViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                return View( model );
            }

            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if ( !changePasswordResult.Succeeded )
            {
                AddErrors( changePasswordResult );
                return View( model );
            }

            await _signInManager.SignInAsync( user, isPersistent: false );
            _logger.LogInformation( "User changed their password successfully." );
            StatusMessage = "Your password has been changed.";

            return RedirectToAction( nameof( ChangePassword ) );
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);

            if ( hasPassword )
            {
                return RedirectToAction( nameof( ChangePassword ) );
            }

            var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword( SetPasswordViewModel model )
        {
            if ( !ModelState.IsValid )
            {
                return View( model );
            }

            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if ( !addPasswordResult.Succeeded )
            {
                AddErrors( addPasswordResult );
                return View( model );
            }

            await _signInManager.SignInAsync( user, isPersistent: false );
            StatusMessage = "Your password has been set.";

            return RedirectToAction( nameof( SetPassword ) );
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var model = new ExternalLoginsViewModel { CurrentLogins = await _userManager.GetLoginsAsync(user) };
            model.OtherLogins = ( await _signInManager.GetExternalAuthenticationSchemesAsync() )
                .Where( auth => model.CurrentLogins.All( ul => auth.Name != ul.LoginProvider ) )
                .ToList();
            model.ShowRemoveButton = await _userManager.HasPasswordAsync( user ) || model.CurrentLogins.Count > 1;
            model.StatusMessage = StatusMessage;

            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkLogin( string provider )
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync( IdentityConstants.ExternalScheme );

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action(nameof(LinkLoginCallback));
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult( provider, properties );
        }

        [HttpGet]
        public async Task<IActionResult> LinkLoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var info = await _signInManager.GetExternalLoginInfoAsync(user.Id);
            if ( info == null )
            {
                throw new ApplicationException( $"Unexpected error occurred loading external login info for user with ID '{user.Id}'." );
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if ( !result.Succeeded )
            {
                throw new ApplicationException( $"Unexpected error occurred adding external login for user with ID '{user.Id}'." );
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync( IdentityConstants.ExternalScheme );

            StatusMessage = "The external login was added.";
            return RedirectToAction( nameof( ExternalLogins ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin( RemoveLoginViewModel model )
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var result = await _userManager.RemoveLoginAsync(user, model.LoginProvider, model.ProviderKey);
            if ( !result.Succeeded )
            {
                throw new ApplicationException( $"Unexpected error occurred removing external login for user with ID '{user.Id}'." );
            }

            await _signInManager.SignInAsync( user, isPersistent: false );
            StatusMessage = "The external login was removed.";
            return RedirectToAction( nameof( ExternalLogins ) );
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var model = new TwoFactorAuthenticationViewModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = user.TwoFactorEnabled,
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user),
            };

            return View( model );
        }

        [HttpGet]
        public async Task<IActionResult> Disable2faWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            if ( !user.TwoFactorEnabled )
            {
                throw new ApplicationException( $"Unexpected error occured disabling 2FA for user with ID '{user.Id}'." );
            }

            return View( nameof( Disable2fa ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable2fa()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if ( !disable2faResult.Succeeded )
            {
                throw new ApplicationException( $"Unexpected error occured disabling 2FA for user with ID '{user.Id}'." );
            }

            _logger.LogInformation( "User with ID {UserId} has disabled 2fa.", user.Id );
            return RedirectToAction( nameof( TwoFactorAuthentication ) );
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            var model = new EnableAuthenticatorViewModel();
            await LoadSharedKeyAndQrCodeUriAsync( user, model );

            return View( model );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableAuthenticator( EnableAuthenticatorViewModel model )
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            if ( !ModelState.IsValid )
            {
                await LoadSharedKeyAndQrCodeUriAsync( user, model );
                return View( model );
            }

            // Strip spaces and hypens
            var verificationCode = model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if ( !is2faTokenValid )
            {
                ModelState.AddModelError( "Code", "Verification code is invalid." );
                await LoadSharedKeyAndQrCodeUriAsync( user, model );
                return View( model );
            }

            await _userManager.SetTwoFactorEnabledAsync( user, true );
            _logger.LogInformation( "User with ID {UserId} has enabled 2FA with an authenticator app.", user.Id );
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData[ RecoveryCodesKey ] = recoveryCodes.ToArray();

            return RedirectToAction( nameof( ShowRecoveryCodes ) );
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            var recoveryCodes = (string[])TempData[RecoveryCodesKey];
            if ( recoveryCodes == null )
            {
                return RedirectToAction( nameof( TwoFactorAuthentication ) );
            }

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes };
            return View( model );
        }

        [HttpGet]
        public IActionResult ResetAuthenticatorWarning()
        {
            return View( nameof( ResetAuthenticator ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            await _userManager.SetTwoFactorEnabledAsync( user, false );
            await _userManager.ResetAuthenticatorKeyAsync( user );
            _logger.LogInformation( "User with id '{UserId}' has reset their authentication app key.", user.Id );

            return RedirectToAction( nameof( EnableAuthenticator ) );
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodesWarning()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            if ( !user.TwoFactorEnabled )
            {
                throw new ApplicationException( $"Cannot generate recovery codes for user with ID '{user.Id}' because they do not have 2FA enabled." );
            }

            return View( nameof( GenerateRecoveryCodes ) );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            var user = await _userManager.GetUserAsync(User);
            if ( user == null )
            {
                throw new ApplicationException( $"Unable to load user with ID '{_userManager.GetUserId( User )}'." );
            }

            if ( !user.TwoFactorEnabled )
            {
                throw new ApplicationException( $"Cannot generate recovery codes for user with ID '{user.Id}' as they do not have 2FA enabled." );
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            _logger.LogInformation( "User with ID {UserId} has generated new 2FA recovery codes.", user.Id );

            var model = new ShowRecoveryCodesViewModel { RecoveryCodes = recoveryCodes.ToArray() };

            return View( nameof( ShowRecoveryCodes ), model );
        }

        #region Helpers

        private void AddErrors( IdentityResult result )
        {
            foreach ( var error in result.Errors )
            {
                ModelState.AddModelError( string.Empty, error.Description );
            }
        }

        private string FormatKey( string unformattedKey )
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while ( currentPosition + 4 < unformattedKey.Length )
            {
                result.Append( unformattedKey.Substring( currentPosition, 4 ) ).Append( " " );
                currentPosition += 4;
            }
            if ( currentPosition < unformattedKey.Length )
            {
                result.Append( unformattedKey.Substring( currentPosition ) );
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri( string email, string unformattedKey )
        {
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode( "DMS" ),
                _urlEncoder.Encode( email ),
                unformattedKey );
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync( ApplicationUser user, EnableAuthenticatorViewModel model )
        {
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if ( string.IsNullOrEmpty( unformattedKey ) )
            {
                await _userManager.ResetAuthenticatorKeyAsync( user );
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync( user );
            }

            model.SharedKey = FormatKey( unformattedKey );
            model.AuthenticatorUri = GenerateQrCodeUri( user.Email, unformattedKey );
        }

        #endregion
    }
}
