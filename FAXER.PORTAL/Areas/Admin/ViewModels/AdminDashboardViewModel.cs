using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int Id { get; set; }

        public string CountryCode { get; set; }

        public string City { get; set; }

        public string Year { get; set; }
        public Month Month { get; set; }
        public string Currency { get; set; }
        public int TotalAgents { get; set; }
        public int TotalBusinessMerchants { get; set; }
        public int TotalSenders { get; set; }
        public int TotalCardUsers { get; set; }
        public int TotalNonCardUsers { get; set; }
        public decimal AmountNotwithdrawnFromMFTC { get; set; }
        public decimal AmountNotwithdrawnfromMFBC { get; set; }
        public int TotalLocalTransaction { get; set; }
        public int TotalCompleteMerchantPayment { get; set; }
        public int TotalStaffs { get; set; }
        public int TotalFeesPaid { get; set; }
        public int TotalWebVisitors { get; set; }
        public int TotalRefundRequest { get; set; }
        public int TotalCompletedRefund { get; set; }
        public int TotalUnCompleteRefund { get; set; }
        public int TotalUpdateonTransferInfo { get; set; }
        public decimal TotalCommissionPaid { get; set; }
        public int TotalCommissionUnPaid { get; set; }
        public int TotalFailTransaction { get; set; }
        public int TotalDeletedStaffs { get; set; }
        public int TotalDeletedSenders { get; set; }
        public int TotalDeletedCardUsers { get; set; }
        public int TotalDeletedBusinessMerchants { get; set; }
        public int TotalDeletedAgents { get; set; }
        public int TotalDeletedNonCardUsers { get; set; }   

    }
}