using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewPaySomeoneElseCardServices
    {
        DB.FAXEREntities dbContext = null;
        public ViewPaySomeoneElseCardServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.ViewPaySomeoneElseCardViewModel> GetTransactionDetails(string CountryCode, string City)
        {


            var data = dbContext.TopUpSomeoneElseCardTransaction.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {


                data = (from c in dbContext.TopUpSomeoneElseCardTransaction
                        join d in dbContext.FaxerInformation.Where(x => x.Country.ToLower() == CountryCode && x.City.ToLower() == City.ToLower())
                        on c.FaxerId equals d.Id
                        select c).ToList();
            }
            else if(!string.IsNullOrEmpty(CountryCode))
            {

                data = (from c in dbContext.TopUpSomeoneElseCardTransaction
                        join d in dbContext.FaxerInformation.Where(x => x.Country.ToLower() == CountryCode)
                        on c.FaxerId equals d.Id
                        select c).ToList();
            }

            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.FaxerId equals d.Id
                          select new ViewModels.ViewPaySomeoneElseCardViewModel()
                          {
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderCity = d.City,
                              SenderAccountNo = d.AccountNo,
                              SenderCountry = Common.Common.GetCountryName(d.Country),
                              CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              CardUserEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                              CardUserMFTCCardNO = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                              ReceivingAmount =c.RecievingAmount,
                              TopUpAmount = c.FaxingAmount,
                              TopUpFee = c.FaxingFee,
                              TopUpReference = c.TopUpReference,
                              TransactionId = c.Id,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              TopUpBy = Enum.GetName( typeof(DB.PayedBy) , c.PayedBy)
                              

                          }).ToList();

            return result;

        }
    }
}