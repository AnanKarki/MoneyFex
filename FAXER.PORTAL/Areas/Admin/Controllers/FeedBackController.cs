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
    public class FeedBackController : Controller
    {
        FeedBackServices _services = null;
        CommonServices _CommonServices = null;
        public FeedBackController()
        {
            _services = new FeedBackServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/FeedBack
        public ActionResult Index(string Country = "", int CustomerType = 0, int Platform = 0 ,string name="" , int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            ViewBag.Name = name;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<FeedBackViewModel> vm = _services.GetFeedBacks(Country, CustomerType, Platform, name).ToPagedList(pageNumber,pageSize);
            return View(vm);
        }
        public ActionResult Delete(int id)
        {
            _services.Delete(id);
            return RedirectToAction("Index", "FeedBack");
        }
        public ActionResult AddFeedback(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");


            if (id > 0)
            {
                FeedBackViewModel vm = _services.GetFeedBack(id);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddFeedback([Bind(Include = FeedBackViewModel.BindProperty)] FeedBackViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                if (vm.Platform == 0)
                {
                    ModelState.AddModelError("Platform", "select platform");
                    return View(vm);
                }
                if (vm.CustomerType == 0)
                {
                    ModelState.AddModelError("CustomerType", "select Customer Type");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    _services.AddFeedback(vm);
                }
                else
                {
                    _services.UpdateFeedback(vm);
                }
                return RedirectToAction("Index", "FeedBack");
            }
            return View();
        }
    }
}