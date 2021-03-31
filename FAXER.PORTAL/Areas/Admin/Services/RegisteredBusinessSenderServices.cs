using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RegisteredBusinessSenderServices
    {
        private DB.FAXEREntities dbContext = null;
        public RegisteredBusinessSenderServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<SenderBusinessprofileViewModel> GetRegisteredBusinessSender(string Country = "", string City = "", string businessName = "", int FaxerId = 0)
        {
            IQueryable<FaxerInformation> senderInfo = dbContext.FaxerInformation.Where(x => x.IsBusiness == true && x.IsDeleted == false);
            IQueryable<FaxerLogin> faxerLogin = dbContext.FaxerLogin;
            IQueryable<BusinessRelatedInformation> businessInfo = dbContext.BusinessRelatedInformation;
            if (!string.IsNullOrEmpty(Country))
            {
                senderInfo = senderInfo.Where(x => x.Country == Country);
            }
            if (!string.IsNullOrEmpty(City))
            {
                senderInfo = senderInfo.Where(x => x.City == City);
            }
            if (FaxerId != 0)
            {
                senderInfo = senderInfo.Where(x => x.Id == FaxerId);
            }

            if (!string.IsNullOrEmpty(businessName))
            {
                businessName = businessName.Trim();
                businessInfo = businessInfo.Where(x => x.BusinessName.ToLower().Contains(businessName.ToLower()));
            }
            var result = (from c in senderInfo
                          join d in businessInfo on c.Id equals d.FaxerId
                          join e in faxerLogin on c.Id equals e.FaxerId
                          join sendercountry in dbContext.Country on c.Country equals sendercountry.CountryCode
                          join businnesscountry in dbContext.Country on c.Country equals businnesscountry.CountryCode
                          select new SenderBusinessprofileViewModel()
                          {
                              Id = c.Id,
                              AccountNo = c.AccountNo,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              City = c.City,
                              BusinessAddress1 = d.AddressLine1,
                              BusinessAddress2 = d.AddressLine2,
                              BusinessCity = d.City,
                              BusinessCountry = businnesscountry.CountryName,
                              Country = sendercountry.CountryName,
                              ContactName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              BusinessName = d.BusinessName,
                              Email = c.Email,
                              RegistrationNumber = d.RegistrationNo,
                              PhoneNumber = c.PhoneNumber,
                              Postal = c.PostalCode,
                              BusinessType = d.BusinessType.ToString(),
                              DOB = c.DateOfBirth.ToString(),
                              Status = e.IsActive == true ? "Active" : "Deactive",
                              CountryCurrency = sendercountry.Currency,
                              CountryFlag = c.Country == null ? "" : c.Country.ToLower(),
                              PhoneCode = sendercountry.CountryPhoneCode,
                          }).ToList();
            return result;
        }

        public SenderBusinessFullDetailViewModel GetSenderFullDetails(int senderId)
        {
            SenderBusinessFullDetailViewModel vm = new SenderBusinessFullDetailViewModel();
            vm.TransactionDetails = TransactionStatement(senderId);
            vm.SenderDetails = GetRegisteredBusinessSender(FaxerId: senderId).FirstOrDefault();
            vm.LimitHistory = GetLimitHistory(senderId);
            vm.Limit = GetLimit(senderId);
            return vm;
        }

        public List<BusinessLimitViewModel> GetLimit(int senderId)
        {
            var data = dbContext.BusinessLimit.Where(x => x.SenderId == senderId).ToList();

            var result = (from c in data
                          join d in dbContext.BusinessRelatedInformation on c.SenderId equals d.FaxerId
                          select new BusinessLimitViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = Common.Common.GetCountryName(c.Country),
                              Frequency = c.Frequency,
                              FrequencyAmount = c.FrequencyAmount,
                              SenderId = c.SenderId,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              FrequencyName = c.Frequency.ToString(),
                              BusinessName = d.BusinessName,
                          }).ToList();
            return result;
        }

        public List<BusinessLimitViewModel> GetLimitHistory(int senderId)
        {
            var data = dbContext.BusinessLimtiHistory.Where(x => x.SenderId == senderId).ToList();

            //if (TransferService != 0)
            //{
            //    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferService).ToList();
            //}

            var result = (from c in data
                          join d in dbContext.BusinessRelatedInformation on c.SenderId equals d.FaxerId
                          select new BusinessLimitViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = Common.Common.GetCountryName(c.Country),
                              Frequency = c.Frequency,
                              FrequencyAmount = c.FrequencyAmount,
                              SenderId = c.SenderId,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              FrequencyName = c.Frequency.ToString(),
                              BusinessName = d.BusinessName,
                              DateTime = c.CreatedDate.ToString("MM-dd-yyy")
                          }).ToList();
            return result;
        }

        public List<SenderBusinessTransactionStatement> TransactionStatement(int senderId)
        {
            SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();
            var data = _senderTransactionHistoryServices.GetTransactionHistories(TransactionServiceType.All, senderId, 0, 0).TransactionHistoryList.ToList();

            data = data.Where(x => x.IsBusiness == true).ToList();

            var result = (from c in data
                          select new SenderBusinessTransactionStatement()
                          {
                              Id = c.Id,
                              Amount = c.SendingCurrencySymbol + " " + c.GrossAmount,
                              TransactionDate = c.TransactionDate,
                              Fee = c.SendingCurrencySymbol + " " + c.Fee,
                              Identifier = c.TransactionIdentifier,
                              Type = c.TransactionType,
                              Method = Common.Common.GetEnumDescription(c.TransactionServiceType),
                              Status = c.StatusName

                          }).Take(8).ToList();
            return result;
        }

        public SenderBusinessTransactionStatementWithSenderDetails TransactionStatementOfBusinessSender(TransactionServiceType transactionServiceType, int year,
            int month, int day, int senderId, int pageSize, int pageNumber, string Identifier = "")
        {
            SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();
            var data = _senderTransactionHistoryServices.GetTransactionHistories((TransactionServiceType)transactionServiceType,
                  senderId, year, month, pageNumber, pageSize , true).TransactionHistoryList.ToList();

            if (day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == day).ToList();
            }
            var result = (from c in data
                          select new SenderBusinessTransactionStatement()
                          {
                              Id = c.Id,
                              Amount = c.SendingCurrencySymbol + " " + c.GrossAmount,
                              TransactionDate = c.TransactionDate,
                              Fee = c.SendingCurrencySymbol + " " + c.Fee,
                              Identifier = c.TransactionIdentifier,
                              Type = "Payment",
                              Method = Common.Common.GetEnumDescription(c.TransactionServiceType),
                              TransactionServiceType = c.TransactionServiceType,
                              Status = c.StatusName,
                              SenderId = c.senderId,
                              SenderName = c.SenderName,
                              TotalCount = c.TotalCount
                          }).ToList();

            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();

                result = result.Where(x => x.Identifier.ToLower().Contains(Identifier.ToLower())).ToList();

            }

            SenderBusinessTransactionStatementWithSenderDetails vm = new SenderBusinessTransactionStatementWithSenderDetails();
            vm.SenderBusinessTransactionStatement = result;
            if (senderId != 0)
            {
                var senderInfo = Common.Common.GetSenderInfo(senderId);
                vm.SenderCountry = senderInfo.Country;
                vm.SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
                vm.SenderAccountNumber = senderInfo.AccountNo;

            }
            return vm;
        }
    }
}