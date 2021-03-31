using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class APIProviderSelection
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public int ApiProviderId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Apiservice Apiservice { get; set; }
        public int? APIProviderByCurrencyId { get; set; }
        public int? AgentId { get; set; }

    }

    public enum Apiservice
    {

        // Nigeria Api Corrider For Bank 
        VGG,
        // Api Corrider For Bank And Mobile
        TransferZero,
        // Api Corrider For Bank of Ghana
        EmergentApi,
        //  Api Corrider For Mobile Wallet Of Cameroon 
        MTN,
        // Api for mobile wallet and bank deposit  
        Zenith,
        Wari,
        CashPot,
        FlutterWave,
        MoneyWave

    }
}