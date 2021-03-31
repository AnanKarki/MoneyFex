using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;

namespace FAXER.PORTAL.Common
{
    public class SmsApi
    {

        //public const int BULKSMSCAPACITY = 1000;
        /// <summary>
        /// Phone Number Should include the phone code of the Particular country
        /// </summary>
        /// <param name="PhoneNumber"> For Instance  : +9779854621300</param>
        /// <param name="Message"></param>
        public void SendBulkSMS(List<string> phoneNumbers, string Message)
        {
            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];
            //string serviceSid = System.Configuration.ConfigurationManager.AppSettings["SmsApiServiceid"];
            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";
            string serviceSid = "ISe5135c0a4f43c7290352ecfee2370436";
            TwilioClient.Init(accountsid, accountToken);
            try
            {
                
                var notification = Twilio.Rest.Notify.V1.Service.NotificationResource.Create(
                      serviceSid,
                       toBinding: phoneNumbers ,
                       body: Message);
                Log.Write("SMS Send Successfully", DB.ErrorType.UnSpecified, "Bulk SMS");

            }
            catch (Exception ex)
            {
                //string execption = ex.Message;

                Log.Write(ex.Message, DB.ErrorType.UnSpecified, "Bulk SMS");
            }

        }
        public void SendSMS(string PhoneNumber, string Message)
        {
            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];
            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";
            TwilioClient.Init(accountsid, accountToken);

