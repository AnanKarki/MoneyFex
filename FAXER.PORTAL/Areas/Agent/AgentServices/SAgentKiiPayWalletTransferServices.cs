using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SAgentKiiPayWalletTransferServices
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        public List<Common.DropDownViewModel> getRecentPhoneNumbers()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).ToList()
                                  select new Common.DropDownViewModel()
                                  {
                                      Id = c.Id,
                                      Code = c.MobileNo,
                                      Name = c.MobileNo
                                  }).ToList();
                    return result;
                }
            }
            return null;
        }
        public List<Common.DropDownViewModel> getRecentNumbers(string Country = "", int PayingAgentStaffId = 0)
        {
            var data = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.ReceivingCountry == Country).ToList();

            if (PayingAgentStaffId != 0)
            {
                data = data.Where(x => x.PayingStaffId == PayingAgentStaffId).ToList();
            }

            var result = (from c in data
                          select new Common.DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.TransferToMobileNo,
                              Name = c.TransferToMobileNo
                          }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }

        public SendMoneToKiiPayWalletViewModel getFaxerInfo(string AccountNoORPHoneNo, SendMoneToKiiPayWalletViewModel vm)
        {
            var data = (from c in dbContext.FaxerInformation.Where(x => x.Email == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).ToList()
                        select new Models.SendMoneToKiiPayWalletViewModel()
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            MiddleName = c.MiddleName,
                            LastName = c.LastName,
                            AddressLine1 = c.Address1,
                            Country = c.Country,
                            ExpiryDate = c.IdCardExpiringDate,
                            IdNumber = c.IdCardNumber,
                            IssuingCountry = c.IssuingCountry,
                            IdType = c.IdCardType == null ? 0 : Int32.Parse(c.IdCardType),
                            Email = c.Email,
                            MobileNo = c.PhoneNumber,
                            DOB = c.DateOfBirth,
                            Searched = true,
                            CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                            SenderFullName = c.FirstName + " " + c.MiddleName + " " + c.LastName

                        }).FirstOrDefault();

            return data;
        }
        public List<ReceiversDetails> getExistingReceiver(int faxerId)
        {
            var data = dbContext.ReceiversDetails.Where(x => x.FaxerID == faxerId).ToList();

            //var data = (from c in dbContext.ReceiversDetails.Where(x => x.FaxerID == faxerid).ToList()
            //            select new ReceiversDetails()
            //            {
            //                Id  = c.Id ,
            //                FirstName = c.FirstName + " " + c.MiddleName + " " + c.LastName
            //            }).ToList();
            return data;

        }
        public void SetAdminPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.AdminSession.PaymentMethodViewModel = vm;
        }

        public PaymentMethodViewModel GetAdminPaymentMethod()
        {

            PaymentMethodViewModel vm = new PaymentMethodViewModel();

            if (Common.AdminSession.PaymentMethodViewModel != null)
            {

                vm = Common.AdminSession.PaymentMethodViewModel;
            }
            return vm;

        }
        public void SetSendMoneToKiiPayWalletViewModel(SendMoneToKiiPayWalletViewModel vm)
        {

            Common.AgentSession.SendMoneToKiiPayWalletViewModel = vm;
        }
        public SendMoneToKiiPayWalletViewModel GetSendMoneToKiiPayWalletViewModel()
        {

            SendMoneToKiiPayWalletViewModel vm = new SendMoneToKiiPayWalletViewModel();

            if (Common.AgentSession.SendMoneToKiiPayWalletViewModel != null)
            {


                vm = Common.AgentSession.SendMoneToKiiPayWalletViewModel;

            }
            return vm;
        }
        public void SetAdminSendMoneToKiiPayWalletViewModel(SendMoneToKiiPayWalletViewModel vm)
        {

            Common.AdminSession.SendMoneToKiiPayWalletViewModel = vm;
        }
        public SendMoneToKiiPayWalletViewModel GetAdminSendMoneToKiiPayWalletViewModel()
        {

            SendMoneToKiiPayWalletViewModel vm = new SendMoneToKiiPayWalletViewModel();

            if (Common.AdminSession.SendMoneToKiiPayWalletViewModel != null)
            {


                vm = Common.AdminSession.SendMoneToKiiPayWalletViewModel;

            }
            return vm;
        }

        public KiiPayReceiverDetailsInformationViewModel GetReceiverDetailsFromReceiptNumber(string ReceiptNumber)
        {

            var result = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.ReceiptNumber == ReceiptNumber).ToList()
                          select new KiiPayReceiverDetailsInformationViewModel()
                          {
                              Country = c.ReceivingCountry,
                              CountryPhoneCode = c.ReceivingCountry,
                              MobileNo = c.TransferToMobileNo,
                              PreviousMobileNumber = c.TransferToMobileNo,
                              ReceiverFullName = c.KiiPayPersonalWalletInformation.FirstName,

                          }).FirstOrDefault();
            return result;
        }
        public void SetKiiPayReceiverDetailsInformationViewModel(KiiPayReceiverDetailsInformationViewModel vm)
        {


            Common.AgentSession.KiiPayReceiverDetailsInformationViewModel = vm;

        }
        public KiiPayReceiverDetailsInformationViewModel GetKiiPayReceiverDetailsInformationViewModel()
        {
            KiiPayReceiverDetailsInformationViewModel vm = new KiiPayReceiverDetailsInformationViewModel();
            if (Common.AgentSession.KiiPayReceiverDetailsInformationViewModel != null)
            {

                vm = Common.AgentSession.KiiPayReceiverDetailsInformationViewModel;
            }

            return vm;
        }

        public KiiPayReceiverDetailsInformationViewModel GetKiiPayReceiverDetailsByMobileNo(string mobileNo)
        {
            KiiPayReceiverDetailsInformationViewModel vm = new KiiPayReceiverDetailsInformationViewModel();
            var kiiPayReceiverDetails = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.TransferToMobileNo == mobileNo).FirstOrDefault();
            if (kiiPayReceiverDetails != null)
            {
                vm = new KiiPayReceiverDetailsInformationViewModel()
                {
                    Country = kiiPayReceiverDetails.ReceivingCountry,
                    CountryPhoneCode = Common.Common.GetCountryPhoneCode(kiiPayReceiverDetails.ReceivingCountry),
                    MobileNo = kiiPayReceiverDetails.TransferToMobileNo,
                    ReceiverFullName = kiiPayReceiverDetails.KiiPayPersonalWalletInformation.FirstName + " " +
                               (string.IsNullOrEmpty(kiiPayReceiverDetails.KiiPayPersonalWalletInformation.MiddleName) ? "" : 
                               kiiPayReceiverDetails.KiiPayPersonalWalletInformation.MiddleName + " ") +
                                kiiPayReceiverDetails.KiiPayPersonalWalletInformation.LastName,
                    
                };
            }
            return vm;
        }

        public void SetAdminKiiPayReceiverDetailsInformationViewModel(KiiPayReceiverDetailsInformationViewModel vm)
        {


            Common.AdminSession.KiiPayReceiverDetailsInformationViewModel = vm;

        }
        public KiiPayReceiverDetailsInformationViewModel GetAdminKiiPayReceiverDetailsInformationViewModel()
        {
            KiiPayReceiverDetailsInformationViewModel vm = new KiiPayReceiverDetailsInformationViewModel();
            if (Common.AdminSession.KiiPayReceiverDetailsInformationViewModel != null)
            {

                vm = Common.AdminSession.KiiPayReceiverDetailsInformationViewModel;
            }

            return vm;
        }
        public DB.KiiPayPersonalWalletInformation getReceiverDetails(string MobileNo)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();
            return result;

        }
        public void SetKiiPayEnterAmount(SendMoneyToKiiPayEnterAmountViewModel vm)
        {
            SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
            if (Common.AgentSession.IsTransferFromCalculateHowMuch == true)
            {
                var dashboarddata = _senderKiiPayServices.GetCommonEnterAmount();
                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrency = Common.Common.GetCountryCurrency(dashboarddata.SendingCountryCode);
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrency = Common.Common.GetCountryCurrency(dashboarddata.ReceivingCountryCode);
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TheyReceive = dashboarddata.TotalAmount;
                    vm.TotalAmount = dashboarddata.TotalAmount;
                    vm.AgentCommission = dashboarddata.AgentCommission;
                    Common.AgentSession.SendMoneyToKiiPayEnterAmountViewModel = vm;
                }
            }
            else
            {
                Common.AgentSession.SendMoneyToKiiPayEnterAmountViewModel = vm;
            }

        }


        public SendMoneyToKiiPayEnterAmountViewModel GetKiiPayEnterAmount()
        {

            SendMoneyToKiiPayEnterAmountViewModel vm = new SendMoneyToKiiPayEnterAmountViewModel();

            if (Common.AgentSession.SendMoneyToKiiPayEnterAmountViewModel != null)
            {


                vm = Common.AgentSession.SendMoneyToKiiPayEnterAmountViewModel;

            }
            return vm;
        }
        public void SetAdminKiiPayEnterAmount(SendMoneyToKiiPayEnterAmountViewModel vm)
        {

            Common.AdminSession.SendMoneyToKiiPayEnterAmountViewModel = vm;

        }


        public SendMoneyToKiiPayEnterAmountViewModel GetAdminKiiPayEnterAmount()
        {

            SendMoneyToKiiPayEnterAmountViewModel vm = new SendMoneyToKiiPayEnterAmountViewModel();

            if (Common.AdminSession.SendMoneyToKiiPayEnterAmountViewModel != null)
            {


                vm = Common.AdminSession.SendMoneyToKiiPayEnterAmountViewModel;

            }
            return vm;
        }
        public DB.TopUpSomeoneElseCardTransaction TopUpSomeoneElseCardTransaction(Models.SendMoneyToKiiPayEnterAmountViewModel vm)
        {
            var agentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            SendMoneyToKiiPayEnterAmountViewModel model = new SendMoneyToKiiPayEnterAmountViewModel();
            var SenderExist = GetSendMoneToKiiPayWalletViewModel();
            SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
            string accountNo = "AMF" + faxerSignUpService.GetNewAccount(6);

            string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(SenderExist.Country);
            if (SenderExist.Id == 0)
            {
                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = SenderExist.FirstName,
                    MiddleName = SenderExist.MiddleName,
                    LastName = SenderExist.LastName,
                    Address1 = SenderExist.AddressLine1,
                    Country = Common.AgentSession.AgentInformation.CountryCode,
                    Email = SenderExist.Email,
                    PhoneNumber = FaxerCountryPhoneCode + SenderExist.MobileNo,
                    IdCardNumber = SenderExist.IdNumber,
                    IdCardType = SenderExist.IdType.ToString(),
                    IssuingCountry = SenderExist.IssuingCountry,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = SenderExist.DOB,
                    GGender = SenderExist.Gender.ToInt(),
                    Address2 = SenderExist.AddressLine2,
                    City = SenderExist.City,


                };

                dbContext.FaxerInformation.Add(FaxerDetails);
                dbContext.SaveChanges();
            }
            else
            {
                var sender = dbContext.FaxerInformation.Where(x => x.Id == SenderExist.Id).FirstOrDefault();
                sender.IdCardNumber = SenderExist.IdNumber;
                sender.IdCardType = SenderExist.IdType.ToString();
                sender.IdCardExpiringDate = SenderExist.ExpiryDate;
                sender.IssuingCountry = SenderExist.IssuingCountry;
                dbContext.Entry(sender).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }

            var SenderInfo = dbContext.FaxerInformation.Where(x => x.Email == SenderExist.Email).FirstOrDefault();

            //CashPickUpReceiverDetailsInformationViewModel receiver = new CashPickUpReceiverDetailsInformationViewModel();
            var receiver = GetKiiPayReceiverDetailsInformationViewModel();
            var kiiPayPersonalWallet = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == receiver.MobileNo).FirstOrDefault();
            if (SenderInfo != null)
            {
                int faxerid = SenderInfo.Id;
                string ReceiverCountryPhoneCode = Common.Common.GetCountryPhoneCode(receiver.Country);

            }
            if (kiiPayPersonalWallet != null)
            {
                // Top Up Someone Else Card Transaction

                decimal MFRate = Common.Common.GetMFRate(SenderExist.Country, receiver.Country, TransactionTransferMethod.KiiPayWallet);

                var receiptNumber = GetNewAgentMoneyTransferReceipt();
                DB.TopUpSomeoneElseCardTransaction TopUpSomeoneElseCardTransaction = new DB.TopUpSomeoneElseCardTransaction()
                {
                    FaxingAmount = vm.SendingAmount,
                    FaxingFee = vm.Fee,
                    ExchangeRate = vm.ExchangeRate,
                    TransactionDate = DateTime.Now,
                    RecievingAmount = vm.ReceivingAmount,
                    FaxerId = SenderInfo.Id, /*SenderExist.Id == null ? 0 : (int)SenderExist.Id,*/
                    TransferToMobileNo = receiver.MobileNo,
                    TotalAmount = vm.TotalAmount,
                    SendingCountry = SenderExist.Country,
                    ReceivingCountry = receiver.Country,
                    ReceiptNumber = receiptNumber,
                    KiiPayPersonalWalletId = kiiPayPersonalWallet.Id,
                    PayingStaffId = agentId,
                    IsAutoPaymentTransaction = false,
                    AgentCommission = vm.AgentCommission,
                    PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                    Margin = Common.Common.GetMargin(MFRate, vm.ExchangeRate, vm.SendingAmount, vm.Fee),
                    MFRate = MFRate
                };

                dbContext.TopUpSomeoneElseCardTransaction.Add(TopUpSomeoneElseCardTransaction);
                dbContext.SaveChanges();

                if (AgentSession.AgentInformation.IsAUXAgent == true)
                {
                    var fundaccountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == AgentSession.AgentInformation.Id).FirstOrDefault();
                    fundaccountBalance.UpdateDateTime = DateTime.Now;
                    fundaccountBalance.TotalBalance = fundaccountBalance.TotalBalance - TopUpSomeoneElseCardTransaction.TotalAmount;
                    dbContext.Entry(fundaccountBalance).State = EntityState.Modified;
                    dbContext.SaveChanges();
                }

                #region Notification Section 

                var kiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == TopUpSomeoneElseCardTransaction.KiiPayPersonalWalletId).FirstOrDefault();
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = (int)TopUpSomeoneElseCardTransaction.PayingStaffId,
                    ReceiverId = kiiPayPersonalWalletInfo.Id,
                    Amount = Common.Common.GetCountryCurrency(kiiPayPersonalWalletInfo.CardUserCountry) + " " + TopUpSomeoneElseCardTransaction.FaxingAmount,
                    CreationDate = DateTime.Now,
                    Title = DB.Title.KiiPayWalletWithdrawal,
                    Message = "Wallet No :" + kiiPayPersonalWalletInfo.MobileNo,
                    NotificationReceiver = DB.NotificationFor.kiiPayPersonal,
                    NotificationSender = DB.NotificationFor.Agent,
                    Name = kiiPayPersonalWalletInfo.FirstName,
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToSenderKiiPayPersonalAccount(notification);
                #endregion
                // End
                return TopUpSomeoneElseCardTransaction;
            }
            else
            {
                return null;
            }
        }

        internal SendMoneToKiiPayWalletViewModel SetSenderDetails(CashPickupInformationViewModel vm)
        {
            SendMoneToKiiPayWalletViewModel model = new SendMoneToKiiPayWalletViewModel()
            {
                AddressLine1 = vm.AddressLine1,
                AddressLine2 = vm.AddressLine2,
                City = vm.City,
                Search = vm.Search,
                Country = vm.Country,
                CountryCode = vm.CountryCode,
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode),
                Day = vm.Day,
                DOB = vm.DOB,
                Email = vm.Email,
                ExpiryDate = vm.ExpiryDate,
                FirstName = vm.FirstName,
                Gender = vm.Gender,
                Id = vm.Id,
                IdNumber = vm.IdNumber,
                IdType = vm.IdType,
                IssuingCountry = vm.IssuingCountry,
                LastName = vm.LastName,
                MiddleName = vm.MiddleName,
                MobileNo = vm.MobileNo,
                Month = vm.Month,
                PostCode = vm.PostCode,
                Searched = vm.Searched,
                SenderFullName = vm.SenderFullName,
                Year = vm.Year,

            };
            return model;
        }

        public void SetDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.FaxerSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetDebitCreditCardDetail(string CountryCode)
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(CountryCode);

            if (Common.FaxerSession.CreditDebitDetails != null)
            {

                vm = Common.FaxerSession.CreditDebitDetails;
            }
            return vm;
        }


        public TopUpSomeoneElseCardTransaction TopUpSomeoneElseCardTransactionInfo(int TransactionId)
        {
            var TranscationInfo = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();

            return TranscationInfo;
        }
        public KiiPayPersonalWalletInformation kiiPayPersonalWalletInfo(int TransactionId)
        {
            var TranscationInfo = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();
            var kiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == TranscationInfo.KiiPayPersonalWalletId).FirstOrDefault();

            return kiiPayPersonalWalletInfo;
        }
        internal string GetNewAgentMoneyTransferReceipt()
        {

            //this code should be unique and random with 8 digit length
            var val = "CD" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = GetNewAgentMoneyTransferReceipt();
            }
            return val;
        }


    }
}