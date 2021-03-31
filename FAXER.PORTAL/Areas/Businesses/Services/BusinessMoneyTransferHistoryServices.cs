using FAXER.PORTAL.Areas.Businesses.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessMoneyTransferHistoryServices
    {
        DB.FAXEREntities dbContext = null;
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
        public BusinessMoneyTransferHistoryServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<BusinessMoneyTransferHistoryViewModel> getTransactionhistoryList(string searchText = "")
        {

            var data = new List<DB.MerchantNonCardTransaction>();
            if (!string.IsNullOrEmpty(searchText))
            {
                string text = Regex.Replace(searchText, @"\s+", "");
                data = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.FaxingStatus != DB.FaxingStatus.NotReceived
                           && x.MFCN.ToLower() == searchText.ToLower() || (x.NonCardReciever.FirstName.ToLower() + " " + x.NonCardReciever.MiddleName.ToLower() +
                           " " + x.NonCardReciever.LastName.ToLower()) == text)
                        select c).ToList();
            }
            else
            {
                data = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.FaxingStatus != DB.FaxingStatus.NotReceived)
                        select c).ToList();
            }
            var result = (from c in data.ToList()
                          select new BusinessMoneyTransferHistoryViewModel()
                          {
                              TransactionId = c.Id,
                              ReceiverId = c.NonCardRecieverId,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              MFCN = c.MFCN,
                              StatusOfTransfer = Enum.GetName(typeof(DB.FaxingStatus), c.FaxingStatus),
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TransactionDateTime = c.TransactionDate,
                              TransferAmount =  Common.Common.GetCurrencySymbol(c.NonCardReciever.Business.BusinessOperationCountryCode) + "" +  c.FaxingAmount + Common.Common.GetCountryCurrency(c.NonCardReciever.Business.BusinessOperationCountryCode)
                          }).OrderByDescending(x => x.TransactionDateTime).ToList();
            return result;
        }

        public DB.MerchantNonCardReceiverDetails GetReceiversDetails(int receiverId)
        {


            var result = dbContext.MerchantNonCardReceiverDetail.Where(x => x.Id == receiverId).FirstOrDefault();

            return result;
        }
    }
}