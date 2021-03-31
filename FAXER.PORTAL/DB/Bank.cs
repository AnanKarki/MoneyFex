using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Bank
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
        public string CountryCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int PayoutProviderId { get; set; }
        public bool IsDeleted { get; set; }

    }

    public class BankBranch
    {

        public int Id { get; set; }
        public int BankId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string BranchAddress { get; set; }
        public virtual Bank Bank { get; set; }

    }

}