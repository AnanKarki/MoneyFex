using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BusinessTransferLimitServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public BusinessTransferLimitServices()
        {
            dbContext = new DB.FAXEREntities();
            _CommonServices = new CommonServices();
        }

        public List<BusinessLimitViewModel> GetBusinessTranferLimit(string Country = "", string City = "", int TransferService = 0,
            int SenderId = 0, bool IsBusiness = true, int frequency = 0)
        {
            var data = dbContext.BusinessLimit.Where(x => x.IsBusiness == IsBusiness);
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country);
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City);
            }
            if (TransferService != 0)
            {
                if (TransferService != 7)
                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferService);

                }
            }
            if (frequency != 0)
            {
                data = data.Where(x => x.Frequency == (AutoPaymentFrequency)frequency);
            }
            if (SenderId != 0)
            {
                data = data.Where(x => x.SenderId == SenderId);
            }

            var result = (from c in data
                          join d in dbContext.BusinessRelatedInformation on c.SenderId equals d.FaxerId into joined
                          from d in joined.DefaultIfEmpty()
                          join e in dbContext.FaxerInformation on c.SenderId equals e.Id into faxerJoined
                          from e in faxerJoined.DefaultIfEmpty()
                          join country in dbContext.Country on c.Country equals country.CountryCode
                          select new BusinessLimitViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = country.CountryName,
                              Frequency = c.Frequency,
                              FrequencyAmount = c.FrequencyAmount,
                              SenderId = c.SenderId,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = c.TransferMethod.ToString(),
                              FrequencyName = c.Frequency.ToString(),
                              BusinessName = d == null ? "All" : d.BusinessName,
                              AccountNumber = e == null ? "" : e.AccountNo,
                              SenderName = e == null ? "All" : e.FirstName + " " + (!string.IsNullOrEmpty(e.MiddleName) == true ? e.MiddleName + " " : "") + e.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownViewModel> GetBuisnessSender(string Country = "")
        {
            if (!string.IsNullOrEmpty(Country))
            {
                var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == true && x.Country == Country).ToList()
                              join d in dbContext.BusinessRelatedInformation on c.Id equals d.FaxerId
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Name = d.BusinessName,
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == true).ToList()
                              join d in dbContext.BusinessRelatedInformation on c.Id equals d.FaxerId
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Name = d.BusinessName,
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }

        }
        public List<DropDownViewModel> GetSender(string Country = "")
        {
            if (!string.IsNullOrEmpty(Country))
            {
                var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == false && x.Country == Country).ToList()
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Name = c.FirstName + " " + c.MiddleName + "" + c.LastName,
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;

            }
            else
            {
                var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == false).ToList()
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Name = c.FirstName + " " + c.MiddleName + "" + c.LastName,
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;

            }
        }

        internal void DeleteLimit(int id)
        {
            var data = dbContext.BusinessLimit.Where(x => x.Id == id).FirstOrDefault();
            dbContext.BusinessLimit.Remove(data);
            dbContext.SaveChanges();
        }

        internal BusinessLimitViewModel GetInfoOfLimit(int id, bool IsBusiness)
        {
            var data = dbContext.BusinessLimit.Where(x => x.Id == id && x.IsBusiness == IsBusiness).ToList();
            var result = (from c in data
                          join d in dbContext.BusinessRelatedInformation on c.SenderId equals d.FaxerId into joined
                          from d in joined.DefaultIfEmpty()
                          select new BusinessLimitViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = c.Country,
                              Frequency = c.Frequency,
                              FrequencyAmount = c.FrequencyAmount,
                              SenderId = c.SenderId,
                              TransferMethod = c.TransferMethod,
                              AccountNumber = c.AccountNumber,
                              BusinessName = d == null ? "" : d.BusinessName,

                          }).FirstOrDefault();
            return result;
        }


        internal void AddLimit(BusinessLimitViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            BusinessLimit model = new BusinessLimit()
            {
                City = vm.City,
                TransferMethod = vm.TransferMethod,
                Country = vm.Country,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                Frequency = vm.Frequency,
                FrequencyAmount = vm.FrequencyAmount,
                SenderId = vm.SenderId,
                AccountNumber = vm.AccountNumber,
                IsBusiness = vm.IsBusiness,

            };

            dbContext.BusinessLimit.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateLimit(BusinessLimitViewModel vm)
        {
            var data = dbContext.BusinessLimit.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.Frequency = vm.Frequency;
            data.FrequencyAmount = vm.FrequencyAmount;
            data.City = vm.City;
            data.Country = vm.Country;
            data.TransferMethod = vm.TransferMethod;
            data.SenderId = vm.SenderId;
            data.AccountNumber = vm.AccountNumber;
            data.IsBusiness = vm.IsBusiness;
            dbContext.Entry<BusinessLimit>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal void AddLimitHistory(BusinessLimitViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            BusinessLimtiHistory model = new BusinessLimtiHistory()
            {
                City = vm.City,
                TransferMethod = vm.TransferMethod,
                Country = vm.Country,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                Frequency = vm.Frequency,
                FrequencyAmount = vm.FrequencyAmount,
                SenderId = vm.SenderId,
                AccountNumber = vm.AccountNumber,
                IsBusiness = vm.IsBusiness



            };

            dbContext.BusinessLimtiHistory.Add(model);
            dbContext.SaveChanges();

        }

        public List<BusinessLimitViewModel> GetBusinessTranferLimitHistory(string Country = "", string City = "", int TransferService = 0, string DateRange = "", int SenderId = 0, bool IsBusiness = true)
        {
            var data = dbContext.BusinessLimtiHistory.Where(x => x.IsBusiness == IsBusiness).ToList();

            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City).ToList();
            }
            if (TransferService != 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferService).ToList();
            }
            if (SenderId != 0)
            {
                data = data.Where(x => x.SenderId == SenderId).ToList();
            }
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                data = data.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();
            }

            var result = (from c in data
                          join d in dbContext.BusinessRelatedInformation on c.SenderId equals d.FaxerId
                          join e in dbContext.FaxerInformation on c.SenderId equals e.Id
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
                              BusinessName = d == null ? "" : d.BusinessName,
                              DateTime = c.CreatedDate.ToString("MM-dd-yyy"),
                              SenderName = e.FirstName + "" + e.MiddleName + "" + e.LastName

                          }).ToList();
            return result;
        }

    }
}