using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderPayForServices
    {
        DB.FAXEREntities dbContext = null;
        public SSenderPayForServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<DropDownViewModel> GetRecentlyPaidInternationalServices(string country)
        {
            //List<DropDownViewModel> list = new List<DropDownViewModel>();
            //list.Add(new DropDownViewModel()
            //{
            //    Id = 1,
            //    Name = "ABC"
            //});

            //list.Add(new DropDownViewModel()
            //{
            //    Id = 2,
            //    Name = "CDE"
            //});

            var data = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id && x.SenderInformation.Country == country)
                        join d in dbContext.KiiPayBusinessInformation on c.KiiPayBusinessInformationId equals d.Id
                        select new DropDownViewModel()
                        {
                            Id = d.Id,
                            Name = d.BusinessMobileNo
                        }).ToList();
            
            return data;
        }

        internal KiiPayBusinessInformation GetBusinessInfo(string businessMobileNo)
        {
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == businessMobileNo).FirstOrDefault();
            return result;
                 
        }

        public void SetSenderPayForGoodsAndServices(SenderPayForGoodsAndServicesVM vm)
        {

            Common.FaxerSession.SenderPayForGoodsAndServices = vm;
        }

        public SenderPayForGoodsAndServicesVM GetSenderPayForGoodsAndServices()
        {

            SenderPayForGoodsAndServicesVM vm = new SenderPayForGoodsAndServicesVM();

            if (Common.FaxerSession.SenderPayForGoodsAndServices != null)
            {

                vm = Common.FaxerSession.SenderPayForGoodsAndServices;
            }
            return vm;
        }

        public LoggedUser GetLoggedUserData()
        {

            LoggedUser vm = new LoggedUser();

            if (Common.FaxerSession.LoggedUser != null)
            {

                vm = Common.FaxerSession.LoggedUser;
            }
            return vm;
        }

        public void SetEnterAmount(SenderMobileEnrterAmountVm vm)
        {

            Common.FaxerSession.SenderMobileEnrterAmount = vm;
        }

        public SenderMobileEnrterAmountVm GetEnterAmount()
        {

            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();

            if (Common.FaxerSession.SenderMobileEnrterAmount != null)
            {

                vm = Common.FaxerSession.SenderMobileEnrterAmount;
            }
            return vm;
        }


        public PaymentMethodViewModel GetPaymentMethod()
        {

            PaymentMethodViewModel vm = new PaymentMethodViewModel();

            if (Common.FaxerSession.PaymentMethodViewModel != null)
            {

                vm = Common.FaxerSession.PaymentMethodViewModel;
            }
            return vm;

        }

        public void SetPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.FaxerSession.PaymentMethodViewModel = vm;
        }


        public void SetDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.FaxerSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetDebitCreditCardDetail()
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();

            if (Common.FaxerSession.CreditDebitDetails != null)
            {

                vm = Common.FaxerSession.CreditDebitDetails;
            }
            return vm;
        }


        public void SetMoneyFexBankAccountDeposit(SenderMoneyFexBankDepositVM vm)
        {

            Common.FaxerSession.SenderMoneyFexBankDeposit = vm;
        }

        public SenderMoneyFexBankDepositVM GetMoneyFexBankAccountDeposit()
        {

            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            if (Common.FaxerSession.SenderMoneyFexBankDeposit != null)
            {

                vm = Common.FaxerSession.SenderMoneyFexBankDeposit;
            }
            return vm;
        }


        public List<DropDownViewModel> GetAddress()
        {
            List<DropDownViewModel> list = new List<DropDownViewModel>();

            DropDownViewModel vm1 = new DropDownViewModel()
            {
                Id = 1,
                Name = "Ktm"
            };

            DropDownViewModel vm2 = new DropDownViewModel()
            {
                Id = 1,
                Name = "Bkt"
            };

            list.Add(vm1);
            list.Add(vm2);
            return list;
        }



        public SenderLocalEnterAmountVM GetLocalPayForServiceEnterAmount()
        {

            SenderLocalEnterAmountVM vm = new SenderLocalEnterAmountVM();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderLocalEnterAmount;
            }
            return vm;
        }


        public void SetLocalPayForServiceEnterAmount(SenderLocalEnterAmountVM vm)
        {

            Common.FaxerSession.SenderLocalEnterAmount = vm;
        }

        public SenderAccountPaymentSummaryViewModel GetLocalPaymentSummary()
        {

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderAccountPaymentSummary;
            }
            return vm;
        }


        public void SetLocalPaymentSummary(SenderAccountPaymentSummaryViewModel vm)
        {

            Common.FaxerSession.SenderAccountPaymentSummary = vm;
        }

        public SenderPayForGoodsAndServicesVM GetInformationFromMobileNo(string MobileNo)
        {
            var data = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.PhoneNumber == MobileNo).ToList()
                        select new SenderPayForGoodsAndServicesVM()
                        {
                            ReceiverName = c.BusinessName,
                            CountryCode = c.BusinessCountry,
                            BusinessMobileNo = c.PhoneNumber,
                            RecentlyPaidBusiness = c.PhoneNumber,
                            
                            
                        }).FirstOrDefault();



            return data;
        }

    }
}