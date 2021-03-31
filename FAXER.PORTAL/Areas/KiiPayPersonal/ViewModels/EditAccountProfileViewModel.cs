using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class EditAccountProfileViewModel
    {
        public const string BindProperty = " KiiPayPersonalUserId,Name ,PhotoUrl , FirstName,MiddleName,LastName , DOB, FullAddress" +
            " ,Country , City, Address1, Address2 ,CountryPhoneCode ,MobileNo ,EmailAddress , IDCardType,IDCardNumber , IDCardExpiringDate" +
            " , IDCardExpiringDay,IDCardExpiringMonth ,IDCardExpiringYear, IDCardIssuingCountry , TempSecurityCode";
        public int KiiPayPersonalUserId { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string FullAddress { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CountryPhoneCode { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string IDCardType { get; set; }
        public string IDCardNumber { get; set; }
        public string IDCardExpiringDate { get; set; }
        public int IDCardExpiringDay { get; set; }
        public Month IDCardExpiringMonth { get; set; }
        public int IDCardExpiringYear { get; set; }
        public string IDCardIssuingCountry { get; set; }
        public string TempSecurityCode { get; set; }

    }
}