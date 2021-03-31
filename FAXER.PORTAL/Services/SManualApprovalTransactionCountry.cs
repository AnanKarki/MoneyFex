using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SManualApprovalTransactionCountry
    {
        DB.FAXEREntities dbContext;
        public SManualApprovalTransactionCountry()
        {

            dbContext = new DB.FAXEREntities();

        }

        public bool IsManaulApprovalTran(string receivingCountry , string receivingCurrency , TransactionTransferMethod transferMethod)
        {

            var result = dbContext.ManualApprovalTransactionCountry.Where(x => x.ReceivingCountry == receivingCountry
             && x.ReceivingCurrency == receivingCurrency
             && x.TransactionTransferMethod == transferMethod).FirstOrDefault();
            if (result != null) {

                return result.IsManualPaymentEnabled;
            }
            return false;
        
        }
    }
}