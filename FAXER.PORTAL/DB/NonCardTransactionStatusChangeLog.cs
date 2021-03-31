using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// Transaction can be hold and unhold by admin the table is use to
    /// </summary>
    public class NonCardTransactionStatusChangeLog
    {

        public int Id { get; set; }

        public int TransactionId { get; set; }


        public int StaffId { get; set; }
        public FaxingStatus OldStatus { get; set; }


        public FaxingStatus NewStatus { get; set; }

        public DateTime StatusChangedDate { get; set; }



    }
}