using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class CardUserNonCardWithdrawalServices
    {
        DB.FAXEREntities dbContext = null;

        public CardUserNonCardWithdrawalServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public Models.PayNonMFTCCardUserViewModel GetTransactionDetails(string MFCN)
        {

            var agentInfo = Common.AgentSession.AgentInformation;
            var data = (from tran in dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                        select new Models.PayNonMFTCCardUserViewModel()
                        {
                            FaxedAmount = String.Format("{0:n}", tran.FaxingAmount),
                            MFCN = tran.MFCN,
                            FaxerAddress = tran.CardUserReceiverDetails.MFTCCardInformation.Address1,
                            FaxerCity = tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCity,
                            FaxerCountryCode = tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry,
                            FaxerCountry = Common.Common.GetCountryName(tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry),
                            FaxerEmail = tran.CardUserReceiverDetails.MFTCCardInformation.CardUserEmail,
                            FaxerFirstName = tran.CardUserReceiverDetails.MFTCCardInformation.FirstName,
                            FaxerLastName = tran.CardUserReceiverDetails.MFTCCardInformation.LastName,
                            FaxerMiddleName = tran.CardUserReceiverDetails.MFTCCardInformation.MiddleName,
                            FaxerTelephone =  tran.CardUserReceiverDetails.MFTCCardInformation.CardUserTel,
                            FaxerPhoneCode = Common.Common.GetCountryPhoneCode(tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry) ,
                            DateTime = tran.TransactionDate,
                            ReceiverId = tran.NonCardRecieverId,
                            ReceiverCity = tran.CardUserReceiverDetails.City,
                            ReceiverFirstName = tran.CardUserReceiverDetails.FirstName,
                            ReceiverMiddleName = tran.CardUserReceiverDetails.MiddleName,
                            ReceiverEmail = tran.CardUserReceiverDetails.EmailAddress,
                            ReceiverLastName = tran.CardUserReceiverDetails.LastName,
                            ReceiverTelephone = tran.CardUserReceiverDetails.PhoneNumber,
                            ReceiverCountryCode = tran.CardUserReceiverDetails.Country,
                            ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(tran.CardUserReceiverDetails.Country),
                            ReceiverCountry = Common.Common.GetCountryName(tran.CardUserReceiverDetails.Country),
                            NameOfAgency = agentInfo.Name,
                            AgencyMFSCode = agentInfo.AccountNo,
                            AgentId = agentInfo.Id,
                            StatusOfFax = tran.FaxingStatus,
                            StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus),
                            RefundRequest = tran.FaxingStatus == FaxingStatus.Refund ? "YES" : "NO",
                            FaxerCurrency = Common.Common.GetCountryCurrency(tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry),
                            FaxerCurrencySymbol = Common.Common.GetCurrencySymbol(tran.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry),
                            ReceiverCurrency = Common.Common.GetCountryCurrency(tran.CardUserReceiverDetails.Country),
                            ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(tran.CardUserReceiverDetails.Country),
                            ReceivingAmount = tran.ReceivingAmount.ToString(),
                            WithdrawalPaymentOf = Models.WithdrawalPaymentOf.CardUser
                        }).FirstOrDefault();

            return data;

        }


        public CardUserNonCardWithdrawal MakeAnwithdrawal(CardUserNonCardWithdrawal model)
        {

            dbContext.CardUserNonCardWithdrawal.Add(model);
            dbContext.SaveChanges();
            return model;
        }


        public CardUserNonCardTransaction UpdateCardUserNonCardTransaction(string MFCN) {

            var data = dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            data.FaxingStatus = FaxingStatus.Received;
            data.StatusChangedDate = DateTime.Now;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }


    }
}