using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewPublicHolidaysServices
    {
        FAXEREntities dbContext = null;
        CommonServices CommonService = null;
        private IQueryable<PublicHolidays> publicHolidays;
        private List<ViewPublicHolidaysViewModel> publicHolidaysViewModels;

        public ViewPublicHolidaysServices()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
            publicHolidaysViewModels = new List<ViewPublicHolidaysViewModel>();
        }
        public List<ViewPublicHolidaysViewModel> GetPublicHolidaysList(string CountryCode = "", string City = "", int Year = 0, int Month = 0, string name = "")
        {
            publicHolidays = dbContext.PublicHolidays.Where(x => x.IsDeleted == false);

            SearchPublicHolidayByParam(new StaffSearchPramVM()
            {
                Country = CountryCode,
                City = City,
                Year = Year,
                Month = Month,
                Title = name
            });
            GetPublicHolidays();
            return publicHolidaysViewModels;
        }

        private void SearchPublicHolidayByParam(StaffSearchPramVM searchPram)
        {
            if (!string.IsNullOrEmpty(searchPram.Country))
            {
                publicHolidays = publicHolidays.Where(x => x.Country == searchPram.Country);
            }
            if (!string.IsNullOrEmpty(searchPram.City))
            {
                searchPram.City = searchPram.City.Trim();
                publicHolidays = publicHolidays.Where(x => x.City.ToLower() == searchPram.City.ToLower());
            }
            if (!string.IsNullOrEmpty(searchPram.Title))
            {
                searchPram.Title = searchPram.Title.Trim();
                publicHolidays = publicHolidays.Where(x => x.Name.ToLower().Contains(searchPram.Title.ToLower()));
            }
            if (searchPram.Year != 0)
            {
                publicHolidays = publicHolidays.Where(x => x.CreatedDate.Year == searchPram.Year);
            }
            if (searchPram.Month != 0)
            {
                publicHolidays = publicHolidays.Where(x => x.CreatedDate.Month == searchPram.Month);
            }
        }

        private void GetPublicHolidays()
        {
            publicHolidaysViewModels = (from c in publicHolidays.OrderByDescending(x => x.FromDate).ToList()
                                        select new ViewPublicHolidaysViewModel()
                                        {
                                            Id = c.Id,
                                            FromDate = c.FromDate.ToFormatedString(),
                                            ToDate = c.ToDate.ToFormatedString(),
                                            City = c.City == null ? "All" : c.City,
                                            Country = c.Country == null ? "All" : CommonService.getCountryNameFromCode(c.Country),
                                            HolidayName = c.Name,
                                            CountryFlag = c.Country == null ? "" : c.Country.ToLower(),
                                        }).ToList();
        }
        public AddPublicHolidayViewModel GetEditPublicHolidayInfo(int Id)
        {
            var data = (from c in dbContext.PublicHolidays.Where(x => x.Id == Id).ToList()
                        select new AddPublicHolidayViewModel()
                        {
                            Id = c.Id,
                            FromDate = c.FromDate,
                            ToDate = c.ToDate,
                            Country = c.Country == null ? "All" : c.Country,
                            City = c.City == null ? "All" : c.City,
                            HolidayName = c.Name
                        }).FirstOrDefault();
            return data;

        }

        public bool SaveEditPublicHoliday(AddPublicHolidayViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.PublicHolidays.Find(model.Id);
                if (data != null)
                {
                    data.FromDate = (DateTime)model.FromDate;
                    data.ToDate = (DateTime)model.ToDate;
                    data.Name = model.HolidayName;
                    data.ModifiedByStaff = Common.StaffSession.LoggedStaff.StaffId;

                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeletePublicHoliday(int Id)
        {
            if (Id != 0)
            {
                var data = dbContext.PublicHolidays.Find(Id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        public bool SavePublicHoliday(AddPublicHolidayViewModel model)
        {
            if (model != null)
            {
                var data = new PublicHolidays()
                {
                    Name = model.HolidayName,
                    FromDate = (DateTime)model.FromDate,
                    ToDate = (DateTime)model.ToDate,
                    Country = model.Country,
                    City = model.City,
                    CreatedByStaff = Common.StaffSession.LoggedStaff.StaffId,
                    CreatedDate = DateTime.Now.Date
                };
                dbContext.PublicHolidays.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ValidatePubilcHolidayDate(int Id, DateTime StartDate, DateTime Enddate)
        {

            var data = dbContext.PublicHolidays.Where(x => (x.Id != Id) && (x.IsDeleted == false)).ToList();

            int val = 0;

            for (int i = 0; i < data.Count; i++)
            {

                if ((StartDate >= data[i].FromDate && StartDate <= data[i].ToDate) || (Enddate >= data[i].FromDate && Enddate <= data[i].ToDate))
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