using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class WhatsAppWebHookController : Controller
    {
        // GET: WhatsAppWebHook
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void ReceiveMessage(WhatsAppViewModel vm)
        {
            try
            {
                WhatsAppManager _whatsAppManager = new WhatsAppManager();
                var whatsAppMessage = new DB.WhatsAppMessage()
                {
                    Sender = vm.From,
                    Receiver = vm.To,
                    Direction = DB.Direction.Inbound,
                    Body = vm.Body,
                    Date = DateTime.Now,
                    MessageSId = vm.MessageSId,
                    NumMedia = vm.NumMedia,
                    Status = vm.Status,
                    SentDate = DateTime.Now,
                };
                _whatsAppManager.AddMessage(whatsAppMessage);

            }
            catch (Exception ex)
            {

                Log.Write(ex.Message, DB.ErrorType.UnSpecified, "What's App webhook");
            }


        }

    }
}