using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffBookHolidaysServices
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        public StaffBookHolidaysServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        public bool CancelHolidays(int HolidayId)
        {

            int staffid = Common.StaffSession.LoggedStaff.StaffId;
            var result = dbContext.StaffHolidays.Where(x => x.Id == HolidayId).FirstOrDefault();
            if (result.HolidaysEndDate.Date < DateTime.Now.Date)
            {
                return false;
            }

            else
            {

                result.HolidaysRequestStatus = HollidayRequestStatus.Cancel;
                result.HoidaysLeft = result.HoidaysLeft + result.HolidaysTaken;
                result.HolidaysTaken = 0;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                int count = dbContext.SaveChanges();
                if (count > 0)
                {
                    int HolidaysLeft = 0;
                    var staffHolidaysSingle = dbContext.StaffHolidays.Where(x => x.Id == HolidayId).FirstOrDefault();
                    if (staffHolidaysSingle != null)
                    {
                        HolidaysLeft = staffHolidaysSingle.HoidaysLeft;

                        var staffHolidaysList = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffid && x.Id > HolidayId && x.HolidaysEndDate.Year == result.HolidaysEndDate.Year).ToList();

                        for (int i = 0; i < staffHolidaysList.Count; i++)
                        {
                            int id = staffHolidaysList[i].Id;
                            var staffHolidays = dbContext.StaffHolidays.Where(x => x.Id == id).FirstOrDefault();
                            staffHolidays.HoidaysLeft = HolidaysLeft - staffHolidays.HolidaysTaken;
                            dbContext.Entry(staffHolidays).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            HolidaysLeft = staffHolidays.HoidaysLeft;

                        }
                    }
                }
                return true;
            }
        }

        public DB.StaffInformation GetStaffById(int staffId)
        {
            var result = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            return result;
        }
        public DB.StaffHolidays GetHolidaysById(int HolidayId)
        {
            var result = dbContext.StaffHolidays.Where(x => x.Id == HolidayId).FirstOrDefault();

            return result;
        }
        public StaffHolidaysEntiltlement GetHolidaysEntitlement(int staffId)
        {


            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();

            return data.StaffHolidaysEntitlemant;
        }
        public DB.StaffHolidays SumHolidaysLeft(int staffId, DateTime date)
        {

            var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId && x.HolidaysStartDate.Year == date.Date.Year && x.HolidaysRequestStatus == HollidayRequestStatus.Approved).ToList().LastOrDefault();

            return data;

        }
        public int SumHolidaysTaken(int staffId, DateTime date)
        {
            var result = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId).ToList();
            if (result.Count > 0)
            {
                var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId && x.HolidaysStartDate.Year == date.Date.Year).Sum(x => x.HolidaysTaken);

                return data;
            }
            else
            {
                return 0;
            }

        }
        public DB.StaffInformation GetStaffInformation(int StaffId)
        {

            var result = dbContext.StaffInformation.Where(x => x.Id == StaffId).FirstOrDefault();

            return result;
        }
        public DB.StaffHolidays SaveHolidays(DB.StaffHolidays staffHolidays)
        {

            dbContext.StaffHolidays.Add(staffHolidays);
            dbContext.SaveChanges();
            return staffHolidays;


        }

        public bool AprroveHolidayRequest(int id)
        {
            var result = dbContext.StaffHolidays.Where(x => x.Id == id).FirstOrDefault();

            var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == result.StaffInformationId && x.HolidaysEndDate.Year == result.HolidaysStartDate.Year && x.HolidaysRequestStatus == HollidayRequestStatus.Approved).ToList().LastOrDefault();
            int holidayLeft = 0;
            if (data != null)
            {
                holidayLeft = data.HoidaysLeft;
            }
            else
            {
                holidayLeft = result.HoidaysLeft;
            }
            var startDate = result.HolidaysStartDate;
            var endDate = result.HolidaysEndDate;
            if (endDate.Date.Year == startDate.Date.Year)
            {

                result.HoidaysLeft = holidayLeft - result.TotalNumberOfHolidaysRequeste;
                result.HolidaysRequestStatus = DB.HollidayRequestStatus.Approved;
                result.HolidaysTaken = result.TotalNumberOfHolidaysRequeste;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                var diff = result.HolidaysEndDate - result.HolidaysStartDate;
                var diffCount = diff.Days + 1;
                int regularHolidaysCountOldYear = 0;
                int regularHolidaysCountNewYear = 0;
                int olddiff = 0;
                int newdiff = 0;
                var publicHolidays = GetPublicHolidays(result.HolidaysStartDate, result.HolidaysEndDate);
                for (int i = 0; i < diffCount; i++)
                {
                    var date = result.HolidaysStartDate.AddDays(i).Date;
                    if (date.Year == result.HolidaysStartDate.Date.Year)
                    {
                        olddiff = olddiff + 1;
                        var regularholidays = result.HolidaysStartDate.AddDays(i).DayOfWeek;

                        if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                        {


                            regularHolidaysCountOldYear = regularHolidaysCountOldYear + 1;
                        }
                        for (int j = 0; j < publicHolidays.Count; j++)
                        {
                            var StartToEndDate = result.HolidaysStartDate.AddDays(i).Date;
                            if (!(StartToEndDate.DayOfWeek == DayOfWeek.Saturday || StartToEndDate.DayOfWeek == DayOfWeek.Sunday))
                            {
                                if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                                {
                                    if (publicHolidays[j].FromDate.Year == StartToEndDate.Year && publicHolidays[j].ToDate.Year == StartToEndDate.Year)
                                    {
                                        regularHolidaysCountOldYear = regularHolidaysCountOldYear + 1;
                                    }
                                }
                            }
                        }

                    }

                    else
                    {
                        newdiff = newdiff + 1;
                        var regularholidays = result.HolidaysStartDate.AddDays(i).DayOfWeek;

                        if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                        {


                            regularHolidaysCountNewYear = regularHolidaysCountNewYear + 1;
                        }
                        for (int j = 0; j < publicHolidays.Count; j++)
                        {
                            var StartToEndDate = result.HolidaysStartDate.AddDays(i).Date;

                            if (!(StartToEndDate.DayOfWeek == DayOfWeek.Saturday || StartToEndDate.DayOfWeek == DayOfWeek.Sunday))
                            {
                                if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                                {
                                    if (publicHolidays[j].FromDate.Year == StartToEndDate.Year && publicHolidays[j].ToDate.Year == StartToEndDate.Year)
                                    {
                                        regularHolidaysCountNewYear = regularHolidaysCountNewYear + 1;
                                    }
                                }
                            }
                        }

                    }

                }

                int totalNumberofHolidaysRequestedOldYear = olddiff - regularHolidaysCountOldYear;

                int totalNumberofHolidaysRequestedNewYear = newdiff - regularHolidaysCountNewYear;

                var HolidaysEndDateOldYear = result.HolidaysStartDate.AddDays(olddiff - 1);
                var HolidayStartDatenewYear = result.HolidaysStartDate.AddDays(olddiff);
                var HolidayEndDatenewYear = HolidayStartDatenewYear.AddDays(newdiff - 1);

                result.HoidaysLeft = result.HoidaysLeft - totalNumberofHolidaysRequestedOldYear;
                result.HolidaysRequestStatus = DB.HollidayRequestStatus.Approved;
                result.HolidaysTaken = totalNumberofHolidaysRequestedOldYear;
                result.HolidaysEndDate = HolidaysEndDateOldYear;
                result.TotalNumberOfHolidaysRequeste = totalNumberofHolidaysRequestedOldYear;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;

                var staffInformation = dbContext.StaffInformation.Where(x => x.Id == result.StaffInformationId).FirstOrDefault();
                DB.StaffHolidays holidays = new StaffHolidays()
                {
                    StaffInformationId = result.StaffInformationId,
                    HolidaysRequestStatus = HollidayRequestStatus.Approved,
                    HolidaysStartDate = HolidayStartDatenewYear,
                    HolidaysEndDate = HolidayEndDatenewYear,
                    TotalNumberOfHolidaysRequeste = totalNumberofHolidaysRequestedNewYear,
                    HolidaysTaken = totalNumberofHolidaysRequestedNewYear,
                    HolidaysReason = result.HolidaysReason,
                    HoidaysLeft = staffInformation.StaffNoOFHolidays - totalNumberofHolidaysRequestedNewYear
                };

                dbContext.StaffHolidays.Add(holidays);
                dbContext.SaveChanges();
            }

            return true;
        }
        public bool RejectholidayRequest(int id)
        {
            var result = dbContext.StaffHolidays.Where(x => x.Id == id).FirstOrDefault();
            result.HolidaysRequestStatus = DB.HollidayRequestStatus.Rejected;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public IEnumerable<ViewModels.StaffBookHolidaysViewModel> getHolidays()
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            var data = from c in dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId).OrderBy(x => x.Id)
                       select c;
            return data.ToList() // now we have in-memory query
                 .Select(c => new ViewModels.StaffBookHolidaysViewModel()
                 {
                     HolidaysStartDate = Common.ConversionExtension.ToFormatedString(c.HolidaysStartDate.Date),
                     HolidaysEndDate = Common.ConversionExtension.ToFormatedString(c.HolidaysEndDate.Date),
                     TotalNoOfHolidays = c.StaffInformation.StaffNoOFHolidays,
                     NoOfHolidaysRequested = c.TotalNumberOfHolidaysRequeste,
                     NoOfHolidaysTaken = c.HolidaysTaken,
                     NoOfHolidaysLeft = c.HoidaysLeft,
                     HolidaysRequestStatus = c.HolidaysRequestStatus,
                     BookHolidayId = c.Id
                 });
        }
        public DB.StaffHolidays GetLastHolidayRequest(int StaffId)
        {
            var result = dbContext.StaffHolidays.Where(x => x.StaffInformationId == StaffId && x.HolidaysRequestStatus != HollidayRequestStatus.Rejected).ToList().LastOrDefault();
            if ((result != null) && result.HolidaysEndDate.Year == DateTime.Now.Year)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        public bool ValidateDate(int staffId, DateTime StartDate, DateTime Enddate)
        {

            var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId && x.HolidaysRequestStatus == HollidayRequestStatus.Approved).ToList();

            int val = 0;

            for (int i = 0; i < data.Count; i++)
            {

                if ((StartDate >= data[i].HolidaysStartDate && StartDate <= data[i].HolidaysEndDate) || (Enddate >= data[i].HolidaysStartDate && Enddate <= data[i].HolidaysEndDate))
                {

                    val = val + 1;


                }
            }
            if (val > 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public List<DB.PublicHolidays> GetPublicHolidays(DateTime StartDate, DateTime EndDate)
        {
            var StaffId = Common.StaffSession.LoggedStaff.StaffId;
            var staffInformation = dbContext.StaffInformation.Where(x => x.Id == StaffId).FirstOrDefault();
            //var result = dbContext.PublicHolidays.Where(x => x.IsDeleted == false && (x.FromDate >= StartDate || x.ToDate <= EndDate)).ToList();

            var data = dbContext.PublicHolidays.Where(x => (x.IsDeleted == false) &&
                        (x.FromDate >= StartDate || x.ToDate <= EndDate) &&
                         (x.Country == staffInformation.Country || x.Country == null) &&
                         (x.City == staffInformation.City || x.City == null)
                          ).ToList();
            return data;
        }

        //public int GetHolidaysTakenNumber(string startDate , string endDate) {

        //    int HolidaysTaken = 0;
        //    var StartDate = Common.ConversionExtension.ToDateTime(startDate);
        //    var EndDate = Common.ConversionExtension.ToDateTime(endDate);
        //    var diff = EndDate - StartDate;
        //    var diffCount = diff.Days + 1;
        //    for (int i = 0; i < diffCount; i++)
        //    {
        //        if (StartDate.Date.AddDays(i) <= DateTime.Now.Date)
        //        {
        //            var regularholidays = StartDate.AddDays(i).DayOfWeek;
        //            if (!(regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday))
        //            {
        //                HolidaysTaken = HolidaysTaken + 1; 
        //            }
        //        }

        //    }
        //    return HolidaysTaken;
        //}
    }
}