using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Agent.Models;
using System.Data.Entity;
using FAXER.PORTAL.Models;

namespace FAXER.PORTAL.Services
{
    public class SOtherMFTCCardAutoTopUpInformation
    {
        DB.FAXEREntities dbContext = null;
        public SOtherMFTCCardAutoTopUpInformation()
        {
            dbContext = new DB.FAXEREntities();
        }
        public void SetSenderAddKiiPayStandingOrder(SenderAutoPaymentAddViewModel vm)
        {

            Common.FaxerSession.SenderAddKiiPayStandingOrder = vm;

        }

        public SenderAutoPaymentAddViewModel GetSenderAddKiiPayStandingOrder()
        {

            SenderAutoPaymentAddViewModel vm = new SenderAutoPaymentAddViewModel();

            if (Common.FaxerSession.SenderAddKiiPayStandingOrder != null)
            {

                vm = Common.FaxerSession.SenderAddKiiPayStandingOrder;
            }
            return vm;
        }
        public ServiceResult<OtherMFTCCardAutoTopUpInformation> Add(OtherMFTCCardAutoTopUpInformation model)
        {
            dbContext.OtherMFTCCardAutoTopUpInformation.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<OtherMFTCCardAutoTopUpInformation>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }

        public ServiceResult<IQueryable<OtherMFTCCardAutoTopUpInformation>> List()
        {
            return new ServiceResult<IQueryable<OtherMFTCCardAutoTopUpInformation>>()
            {
                Data = dbContext.OtherMFTCCardAutoTopUpInformation,
                Status = ResultStatus.OK
            };

        }
        public ServiceResult<IQueryable<KiiPayPersonalWalletInformation>> ListofKiiPayPersonalWallet()
        {
            return new ServiceResult<IQueryable<KiiPayPersonalWalletInformation>>()
            {
                Data = dbContext.KiiPayPersonalWalletInformation,
                Status = ResultStatus.OK
            };

        }


        public ServiceResult<OtherMFTCCardAutoTopUpInformation> Update(OtherMFTCCardAutoTopUpInformation model)
        {
            dbContext.Entry<OtherMFTCCardAutoTopUpInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<OtherMFTCCardAutoTopUpInformation>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<int> Remove(OtherMFTCCardAutoTopUpInformation model)
        {
            dbContext.OtherMFTCCardAutoTopUpInformation.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }


    }
}