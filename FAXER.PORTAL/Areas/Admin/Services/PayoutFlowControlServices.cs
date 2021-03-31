using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class PayoutFlowControlServices
    {
        FAXEREntities dbContext = null;
        public PayoutFlowControlServices()
        {
            dbContext = new FAXEREntities();
        }
        public List<PayoutFlowControlViewModel> GetPayoutFlowControlData()
        {

            List<PayoutFlowControlViewModel> data = new List<PayoutFlowControlViewModel>();

            List<PayoutFlowControlMasterViewModel> MasterData = (from c in dbContext.PayoutFlowControl.ToList()
                                                                 select new PayoutFlowControlMasterViewModel()
                                                                 {
                                                                     Id = c.Id,
                                                                     ReceivingCurrency = c.ReceivingCurrency,
                                                                     SendingCurrency = c.SendingCurrency,
                                                                     CreatedBy = c.CreatedBy,
                                                                     PayoutApi = c.PayoutApi,
                                                                     CreatedDate = c.CreatedDate,
                                                                     IsPayoutEnabled = c.IsPayoutEnabled,
                                                                     TransferMethod = c.TransferMethod,

                                                                 }).ToList();



            foreach (var item in MasterData)
            {

                List<PayoutFlowControlDetailsViewModel> detailList = GetPayoutFlowControlDetails(item.Id);

                data.Add(new PayoutFlowControlViewModel()
                {
                    Details = detailList,
                    Master = item
                });
            }
            return data;
        }

        public List<PayoutFlowControlDetailsViewModel> GetPayoutFlowControlDetails(int payoutFlowControlId)
        {
            List<PayoutFlowControlDetailsViewModel> detailList = (from c in dbContext.PayoutFlowControlDetails.ToList()
                                                                  select new PayoutFlowControlDetailsViewModel()
                                                                  {
                                                                      Id = c.Id,
                                                                      PayoutFlowControlId = c.PayoutFlowControlId,
                                                                      PayoutProviderId = c.PayoutProviderId,
                                                                      PayoutProviderName = GetBankOrWalletName(c.PayoutProviderId, c.PayoutFlowControl.TransferMethod)
                                                                  }).ToList();


            return detailList;
        }

        private string GetBankOrWalletName(int payoutProviderId, TransactionTransferMethod transferMethod)
        {
            string Name = "";
            switch (transferMethod)
            {
                case TransactionTransferMethod.BankDeposit:
                    Name = Common.Common.getBankName(payoutProviderId);
                    break;
                case TransactionTransferMethod.OtherWallet:
                    Name = Common.Common.GetMobileWalletInfo(payoutProviderId).Name;
                    break;
            }
            return Name;
        }

        internal string UpdateEnableAndDisablePayoutProvider(int payoutFlowControlId)
        {
            string Message = "";
            var payoutFlowControl = Master().Where(x => x.Id == payoutFlowControlId).FirstOrDefault();
            var payoutFlowControlDetails = Details().Where(x => x.PayoutFlowControlId == payoutFlowControlId).ToList();
            if (payoutFlowControl.IsPayoutEnabled == false)
            {
                payoutFlowControl.IsPayoutEnabled = true;
                Message = "Payout Flow Control enabled";
            }
            else
            {
                payoutFlowControl.IsPayoutEnabled = false;
                Message = "Payout Flow Control disabled";
            }
            dbContext.Entry(payoutFlowControl).State = EntityState.Modified;
            dbContext.SaveChanges();

            if (payoutFlowControl.IsPayoutEnabled == true)
            {
                SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();
                switch (payoutFlowControl.TransferMethod)
                {
                    case TransactionTransferMethod.BankDeposit:
                        var bankdepositData = dbContext.BankAccountDeposit.Where(x => x.Status == BankDepositStatus.Paused
                        && x.Apiservice == payoutFlowControl.PayoutApi).ToList();

                        var bankAccountDeposit = (from c in bankdepositData
                                                  join d in payoutFlowControlDetails on c.BankId equals d.PayoutProviderId
                                                  select new BankAccountDeposit()
                                                  {
                                                      AgentCommission = c.AgentCommission,
                                                      Apiservice = c.Apiservice,
                                                      BankCode = c.BankCode,
                                                      BankId = c.BankId,
                                                      BankName = c.BankName,
                                                      ComplianceApprovedBy = c.ComplianceApprovedBy,
                                                      ComplianceApprovedDate = c.ComplianceApprovedDate,
                                                      DuplicateTransactionReceiptNo = c.DuplicateTransactionReceiptNo,
                                                      ExchangeRate = c.ExchangeRate,
                                                      ExtraFee = c.ExtraFee,
                                                      Fee = c.Fee,
                                                      HasMadePaymentToBankAccount = c.HasMadePaymentToBankAccount,
                                                      Id = c.Id,
                                                      IsBusiness = c.IsBusiness,
                                                      IsComplianceApproved = c.IsComplianceApproved,
                                                      IsComplianceNeededForTrans = c.IsComplianceNeededForTrans,
                                                      IsEuropeTransfer = c.IsEuropeTransfer,
                                                      IsManualDeposit = c.IsManualDeposit,
                                                      IsTransactionDuplicated = c.IsTransactionDuplicated,
                                                      Margin = c.Margin,
                                                      MFRate = c.MFRate,
                                                      PaidFromModule = c.PaidFromModule,
                                                      PayingStaffId = c.PayingStaffId,
                                                      PayingStaffName = c.PayingStaffName,
                                                      PaymentReference = c.PaymentReference,
                                                      PaymentType = c.PaymentType,
                                                      ReasonForTransfer = c.ReasonForTransfer,
                                                      ReceiptNo = c.ReceiptNo,
                                                      ReceiverAccountNo = c.ReceiverAccountNo,
                                                      ReceiverCity = c.ReceiverCity,
                                                      ReceiverCountry = c.ReceiverCountry,
                                                      ReceiverMobileNo = c.ReceiverMobileNo,
                                                      ReceiverName = c.ReceiverName,
                                                      ReceivingAmount = c.ReceivingAmount,
                                                      ReceivingCountry = c.ReceivingCountry,
                                                      RecipientId = c.RecipientId,
                                                      SenderId = c.SenderId,
                                                      SenderPaymentMode = c.SenderPaymentMode,
                                                      SendingAmount = c.SendingAmount,
                                                      SendingCountry = c.SendingCountry,
                                                      Status = c.Status,
                                                      TotalAmount = c.TotalAmount,
                                                      TransactionDate = c.TransactionDate,
                                                      TransferReference = c.TransferReference,
                                                      TransferZeroSenderId = c.TransferZeroSenderId

                                                  }).ToList();


                        foreach (var item in bankAccountDeposit)
                        {

                            _senderDocumentationServices.ReInitialBankDepositTransaction(item);
                        }
                        break;

                    case TransactionTransferMethod.OtherWallet:

                        var mobileWalletData = dbContext.MobileMoneyTransfer.Where(x => x.Status == MobileMoneyTransferStatus.Paused
                        && x.Apiservice == payoutFlowControl.PayoutApi).ToList();

                        var mobileWalletTransfer = (from c in mobileWalletData
                                                    join d in payoutFlowControlDetails on c.WalletOperatorId equals d.PayoutProviderId
                                                    select new MobileMoneyTransfer
                                                    {
                                                        WalletOperatorId = c.WalletOperatorId,
                                                        AgentCommission = c.AgentCommission,
                                                        Apiservice = c.Apiservice,
                                                        ComplianceApprovedBy = c.ComplianceApprovedBy,
                                                        ComplianceApprovedDate = c.ComplianceApprovedDate,
                                                        ExchangeRate = c.ExchangeRate,
                                                        ExtraFee = c.ExtraFee,
                                                        Fee = c.Fee,
                                                        Id = c.Id,
                                                        IsComplianceApproved = c.IsComplianceApproved,
                                                        IsComplianceNeededForTrans = c.IsComplianceNeededForTrans,
                                                        Margin = c.Margin,
                                                        MFRate = c.MFRate,
                                                        PaidFromModule = c.PaidFromModule,
                                                        PaidToMobileNo = c.PaidToMobileNo,
                                                        PayingStaffId = c.PayingStaffId,
                                                        PayingStaffName = c.PayingStaffName,
                                                        PaymentReference = c.PaymentReference,
                                                        PaymentType = c.PaymentType,
                                                        ReceiptNo = c.ReceiptNo,
                                                        ReceiverCity = c.ReceiverCity,
                                                        ReceiverName = c.ReceiverName,
                                                        ReceivingAmount = c.ReceivingAmount,
                                                        ReceivingCountry = c.ReceivingCountry,
                                                        RecipientId = c.RecipientId,
                                                        SenderId = c.SenderId,
                                                        SenderPaymentMode = c.SenderPaymentMode,
                                                        SendingAmount = c.SendingAmount,
                                                        SendingCountry = c.SendingCountry,
                                                        Status = c.Status,
                                                        TotalAmount = c.TotalAmount,
                                                        TransactionDate = c.TransactionDate,
                                                        TransferReference = c.TransferReference,
                                                        TransferZeroSenderId = c.TransferZeroSenderId
                                                    }).ToList();
                        foreach (var item in mobileWalletTransfer)
                        {

                            _senderDocumentationServices.ReInitialMobileDepositTransaction(item);
                        }

                        break;
                    case TransactionTransferMethod.CashPickUp:
                        var cashPickUpData = dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Paused
                        && x.Apiservice == payoutFlowControl.PayoutApi).ToList();

                        foreach (var item in cashPickUpData)
                        {
                            _senderDocumentationServices.ReInitialCashPickUpTransaction(item);

                        }
                        break;

                }
            }
            return Message;
        }

        internal void UpdatePayoutFlowControl(PayoutFlowControlViewModel vm)
        {
            var master = dbContext.PayoutFlowControl.Where(x => x.Id == vm.Master.Id).FirstOrDefault();
            master.ReceivingCurrency = vm.Master.ReceivingCurrency;
            master.SendingCurrency = vm.Master.SendingCurrency;
            master.CreatedBy = vm.Master.CreatedBy;
            master.CreatedDate = master.CreatedDate;
            master.IsPayoutEnabled = vm.Master.IsPayoutEnabled;
            master.PayoutApi = vm.Master.PayoutApi;

            dbContext.Entry<PayoutFlowControl>(master).State = EntityState.Modified;
            dbContext.SaveChanges();

            var details = dbContext.PayoutFlowControlDetails.Where(x => x.PayoutFlowControlId == master.Id).ToList();
            dbContext.PayoutFlowControlDetails.RemoveRange(details);
            dbContext.SaveChanges();

            List<PayoutFlowControlDetails> payoutFlowControlDetails = (from c in vm.Details
                                                                       select new PayoutFlowControlDetails()
                                                                       {
                                                                           PayoutFlowControlId = master.Id,
                                                                           PayoutProviderId = c.PayoutProviderId,

                                                                       }).ToList();
            AddPayoutFlowControlDetails(payoutFlowControlDetails);

        }



        public List<PayoutFlowControl> Master()
        {
            var data = dbContext.PayoutFlowControl.ToList();
            return data;
        }


        public List<DropDownViewModel> GetAPiProvider()
        {
            var result = (from c in dbContext.APIProvider
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.APIProviderName
                          }
                      ).Distinct().ToList();

            return result;
        }
        public List<DropDownViewModel> GetBankOrWallet(TransactionTransferMethod transferMethod)
        {
            var result = new List<DropDownViewModel>();
            if (TransactionTransferMethod.BankDeposit == transferMethod)
            {
                result = (from c in dbContext.Bank
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }
                     ).Distinct().ToList();

            }
            else if (TransactionTransferMethod.OtherWallet == transferMethod)
            {
                result = (from c in dbContext.MobileWalletOperator
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }
                   ).Distinct().ToList();
            }

            return result;


        }
        public List<PayoutFlowControlDetails> Details()
        {
            var data = dbContext.PayoutFlowControlDetails.ToList();
            return data;
        }
        public PayoutFlowControlMasterViewModel GetPayoutProvideeMasterDetails(int payoutFlowControlId)
        {
            var data = Master().Where(x => x.Id == payoutFlowControlId);
            var master = (from c in data
                          select new PayoutFlowControlMasterViewModel()
                          {
                              Id = c.Id,
                              TransferMethod = c.TransferMethod,
                              IsPayoutEnabled = c.IsPayoutEnabled,
                              PayoutApi = c.PayoutApi,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency
                          }).FirstOrDefault();


            return master;
        }
        public List<PayoutFlowControlDetailsViewModel> GetPayoutProvideDetails(int? payoutFlowControlId)
        {
            var data = Details().Where(x => x.PayoutFlowControlId == payoutFlowControlId);
            List<PayoutFlowControlDetailsViewModel> detailList = (from c in data
                                                                  select new PayoutFlowControlDetailsViewModel()
                                                                  {
                                                                      Id = c.Id,
                                                                      PayoutFlowControlId = c.PayoutFlowControlId,
                                                                      PayoutProviderId = c.PayoutProviderId
                                                                  }).ToList();


            return detailList;
        }
        public void AddPayoutFlowControlDetails(List<PayoutFlowControlDetails> payoutFlowControlDetails)
        {
            dbContext.PayoutFlowControlDetails.AddRange(payoutFlowControlDetails);
            dbContext.SaveChanges();

        }
        public void Remove(int id)
        {
            var master = dbContext.PayoutFlowControl.Where(x => x.Id == id).FirstOrDefault();
            dbContext.PayoutFlowControl.Remove(master);
            dbContext.SaveChanges();

        }
        public bool AddPayoutFlowControl(PayoutFlowControlViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            PayoutFlowControl PayoutFlowControl = new PayoutFlowControl()
            {
                ReceivingCurrency = vm.Master.ReceivingCurrency,
                SendingCurrency = vm.Master.SendingCurrency,
                IsPayoutEnabled = vm.Master.IsPayoutEnabled,
                PayoutApi = vm.Master.PayoutApi,
                TransferMethod = vm.Master.TransferMethod,
                CreatedBy = staffId,
                CreatedDate = DateTime.Now,

            };
            dbContext.PayoutFlowControl.Add(PayoutFlowControl);
            dbContext.SaveChanges();
            if (vm.Details != null)
            {

                List<PayoutFlowControlDetails> PayoutFlowControlDetails = (from c in vm.Details
                                                                           select new PayoutFlowControlDetails()
                                                                           {
                                                                               PayoutFlowControlId = PayoutFlowControl.Id,
                                                                               PayoutProviderId = c.PayoutProviderId,
                                                                           }).ToList();
                AddPayoutFlowControlDetails(PayoutFlowControlDetails);
            }
            return true;
        }
        public PayoutFlowControlViewModel payoutFlowControlByMasterId(int id)
        {
            var Master = GetPayoutProvideeMasterDetails(id);

            var detailList = GetPayoutProvideDetails(Master.Id);

            PayoutFlowControlViewModel payoutFlowControl = new PayoutFlowControlViewModel()
            {
                Details = detailList,
                Master = Master
            };
            return payoutFlowControl;
        }

    }

}