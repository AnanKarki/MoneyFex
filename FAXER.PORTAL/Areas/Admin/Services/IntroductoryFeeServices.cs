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
    public class IntroductoryFeeServices
    {
        DB.FAXEREntities dbContext = null;

        public IntroductoryFeeServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public ServiceResult<List<IntroductoryFeeViewModel>> GetIntroductoryfee(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0)
        {
            var data = dbContext.IntroductoryFee.ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {

                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType > 0)
            {

                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (TransferMethod > 0)
            {

                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod).ToList();

            }
            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType
            }).Select(d => new IntroductoryFeeViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCountry = d.FirstOrDefault().SendingCountry,
                ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                Range = d.FirstOrDefault().Range,
                RangeName = Common.Common.GetEnumDescription(d.FirstOrDefault().Range),
                RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),
            }).ToList();

            return new ServiceResult<List<IntroductoryFeeViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };

        }

        public List<string> GetListofRange(string receivingCountry, string sendingCountry, TransactionTransferMethod transferMethod, TransactionTransferType transferType)
        {
            var data = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
          x.TransferMethod == transferMethod && x.TransferType == transferType).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.Range))
            {
                string RangeName = Common.Common.GetEnumDescription(item.Range);

                RangeList.Add(RangeName);
            }

            return RangeList;
        }

        public ServiceResult<List<IntroductoryFeeHistoryViewModel>> GetIntroductoryfeeHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int Year = 0, int Month = 0)
        {
            var data = dbContext.IntroductoryFeeHistory.ToList();


            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {

                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType > 0)
            {

                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (TransferMethod > 0)
            {

                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod).ToList();


            }
            if (Year > 0)
            {

                data = data.Where(x => x.CreatedDate.Year == Year).ToList();
            }
            if (Month > 0)
            {

                data = data.Where(x => x.CreatedDate.Month == Month).ToList();
            }
            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType
            }).Select(d => new IntroductoryFeeHistoryViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCountry = d.FirstOrDefault().SendingCountry,
                ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                Range = d.FirstOrDefault().Range,
                RangeName = Common.Common.GetEnumDescription(d.FirstOrDefault().Range),
                CreatedDate = d.FirstOrDefault().CreatedDate.ToString("dd/MM/yyyy"),

            }).ToList();

            return new ServiceResult<List<IntroductoryFeeHistoryViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };


        }


        public ServiceResult<IQueryable<IntroductoryFee>> List()
        {
            return new ServiceResult<IQueryable<IntroductoryFee>>()
            {
                Data = dbContext.IntroductoryFee,
                Status = ResultStatus.OK
            };

        }

        public ServiceResult<int> Remove(IntroductoryFee model)
        {
            dbContext.IntroductoryFee.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

        public IntroductoryFee GetFeeDetials(string sendingCountry = "", string receivingCounrty = "", int transferType = 0, int method = 0, int Range = 0, int FeeType = 0, int NumberOfTransaction = 0, int AgentId = 0)
        {
            var data = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
            && x.TransferMethod == (TransactionTransferMethod)method && x.Range == (TranfserFeeRange)Range && x.FeeType == (FeeType)FeeType && x.NumberOfTransaction == (NumberOfTransaction)NumberOfTransaction && x.AgentId == AgentId).FirstOrDefault();

            return data;
        }

        public IntroductoryFeeViewModel GetIntroductoryfeeById(int id)
        {
            var introductoryfee = dbContext.IntroductoryFee.Where(x => x.Id == id).FirstOrDefault();
            var vm = new IntroductoryFeeViewModel()
            {
                SendingCountry = introductoryfee.SendingCountry,
                ReceivingCountry = introductoryfee.ReceivingCountry,
                TransferMethod = introductoryfee.TransferMethod,
                FeeType = introductoryfee.FeeType,
                TransferType = introductoryfee.TransferType,
                OtherRange = introductoryfee.OtherRange,
                Fee = introductoryfee.Fee,
                Range = introductoryfee.Range,
                NumberOfTransaction = introductoryfee.NumberOfTransaction,
                AgentId = introductoryfee.AgentId,

            };
            return vm;
        }


        public void CreateIntroductoryFee(IntroductoryFeeViewModel model)
        {
            var introductoryFee = GetFeeDetials(model.SendingCountry, model.ReceivingCountry, (int)model.TransferType, (int)model.TransferMethod, (int)model.Range, (int)model.FeeType);

            if (introductoryFee != null)
            {
                introductoryFee.Fee = model.Fee;
                dbContext.Entry<IntroductoryFee>(introductoryFee).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                var vm = new IntroductoryFee()
                {

                    ReceivingCountry = model.ReceivingCountry,
                    SendingCountry = model.SendingCountry,
                    TransferType = model.TransferType,
                    TransferMethod = model.TransferMethod,
                    Range = model.Range,
                    OtherRange = model.OtherRange,
                    Fee = model.Fee,
                    FeeType = model.FeeType,
                    CreatedDate = DateTime.Now,
                    NumberOfTransaction = model.NumberOfTransaction,
                    AgentId = model.AgentId ?? 0
                };
                var data = dbContext.IntroductoryFee.Add(vm);
                dbContext.SaveChanges();
            }
        }

        public void CreateIntroductoryFeeHistory(IntroductoryFeeViewModel model)
        {
            var vm = new IntroductoryFeeHistory
            {
                ReceivingCountry = model.ReceivingCountry,
                SendingCountry = model.SendingCountry,
                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                Range = model.Range,
                OtherRange = model.OtherRange,
                Fee = model.Fee,
                FeeType = model.FeeType,
                CreatedDate = DateTime.Now,
                NumberOfTransaction = model.NumberOfTransaction,
                AgentId = model.AgentId ?? 0


            };
            var data = dbContext.IntroductoryFeeHistory.Add(vm);
            dbContext.SaveChanges();

        }

        public IntroductoryFee UpdateIntroductoryFeePercentage(IntroductoryFeeViewModel model)
        {

            var introductoryFee = dbContext.IntroductoryFee.Where(x => x.Id == model.Id).FirstOrDefault();
            introductoryFee.ReceivingCountry = model.ReceivingCountry;
            introductoryFee.SendingCountry = model.SendingCountry;
            introductoryFee.FeeType = model.FeeType;
            introductoryFee.Fee = model.Fee;
            introductoryFee.TransferMethod = model.TransferMethod;
            introductoryFee.TransferType = model.TransferType;
            introductoryFee.Range = model.Range;
            introductoryFee.OtherRange = model.OtherRange;
            introductoryFee.CreatedDate = introductoryFee.CreatedDate;
            introductoryFee.NumberOfTransaction = model.NumberOfTransaction;
            introductoryFee.AgentId = model.AgentId ?? 0;


            dbContext.Entry<IntroductoryFee>(introductoryFee).State = EntityState.Modified;
            dbContext.SaveChanges();
            return introductoryFee;
        }



    }
}