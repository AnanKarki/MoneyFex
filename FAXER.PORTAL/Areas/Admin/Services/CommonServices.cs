using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CommonServices
    {
        private FAXEREntities dbContext = null;

        public CommonServices()
        {
            dbContext = new FAXEREntities();

        }


        public string GetRange(decimal fromRange, decimal toRange)
        {

            var from = (int)fromRange;
            var to = (int)toRange;
            var range = from + "-" + to;

            return range == null ? "0" : range;
        }

        public string getStaffCountry(int staffId)
        {
            var code = dbContext.StaffInformation.Where(x => x.Id == staffId).Select(x => x.Country).FirstOrDefault();
            return code;
        }
        public List<StaffInformation> getAdminStaffInformation()
        {
            var list = dbContext.StaffInformation.ToList();
            return list;
        }
        public List<DropDownViewModel> GetCountries(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.Country.Where(x => x.CountryCode == CountryCode)
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Code = c.CountryCode,
                                  Name = c.CountryName,
                                  CountryCurrency = c.Currency
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.Country
                              select new DropDownViewModel()
                              {

                                  Id = c.Id,
                                  Code = c.CountryCode,
                                  Name = c.CountryName,
                                  CountryCurrency = c.Currency
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
        }



        public List<DropDownViewModel> GetRecipients(int senderId = 0)
        {
            var data = dbContext.Recipients.ToList();

            if (senderId > 0)
            {
                data = data.Where(x => x.SenderId == senderId).ToList();
            }
            var result = (from c in data
                          select new DropDownViewModel()
                          {

                              Id = c.Id,
                              Name = c.ReceiverName,
                              CountryCode = c.Country,

                          }
                      ).ToList();

            return result;
        }


        public string GetRecipientsName(int recipientId)
        {

            var result = dbContext.Recipients.Where(x => x.Id == recipientId).FirstOrDefault();
            if (result != null)
            {
                return result.ReceiverName;
            }
            else
            {
                return "";
            }


        }
        public string GetRecipientsAccountNo(int recipientId)
        {

            var result = dbContext.Recipients.Where(x => x.Id == recipientId).FirstOrDefault();
            if (result != null)
            {
                return result.AccountNo;
            }
            else
            {
                return "";
            }


        }


        public List<CityDropDownViewModel> GetCitiesByName(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.City.Where(x => x.CountryCode == CountryCode)
                              select new CityDropDownViewModel()
                              {
                                  Id = c.Id,
                                  City = c.Name,

                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.City
                              select new CityDropDownViewModel()
                              {

                                  Id = c.Id,
                                  City = c.Name,
                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }


        }

        public List<DropDownViewModel> GetCountryCurrencies(string country = "")
        {
            IQueryable<Country> currencies = dbContext.Country.Where(x => x.Currency != "" && x.Currency != null);
            if (!string.IsNullOrEmpty(country))
            {
                currencies = currencies.Where(x => x.CountryCode == country);

            }
            var result = (from c in currencies.OrderBy(x => x.Currency)
                          select new DropDownViewModel()
                          {
                              Code = c.Currency,
                              Name = c.Currency
                          }
                      ).Distinct().ToList();

            return result;
        }
        public List<DropDownViewModel> GetAPIProviders()
        {
            var result = (from c in dbContext.APIProvider
                          select new DropDownViewModel()
                          {

                              Id = c.Id,
                              Name = c.APIProviderName
                          }
                      ).ToList();

            return result;
        }
        public List<DropDownViewModel> GetBanks(string Country = "")
        {
            var data = dbContext.Bank.ToList();

            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.CountryCode == Country).ToList();

            }
            var result = (from c in data
                          select new DropDownViewModel()
                          {

                              Id = c.Id,
                              Name = c.Name
                          }
                      ).ToList();

            return result;
        }

        public List<DropDownViewModel> GetWalletProvider(string Country = "")
        {
            var data = dbContext.MobileWalletOperator.ToList();

            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();

            }
            var result = (from c in data
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }
                      ).ToList();

            return result;
        }






        public List<CityDropDownViewModel> GetCities(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.City.Where(x => x.CountryCode == CountryCode)
                              select new CityDropDownViewModel()
                              {

                                  Id = c.Id,
                                  City = c.Name,
                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.City
                              select new CityDropDownViewModel()
                              {

                                  Id = c.Id,
                                  City = c.Name,
                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }

        }
        public List<AgentInformation> GetAuxAgentInformation()
        {
            var list = dbContext.AgentInformation.Where(x => x.IsAUXAgent == true).ToList();
            return list;
        }
        public int getPayingAgentStaffId(int agentId)
        {
            var list = dbContext.AgentStaffInformation.Where(x => x.AgentId == agentId).Select(x => x.Id).FirstOrDefault();
            return list;
        }

        internal List<DropDownAgentViewModel> GetAgent(string CountryCode = "", bool IsAUXAgent = false, string city = "")
        {
            IQueryable<AgentInformation> data = dbContext.AgentInformation;
            if (!string.IsNullOrEmpty(CountryCode) && CountryCode.ToLower() != "all")
            {
                data = data.Where(x => x.CountryCode == CountryCode);
            }
            if (!string.IsNullOrEmpty(city))
            {
                city = city.Trim();
                data = data.Where(x => x.City.ToLower() == city.ToLower());
            }
            if (IsAUXAgent)
            {
                data = data.Where(x => x.IsAUXAgent == true);
            }
            else
            {
                data = data.Where(x => x.IsAUXAgent == false);
            }
            var result = (from c in data
                          select new DropDownAgentViewModel()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name,
                              AgentCountry = c.CountryCode,
                              AgentCity = c.City
                          }).ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetFilteredStaffList(string country, string city)
        {
            IQueryable<StaffInformation> staffInfo = dbContext.StaffInformation;
            if (!string.IsNullOrEmpty(country))
            {
                staffInfo = staffInfo.Where(x => x.Country == country);
            }
            if (!string.IsNullOrEmpty(city))
            {
                staffInfo = staffInfo.Where(x => x.City.Trim().ToLower() == city.ToLower());
            }
            var result = (from c in staffInfo.ToList()
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownCardTypeViewModel> GetCardType()
        {
            var result = (from c in dbContext.IdentityCardType
                          select new DropDownCardTypeViewModel()
                          {
                              Id = c.Id,
                              CardType = c.CardType
                          }
                          ).ToList();
            return result;
        }

        internal DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(int mFBCCardID)
        {

            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == mFBCCardID).FirstOrDefault();
            return data;
        }

        public List<DropDownContinentViewModel> GetContinent()
        {
            var result = (from c in dbContext.Continent
                          join d in dbContext.Commission on c.Code equals d.Continent1
                          into j
                          from joined in j.DefaultIfEmpty()
                          select new DropDownContinentViewModel()
                          {
                              Id = c.ID,
                              Code = c.Code,
                              Name = c.Name,
                              Commission = joined == null ? 0 : joined.Rate
                          }
                          ).ToList();
            return result;
        }

        public string GetCardTypeById(int id)
        {
            var name = dbContext.IdentityCardType.Where(x => x.Id == id).FirstOrDefault();
            if (name != null)
            {

                return name.CardType;
            }
            return "";
        }

        public string getCountryNameFromCode(string code)
        {
            var name = dbContext.Country.Where(x => x.CountryCode == code).FirstOrDefault();
            if (name != null)
            {
                return name.CountryName;
            }
            return "";
        }

        internal List<SenderBusinessDocumentation> GetSenderDocumentation(int senderId)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).ToList();
            return data;
        }

        public FaxerInformation GetSenderInfo(int senderId)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return data;
        }
        public List<FaxerInformation> GetAllSenderInfo()
        {
            var data = dbContext.FaxerInformation.ToList();
            return data;
        }
        public List<AgentInformation> GetAllAgentInformation()
        {
            var data = dbContext.AgentInformation.ToList();
            return data;
        }
        public List<SenderRegisteredByAgent> GetSenderRegisteredByAgent()
        {
            var data = dbContext.SenderRegisteredByAgent.ToList();
            return data;
        }
        public string GetSenderName(int senderId)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();

            if (data != null)
            {
                return data.FirstName + ' ' + data.MiddleName + ' ' + data.LastName;


            }
            else
            {
                return "";
            }
        }
        public string GetSenderCountry(int senderId)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();

            if (data != null)
            {
                return data.Country;


            }
            else
            {
                return "";
            }
        }
        public string getCurrencyCodeFromCountry(string code)
        {
            var name = dbContext.Country.Where(x => x.CountryCode == code).FirstOrDefault();
            if (name != null)
            {

                return name.Currency;
            }
            return "";
        }
        public string getCountryCodeFromCurrency(string currency)
        {
            var name = dbContext.Country.Where(x => x.Currency == currency).FirstOrDefault();
            if (name != null)
            {

                return name.CountryCode;
            }
            return "";
        }
        public string getPhoneCodeFromCountry(string countryCode)
        {
            var code = dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault();
            if (code != null)
            {

                return code.CountryPhoneCode;
            }
            return "";
        }
        public string getCurrencySymbol(string countryCode)
        {
            var symbol = dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault();
            if (symbol != null)
            {

                return symbol.CurrencySymbol;
            }
            return "";
        }

        public string getCurrency(string countryCode)
        {
            var currency = dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault();
            if (currency != null)
            {

                return currency.Currency;
            }
            return "";
        }
        public string getStaffName(int id = 0)
        {

            var data = dbContext.StaffInformation.Find(id);
            string name = "";
            if (data != null)
            {
                name = data.FirstName + " " + data.MiddleName + " " + data.LastName;

            }
            return name;

        }
        public List<DropDownViewModel> getStaffList(int id = 0)
        {

            var data = dbContext.StaffInformation.ToList();

            var result = (from c in data
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              CountryCode = c.Country
                          }).ToList();
            return result;

        }
        public string getFaxerCurrencyFromId(int id)
        {
            var data = dbContext.FaxerInformation.Find(id);
            return getCurrencyCodeFromCountry(data.Country);
        }
        public string getBusinessCurrencyFromId(int id)
        {
            var data = dbContext.KiiPayBusinessInformation.Find(id);
            return getCurrencyCodeFromCountry(data.BusinessOperationCountryCode);
        }
        public string getStaffMFSCode(int id = 0)
        {

            if (id == 0)
            {
                return "";
            }
            var data = dbContext.StaffInformation.Find(id);
            return data.StaffMFSCode;

        }
        public string getStaffLoginCode(int id = 0)
        {
            var data = dbContext.StaffLogin.Find(id);
            return data.LoginCode ?? "";

        }

        public string getFaxerName(int id = 0)
        {

            var data = dbContext.FaxerInformation.Find(id);
            if (data == null)
            {
                return "";
            }
            return data.FirstName + " " + data.MiddleName + " " + data.LastName;
        }


        public List<AgentsListDropDown> GetAgents(string City = "")
        {
            var data = dbContext.AgentInformation.ToList();
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City).ToList();
            }


            var agents = (from c in data
                          select new AgentsListDropDown()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name,
                              Country = c.CountryCode
                          }).ToList();
            return agents;

        }
        public List<AgentsListDropDown> GetAuxAgents()
        {

            var agents = (from c in dbContext.AgentInformation.Where(x => x.IsAUXAgent == true)
                          select new AgentsListDropDown()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name,
                              Country = c.CountryCode,
                              AgentCode = c.AccountNo,
                          }).ToList();
            return agents;

        }
        public List<SenderBusinessDocumentation> GetSenderBusinessDocumentations()
        {
            return dbContext.SenderBusinessDocumentation.ToList();
        }
        public List<SenderListDropDown> GetBusinessSenderList(string City = "")
        {
            if (string.IsNullOrEmpty(City))
            {
                var agents = (from c in dbContext.FaxerInformation
                              join d in dbContext.BusinessRelatedInformation on c.Id equals d.FaxerId
                              select new SenderListDropDown()
                              {
                                  senderId = c.Id,
                                  senderName = d.BusinessName.Trim(),
                                  Country = c.Country
                              }).ToList();
                return agents;
            }
            else
            {

                var agents = (from c in dbContext.FaxerInformation.Where(x => x.City == City)
                              join d in dbContext.BusinessRelatedInformation on c.Id equals d.FaxerId
                              select new SenderListDropDown()
                              {
                                  senderId = c.Id,
                                  senderName = d.BusinessName.Trim(),
                                  Country = c.Country
                              }).ToList();
                return agents;
            }
        }
        public List<SenderListDropDown> GetSenderRegisteredByAuxAgent(string country = "", string city = "")
        {
            IQueryable<FaxerInformation> senderInfo = dbContext.FaxerInformation;
            if (!string.IsNullOrEmpty(country) && country != "All")
            {
                senderInfo = senderInfo.Where(x => x.Country == country);
            }
            if (!string.IsNullOrEmpty(city))
            {
                city = city.Trim();
                senderInfo = senderInfo.Where(x => x.City.ToLower() == city.ToLower());
            }

            var senders = (from c in senderInfo
                           join d in dbContext.SenderRegisteredByAgent on c.Id equals d.SenderId
                           select new SenderListDropDown()
                           {
                               senderId = c.Id,
                               senderName = c.FirstName + " " + (!string.IsNullOrEmpty(c.MiddleName) == true ? c.MiddleName + " " : "") + c.LastName,
                               Country = c.Country
                           }).ToList();
            return senders;

        }
        public List<SenderListDropDown> GetSenderList(string City = "")
        {
            if (string.IsNullOrEmpty(City))
            {
                var senders = (from c in dbContext.FaxerInformation

                               select new SenderListDropDown()
                               {
                                   senderId = c.Id,
                                   senderName = c.FirstName.Trim() + " " + 
                                   (string.IsNullOrEmpty(c.MiddleName) == true ? "" : c.MiddleName.Trim() + " ")
                                   + c.LastName.Trim(),
                                   Country = c.Country
                               }).ToList();
                return senders;
            }
            else
            {

                var senders = (from c in dbContext.FaxerInformation.Where(x => (x.City ?? "").Trim() == City.Trim() && x.IsBusiness == false)
                               select new SenderListDropDown()
                               {
                                   senderId = c.Id,
                                   senderName = c.FirstName.Trim() + " " +
                                   (string.IsNullOrEmpty(c.MiddleName) == true ? "" : c.MiddleName.Trim() + " ")
                                   + c.LastName.Trim(),
                                   Country = c.Country
                               }).ToList();
                return senders;
            }
        }
        public string GetAgentAccNo(int agentId)
        {
            string accountNo = dbContext.AgentInformation.Where(x => x.Id == agentId).Select(x => x.AccountNo).FirstOrDefault();
            if (!string.IsNullOrEmpty(accountNo))
            {
                return accountNo;
            }
            return null;
        }
        public string GetAgentStaffMFSCode(int agentStaffId)
        {
            string Code = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).Select(x => x.AgentMFSCode).FirstOrDefault();
            if (!string.IsNullOrEmpty(Code))
            {
                return Code;
            }
            return null;
        }
        public DB.AgentInformation GetAgentInformation(int AgentId)
        {
            var agentInfo = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            if (agentInfo != null)
            {
                return agentInfo;
            }
            else
            {
                return null;
            }

        }

        public DB.AgentLogin GetAgentLoginInformation(int AgentId)
        {
            var agentInfo = dbContext.AgentLogin.Where(x => x.Id == AgentId).FirstOrDefault();
            return agentInfo;
        }
        internal string GetSenderAccountNoBySenderId(int senderId)
        {
            string AccountNumber = dbContext.FaxerInformation.Where(x => x.Id == senderId).Select(x => x.AccountNo).FirstOrDefault();
            return AccountNumber;
        }
        public string getAgentName(int id = 0)
        {
            if (id == 0)
            {
                return "All";
            }
            var data = dbContext.AgentInformation.Find(id);
            string name = data.Name;
            return name;
        }

        public string getBusinessName(int id = 0)
        {
            if (id == 0)
            {
                return "All";
            }
            var data = dbContext.KiiPayBusinessInformation.Find(id);
            string name = data.BusinessName;
            return name;
        }

        public string getBusinessMobileNo(int id = 0)
        {
            if (id == 0)
            {
                return "";
            }
            var data = dbContext.KiiPayBusinessInformation.Find(id);
            string mfsCode = data.BusinessMobileNo;
            return mfsCode;


        }
        public StripeResult ValidateTransactionUsingStripe(TransactionSummaryVM model)
        {
            var senderInfo = Common.AdminSession.SendMoneToKiiPayWalletViewModel;

            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = model.CreditORDebitCardDetials.NameOnCard,
                ExpirationMonth = model.CreditORDebitCardDetials.EndMM,
                ExpiringYear = model.CreditORDebitCardDetials.EndYY,
                Number = model.CreditORDebitCardDetials.CardNumber,
                SecurityCode = model.CreditORDebitCardDetials.SecurityCode,
                billingpostcode = senderInfo.PostCode,
                billingpremise = senderInfo.AddressLine1,
                CurrencyCode = Common.Common.GetCurrencyCode(senderInfo.Country)

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);


            if (!StripeResult.IsValid)
            {

                return StripeResult;
            }

            else
            {



            }
            return StripeResult;


        }
        public void SetDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.AdminSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetDebitCreditCardDetail(string CountryCode)
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel(CountryCode);

            if (Common.AdminSession.CreditDebitDetails != null)
            {

                vm = Common.AdminSession.CreditDebitDetails;
            }
            return vm;
        }
        public bool HasOneCardSaved(int senderId)
        {
            try
            {
                FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
                var data = dbContext.SavedCard.Where(x => x.UserId == senderId && x.IsDeleted == false).ToList();
                if (data.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }




            return false;
        }

    }



    public class DropDownViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string CountryCurrency { get; set; }
    }

    public class DropDownContinentViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Commission { get; set; }
    }

    public class DropDownCardTypeViewModel
    {
        public int Id { get; set; }

        public string CardType { get; set; }
    }

    public class CityDropDownViewModel
    {

        public int Id { get; set; }
        public string City { get; set; }
    }
}