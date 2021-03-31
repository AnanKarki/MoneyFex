using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{

    public class APIProviderSelectionByCurrencyServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices commonServices = new CommonServices();
        public APIProviderSelectionByCurrencyServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void Delete(int id)
        {
            var data = dbContext.APIProviderSelectionByCurrency.Where(x => x.Id == id).FirstOrDefault();
            var masterExisting = dbContext.APIProviderSelection.Where(x => x.APIProviderByCurrencyId == data.Id).ToList();
            if (masterExisting.Count > 0)
            {
                dbContext.APIProviderSelection.RemoveRange(masterExisting);
                dbContext.SaveChanges();
            }

            dbContext.APIProviderSelectionByCurrency.Remove(data);
            dbContext.SaveChanges();
        }

        public void AddAPiProviderSelectionByCurrency(APIProviderSelectionViewModel vm)
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
            APIProviderSelectionByCurrency model = new APIProviderSelectionByCurrency()
            {
                SendingCurrency = vm.SendingCurrency,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                FromRange = FromRange,
                ToRange = ToRange,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDate = DateTime.Now,
                ApiProviderId = vm.ApiProviderId,
                Apiservice = vm.Apiservice,
                AgentId = vm.AgentId ?? 0

            };

            var data = dbContext.APIProviderSelectionByCurrency.Add(model);
            dbContext.SaveChanges();
            var sendingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.SendingCurrency, vm.SendingCountry);
            var receivingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.ReceivingCurrency, vm.ReceivingCountry);

            foreach (var sendingCountry in sendingCountries)
            {
                foreach (var receivingCountry in receivingCountries)
                {
                    APIProviderSelection APICountry = new APIProviderSelection()
                    {
                        SendingCountry = sendingCountry,
                        ReceivingCountry = receivingCountry,
                        FromRange = FromRange,
                        ToRange = ToRange,
                        TransferMethod = vm.TransferMethod,
                        TransferType = vm.TransferType,
                        CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                        CreatedDate = DateTime.Now,
                        ApiProviderId = vm.ApiProviderId,
                        Apiservice = vm.Apiservice,
                        APIProviderByCurrencyId = data.Id,
                        AgentId = vm.AgentId ?? 0
                    };
                    dbContext.APIProviderSelection.Add(APICountry);
                    dbContext.SaveChanges();

                }
            }
        }

        public List<APIProviderSelectionViewModel> GetAPIProviderSelctionByCurrencyList(string sendingCurrency, string receivingCurrency, int transferMethod, int transferType)
        {
            IQueryable<APIProviderSelectionByCurrency> data = dbContext.APIProviderSelectionByCurrency;

            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                data = data.Where(x => x.SendingCurrency == sendingCurrency);
            }
            if (!string.IsNullOrEmpty(receivingCurrency))
            {
                data = data.Where(x => x.ReceivingCurrency == receivingCurrency);
            }
            if (transferMethod > 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (transferType > 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)transferType);
            }
            CommonServices _CommonServices = new CommonServices();
            var result = data.ToList().GroupBy(x => new
            {
                x.SendingCurrency,
                x.ReceivingCurrency,
                x.TransferMethod,
                x.TransferType,
                x.ApiProviderId,
            }).Select(d => new APIProviderSelectionViewModel()
            {
                Id = d.FirstOrDefault().Id,
                SendingCurrency = d.FirstOrDefault().SendingCurrency,
                ReceivingCurrency = d.FirstOrDefault().ReceivingCurrency,

                TransferType = d.FirstOrDefault().TransferType,
                TransferMethod = d.FirstOrDefault().TransferMethod,
                TransferMethodName = d.FirstOrDefault().TransferMethod.ToString(),
                TransferTypeName = d.FirstOrDefault().TransferType.ToString(),
                ApiProviderId = d.FirstOrDefault().ApiProviderId,
                ApiProviderName = GetAPIProviderName(d.FirstOrDefault().ApiProviderId),
                Range = Common.Common.GetRangeName(d.FirstOrDefault().FromRange + "-" + d.FirstOrDefault().ToRange),
                RangeList = GetAPIProviderRange(d.FirstOrDefault().SendingCurrency, d.FirstOrDefault().ReceivingCurrency,
                            d.FirstOrDefault().TransferType, d.FirstOrDefault().TransferMethod),
                AgentId = d.FirstOrDefault().AgentId,
                AgentName = _CommonServices.getAgentName(d.FirstOrDefault().AgentId ?? 0)
            }).ToList();


            return result;
        }

        public APIProviderSelectionViewModel GetPreviousAPIProviderSelctionByCurrency(string sendingCurrency, string receivingCurrency, string sendigCountry,
            string receivingCountry, int transferType, int transferMethod, string range, int agentId)
        {

            string[] Range = range.Split('-');
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

            var data = dbContext.APIProviderSelectionByCurrency.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency &&
            x.SendingCountry == sendigCountry && x.ReceivingCountry == receivingCountry && x.TransferMethod == (TransactionTransferMethod)transferMethod
            && x.TransferType == (TransactionTransferType)transferType && x.FromRange == FromRange && x.ToRange == ToRange);
            if (transferType == 2 || transferType == 4) {
                data = data.Where(x => x.AgentId == agentId);
            }

            var result = (from c in data
                          select new APIProviderSelectionViewModel()
                          {
                              Id = c.Id,
                              FromRange = c.FromRange,
                              ApiProviderId = c.ApiProviderId,
                              Range = c.FromRange + "-" + c.ToRange,
                              TransferMethod = c.TransferMethod,
                              TransferType = c.TransferType,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              Apiservice = c.Apiservice,
                              AgentId = c.AgentId,
                              SendingCountry = c.SendingCountry,
                              ReceivingCountry = c.ReceivingCountry
                          }).FirstOrDefault();
            return result;
        }

        public string GetAPIProviderName(int apiProviderId)
        {
            string data = dbContext.APIProvider.Where(x => x.Id == apiProviderId).Select(x => x.APIProviderName).FirstOrDefault();
            return data;
        }
        public List<string> GetAPIProviderRange(string SendingCurrency, string ReceivingCurrency, TransactionTransferType transferType
                                                , TransactionTransferMethod method)
        {
            var data1 = dbContext.APIProviderSelection.Where(x => x.SendingCountry == SendingCurrency && x.ReceivingCountry == ReceivingCurrency &&
            x.TransferMethod == method && x.TransferType == transferType).ToList();
            List<string> data = new List<string>();
            foreach (var item in data1.OrderBy(x => x.FromRange))
            {
                var range = Common.Common.GetRangeName(item.FromRange + "-" + item.ToRange);
                data.Add(range);
            }

            return data;
        }

        public void UpdateAPiProviderSelectionByCurrency(APIProviderSelectionViewModel vm)
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

            var model = dbContext.APIProviderSelectionByCurrency.Where(x => x.Id == vm.Id).FirstOrDefault();
            model.SendingCurrency = vm.SendingCurrency;
            model.ReceivingCurrency = vm.ReceivingCurrency;
            model.FromRange = FromRange;
            model.ToRange = ToRange;
            model.TransferMethod = vm.TransferMethod;
            model.TransferType = vm.TransferType;
            model.ApiProviderId = vm.ApiProviderId;
            model.Apiservice = vm.Apiservice;
            model.AgentId = vm.AgentId ?? 0;
            dbContext.Entry<APIProviderSelectionByCurrency>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            var masterExisting = dbContext.APIProviderSelection.Where(x => x.APIProviderByCurrencyId == model.Id).ToList();
            dbContext.APIProviderSelection.RemoveRange(masterExisting);
            dbContext.SaveChanges();

            var sendingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.SendingCurrency , vm.SendingCountry);
            var ReceivingCountry = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.ReceivingCurrency, vm.ReceivingCountry);

            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceivingCountry)
                {
                    APIProviderSelection APICountry = new APIProviderSelection()
                    {
                        SendingCountry = item,
                        ReceivingCountry = receivingCountry,
                        FromRange = FromRange,
                        ToRange = ToRange,
                        TransferMethod = vm.TransferMethod,
                        TransferType = vm.TransferType,
                        CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                        CreatedDate = DateTime.Now,
                        ApiProviderId = vm.ApiProviderId,
                        Apiservice = vm.Apiservice,
                        AgentId = vm.AgentId ?? 0,
                        APIProviderByCurrencyId = model.Id
                    };
                    dbContext.APIProviderSelection.Add(APICountry);
                    dbContext.SaveChanges();
                }
            }
        }

        public APIProviderSelectionViewModel GetAPIProviderSelctionByCurrency(int id)
        {
            var data = dbContext.APIProviderSelectionByCurrency.Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new APIProviderSelectionViewModel()
                          {
                              Id = c.Id,
                              ApiProviderId = c.ApiProviderId,
                              FromRange = c.FromRange,
                              Range = c.FromRange + "-" + c.ToRange,
                              ToRange = c.ToRange,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = c.TransferMethod.ToString(),
                              TransferType = c.TransferType,
                              TransferTypeName = c.TransferType.ToString(),
                              Apiservice = c.Apiservice,
                              ApiserviceName = c.Apiservice.ToString(),
                              AgentId = c.AgentId,
                              SendingCountry = c.SendingCountry,
                              ReceivingCountry = c.ReceivingCountry
                          }).FirstOrDefault();

            return result;

        }
    }
}