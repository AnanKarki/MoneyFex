using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayToBankAccountSessionViewModel
    {
        public int Id { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryPhoneCode { get; set; }
        public string ReceivingCountryCurrency { get; set; }
        public string ReceivingCoutnryCurrencySymbol { get; set; }
        public PaymentType PaymentType { get; set; }
        public string AccountOwnerName { get; set; }
        public string BankAccountNumber { get; set; }
        public string AccountHolderPhoneNumber { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        public string BranchCode { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal SmsFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentReferene  { get; set; }
    }
}