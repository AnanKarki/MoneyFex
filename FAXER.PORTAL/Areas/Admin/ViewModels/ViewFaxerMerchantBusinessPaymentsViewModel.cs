using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
   

    public class ViewFaxerMerchantBusinessPaymentsViewModel
    {

        public int Id { get; set; }
        public string FaxerName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerCity { get; set; }
        public string MerchantName { get; set; }
        public string MerchantAccountNo { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCity { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentCurrency { get; set; }
        public string PaymentReference { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string PaymentByAdmin { get; set; }
        public string AdminPayer { get; set; }
        public string AutoPaymentEnable { get; set; }
        public decimal AutoPaymentAmount { get; set; }
        public string FrequencyOfAutoPay { get; set; }
    }
}