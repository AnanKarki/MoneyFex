using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Windows.Media.Animation;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class STransferFeePercentageServices
    {
        DB.FAXEREntities dbContext = null;

        public STransferFeePercentageServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<DropDownViewModel> GetCountries(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.Country.Where(x => x.CountryCode == CountryCode)
                              select new DropDownViewModel()
                              {

                                  Code = c.CountryCode,
                                  Name = c.CountryName
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.Country
                              select new DropDownViewModel()
                              {

                                  Code = c.CountryCode,
                                  Name = c.CountryName
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }

        }

        public ServiceResult<List<TransferFeePercentageViewModel>> GetTranferfee(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0)
        {
            var data = dbContext.TransferFeePercentage.ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {

                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (TransferMethod != 0)
            {
                if (TransferMethod == 7)
                {
                    data = dbContext.TransferFeePercentage.ToList();
                }
                else
                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod).ToList();

                }

            }

            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType,


            }).Select(d => new TransferFeePercentageViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCountry = Common.Common.GetCountryName(d.FirstOrDefault().SendingCountry),
                ReceivingCountry = Common.Common.GetCountryName(d.FirstOrDefault().ReceivingCountry),
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                Range = d.FirstOrDefault().Range,
                //RangeName = Common.Common.GetEnumDescription(d.FirstOrDefault().Range),
                RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),

                //   KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),

            }).ToList();

            //foreach (var item in result)
            //{
            //    item.SendingCountry = Common.Common.GetCountryName(item.SendingCountry);
            //    item.ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            //}


            return new ServiceResult<List<TransferFeePercentageViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };


        }

        public ServiceResult<List<TransferFeePercentageViewModel>> GetAuxAgentTranferfee(string SendingCountry = "", int AgentId = 0, string Date = "", int Method = 0)
        {
            var data = dbContext.TransferFeePercentage.ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                data = data.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();
            }
            if (AgentId != 0)
            {
                data = data.Where(x => x.AgetnId == AgentId).ToList();
            }
            if (Method > 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)Method).ToList();
            }
            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType,



            }).Select(d => new TransferFeePercentageViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCountry = Common.Common.GetCountryName(d.FirstOrDefault().SendingCountry),
                ReceivingCountry = Common.Common.GetCountryName(d.FirstOrDefault().ReceivingCountry),
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), d.FirstOrDefault().TransferMethod),
                Range = d.FirstOrDefault().Range,
                AgentName = d.FirstOrDefault().AgetnId == 0 ? "" : AgentName((int)d.FirstOrDefault().AgetnId),
                RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),


            }).ToList();



            return new ServiceResult<List<TransferFeePercentageViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };


        }
        public string AgentName(int AgentId)
        {
            var AgentName = dbContext.AgentInformation.Where(x => x.Id == AgentId).Select(x => x.Name).FirstOrDefault();
            if (AgentName == null)
            {
                return "All";
            }
            return AgentName;
        }
        public List<string> GetListofRange(string receivingCountry, string sendingCountry, TransactionTransferMethod transactionTransferMethod, TransactionTransferType transactionTransferType)
        {
            var data = dbContext.TransferFeePercentage.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransferMethod == transactionTransferMethod && x.TransferType == transactionTransferType).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.Range))
            {
                string RangeName = Common.Common.GetEnumDescription(item.Range);

                RangeList.Add(RangeName);
            }

            return RangeList;
        }

        public ServiceResult<IQueryable<TransferFeePercentage>> List()
        {
            return new ServiceResult<IQueryable<TransferFeePercentage>>()
            {
                Data = dbContext.TransferFeePercentage,
                Status = ResultStatus.OK
            };

        }
        public ServiceResult<IQueryable<AgentInformation>> AgentInfo()
        {
            return new ServiceResult<IQueryable<AgentInformation>>()
            {
                Data = dbContext.AgentInformation,
                Status = ResultStatus.OK
            };

        }


        public TransferFeePercentageViewModel GetTranferfeeById(int id)
        {
            string sendingCountry = "";
            string receivingCountry = "";
            var transferFee = dbContext.TransferFeePercentage.Where(x => x.Id == id).FirstOrDefault();
            var transferFeeByCurrency = dbContext.TransferFeePercentageByCurrency.Where(x => x.Id == transferFee.TransfeFeeByCurrencyId).FirstOrDefault();
            if (transferFeeByCurrency != null)
            {
                sendingCountry = transferFeeByCurrency.SendingCountry;
                receivingCountry = transferFeeByCurrency.ReceivingCountry;
            }
            else
            {
                sendingCountry = transferFee.SendingCountry;
                receivingCountry = transferFee.ReceivingCountry;
            }
            var vm = new TransferFeePercentageViewModel()
            {
                SendingCountry = transferFee.SendingCountry,
                ReceivingCountry = transferFee.ReceivingCountry,
                TransferMethod = transferFee.TransferMethod,
                FeeType = transferFee.FeeType,
                TransferType = transferFee.TransferType,
                OtherRange = transferFee.OtherRange,
                Fee = transferFee.Fee,
                Range = transferFee.Range,
                AgentId = transferFee.AgetnId,
                SendingCurrency = transferFee.SendingCurrency,
                ReceivingCurrency = transferFee.ReceivingCurrency,
                TransfeFeeByCurrencyId = transferFee.TransfeFeeByCurrencyId

            };
            return vm;
        }


        public ServiceResult<List<TransferFeePercentageHistoryViewModel>> GetTranferfeeHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int Year = 0, int Month = 0)
        {
            var data = dbContext.TransferFeePercentageHistory.ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (TransferMethod != 0)
            {
                if (TransferMethod == 7)
                {
                    data = dbContext.TransferFeePercentageHistory.ToList();
                }
                else
                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod).ToList();
                }
            }
            if (Year != 0)
            {

                data = data.Where(x => x.CreatedDate.Year == Year).ToList();
            }
            if (Month != 0)
            {
                data = data.Where(x => x.CreatedDate.Month == Month).ToList();
            }
            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType
            }).Select(d => new TransferFeePercentageHistoryViewModel()
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
                //   KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                //    BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),

            }).ToList();

            //foreach (var item in result)
            //{
            //    item.SendingCountry = Common.Common.GetCountryName(item.SendingCountry);
            //    item.ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            //}


            return new ServiceResult<List<TransferFeePercentageHistoryViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };


        }

        public ServiceResult<int> Remove(TransferFeePercentage model)
        {
            dbContext.TransferFeePercentage.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

        public TransferFeePercentageViewModel GetFee(string sendingCountry, string receivingCounrty,
            string sendingCurrency, string receivingCurrency, int transferType, int method, int range, int feeType, int AgentId)
        {
            var transferFees = dbContext.TransferFeePercentage.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
                         && x.TransferMethod == (TransactionTransferMethod)method && x.Range == (TranfserFeeRange)range && x.FeeType == (FeeType)feeType);

            var fee = transferFees.FirstOrDefault();
            if (transferType == 2 || transferType == 4)
            {
                fee = transferFees.Where(x => x.AgetnId == AgentId).FirstOrDefault();
            }
            if (fee == null)
            {
                return null;
            }
            var result = new TransferFeePercentageViewModel()
            {
                Id = fee.Id,
                Range = fee.Range,
                ReceivingCountry = fee.ReceivingCountry,
                ReceivingCurrency = fee.ReceivingCurrency,
                SendingCurrency = fee.SendingCurrency,
                SendingCountry = fee.SendingCountry,
                OtherRange = fee.OtherRange,
                TransferMethod = fee.TransferMethod,
                TransferType = fee.TransferType,
                AgentId = fee.AgetnId,
                Fee = fee.Fee,
                FeeType = fee.FeeType,
                TransfeFeeByCurrencyId = fee.TransfeFeeByCurrencyId
            };
            return result;
        }
        public TransferFeePercentage GetFeeDetials(string sendingCountry = "", string receivingCounrty = "", int transferType = 0, int method = 0, int Range = 0, int FeeType = 0, int AgentId = 0)
        {

            var data = dbContext.TransferFeePercentage.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
            && x.TransferMethod == (TransactionTransferMethod)method && x.Range == (TranfserFeeRange)Range && x.FeeType == (FeeType)FeeType && x.AgetnId == AgentId).FirstOrDefault();
            return data;
        }
        public void CreateTransferFee(TransferFeePercentageViewModel model)
        {
            //AddTransferFeeByCurency(model);
            //  AddTransferFeeAndHistory(model);

            var vm = new TransferFeePercentage()
            {
                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                Range = model.Range,
                OtherRange = model.OtherRange,
                Fee = model.Fee,
                FeeType = model.FeeType,
                CreatedDate = DateTime.Now,
                AgetnId = model.AgentId ?? 0,
                ReceivingCurrency = model.ReceivingCurrency,
                SendingCurrency = model.SendingCurrency,
                ReceivingCountry = model.ReceivingCountry,
                SendingCountry = model.SendingCountry,
            };
            dbContext.TransferFeePercentage.Add(vm);
            dbContext.SaveChanges();

            var history = new TransferFeePercentageHistory
            {

                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                Range = model.Range,
                OtherRange = model.OtherRange,
                Fee = model.Fee,
                FeeType = model.FeeType,
                CreatedDate = DateTime.Now,
                AgentId = model.AgentId ?? 0,
                SendingCurrency = model.SendingCurrency,
                ReceivingCurrency = model.ReceivingCurrency,
                SendingCountry = model.SendingCountry,
                ReceivingCountry = model.ReceivingCountry
            };
            dbContext.TransferFeePercentageHistory.Add(history);
            dbContext.SaveChanges();
        }

        private void AddTransferFeeAndHistory(TransferFeePercentageViewModel model)
        {
            var sendingCountries = CountryCommon.GetCountriesByCurrencyAndCountry(model.SendingCurrency, model.SendingCountry);
            var receivingCountries = CountryCommon.GetCountriesByCurrencyAndCountry(model.ReceivingCurrency, model.ReceivingCountry);

            var vm = new TransferFeePercentage()
            {
                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                Range = model.Range,
                OtherRange = model.OtherRange,
                Fee = model.Fee,
                FeeType = model.FeeType,
                CreatedDate = DateTime.Now,
                AgetnId = model.AgentId ?? 0,
                ReceivingCurrency = model.ReceivingCurrency,
                SendingCurrency = model.SendingCurrency,
                //TransfeFeeByCurrencyId = model.TransfeFeeByCurrencyId
            };

            var history = new TransferFeePercentageHistory
            {

                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                Range = model.Range,
                OtherRange = model.OtherRange,
                Fee = model.Fee,
                FeeType = model.FeeType,
                CreatedDate = DateTime.Now,
                AgentId = model.AgentId ?? 0,
                SendingCurrency = model.SendingCurrency,
                ReceivingCurrency = model.ReceivingCurrency
            };

            foreach (var sendingCountry in sendingCountries)
            {
                foreach (var receivingCountry in receivingCountries)
                {
                    vm.ReceivingCountry = receivingCountry;
                    vm.SendingCountry = sendingCountry;
                    dbContext.TransferFeePercentage.Add(vm);
                    dbContext.SaveChanges();

                    history.SendingCountry = sendingCountry;
                    history.ReceivingCountry = receivingCountry;
                    dbContext.TransferFeePercentageHistory.Add(history);
                    dbContext.SaveChanges();
                }
            }
        }

        private void AddTransferFeeByCurency(TransferFeePercentageViewModel vm)
        {
            decimal fromRange = 0;
            decimal toRange = 0;

            string Range = Common.Common.GetEnumDescription(vm.Range);
            if (Range == "All")
            {
                Range = "0-0";
            }

            string[] RangeArray = Range.Split('-');
            fromRange = decimal.Parse(RangeArray[0]);
            if (Range.Length < 2)
            {
                toRange = int.MaxValue;
            }
            else
            {
                toRange = decimal.Parse(RangeArray[1]);
            }
            var model = new TransferFeePercentageByCurrency()
            {
                AgentId = vm.AgentId ?? 0,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCurrency = vm.SendingCurrency,
                CreatedBy = StaffSession.LoggedStaff.StaffId,
                CreatedDate = DateTime.Now,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                OtherRange = vm.OtherRange,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                FromRange = fromRange,
                ToRange = toRange,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry
            };
            dbContext.TransferFeePercentageByCurrency.Add(model);
            dbContext.SaveChanges();
            vm.TransfeFeeByCurrencyId = model.Id;
        }

        public void CreateTransferFeeHistory(TransferFeePercentageViewModel model)
        {
            var vm = new TransferFeePercentageHistory
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
                AgentId = model.AgentId ?? 0,
                SendingCurrency = model.SendingCurrency,
                ReceivingCurrency = model.ReceivingCurrency
            };
            dbContext.TransferFeePercentageHistory.Add(vm);
            dbContext.SaveChanges();
        }

        public void UpdatetransferFeePercentage(TransferFeePercentageViewModel model)
        {
            var transferFee = dbContext.TransferFeePercentage.Where(x => x.Id == model.Id).FirstOrDefault();
            transferFee.TransferType = model.TransferType;
            transferFee.TransferMethod = model.TransferMethod;
            transferFee.Range = model.Range;
            transferFee.OtherRange = model.OtherRange;
            transferFee.Fee = model.Fee;
            transferFee.FeeType = model.FeeType;
            transferFee.CreatedDate = DateTime.Now;
            transferFee.AgetnId = model.AgentId ?? 0;
            transferFee.ReceivingCurrency = model.ReceivingCurrency;
            transferFee.SendingCurrency = model.SendingCurrency;
            transferFee.SendingCountry = model.SendingCountry;
            transferFee.ReceivingCountry = model.ReceivingCountry;
            dbContext.Entry(transferFee).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        private void RemoveRangeTansferFee(int transferFeeByCurrencyId)
        {
            var transferFee = dbContext.TransferFeePercentage.Where(x => x.TransfeFeeByCurrencyId == transferFeeByCurrencyId);
            dbContext.TransferFeePercentage.RemoveRange(transferFee);
            dbContext.SaveChanges();
        }
    }
}