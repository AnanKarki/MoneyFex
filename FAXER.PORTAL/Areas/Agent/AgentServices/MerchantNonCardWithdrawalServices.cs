using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class MerchantNonCardWithdrawalServices
    {
        DB.FAXEREntities dbContext = null;
        public MerchantNonCardWithdrawalServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public Models.PayNonMFTCCardUserViewModel GetTransactionDetails(string MFCN)
        {

            var agentInfo = Common.AgentSession.AgentInformation;
            var data = (from tran in dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                        join sender in dbContext.KiiPayBusinessWalletInformation on tran.MFBCCardID equals sender.Id
                        select new Models.PayNonMFTCCardUserViewModel()
                        {
                            FaxedAmount = String.Format("{0:n}", tran.FaxingAmount),
                            MFCN = tran.MFCN,
                            FaxerAddress = sender.AddressLine1,
                            FaxerCity = sender.City,
                            FaxerCountryCode = sender.Country,
                            FaxerCountry = Common.Common.GetCountryName(sender.Country),
                            FaxerEmail = sender.Email,
                            FaxerFirstName = sender.FirstName,
                            FaxerLastName = sender.LastName,
                            FaxerMiddleName = sender.MiddleName,
                            FaxerTelephone = sender.PhoneNumber,
                            FaxerPhoneCode = Common.Common.GetCountryPhoneCode(sender.Country),
                            DateTime = tran.TransactionDate,
                            ReceiverId = tran.NonCardRecieverId,
                            ReceiverCity = tran.NonCardReciever.City,
                            ReceiverFirstName = tran.NonCardReciever.FirstName,
                            ReceiverMiddleName = tran.NonCardReciever.MiddleName,
                            ReceiverEmail = tran.NonCardReciever.EmailAddress,
                            ReceiverLastName = tran.NonCardReciever.LastName,
                            ReceiverTelephone = tran.NonCardReciever.PhoneNumber,
                            ReceiverCountryCode = tran.NonCardReciever.Country,
                            ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(tran.NonCardReciever.Country),
                            ReceiverCountry = Common.Common.GetCountryName(tran.NonCardReciever.Country),
                            NameOfAgency = agentInfo.Name,
                            AgencyMFSCode = agentInfo.AccountNo,
                            AgentId = agentInfo.Id,
                            StatusOfFax = tran.FaxingStatus,
                            StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus),
                            RefundRequest = tran.FaxingStatus == FaxingStatus.Refund ? "YES" : "NO",
                            FaxerCurrency = Common.Common.GetCountryCurrency(sender.Country),
                            FaxerCurrencySymbol = Common.Common.GetCurrencySymbol(sender.Country),
                            ReceiverCurrency = Common.Common.GetCountryCurrency(tran.NonCardReciever.Country),
                            ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(tran.NonCardReciever.Country),
                            ReceivingAmount = tran.ReceivingAmount.ToString(),
                            WithdrawalPaymentOf = Models.WithdrawalPaymentOf.Merchant
                        }).FirstOrDefault();

            return data;

        }


        public DB.MerchantNonCardWithdrawal MakeAnWithdrawal(MerchantNonCardWithdrawal model)
        {

            dbContext.MerchantNonCardWithdrawal.Add(model);
            dbContext.SaveChanges();
            return model;


        }

        public DB.MerchantNonCardTransaction UpdateMerchantNonCardTransaction(string MFCN)
        {


            var data = dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            data.FaxingStatus = FaxingStatus.Received;
            data.StatusChangedDate = DateTime.Now;

            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
    }
}