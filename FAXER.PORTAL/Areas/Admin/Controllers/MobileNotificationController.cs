using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class MobileNotificationController : Controller
    {
        CommonServices _commonServices = null;
        MoblieNotificationManager _moblieNotificationServices = null;
        List<SenderListDropDown> _senderDropDowns;
        public MobileNotificationController()
        {
            _commonServices = new CommonServices();
            _moblieNotificationServices = new MoblieNotificationManager();
            _senderDropDowns = new List<SenderListDropDown>();

        }
        // GET: Admin/MobileNotification
        public ActionResult Index(string sendingCurrency = "", string receivingCurrecy = "", int notificationType = 0, string date = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            SetCurrenciesViewBag();
            IPagedList<MobileNotifiactionViewModel> vm = _moblieNotificationServices.GetAllMobileNotificationList(sendingCurrency, receivingCurrecy, notificationType, date).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult MobileNotification(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            MobileNotifiactionViewModel vm = new MobileNotifiactionViewModel();
            if (id > 0)
            {
                vm = _moblieNotificationServices.GetAllMobileNotificationList().SingleOrDefault(x => x.Id.Equals(id));
                //vm.SendingCountry.Trim();
                //vm.ReceivingCountry.Trim();
                //ViewBag.SelectedSendingCurrency = vm.SendingCurrency;
                //ViewBag.SelectedReceivingCurrency = vm.ReceivingCurrency;
                //ViewBag.SelectedSendingCountry = vm.SendingCountry;
                //ViewBag.SelectedReceivingCountry = vm.ReceivingCountry;

            }
            SetCurrenciesViewBag(vm.SendingCountry, vm.ReceivingCountry);
            SetCountriesViewBag(vm.SendingCurrency, vm.ReceivingCurrency);
            SetSenderViewBag();
            return View(vm);
        }
        [HttpPost]
        public ActionResult MobileNotification([Bind(Include = MobileNotifiactionViewModel.BindProperty)] MobileNotifiactionViewModel vm)
        {
            SetCurrenciesViewBag(vm.SendingCountry, vm.ReceivingCountry);
            SetCountriesViewBag(vm.SendingCurrency, vm.ReceivingCurrency);
            SetSenderViewBag();
            if (ModelState.IsValid)
            {
                if (vm.NotificationType == DB.NotificationType.Select)
                {
                    ModelState.AddModelError("NotificationType", "Select Type");
                    return View(vm);
                }
                if (vm.SendingNotificationMethod == DB.SendingNotificationMethod.Select)
                {
                    ModelState.AddModelError("SendingNotificationMethod", "Select Sending Method");
                    return View(vm);
                }
                if (vm.NotificationType != DB.NotificationType.General)
                {
                    if (string.IsNullOrEmpty(vm.ReceivingCurrency))
                    {
                        ModelState.AddModelError("ReceivingCurrency", "Select Currency");
                        return View(vm);
                    }
                }
                if (vm.Id == 0)
                {
                    DB.MobileNotification mobileNotification = new DB.MobileNotification()
                    {
                        NotificationHeading = vm.NotificationHeading,
                        NotificationType = vm.NotificationType,
                        ReceivingCountry = vm.ReceivingCountry,
                        ReceivingCurrency = vm.ReceivingCurrency,
                        SenderId = vm.SenderId,
                        SendingCountry = vm.SendingCountry,
                        SendingCurrency = vm.SendingCurrency,
                        FullNotification = vm.FullNotification,
                        CreatedDate = DateTime.Now,
                        CreatedBy = StaffSession.LoggedStaff.StaffId,
                        SendingNotificationMethod = vm.SendingNotificationMethod,
                        CustomerTpyeAccToTime = vm.CustomerTpyeAccToTime
                    };
                    _moblieNotificationServices.AddMobileNotification(mobileNotification);
                }
                else
                {
                    var mobileNotification = _moblieNotificationServices.MobileNotificationById(vm.Id);
                    mobileNotification.NotificationHeading = vm.NotificationHeading;
                    mobileNotification.NotificationType = vm.NotificationType;
                    mobileNotification.ReceivingCountry = vm.ReceivingCountry;
                    mobileNotification.ReceivingCurrency = vm.ReceivingCurrency;
                    mobileNotification.SenderId = vm.SenderId;

                    mobileNotification.SendingCountry = vm.SendingCountry;
                    mobileNotification.SendingCurrency = vm.SendingCurrency;
                    mobileNotification.FullNotification = vm.FullNotification;
                    mobileNotification.CreatedDate = DateTime.Now;
                    mobileNotification.CreatedBy = StaffSession.LoggedStaff.StaffId;
                    mobileNotification.SendingNotificationMethod = vm.SendingNotificationMethod;
                    mobileNotification.CustomerTpyeAccToTime = vm.CustomerTpyeAccToTime;
                    _moblieNotificationServices.UpdateMobileNotification(mobileNotification);
                }
                return RedirectToAction("Index", "MobileNotification");

            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteMobileNotification(int id)
        {
            if (id > 0)
            {
                var mobileNotification = _moblieNotificationServices.MobileNotificationById(id);
                _moblieNotificationServices.DeleteMobileNotification(mobileNotification);
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

        [HttpGet]
        public JsonResult SendNotificationTosender(int id = 0)
        {
            var mobileNotification = _moblieNotificationServices.MobileNotificationById(id);
            _moblieNotificationServices.SendNotificationToSender(mobileNotification);
            return Json(new
            {
                Data = true,
            }, JsonRequestBehavior.AllowGet);
        }

        public void SetCurrenciesViewBag(string sendingCountry = "", string receivingCountry = "")
        {
            var sendingCurrencies = _commonServices.GetCountryCurrencies();
            var receivingCurrencies = _commonServices.GetCountryCurrencies();
            ViewBag.SendingCurrency = new SelectList(sendingCurrencies, "Code", "Name");
            ViewBag.ReceivingCurrency = new SelectList(receivingCurrencies, "Code", "Name");
        }
        public void SetCountriesViewBag(string sendingCurrency = "", string receivingCurrency = "")
        {

            var sendingCountries = new List<Services.DropDownViewModel>();
            sendingCountries.Add(new Services.DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            var receivingCountries = new List<Services.DropDownViewModel>();
            receivingCountries.Add(new Services.DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });

            sendingCountries.AddRange(GetCountriesOfCurrency(sendingCurrency));
            receivingCountries.AddRange(GetCountriesOfCurrency(sendingCurrency));
            ViewBag.SendingCountry = new SelectList(sendingCountries, "Code", "Name");
            ViewBag.ReceivingCountry = new SelectList(receivingCountries, "Code", "Name");
        }

        private List<Services.DropDownViewModel> GetCountriesOfCurrency(string currency = "")
        {
            var countries = _commonServices.GetCountries().ToList();

            if (!string.IsNullOrEmpty(currency))
            {

                countries = countries.Where(x => x.CountryCurrency == currency.Trim()).ToList();
            }
            return countries;
        }
        public void SetSenderViewBag()
        {
            var senders = new List<SenderListDropDown>(); //_commonServices.GetSenderList();
            senders.Add(new SenderListDropDown()
            {
                senderId = 0,
                senderName = "All"
            });
            ViewBag.Senders = new SelectList(senders, "senderId", "senderName");
        }
        public JsonResult GetCountyByCurrency(string currency)
        {
            var countries = _commonServices.GetCountries().Where(x => x.CountryCurrency == currency).ToList();

            return Json(new
            {
                Data = countries
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetsenderByCountry(string country = "", string currency = "")
        {

            var senders = new List<SenderListDropDown>();
            List<string> senderdropDownValues = new List<string>();
            if (country == "All" || country == "")
            {
                var countries = (from c in _commonServices.GetCountries().Where(x => x.CountryCurrency == currency)
                                 select c).ToList();
                senders = (from c in _commonServices.GetSenderList().ToList()
                           join d in countries on c.Country equals d.CountryCode
                           select c).ToList();

            }
            else
            {
                var senderInformations = _commonServices.GetSenderList().ToList();
                senders = senderInformations.Where(x => x.Country == country.Trim()).ToList();

            }


            foreach (var sender in senders)
            {
                senderdropDownValues.Add("<option value = '" + sender.senderId + "'>" + sender.senderName + "</ option >");
            }

            return Json(new
            {

                Data = senderdropDownValues
            }, JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetAccountNumber(int senderId = 0)
        {
            string Accountnumber = _commonServices.GetSenderAccountNoBySenderId(senderId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSenders(SelectSearchParam param)
        {
            var senderInformations = _commonServices.GetSenderList().ToList();

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

    public class SelectDropDownVm
    {

        public int Id { get; set; }
        public string text { get; set; }
    }
    public class SelectSearchParam
    {
        public string query { get; set; }
        public string country { get; set; }
        public string City { get; set; }
        public string Currecny { get; set; }
        public bool isBusiness { get; set; }
        public bool isAuxAgent { get; set; }

    }
}