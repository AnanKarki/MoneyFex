
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderRegisteredWallets
    {
        DB.FAXEREntities dbContext = null;
        public SSenderRegisteredWallets()
        {
            dbContext = new DB.FAXEREntities();
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
        public ServiceResult<KiiPayPersonalWalletInformation> Add(KiiPayPersonalWalletInformation model)
        {
            dbContext.KiiPayPersonalWalletInformation.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<KiiPayPersonalWalletInformation>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<SenderKiiPayPersonalAccount> AddPersonalAccount(SenderKiiPayPersonalAccount model)
        {
            dbContext.SenderKiiPayPersonalAccount.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<SenderKiiPayPersonalAccount>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<IQueryable<KiiPayPersonalWalletInformation>> List()
        {
            return new ServiceResult<IQueryable<KiiPayPersonalWalletInformation>>()
            {
                Data = dbContext.KiiPayPersonalWalletInformation,
                Status = ResultStatus.OK
            };

        }

        public ServiceResult<IQueryable<SenderKiiPayPersonalAccount>> ListofSender()
        {
            return new ServiceResult<IQueryable<SenderKiiPayPersonalAccount>>()
            {
                Data = dbContext.SenderKiiPayPersonalAccount,
                Status = ResultStatus.OK
            };

        }


        public ServiceResult<KiiPayPersonalWalletInformation> Update(KiiPayPersonalWalletInformation model)
        {
            dbContext.Entry<KiiPayPersonalWalletInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<KiiPayPersonalWalletInformation>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<SenderKiiPayPersonalAccount> UpdateSender(SenderKiiPayPersonalAccount model)
        {
            //dbContext.Entry<SenderKiiPayPersonalAccount>(model).State = EntityState.Modified;
            dbContext.Set<SenderKiiPayPersonalAccount>().AddOrUpdate(model);
            dbContext.SaveChanges();
            return new ServiceResult<SenderKiiPayPersonalAccount>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<int> Remove(KiiPayPersonalWalletInformation model)
        {
            dbContext.KiiPayPersonalWalletInformation.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

        public  SenderControlWalletUsageAmountVM GetSenderControlWalletUsageAmount()
        {


            var model = new SenderControlWalletUsageAmountVM();
            if (Common.FaxerSession.SenderControlWalletUsageAmountVM != null) {

                model = Common.FaxerSession.SenderControlWalletUsageAmountVM;
            }

            return model;
        }

        

    }
}