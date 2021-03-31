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
    public class RecipientTransactionCountController : Controller
    {
        RecipientTransactionCountServices _services = null;
        CommonServices _commonServices = null;
        public RecipientTransactionCountController()
        {
            _services = new RecipientTransactionCountServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/RecipientTransactionCount
        public ActionResult Index(int? page = null)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<RecipientTransactionCountViewModel> vm = _services.GetRecipientTransactionCountList().ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        public ActionResult SetRecipientTransactionCount(int id = 0, string sendingCountry = "", string receivingCountry = "", int senderId = 0, int recipientId = 0)
        {
            SetCountyViewBag();
            SetSenderViewBag(sendingCountry);
            SetRecipientViewBag(senderId);
            RecipientTransactionCountViewModel vm = new RecipientTransactionCountViewModel();
            if (id > 0)
            {
                vm = _services.GetRecipientTransactionCountList().SingleOrDefault(x => x.Id == id);
            }
            if (!string.IsNullOrEmpty(sendingCountry) && !string.IsNullOrEmpty(receivingCountry) && senderId >= 0 && senderId >= 0 && recipientId >= 0)
            {
                var transactionAmountLimit = _services.GetRecipientTransactionCountList().Where(x => x.SendingCountry == sendingCountry &&
              x.ReceivingCountry == receivingCountry && x.SenderId == senderId && x.RecipientId == recipientId).FirstOrDefault();
                if (transactionAmountLimit != null)
                {
                    vm = transactionAmountLimit;
                }
                else
                {
                    vm.SendingCountry = sendingCountry;
                    vm.ReceivingCountry = receivingCountry;
                    vm.SenderId = senderId;
                    vm.SenderAccountNo = _commonServices.GetSenderAccountNoBySenderId(senderId);
                    vm.RecipientId = recipientId;
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetRecipientTransactionCount([Bind(Include = RecipientTransactionCountViewModel.BindProperty)] RecipientTransactionCountViewModel vm)
        {
            SetCountyViewBag();
            SetSenderViewBag(vm.SendingCountry);
            SetRecipientViewBag(vm.SenderId);
            if (ModelState.IsValid)
            {
                if (vm.Id > 0)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);
                }
                return RedirectToAction("Index", "RecipientTransactionCount");

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
            //_commonServices.GetCountries();
            Country.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            Country.AddRange(_commonServices.GetCountries());
            ViewBag.Countries = new SelectList(Country, "Code", "Name");
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
        public void SetRecipientViewBag(int SenderId = 0)
        {
            var Recipient = _commonServices.GetRecipients(SenderId);

            Recipient.Add(new DropDownViewModel()
            {
                Id = 0,
                Name = "All"
            });
            ViewBag.Recipients = new SelectList(Recipient, "Id", "Name");
        }

        public JsonResult GetsenderByCountry(string Country = "")
        {
            var data = _commonServices.GetSenderList().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecipient(string ReceivingCountry = "", int SenderId = 0)
        {
            var data = _commonServices.GetRecipients(SenderId).Where(x => x.CountryCode == ReceivingCountry).ToList();
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