using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Agent.Models;
using System.Data.Entity;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Areas.KiiPayBusiness.Services;

namespace FAXER.PORTAL.Services
{
    public class SPayBill
    {
        DB.FAXEREntities dbContext = null;
        public SPayBill()
        {
            dbContext = new DB.FAXEREntities();
        }
        public SenderPayingSupplierAbroadReferenceOneVM GetSenderPayingSupplierAbroadReferenceOne()
        {
            SenderPayingSupplierAbroadReferenceOneVM vm = new SenderPayingSupplierAbroadReferenceOneVM();
           

            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderPayingSupplierAbroadReferenceOne != null)
            {
                vm = Common.FaxerSession.SenderPayingSupplierAbroadReferenceOne;
            }

            return vm;

        }

        public void SetSenderPayingSupplierAbroadReferenceOne(SenderPayingSupplierAbroadReferenceOneVM vm)
        {
            Common.FaxerSession.SenderPayingSupplierAbroadReferenceOne = vm;
        }

        public PayingSupplierReferenceViewModel GetAgentPayingSupplierReference()
        {
            PayingSupplierReferenceViewModel vm = new PayingSupplierReferenceViewModel();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.AgentSession.AgentPayingSupplierReference != null)
            {
                vm = Common.AgentSession.AgentPayingSupplierReference;
            }

            return vm;

        }
        public void SetAgentPayingSupplierReference(PayingSupplierReferenceViewModel vm)
        {
            Common.AgentSession.AgentPayingSupplierReference = vm;
        }


        public PayMonthlyBillViewModel GetPayMonthlyBillViewModel()
        {
            PayMonthlyBillViewModel vm = new PayMonthlyBillViewModel();


            // if user try to go back get the session value of business PersonalDetail  
            if (Common.AgentSession.AgentPayingSupplierReference != null)
            {
                vm = Common.AgentSession.PayMonthlyBillViewModel;
            }

            return vm;

        }
        public void SetPayMonthlyBillViewModel(PayMonthlyBillViewModel vm)
        {
            Common.AgentSession.PayMonthlyBillViewModel = vm;
        }
        public ServiceResult<PayBill> Add(PayBill model)
        {
            dbContext.PayBill.Add(model);
            dbContext.SaveChanges();
            #region Notification Section 
            var supplier = dbContext.Suppliers.Where(x => x.Id == model.SupplierId).FirstOrDefault();
            var KiiPayBusinessInfo = dbContext.KiiPayBusinessInformation.Where(x => x.Id == supplier.KiiPayBusinessInformationId).FirstOrDefault();
            DB.Notification notification = new DB.Notification()
            {
                SenderId = model.PayerId,
                ReceiverId = KiiPayBusinessInfo.Id,
                Amount = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(model.PayerCountry)) + " " + model.SendingAmount,
                CreationDate = DateTime.Now,
                Title = DB.Title.KiiPayWalletWithdrawal,
                Message = "Wallet No :" + KiiPayBusinessInfo.BusinessMobileNo,
                NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                NotificationSender = DB.NotificationFor.Agent,
                Name = KiiPayBusinessInfo.BusinessName,
            };
            KiiPayBusinessCommonServices kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            kiiPayBusinessCommonServices.SendNotification(notification);
            #endregion
            return new ServiceResult<PayBill>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }

        public ServiceResult<IQueryable<PayBill>> List()
        {
            return new ServiceResult<IQueryable<PayBill>>()
            {
                Data = dbContext.PayBill,
                Status = ResultStatus.OK
            };

        }
       

        public ServiceResult<PayBill> Update(PayBill model)
        {
            dbContext.Entry<PayBill>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<PayBill>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<int> Remove(PayBill model)
        {
            dbContext.PayBill.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

        public SenderPayMonthlyBillVM GetInformationFromMobileNo(string mobileNo)
        {
            var data = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.PhoneNumber == mobileNo).ToList()
                        select new SenderPayMonthlyBillVM()
                        {
                            SupplierCountryCode = c.BusinessCountry,
                        }).FirstOrDefault();



            return data;
        }
        public void SetSenderPayMonthlyBillVM(SenderPayMonthlyBillVM vm)
        {

            Common.FaxerSession.SenderPayMonthlyBillVM = vm;

        }

        public SenderPayMonthlyBillVM GetSenderPayMonthlyBillVM()
        {

            SenderPayMonthlyBillVM vm = new SenderPayMonthlyBillVM();

            if (Common.FaxerSession.SenderPayMonthlyBillVM != null)
            {

                vm = Common.FaxerSession.SenderPayMonthlyBillVM;
            }
            return vm;
        }


        public PayBill payBill(int TransactionId)
        {
            var Transaction = dbContext.PayBill.Where(x => x.Id == TransactionId).FirstOrDefault();
            return Transaction;
        }
    }

}