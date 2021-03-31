using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class StaffNoticeBoardServices
    {
        FAXEREntities dbContext = null;
        CommonServices CommonService = null;
        private List<ViewStaffNoticeViewModel> staffNoticesVm;
        private IQueryable<StaffNotice> staffNotices;
        private IQueryable<StaffInformation> staffInformation;
        public StaffNoticeBoardServices()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
            staffNoticesVm = new List<ViewStaffNoticeViewModel>();
        }

        public List<ViewStaffNoticeViewModel> GetStaffNotices(string countryCode = "",
            string city = "", int year = 0, int month = 0, string staffName = "", string title = "")
        {
            staffNotices = dbContext.StaffNotice.Where(x => x.IsDeleted == false);
            staffInformation = dbContext.StaffInformation;
            SearchStaffNoticeByParam(new StaffSearchPramVM()
            {
                City = city,
                Country = countryCode,
                Year = year,
                Month = month,
                StaffName = staffName,
                StaffNoticeTitle = title,
            });
            GetStaffNoticesInfo();
            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                staffNoticesVm = staffNoticesVm.Where(x => x.StaffName.ToLower().Contains(staffName.ToLower())).ToList();
            }
            return staffNoticesVm;
        }

        private void GetStaffNoticesInfo()
        {
            staffNoticesVm = (from c in staffNotices.OrderByDescending(x => x.Date).ToList()
                              join country in dbContext.Country on c.Country equals country.CountryCode into countryJoin
                              from country in countryJoin.DefaultIfEmpty()
                              join d in staffInformation on c.StaffId equals d.Id into joinedT
                              from d in joinedT.DefaultIfEmpty()
                              select new ViewStaffNoticeViewModel()
                              {
                                  Id = c.Id,
                                  DateAndTime = c.Date,
                                  CountryFlag = string.IsNullOrEmpty(c.Country) == true ? " " : c.Country.ToLower(),
                                  Headline = c.Headline,
                                  Country = string.IsNullOrEmpty(c.Country) == true ? "All" : country.CountryName,
                                  City = string.IsNullOrEmpty(c.City) == true ? "All" : c.City,
                                  StaffId = c.StaffId,
                                  StaffName = d == null ? "All" : d.FirstName + " " + d.MiddleName + " " + d.LastName

                              }).ToList();
        }
        private void SearchStaffNoticeByParam(StaffSearchPramVM searchPram)
        {
            if (!string.IsNullOrEmpty(searchPram.Country))
            {
                staffNotices = staffNotices.Where(x => x.Country == searchPram.Country);
            }
            if (!string.IsNullOrEmpty(searchPram.City))
            {
                staffNotices = staffNotices.Where(x => x.City.ToLower() == searchPram.City.ToLower());
            }
            if (searchPram.Year != 0)
            {
                staffNotices = staffNotices.Where(x => x.Date.Year == searchPram.Year);
            }
            if (searchPram.Month != 0)
            {
                staffNotices = staffNotices.Where(x => x.Date.Month == searchPram.Month);
            }
            if (!string.IsNullOrEmpty(searchPram.StaffNoticeTitle))
            {
                searchPram.StaffNoticeTitle = searchPram.StaffNoticeTitle.Trim();
                staffNotices = staffNotices.Where(x => x.Headline.ToLower().Contains(searchPram.StaffNoticeTitle.ToLower()));
            }
            //if (!string.IsNullOrEmpty(searchPram.StaffName))
            //{
            //    searchPram.StaffName = searchPram.StaffName.Trim();
            //    staffInformation = staffInformation.Where(x => x.FirstName.ToLower().Contains(searchPram.StaffName.ToLower()) ||
            //                                                   x.MiddleName.ToLower().Contains(searchPram.StaffName.ToLower()) ||
            //                                                   x.LastName.ToLower().Contains(searchPram.StaffName.ToLower()));
            //}
        }

        public AddStaffNoticeViewModel EditNoticeInfo(int id)
        {
            var result = (from c in dbContext.StaffNotice.Where(x => x.Id == id).ToList()
                          join d in dbContext.StaffInformation on c.StaffId equals d.Id into joinedT
                          from d in joinedT.DefaultIfEmpty()
                          select new AddStaffNoticeViewModel()
                          {
                              Id = c.Id,
                              Country = c.Country,
                              CountryName = CommonService.getCountryNameFromCode(c.Country),
                              StaffName = d == null ? "" : d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              City = c.City,
                              StaffId = c.StaffId,
                              Headline = c.Headline,
                              FullNotice = c.FullNotice

                          }).FirstOrDefault();
            return result;
        }

        public bool SaveEditNoticeInfo(AddStaffNoticeViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.StaffNotice.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.Headline = model.Headline;
                    data.FullNotice = model.FullNotice;

                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        public bool SaveStaffNotices(AddStaffNoticeViewModel model)
        {
            if (model != null)
            {
                StaffNotice data = new StaffNotice()
                {
                    StaffId = model.StaffId,
                    Country = model.Country,
                    City = model.City,
                    Date = DateTime.Now,
                    Headline = model.Headline,
                    FullNotice = model.FullNotice,
                    ModifiedBy = Common.StaffSession.LoggedStaff.StaffId
                };
                dbContext.StaffNotice.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteNotice(int id)
        {
            if (id != 0)
            {
                var data = dbContext.StaffNotice.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.IsDeleted = true;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public List<DropDownCityViewModel> GetCities()
        {
            var result = (from c in dbContext.StaffInformation
                          select new DropDownCityViewModel()
                          {
                              City = (c.City.Trim()).ToLower()
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownCityViewModel> GetCitiesFromCountry(string countrycode)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Country == countrycode)
                          select new DropDownCityViewModel()
                          {
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
            IQueryable<StaffInformation> staffInfo = dbContext.StaffInformation;
            if (!string.IsNullOrEmpty(country))
            {
                staffInfo = staffInfo.Where(x => x.Country == country);
            }
            if (!string.IsNullOrEmpty(city))
            {
                staffInfo = staffInfo.Where(x => x.City.Trim().ToLower() == city.ToLower());
            }
            var result = (from c in staffInfo.ToList()
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }
    }
}