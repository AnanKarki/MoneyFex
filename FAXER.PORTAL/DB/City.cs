using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// All city information should be inserted into database from setup
    /// registration (Agent,business, faxer, staff, card user)
    /// </summary>
    public class City
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public Module Module { get; set; }

    }
    public enum Module
    {
        Faxer,
        CardUser,
        BusinessMerchant,
        Agent,
        Staff,
        KiiPayBusiness,
        KiiPayPersonal
    }

}