
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class MobileNotificationApi
    {
        public virtual async Task<bool> SendNotification(List<string> clientToken, string title, string body)
        {
            try
            {
                //var path = Common.GetAppSettingValue("FirebaseAppCredentialPath");
                var path = HttpContext.Current.Server.MapPath("~/JS/");
            //var path = @"F:\Web Svn Projects\2020-12-09 MoneyFex\trunk\FAXER.PORTAL\JS\";
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential 
                .FromFile(path + ("moneyfex-dce08-firebase-adminsdk-rd6xa-bfdb84661d.json"))
            });
           
                var message = new MulticastMessage()
                {
                    //Topic = "this is topic",
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Tokens = clientToken,
                };
                BatchResponse response = FirebaseMessaging.DefaultInstance.SendMulticastAsync(message).Result;
                FirebaseApp.DefaultInstance.Delete();

                Log.Write("Notification Send", DB.ErrorType.UnSpecified, "Notification");
                return true;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, DB.ErrorType.UnSpecified, "Notification");
                return false;

            }
        }

    }
}