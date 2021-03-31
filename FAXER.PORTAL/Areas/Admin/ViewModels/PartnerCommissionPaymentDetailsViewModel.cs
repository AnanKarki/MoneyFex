using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PartnerCommissionPaymentDetailsViewModel
    {
        public int Id{ get; set; }
        public string  PartnerName{ get; set; }
        public int PartnerId{ get; set; }
        public string AccountNo{ get; set; }
        public string Country{ get; set; }
        public string  TransactionDate{ get; set; }
        public decimal  CommissionRate{ get; set; }
        public string  CommissionType{ get; set; }
        public string  ServiceType{ get; set; }
        public int  TotalNoOfTransaction{ get; set; }
        public decimal  TotalAmount{ get; set; }
        public decimal  CurrencySymbol{ get; set; }
        public decimal  TotalCommission{ get; set; }
        public decimal  Status{ get; set; }
    }
}