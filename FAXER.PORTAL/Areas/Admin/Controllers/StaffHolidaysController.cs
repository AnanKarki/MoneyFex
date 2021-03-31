using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class StaffHolidaysController : Controller
    {
        StaffHolidaysServices Service = new StaffHolidaysServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/StaffHolidays
        public ActionResult Index(string CountryCode = "", string City = "", string message = "", int Year = 0, int Month = 0, int StaffId = 0,
            string staffName = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Holiday added successfully !";
                message = "";
            }
            else if (message == "updatesuccess")
            {
                ViewBag.Message = "Holiday Update Successfully !";
                message = "";
            }
            ViewBag.staffName = staffName;

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<StaffHolidaysViewModel> vm = Service.GetHolidaysList(CountryCode, City, Year, Month, StaffId, staffName).ToPagedList(pageNumber, pageSize);

            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }

            return View(vm);
        }
        [HttpGet]
        public ActionResult AddStaffHolidays(int Id = 0, string country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(country);
            SetViewBagForStaffs(country, city);
            AddStaffHolidaysViewModel vm = new AddStaffHolidaysViewModel();
            if (Id > 0)
            {
                vm = Service.GetEditHolidayList(Id);
                SetViewBagForStaffs(vm.Country, vm.City);
            }
            return View(vm);

        }

        [HttpPost]
        public ActionResult AddStaffHolidays([Bind(Include = AddStaffHolidaysViewModel.BindProperty)]AddStaffHolidaysViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                ModelState.AddModelError("Country", "Sorry, No logged staff found !");
                SetViewBagForCountries();
                SetViewBagForCities(model.Country);
                SetViewBagForStaffs(model.Country, model.City);
                return View(model);
            }

            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "Select Country");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "Select City");
                    valid = false;
                }
                else if (model.StaffId == 0)
                {
                    ModelState.AddModelError("StaffId", "Selct Staff");
                    valid = false;
                }
                else if (model.StartDate == default(DateTime))
                {
                    ModelState.AddModelError("StartDate", "Enter Start Date");
                    valid = false;
                }
                else if (model.FinishDate == default(DateTime))
                {
                    ModelState.AddModelError("FinishDate", "Enter Finish Date");
                    valid = false;
                }
                if (valid == false)
                {
                    SetViewBagForCountries();
                    SetViewBagForCities(model.Country);
                    SetViewBagForStaffs(model.Country, model.City);
                    return View(model);
                }


                var entitlement = Service.GetStaffHolidaysEntitlement(model.StaffId);
                if (entitlement == StaffHolidaysEntiltlement.No)
                {
                    ModelState.AddModelError("StaffId", "Sorry, this staff is not entitled for holidays !");
                    SetViewBagForCountries();
                    SetViewBagForCities(model.Country);
                    SetViewBagForStaffs(model.Country, model.City);
                    return View(model);
                }

                var HolidaysStartDate = Convert.ToDateTime(model.StartDate);
                var HolidaysEndDate = Convert.ToDateTime(model.FinishDate);

                var diff = HolidaysEndDate - HolidaysStartDate;
                var diffCount = diff.Days + 1;
                int regularHolidaysCount = 0;

                var publicHolidays = Service.GetPublicHolidays(model.StaffId);

                for (int i = 0; i < diffCount; i++)
                {

                    var regularholidays = HolidaysStartDate.AddDays(i).DayOfWeek;
                    if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                    {

                        regularHolidaysCount = regularHolidaysCount + 1;
                    }


                    for (int j = 0; j < publicHolidays.Count; j++)
                    {
                        var StartToEndDate = HolidaysStartDate.AddDays(i).Date;
                        if (!(StartToEndDate.DayOfWeek == DayOfWeek.Saturday || StartToEndDate.DayOfWeek == DayOfWeek.Sunday))
                        {
                            if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                            {

                                regularHolidaysCount = regularHolidaysCount + 1;
                            }
                        }
                    }

                }
                int totalNumberofHolidays = diffCount - regularHolidaysCount;

                StaffHolidays data = new StaffHolidays()
                {
                    StaffInformationId = model.StaffId,
                    HolidaysStartDate = HolidaysStartDate,
                    HolidaysEndDate = HolidaysEndDate,
                    TotalNumberOfHolidaysRequeste = totalNumberofHolidays,
                    HolidaysRequestStatus = HollidayRequestStatus.Approved,
                    ModifiedBy = Common.StaffSession.LoggedStaff.StaffId,
                    HolidaysTaken = model.NoTaken,
                    HoidaysLeft = model.NoLeft,
                    HolidaysEntitled = model.NoOfDaysEntitled,


                };

                var addresult = false;
                if (valid == true)
                {
                    if (model.Id > 0)
                    {
                        bool result = Service.EditHolidays(model);
                        if (result)
                        {


                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        addresult = Service.SaveHolidays(data);
                    }


                }

                if (addresult)
                {
                    MailCommon mail = new MailCommon();
                    try
                    {
                        string email = Service.getEmail(data.StaffInformationId);
                        string msg = "Your holiday has been approved from " + data.HolidaysStartDate + " to " + data.HolidaysEndDate + " !";
                        //mail.SendMail(email, "Holiday Request", msg);
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                    return RedirectToAction("Index", new { @message = "success" });
                }
            }
            SetViewBagForCountries();
            SetViewBagForCities("");
            SetViewBagForStaffs("", "");
            return View(model);
        }

        public JsonResult NoOfDaysCalculation(int StaffId, string StartDate, string FinishDate)
        {
            var HolidaysStartDate = Convert.ToDateTime(StartDate);
            var HolidaysEndDate = Convert.ToDateTime(FinishDate);

            var isValidDate = Service.ValidateDate(StaffId, HolidaysStartDate, HolidaysEndDate);
            if (isValidDate == false)
            {
                return Json(new
                {
                    AlreadyTaken = false
                }, JsonRequestBehavior.AllowGet);
            }
            var diff = HolidaysEndDate - HolidaysStartDate;
            var diffCount = diff.Days + 1;
            int regularHolidaysCount = 0;

            var publicHolidays = Service.GetPublicHolidays(StaffId);

            for (int i = 0; i < diffCount; i++)
            {

                var regularholidays = HolidaysStartDate.AddDays(i).DayOfWeek;
                if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                {

                    regularHolidaysCount = regularHolidaysCount + 1;
                }


                for (int j = 0; j < publicHolidays.Count; j++)
                {

                    var StartToEndDate = HolidaysStartDate.AddDays(i).Date;

                    if (!(StartToEndDate.DayOfWeek == DayOfWeek.Saturday || StartToEndDate.DayOfWeek == DayOfWeek.Sunday))
                    {
                        if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                        {

                            regularHolidaysCount = regularHolidaysCount + 1;
                        }
                    }
                }

            }


            int totalNumberofHolidays = diffCount - regularHolidaysCount;
            int NoOfHolidays = Service.SumHolidaysLeft(StaffId);
            //bool HolidayAlreadyTaken = Service.HolidayAlreadyTaken(StaffId, HolidaysStartDate);
            //if (HolidayAlreadyTaken == false)
            //{
            //    return Json(new
            //    {
            //        AlreadyTaken = false
            //    }, JsonRequestBehavior.AllowGet);
            //}

            return Json(new
            {
                NoOfDays = totalNumberofHolidays,
                NoLeft = NoOfHolidays - totalNumberofHolidays,
                NoTaken = totalNumberofHolidays
            }, JsonRequestBehavior.AllowGet);
        }




        [HttpGet]
        public ActionResult EditHolidays(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.GetEditHolidayList(id);
            if (vm == null)
            {
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditHolidays([Bind(Include = AddStaffHolidaysViewModel.BindProperty)]AddStaffHolidaysViewModel model)
        {
            if (StaffSession.LoggedStaff == null)
            {
                ModelState.AddModelError("Country", "You must be logged in as staff first ! ");
                return View(model);
            }
            if (model != null)
            {
                bool valid = true;
                if (model.StartDate == default(DateTime))
                {
                    ModelState.AddModelError("StartDate", "This field can't be blank !");
                    valid = false;
                }
                else if (model.FinishDate == default(DateTime))
                {
                    ModelState.AddModelError("FinishDate", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)

                {
                    bool result = Service.EditHolidays(model);
                    if (result)
                    {


                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }


        public JsonResult DeleteHolidays(int id)
        {
            if (id != 0)
            {
                bool result = Service.DeleteStaffHolidays(id);
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

        public void SetViewBagForCities(string country = "")
        {
            var cities = Service.GetCities();
            if (country != "")
            {
                cities = Service.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForStaffs(string country = "", string city = "")
        {
            var staffs = Service.GetEmptyStaffList();
            if (country != "" && city != "")
            {
                city = (city.Trim()).ToLower();

                staffs = Service.GetFilteredStaffList(country, city);
            }
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

    }


}