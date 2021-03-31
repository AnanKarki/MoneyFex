using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SavedBank
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string OwnerName { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }

        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public int BranchId { get; set; }

        public string BranchName { get; set; }
        public string Address { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Module UserType { get; set; }
        public bool isDeleted { get; set; }
    }
}