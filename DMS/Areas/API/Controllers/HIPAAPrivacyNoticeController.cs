using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Services.Interfaces;
using DMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Areas.API.Controllers
{
    [Area( "API" )]
    public class HIPAAPrivacyNoticeController : Controller
    {
        private IHIPAANoticeRepository _hIPAARepository;

        public HIPAAPrivacyNoticeController( IHIPAANoticeRepository hIPAARepository )
        {
            _hIPAARepository = hIPAARepository;

        } // constructor


        // POST: HIPAAPrivacyNotice/ReadNewest
        [HttpPost]
        public IActionResult ReadNewestNotice()
        {
            try
            {
                HIPAAPrivacyNotice notice = _hIPAARepository.ReadNewest();
                return new JsonResult( notice );
                //new
                //{
                //    version = notice.Version
                //} );
            }
            catch
            {

            }

            return new JsonResult( new { errorCode = ErrorCode.UNKNOWN } );
        }


        // POST: HIPAAPrivacyNotice/ReadNewest
        [HttpPost]
        public IActionResult ReadNewestVersion()
        {
            try
            {
                return new JsonResult( new { version = _hIPAARepository.ReadNewestVersion() } );
            }
            catch
            {

            }

            return new JsonResult( new { errorCode = ErrorCode.UNKNOWN } );
        }

    } // class

} // namespace