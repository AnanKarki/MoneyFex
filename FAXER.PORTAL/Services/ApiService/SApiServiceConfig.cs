using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services.ApiService
{
    public class SApiServiceConfig
    {
        DB.FAXEREntities dbContext;
        public SApiServiceConfig()
        {
            dbContext = new FAXEREntities();
        }
        public SApiServiceConfig(DB.FAXEREntities dbContext)
        {
            this.dbContext = dbContext;

        }
        public Apiservice? GetApiServiceType(ApiServiceRequestParam requestParam)
        {
            IQueryable<APIProviderSelection> apiProviders = (from c in dbContext.APIProviderSelection.
                                                            Where(x => x.SendingCountry == requestParam.SendingCountry
                                                            && x.ReceivingCountry == requestParam.ReceivingCountry)
                                                             select c);

            apiProviders = filterApiProviderByTransferType(apiProviders, requestParam.TransferType);

            apiProviders = filterApiProviderByTransferMethod(apiProviders, requestParam.TransactionTransferMethod);
            if (requestParam.AgentId > 0)
            {

                apiProviders = filterApiProviderByAgent(apiProviders, requestParam.AgentId);
            }
            if (requestParam.SendingAmount > 0)
            {

                apiProviders = filterApiProviderByRange(apiProviders, requestParam.SendingAmount);
            }
            var data = apiProviders.FirstOrDefault();
            if (data == null)
            {

                return null;
            }
            return data.Apiservice;
        }

        private IQueryable<APIProviderSelection> filterApiProviderByTransferType(IQueryable<APIProviderSelection> apiProviders, TransactionTransferType transferType)
        {
            throw new NotImplementedException();
        }
        private IQueryable<APIProviderSelection> filterApiProviderByTransferMethod(IQueryable<APIProviderSelection> apiProviders, TransactionTransferMethod transactionTransferMethod)
        {
            var data = apiProviders.Where(x => x.TransferMethod == transactionTransferMethod);
            if (data.Count() == 0)
            {
                data = apiProviders.Where(x => x.TransferMethod == TransactionTransferMethod.All);
                if (data.Count() == 0)
                {
                    return apiProviders;
                }
            }
            return data;

        }

        private IQueryable<APIProviderSelection> filterApiProviderByAgent(IQueryable<APIProviderSelection> apiProviders, int agentId)
        {
            var data = apiProviders.Where(x => x.AgentId == agentId);
            if (data.Count() == 0)
            {
                return apiProviders;
            };
            return data;
        }

        public IQueryable<APIProviderSelection> filterApiProviderByRange(IQueryable<APIProviderSelection> apiProviders, decimal Amount)
        {

            var data = apiProviders.Where(x => x.FromRange >= Amount && x.ToRange <= Amount);
            if (data.Count() == 0)
            {
                // this is all condition if 
                // exchange has been set for all range
                data = apiProviders.Where(x => x.FromRange == 0 && x.ToRange == 0);
                if (data.Count() == 0)
                {
                    data = GetRatesByRanges(apiProviders, Amount);
                }
                return data;
            }
            return data;
        }
        private IQueryable<APIProviderSelection> GetRatesByRanges(IQueryable<APIProviderSelection> apiProviders, decimal Amount)
        {
            var ranges = GetRangeArray();
            foreach (var range in ranges)
            {

                try
                {


                    decimal from = 0;
                    decimal to = 0;
                    string[] range_values = range.Split('-');
                    decimal.TryParse(range_values[0], out from);
                    decimal.TryParse(range_values[1], out to);
                    IQueryable<APIProviderSelection> range_api_query;
                    if (to > 0)
                    {
                        range_api_query = apiProviders.Where(x => x.FromRange >= to && x.FromRange <= to);

                    }
                    else
                    {
                        range_api_query = apiProviders.Where(x => x.FromRange >= from);
                    }

                    if (range_api_query.Count() > 0)
                    {
                        return range_api_query;
                        //break;
                    }
                }
                catch (Exception)
                {

                   
                }
            }
            return apiProviders;
        }
        private string[] GetRangeArray()
        {
            string[] ranges = new string[9];
            ranges[0] = "1-100";
            ranges[1] = "101-500";
            ranges[2] = "501-1000";
            ranges[3] = "1001-1500";
            ranges[4] = "1501-2000";
            ranges[5] = "2001-3000";
            ranges[6] = "3001-5000";
            ranges[7] = "5001-10000";
            ranges[8] = "11000";
            return ranges;
        }


    }

    public class ApiServiceRequestParam
    {

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public decimal SendingAmount { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }

        public TransactionTransferType TransferType { get; set; }
        public int AgentId { get; set; }

    }
}