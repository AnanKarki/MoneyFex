using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public static class OtherUtlities
    {

        public static bool IsEmailExist(string email)
        {
            FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
            if (dbContext.FaxerInformation.Where(x => x.Email == email).Count() > 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsEmailExistInAgent(string email)
        {
            FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
            if (dbContext.AgentInformation.Where(x => x.Email == email).Count() > 0)
            {
                return false;
            }
            return true;
        }

        public static bool IsMobileNoExist(string mobileNo)
        {
            FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
            if (dbContext.FaxerInformation.Where(x => x.PhoneNumber == mobileNo).Count() > 0)
            {
                return false;
            }
            return true;
        }
        public static bool IsMobileNoExistInAgent(string mobileNo)
        {
            FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
            if (dbContext.AgentInformation.Where(x => x.PhoneNumber == mobileNo).Count() > 0)
            {
                return false;
            }
            return true;
        } 
    }
}