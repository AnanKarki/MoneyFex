using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentStaffInformation
    {
        public int Id { get; set; }

        public int AgentId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string BirthCountry { get; set; }
        public Gender Gender { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public bool IsDeleted { get; set; }

        public StaffType AgentStaffType { get; set; }


        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string AgentMFSCode { get; set; }

        #region Id Card Information 

        public string IdCardType { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime IdCardExpiryDate { get; set; }
        public string IssuingCountry { get; set; }

        public string IDDocPhoto { get; set; }

        public bool HasPassport { get; set; }

        public string Passport1Photo { get; set; }
        public string Passport2Photo { get; set; }
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string Id3 { get; set; }

        #endregion

        public virtual AgentInformation Agent { get; set; }



    }

    public class AgentStaffLogin {

        public int Id { get; set; }
        public int AgentStaffId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string ActivationCode { get; set; }
        public int LoginFailedCount { get; set; }
        /// <summary>
        /// Login Code of Agent Come from agent login table
        /// </summary>
        public string AgencyLoginCode { get; set; }

        public string StaffLoginCode { get; set; }

        public bool IsFirstLogin { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DayOfWeek StartDay { get; set; }
        public DayOfWeek EndDay { get; set; }
        public int? UpdataedBy { get; set; }
        public DateTime? UpdataedDate { get; set; }


        public virtual AgentStaffInformation AgentStaff { get; set; }


    }

}