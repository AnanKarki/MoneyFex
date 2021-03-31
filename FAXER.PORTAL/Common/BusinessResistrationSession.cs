using FAXER.PORTAL.Areas.Businesses.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class BusinessResistrationSession
    {
        public static BusinessRegistrationsSignUpVm BusinessRegistrationsSignUpVm
        {
            get
            {
                return GetSession("BusinessRegistrationsSignUpVm") as BusinessRegistrationsSignUpVm;
            }
            set
            {
                SetSession("BusinessRegistrationsSignUpVm", value);
            }
        }
        public static BusinessOwnerInformationVm BusinessContactDetails
        {
            get
            {
                return GetSession("BusinessContactDetails") as BusinessOwnerInformationVm;
            }
            set
            {
                SetSession("BusinessContactDetails", value);
            }
        }
        public static PaymentServiceAgreementVm PaymentServiceAgreement
        {
            get
            {
                return GetSession("PaymentServiceAgreement") as PaymentServiceAgreementVm;
            }
            set
            {
                SetSession("PaymentServiceAgreement", value);
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
