using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RegisterAuxSenderServices
    {
        FAXEREntities db = null;
        private IQueryable<FaxerInformation> senderInformation;
        private IQueryable<SenderRegisteredByAgent> senderRegisteredByAgent;
        private List<RegisteredAUXSenderViewModel> RegisteredAuxSenders;
        public RegisterAuxSenderServices()
        {
            db = new FAXEREntities();
            RegisteredAuxSenders = new List<RegisteredAUXSenderViewModel>();
        }

        public List<RegisteredAUXSenderViewModel> GetRegisteredAUXSenders(string SendingCountry = "", string City = "", string Date = "",
            string SenderName = "", string AccountNo = "", string Address = "",
            string Telephone = "", string Email = "")
        {
            senderInformation = db.FaxerInformation.Where(x => x.RegisteredByAgent == true);
            senderRegisteredByAgent = db.SenderRegisteredByAgent;
            SearchSenderResgisteredByParam(new SearchParamVM()
            {
                AccountNo = AccountNo,
                City = City,
                SendingCountry = SendingCountry,
                Date = Date,
                Email = Email,
                Telephone = Telephone,
                SenderName = SenderName,
                Address = Address
            });
            GetSendersRegisteredByAux();
            return RegisteredAuxSenders;
        }

        private void GetSendersRegisteredByAux()
        {
            RegisteredAuxSenders = (from c in senderInformation.ToList()
                                    join country in db.Country on c.Country equals country.CountryCode
                                    join d in senderRegisteredByAgent on c.Id equals d.SenderId
                                    join agent in db.AgentInformation on d.AgentId equals agent.Id
                                    select new RegisteredAUXSenderViewModel()
                                    {
                                        Id = c.Id,
                                        AccountNo = c.AccountNo,
                                        Address = c.Address1,
                                        SenderName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                        Country = country.CountryName,
                                        CountryCode = c.Country,
                                        City = c.City,
                                        Email = c.Email,
                                        Telephone = c.PhoneNumber,
                                        StatusName = " No Status in FaxerInformation",
                                        Date = c.CreatedDate.ToString(),
                                        CreationDate = c.CreatedDate,
                                        DateOfBirth = c.DateOfBirth.ToString(),
                                        GenderName = Enum.GetName(typeof(Gender), c.GGender),
                                        AgentName = agent.Name,
                                        AgentAccount = agent.AccountNo
                                    }).OrderByDescending(x => x.CreationDate).ToList();
        }

        private void SearchSenderResgisteredByParam(SearchParamVM searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.SendingCountry))
            {
                senderInformation = senderInformation.Where(x => x.Country == searchParam.SendingCountry);
            }
            if (!string.IsNullOrEmpty(searchParam.City))
            {
                senderInformation = senderInformation.Where(x => x.City == searchParam.City);
            }
            if (!string.IsNullOrEmpty(searchParam.SenderName))
            {
                searchParam.SenderName = searchParam.SenderName.Trim();
                senderInformation = senderInformation.Where(x => x.FirstName.ToLower().Contains(searchParam.SenderName.ToLower()) ||
                                                            x.MiddleName.ToLower().Contains(searchParam.SenderName.ToLower()) ||
                                                           x.LastName.ToLower().Contains(searchParam.SenderName.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.AccountNo))
            {
                searchParam.AccountNo = searchParam.AccountNo.Trim();
                senderInformation = senderInformation.Where(x => x.AccountNo.ToLower().Contains(searchParam.AccountNo.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Address))
            {
                searchParam.Address = searchParam.Address.Trim();
                senderInformation = senderInformation.Where(x => x.Address1.ToLower().Contains(searchParam.Address.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Telephone))
            {
                searchParam.Telephone = searchParam.Telephone.Trim();
                senderInformation = senderInformation.Where(x => x.PhoneNumber.ToLower().Contains(searchParam.Telephone.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Email))
            {
                searchParam.Email = searchParam.Email.Trim();
                senderInformation = senderInformation.Where(x => x.Email.ToLower().Contains(searchParam.Email.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Date))
            {
                string[] DateString = searchParam.Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                senderInformation = senderInformation.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);
            }
        }

    }
}