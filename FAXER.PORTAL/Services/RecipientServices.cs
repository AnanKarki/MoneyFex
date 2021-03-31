using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class RecipientServices
    {
        DB.FAXEREntities dbContext = null;
        public RecipientServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public RecipientServices(DB.FAXEREntities dbContext)
        {
            dbContext = new FAXEREntities();
        }
        public void AddReceipients(RecipientsViewModel vm)
        {
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
                City = vm.ReceiverCity,
                Email = vm.ReceiverCity,
                PostalCode = vm.ReceiverPostalCode,
                Street = vm.ReceiverStreet,
                IdentificationNumber = vm.IdentityCardNumber,
                IdentificationTypeId = vm.IdentityCardId
            };
            Add(model);
        }

        public void Add(Recipients model)
        {
            dbContext.Recipients.Add(model);
            dbContext.SaveChanges();
        }

        public List<DropDownViewModel> GetWallets(string Country = "")
        {
            if (!string.IsNullOrEmpty(Country))
            {
                var list = (from c in dbContext.MobileWalletOperator.Where(x => x.Country == Country).ToList()
                            select new DropDownViewModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                CountryCode = c.Country.ToUpper()
                            }).ToList();
                return list;
            }
            else
            {
                var list = (from c in dbContext.MobileWalletOperator.ToList()
                            select new DropDownViewModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                CountryCode = c.Country.ToUpper()
                            }).ToList();
                return list;
            }
        }

        public List<Recipients> RecipientsList()
        {
            var data = dbContext.Recipients.ToList();
            return data;
        }

        public IQueryable<Recipients> Recipients()
        {
            return dbContext.Recipients;
        }
        public SenderBankAccountDepositVm GetBankRecipients(int id)
        {
            var data = RecipientsList().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new SenderBankAccountDepositVm()
                          {
                              Id = c.Id,
                              AccountNumber = c.AccountNo,
                              AccountOwnerName = c.ReceiverName,
                              BankId = c.BankId,
                              BranchCode = c.BranchCode,
                              CountryCode = c.Country,
                              IsBusiness = c.IBusiness,
                              ReasonForTransfer = c.Reason,
                              ReceiverStreet = c.Street,
                              ReceiverEmail = c.Email,
                              ReceiverCity = c.City,
                              ReceiverPostalCode = c.PostalCode,
                              MobileNumber = c.MobileNo
                          }).FirstOrDefault();
            return result;
        }
        public SenderMobileMoneyTransferVM GetWalletRecipients(int id)
        {
            var data = RecipientsList().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new SenderMobileMoneyTransferVM()
                          {

                              CountryCode = c.Country,
                              MobileNumber = c.MobileNo,
                              ReceiverName = c.ReceiverName,
                              CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                              WalletId = c.MobileWalletProvider,
                              Id = c.Id,
                          }).FirstOrDefault();
            return result;
        }
        public SenderCashPickUpVM GetCashPickUpRecipients(int id)
        {
            var data = RecipientsList().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new SenderCashPickUpVM()
                          {

                              CountryCode = c.Country,
                              MobileNumber = c.MobileNo,
                              FullName = c.ReceiverName,
                              IdentityCardNumber = c.IdentificationNumber,
                              IdenityCardId = c.IdentificationTypeId,
                              Id = c.Id,
                          }).FirstOrDefault();
            return result;
        }
        public SearchKiiPayWalletVM GetKiiPayWalletRecipients(int id)
        {
            var data = RecipientsList().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new SearchKiiPayWalletVM()
                          {
                              Id = c.Id,
                              CountryCode = c.Country,
                              MobileNo = c.MobileNo,
                              ReceiverName = c.ReceiverName,
                          }).FirstOrDefault();
            return result;
        }

        internal void UpdateReceipts(Recipients model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        internal void DeleteReceipts(int id)
        {
            var model = RecipientsList().Where(x => x.Id == id).FirstOrDefault();
            dbContext.Recipients.Remove(model);
            dbContext.SaveChanges();
        }


        public List<RecipientsViewModel> GetReceiverInformation(int senderId = 0, string receiverName = "")
        {
            var recipients = dbContext.Recipients.Where(x => x.SenderId == senderId).ToList();
            if (!string.IsNullOrEmpty(receiverName))
            {

                recipients = recipients.Where(x => x.ReceiverName.ToLower().StartsWith(receiverName.ToLower())).ToList();
            }

            var recipientsResult = (from c in recipients
                                    select new RecipientsViewModel()
                                    {
                                        Id = c.Id,
                                        SenderId = c.SenderId,
                                        MobileNo = c.MobileNo,
                                        Country = Common.Common.GetCountryCurrency(c.Country),
                                        ReceiverName = c.ReceiverName,
                                        AccountNo = c.AccountNo,
                                        BankId = c.BankId,
                                        BankName = Common.Common.getBankName(c.BankId),
                                        BranchCode = c.BranchCode,
                                        IBusiness = c.IBusiness,
                                        MobileWalletProvider = c.MobileWalletProvider,
                                        Service = c.Service,
                                        ServiceName = Common.Common.GetEnumDescription(c.Service),
                                        MobileWalletProviderName = GetMobileWalletname(c.MobileWalletProvider),
                                        ReceiverCountryLower = c.Country.ToLower(),
                                        ReciverFirstLetter = GetReceiverFirstLetter(c.ReceiverName),
                                    }).OrderByDescending(x => x.Id).ToList();


            return recipientsResult;
        }
        private string GetMobileWalletname(int mobileWalletProvider)
        {
            string MobileWalletProvide = dbContext.MobileWalletOperator.
                                        Where(x => x.Id == mobileWalletProvider).
                                        Select(x => x.Name).FirstOrDefault();
            return MobileWalletProvide;
        }

        private string GetReceiverFirstLetter(string receiverName)
        {
            string name = receiverName;
            if (string.IsNullOrEmpty(receiverName))
            {

                return "";
            }
            var firstletter = name.Substring(0, 1);
            return firstletter;

        }


    }
}