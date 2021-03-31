using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderSendingBusinessOrderServices
    {
        DB.FAXEREntities dbContext = null;
        public SSenderSendingBusinessOrderServices()
        {
            dbContext = new DB.FAXEREntities();
        }
       

        public List<MobileNumberDropDown> GetMobileNumber()
        {
            var SenderId = Common.FaxerSession.LoggedUser == null ? 0 : Common.FaxerSession.LoggedUser.Id;
            var result = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == SenderId)
                          select new MobileNumberDropDown()
                          {
                              Id = c.Id,
                              MobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                          }
                      ).ToList();
            return result;
        }

        public void SetSenderAddKiiPayStandingOrder(SenderAutoPaymentAddViewModel vm)
        {
            Common.FaxerSession.SenderAddKiiPayStandingOrder = vm;
        }

        public SenderAutoPaymentAddViewModel GetSenderAddStandingOrder()
        {
            SenderAutoPaymentAddViewModel vm = new SenderAutoPaymentAddViewModel();
            if (Common.FaxerSession.SenderAddKiiPayStandingOrder != null)
            {
                vm = Common.FaxerSession.SenderAddKiiPayStandingOrder;
            }
            return vm;
        }
        public List<SenderSendingBusinessOrdersViewModel> GetGridList(string MobileNo = "")
        {

            var gridLst = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == MobileNo).ToList()
                           join e in dbContext.Country on c.KiiPayBusinessInformation.BusinessOperationCountryCode equals e.CountryCode
                           select new SenderSendingBusinessOrdersViewModel()
                           {
                               AutoAmount = c.AutoPaymentAmount.ToString(),
                               City = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                               Country = e.CountryName,
                               Id = c.Id,
                               WalletName = c.KiiPayBusinessInformation.BusinessName,
                               MobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                               FrequencyDetails = c.FrequencyDetails,
                               EnableAutoPayment  = c.EnableAutoPayment == true ? "Yes" : "No"
                           }).ToList(); 
            return gridLst;
        }

        public List<SenderKiiPayBusinessPaymentInformation> GetList()
        {
               var SenderId = Common.FaxerSession.LoggedUser == null ? 0 : Common.FaxerSession.LoggedUser.Id;

            var lst = dbContext.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == SenderId).ToList();
            return lst;

        }


        public SavedCard GetSavedCard()
        {
            var savedCard = dbContext.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            return savedCard;
        }
        public ServiceResult<SenderKiiPayBusinessPaymentInformation> Add(SenderKiiPayBusinessPaymentInformation model)
        {
            dbContext.FaxerMerchantPaymentInformation.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<SenderKiiPayBusinessPaymentInformation>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }

        public ServiceResult<SenderKiiPayBusinessPaymentInformation> Update(SenderKiiPayBusinessPaymentInformation model)
        {
            dbContext.Entry<SenderKiiPayBusinessPaymentInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<SenderKiiPayBusinessPaymentInformation>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<int> Remove(SenderKiiPayBusinessPaymentInformation model)
        {
            dbContext.FaxerMerchantPaymentInformation.Remove(model);
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
