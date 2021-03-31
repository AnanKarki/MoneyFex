using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FormStaffPayslipController : Controller
    {
        CommonServices CommonService = new CommonServices();
        FormStaffPayslipServices Service = new FormStaffPayslipServices();
        // GET: Admin/FormStaffPayslip
        public ActionResult Index(string CountryCode = "", string City = "", string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Operation Completed Successfully !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForStaffs(CountryCode, City);
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            FormStaffPayslipViewModel vm = new FormStaffPayslipViewModel();
            vm.Month = Month.January;
            if (!string.IsNullOrEmpty(CountryCode))
            {
                vm.Country = CountryCode;
            }
            vm.City = City;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index ([Bind(Include = FormStaffPayslipViewModel.BindProperty)]FormStaffPayslipViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank !");
                    valid = false;
                }
                if (model.StaffId == 0)
                {
                    ModelState.AddModelError("StaffId", "This field can't be blank !");
                    valid = false;
                }
                if (model.Year == 0)
                {
                    ModelState.AddModelError("Year", "This field can't be blank !");
                    valid = false;
                }
                if (model.Month == 0)
                {
                    ModelState.AddModelError("Month", "This field can't be blank !");
                    valid = false;
                }
                if (Request.Files.Count > 0)
                {
                    string fileName = "";
                    string directory = Server.MapPath("/Documents");
                    var paySlip = Request.Files[0];

                    if (paySlip != null && paySlip.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + paySlip.FileName.Split('.')[1];
                        paySlip.SaveAs(Path.Combine(directory, fileName));
                        model.PayslipURL = "/Documents/" + fileName;
                    }
                    else
                    {
                        ModelState.AddModelError("PayslipURL", "Please upload your payslip !");
                        valid = false;
                    }
                }
                else
                {
                    ModelState.AddModelError("PayslipURL", "Please upload your payslip !");
                    valid = false;
                }
                if (valid == true)
                {
                    
                    bool save = Service.savePayslipInfo(model);
                    if (save)
                    {
                        return RedirectToAction("Index", new { @message="success"});
                    }
                }
            }
            SetViewBagForCountries();
            SetViewBagForSCities(model.Country);
            SetViewBagForStaffs(model.Country, model.City);
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            
            return View(model);
        }
        public ActionResult getStaffMFSCode(int id)
        {
            string mfsCode = CommonService.getStaffMFSCode(id);
            return Json(new
            {
                StaffMFSCode = mfsCode
            }, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

        public void SetViewBagForStaffs(string Country = "", string City = "")
        {
            var staffs = Service.GetEmptyStaffList();
            if (!string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City))
            {
                City = (City.Trim()).ToLower();
                staffs = Service.GetFilteredStaffList(Country, City);
            }
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }
    }
}