using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class CashWithdrawalCodeFromStaffOrAgentController : Controller
    {
        GenerateCashWithdrawalCodeServices _generateCashWithdrawalCodeServices = null;
        CommonServices CommonService = null;
        public CashWithdrawalCodeFromStaffOrAgentController()
        {

            _generateCashWithdrawalCodeServices = new GenerateCashWithdrawalCodeServices();
            CommonService = new CommonServices();
        }
        // GET: Admin/CashWithdrawalCodeFromStaffOrAgent
        public ActionResult Index(string CountryCode = "", string City = "", string Date = "", string AgentName = "",
            string AccountNo = "", string WithdrawalCode = "", string WithdrawalStaff = "", string status = "", int? page = null)
        {

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            int pageSize = 10;  
            int pageNumber = (page ?? 1);
            ViewBag.AgentName = AgentName;
            ViewBag.AccountNo = AccountNo;
            ViewBag.WithdrawalCode = WithdrawalCode;
            ViewBag.WithdrawalStaff = WithdrawalStaff;
            ViewBag.status = status;
            IPagedList<GenerateCashWithdrawalCodeVM> data = _generateCashWithdrawalCodeServices.GetWithdrawalCodeInformation().ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(CountryCode))
            {

                data = data.Where(x => x.CountryCode == CountryCode).ToPagedList(pageNumber, pageSize);
            }
            else if (!string.IsNullOrEmpty(City))
            {

                data = data.Where(x => x.CountryCode == CountryCode && x.City == City).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                data = data.Where(x => x.Date >= FromDate && x.Date <= ToDate).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(AgentName))
            {
                AgentName = AgentName.Trim();
                data = data.Where(x => x.AgentName.ToLower().Contains(AgentName.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                data = data.Where(x => x.AgentCode.ToLower().Contains(AccountNo.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(WithdrawalCode))
            {
                WithdrawalCode = WithdrawalCode.Trim();
                data = data.Where(x => x.WithdrawalCode.ToLower().Contains(WithdrawalCode.ToLower())).ToPagedList(pageNumber, pageSize);
            } if (!string.IsNullOrEmpty(WithdrawalStaff))
            {
                WithdrawalStaff = WithdrawalStaff.Trim();
                data = data.Where(x => x.StaffName.ToLower().Contains(WithdrawalStaff.ToLower())).ToPagedList(pageNumber, pageSize);
            }if (!string.IsNullOrEmpty(status))
            {
                status = status.Trim();
                data = data.Where(x => x.StatusName.ToLower().Contains(status.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            return View(data);
        }

        [HttpGet]
        public ActionResult GenerateWithdrawalCode(string CountryCode = "", string City = "")
        {
            AdminResult adminResult = new AdminResult();

            GenerateCashWithdrawalCodeVM vm = new GenerateCashWithdrawalCodeVM();
            vm.CountryCode = CountryCode;
            vm.City = City;
            SetViewBagForCountries();
            SetViewBagForCities(CountryCode);
            SetViewBagForStaffs(CountryCode, City);
            SetViewBagForAgents(CountryCode, City);
            ViewBag.AdminResult = adminResult;
            return View(vm);
        }

        [HttpPost]
        public ActionResult GenerateWithdrawalCode([Bind(Include = GenerateCashWithdrawalCodeVM.BindProperty)]GenerateCashWithdrawalCodeVM vm)
        {


            AdminResult adminResult = new AdminResult();
            SetViewBagForCountries();
            SetViewBagForCities(vm.CountryCode);
            SetViewBagForStaffs(vm.CountryCode, vm.City);
            SetViewBagForAgents(vm.CountryCode, vm.City);

            if (ModelState.IsValid)
            {

                var result = _generateCashWithdrawalCodeServices.Add(vm);
                adminResult.Message = "Withdrawal code generated successfully";
                adminResult.Status = AdminResultStatus.OK;

            }


            ViewBag.AdminResult = adminResult;

            return RedirectToAction("Index");

        }


        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        public void SetViewBagForCities(string Country = "")
        {
            ViewAlertsServices Service = new ViewAlertsServices();

            var cities = Service.GetCities();
            if (!(string.IsNullOrEmpty(Country)))
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }


        public void SetViewBagForStaffs(string country = "", string city = "")
        {
            if (city != null)
            {
                city = city.Trim();
                city = city.ToLower();
            }

            var staffs = _generateCashWithdrawalCodeServices.GetStaffDropdownList(country, city);

            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }
        public void SetViewBagForAgents(string country = "", string city = "")
        {
            if (city != null)
            {
                city = city.Trim();
                city = city.ToLower();
            }

            var staffs = _generateCashWithdrawalCodeServices.GetAgentDropdownList(country, city);

            ViewBag.Agents = new SelectList(staffs, "AgentId", "AgentName");

        }

        public JsonResult GetAgentCode(int AgentId)
        {

            var AgentCode = _generateCashWithdrawalCodeServices.GetAgentInformation(AgentId).AccountNo;

            return Json(new
            {
                AgentCode = AgentCode

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetStaffCode(int StaffId)
        {


            var staffCode = _generateCashWithdrawalCodeServices.GetStaffInformation(StaffId).StaffMFSCode;
            return Json(new
            {
                StaffCode = staffCode
            }, JsonRequestBehavior.AllowGet);

        }



    }
}