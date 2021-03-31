using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class StaffTransactionHistoryServices
    {
        FAXEREntities db = null;
        CommonServices _commonServices = null;
        public StaffTransactionHistoryServices()
        {
            db = new FAXEREntities();
            _commonServices = new CommonServices();

        }
        public List<DailyTransactionStatementListVm> GetStaffTransactionStatementList()
        {
            var CashPickupTransfer = (from c in db.FaxingNonCardTransaction.Where(x => x.OperatingUserType == OperatingUserType.Admin).ToList()

                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionServiceType = TransactionServiceType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                          Amount = c.FaxingAmount,
                                          Fee = c.FaxingFee,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = _commonServices.getStaffName((int)c.PayingStaffId),
                                          Type = Agent.Models.Type.Transfer,
                                          AgentCommission = c.AgentCommission,
                                          StatusName = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus),
                                          FormatedDate = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                          ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                          ReceiverName = _commonServices.GetRecipientsName(c.RecipientId),
                                          SenderId = c.SenderId,
                                      }).ToList();
            var KiiPayWalletTransfer = (from c in db.TopUpSomeoneElseCardTransaction.Where(x => x.PayedBy == PayedBy.Admin).ToList()

                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                            TransactionServiceType = TransactionServiceType.KiiPayWallet,
                                            TransactionType = TransactionType.KiiPayWallet,
                                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                            Amount = c.FaxingAmount,
                                            Fee = c.FaxingFee,
                                            TransactionIdentifier = c.ReceiptNumber,
                                            DateAndTime = c.TransactionDate,
                                            StaffName = _commonServices.getStaffName((int)c.PayingStaffId),
                                            Type = Agent.Models.Type.Transfer,
                                            AgentCommission = c.AgentCommission,
                                            StatusName = "No Status Found",
                                            FormatedDate = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                            ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                            ReceiverName = _commonServices.GetRecipientsName(c.RecipientId),
                                            SenderId = c.FaxerId,


                                        }).ToList();

            var OtherMobilesWalletsTransfer = (from c in db.MobileMoneyTransfer.Where(x => x.PaidFromModule == Module.Staff).ToList()

                                               select new DailyTransactionStatementListVm()
                                               {
                                                   Id = c.Id,
                                                   TransactionType = TransactionType.OtherWalletTransfer,
                                                   TransactionServiceType = TransactionServiceType.MobileWallet,
                                                   TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.OtherWalletTransfer),
                                                   CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                   Amount = c.SendingAmount,
                                                   Fee = c.Fee,
                                                   TransactionIdentifier = c.ReceiptNo,
                                                   DateAndTime = c.TransactionDate,
                                                   StaffName = _commonServices.getStaffName((int)c.PayingStaffId),
                                                   AgentCommission = c.AgentCommission,
                                                   Type = Agent.Models.Type.Transfer,
                                                   StatusName = Enum.GetName(typeof(MobileMoneyTransferStatus), c.Status),
                                                   FormatedDate = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                                   ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                                   ReceiverName = _commonServices.GetRecipientsName(c.RecipientId),
                                                   SenderId = c.SenderId,


                                               }).ToList();
            var BankAccountDepositTransfer = (from c in db.BankAccountDeposit.Where(x => x.PaidFromModule == Module.Staff).ToList()

                                              select new DailyTransactionStatementListVm()
                                              {
                                                  Id = c.Id,
                                                  TransactionType = TransactionType.BankAccountDeposit,
                                                  TransactionServiceType = TransactionServiceType.BankDeposit,
                                                  TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.BankAccountDeposit),
                                                  CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                  Amount = c.SendingAmount,
                                                  Fee = c.Fee,
                                                  TransactionIdentifier = c.ReceiptNo,
                                                  DateAndTime = c.TransactionDate,
                                                  StaffName = _commonServices.getStaffName((int)c.PayingStaffId),
                                                  AgentCommission = c.AgentCommission,
                                                  Type = Agent.Models.Type.Transfer,
                                                  StatusName = Enum.GetName(typeof(FAXER.PORTAL.DB.BankAccountStatus), c.Status),
                                                  FormatedDate = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                                  ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                                  ReceiverName = _commonServices.GetRecipientsName(c.RecipientId),
                                                  SenderId = c.SenderId


                                              }).ToList();

            var finalList = CashPickupTransfer.Concat(KiiPayWalletTransfer).Concat(OtherMobilesWalletsTransfer).Concat(BankAccountDepositTransfer).ToList();
            return finalList.OrderByDescending(x => x.DateAndTime).ToList();
        }

        public AgentTransactionHistoryViewModel GetTransactionHistories(TransactionType transactionService, int Id)
        {
            AgentTransactionHistoryViewModel transactionHistory = new AgentTransactionHistoryViewModel();
            transactionHistory.FilterKey = transactionService;
            transactionHistory.TransactionHistoryList = new List<AgentTransactionHistoryList>();
            switch (transactionService)
            {
                case TransactionType.KiiPayWallet:
                    transactionHistory.TransactionHistoryList = GetKiiPayWalletPayment(Id);

                    break;
                case TransactionType.CashPickUp:

                    transactionHistory.TransactionHistoryList = GetCashPickUpPayment(Id);

                    break;
                case TransactionType.OtherWalletTransfer:
                    transactionHistory.TransactionHistoryList = GetOtherMobileWallets(Id);
                    break;
                case TransactionType.BankAccountDeposit:
                    transactionHistory.TransactionHistoryList = GetBankAccountDeposit(Id);
                    break;

                default:

                    transactionHistory.TransactionHistoryList = GetKiiPayWalletPayment(Id)
                                                                 .Concat(GetCashPickUpPayment(Id)).
                                                                 Concat(GetOtherMobileWallets(Id)).Concat(GetBankAccountDeposit(Id)).ToList();

                    break;
            }



            transactionHistory.TransactionHistoryList = transactionHistory.TransactionHistoryList.OrderByDescending(x => x.TransactionDate).ToList();
            return transactionHistory;
        }
        public List<AgentTransactionHistoryList> GetKiiPayWalletPayment(int Id)
        {
            var result = (from c in db.TopUpSomeoneElseCardTransaction.Where(x => x.Id == Id).ToList()
                          join d in db.FaxerInformation on c.FaxerId equals d.Id

                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.FaxingFee,
                              AmountSent = c.FaxingAmount,
                              ReceiverName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.RecievingAmount,
                              ReceiverEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                              ReceiverNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                              AmountPaid = c.TotalAmount,
                              Type = Agent.Models.Type.Transfer,
                              ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNumber,
                              TransactionStaff = c.PayingStaffName,

                              PaymentMethod = SenderPaymentMode.Cash.ToString(),
                              ReceiverDOB = c.KiiPayPersonalWalletInformation.CardUserDOB.ToString("MMM d, yyyy"),
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.KiiPayWallet
                          }).ToList();

            return result;

        }



        public List<AgentTransactionHistoryList> GetCashPickUpPayment(int Id)
        {
            var result = (from c in db.FaxingNonCardTransaction.Where(x => x.Id == Id).ToList()
                          join d in db.FaxerInformation on c.NonCardReciever.FaxerID equals d.Id

                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.FaxingFee,
                              AmountSent = c.FaxingAmount,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverEmail = c.NonCardReciever.EmailAddress,
                              ReceiverNumber = c.NonCardReciever.PhoneNumber,
                              AmountPaid = c.TotalAmount,
                              Type = Agent.Models.Type.Transfer,
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNumber,
                              TransactionStaff = c.AgentStaffName,

                              PaymentMethod = c.PaymentMethod,
                              MFCN = c.MFCN,
                              Status = Common.Common.GetEnumDescription(c.FaxingStatus),
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              CustomerType = SystemCustomerType.CustomerPayment,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerPayment),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.CashPickUp
                          }).ToList();

            return result;
        }


        public List<AgentTransactionHistoryList> GetOtherMobileWallets(int Id)
        {
            var data = db.MobileMoneyTransfer.Where(x => x.Id == Id).ToList();

            var result = new List<AgentTransactionHistoryList>();
            try
            {
                result = (from c in db.MobileMoneyTransfer.Where(x => x.Id == Id).ToList()
                          join d in db.FaxerInformation on c.SenderId equals d.Id

                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceiverName = c.ReceiverName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverNumber = c.PaidToMobileNo,
                              AmountPaid = c.TotalAmount,
                              Type = Agent.Models.Type.Transfer,
                              ReceiverCity = c.ReceiverCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,

                              SenderDOB = d.DateOfBirth == null ? "" : d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              PaymentMethod = "Cash",
                              AccountNumber = c.PaidToMobileNo,
                              WalletName = c.WalletOperatorId > 0 ? Common.Common.GetMobileWalletInfo(c.WalletOperatorId).Name : "",
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.OtherWalletTransfer
                          }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }


            return result;
        }

        public List<AgentTransactionHistoryList> GetBankAccountDeposit(int Id)
        {
            var result = (from c in db.BankAccountDeposit.Where(x => x.Id == Id).ToList()
                          join d in db.FaxerInformation on c.SenderId equals d.Id

                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceiverName = c.ReceiverName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverNumber = c.ReceiverMobileNo,
                              BankBranch = c.BankCode,
                              BankName = db.Bank.Where(x => x.Id == c.BankId).Select(x => x.Name).FirstOrDefault(),
                              AmountPaid = c.TotalAmount,
                              Type = Agent.Models.Type.Transfer,
                              ReceiverCity = c.ReceiverCity,
                              SenderCountry = Common.Common.GetCountryName(c.SendingCountry),
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,

                              AccountNumber = c.ReceiverAccountNo,
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              PaymentMethod = "Cash",
                              Status = Common.Common.GetEnumDescription(c.Status),
                              BankStatus = c.Status,
                              IsRetryableCountry = Common.Common.IsRetryAbleCountry(c.Id),
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.BankAccountDeposit

                          }).ToList();

            return result;
        }




    }
}