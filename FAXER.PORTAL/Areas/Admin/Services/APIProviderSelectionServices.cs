using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class APIProviderSelectionServices
    {
        DB.FAXEREntities dbContext = null;
        public APIProviderSelectionServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void Delete(int id)
        {
            var data = dbContext.APIProviderSelection.Where(x => x.Id == id).FirstOrDefault();
            dbContext.APIProviderSelection.Remove(data);
            dbContext.SaveChanges();
        }

        public void AddAPiProviderSelection(APIProviderSelectionViewModel vm)
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
            APIProviderSelection model = new APIProviderSelection()
            {
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

            dbContext.APIProviderSelection.Add(model);
            dbContext.SaveChanges();
        }

        public List<APIProviderSelectionViewModel> GetAPIProviderSelctionList(string sendingCountry, string receivingCounty, int transferMethod, int transferType)
        {
            IQueryable<APIProviderSelection> data = dbContext.APIProviderSelection;

            if (!string.IsNullOrEmpty(sendingCountry))
            {
                data = data.Where(x => x.SendingCountry == sendingCountry);
            }
            if (!string.IsNullOrEmpty(receivingCounty))
            {
                data = data.Where(x => x.ReceivingCountry == receivingCounty);
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
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.TransferType,
                x.ApiProviderId,
            }).Select(d => new APIProviderSelectionViewModel()
            {
                Id = d.FirstOrDefault().Id,
                SendingCountry = d.FirstOrDefault().SendingCountry,
                ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
                TransferType = d.FirstOrDefault().TransferType,
                TransferMethod = d.FirstOrDefault().TransferMethod,
                TransferMethodName = d.FirstOrDefault().TransferMethod.ToString(),
                TransferTypeName = d.FirstOrDefault().TransferType.ToString(),
                ApiProviderId = d.FirstOrDefault().ApiProviderId,
                ApiProviderName = GetAPIProviderName(d.FirstOrDefault().ApiProviderId),
                Range = Common.Common.GetRangeName(d.FirstOrDefault().FromRange + "-" + d.FirstOrDefault().ToRange),
                RangeList = GetAPIProviderRange(d.FirstOrDefault().SendingCountry, d.FirstOrDefault().ReceivingCountry,
                            d.FirstOrDefault().TransferType, d.FirstOrDefault().TransferMethod),
                AgentId = d.FirstOrDefault().AgentId,
                AgentName = _CommonServices.getAgentName(d.FirstOrDefault().AgentId ?? 0)
            }).ToList();


            return result;
        }
        public APIProviderSelectionViewModel GetPreviousAPIProviderSelction(string sendingCountry, string receivingCountry, int transferType, int transferMethod, string range)
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

            var data = dbContext.APIProviderSelection.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransferMethod == (TransactionTransferMethod)transferMethod && x.TransferType == (TransactionTransferType)transferType && x.FromRange == FromRange && x.ToRange == ToRange).ToList();


            var result = (from c in data
                          select new APIProviderSelectionViewModel()
                          {
                              Id = c.Id,
                              FromRange = c.FromRange,
                              ApiProviderId = c.ApiProviderId,
                              Range = c.FromRange + "-" + c.ToRange,
                              TransferMethod = c.TransferMethod,
                              TransferType = c.TransferType,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountry = c.SendingCountry,
                              Apiservice = c.Apiservice,
                          }).FirstOrDefault();
            return result;
        }

        public string GetAPIProviderName(int apiProviderId)
        {
            string data = dbContext.APIProvider.Where(x => x.Id == apiProviderId).Select(x => x.APIProviderName).FirstOrDefault();
            return data;
        }
        public List<string> GetAPIProviderRange(string SendingCountry, string ReceivingCountry, TransactionTransferType transferType
                                                , TransactionTransferMethod method)
        {
            var data1 = dbContext.APIProviderSelection.Where(x => x.SendingCountry == SendingCountry && x.ReceivingCountry == ReceivingCountry &&
            x.TransferMethod == method && x.TransferType == transferType).ToList();
            List<string> data = new List<string>();
            foreach (var item in data1.OrderBy(x => x.FromRange))
            {
                var range = Common.Common.GetRangeName(item.FromRange + "-" + item.ToRange);
                data.Add(range);
            }

            return data;
        }

        public void UpdateAPiProviderSelection(APIProviderSelectionViewModel vm)
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

            var model = dbContext.APIProviderSelection.Where(x => x.Id == vm.Id).FirstOrDefault();

            model.Id = vm.Id;
            model.SendingCountry = vm.SendingCountry;
            model.ReceivingCountry = vm.ReceivingCountry;
            model.FromRange = FromRange;
            model.ToRange = ToRange;
            model.TransferMethod = vm.TransferMethod;
            model.TransferType = vm.TransferType;
            model.ApiProviderId = vm.ApiProviderId;
            model.Apiservice = vm.Apiservice;
            model.AgentId = vm.AgentId ?? 0;
            dbContext.Entry<APIProviderSelection>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public APIProviderSelectionViewModel GetAPIProviderSelction(int id)
        {
            var data = dbContext.APIProviderSelection.Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new APIProviderSelectionViewModel()
                          {
                              Id = c.Id,
                              ApiProviderId = c.ApiProviderId,
                              FromRange = c.FromRange,
                              Range = c.FromRange + "-" + c.ToRange,
                              ToRange = c.ToRange,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountry = c.SendingCountry,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = c.TransferMethod.ToString(),
                              TransferType = c.TransferType,
                              TransferTypeName = c.TransferType.ToString(),
                              Apiservice = c.Apiservice,
                              ApiserviceName = c.Apiservice.ToString(),
                              AgentId = c.AgentId
                          }).FirstOrDefault();
            return result;
        }
    }
}