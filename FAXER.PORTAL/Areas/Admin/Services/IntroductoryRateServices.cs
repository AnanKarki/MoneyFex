using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class IntroductoryRateServices
    {

        DB.FAXEREntities db = null;
        public IntroductoryRateServices()
        {
            db = new DB.FAXEREntities();
        }

        public bool AddIntroductoryRate(IntroductoryRateVm vm)
        {


            string[] Range = vm.Range.Split('-');
            decimal FromRange = decimal.Parse(Range[0]);
            decimal ToRange = 0;
            if (Range.Length < 2)
            {
                ToRange = int.MaxValue;
            }
            else
            {
                ToRange = decimal.Parse(Range[1]);
            }
            IntroductoryRate introductoryRate = new IntroductoryRate()
            {
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                AgentId = vm.AgentId ?? 0,
                FromRange = FromRange,
                ToRange = ToRange,
                Rate = vm.Rate,
                TransactionTransferMethod = vm.TransactionTransferMethod,
                TransactionTransferType = vm.TransactionTransferType,
                NoOfTransaction = vm.NoOfTransaction,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,

            };
            IntroductoryRateHistory introductoryRateHistory = new IntroductoryRateHistory()
            {
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                AgentId = vm.AgentId ?? 0,
                FromRange = FromRange,
                ToRange = ToRange,
                Rate = vm.Rate,
                TransactionTransferMethod = vm.TransactionTransferMethod,
                TransactionTransferType = vm.TransactionTransferType,
                NoOfTransaction = vm.NoOfTransaction,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,

            };
            AddIntroductoryRateHistory(introductoryRateHistory);
            db.IntroductoryRate.Add(introductoryRate);
            db.SaveChanges();

            return true;
        }

        internal List<ViewModels.IntroductoryRateListVm> GetIntroductoryRate(string sendingCountry, string receivingCountry, int transferType, int agent, int TransferMethod)
        {
            var introductoryRates = db.IntroductoryRate.ToList();

            if (!string.IsNullOrEmpty(sendingCountry))
            {

                introductoryRates = introductoryRates.Where(x => x.SendingCountry == sendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(receivingCountry))
            {
                introductoryRates = introductoryRates.Where(x => x.ReceivingCountry == receivingCountry).ToList();
            }
            if (transferType > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.TransactionTransferType == (TransactionTransferType)transferType).ToList();
            }
            if (TransferMethod > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.TransactionTransferMethod == (TransactionTransferMethod)TransferMethod).ToList();
            }
            if (agent > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.AgentId == agent).ToList();
            }
            CommonServices _commonServices = new CommonServices();
            //var result = (from c in introductoryRates.ToList()
            //              select new IntroductoryRateListVm()
            //              {
            //                  Id = c.Id,
            //                  Range = c.FromRange + "-" + c.ToRange,
            //                  Rate = c.Rate,
            //                  ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
            //                  SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
            //                  TransferMethod = Enum.GetName(typeof(TransactionTransferMethod), c.TransactionTransferMethod),
            //                  AgentName = _commonServices.getAgentName(c.AgentId),

            //              }).ToList();




            var data = introductoryRates.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransactionTransferMethod,
                x.TransactionTransferType,
            }).Select(c => new IntroductoryRateListVm()
            {
                Id = c.FirstOrDefault().Id,
                Range = c.FirstOrDefault().FromRange + "-" + c.FirstOrDefault().ToRange,
                Rate = c.FirstOrDefault().Rate,
                ReceivingCountry = Common.Common.GetCountryName(c.FirstOrDefault().ReceivingCountry),
                SendingCountry = Common.Common.GetCountryName(c.FirstOrDefault().SendingCountry),
                SendingCountryFlag = c.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = c.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferMethod = c.FirstOrDefault().TransactionTransferMethod.ToString(),
                AgentName = c.FirstOrDefault().AgentId != 0 ? getAgentName(c.FirstOrDefault().AgentId) : "All",

                RangeList = GetListofRange(c.FirstOrDefault().ReceivingCountry, c.FirstOrDefault().SendingCountry, c.FirstOrDefault().TransactionTransferMethod
                , c.FirstOrDefault().TransactionTransferType),
            }).ToList();

            return data;

        }
        public string getAgentName(int id)
        {
            string data = db.AgentInformation.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            return data;
        }
        public List<string> GetListofRange(string receivingCountry, string sendingCountry, TransactionTransferMethod transactionTransferMethod, TransactionTransferType transactionTransferType)
        {
            var data = db.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransactionTransferMethod == transactionTransferMethod && x.TransactionTransferType == transactionTransferType).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.FromRange))
            {
                RangeList.Add(item.FromRange + "-" + item.ToRange);
            }

            return RangeList;
        }

        public bool AddIntroductoryRateHistory(IntroductoryRateHistory introductoryRateHistory)
        {
            db.IntroductoryRateHistory.Add(introductoryRateHistory);
            db.SaveChanges();
            return true;
        }

        public bool UpdateIntroductoryRate(IntroductoryRateVm vm)
        {
            string[] Range = vm.Range.Split('-');
            decimal FromRange = decimal.Parse(Range[0]);
            decimal ToRange = 0;
            if (Range.Length < 2)
            {
                ToRange = int.MaxValue;
            }
            else
            {
                ToRange = decimal.Parse(Range[1]);
            }
            IntroductoryRate introductoryRate = new IntroductoryRate()
            {
                Id = vm.Id ?? 0,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                AgentId = vm.AgentId ?? 0,
                FromRange = FromRange,
                ToRange = ToRange,
                Rate = vm.Rate,
                TransactionTransferMethod = vm.TransactionTransferMethod,
                TransactionTransferType = vm.TransactionTransferType,
                NoOfTransaction = vm.NoOfTransaction,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,

            };

            db.Entry<IntroductoryRate>(introductoryRate).State = System.Data.Entity.EntityState.Modified;

            IntroductoryRateHistory introductoryRateHistory = new IntroductoryRateHistory()
            {
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                AgentId = vm.AgentId ?? 0,
                FromRange = FromRange,
                ToRange = ToRange,
                Rate = vm.Rate,
                TransactionTransferMethod = vm.TransactionTransferMethod,
                TransactionTransferType = vm.TransactionTransferType,
                NoOfTransaction = vm.NoOfTransaction,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,

            };
            db.IntroductoryRateHistory.Add(introductoryRateHistory);
            db.SaveChanges();
            return true;

        }

        internal List<IntroductoryRateListVm> GetIntroductoryRateHistory(string sendingCountry, string receivingCountry, int transferType, int agentId, int TransferMethod)
        {

            IQueryable<IntroductoryRateHistory> introductoryRates = db.IntroductoryRateHistory;

            if (!string.IsNullOrEmpty(sendingCountry))
            {

                introductoryRates = introductoryRates.Where(x => x.SendingCountry == sendingCountry);
            }
            if (!string.IsNullOrEmpty(receivingCountry))
            {
                introductoryRates = introductoryRates.Where(x => x.ReceivingCountry == receivingCountry);
            }
            if (transferType > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.TransactionTransferType == (TransactionTransferType)transferType);
            }
            if (TransferMethod > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.TransactionTransferMethod == (TransactionTransferMethod)TransferMethod);
            }
            if (agentId > 0)
            {
                introductoryRates = introductoryRates.Where(x => x.AgentId == agentId);
            }

            var result = (from c in introductoryRates.OrderByDescending(x => x.CreatedDate).ToList()
                          select new IntroductoryRateListVm()
                          {
                              Id = c.Id,
                              Range = c.FromRange + "-" + c.ToRange,
                              Rate = c.Rate,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                              ReceivingCountryFlag = c.ReceivingCountry.ToLower(),
                              SendingCountryFlag = c.SendingCountry.ToLower(),
                              TransferMethod = Enum.GetName(typeof(TransactionTransferMethod), c.TransactionTransferMethod),
                              CreationDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;

        }

        internal bool Delete(int id)
        {
            var data = db.IntroductoryRate.Where(x => x.Id == id).FirstOrDefault();
            db.IntroductoryRate.Remove(data);
            db.SaveChanges();
            return true;
        }

        public IQueryable<IntroductoryRate> GetRates()
        {

            return db.IntroductoryRate;
        }

    }
}