            try
            {
                var message = MessageResource.Create(
             body: Message,
             //from: new Twilio.Types.PhoneNumber("+447723455809"),
             from: "MoneyFex",
             to: new Twilio.Types.PhoneNumber(PhoneNumber)

             );
                Log.Write(PhoneNumber, DB.ErrorType.UnSpecified, "Sms send Successfully");

            }
            catch (Exception ex)
            {

                Log.Write(ex.Message, DB.ErrorType.UnSpecified, "SMS" + PhoneNumber);
            }

        }
        public bool IsValidMobileNo(string PhoneNumber)
        {
            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";
            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];
            bool IsValidMobileNo = true;
            TwilioClient.Init(accountsid, accountToken);

            try
            {
                var phoneNumber = PhoneNumberResource.Fetch(
                                  countryCode: "US",
                                 pathPhoneNumber: new Twilio.Types.PhoneNumber(PhoneNumber)
                                  );
                IsValidMobileNo = true;
            }
            catch (Exception ex)
            {

                IsValidMobileNo = false;
                string execption = ex.Message;
            }    
            

            return IsValidMobileNo;
        }
        public string GetCountryCodeViaPhoneNo(string PhoneNumber)
        {
            string accountsid = "ACf38f1fa489bbf2c521df42b491c3f314";
            string accountToken = "06292696b82c2c4b01d4424253efa8b9";
            //string accountsid = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountsid"];
            //string accountToken = System.Configuration.ConfigurationManager.AppSettings["SmsApiAccountToken"];
            string CountryCode = "";
            TwilioClient.Init(accountsid, accountToken);
            try
            {
                var phoneNumber = PhoneNumberResource.Fetch(
                                  countryCode: "US",
                                 pathPhoneNumber: new Twilio.Types.PhoneNumber(PhoneNumber)
                                  );
                CountryCode = phoneNumber.CountryCode;
            }
            catch (Exception ex)
            {

                CountryCode = "";
                string execption = ex.Message;
            }

            return CountryCode;
        }


        public string GetBankAccountDepositMsg(string FirstName, string Amount, string reference, string receiverFirstName, DB.BankDepositStatus status)
        {


            //Great News!
            //We've inform { first name} about your bank deposit of NGN 300,000 to them
            //Reference: BD54587
            //Thanks for using us
            //MoneyFex
            string message = "";
            if (status == DB.BankDepositStatus.Confirm)
            {
                message = string.Format("{0}{1}{2}{3}",
                            "Money transfer to " + receiverFirstName + "\n",
                            "We've inform " + receiverFirstName + " about your bank deposit of " + Amount + " to them." + "\n"
                            , "Reference : " + reference + "\n",
                             "MoneyFex");

            }
            else
            {
                message = string.Format("{0}{1}{2}{3}",
                                        "Transfer to" + " " + receiverFirstName + "," + "\n",
                                        "Your bank deposit of " + Amount + " " + "is in progress." + "\n",
                                        "Reference: " + reference + "\n",
                                        "\n\n MoneyTransfer = MoneyFex" + "\n");

            }
            return message;
        }

        public string GetMobileTransferMsg(string FirstName, string amount, string reference)
        {

            string message = string.Format("{0}{1}{2}{3}",
                       "Transfer to " + FirstName + "\n",
                       "We've inform " + FirstName + " about your Mobile money transfer of " + amount + " to them." + "\n"
                       , "Reference : " + reference + "\n",
                        "MoneyFex");

            return message;

        }

        public string GetMobileTransferInProgressMsg(string FirstName, string amount, string reference)
        {

            string message = string.Format("{0}{1}{2}{3}",
                       "Transfer to " + FirstName + "\n",
                       "Your Mobile Money transfer of " + amount + " to " + FirstName + " " + "is in progress." + "\n"
                       , "Reference : " + reference + "\n",
                        "MoneyFex");

            return message;

        }

        /// <summary>
        /// Registration SMS For three types of registration 
        /// Sender , Business , Virtual Account 
        /// </summary>
        /// <param name="VerificationCode"></param>
        /// <returns></returns>
        public string GetRegistrationMessage(string VerificationCode)
        {
            //string message = string.Format("{0}{1}{2}{3}{4}{5}",
            //    "Welcome to MoneyFex!\n\n",
            //    "Your verification code : " + VerificationCode + "\n\n", ""
            //    + "Please enter this code to login to your account.\n\n",
            //     "Thanks\n\n",
            //    "Customer Service\n",
            //     "MoneyFex");

            string message = string.Format("{0}{1}{2}",
                               "Verification Code:" + " " + VerificationCode + "\n\n",
                                "MoneyTransfer = MoneyFex \n",
                                "Fast, Easy and Cheap");

            return message;
        }

        /// <summary>
        /// Cash to Cash Money Transfer Sms message
        /// </summary>
        /// <param name="SenderName"></param>
        /// <param name="MFCN"></param>
        /// <param name="Amount"> Do Include Currency Symbol before amount as $500</param>
        /// <returns></returns>
        public string GetCashToCashTransferMessage(string SenderName, string MFCN, string Amount, string Fee = "", string AmountReceive = "")
        {

            string Message = string.Format("{0}{1}{2}{3}{4}{5}",
                                           "Cash pickup completed!\n\n",
                                           "MFCN : " + MFCN + "\n",
                                           "Amount sent: " + Amount + "\n",
                                           "Fee Amt:" + Fee + "\n",
                                           "Receiving Amt: " + AmountReceive,
                                           "\n\n MoneyFex");


            return Message;

        }


        public string GetCashPickUPTransferMessage(string senderFirstName, string receiverFirstname, string MFCN, string Amount, string Fee = "", string AmountReceive = "")
        {
            string Message = string.Format("{0}{1}{2}{3}{4}",
                                           "Transfer to" + " " + senderFirstName + "\n",
                                           "We've informed" + " " + receiverFirstname + " " + "about your transfer of" + " " + Amount + " " + "to them " + "\n",
                                           "MFCN : " + MFCN + "\n",
                                           "Track transfer",
                                           "\n\n MoneyTransfer = MoneyFex");
            return Message;

        }
        public string GetCashPickUPReceivedMessage(string senderFirstName, string receiverFirstname,
            string MFCN, string Amount, string Fee = "", string AmountReceive = "")
        {
            string Message = string.Format("{0}{1}{2}{3}{4}{5}",
                                           "Great News!" + "/n",
                                            receiverFirstname + " " + "has received" + " " + Amount + "\n",
                                           "MFCN : " + MFCN + "\n",
                                           "Thanks for using us",
                                           "\n\n MoneyTransfer = MoneyFex" + "\n",
                                           "Fast, Easy and Cheap");
            return Message;
        }

        public string GetCashPickupINProgressMessage(string senderFirstName, string receiverFirstname,
         string MFCN, string Amount, string Fee = "", string AmountReceive = "")
        {
            string Message = string.Format("{0}{1}{2}{3}",
                                            "Good News!" + "/n",
                                            "Your tranfer of " + Amount + " to " + receiverFirstname
                                            + " is in progress" + "\n",
                                           "MFCN : " + MFCN + "\n",
                                           "\n\n MoneyTransfer = MoneyFex");
            return Message;
        }
        public string GetCashPickUpReceivedToReceiverMessage(string senderCountry)
        {
            string Message = string.Format("{0}{1}{2}{3}",
                                           "Great News!" + "/n",
                                             "You've received a money transfer from " + senderCountry + ", they will contact you with information shortly" + "\n",
                                           "Thank you" + "\n",
                                           "\n\n MoneyTransfer = MoneyFex" + "\n");
            return Message;
        }

        public string GetStaffLoginCodeMessage(string AgentName, string code)
        {
            string Message = string.Format("{0}{1}{2}",
                                           "Hello" + " " + AgentName + "\n",
                                           "Your Staff Login Code is : " + code + "\n",
                                           "\n MoneyFex");
            return Message;
        }

        public string GetManualBankDepositAgentMessage(string Identifier, string AccName, string BankName, string AccNo, string BranchCode, string Amount)
        {
            string Message = string.Format("{0}{1}{2}{3}{4}{5}",
                                           "Identifier:" + " " + Identifier + "\n",
                                           "Acc.Name: " + AccName + "\n",
                                           "Bank: " + BankName + "\n",
                                           "Account: " + AccNo + "\n",
                                           "Branch Code: " + BranchCode + "\n",
                                           "Amount: " + Amount + "\n");
            return Message;
        }
        public string GetManualBankDepositSenderFirstMessage(string ReceiverName, string Amount, string Country,
            string Reference, bool IsIsCheck = false)
        {
            string Message = string.Format("{0}{1}{2}{3}",
                                           "Transfer to" + " " + ReceiverName + "," + "\n",
                                           "Your bank deposit of " + Amount + " " + "to" +
                                           " " + Country + " " + "is in progress." + "\n",
                                           "Reference: " + Reference + "\n",
                                        "\n\n MoneyTransfer = MoneyFex" + "\n");


            return Message;
        }
        public string GetManualBankDepositSenderSecondMessage(string ReceiverName, string Amount, string Reference)
        {

            string Message = string.Format("{0}{1}{2}{3}{4}{5}",
                                           " Great News!" + "\n",
                                           ReceiverName + " has received" + " " + Amount + "\n",
                                           "Reference:" + " " + Reference + "\n",
                                           "Thanks for using us" + "\n",
                                           "\n\n MoneyTransfer = MoneyFex" + "\n",
                                           "Fast, Easy and Cheap" + "\n");
            return Message;
        }

        public string GetManualBankDepositReceiverMessage(string SenderName, string Amount, string SenderCountry)
        {

            string Message = string.Format("{0}{1}{2}{3}",
                                           "Great News!" + "\n",
                                          "Your bank account has been credited with" + Amount + " " + "by" + " " + SenderName + " " +
                                           "who lives in " + SenderCountry + " " + "\n",
                                           "Thanks for using us" + "\n",
                                           "\n\n  MoneyFex" + "\n");
            return Message;
        }





        /// <summary>
        /// The function is used for three different portal
        /// Sender , Business 
        /// </summary>
        /// <param name="SecurityCode"></param>
        /// <returns></returns>

        public string GetPasswordResetMessage(string SecurityCode)
        {



            //string message = string.Format("{0}{1}{2}{3}{4}",
            //                                "Hello,\n\n",
            //                                "Your password reset code is: " + SecurityCode + "\n\n",
            //                                "Thanks\n",
            //                                "Customer Service\n",
            //                                "MoneyFex");


            string message = string.Format("{0}{1}{2}",
                               "Reset Code:" + SecurityCode + "\n\n",
                                "MoneyTransfer = MoneyFex \n",
                                "Fast, Easy and Cheap");
            return message;

        }

        internal string GetPinCodeMsg(string PinCode)
        {

            string message = string.Format("{0}{1}",
                                        "Your pin code is: " + PinCode + "\n\n",
                                        "MoneyFex");

            return message;

        }

        /// <summary>
        /// Sender Portal Address Update Verification Code before 
        /// Updating their address they need to verify account first
        /// through the given verification code
        /// </summary>
        /// <param name="VerificationCode"></param>
        /// <returns></returns>
        public string GetAddressUpdateMessage(string VerificationCode)
        {




            string message = string.Format("{0}{1}{2}{3}{4}",
                                            "Hello,\n\n",
                                            "Your address update verification code is: " + VerificationCode + "\n\n",
                                            "Thanks\n",
                                            "Customer Service\n",
                                            "MoneyFex");

            return message;
        }

        /// <summary>
        /// SMS is used by all the portal
        /// </summary>
        /// <param name="SenderName"></param>
        /// <param name="VirtualAccountNo"></param>
        /// <param name="Amount">Do Include Currency Symbol before amount as $500</param>
        /// <returns></returns>
        public string GetVirtualAccountDepositMessage(string SenderName, string MoblileNo, string Amount, string AmountReceive = "")
        {


            string message = string.Format("{0}{1}{2}{3}{4}",
                                         "Virtual account deposit completed!\n\n",
                                         "Virtual Account No :" + MoblileNo + "\n",
                                         "Amount :" + Amount + "\n",
                                         "Receiving Amt: " + AmountReceive + "\n\n",
                                         "MoneyFex");

            return message;
        }

        /// <summary>
        /// Business Payment SMS all the portal 
        /// </summary>
        /// <param name="SenderName"></param>
        /// <param name="BusinessAccountNo"></param>
        /// <param name="BusinessName"></param>
        /// <param name="Amount">Amount Should Include Currency Symbol a like $500</param>
        /// <param name="PaymentReference"></param>
        /// <returns></returns>
        public string GetBusinessPaymentMessage(string SenderName, string BusinessAccountNo, string BusinessName, string Amount, string PaymentReference, string AmountReceive = "")
        {


            string message = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                         "Payment confirmation!\n\n",
                                         "Business Name :" + BusinessName + "\n",
                                         "Acc. No. :" + BusinessAccountNo + "\n",
                                         "Amount :" + Amount + "\n",
                                         "Receving Amt: " + AmountReceive + "\n",
                                         "Ref :" + PaymentReference + "\n\n",
                                         "MoneyFex");

            return message;
        }


        /// <summary>
        /// The sms will be used form sender portal agent portal 
        /// </summary>
        /// <param name="SenderName"> Sender Is the Virtual account registar</param>
        /// <param name="VirtualAccountNo"></param>
        /// <param name="AccountUserName"></param>
        /// <param name="AccountUserCountry"></param>
        /// <returns></returns>
        public string GetVirtualAccountBalanceZeroMessage(string SenderName, string VirtualAccountNo, string AccountUserName, string AccountUserCountry)
        {
            string message = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                                            "hello " + SenderName + ",\n\n",
                                            "Virtual account balance is zero!\n\n",
                                            "Account No : " + VirtualAccountNo + "\n",
                                            "Account User Name :" + AccountUserName + "\n",
                                            "Country :" + AccountUserCountry + "\n\n",
                                            "Thanks \n\n",
                                            "MoneyFex");

            return message;
        }
        public string GetCashWithdrawalCodeMSG(string AgentName, string WithdrawalCode)
        {
            string message = string.Format("{0}{1}",
                                            "Withdrawal for" + AgentName + ":\n\n",
                                            "" + WithdrawalCode
                                            );

            return message;
        }

        public void SendKiiPayPersonalPaymentSMS(Models.KiiPayPersonalPaymentSMSVM model)
        {


            string message = GetVirtualAccountDepositMessage(model.SenderName, model.ReceiverPhoneNo, model.SendingAmount, model.ReceivingAmount);
            string phoneNumber = Common.GetCountryPhoneCode(model.SenderCountry) + model.SenderPhoneNo;
            SendSMS(phoneNumber, message);

            string receiverPhoneNo = Common.GetCountryPhoneCode(model.ReceiverCountry) + "" + model.ReceiverPhoneNo;
            SendSMS(receiverPhoneNo, message);
        }

        public void SendKiiPayBusinessPaymentSMS(Models.KiiPayBusinessPaymentSmsVM model)
        {


            string message = GetBusinessPaymentMessage(model.SenderName, model.ReceiverBusinessMobileNo, model.ReceiverBusinessName, model.SendingAmount, model.PaymentReference, model.ReceivingAmount);

            string SenderphoneNumber = Common.GetCountryPhoneCode(model.SenderCountry) + model.SenderPhoneNo;
            SendSMS(SenderphoneNumber, message);

            string receiverPhoneNo = Common.GetCountryPhoneCode(model.ReceiverCountry) + "" + model.ReceiverBusinessMobileNo;
            SendSMS(receiverPhoneNo, message);
        }
        public List<string> PrepareSmsGroup(List<NotifyUserSMSVm> users , Sms_binding_type sms_Binding_Type = Sms_binding_type.sms )
        {
            List<string> userGroups = new List<string>();

            string notify_binding_type = GetNotifyBindingType(sms_Binding_Type);
            foreach (var user in users)
            {
                userGroups.Add("{\"binding_type\":\"" + notify_binding_type +
                    "\",\"address\":\"" +  user.Address + "\"}");
            }
            return userGroups;
        }

        public string GetNotifyBindingType(Sms_binding_type binding_Type) {

            string notify_binding_type = "";
            switch (binding_Type)
            {
                case Sms_binding_type.sms:
                    notify_binding_type = "sms";
                    break;
                case Sms_binding_type.whatsapp:
                    notify_binding_type = "whatsapp";
                    break;
                default:
                    break;
            }
            return notify_binding_type;
        }
    }

   
  
}