using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FaxingCommissionSettingController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.FaxingCommissionSettingServices service = new Services.FaxingCommissionSettingServices();

        // GET: Admin/FaxingCommissionSetting
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var listview = service.getlist();
            return View(listview);
        }


        [HttpGet]
        public ActionResult AddFaxingContinentCommission(string Continent = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var continents = CommonService.GetContinent();
            ViewBag.Continents = new SelectList(continents, "Code", "Name", Continent);

            if (string.IsNullOrEmpty(Continent) == false)
            {
                var viewmodel = new ViewModels.FaxingCommissionSettingViewModel();
                viewmodel.Commission = continents.Where(x => x.Code == Continent).FirstOrDefault().Commission;
                return View(viewmodel);
            }
            return View();
        }





        [HttpPost]
        public ActionResult AddFaxingContinentCommission([Bind(Include = FaxingCommissionSettingViewModel.BindProperty)]FaxingCommissionSettingViewModel model)
        {
            if (model.Continent != "" && model.Commission != 0)
            {
                bool result = service.SaveCommission(model.Continent, model.Commission);
                if (result)
                {
                    return RedirectToAction("AddFaxingContinentCommission", "FaxingCommissionSetting");
                }
            }
            return RedirectToAction("AddFaxingContinentCommission", "FaxingCommissionSetting");
        }

        public ActionResult FaxingCommissionSetting()
        {
            return View();
        }

        public ActionResult DeleteCommission([Bind(Include = FaxingCommissionSettingViewModel.BindProperty)]FaxingCommissionSettingViewModel model)
        {
            if (model.Id != 0 && model.Code != "")
            {
                bool result = service.DeleteCommission(model.Id);
                if (result)
                {
                    return RedirectToAction("Index", "FaxingCommissionSetting");
                }
            }
            return RedirectToAction("Index", "FaxingCommissionSetting");

        }
    }
}