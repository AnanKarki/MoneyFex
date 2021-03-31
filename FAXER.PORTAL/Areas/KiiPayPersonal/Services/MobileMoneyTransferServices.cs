using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class MobileMoneyTransferServices
    {
        FAXEREntities dbContext = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public MobileMoneyTransferServices()
        {
            dbContext = new FAXEREntities();
            _commonServices = new KiiPayPersonalCommonServices();
        }



        public List<DropDownViewModel> getRecentMobileNumbersNational()
        {
            var result = (from c in dbContext.KiiPayPerosnalToMobileWalletTransfer.Where(x => x.ReceiverCountry.Trim().ToLower() == Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim().ToLower())
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.ReceiverMobileNumber,
                              Name = c.ReceiverMobileNumber
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> getRecentMobileNumbersInterNational()
        {
            var result = (from c in dbContext.KiiPayPerosnalToMobileWalletTransfer.Where(x => x.ReceiverCountry.Trim().ToLower() != Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim().ToLower())
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.ReceiverMobileNumber,
                              Name = c.ReceiverMobileNumber
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public bool saveMobileTransferNational()
        {
            if(Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                var data = new DB.KiiPayPerosnalToMobileWalletTransfer()
                {
                    KiiPayPersonalWalletInformationId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId,
                    ReceiverMobileNumber = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber,
                    ReceiverCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                    SendingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                    ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount,
                    TotalAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                    ExchangeRate = 1,
                    Fee = 0,
                    PaymentReference = "",
                    PaymentType = PaymentType.Local,
                    TransactionDate = DateTime.Now
                };
                dbContext.KiiPayPerosnalToMobileWalletTransfer.Add(data);
                dbContext.SaveChanges();
                Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _commonServices.getAvailableBalance();
                return true;

            }
            return false;
        }

        public bool saveMobileTransferInternational()
        {
            if(Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                var data = new DB.KiiPayPerosnalToMobileWalletTransfer()
                {
                    KiiPayPersonalWalletInformationId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId,
                    ReceiverMobileNumber = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber,
                    ReceiverCountry = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCountryCode,
                    SendingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                    ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount,
                    TotalAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                    ExchangeRate = Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate,
                    Fee = Common.CardUserSession.PayIntoAnotherWalletSession.Fee,
                    PaymentReference = "",
                    PaymentType = PaymentType.International,
                    TransactionDate = DateTime.Now
                };
                dbContext.KiiPayPerosnalToMobileWalletTransfer.Add(data);
                dbContext.SaveChanges();
                Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _commonServices.getAvailableBalance();
                return true;

            }
            return false;
        }
    }
}