using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class WhatsAppController : Controller
    {
        CommonServices _CommonServices = null;
        WhatsAppManager _whatsAppManager = null;
        public WhatsAppController()
        {
            _CommonServices = new CommonServices();
            _whatsAppManager = new WhatsAppManager();

        }
        // GET: Staff/WhatsApp
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            return View();
        }
        [HttpGet]
        public JsonResult GetCountries()
        {
            var result = _CommonServices.GetCountries();
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllReceivers(string countryCode = "")
        {
            CommonServices _commonServices = new CommonServices();

            var senderInfo = _commonServices.GetAllSenderInfo();
            if (!string.IsNullOrEmpty(countryCode) && countryCode != "undefined")
            {
                senderInfo = senderInfo.Where(x => x.Country == countryCode).ToList();
            }
            var result = (from c in senderInfo
                          select new ReceiverViewModel()
                          {
                              Id = c.Id,
                              PhoneNumber = "whatsapp:" + Common.Common.GetCountryPhoneCode(c.Country) + c.PhoneNumber,
                              ReceiverName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                          }).ToList();
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetReceivers()
        {
            var result = new List<ReceiverViewModel>();
            var whatsAppData = _whatsAppManager.GetWhatsAppMessage();
            var Sender = whatsAppData.GroupBy(x => x.Sender).ToList();
            var Receiver = whatsAppData.GroupBy(x => x.Receiver).ToList();
            var data = Sender.Concat(Receiver).GroupBy(x => x.Key);
            foreach (var item in data)
            {
                var smsapi = new SmsApi();
                CommonServices _commonServices = new CommonServices();
                string sender = item.Key;
                var senderNumber = sender.Split(':')[1];
                var countryCode = smsapi.GetCountryCodeViaPhoneNo(senderNumber);
                var countryPhonecode = Common.Common.GetCountryPhoneCode(countryCode);
                var sendernumberWithoutPhncode = senderNumber.Substring(countryPhonecode.Length, senderNumber.Length - countryPhonecode.Length);
                var senderInfo = _commonServices.GetAllSenderInfo().Where(x => x.PhoneNumber == sendernumberWithoutPhncode).FirstOrDefault();
                if (senderInfo != null)
                {
                    result.Add(new ReceiverViewModel()
                    {
                        Id = senderInfo.Id,
                        PhoneNumber = sender,
                        ReceiverName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName
                    });
                }

            }
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMessages(ReceiverViewModel model)
        {
            var result = (from c in _whatsAppManager.GetWhatsAppMessage().Where(x => (x.Receiver == model.PhoneNumber || x.Sender == model.PhoneNumber)).ToList()
                          select new MessageViewModel()
                          {
                              ReceivedMessageWithTime = c.Direction == DB.Direction.Inbound ? c.Body + "     " + c.SentDate.ToString("hh:mm tt") : "",
                              SentMessageWithTime = c.Direction == DB.Direction.Outbound ?  c.Body + "     " + c.SentDate.ToString("hh:mm tt") : "",
                              DateTime = c.Date,
                              SenderName = c.SentBy,
                              FileURL = c.Media
                          }).OrderBy(x => x.DateTime).ToList();

            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SendMessage(WhatsAppViewModel vm)
        {
            //var smsapi = new WhatsAppApi();

            var messageResource = _whatsAppManager.SendMessage(vm);
            if (messageResource != null)
            {
                WhatsAppMessage whatsAppMessage = new WhatsAppMessage()
                {
                    Body = vm.Body,
                    Date = DateTime.Now,
                    Direction = Direction.Outbound,
                    Sender = messageResource.From.ToString(),
                    Receiver = vm.To,
                    SentBy = StaffSession.LoggedStaff.FirstName + " " + StaffSession.LoggedStaff.MiddleName + " " + StaffSession.LoggedStaff.LastName,
                    Status = messageResource.Status.ToString(),
                    MessageSId = messageResource.Sid,
                    NumMedia = messageResource.NumMedia,
                    SentDate = DateTime.Now,
                    Media = vm.FileURL,
                };
                _whatsAppManager.AddMessage(whatsAppMessage);
                var messageModel = new MessageViewModel()
                {
                    SentMessageWithTime = whatsAppMessage.Body + "     " + whatsAppMessage.SentDate.ToString("hh:mm tt"),
                };
                return Json(new
                {
                    Data = messageModel,
                    ReceiverPhoneNumber = whatsAppMessage.Receiver,
                    Status = true,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = new MessageViewModel(),
                    ReceiverPhoneNumber = vm.To,
                    Status = false,
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}