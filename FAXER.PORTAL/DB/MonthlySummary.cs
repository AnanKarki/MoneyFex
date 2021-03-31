using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MonthlySummary
    {
        [Key]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public Month Month { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal TransactionAmount { get; set; }
    }
}