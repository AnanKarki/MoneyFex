using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class MiscSession
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
        public static string PasswordSecurityCode
        {
            get
            {
                return (GetSession("PasswordSecurityCode") ?? "").ToString();
            }
            set { SetSession("PasswordSecurityCode", value); }
        }
        public static string EditProfielPhoneCode
        {
            get
            {
                return GetSession("EditProfielPhoneCode").ToString();
            }
            set { SetSession("EditProfielPhoneCode", value); }
        }

        public static string ForgotPasswordEmailAddress
        {
            get
            {
                return GetSession("ForgotPasswordEmailAddress").ToString();
            }
            set { SetSession("ForgotPasswordEmailAddress", value); }
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