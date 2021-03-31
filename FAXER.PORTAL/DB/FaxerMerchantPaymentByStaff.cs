using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxerMerchantPaymentByStaff
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public int FaxerId { get; set; }
        public int StaffId { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public int PaymentMethod { get; set; }
        public string Reference { get; set; }
        public DateTime DateAndTime { get; set; }

        public virtual KiiPayBusinessInformation KiiPayBusinessInformation { get; set; }
        public virtual FaxerInformation Faxer { get; set; }
        public virtual StaffInformation Staff { get; set; }

    }
}