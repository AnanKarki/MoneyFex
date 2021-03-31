using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewStaffPayslipController : Controller
    {
        ViewStaffPayslipServices Service = new ViewStaffPayslipServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/ViewStaffPayslip
        public ActionResult Index(string CountryCode = "", string City = "", int Year = 0, int month = 0, string message = ""
            , string StaffName = "", string Reference = "", string status = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
                message = "";
            }

            SetViewBagForCountries();

            ViewBag.StaffName = StaffName;
            ViewBag.Reference = Reference;
            ViewBag.status = status;
            SetViewBagForSCities(CountryCode);
            if (!string.IsNullOrEmpty(CountryCode))
            {
                ViewBag.Country = CountryCode;
            }
            if (!string.IsNullOrEmpty(City))
            {
                ViewBag.City = City;
            }

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            #region old
            //if (CountryCode != "" && City != "" && Year != 0 && month != 0)
            //{
            //    IPagedList<ViewStaffPayslipViewModel> vm = Service.getList(CountryCode, City, Year, month).ToPagedList(pageNumber, pageSize);

            //    return View(vm);
            //}
            //else
            //{

            //    IPagedList<ViewStaffPayslipViewModel> vmm = Service.getStaffPaySlipList().ToPagedList(pageNumber, pageSize);
            //    if (StaffId != 0)
            //    {
            //        vmm = vmm.Where(x => x.StaffId == StaffId).ToPagedList(pageNumber, pageSize);
            //    }

            //    return View(vmm);
            //}
            #endregion

            IPagedList<ViewStaffPayslipViewModel> vm = Service.getList(CountryCode, City, Year, month).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(StaffName))
            {
                StaffName = StaffName.Trim();
                vm = vm.Where(x => x.StaffName.ToLower().Contains(StaffName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Reference))
            {
                Reference = Reference.Trim();
                vm = vm.Where(x => x.StaffMFSCode.ToLower().Contains(Reference.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(status))
            {
                status = status.Trim();
                vm = vm.Where(x => x.StaffStatus.ToLower().Contains(status.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            return View(vm);


        }
        public ActionResult AddStaffPayslip(int id = 0, string country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(country);
            SetViewBagForStaffs(country, city);
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewStaffPayslipViewModel vm = new ViewStaffPayslipViewModel();
            if (id > 0)
            {
                vm = Service.GetPaySlipById(id);
                SetViewBagForStaffs(vm.StaffCountry, vm.StaffCity);

            }
            if (country != "")
            {
                vm.StaffCountry = country;

            }
            if (city != "")
            {
                vm.StaffCity = city;
            }


            return View(vm);

        }

        [HttpPost]
        public ActionResult AddStaffPayslip([Bind(Include = ViewStaffPayslipViewModel.BindProperty)]ViewStaffPayslipViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                ModelState.AddModelError("Country", "Sorry, No logged staff found !");
                SetViewBagForCountries();
                SetViewBagForCities(model.StaffCountry);
                SetViewBagForStaffs(model.StaffCountry, model.StaffCity);
                ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
                return View(model);
            }
            if (model != null)
            {
                bool valid = true;

                if (model.StaffId == 0)
                {
                    ModelState.AddModelError("StaffId", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffCountry))
                {
                    ModelState.AddModelError("Country", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffCity))
                {
                    ModelState.AddModelError("City", "This field can't be blank !");
                    valid = false;
                }
                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentUrlPhoto"];

                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["DocumentUrlPhoto"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    var extension = IdentificationDoc.FileName.Split('.')[1];
                    extension = extension.ToLower();
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];

                    if (allowedExtensions.Contains(extension))
                    {

                        try
                        {


                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {


                        }

                        model.PayslipPDF = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        ModelState.AddModelError("DocumentUrl", "File type not allowed to upload. ");
                        return View(model);
                    }

                }
                if (valid == true)
                {
                    if (model.Id > 0)
                    {
                        bool result = Service.EditPaySpil(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "Update Successfully!" });
                        }

                    }
                    else
                    {
                        bool result = Service.SavePaySpil(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "Added Successfully!" });
                        }
                    }

                }
            }
            SetViewBagForCountries();
            SetViewBagForCities(model.StaffCountry);
            SetViewBagForStaffs(model.StaffCountry, model.StaffCity);
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            return View(model);
        }

        public JsonResult DeleteStaffPayslip(int id)
        {
            if (id != 0)
            {
                bool result = Service.deletePayslip(id);
                if (result)
                {
                    return Json(new
                    {
                        Data = true,
                        Message = "Deleted Sucessfully"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                Data = false,
                Message = "Something went wrong. Please try again!"
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
        public void SetViewBagForCities(string country = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var cities = Service.GetCities();
            if (country != "")
            {
                cities = Service.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForStaffs(string country = "", string city = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var staffs = Service.GetEmptyStaffList();
            if (country != "" && city != "")
            {
                city = (city.Trim()).ToLower();

                staffs = Service.GetFilteredStaffList(country, city);
            }
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }

        public JsonResult GetStaffAccountNo(int StaffId)
        {
            var Accoutno = CommonService.getStaffMFSCode(StaffId);


            return Json(new
            {
                Data = Accoutno
            }, JsonRequestBehavior.AllowGet);
        }

    }
}