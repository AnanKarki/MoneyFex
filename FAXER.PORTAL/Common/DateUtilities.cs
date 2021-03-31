using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public static class DateUtilities
    {
        public static bool ValidateAge(this DateTime dateOfBirth)
        {
            DateTime bday = dateOfBirth;
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (age < 18)
            {
                return false;
            }
            return true;
        }
        public static bool ValidateAge(this DateTime? dateOfBirth)
        {
            DateTime bday = new DateTime() ;
            DateTime.TryParse(dateOfBirth.ToString(), out bday);
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (age < 18)
            {
                return false;
            }
            return true;
        }

        public static bool DateGreaterThanToday(object value)
        {
            if (value != null)
            {
                DateTime _date = Convert.ToDateTime(value);
                if (_date < DateTime.Now)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="day"></param>
        /// <param name="Month"></param>
        /// <param name="Year"></param>
        /// <returns></returns>

        public static DateTime? GetDateByYMD(int day, int Month, int Year)
        {
            try
            {

                var date = new DateTime().AddDays(day).AddMonths(Month).AddYears(Year);

                return date;
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}