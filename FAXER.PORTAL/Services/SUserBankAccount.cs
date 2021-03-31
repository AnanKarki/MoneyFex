using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SUserBankAccount
    {
        DB.FAXEREntities dbContext = null;
        public SUserBankAccount()
        {
            dbContext = new DB.FAXEREntities();
        }

        public ServiceResult<SavedBank> Add(SavedBank model)
        {
            dbContext.SavedBank.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<SavedBank>()
            {
                Data = model,
                Message = "Save",
                Status= ResultStatus.OK

            };
        }

        public ServiceResult<IQueryable<SavedBank>> List()
        {
            return new ServiceResult<IQueryable<SavedBank>>()
            {
                Data = dbContext.SavedBank,
                Status = ResultStatus.OK
            };

        }

       
        internal Bank GetBankCode(int bankId)
        {
            var code = dbContext.Bank.Where(x => x.Id == bankId).FirstOrDefault();
            return code;

        }
        public ServiceResult<IQueryable<SenderKiiPayPersonalAccount>> SenderKiiPayList()
        {
            return new ServiceResult<IQueryable<SenderKiiPayPersonalAccount>>()
            {
                Data = dbContext.SenderKiiPayPersonalAccount,
                Status = ResultStatus.OK
            };

        }



        public ServiceResult<SavedBank> Update (SavedBank model)
        {
            dbContext.Entry<SavedBank>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<SavedBank>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<int> Remove (SavedBank model)
        {
            dbContext.SavedBank.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message="Remove",
                Status = ResultStatus.OK
            };
        }


        public string SetMobilePinCode(string pinCode)
        {
            Common.FaxerSession.SentMobilePinCode = pinCode;
            return pinCode;
        }
       
       
        public string GetMobilePinCode()
        {
            // SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();
            var pinCode = "";
            if (Common.FaxerSession.SentMobilePinCode != null)
            {

                pinCode = Common.FaxerSession.SentMobilePinCode;
            }
            return pinCode;
        }
        
        public int SetBankId(int BankId)
        {
            Common.FaxerSession.BankId = BankId;
            return BankId;
        }

        public int GetBankId()
        {  
            int BankId = Common.FaxerSession.BankId;
            return BankId;
        }
        public string GetBankName(int bankId)
        {
          
            string bankName = dbContext.SavedBank.Where(x => x.Id == bankId).Select(x => x.BankName).FirstOrDefault();
            return bankName;
        }

        public string GetAccountNumber(int bankId)
        {
            string AccountNumber = dbContext.SavedBank.Where(x => x.Id == bankId).Select(x => x.AccountNumber).FirstOrDefault();
            return AccountNumber;
        }

        public decimal SetAmount( decimal Amount)
        {
            Common.FaxerSession.AmountToBeTransferred = Amount;
            return Amount;
        }

        public decimal GetAmount()
        {
            decimal Amount = Common.FaxerSession.AmountToBeTransferred;
            return Amount;
        }

        
    }
}