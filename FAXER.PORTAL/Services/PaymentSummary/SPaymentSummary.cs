using FAXER.PORTAL.Models.PaymentSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services.PaymentSummary
{
    public class SPaymentSummary
    {
        DB.FAXEREntities dbContext = null;
        public SPaymentSummary()
        {
            dbContext = new DB.FAXEREntities();
        }
        public SPaymentSummary(DB.FAXEREntities dbContext)
        {
            this.dbContext = dbContext;
        }
        public void GetTransactionPaymentSummary(PaymentSummaryRequestParamVm requestParam )
        {
            
        
        }

    }
}