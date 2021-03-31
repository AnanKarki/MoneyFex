using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderFamilyAndFriends
    {
        DB.FAXEREntities dbContext = null;
        public SSenderFamilyAndFriends()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<DropDownViewModel> GetRegisteredWallets()
        {
            
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var data = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == senderId && x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Family).ToList();
            var result = (from c in data
                          select new DropDownViewModel()
                          {
                              Id = c.KiiPayPersonalWalletId,
                              Code = c.KiiPayPersonalWalletInformation.MobileNo,
                              Name  =c.KiiPayPersonalWalletInformation.MobileNo,
                              CountryCode = c.KiiPayPersonalWalletInformation.CardUserCountry
                          }).ToList();
            return result;
        }

        public void SetSenderTransferFamilyAndFriends(SenderTransferFamilyAndFriendsVm vm)
        {

            Common.FaxerSession.SenderTransferFamilyAndFriends = vm;

        }

        public SenderTransferFamilyAndFriendsVm GetSenderTransferFamilyAndFriends()
        {

            SenderTransferFamilyAndFriendsVm vm = new SenderTransferFamilyAndFriendsVm();

            if (Common.FaxerSession.SenderTransferFamilyAndFriends != null)
            {

                vm = Common.FaxerSession.SenderTransferFamilyAndFriends;
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


        public SenderMobileEnrterAmountVm GetReceiversDataFromWalletId(int walletId)
        {
            return new SenderMobileEnrterAmountVm()
            {
                ReceiverId = 1,
                ReceiverName = "Test Receiver",
                ReceivingCurrencyCode = Common.Common.GetCountryCurrency("INR"),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol("INR")
            };
        }

        public void SetSenderTransferFamilyAndFriendsEnrterAmount(SenderMobileEnrterAmountVm vm)
        {

            Common.FaxerSession.SenderMobileEnrterAmount = vm;
        }

        public SenderMobileEnrterAmountVm GetSenderTransferFamilyAndFriendsEnrterAmount()
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
    }
}