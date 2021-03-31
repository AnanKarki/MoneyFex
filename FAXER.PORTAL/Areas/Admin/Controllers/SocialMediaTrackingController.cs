using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SocialMediaTrackingController : Controller
    {
        SocialMedaiTrackingServices _service = null;

        public SocialMediaTrackingController()
        {
            _service = new SocialMedaiTrackingServices();
        }
        // GET: Admin/SocialMediaTracking
        public ActionResult Index(string Services = "", int ApplicationType = 0, string TrackingPage = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<SocialMediaTrackingViewModel> vm = _service.GetTrackingPageInfo(Services, ApplicationType, TrackingPage).ToPagedList(pageNumber, pageSize);
            ViewBag.TrackingPage = TrackingPage;
            ViewBag.ApplicationType = ApplicationType;
            ViewBag.Services = Services;

            return View(vm);
        }
        public ActionResult AddTrackingPage(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (id != 0)
            {
                var model = _service.GetSocialMediaTrackings().Where(x => x.Id == id).FirstOrDefault();

                SocialMediaTrackingViewModel vm = new SocialMediaTrackingViewModel()
                {
                    Services = model.Services,
                    TrackingCode = model.TrackingCode,
                    TrackingPage = model.TrackingPage,
                    ApplicationType = model.ApplicationType
                };
                ViewBag.TrackingPage = vm.TrackingPage;
                ViewBag.Services = vm.Services;

                return View(vm);

            }

            return View();
        }

        [HttpPost]
        public ActionResult AddTrackingPage([Bind(Include = SocialMediaTrackingViewModel.BindProperty)]SocialMediaTrackingViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.TrackingPage = vm.TrackingPage;
            ViewBag.Services = vm.Services;
            if (ModelState.IsValid)
            {
                if (vm.ApplicationType == DB.ApplicationType.Select)
                {

                    ModelState.AddModelError("ApplicationType", "Select Application Type");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    _service.Add(vm);
                }
                else
                {
                    _service.Update(vm);
                }

                return RedirectToAction("Index", "SocialMediaTracking");
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _service.Remove(id);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}