using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Conversations.V1;

namespace FAXER.PORTAL.Common
{
    public class WhatsAppApi
    {
        public MessageResource SendWhatsAppSMS(string PhoneNumber, string Message)
        {

            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];

            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";
            TwilioClient.Init(accountsid, accountToken);

            try
            {
                var message = MessageResource.Create(
                from: new Twilio.Types.PhoneNumber("whatsapp:+447723455809"),
                body: Message,
                to: new Twilio.Types.PhoneNumber(PhoneNumber));
                return message;
            }
            catch (Exception ex)
            {
                string execption = ex.Message;
                return null;
            }
        }
        public MessageResource SendWhatsAppSMSWithMedia(string PhoneNumber, string Message, string FileUrl)
        {
            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];

            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";

            TwilioClient.Init(accountsid, accountToken);

            var mediaUrl = new[] {
            new Uri(FileUrl)
               }.ToList();
            try
            {
                var message = MessageResource.Create(
                body: Message,
                mediaUrl: mediaUrl,
                from: new Twilio.Types.PhoneNumber("whatsapp:+14155238886"),
                to: new Twilio.Types.PhoneNumber(PhoneNumber));
                return message;
            }
            catch (Exception ex)
            {
                string execption = ex.Message;
                return null;
            }


        }


        public string CreateAGroup()
        {
            // Find your Account Sid and Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            //string accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
            //string authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");

            string accountSid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";

            TwilioClient.Init(accountSid, accountToken);
            try
            {
                var conversation = Twilio.Rest.Conversations.V1.ConversationResource.Create(
                        friendlyName: "MoneyFex"
                    );

                var participant = Twilio.Rest.Conversations.V1.Conversation.ParticipantResource.Create(
                          messagingBindingAddress: "whatsapp:+9779818548590",
                          messagingBindingProxyAddress: "whatsapp:+447723455809",
                          pathConversationSid: conversation.Sid
                         );
                var message = Twilio.Rest.Conversations.V1.Conversation.MessageResource.Create(
                            author: "whatsapp:+447723455809",
                            body: "Hello Robert, your food delivery is almost there but Alicia (your rider) needs help finding your door. Are you willing to chat with them?",
                            pathConversationSid: conversation.Sid
                        );
                return conversation.Sid;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
        public void AddChatParticipant(string pathConversationSid, string ParticipantName, string ParticipantNumber)
        {
            // Find your Account Sid and Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure

            string accountSid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string authToken = "06292696b82c2c4b01d4424253efa8b9";

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var participant = Twilio.Rest.Conversations.V1.Conversation.ParticipantResource.Create(
                messagingBindingAddress: "whatsapp:+447723455809",
                messagingBindingProxyAddress: "whatsapp:447723455809",
                pathConversationSid: pathConversationSid
               );
            }
            catch (Exception ex)
            {
                string execption = ex.Message;
            }
        }

    }
}