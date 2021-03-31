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
    public class TransactionAmountLimitController : Controller
    {
        TransactionAmountLimitServices _services = null;
        CommonServices _commonServices = null;
        public TransactionAmountLimitController()
        {
            _services = new TransactionAmountLimitServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/TransactionAmountLimit
        public ActionResult Index(int? page = null)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<TransactionAmountLimitViewModel> vm = _services.GetTransactionAmountLimitList(DB.Module.Faxer).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult SetTransactionAmountLimit(int id = 0, string sendingCurrency = "", string receivingCurrency = "", string sendingCountry = "", string receivingCountry = "", int senderId = 0)
        {
            TransactionAmountLimitViewModel vm = new TransactionAmountLimitViewModel();
            SetCurrencyViewBag();
            SetSendingCountryViewBag(sendingCurrency);
            SetReceivingCountryViewBag(receivingCurrency);
            SetSenderViewBag(sendingCountry);
            if (id > 0)
            {
                vm = _services.GetTransactionAmountLimitList(DB.Module.Faxer).SingleOrDefault(x => x.Id == id);
            }
            if (!string.IsNullOrEmpty(sendingCountry) && !string.IsNullOrEmpty(receivingCountry) && senderId >= 0)
            {
                var transactionAmountLimit = _services.GetTransactionAmountLimitList(DB.Module.Faxer).Where(x => x.SendingCountry == sendingCountry &&
                x.ReceivingCountry == receivingCountry && x.SenderId == senderId).FirstOrDefault();
                if (transactionAmountLimit != null)
                {
                    vm = transactionAmountLimit;
                }
                else
                {
                    vm.ReceivingCurrency = receivingCurrency;
                    vm.SendingCurrency = sendingCurrency;
                    vm.SendingCountry = sendingCountry;
                    vm.ReceivingCountry = receivingCountry;
                    vm.SenderId = senderId;
                    vm.SenderAccountNo = _commonServices.GetSenderAccountNoBySenderId(senderId);
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetTransactionAmountLimit([Bind(Include = TransactionAmountLimitViewModel.BindProperty)] TransactionAmountLimitViewModel vm)
        {
            SetCurrencyViewBag();
            SetSendingCountryViewBag(vm.SendingCurrency);
            SetReceivingCountryViewBag(vm.ReceivingCurrency);
            SetSenderViewBag(vm.SendingCountry);
            if (ModelState.IsValid)
            {
                vm.ForModule = DB.Module.Faxer;
                if (vm.Id > 0)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);
                }
                return RedirectToAction("Index", "TransactionAmountLimit");

            }
            return View(vm);

        }
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            if (id > 0)
            {
                _services.Delete(id);
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

        public ActionResult StaffTransactionApprovalLimit(int? page = null)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<TransactionAmountLimitViewModel> vm = _services.GetTransactionAmountLimitList(DB.Module.Staff).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        public ActionResult SetStaffTransactionAmountLimit(int id = 0, string sendingCurrency = "", string receivingCurrency = "", string sendingCountry = "",
            string receivingCountry = "", int staffId = 0)
        {
            TransactionAmountLimitViewModel vm = new TransactionAmountLimitViewModel();
            SetCurrencyViewBag();
            SetSendingCountryViewBag(sendingCurrency);
            SetReceivingCountryViewBag(receivingCurrency);
            SetStaffViewBag(sendingCountry);
            if (id > 0)
            {
                vm = _services.GetTransactionAmountLimitList(DB.Module.Staff).SingleOrDefault(x => x.Id == id);

            }
            if (!string.IsNullOrEmpty(sendingCountry) && !string.IsNullOrEmpty(receivingCountry) && staffId >= 0)
            {
                var transactionAmountLimit = _services.GetTransactionAmountLimitList(DB.Module.Staff).Where(x => x.SendingCountry == sendingCountry &&
              x.ReceivingCountry == receivingCountry && x.StaffId == staffId).FirstOrDefault();
                if (transactionAmountLimit != null)
                {
                    vm = transactionAmountLimit;
                }
                else
                {
                    vm.ReceivingCurrency = receivingCurrency;
                    vm.SendingCurrency = sendingCurrency;
                    vm.SendingCountry = sendingCountry;
                    vm.ReceivingCountry = receivingCountry;
                    vm.StaffId = staffId;
                    vm.StaffAccountNo = _commonServices.getStaffMFSCode(staffId);
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetStaffTransactionAmountLimit([Bind(Include = TransactionAmountLimitViewModel.BindProperty)] TransactionAmountLimitViewModel vm)
        {
            SetCurrencyViewBag();
            SetSendingCountryViewBag(vm.SendingCountry);
            SetReceivingCountryViewBag(vm.ReceivingCountry);
            SetStaffViewBag(vm.SendingCountry);
            if (ModelState.IsValid)
            {
                vm.ForModule = DB.Module.Staff;
                if (vm.Id > 0)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);
                }
                return RedirectToAction("StaffTransactionApprovalLimit", "TransactionAmountLimit");

            }
            return View(vm);

        }
        [HttpGet]
        public JsonResult DeleteStaffTransactionApprovalLimit(int id = 0)
        {
            if (id > 0)
            {
                _services.Delete(id);
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


        public void SetCurrencyViewBag()
        {
            var Currencies = _commonServices.GetCountryCurrencies();
            ViewBag.Currencies = new SelectList(Currencies, "Code", "Name");
        }
        public void SetSenderViewBag(string sendingCountry = "")
        {
            var sender = _commonServices.GetSenderList();
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                sender = sender.Where(x => x.Country == sendingCountry).ToList();
            }
            sender.Add(new SenderListDropDown()
            {
                senderId = 0,
                senderName = "All"
            });
            ViewBag.Senders = new SelectList(sender, "senderId", "senderName");
        }
        public void SetSendingCountryViewBag(string sendingCurrency = "")
        {
            var Country = _commonServices.GetCountries();
            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                Country = Country.Where(x => x.CountryCurrency == sendingCurrency).ToList();
            }
            Country.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.SendingCountries = new SelectList(Country, "Code", "Name");
        }
        public void SetReceivingCountryViewBag(string receivingCurrency = "")
        {
            var Country = _commonServices.GetCountries();
            if (!string.IsNullOrEmpty(receivingCurrency))
            {
                Country = Country.Where(x => x.CountryCurrency == receivingCurrency).ToList();
            }
            Country.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.ReceivingCountries = new SelectList(Country, "Code", "Name");
        }
        public void SetStaffViewBag(string sendingCountry = "")
        {
            var staff = _commonServices.getStaffList();
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                staff = staff.Where(x => x.CountryCode == sendingCountry).ToList();
            }
            staff.Add(new DropDownViewModel()
            {
                Id = 0,
                Name = "All"
            });
            ViewBag.Staffs = new SelectList(staff, "Id", "Name");
        }

        public JsonResult GetCountryByCurrency(string Currency = "")
        {
            var data = _commonServices.GetCountries().Where(x => x.CountryCurrency == Currency).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetsenderByCountry(string Country = "")
        {
            var data = _commonServices.GetSenderList().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStaffByCountry(string Country = "")
        {
            var data = _commonServices.getStaffList().Where(x => x.CountryCode == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountNumber(int SenderId = 0)
        {
            string Accountnumber = _commonServices.GetSenderAccountNoBySenderId(SenderId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffAccountNumber(int StaffId = 0)
        {
            string Accountnumber = _commonServices.getStaffMFSCode(StaffId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }


    }
}