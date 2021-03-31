using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using Microsoft.SqlServer.Server;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ReceiverLimitController : Controller
    {
        // GET: Admin/ReceiverLimit

        CommonServices _CommonServices = null;
        ReceiverLimitServices _receiverLimitServices = null;

        public ReceiverLimitController()
        {
            _CommonServices = new CommonServices();
            _receiverLimitServices = new ReceiverLimitServices();
        }
        public ActionResult Index(string Country = "", int Services = 0, string city = "", string Date = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(City, "City", "City");
            ViewBag.DateRange = Date;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList <AgentTransferLimtViewModel> vm = _receiverLimitServices.getReceiverLimit(Country, Services, city, Date).ToPagedList(pageNumber,pageSize);
            return View(vm);
        }


        [HttpPost]
        public ActionResult SetReceiverLimit(AgentTransferLimtViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Receiver = _CommonServices.GetRecipients();
            ViewBag.Receivers = new SelectList(Receiver, "Id", "Name");
            if (ModelState.IsValid)
            {

                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransactionTransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Amount == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Amount");
                    return View(vm);
                }


                if (vm.Id > 0)
                {
                    _receiverLimitServices.UpdateReceiverLimit(vm);

                }
                else
                {

                    _receiverLimitServices.AddReceiverLimit(vm);

                }
                return RedirectToAction("Index");

            }
            return View(vm);
        }

        public ActionResult SetReceiverLimit(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");

            var Receiver = _CommonServices.GetRecipients();
            ViewBag.Receivers = new SelectList(Receiver, "Id", "Name");
            var vm = GetReceiverLimit(Id);

            return View(vm);
        }

        public ViewModels.AgentTransferLimtViewModel GetReceiverLimit(int Id)
        {

            if (Id > 0)
            {
                var rate = (from c in _receiverLimitServices.List().Where(x => x.Id == Id).ToList()
                            select new ViewModels.AgentTransferLimtViewModel()
                            {
                                Id = c.Id,
                                ReceiverId = c.ReceiverId,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                                Amount = c.Amount,
                                City = c.City,
                                Country = c.Country,
                                TransferMethod = c.TransferMethod,
                                Frequency = c.Frequency
                            }).FirstOrDefault();
                return rate;
            }
            else
            {
                return null;

            }


        }


        public ActionResult DeleteReceierLimit(int Id)
        {

            _receiverLimitServices.Delete(Id);
            return RedirectToAction("Index");
        }

        public ActionResult ReceiverLimitHistory(string Country = "", int Services = 0, string city = "", string Date = "",int? page=null)
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Receiver = _CommonServices.GetRecipients();
            ViewBag.Receivers = new SelectList(Receiver, "Id", "Name");
            ViewBag.DateRange = Date;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AgentTransferLimtViewModel> vm = _receiverLimitServices.GetReceiverLimitHistory(Country, Services, city, Date).ToPagedList(pageNumber,pageSize);
            return View(vm);
        }

        public JsonResult GetReceiverByCountry(string Country)
        {

            var data = _CommonServices.GetRecipients().Where(x => x.CountryCode == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }


    }
}