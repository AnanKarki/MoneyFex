using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewBusinessAlertsServices
    {
        FAXEREntities dbContext = null;
        CommonServices CommonService = null;

        public ViewBusinessAlertsServices()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
        }

        public List<MasterViewBusinessMerchantViewModel> getAlertsList(string CountryCode = "", string City = "", int Business = 0, string Date = "")
        {
            var data = dbContext.BusinessAlerts.Where(x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(CountryCode))
            {
                data = data.Where(x => x.Country == CountryCode);
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City.ToLower() == City.ToLower());
            }
            if (Business != 0)
            {
                data = data.Where(x => x.SenderId == Business);
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                data = data.Where(x => x.PublishedDateAndTime >= FromDate && x.PublishedDateAndTime <= ToDate);
            }
            var result = (from c in data.OrderByDescending(x => x.PublishedDateAndTime).ToList()
                          select new MasterViewBusinessMerchantViewModel()
                          {
                              Id = c.Id,
                              DateAndTime = c.PublishedDateAndTime,
                              Date = c.PublishedDateAndTime.ToString("dd/MM/yyyy") + " " + c.PublishedDateAndTime.ToString("HH:mm"),
                              Heading = c.Heading,
                              Country = c.Country == null ? "All" : CommonService.getCountryNameFromCode(c.Country),
                              City = c.City == null ? "All" : c.City,
                              BusinessMerchant = c.SenderId == 0 ? "All" : CommonService.GetSenderName(c.SenderId ?? default(int)),
                              StartDate = c.StartDate.ToString("MMM-dd-yyyy hh:mm"),
                              EndDate = c.EndDate.ToString("MMM-dd-yyyy hh:mm"),
                              CountryFlag = c.Country.ToLower()
                          }).ToList();
            return result;
        }

        public AddNewBusinessAlertViewModel getAlertEdit(int id)
        {
            var result = (from c in dbContext.BusinessAlerts.Where(x => x.Id == id).ToList()
                          select new AddNewBusinessAlertViewModel()
                          {
                              Id = c.Id,
                              Heading = c.Heading,
                              FullMessage = c.FullMessage,
                              Photo = c.PhotoUrl,
                              PublishedDate = c.PublishedDateAndTime.Date,
                              EndDate = c.EndDate,
                              Country = c.Country == null ? "All" : c.Country,
                              BusinessName = ((c.SenderId ?? 0) == 0) ? "All" : CommonService.GetSenderName(c.SenderId ?? default(int)),
                              City = c.City == null ? "All" : c.City,
                              StartDate = c.StartDate,
                              Business = (int)c.SenderId,
                              BusinessAccountNo = CommonService.GetSenderAccountNoBySenderId((int)c.SenderId)

                          }).FirstOrDefault();
            return result;
        }

        public bool saveNewAlert(AddNewBusinessAlertViewModel model)
        {
            if (model != null)
            {
                BusinessAlerts data = new BusinessAlerts()
                {
                    Heading = model.Heading,
                    FullMessage = model.FullMessage,
                    Country = model.Country,
                    City = model.City,
                    SenderId = model.Business,
                    PhotoUrl = model.Photo,
                    PublishedDateAndTime = DateTime.Now,
                    EndDate = (DateTime)model.EndDate,
                    CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                    IsDeleted = false,
                    StartDate = (DateTime)model.StartDate
                };
                dbContext.BusinessAlerts.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool saveEditedAlert(AddNewBusinessAlertViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.BusinessAlerts.Find(model.Id);
                if (data != null)
                {
                    data.Heading = model.Heading;
                    data.FullMessage = model.FullMessage;
                    data.PhotoUrl = model.Photo;
                    data.EndDate = (DateTime)model.EndDate;
                    data.ModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.StartDate = (DateTime)model.StartDate;

                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool DeleteAlert(int id)
        {
            if (id != 0)
            {
                var data = dbContext.BusinessAlerts.Find(id);
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











        public List<DropDownCityViewModel> GetCities()
        {
            var result = (from c in dbContext.FaxerInformation
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.Country,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownCityViewModel> GetCitiesFromCountry(string CountryCode)
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.Country == CountryCode)
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.Country,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownBusinessViewModel> GetEmptyBusinessList()
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == true)
                          select new DropDownBusinessViewModel()
                          {
                              KiiPayBusinessInformationId = c.Id,
                              BusinessName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }


        public List<DropDownBusinessViewModel> GetBusinessFromCountryCity(string CountryCode = "", string City = "")
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => ((x.Country.Trim()) == CountryCode) && ((x.City.Trim()).ToLower() == City))
                          select new DropDownBusinessViewModel()
                          {
                              KiiPayBusinessInformationId = c.Id,
                              BusinessName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }
    }

    public class DropDownBusinessViewModel
    {
        public int KiiPayBusinessInformationId { get; set; }
        public string BusinessName { get; set; }
    }
}