
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Areas.Mobile.Common;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Areas.KiiPayBusiness.Services;

namespace FAXER.PORTAL.Services
{
    public class STopUpToSupplier
    {
        DB.FAXEREntities dbContext = null;
        public STopUpToSupplier()
        {
            dbContext = new DB.FAXEREntities();
        }
        public SenderTopUpAnAccountVM GetSenderTopUpAnAccount()
        {
            SenderTopUpAnAccountVM vm = new SenderTopUpAnAccountVM();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderTopUpAnAccount != null)
            {
                vm = Common.FaxerSession.SenderTopUpAnAccount;
            }

            return vm;

        }
        public void SetSenderTopUpAnAccount(SenderTopUpAnAccountVM vm)
        {
            Common.FaxerSession.SenderTopUpAnAccount = vm;
        }
        public SenderTopUpSupplierAbroadVm GetSenderTopUpSupplierLocal()
        {
            SenderTopUpSupplierAbroadVm vm = new SenderTopUpSupplierAbroadVm();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderTopUpAnLocalAccount != null)
            {
                vm = Common.FaxerSession.SenderTopUpAnLocalAccount;
            }

            return vm;

        }
        public void SetSenderTopUpSupplierLocal(SenderTopUpSupplierAbroadVm vm)
        {
            Common.FaxerSession.SenderTopUpAnLocalAccount = vm;
        }

       

        public SenderTopUpSupplierAbroadAbroadEnterAmontVM GetSenderTopUpSupplierAbroadAbroadEnterAmont()
        {
            SenderTopUpSupplierAbroadAbroadEnterAmontVM vm = new SenderTopUpSupplierAbroadAbroadEnterAmontVM();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderTopUpSupplierAbroadAbroadEnterAmont != null)
            {
                vm = Common.FaxerSession.SenderTopUpSupplierAbroadAbroadEnterAmont;
            }

            return vm;

        }

        public void SetSenderTopUpSupplierAbroadAbroadEnterAmont(SenderTopUpSupplierAbroadAbroadEnterAmontVM vm)
        {
            Common.FaxerSession.SenderTopUpSupplierAbroadAbroadEnterAmont = vm;
        }

        public TopUpSupplierEnterAmountVM GetAgentTopUpSupplierEnterAmount()
        {
            TopUpSupplierEnterAmountVM vm = new TopUpSupplierEnterAmountVM();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.AgentSession.AgentTopUpSupplierEnterAmount != null)
            {
                vm = Common.AgentSession.AgentTopUpSupplierEnterAmount;
            }

            return vm;

        }

        public void SetAgentTopUpSupplierEnterAmount(TopUpSupplierEnterAmountVM vm)
        {
            Common.AgentSession.AgentTopUpSupplierEnterAmount = vm;
        }
        public TopUpAnAccountViewModel GetTopUpAnAccountViewModel()
        {
            TopUpAnAccountViewModel vm = new TopUpAnAccountViewModel();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.AgentSession.TopUpAnAccountViewModel != null)
            {
                vm = Common.AgentSession.TopUpAnAccountViewModel;
            }

            return vm;

        }

        public void SetTopUpAnAccountViewModel(TopUpAnAccountViewModel vm)
        {
            Common.AgentSession.TopUpAnAccountViewModel = vm;
        }
        public ServiceResult<TopUpToSupplier> Add(TopUpToSupplier model)
        {
            dbContext.TopUpToSupplier.Add(model);
            dbContext.SaveChanges();
            //#region Notification Section 
            
            //var payer = dbContext.FaxerInformation.Where(x => x.Id == model.PayerId).FirstOrDefault();
            //var supplier = dbContext.Suppliers.Where(x => x.Id == model.SuplierId).FirstOrDefault();
            //var KiiPayBusinessInfo = dbContext.KiiPayBusinessInformation.Where(x => x.Id == supplier.KiiPayBusinessInformationId).FirstOrDefault();
            //DB.Notification notification = new DB.Notification()
            //{
            //    SenderId = model.PayerId,
            //    ReceiverId = KiiPayBusinessInfo.Id,
            //    Amount = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(model.PayingCountry)) + " " + model.SendingAmount,
            //    CreationDate = DateTime.Now,
            //    Title = DB.Title.KiiPayWalletWithdrawal,
            //    Message = "Wallet No :" + KiiPayBusinessInfo.BusinessMobileNo,
            //    NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
            //    NotificationSender = DB.NotificationFor.Agent,
            //    Name =payer.FirstName,
            //};
            //KiiPayBusinessCommonServices kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            //kiiPayBusinessCommonServices.SendNotification(notification);
            //#endregion
            return new ServiceResult<TopUpToSupplier>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }

        public SenderTopUpAnAccountVM GetInformationFromMobileNo(string mobileNo)
        {
            var data = (from c in dbContext.TopUpToSupplier.Where(x => x.WalletNo == mobileNo).ToList()
                        select new SenderTopUpAnAccountVM()
                        {
                            WalletNo = c.WalletNo,
                            Country = Common.Common.GetCountryCodeByCountryName(c.SupplierCountry),
                            Supplier = getsuppliernamebysupplierid(c.SuplierId)
                        }).FirstOrDefault();

            return data;
        }

        private string getsuppliernamebysupplierid(int SuplierId)
        {
            var result = dbContext.Suppliers.ToList().Where(x => x.Id == SuplierId).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault();
            return result;
        }


        public ServiceResult<SupplierStandingOrderPayment> AddSupplierStandingOrderPayment(SupplierStandingOrderPayment model)
        {
            dbContext.SupplierStandingOrderPayment.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<SupplierStandingOrderPayment>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }


        public ServiceResult<IQueryable<TopUpToSupplier>> List()
        {
            return new ServiceResult<IQueryable<TopUpToSupplier>>()
            {
                Data = dbContext.TopUpToSupplier,
                Status = ResultStatus.OK
            };

        }


        public ServiceResult<TopUpToSupplier> Update(TopUpToSupplier model)
        {
            dbContext.Entry<TopUpToSupplier>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<TopUpToSupplier>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        

        public SenderTopUpSupplierAbroadVm GetInformationFormMobileOfLocalSupplier(string mobileNo)
        {
            var data = (from c in dbContext.TopUpToSupplier.Where(x => x.WalletNo == mobileNo).ToList()
                        select new SenderTopUpSupplierAbroadVm()
                        {
                            WalletNo = c.WalletNo,
                            Supplier = getsuppliernamebysupplierid(c.SuplierId)
                        }).FirstOrDefault();

            return data;
        }

        public ServiceResult<int> Remove(TopUpToSupplier model)
        {
            dbContext.TopUpToSupplier.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

        public void SetSenderTopUpSupplierAbroadVm(SenderTopUpSupplierAbroadVm vm)
        {
            Common.FaxerSession.SenderTopUpSupplierAbroadVm = vm;
        }

        public SenderTopUpSupplierAbroadVm GetSenderTopUpSupplierAbroadVm()
        {
            SenderTopUpSupplierAbroadVm vm = new SenderTopUpSupplierAbroadVm();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderTopUpSupplierAbroadVm != null)
            {
                vm = Common.FaxerSession.SenderTopUpSupplierAbroadVm;
            }

            return vm;

        }

        public TopUpToSupplier TopUpToSupplier(int TransactionId)
        {
            var TranscationInfo = dbContext.TopUpToSupplier.Where(x => x.Id == TransactionId).FirstOrDefault();

            return TranscationInfo;

        }
    }
}