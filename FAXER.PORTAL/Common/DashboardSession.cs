using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class DashboardSession
    {
        public static string CountryCode
        {

            get
            {
                return GetSession("CountryCode").ToString();
            }
            set
            {
                SetSession("CountryCode", value);
            }
        }

        public static string City
        {

            get
            {
                return GetSession("City").ToString();
            }
            set
            {
                SetSession("City", value);
            }
        }

        private static object GetSession(string key)
        {
            return HttpContext.Current.Session[key];
        }
        private static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }

    }
}