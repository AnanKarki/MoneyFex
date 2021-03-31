using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ReceiverLimitServices
    {
        DB.FAXEREntities db = null;
        public ReceiverLimitServices()
        {
            db = new DB.FAXEREntities();
        }
        public List<ReceiverLimit> List()
        {
            var data = db.ReceiverLimit.ToList();
            return data;
        }
        internal List<ViewModels.AgentTransferLimtViewModel> getReceiverLimit(string country, int Services, string City, string Date)
        {
            var ReceiverLimitList = db.ReceiverLimit.ToList();


            if (!string.IsNullOrEmpty(country.Trim()))
            {

                ReceiverLimitList = ReceiverLimitList.Where(x => x.Country.ToLower().Trim() == country.ToLower().Trim()).ToList();
            }
            if (Services > 0)
            {
                ReceiverLimitList = ReceiverLimitList.Where(x => x.TransferMethod == (TransactionTransferMethod)Services).ToList();
            }

            if (!string.IsNullOrEmpty(City.Trim()))
            {
                ReceiverLimitList = ReceiverLimitList.Where(x => x.City.ToLower().Trim() == City.ToLower().Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                ReceiverLimitList = ReceiverLimitList.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();
            }
            CommonServices _commonServices = new CommonServices();

            var data = (from c in ReceiverLimitList
                        select new AgentTransferLimtViewModel()
                        {
                            Id = c.Id,
                            Country = Common.Common.GetCountryName(c.Country),
                            City = c.City,
                            TransferMethodName = c.TransferMethod.ToString(),
                            Amount = c.Amount,
                            FrequencyName = c.Frequency.ToString(),
                            CountryCurrencySymbol = _commonServices.getCurrencySymbol(c.Country),
                            CreatedDate = c.CreatedDate,
                            CreationDate = c.CreatedDate.ToString("MMM-dd-yyyy")
                        }).ToList();

            return data;

        }

        public bool UpdateReceiverLimit(AgentTransferLimtViewModel vm)
        {

            ReceiverLimit ReceiverLimit = new ReceiverLimit()
            {
                Id = vm.Id ?? 0,
                Country = vm.Country,
                ReceiverId = vm.ReceiverId ?? 0,
                Amount = vm.Amount,
                TransferMethod = vm.TransferMethod,
                Frequency = vm.Frequency,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,
                City = vm.City,
            };

            db.Entry<ReceiverLimit>(ReceiverLimit).State = System.Data.Entity.EntityState.Modified;

            ReceiverLimitHistory ReceiverLimitHistory = new ReceiverLimitHistory()
            {
                Country = vm.Country,
                ReceiverId = vm.ReceiverId ?? 0,
                Amount = vm.Amount,
                City = vm.City,
                Frequency = vm.Frequency,
                TransferMethod = vm.TransferMethod,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            db.ReceiverLimitHistory.Add(ReceiverLimitHistory);
            db.SaveChanges();
            return true;

        }
        public bool AddReceiverLimit(AgentTransferLimtViewModel vm)
        {



            ReceiverLimit ReceiverLimit = new ReceiverLimit()
            {

                Country = vm.Country,

                ReceiverId = vm.ReceiverId ?? 0,
                Amount = vm.Amount,

                TransferMethod = vm.TransferMethod,

                Frequency = vm.Frequency,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,

                City = vm.City,



            };
            ReceiverLimitHistory ReceiverLimitHistory = new ReceiverLimitHistory()
            {
                Country = vm.Country,

                ReceiverId = vm.ReceiverId ?? 0,
                AccountNo = vm.AccountNo,
                Amount = vm.Amount,
                City = vm.City,
                Frequency = vm.Frequency,

                TransferMethod = vm.TransferMethod,

                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            AddReceiverLimitHistory(ReceiverLimitHistory);
            db.ReceiverLimit.Add(ReceiverLimit);
            db.SaveChanges();

            return true;
        }
        public bool AddReceiverLimitHistory(ReceiverLimitHistory ReceiverLimitHistory)
        {
            db.ReceiverLimitHistory.Add(ReceiverLimitHistory);
            db.SaveChanges();
            return true;
        }

        internal bool Delete(int id)
        {
            var data = db.ReceiverLimit.Where(x => x.Id == id).FirstOrDefault();
            db.ReceiverLimit.Remove(data);
            db.SaveChanges();
            return true;
        }
        internal List<AgentTransferLimtViewModel> GetReceiverLimitHistory(string Country = "", int Services = 0, string city = "",string Date="")
        {
            CommonServices _commonServices = new CommonServices();



            var ReceiverLimitListHistory = db.ReceiverLimitHistory.ToList();


            if (!string.IsNullOrEmpty(Country.Trim()))
            {

                ReceiverLimitListHistory = ReceiverLimitListHistory.Where(x => x.Country.ToLower().Trim() == Country.ToLower().Trim()).ToList();
            }



            if (Services > 0)
            {
                ReceiverLimitListHistory = ReceiverLimitListHistory.Where(x => x.TransferMethod == (TransactionTransferMethod)Services).ToList();
            }
 
            if (!string.IsNullOrEmpty(city.Trim()))
            {
                ReceiverLimitListHistory = ReceiverLimitListHistory.Where(x => x.City.ToLower().Trim() == city.ToLower().Trim()).ToList();
            }

            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                ReceiverLimitListHistory = ReceiverLimitListHistory.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();
            }
            var result = (from c in ReceiverLimitListHistory.OrderByDescending(x => x.CreatedDate).ToList()
                          select new AgentTransferLimtViewModel()
                          {
                              Id = c.Id,
                              Country = Common.Common.GetCountryName(c.Country),
                              TransferMethodName = c.TransferMethod.ToString(),
                              ReceiverName = c.ReceiverId != 0 ? getReceiverName(c.ReceiverId) : "All",
                              Amount = c.Amount,
                              FrequencyName = c.Frequency.ToString(),
                              CountryCurrencySymbol = _commonServices.getCurrencySymbol(Country),
                              CreationDate = c.CreatedDate.ToString("dd/MM/yyyy"),
                              City=c.City
                          }).ToList();
            return result;

        }

        public string getReceiverName(int id)
        {
            string data = db.Recipients.Where(x => x.Id == id).Select(x => x.ReceiverName).FirstOrDefault();
            return data;
        }


    }
}