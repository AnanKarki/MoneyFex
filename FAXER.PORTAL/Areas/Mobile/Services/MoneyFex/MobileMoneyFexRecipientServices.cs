using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobileMoneyFexRecipientServices
    {
        DB.FAXEREntities dbContext = null;
        public MobileMoneyFexRecipientServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public ServiceResult<IQueryable<Recipients>> RecipientsList()
        {
            return new ServiceResult<IQueryable<Recipients>>
            {
                Data = dbContext.Recipients,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<Recipients>> LimitedRecipientsList(int count)
        {
            return new ServiceResult<IQueryable<Recipients>>
            {
                Data = dbContext.Recipients,    
                Status = ResultStatus.OK
            };
        }
       
        public MobileRecipentViewModel AddReceipients(MobileRecipentViewModel vm)
        {
            //var data = dbContext.Recipients.Where(x => x.MobileNo == vm.MobileNo && x.ReceiverName == vm.ReceiverName && x.Service == vm.Service && x.Country == vm.Country).ToList();
            //if (data.Count <= 0)
            //{

            Recipients model = new Recipients()
            {
                AccountNo = vm.AccountNo,
                BankId = vm.BankId,
                BranchCode = vm.BranchCode,
                Country = vm.Country,
                MobileNo = vm.MobileNo,
                MobileWalletProvider = vm.MobileWalletProvider,
                Reason = vm.Reason,
                ReceiverName = vm.ReceiverName,
                SenderId = vm.SenderId,
                Service = vm.Service,
                IBusiness = vm.IBusiness,
                City = vm.City,
                Email = vm.EmailAddress,
                PostalCode = vm.PostCode,
                Street = vm.Address,
                IdentificationNumber = vm.IdentificationNumber,
                IdentificationTypeId = vm.IdentificationTypeId
            };
            dbContext.Recipients.Add(model);
            dbContext.SaveChanges();
            //}
            return vm;
        }
        public MobileRecipentViewModel UpdateReceipients(MobileRecipentViewModel vm)
        {
            var data = dbContext.Recipients.Where(x => x.Id == vm.Id).FirstOrDefault();

            data.AccountNo = vm.AccountNo;
            data.BankId = vm.BankId;
            data.BranchCode = vm.BranchCode;
            data.Country = vm.Country;
            data.MobileNo = vm.MobileNo;
            data.MobileWalletProvider = vm.MobileWalletProvider;
            data.Reason = vm.Reason;
            data.ReceiverName = vm.ReceiverName;
            data.SenderId = vm.SenderId;
            data.Service = vm.Service;
            data.City = vm.City;
            data.Email = vm.EmailAddress;
            data.PostalCode = vm.PostCode;
            data.Street = vm.Address;
            data.IBusiness = vm.IBusiness;
            data.IdentificationNumber = vm.IdentificationNumber;
            data.IdentificationTypeId = vm.IdentificationTypeId;
            dbContext.Entry<Recipients>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return vm;
        }
        public ServiceResult<bool> DeleteReceipients(int Id)
        {
            var data = dbContext.Recipients.Where(x => x.Id == Id).FirstOrDefault();

            if (data != null)
            {
                dbContext.Recipients.Remove(data);
                dbContext.SaveChanges();
            }

            return new ServiceResult<bool>()
            {
                Data = true,
                Message = "",
                Status = ResultStatus.OK
            };
        }

       

        public MobileRecipentViewModel ReceipientsDetails(int Id)
        {
            var data = dbContext.Recipients.Where(x => x.Id == Id).FirstOrDefault();
            MobileRecipentViewModel model = new MobileRecipentViewModel();
            if (data != null)
            {


                model.AccountNo = data.AccountNo;
                model.BankId = data.BankId;
                model.BranchCode = data.BranchCode;
                model.Country = FAXER.PORTAL.Common.Common.GetCountryName(data.Country);
                model.CountryCode = data.Country;
                model.MobileNo = data.MobileNo;
                model.MobileWalletProvider = data.MobileWalletProvider;
                model.Reason = data.Reason;
                model.ReceiverName = data.ReceiverName;
                model.SenderId = data.SenderId;
                model.Service = data.Service;
                model.IBusiness = data.IBusiness;
                model.Address = data.Street;
                model.IdentificationTypeName = getIdentificationTypeName(data.IdentificationTypeId);
                model.IdentificationTypeId = data.IdentificationTypeId;
                model.IdentificationNumber = data.IdentificationNumber;
                model.City = data.City;
                model.EmailAddress = data.Email;
                model.PostCode = data.PostalCode;
                model.BankName = getBankName(data.BankId);
                model.MobileWalletProviderName = GetMobileWalletName(data.MobileWalletProvider);
                model.Id = data.Id;
                model.CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(data.Country);

            }
            return model;
        }

        private string getIdentificationTypeName(int identificationTypeId=0)
        {
            string name = "";
            if (identificationTypeId != 0)
            {
                name = dbContext.IdentityCardType.Where(x => x.Id == identificationTypeId).Select(x => x.CardType).FirstOrDefault();

            }
            return name;
        }

        public List<TransactionDetailsViewModel> TransacationStatementDetails(int receipentId)
        {
            var bankAccountDeposit = (from c in dbContext.BankAccountDeposit.Where(x => x.RecipientId == receipentId && x.Status != BankDepositStatus.Cancel).ToList()
                                      select new TransactionDetailsViewModel()
                                      {
                                          TransactionId = c.TransactionId,
                                          City = c.ReceiverCity,
                                          ExchangeRate = c.ExchangeRate,
                                          PaymentMethod = c.SenderPaymentMode.ToString(),
                                          Fee = c.Fee,
                                          ReceiverName = c.ReceiverName,
                                          ReceivingAmount = c.ReceivingAmount,
                                          ReceivingCountry = getCountryName(c.ReceivingCountry),
                                          ReceivingCountryCode = c.ReceivingCountry,
                                          ReceivingCountryCurrency = getCountryCurrency(c.ReceivingCountry),
                                          ReceivingCountrySymbol = getCountryCurrencySymbol(c.ReceivingCountry),
                                          RecipentId = c.RecipientId,
                                          ReciverFirstLetter = !string.IsNullOrEmpty(c.ReceiverName) ? c.ReceiverName.Substring(0, 1) : "",
                                          SenderId = c.SenderId,
                                          SendingAmount = c.SendingAmount,
                                          SendingCountry = getCountryName(c.SendingCountry),
                                          SendingCountryCode = c.SendingCountry,
                                          SendingCountryCurrency = getCountryCurrency(c.SendingCountry),
                                          SendingCountrySymbol = getCountryCurrencySymbol(c.SendingCountry),
                                          Service = Service.BankAccount,
                                          TotalAmount = c.TotalAmount,
                                          Status = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                                          TransactionDate = c.TransactionDate.ToFormatedString(),
                                          FormattedTransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                                          FormattedAccountNo = !string.IsNullOrEmpty(c.ReceiverAccountNo) ? "**" + c.ReceiverAccountNo.Substring(0, 4) : "",
                                          AccountNo = c.ReceiverAccountNo
                                      }).ToList();

            var MobileWallet = (from c in dbContext.MobileMoneyTransfer.Where(x => x.RecipientId == receipentId && x.Status != MobileMoneyTransferStatus.Cancel).ToList()
                                select new TransactionDetailsViewModel()
                                {
                                    TransactionId = c.Id,
                                    City = c.ReceiverCity,
                                    ExchangeRate = c.ExchangeRate,
                                    PaymentMethod = c.SenderPaymentMode.ToString(),
                                    Fee = c.Fee,
                                    ReceiverName = c.ReceiverName,
                                    ReceivingAmount = c.ReceivingAmount,
                                    ReceivingCountry = getCountryName(c.ReceivingCountry),
                                    ReceivingCountryCode = c.ReceivingCountry,
                                    ReceivingCountryCurrency = getCountryCurrency(c.ReceivingCountry),
                                    ReceivingCountrySymbol = getCountryCurrencySymbol(c.ReceivingCountry),
                                    RecipentId = c.RecipientId,
                                    ReciverFirstLetter = !string.IsNullOrEmpty(c.ReceiverName) ? c.ReceiverName.Substring(0, 1) : "",
                                    SenderId = c.SenderId,
                                    SendingAmount = c.SendingAmount,
                                    SendingCountry = getCountryName(c.SendingCountry),
                                    SendingCountryCode = c.SendingCountry,
                                    SendingCountryCurrency = getCountryCurrency(c.SendingCountry),
                                    SendingCountrySymbol = getCountryCurrencySymbol(c.SendingCountry),
                                    Service = Service.MobileWallet,
                                    TotalAmount = c.TotalAmount,
                                    Status = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                                    TransactionDate = c.TransactionDate.ToFormatedString(),
                                    FormattedTransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                                    AccountNo = c.PaidToMobileNo,
                                    FormattedAccountNo = !string.IsNullOrEmpty(c.PaidToMobileNo) ? "**" + c.PaidToMobileNo.Substring(Math.Max(0, c.PaidToMobileNo.Length - 4)) : ""

                                }).ToList();
            var cashPickUp = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.RecipientId == receipentId && x.FaxingStatus != FaxingStatus.Cancel).ToList()
                              join d in dbContext.Recipients on c.RecipientId equals d.Id
                              select new TransactionDetailsViewModel()
                              {
                                  TransactionId = c.Id,

                                  ExchangeRate = c.ExchangeRate,
                                  PaymentMethod = c.SenderPaymentMode.ToString(),
                                  Fee = c.FaxingFee,
                                  ReceiverName = d.ReceiverName,
                                  ReceivingAmount = c.ReceivingAmount,
                                  ReceivingCountry = getCountryName(c.ReceivingCountry),
                                  ReceivingCountryCode = c.ReceivingCountry,
                                  ReceivingCountryCurrency = getCountryCurrency(c.ReceivingCountry),
                                  ReceivingCountrySymbol = getCountryCurrencySymbol(c.ReceivingCountry),
                                  RecipentId = c.RecipientId,
                                  ReciverFirstLetter = !string.IsNullOrEmpty(d.ReceiverName) ? d.ReceiverName.Substring(0, 1) : "",
                                  SenderId = c.SenderId,
                                  SendingAmount = c.FaxingAmount,
                                  SendingCountry = getCountryName(c.SendingCountry),
                                  SendingCountryCode = c.SendingCountry,
                                  SendingCountryCurrency = getCountryCurrency(c.SendingCountry),
                                  SendingCountrySymbol = getCountryCurrencySymbol(c.SendingCountry),
                                  Service = Service.CashPickUP,
                                  TotalAmount = c.TotalAmount,
                                  Status = FAXER.PORTAL.Common.Common.GetEnumDescription(c.FaxingStatus),
                                  TransactionDate = c.TransactionDate.ToFormatedString(),
                                  FormattedTransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                                  AccountNo = d.MobileNo,
                                  FormattedAccountNo = !string.IsNullOrEmpty(d.MobileNo) ? "**" + d.MobileNo.Substring(0, 4) : ""
                              }).ToList();

            var result = bankAccountDeposit.Concat(MobileWallet).Concat(cashPickUp).ToList();
            return result;
        }
        public string getBankName(int bankId = 0)
        {
            string name = "";
            if (bankId != 0)
            {
                name = dbContext.Bank.Where(x => x.Id == bankId).Select(x => x.Name).FirstOrDefault();

            }
            return name;
        }
        public string getCountryName(string CountryCode)
        {
            string name = "";
            if (!string.IsNullOrEmpty(CountryCode))
            {
                name = getCountryInfo(CountryCode).CountryName;

            }
            return name;
        }
        public string getCountryCurrencySymbol(string CountryCode)
        {
            string symbol = "";
            if (!string.IsNullOrEmpty(CountryCode))
            {
                symbol = getCountryInfo(CountryCode).CurrencySymbol;

            }
            return symbol;
        }
        public string getCountryCurrency(string CountryCode)
        {
            string CountryCurrency = "";
            if (!string.IsNullOrEmpty(CountryCode))
            {
                CountryCurrency = getCountryInfo(CountryCode).CurrencySymbol;

            }
            return CountryCurrency;
        }
        public Country getCountryInfo(string CountryCode)
        {
            var country = dbContext.Country.Where(x => x.CountryCode.ToLower() == CountryCode.ToLower()).FirstOrDefault();
            return country;
        }

        public string GetMobileWalletName(int MobileWalletProvider = 0)
        {
            string name = "";
            if (MobileWalletProvider != 0)
            {
                name = dbContext.MobileWalletOperator.Where(x => x.Id == MobileWalletProvider).Select(x => x.Name).FirstOrDefault();

            }
            return name;
        }
    }
}