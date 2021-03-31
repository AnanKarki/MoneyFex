using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class BankAccountDepositServices
    {
        FAXEREntities dbContext = null;
        public BankAccountDepositServices()
        {
            dbContext = new FAXEREntities();

        }


        public List<DropDownViewModel> getRecentAccountNumberNational()
        {
            var result = (from c in dbContext.KiiPayPersonalToBankAccountTransfer.Where(x => x.ReceivingCountry.Trim().ToLower() == Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim().ToLower()).ToList()
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.BankAccountNumber,
                              Name = c.BankAccountNumber
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> getRecentAccountNumberInternational()
        {
            var result = (from c in dbContext.KiiPayPersonalToBankAccountTransfer.Where(x => x.ReceivingCountry.Trim().ToLower() != Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim().ToLower()).ToList()
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.BankAccountNumber,
                              Name = c.BankAccountNumber
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> getBanksList()
        {
            var result = new List<DropDownViewModel>();
            result.Add(new DropDownViewModel() { Id = 1, Code = "SBI", Name = "State Bank Of India" });
            result.Add(new DropDownViewModel() { Id = 2, Code = "NBl", Name = "Nabil Bank" });
            result.Add(new DropDownViewModel() { Id = 2, Code = "SBL", Name = "Siddhartha Bank Limited" });
            result.Add(new DropDownViewModel() { Id = 4, Code = "GIME", Name = "Global IME Bank Ltd." });
            return result;
        }

        public List<DropDownViewModel> getBranchesList()
        {
            var result = new List<DropDownViewModel>();
            result.Add(new DropDownViewModel() { Id = 1, Code = "KTM", Name = "Kathmandu" });
            result.Add(new DropDownViewModel() { Id = 2, Code = "PKR", Name = "Pokhara" });
            result.Add(new DropDownViewModel() { Id = 2, Code = "DL", Name = "Dhulikhel" });
            result.Add(new DropDownViewModel() { Id = 4, Code = "BTK", Name = "Bhaktapur" });
            return result;
        }

        public bool savePaymentTransferData()
        {
            if (Common.CardUserSession.PayToBankAccountSession != null)
            {
                //deducting amount from kiipaywallet
                var kiiPayWalletData = dbContext.KiiPayPersonalWalletInformation.Find(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId);
                if (kiiPayWalletData != null)
                {
                    kiiPayWalletData.CurrentBalance = kiiPayWalletData.CurrentBalance - Common.CardUserSession.PayToBankAccountSession.TotalAmount;
                    dbContext.Entry(kiiPayWalletData).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    var data = new DB.KiiPayPersonalToBankAccountTransfer()
                    {
                        KiiPayPersonalWalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId,
                        SendingCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                        ReceivingCountry = Common.CardUserSession.PayToBankAccountSession.ReceivingCountry,
                        AccountOwnerName = Common.CardUserSession.PayToBankAccountSession.AccountOwnerName,
                        BankAccountNumber = Common.CardUserSession.PayToBankAccountSession.BankAccountNumber,
                        AccountHolderPhoneNo = Common.CardUserSession.PayToBankAccountSession.AccountHolderPhoneNumber,
                        BankId = Common.CardUserSession.PayToBankAccountSession.BankId,
                        BankBranchId = Common.CardUserSession.PayToBankAccountSession.BranchId,
                        BankBranchCode = Common.CardUserSession.PayToBankAccountSession.BranchCode,
                        SendingAmount = Common.CardUserSession.PayToBankAccountSession.SendingAmount,
                        ReceivingAmount = Common.CardUserSession.PayToBankAccountSession.ReceivingAmount,
                        Fee = Common.CardUserSession.PayToBankAccountSession.Fee,
                        SmsFee = Common.CardUserSession.PayToBankAccountSession.SmsFee,
                        ExchangeRate = Common.CardUserSession.PayToBankAccountSession.ExchangeRate,
                        TotalAmount = Common.CardUserSession.PayToBankAccountSession.TotalAmount,
                        PaymentType = Common.CardUserSession.PayToBankAccountSession.PaymentType,
                        TransactionDate = DateTime.Now
                    };
                    dbContext.KiiPayPersonalToBankAccountTransfer.Add(data);
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }
    }
}