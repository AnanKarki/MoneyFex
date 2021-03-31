using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Common
{
    public class StaffSession
    {

        public static Areas.Staff.ViewModels.StaffDetailsViewModel StaffDetails
        {
            get
            {
                return GetSession("StaffDetails") as Areas.Staff.ViewModels.StaffDetailsViewModel;
            }
            set
            {
                SetSession("StaffDetails", value);
            }
        }
     
        public static StaffContactDetailsViewModel StaffContactDetails
        {
            get
            {
                return GetSession("StaffContactDetails") as StaffContactDetailsViewModel;
            }
            set
            {
                SetSession("StaffContactDetails", value);
            }
        }
        public static StaffContactDetails_1ViewModel StaffContactDetails1
        {
            get
            {
                return GetSession("StaffContactDetails1") as StaffContactDetails_1ViewModel;
            }
            set
            {
                SetSession("StaffContactDetails1", value);
            }
        }
        public static StaffContactDetails_2ViewModel StaffContactDetails2
        {
            get
            {
                return GetSession("StaffContactDetails2") as StaffContactDetails_2ViewModel;
            }
            set
            {
                SetSession("StaffContactDetails2", value);
            }
        }
        public static StaffNextOfKinViewModel StaffNextOfKinDetails
        {
            get
            {
                return GetSession("StaffNextOfKinDetails") as StaffNextOfKinViewModel;
            }
            set
            {
                SetSession("StaffNextOfKinDetails", value);
            }
        }


        public static StaffComplianceDocumentationViewModel StaffComplianceDocumentation
        {
            get
            {
                return GetSession("StaffComplianceDocumentation") as StaffComplianceDocumentationViewModel;
            }
            set
            {
                SetSession("StaffComplianceDocumentation", value);
            }
        }
        public static StaffInformation StaffInformation
        {
            get
            {
                return GetSession("StaffInformation") as StaffInformation;
            }
            set
            {
                SetSession("StaffInformation", value);
            }
        }
        public static StaffLogin StaffLogin {

            get
            {
                return GetSession("StaffLogin") as StaffLogin;
            }
            set
            {
                SetSession("StaffLogin", value);
            }
        }
        public static string StaffPasswordSecurityCode {

            get
            {
                return GetSession("StaffPasswordSecurityCode").ToString();
            }
            set
            {
                SetSession("StaffPasswordSecurityCode", value);
            }
        }
        public static string StaffEmailAddress
        {

            get
            {
                return GetSession("StaffEmailAddress").ToString();
            }
            set
            {
                SetSession("StaffEmailAddress", value);
            }
        }
        public static LoggedStaff LoggedStaff
        {

            get
            {
                return GetSession("LoggedStaff") as LoggedStaff;
            }
            set
            {
                SetSession("LoggedStaff", value);
            }
        }
        public static bool IsFromAuxAgnet
        {
            get
            {
                return (bool)(GetSession("IsFromAuxAgnet") ?? false) ;
            }
            set
            {
                SetSession("IsFromAuxAgnet", value);
            }
        }

        public static string StaffTimeZone
        {
            get
            {
                return GetSession("StaffTimeZone").ToString();
            }
            set
            {
                SetSession("StaffTimeZone", value);
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

        #region transfer Session
        public static CommonEnterAmountViewModel CommonEnterAmountViewModel
        {

            get
            {
                return (GetSession("CommonEnterAmountViewModel") as CommonEnterAmountViewModel);
            }
            set
            {
                SetSession("CommonEnterAmountViewModel", value);
            }
        }

        public static NonCardReceiversDetailsViewModel NonCardReceiversDetails
        {
            get
            {
                return (GetSession("NonCardReceiversDetails") as NonCardReceiversDetailsViewModel);
            }
            set
            {
                SetSession("NonCardReceiversDetails", value);
            }
        }
        #endregion


    }
}