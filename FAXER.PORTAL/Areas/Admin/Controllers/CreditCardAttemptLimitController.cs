using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class CreditCardAttemptLimitController : Controller
    {

        CreditCardAttemptLimitServices _services = null;
        CommonServices _commonServices = null;
        public CreditCardAttemptLimitController()
        {
            _services = new CreditCardAttemptLimitServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/CreditCardAttemptLimit
        public ActionResult Index(int? page = null)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<CreditCardAttemptLimitViewModel> vm = _services.GetCreditCardAttemptLimitList().ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult SetCreditCardAttemptLimit(int id = 0, string sendingCountry = "", string receivingCountry = "", int senderId = 0)
        {
            CreditCardAttemptLimitViewModel vm = new CreditCardAttemptLimitViewModel();
            SetCountyViewBag();
            SetSenderViewBag(sendingCountry);
            if (id > 0)
            {
                vm = _services.GetCreditCardAttemptLimitList().SingleOrDefault(x => x.Id == id);
            }
            if (!string.IsNullOrEmpty(sendingCountry) && !string.IsNullOrEmpty(receivingCountry) && senderId >= 0)
            {
                var AttemptLimit = _services.GetCreditCardAttemptLimitList().Where(x => x.SendingCountry == sendingCountry &&
                          x.ReceivingCountry == receivingCountry && (x.SenderId == senderId || x.SenderId == null)).FirstOrDefault();
                if (AttemptLimit != null)
                {
                    vm = AttemptLimit;
                }
                else
                {
                    vm.SendingCountry = sendingCountry;
                    vm.ReceivingCountry = receivingCountry;
                    vm.SenderId = senderId;
                    vm.SenderAccountNo = _commonServices.GetSenderAccountNoBySenderId(senderId);
                }
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult SetCreditCardAttemptLimit([Bind(Include = CreditCardAttemptLimitViewModel.BindProperty)] CreditCardAttemptLimitViewModel vm)
        {
            SetCountyViewBag();
            SetSenderViewBag(vm.SendingCountry);
            if (ModelState.IsValid)
            {
                if (vm.SenderId == 0)
                {
                    vm.SenderId = null;
                }
                if (vm.Id > 0)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);
                }
                return RedirectToAction("Index", "CreditCardAttemptLimit");

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
        public void SetCountyViewBag()
        {
            var Country = new List<DropDownViewModel>();
            Country.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            Country.AddRange(_commonServices.GetCountries());

            ViewBag.Countries = new SelectList(Country, "Code", "Name");
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
        public JsonResult GetsenderByCountry(string Country = "")
        {
            var data = _commonServices.GetSenderList().Where(x => x.Country == Country).ToList();
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


    }
}