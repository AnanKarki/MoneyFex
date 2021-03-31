using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Logger
    {
        public int Id { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateTime { get; set; }
        public string Source { get; set; }
    }
    public enum ErrorType
    {
        VGG,
        TransferZero,
        EmergentApi,
        MTN,
        Zenith,
        Wari,
        PaymentGateway,
        UnSpecified,
        CashPot,
        FlutterWave,
        T365,
        MoneyWave
    }
}