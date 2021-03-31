using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class LogoAssignController : Controller
    {
        LogoAssignServices _logoAssignService = null;
        CommonServices _CommonServices = null;
        public LogoAssignController()
        {
            _logoAssignService = new LogoAssignServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/LogoAssign
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            List<LogoAssignViewModel> model = _logoAssignService.GetLogoAssignedData();
            return View(model);
        }
        public ActionResult LogoAssign(string SendingCountry = "", string ReceivingCountry = "", int Services = 0, int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name", SendingCountry);
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name", ReceivingCountry);
            LogoAssignViewModel model = new LogoAssignViewModel();
            model.Details = _logoAssignService.GetLogos(ReceivingCountry, Services);
            ViewBag.Services = Services;
            if (Id != 0)
            {

                model.Master = _logoAssignService.GetLogoAssignedData().Where(x => x.Master.Id == Id).Select(x => x.Master).FirstOrDefault();
                string sendingCountryCode = Common.Common.GetCountryCodeByCountryName(model.Master.SendingCountry);
                string receivingCountryCode = Common.Common.GetCountryCodeByCountryName(model.Master.ReceivingCountry);
                model.Master.SendingCountry = sendingCountryCode;
                model.Master.ReceivingCountry = receivingCountryCode;
                ViewBag.Services =(int)model.Master.Services;
                var data = _logoAssignService.DetailsData().Data.Where(x => x.LogoAssignId == model.Master.Id).ToList();

                model.Details = _logoAssignService.GetLogos(receivingCountryCode, (int)model.Master.Services);

                model.Details = (from c in model.Details
                                 join d in data on c.LogoUploadId equals d.ServiceProvider into cd
                                 from d in cd.DefaultIfEmpty()

                                 select new LogoAssignDetailsViewModel()
                                 {
                                     Id = c.Id,
                                     ImageUrl = c.ImageUrl,
                                     IsChecked = d == null ? false : true,
                                     LogoAssignId = c.LogoAssignId,
                                     LogoUpload = c.LogoUpload,
                                     LogoUploadId = c.LogoUploadId,

                                 }).ToList();
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult LogoAssign([Bind(Include = LogoAssignViewModel.BindProperty)]LogoAssignViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.Services =(int)model.Master.Services;
            if (string.IsNullOrEmpty(model.Master.SendingCountry) == true || string.IsNullOrEmpty(model.Master.ReceivingCountry) == true)
            {
                model.Details = _logoAssignService.GetLogos(model.Master.ReceivingCountry, (int)model.Master.Services);
                return View(model);
            }
            if (model.Details != null)
            {
                if (model.Details.Where(x => x.IsChecked == true).Count() == 0)
                {
                    model.Details = _logoAssignService.GetLogos(model.Master.ReceivingCountry, (int)model.Master.Services);
                    return View(model);
                }
            }
            else
            {
                model.Details = _logoAssignService.GetLogos(model.Master.ReceivingCountry, (int)model.Master.Services);

                return View(model);
            }

            if (model.Master.Id == 0)
            {
                _logoAssignService.AddLogoAssign(model);
                return RedirectToAction("Index", "LogoAssign");
            }
            else
            {
                _logoAssignService.UpdateLogoAssign(model);
                return RedirectToAction("Index", "LogoAssign");
            }

            return View(model);
        }
        public ActionResult Delete(int id)
        {
            _logoAssignService.Remove(id);
            return RedirectToAction("Index", "LogoAssign");

        }

    }
}