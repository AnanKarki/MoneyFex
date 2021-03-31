using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    //public class FaxerSession
    //{

    //}

    public class FaxerSession_RegisterViewModel
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AccountNo { get; set; }
        public Gender? GGender
        { get; set; } //= null;

    }
    //public enum Gender
    //{
    //    Male = 0,
    //    Female = 1,
    //}

    public class FaxerSession_FaxerIdentification
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string IdCardType { get; set; }


        public string IdCardNumber { get; set; }


        public DateTime IdCardExpiringDate { get; set; }

        public string IssuingCountry { get; set; }

        public string CardUrl { get; set; }

        public bool CheckAmount { get; set; }

       
    }
}