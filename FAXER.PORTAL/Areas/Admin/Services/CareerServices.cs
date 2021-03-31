using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CareerServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<CareerIndexViewModel> getList(string CountryCode = "", string City = "")
        {
            var data = dbContext.Career.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.Career.Where(x => x.IsDeleted == false && x.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.Career.Where(x => x.IsDeleted == false && x.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.Career.Where(x => x.IsDeleted == false && x.City.ToLower() == City.ToLower() && x.Country == CountryCode).ToList();
            }
            else
            {
                data = dbContext.Career.Where(x => x.IsDeleted == false).ToList();
            }
            var result = (from c in data.OrderByDescending(x => x.PublishedDateTime)
                          select new CareerIndexViewModel()
                          {
                              Id = c.Id,
                              JobTitle = c.JobTitle,
                              Description = c.Description.Length > 85 ? c.Description.Substring(0, 85) + "..." : c.Description,
                              Country = c.Country == "All" ? "All" : CommonService.getCountryNameFromCode(c.Country),
                              City = c.City,
                              ContractType = c.ContractType,
                              SalaryRange = c.SalaryRange,
                              ClosingDate = c.ClosingDate.ToFormatedString(),
                              PublishDate = c.PublishedDateTime.ToString("MMM-dd-yyyy"),
                              CountrySymbol = CommonService.getCurrencySymbol(c.Country)
                          }).ToList();


            return result;
        }

        public CareerViewModel getJob(int id)
        {
            var result = (from c in dbContext.Career.Where(x => x.Id == id).ToList()
                          select new CareerViewModel()
                          {
                              Id = c.Id,
                              JobTitle = c.JobTitle,
                              Description = c.Description,
                              Country = c.Country == "All" ? "All" : c.Country,
                              City = c.City,
                              ContractType = c.ContractType,
                              SalaryRange = c.SalaryRange,
                              ClosingDate = (DateTime)c.ClosingDate,
                              Image = c.Image

                          }).FirstOrDefault();
            return result;
        }

        public bool saveEditedJob(CareerViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.Career.Find(model.Id);
                if (data != null)
                {
                    data.JobTitle = model.JobTitle;
                    data.Description = model.Description;
                    data.ContractType = model.ContractType;
                    data.SalaryRange = model.SalaryRange;
                    data.ClosingDate = (DateTime)model.ClosingDate;
                    data.Image = model.Image;
                    data.LastModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool saveCareer(CareerViewModel model)
        {
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Country))
                {
                    model.Country = "All";
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    model.City = "All";
                }
                Career data = new Career()
                {
                    JobTitle = model.JobTitle,
                    Description = model.Description,
                    Country = model.Country,
                    City = model.City,
                    ContractType = model.ContractType,
                    SalaryRange = model.SalaryRange,
                    ClosingDate = (DateTime)model.ClosingDate,
                    Image = model.Image,
                    PublishedDateTime = DateTime.Now,
                    PublishedBy = Common.StaffSession.LoggedStaff.StaffId
                };
                dbContext.Career.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool deleteJob(int id)
        {
            if (id != 0)
            {
                var data = dbContext.Career.Find(id);
                data.IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<DropDownCityViewModel> GetCitiesFromCountry(string CountryCode = "")
        {
            var cities = (from c in dbContext.City.Where(x => x.CountryCode == CountryCode)
                          select new DropDownCityViewModel()
                          {
                              City = (c.Name.Trim()).ToLower()
                          }).Distinct().ToList();

            return cities;
        }





    }
}