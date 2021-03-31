using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio.Rest.Api.V2010.Account;

namespace FAXER.PORTAL.Services
{
    public class WhatsAppManager : IWhatsAppManager
    {
        FAXEREntities dbContext = null;
        public WhatsAppManager()
        {
            dbContext = new FAXEREntities();
        }

        public void AddMessage(WhatsAppMessage vm)
        {
            dbContext.WhatsAppMessage.Add(vm);
            dbContext.SaveChanges();
        }

        public List<WhatsAppMessage> GetWhatsAppMessage()
        {
            return dbContext.WhatsAppMessage.ToList();
        }

        public MessageResource SendMessage(WhatsAppViewModel vm)
        {
            var smsapi = new WhatsAppApi();

            if (!string.IsNullOrEmpty(vm.FileURL))
            {

                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                var fileURL = baseUrl + vm.FileURL;
                var messageResource = smsapi.SendWhatsAppSMSWithMedia(vm.To, vm.Body, fileURL);
                return messageResource;
            }
            else
            {
                var messageResource = smsapi.SendWhatsAppSMS(vm.To, vm.Body);
                return messageResource;
            }
        }
    }
}