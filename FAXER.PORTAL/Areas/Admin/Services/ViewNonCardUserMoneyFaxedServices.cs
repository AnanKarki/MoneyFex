using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FAXER.PORTAL.Areas.Admin.Services
{

    public class ViewNonCardUserMoneyFaxedServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices common = new CommonServices();
        public ViewNonCardUserMoneyFaxedViewModel getNonCardUserMoneyFaxedList()
        {
            var result = new ViewNonCardUserMoneyFaxedViewModel();
            result.ViewNonCardUserFaxer = (from c in dbContext.FaxingNonCardTransaction.ToList()
                                           join d in dbContext.ReceiverNonCardWithdrawl on c.MFCN equals d.MFCN
                                           into k
                                           from joined in k.DefaultIfEmpty()
                                           select new ViewNonCardUserMoneyFaxedFaxerViewModel()
                                           {
                                               Id = c.Id,
                                               FaxerFirstName = c.NonCardReciever.FaxerInformation.FirstName,
                                               FaxerMiddleName = c.NonCardReciever.FaxerInformation.MiddleName,
                                               FaxerLastName = c.NonCardReciever.FaxerInformation.LastName,
                                               FaxerAddress = c.NonCardReciever.FaxerInformation.Address1,
                                               FaxerCity = c.NonCardReciever.FaxerInformation.City,
                                               FaxerCountry = common.getCountryNameFromCode(c.NonCardReciever.FaxerInformation.Country),
                                               FaxerTelephone = c.NonCardReciever.FaxerInformation.PhoneNumber,
                                               FaxerEmail = c.NonCardReciever.FaxerInformation.Email,
                                               FaxerIDCardNumber = c.NonCardReciever.FaxerInformation.IdCardNumber,
                                               FaxerIDCardType = c.NonCardReciever.FaxerInformation.IdCardType,
                                               FaxerIDCardExpDate = c.NonCardReciever.FaxerInformation.IdCardExpiringDate.ToString("dd-MM-yyyy"),
                                               FaxerIDCardIssuingCountry = common.getCountryNameFromCode(c.NonCardReciever.FaxerInformation.IssuingCountry),
                                               FaxedAmount = c.FaxingAmount,
                                               Fee = c.FaxingFee,
                                               PaymentMethod = c.PaymentMethod == null ? "" : (c.PaymentMethod == "PM001" || c.PaymentMethod == "PM002") ? "Fax Money Using Credit/Debit Card" : c.PaymentMethod == "PM003" ? "Fax Money using Bank-to-bank Payment" : "Bit Coin",
                                               FaxingMethod = Enum.GetName(typeof(OperatingUserType), c.OperatingUserType),
                                               NameofSendingPortal = Enum.GetName(typeof(OperatingUserType), c.OperatingUserType),
                                               //PortalFaxingAdminName = c.na
                                               MFCN = c.MFCN,

                                               //receiver withdrawal merged
                                               ReceiverFirstName = joined == null ? "" : joined.ReceiversDetails.FirstName,
                                               ReceiverMiddleName = joined == null ? "" : joined.ReceiversDetails.MiddleName,
                                               ReceiverLastName = joined == null ? "" : joined.ReceiversDetails.LastName,
                                               ReceiverCity = joined == null ? "" : joined.ReceiversDetails.City,
                                               ReceiverCountry = joined == null ? "" : common.getCountryNameFromCode(joined.ReceiversDetails.Country),
                                               ReceiverTelephone = joined == null ? "" : joined.ReceiversDetails.PhoneNumber,
                                               ReceiverEmail = joined == null ? "" : joined.ReceiversDetails.EmailAddress,
                                               FaxingAgentName = joined == null ? "" : joined.Agent.Name,
                                               FaxingAgentMFSCode = joined == null ? "" : joined.Agent.AccountNo,
                                               FaxingAgentVerifier = joined == null ? "" : joined.PayingAgentName,
                                               Date = joined == null ? "" : joined.TransactionDate.ToString("dd-MM-yyyy"),
                                               Time = joined == null ? "" : joined.TransactionDate.ToString("HH:mm"),
                                               //FaxingUpdate = c.upd
                                               Status = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)

                                           }).ToList();



            return result;

        }


        public FaxingNonCardTransaction GetTransactionDetails(int TransactionID)
        {
            var data = dbContext.FaxingNonCardTransaction.Where(x => x.Id == TransactionID).FirstOrDefault();
            return data;
        }
        internal bool HoldUnholdNonCardTransaction(FaxingNonCardTransaction model)
        {

            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return true;

        }

        public ViewNonCardUserMoneyFaxedViewModel getFilterNonCardMoneyFaxedList(string CountryCode, string City)
        {

            var data = new List<DB.FaxingNonCardTransaction>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = (from c in dbContext.FaxingNonCardTransaction
                        join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.Country == CountryCode
                        select c).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = (from c in dbContext.FaxingNonCardTransaction
                        join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.City.ToLower() == City.ToLower()
                        select c).ToList();
            }
            else
            {

                data = (from c in dbContext.FaxingNonCardTransaction
                        join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.Country == CountryCode && e.City.ToLower() == City.ToLower()
                        select c).ToList();
            }

            var result = new ViewNonCardUserMoneyFaxedViewModel();
            result.ViewNonCardUserFaxer = (from c in data.OrderByDescending(x => x.TransactionDate)
                                           join d in dbContext.AgentFaxMoneyInformation on c.Id equals d.NonCardTransactionId into k
                                           from joined in k.DefaultIfEmpty()
                                           join agentUpdate in dbContext.FaxingUpdatedInformation on c.Id equals agentUpdate.NonCardTransactionId into l
                                           from agentUpdated in l.DefaultIfEmpty()
                                           join adminUpdate in dbContext.FaxingUpdatedInformationByAdmin on c.Id equals adminUpdate.NonCardTransactionId into m
                                           from adminUpdated in m.DefaultIfEmpty()

                                           select new ViewNonCardUserMoneyFaxedFaxerViewModel()
                                           {
                                               Id = c.Id,
                                               FaxerFirstName = c.NonCardReciever.FaxerInformation.FirstName,
                                               FaxerMiddleName = c.NonCardReciever.FaxerInformation.MiddleName,
                                               FaxerLastName = c.NonCardReciever.FaxerInformation.LastName,
                                               FaxerAddress = c.NonCardReciever.FaxerInformation.Address1,
                                               FaxerCity = c.NonCardReciever.FaxerInformation.City,
                                               FaxerCountry = common.getCountryNameFromCode(c.NonCardReciever.FaxerInformation.Country),
                                               FaxerTelephone = common.getPhoneCodeFromCountry(c.NonCardReciever.FaxerInformation.Country) + c.NonCardReciever.FaxerInformation.PhoneNumber,
                                               FaxerEmail = c.NonCardReciever.FaxerInformation.Email,
                                               FaxerIDCardNumber = c.NonCardReciever.FaxerInformation.IdCardNumber,
                                               FaxerIDCardType = c.NonCardReciever.FaxerInformation.IdCardType,
                                               FaxerIDCardExpDate = c.NonCardReciever.FaxerInformation.IdCardExpiringDate.ToString("dd-MM-yyyy"),
                                               FaxerIDCardIssuingCountry = common.getCountryNameFromCode(c.NonCardReciever.FaxerInformation.IssuingCountry),
                                               FaxedAmount = c.FaxingAmount,
                                               FaxingCurrency = common.getCurrencyCodeFromCountry(c.NonCardReciever.FaxerInformation.Country),
                                               Fee = c.FaxingFee,
                                               PaymentMethod = c.PaymentMethod == null ? Enum.GetName(typeof(OperatingUserType), c.OperatingUserType) : (c.PaymentMethod == "PM001" || c.PaymentMethod == "PM002") ? "Fax Money Using Credit/Debit Card" : c.PaymentMethod == "PM003" ? "Fax Money using Bank-to-bank Payment" : "Bit Coin",
                                               FaxingMethod = Enum.GetName(typeof(OperatingUserType), c.OperatingUserType),
                                               PortalFaxingAdminName = c.OperatingUserType == OperatingUserType.Admin ? common.getStaffName(c.UserId) : "",
                                               MFCN = c.MFCN,

                                               //receiver withdrawal merged
                                               ReceiverFirstName = c.NonCardReciever.FirstName,//joined == null ? "" : joined.ReceiversDetails.FirstName,
                                               ReceiverMiddleName = c.NonCardReciever.MiddleName, //joined == null ? "" : joined.ReceiversDetails.MiddleName,
                                               ReceiverLastName = c.NonCardReciever.LastName, //joined == null ? "" : joined.ReceiversDetails.LastName,
                                               ReceiverCity = c.NonCardReciever.City, //joined == null ? "" : joined.ReceiversDetails.City,
                                               ReceiverCountry = common.getCountryNameFromCode(c.NonCardReciever.Country), //joined == null ? "" : common.getCountryNameFromCode(joined.ReceiversDetails.Country),
                                               ReceiverTelephone = common.getPhoneCodeFromCountry(c.NonCardReciever.Country) + c.NonCardReciever.PhoneNumber,  //joined == null ? "" : common.getPhoneCodeFromCountry(joined.ReceiversDetails.Country) + joined.ReceiversDetails.PhoneNumber,
                                               ReceiverEmail = c.NonCardReciever.EmailAddress, //joined == null ? "" : joined.ReceiversDetails.EmailAddress,
                                               FaxingAgentName = c.OperatingUserType == OperatingUserType.Agent ? common.getAgentName(joined.AgentId) : "", //joined == null ? "" : joined.Agent.Name,
                                               FaxingAgentMFSCode = c.OperatingUserType == OperatingUserType.Agent ? joined.Agent.AccountNo : "", //joined == null ? "" : joined.Agent.AccountNo,
                                               FaxingAgentVerifier = c.OperatingUserType == OperatingUserType.Agent ? joined.NameOfPayingAgent : "", //joined == null ? "" : joined.PayingAgentName,
                                               Date = c.TransactionDate.ToFormatedString(), //joined == null ? "" : joined.TransactionDate.ToString("dd-MM-yyyy"),
                                               Time = c.TransactionDate.ToString("HH:mm"), //joined == null ? "" : joined.TransactionDate.ToString("HH:mm"),
                                               FaxingUpdate = agentUpdated != null ? "Agent" : adminUpdated != null ? "Admin" : "Not Updated",
                                               UpdatingName = agentUpdated != null ? agentUpdated.NameOfUpdatingAgent : adminUpdated != null ? adminUpdated.NameOfUpdatingAdmin : "",
                                               UpdatingDate = agentUpdated != null ? agentUpdated.Date.ToFormatedString() : adminUpdated != null ? adminUpdated.Date.ToFormatedString() : "",
                                               UpdatingTime = agentUpdated != null ? agentUpdated.Date.ToString("HH:mm") : adminUpdated != null ? adminUpdated.Date.ToString("HH:mm") : "",
                                               Status = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus),
                                               FaxingStatus = c.FaxingStatus
                                           }).ToList();



            return result;

        }

        public DB.FaxingNonCardTransaction cancelTransaction(int id)
        {
            var data = dbContext.FaxingNonCardTransaction.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.FaxingStatus = FaxingStatus.Cancel;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                #region Bank AccountCredit Update 

                if (data.OperatingUserType == OperatingUserType.Agent)
                {

                    var bankAccountCreditUpdate = dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.NonCardTransactionId == data.Id).FirstOrDefault();
                    if (bankAccountCreditUpdate != null)
                    {
                        bankAccountCreditUpdate.CustomerDeposit = bankAccountCreditUpdate.CustomerDeposit - data.FaxingAmount;
                        dbContext.Entry(bankAccountCreditUpdate).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }
                }
                #endregion

                //send email for Fax Cancellation 
                MailCommon mail = new MailCommon();

                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = common.getFaxerName(data.NonCardReciever.FaxerID);
                string FaxerEmail = data.NonCardReciever.FaxerInformation.Email;
                string FaxerCurrency = common.getCurrency(data.NonCardReciever.FaxerInformation.Country);
                var receiverDetails = data.NonCardReciever;
                string body = "";
                string ReceiverCountry = common.getCountryNameFromCode(data.NonCardReciever.Country);
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxCancellationEmail?FaxerName=" + FaxerName +
                    "&MFCN=" + data.MFCN + "&SentAmount=" + data.FaxingAmount + " " + FaxerCurrency +
                    "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                    "&ReceiverCountry=" + ReceiverCountry + "&ReceiverCity=" + receiverDetails.City + "&SentDate=" + data.TransactionDate);

                mail.SendMail(FaxerEmail, "Transaction Cancellation Request", body);


                string body2 = "";
                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MoneyFexTransactionCancellation?NameOfCancellar=" + common.getStaffName(Common.StaffSession.LoggedStaff.StaffId)
                    + "&SenderName=" + FaxerName + "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                    "&MFCN=" + data.MFCN + "&TransactionAmount=" + data.FaxingAmount + " " + FaxerCurrency);

                mail.SendMail("refund@moneyfex.com", "Alert - Transaction Cancellation " + "MFCN" + data.MFCN, body2);

                return data;
            }
            return null;
        }
        public AdminNonCardMoneyTransferReceiptViewModel getReceiptInfo(int id)
        {
            var result = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.Id == id).ToList()
                          select new AdminNonCardMoneyTransferReceiptViewModel()
                          {
                              MFReceiptNumber = c.ReceiptNumber,
                              TransactionDate = c.TransactionDate.ToFormatedString(),
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              FaxerFullName = c.NonCardReciever.FaxerInformation.FirstName + " " + c.NonCardReciever.FaxerInformation.MiddleName + " " + c.NonCardReciever.FaxerInformation.LastName,
                              FaxerCountryCode = c.NonCardReciever.FaxerInformation.Country,
                              FaxerPhoneNo = c.NonCardReciever.FaxerInformation.PhoneNumber,
                              MFCN = c.MFCN,
                              ReceiverFullName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              Telephone = c.NonCardReciever.PhoneNumber,
                              StaffName = c.OperatingUserType == OperatingUserType.Admin ? common.getStaffName(c.UserId) : "",
                              StaffCode = c.OperatingUserType == OperatingUserType.Admin ? common.getStaffMFSCode(c.UserId) : "",
                              StaffLoginCode = c.OperatingUserType == OperatingUserType.Admin ? common.getStaffLoginCode(c.UserId) : "",
                              AmountSent = c.FaxingAmount.ToString(),
                              ExchangeRate = c.ExchangeRate.ToString(),
                              Fee = c.FaxingFee.ToString(),
                              AmountReceived = c.ReceivingAmount.ToString(),
                              TotalAmountSentAndFee = (c.FaxingAmount + c.FaxingFee).ToString(),
                              SendingCurrency = common.getFaxerCurrencyFromId(c.NonCardReciever.FaxerID),
                              ReceivingCurrency = common.getCurrencyCodeFromCountry(c.NonCardReciever.Country)
                          }).FirstOrDefault();
            return result;
        }




    }
}