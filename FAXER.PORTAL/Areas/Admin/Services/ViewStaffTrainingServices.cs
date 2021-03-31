using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Pricing.V1.Messaging;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewStaffTrainingServices
    {
        FAXEREntities dbContext = null;
        CommonServices ComonnService = null;
        private IQueryable<StaffTraining> staffTraining;
        private List<ViewStaffTrainingViewModel> staffTrainingVms;
        public ViewStaffTrainingServices()
        {
            dbContext = new FAXEREntities();
            ComonnService = new CommonServices();
            staffTrainingVms = new List<ViewStaffTrainingViewModel>();
        }
        public List<ViewStaffTrainingViewModel> getTrainingList(string CountryCode = "", string City = "", int Year = 0, int Month = 0,
            int staffId = 0, string staffName = "", string title = "")
        {
            staffTraining = dbContext.StaffTraining.Where(x => x.isDeleted == false);
            SearchStaffTrainingByParam(new StaffSearchPramVM()
            {
                City = City,
                Country = CountryCode,
                Title = title,
                StaffName = staffName,
                Year = Year,
                Month = Month,
                StaffId = staffId
            });
            GetStaffTrainings();
            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                staffTrainingVms = staffTrainingVms.Where(x => x.StaffFullName.ToLower().Contains(staffName.ToLower())).ToList();
            }
            return staffTrainingVms;
        }

        private void SearchStaffTrainingByParam(StaffSearchPramVM searchPram)
        {
            if (!string.IsNullOrEmpty(searchPram.Country))
            {
                staffTraining = staffTraining.Where(x => x.Country == searchPram.Country);
            }
            if (!string.IsNullOrEmpty(searchPram.City))
            {
                staffTraining = staffTraining.Where(x => (x.isDeleted == false) && (x.City.ToLower() == searchPram.City.ToLower()));
            }
            if (searchPram.Year != 0)
            {
                staffTraining = staffTraining.Where(x => x.TrainingAddedDate.Year == searchPram.Year);
            }
            if (searchPram.Month != 0)
            {
                staffTraining = staffTraining.Where(x => x.TrainingAddedDate.Month == searchPram.Month);
            }
            if (searchPram.StaffId != 0)
            {
                staffTraining = staffTraining.Where(x => x.StaffInformationId == searchPram.StaffId);
            }
            if (!string.IsNullOrEmpty(searchPram.Title))
            {
                searchPram.Title = searchPram.Title.Trim();
                staffTraining = staffTraining.Where(x => x.Title.ToLower().Contains(searchPram.Title.ToLower()));

            }
        }

        private void GetStaffTrainings()
        {
            staffTrainingVms = (from c in staffTraining.OrderByDescending(x => x.TrainingAddedDate).ToList()
                                join country in dbContext.Country on c.Country equals country.CountryCode
                                join d in dbContext.StaffInformation on c.StaffInformationId equals d.Id into joinedT
                                from d in joinedT.DefaultIfEmpty()
                                select new ViewStaffTrainingViewModel()
                                {
                                    Id = c.Id,
                                    staffId = c.StaffInformationId,
                                    staffFirstName = d == null ? "All" : d.FirstName,
                                    staffMiddleName = d == null ? "" : d.MiddleName,
                                    staffLastName = d == null ? "" : d.LastName,
                                    Country = c.Country == null ? "All" : ComonnService.getCountryNameFromCode(c.Country),
                                    City = c.City == null ? "All" : c.City,
                                    Title = c.Title,
                                    StaffFullName = d == null ? "All" : d.FirstName + " " + d.MiddleName + " " + d.LastName,
                                    Link = c.Link,
                                    OutstandingTraining = c.OutstandingTraining,
                                    EndDate = c.EndDate,
                                    StartDate = c.StartDate,
                                    CountryFlag = c.Country == null ? "" : c.Country.ToLower(),
                                    FullNotice = c.FullNotice
                                }).ToList();
        }

        public List<DropDownCityViewModel> GetCities()
        {
            var result = (from c in dbContext.StaffInformation
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.Country,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownCityViewModel> GetCitiesFromCountry(string CountryCode)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Country == CountryCode)
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.Country,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetStaffDropdownList(string countryCode, string city)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => (x.Country == countryCode) && ((x.City.Trim()).ToLower() == city))
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetEmptyStaffDropdownList()
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Id == 0)
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public bool deleteTraining(int id)
        {
            var data = dbContext.StaffTraining.Find(id);
            if (data != null)
            {
                data.isDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public AddStaffTrainingViewModel EditStaffTrainingInfo(int id)
        {
            var result = (from c in dbContext.StaffTraining.Where(x => x.Id == id).ToList()
                          join d in dbContext.StaffInformation on c.StaffInformationId equals d.Id into joinedT
                          from d in joinedT.DefaultIfEmpty()
                          select new AddStaffTrainingViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = c.Country,
                              staffName = d == null ? "" : d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              Title = c.Title,
                              Link = c.Link,
                              Deadline = c.Deadline,
                              completeTraining = c.CompleteTraining,
                              outstandingTraining = c.OutstandingTraining,
                              StartDate = c.StartDate,
                              EndDate = c.EndDate,
                              FullNotice = c.FullNotice,
                              staffId = c.StaffInformationId
                          }).FirstOrDefault();
            return result;
        }

        public bool SaveTrainingEditInfo(AddStaffTrainingViewModel model)
        {
            var data = dbContext.StaffTraining.Where(x => x.Id == model.Id).FirstOrDefault();
            if (data != null)
            {
                data.Title = model.Title;
                data.Country = model.Country;
                data.City = model.City;
                data.Link = model.Link;
                data.Deadline = model.Deadline;
                data.CompleteTraining = model.completeTraining;
                data.OutstandingTraining = model.outstandingTraining;
                data.StartDate = model.StartDate;
                data.EndDate = model.EndDate;
                data.FullNotice = model.FullNotice;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool saveStaffTrainingInfo(AddStaffTrainingViewModel model)
        {
            if (model != null)
            {
                StaffTraining data = new StaffTraining()
                {
                    StaffInformationId = model.staffId,
                    Country = model.Country,
                    City = model.City,
                    Title = model.Title,
                    Link = model.Link,
                    Deadline = model.Deadline,
                    CompleteTraining = model.completeTraining,
                    OutstandingTraining = model.outstandingTraining,
                    TrainingAddedByStaff = Common.StaffSession.LoggedStaff.StaffId,
                    TrainingAddedDate = DateTime.Now.Date,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    FullNotice = model.FullNotice
                };
                dbContext.StaffTraining.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }

    public class DropDownCityViewModel
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
    }
    public class DropDownStaffViewModel
    {
        public int staffId { get; set; }
        public string staffName { get; set; }
    }
    public class DropDownAgentViewModel
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string AgentCountry { get; set; }
        public string AgentCity { get; set; }
    }
}