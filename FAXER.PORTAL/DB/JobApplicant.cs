using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class JobApplicant
    {

        public int Id { get; set; }

        public int JobId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Telephone { get; set; }

        public string Email { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string PositionAppliedFor { get; set; }

        public string CvURL { get; set; }

        public string SupportingStatementURL { get; set; }

        public virtual Career Job { get; set; }
    }
}