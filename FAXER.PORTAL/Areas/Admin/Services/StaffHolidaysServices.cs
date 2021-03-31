using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class StaffHolidaysServices
    {
        FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();
        private IQueryable<StaffHolidays> staffHolidays;
        private List<StaffHolidaysViewModel> staffHolidayViewModels;
        public StaffHolidaysServices()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
            staffHolidayViewModels = new List<StaffHolidaysViewModel>();
        }
        public List<StaffHolidaysViewModel> GetHolidaysList(string CountryCode = "", string City = "", int Year = 0, int Month = 0, int staffId = 0, string staffName = "")
        {
            staffHolidays = dbContext.StaffHolidays.Where(x => x.IsDeleted == false);

            SearchHolidayListByParam(new StaffSearchPramVM()
            {
                Country = CountryCode,
                City = City,
                Year = Year,
                Month = Month,
                StaffId = staffId,
                StaffName = staffName
            });
            GetStaffHolidays();
            return staffHolidayViewModels;
        }

        private void GetStaffHolidays()
        {
            staffHolidayViewModels = (from c in staffHolidays.OrderByDescending(x => x.HolidaysStartDate).ToList()
                                      select new StaffHolidaysViewModel()
                                      {
                                          Id = c.Id,
                                          StaffName = c.StaffInformation.FirstName + " " + c.StaffInformation.MiddleName + " " + c.StaffInformation.LastName,
                                          StaffId = c.StaffInformationId,
                                          Country = CommonService.getCountryNameFromCode(c.StaffInformation.Country),
                                          City = c.StaffInformation.City,
                                          NoOfDays = c.TotalNumberOfHolidaysRequeste,
                                          HolidayStatus = c.HolidaysRequestStatus,
                                          StartDate = c.HolidaysStartDate.ToFormatedString(),
                                          FinishDate = c.HolidaysEndDate.ToFormatedString(),
                                          NoTaken = c.HolidaysTaken, //GetHolidaysTakenNumber(c.HolidaysStartDate.ToFormatedString(), c.HolidaysEndDate.ToFormatedString()),
                                          NoLeft = c.TotalNumberOfHolidaysRequeste - GetHolidaysTakenNumber(c.HolidaysStartDate.ToFormatedString(), c.HolidaysEndDate.ToFormatedString()),
                                          CountryFlag = c.StaffInformation.Country.ToLower(),
                                          Entitled = c.HolidaysEntitled
                                      }).ToList();
        }


        private void SearchHolidayListByParam(StaffSearchPramVM searchPram)
        {
            if (!string.IsNullOrEmpty(searchPram.Country))
            {
                staffHolidays = staffHolidays.Where(x => x.StaffInformation.Country == searchPram.Country);
            }
            if (!string.IsNullOrEmpty(searchPram.City))
            {
                staffHolidays = staffHolidays.Where(x => x.StaffInformation.City.ToLower() == searchPram.City.ToLower());
            }
            if (searchPram.Year > 0)
            {
                staffHolidays = staffHolidays.Where(x => x.HolidaysStartDate.Year == searchPram.Year);
            }
            if (searchPram.Month > 0)
            {
                staffHolidays = staffHolidays.Where(x => x.HolidaysStartDate.Month == searchPram.Month);
            }
            if (searchPram.StaffId > 0)
            {
                staffHolidays = staffHolidays.Where(x => x.StaffInformationId == searchPram.StaffId);
            }
            if (!string.IsNullOrEmpty(searchPram.StaffName))
            {
                staffHolidays = staffHolidays.Where(x => x.StaffInformation.FirstName == searchPram.StaffName ||
                                                         x.StaffInformation.MiddleName == searchPram.StaffName ||
                                                         x.StaffInformation.LastName == searchPram.StaffName);
            }
        }

        public AddStaffHolidaysViewModel GetEditHolidayList(int id)
        {
            var result = (from c in dbContext.StaffHolidays.Where(x => (x.Id == id)).ToList()
                          select new AddStaffHolidaysViewModel()
                          {
                              Id = c.Id,
                              Country = c.StaffInformation.Country,
                              City = c.StaffInformation.City,
                              StaffId = c.StaffInformationId,
                              StaffName = c.StaffInformation.FirstName + " " + c.StaffInformation.MiddleName + " " + c.StaffInformation.LastName,
                              NoOfDays = c.TotalNumberOfHolidaysRequeste,
                              StartDate = c.HolidaysStartDate,
                              FinishDate = c.HolidaysEndDate,
                              NoTaken = c.HolidaysTaken,
                              NoLeft = c.TotalNumberOfHolidaysRequeste - GetHolidaysTakenNumber(c.HolidaysStartDate.ToFormatedString(), c.HolidaysEndDate.ToFormatedString()),
                              NoOfDaysEntitled = c.HolidaysEntitled,

                          }).FirstOrDefault();
            return result;
        }

        public bool DeleteStaffHolidays(int id)
        {
            var data = dbContext.StaffHolidays.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool SaveHolidays(StaffHolidays data)
        {
            if (data != null)
            {
                dbContext.StaffHolidays.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EditHolidays(AddStaffHolidaysViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.StaffHolidays.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.HolidaysStartDate = model.StartDate;
                    data.HolidaysEndDate = model.FinishDate;
                    data.TotalNumberOfHolidaysRequeste = model.NoOfDays;
                    data.HolidaysTaken = model.NoTaken;
                    data.HoidaysLeft = model.NoLeft;
                    data.HolidaysRequestStatus = HollidayRequestStatus.Approved;
                    data.ModifiedBy = StaffSession.LoggedStaff.StaffId;

                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public string getEmail(int staffId)
        {
            if (staffId != 0)
            {
                var data = dbContext.StaffInformation.Find(staffId);
                return data.EmailAddress;
            }
            return null;
        }

        public List<PublicHolidays> GetPublicHolidays(int staffId)
        {
            string country = dbContext.StaffInformation.Find(staffId).Country;
            string city = dbContext.StaffInformation.Find(staffId).City;

            var result = dbContext.PublicHolidays.Where(x => (x.IsDeleted == false) && (x.Country == country || x.Country == null) && (x.City == city || x.City == null)).ToList();
            return result;

        }

        public int SumHolidaysLeft(int staffId)
        {
            var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId && x.HolidaysRequestStatus == HollidayRequestStatus.Approved).ToList().LastOrDefault();
            if (data == null)
            {
                var days = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
                if (days != null)
                {
                    return days.StaffNoOFHolidays;
                }
                return 0;
            }

            return data.HoidaysLeft;
        }

        public bool HolidayAlreadyTaken(int staffId, DateTime StartDate)
        {
            var data = dbContext.StaffHolidays.Where(x => x.StaffInformationId == staffId && x.HolidaysRequestStatus == HollidayRequestStatus.Approved).ToList().LastOrDefault();
            if (StartDate <= data.HolidaysEndDate)
            {
                return false;
            }
            return true;
        }

        public DB.HollidayRequestStatus isApproved(int Id)
        {
            var data = dbContext.StaffHolidays.Where(x => x.Id == Id).FirstOrDefault();
            return data.HolidaysRequestStatus;
        }

        public StaffHolidaysEntiltlement GetStaffHolidaysEntitlement(int staffId)
        {
            var result = dbContext.StaffInformation.Find(staffId).StaffHolidaysEntitlemant;
            return result;

        }


        public int GetHolidaysTakenNumber(string startDate, string endDate)
        {

            int HolidaysTaken = 0;
            var StartDate = Common.ConversionExtension.ToDateTime(startDate);
            var EndDate = Common.ConversionExtension.ToDateTime(endDate);
            var diff = EndDate - StartDate;
            var diffCount = diff.Days + 1;
            for (int i = 0; i < diffCount; i++)
            {
                if (StartDate.Date.AddDays(i) <= DateTime.Now.Date)
                {
                    var regularholidays = StartDate.AddDays(i).DayOfWeek;
                    if (!(regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday))
                    {
                        HolidaysTaken = HolidaysTaken + 1;
                    }
                }

            }
            return HolidaysTaken;
        }

        public List<DropDownCityViewModel> GetCities()
        {
            var result = (from c in dbContext.StaffInformation
                          select new DropDownCityViewModel()
                          {
                              //CountryCode = c.Country,
                              City = (c.City.Trim()).ToLower()
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownCityViewModel> GetCitiesFromCountry(string countrycode)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Country == countrycode)
                          select new DropDownCityViewModel()
                          {
                              //CountryCode = c.Country,
                              City = (c.City.Trim()).ToLower()
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetEmptyStaffList()
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Id == 0)
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetFilteredStaffList(string country, string city)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => (x.Country == country) && ((x.City.Trim()).ToLower() == city))
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
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
    }



}