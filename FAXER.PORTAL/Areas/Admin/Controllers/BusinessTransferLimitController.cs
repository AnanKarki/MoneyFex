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
    public class BusinessTransferLimitController : Controller
    {
        CommonServices _CommonServices = null;
        BusinessTransferLimitServices _services = null;
        public BusinessTransferLimitController()
        {
            _CommonServices = new CommonServices();
            _services = new BusinessTransferLimitServices();
        }
        // GET: Admin/BusinessTransferLimit
        public ActionResult Index(string Country = "", string City = "", int TransferService = 0, int SenderId = 0, bool IsBusiness = true, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City", City);

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name", SenderId);

            var sender = _services.GetSender();
            ViewBag.Senders = new SelectList(sender, "Id", "Name", SenderId);

            ViewBag.IsBusiness = IsBusiness;

            ViewBag.TransferMethod = TransferService;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<BusinessLimitViewModel> vm = _services.GetBusinessTranferLimit(Country, City, TransferService, SenderId, IsBusiness).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteLimit(int id)
        {
            if (id != 0)
            {
                _services.DeleteLimit(id);
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
        public ActionResult SetNewLimit(int id = 0, string country = "", string City = "", int SenderId = 0, int service = 0, bool IsBusiness = true)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");

            var sender = _services.GetSender();
            ViewBag.Senders = new SelectList(sender, "Id", "Name", SenderId);

            BusinessLimitViewModel vm = new BusinessLimitViewModel();
            vm.SenderId = SenderId;
            vm.AccountNumber = _CommonServices.GetSenderAccountNoBySenderId(SenderId);
            vm.TransferMethod = (DB.TransactionTransferMethod)service;
            ViewBag.IsBusiness = IsBusiness;
            vm.IsBusiness = IsBusiness;
            if (id != 0)
            {
                vm = _services.GetInfoOfLimit(id, IsBusiness);
                vm.IsBusiness = IsBusiness;
                return View(vm);
            }
            if (!string.IsNullOrEmpty(country) && SenderId != 0 && service != 0)
            {
                BusinessLimitViewModel model = _services.GetBusinessTranferLimit(country, City, service, SenderId, IsBusiness).FirstOrDefault();
                if (model != null)
                {
                    vm.IsBusiness = IsBusiness;
                    return View(model);
                }


            }

            return View(vm);
        }
        [HttpPost]
        public ActionResult SetNewLimit([Bind(Include = BusinessLimitViewModel.BindProperty)]BusinessLimitViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");

            var sender = _services.GetSender();
            ViewBag.Senders = new SelectList(sender, "Id", "Name");
            ViewBag.IsBusiness = vm.IsBusiness;
            if (ModelState.IsValid)
            {
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Method");
                    return View(vm);
                }
                if (vm.Frequency == 0)
                {
                    ModelState.AddModelError("Frequency", "Select Frequency");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    _services.AddLimit(vm);
                    _services.AddLimitHistory(vm);

                }
                else
                {
                    _services.UpdateLimit(vm);
                    _services.AddLimitHistory(vm);

                }

                return RedirectToAction("Index", "BusinessTransferLimit", new { @IsBusiness = vm.IsBusiness });
            }
            return View(vm);
        }

        public ActionResult BusinessTransferLimitHisotry(string Country = "", string City = "", int TransferService = 0, string DateRange = "", int senderId = 0, bool IsBusiness = true, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");

            //ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            //ViewBag.Month = month;
            ViewBag.DateRange = DateRange;

            ViewBag.IsBusiness = IsBusiness;
            ViewBag.TransferMethod = TransferService;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<BusinessLimitViewModel> vm = _services.GetBusinessTranferLimitHistory(Country, City, TransferService, DateRange, senderId, IsBusiness).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }


        public JsonResult GetsenderByCountry(string Country = "", string City = "", bool IsBusiness = true)
        {
            var data = new List<SenderListDropDown>();
            if (IsBusiness)
            {
                data = _CommonServices.GetBusinessSenderList().Where(x => x.Country == Country).ToList();
            }
            else
            {
                data = _CommonServices.GetSenderList().Where(x => x.Country == Country).ToList();

            }
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetAccountNumber(int SenderId = 0)
        {
            string Accountnumber = _CommonServices.GetSenderAccountNoBySenderId(SenderId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPrevioustLimit(string country = "", string City = "", int SenderId = 0, int service = 0, bool IsBusiness = true, int Frequency = 0)
        {
            var FrequencyAmount = "";
            int Id = 0;
            var transferLimit = _services.GetBusinessTranferLimit(country, City, service, SenderId, IsBusiness, Frequency).FirstOrDefault();
            if (transferLimit != null)
            {
                FrequencyAmount = transferLimit.FrequencyAmount;
                Id = transferLimit.Id;
            }
            return Json(new
            {
                FrequencyAmount = FrequencyAmount,
                Id = Id
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSenders(SelectSearchParam param)
        {
            var senderInformations = _CommonServices.GetSenderList().ToList();
            if (param.country != "All" && !string.IsNullOrEmpty(param.country))
            {
                senderInformations = senderInformations.Where(x => x.Country == param.country.Trim()).ToList();
            }
            if (param.query == null) { param.query = ""; }
            var senders = (from c in senderInformations.Where(x => x.senderName.ToLower().Contains(param.query.ToLower()))
                           select new SelectDropDownVm()
                           {
                               Id = c.senderId,
                               text = c.senderName
                           }).ToList();

            return Json(new
            {
                senders
            }, JsonRequestBehavior.AllowGet);
        }

    }
}