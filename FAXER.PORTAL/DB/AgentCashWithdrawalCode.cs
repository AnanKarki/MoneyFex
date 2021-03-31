using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentCashWithdrawalCode
    {

        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public string AgentCode { get; set; }
        public int AgentId { get; set; }
        public int StaffId { get; set; }
        public string StaffCode { get; set; }

        public string WithdrawalCode { get; set; }


        public int CodeGeneratorId { get; set; }
        public DateTime GeneratedDate { get; set; }
        public AgentWithdrawalCodeStatus Status { get; set; }


        public virtual AgentInformation Agent { get; set; }
        public virtual StaffInformation Staff { get; set; }




    }

    public enum AgentWithdrawalCodeStatus
    {

        Use,
        NoUse,
        Expired
    }
}