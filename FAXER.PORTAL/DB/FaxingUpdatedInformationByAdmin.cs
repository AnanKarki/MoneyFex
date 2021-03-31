using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxingUpdatedInformationByAdmin
    {
        [Key]
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int NonCardTransactionId { get; set; }
        public string NameOfUpdatingAdmin { get; set; }
        public DateTime Date { get; set; }


        public virtual StaffInformation Staff { get; set; }
        public virtual FaxingNonCardTransaction NonCardTransaction { get; set; }
    }
}