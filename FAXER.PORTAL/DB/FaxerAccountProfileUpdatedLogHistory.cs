using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxerAccountProfileUpdatedLogHistory
    {
        public int Id { get; set; }
        
        public int UpdatedById { get; set; }

        public string TableName { get; set; }
        public string UpdateByEmailAddress { get; set; }

        public string UpdatedColumn { get; set; }

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public DateTime UpdatedDateTime { get; set; }


    }
}