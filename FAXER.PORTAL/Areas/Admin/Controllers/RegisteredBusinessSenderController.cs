using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RegisteredBusinessSenderController : Controller
    {
        RegisteredBusinessSenderServices _services = null;
        CommonServices _CommonServices = null;
        public RegisteredBusinessSenderController()
        {
            _services = new RegisteredBusinessSenderServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/RegisteredBusinessSender
        public ActionResult Index(string Country = "", string City = "", string businessName = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);

            var Cities = _CommonServices.GetCitiesByName(Country);
            ViewBag.Cities = new SelectList(Cities, "City", "City", City);

            ViewBag.BusinessName = businessName;
            List<SenderBusinessprofileViewModel> vm = _services.GetRegisteredBusinessSender(Country, City, businessName);


            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteFaxerInformation(int id)
        {
            if (id != 0)
            {
                ViewRegisteredFaxersServices faxer = new ViewRegisteredFaxersServices();
                var result = faxer.DeleteFaxerInformation(id);
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

        public ActionResult BusinessDashboard(int SenderId = 0)
        {
            SenderBusinessFullDetailViewModel vm = _services.GetSenderFullDetails(SenderId);


            return View(vm);
        }
        public ActionResult TransactionStatementOfSenderBusiness(TransactionServiceType transactionServiceType = TransactionServiceType.All,
            int year = 0, int month = 0, int day = 0,
            int senderId = 0, string Identifier = "", int? page = null, int pageSize = 10, int CurrentpageCount = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.Month = month;
            ViewBag.day = day;
            ViewBag.senderId = senderId;
            ViewBag.Identifier = Identifier;
            int pageNumber = (page ?? 1);
            SenderBusinessTransactionStatementWithSenderDetails vm = _services.TransactionStatementOfBusinessSender(transactionServiceType, year, month, day, senderId, pageSize, pageNumber, Identifier);

            ViewBag.NumberOfPage = 0;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.ButtonCount = 0;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = page ?? 1;
            if (vm.SenderBusinessTransactionStatement.Count != 0)
            {
                var TotalCount = vm.SenderBusinessTransactionStatement.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, pageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                ViewBag.ButtonCount = NumberOfPage > 10 ? 10 : NumberOfPage;
            }

            return View(vm);
        }
    }
}