using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RefundBalanceOnDeletedMFTCViewModel
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string FaxerName { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string MFTCNumber { get; set; }
        public decimal AmountOnMFTC { get; set; }
        public string Currency { get; set; }
        public string MFTCDeletionDate { get; set; }
        public string MFTCDeleter { get; set; }
        public string AdminRefunder { get; set; }
        public string AdminRefunderMFSCode { get; set; }
        public string RefundConfirm { get; set; }
        public string RefundDate { get; set; }
        public string RefundTime { get; set; }
        public string ReasonForRefundRequest { get; set; }

        public string StaffLoginCode { get; set; }


    }
}