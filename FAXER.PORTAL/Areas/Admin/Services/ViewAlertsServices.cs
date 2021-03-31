using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewAlertsServices
    {
        FAXEREntities dbContext = null;
        CommonServices CommonService = null;

        public ViewAlertsServices()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
        }

        public List<MasterViewAlertsViewModel> getAlertsList(string CountryCode = "", string City = "", int AgentId = 0, string Date = "", Module module = 0)
        {
            var data = dbContext.AgentAlerts.Where(x => x.IsDeleted == false && x.Module == module);
            if (!string.IsNullOrEmpty(CountryCode))
            {
                data = data.Where(x => x.Country == CountryCode);
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City.ToLower() == City.ToLower());
            }

            //Treated sender id and staff Id as a Agent Id
            if (AgentId != 0)
            {
                data = data.Where(x => x.AgentId == AgentId);
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                data = data.Where(x => x.PublishedDateAndTime >= FromDate && x.PublishedDateAndTime <= ToDate);
            }
            var result = (from c in data.OrderByDescending(x => x.PublishedDateAndTime).ToList()
                          join country in dbContext.Country on c.Country equals country.CountryCode into joined
                          from country in joined.DefaultIfEmpty()
                          select new MasterViewAlertsViewModel()
                          {
                              Id = c.Id,
                              DateAndTime = c.PublishedDateAndTime,
                              Heading = c.Heading,
                              Country = c.Country == null ? "All" : country.CountryName,
                              City = c.City == null ? "All" : c.City,
                              Agent = getName(module, (int)c.AgentId),
                              StartDate = c.StartDate.ToString("MMM-dd-yyyy"),
                              EndDate = c.EndDate.ToString("MMM-dd-yyyy"),
                              FullMessage = c.FullMessage,
                              CountryFlag = c.Country.ToLower(),
                          }).ToList();
            return result;
        }
        public string getName(Module module, int Id)
        {
            var result = "";
            if (module == Module.Agent)
            {
                result = CommonService.getAgentName(Id);

            }
            else if (module == Module.Staff)
            {
                result = CommonService.getStaffName(Id);
            }
            else
            {
                result = CommonService.GetSenderName(Id);
            }
            if (result != "")
            {

                return result;
            }
            else
            {
                return "All";
            }
        }

        public string getAccountName(Module module, int Id)
        {
            var result = "";
            if (module == Module.Agent)
            {
                result = CommonService.GetAgentAccNo(Id);

            }
            else if (module == Module.Staff)
            {
                result = CommonService.getStaffMFSCode(Id);
            }
            else
            {
                result = CommonService.GetSenderAccountNoBySenderId(Id);
            }
            if (result != "")
            {

                return result;
            }
            else
            {
                return "No Account Number Found";
            }
        }
        public AddNewAlertViewModel getAlertEdit(int id, Module Module)
        {
            var result = (from c in dbContext.AgentAlerts.Where(x => x.Id == id).ToList()
                          select new AddNewAlertViewModel()
                          {
                              Id = c.Id,
                              Heading = c.Heading,
                              FullMessage = c.FullMessage,
                              Photo = c.PhotoUrl,
                              PublishedDate = c.PublishedDateAndTime.Date,
                              EndDate = c.EndDate,
                              StartDate = c.StartDate,
                              Country = c.Country == null ? "All" : c.Country,
                              City = c.City == null ? "All" : c.City,
                              AccountNo = getAccountName(Module, (int)c.AgentId),
                              Agent = (int)c.AgentId,
                          }).FirstOrDefault();
            return result;
        }

        public bool saveNewAlert(AddNewAlertViewModel model)
        {
            if (model != null)
            {
                AgentAlerts data = new AgentAlerts()
                {
                    Heading = model.Heading,
                    FullMessage = model.FullMessage,
                    Country = model.Country,
                    City = model.City,
                    AgentId = model.Agent,
                    PhotoUrl = model.Photo,
                    PublishedDateAndTime = DateTime.Now,
                    EndDate = (DateTime)model.EndDate,
                    CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                    IsDeleted = false,
                    StartDate = (DateTime)model.StartDate,
                    Module = model.Module,
                };
                dbContext.AgentAlerts.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool saveEditedAlert(AddNewAlertViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.AgentAlerts.Find(model.Id);
                if (data != null)
                {
                    data.Heading = model.Heading;
                    data.FullMessage = model.FullMessage;
                    data.PhotoUrl = model.Photo;
                    data.ModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.StartDate = (DateTime)model.StartDate;
                    data.EndDate = (DateTime)model.EndDate;
                    data.Module = model.Module;
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
                var data = dbContext.AgentAlerts.Find(id);
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
            var result = (from c in dbContext.AgentInformation
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.CountryCode,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }
        public List<DropDownCityViewModel> GetCitiesFromCountry(string CountryCode)
        {
            var result = (from c in dbContext.AgentInformation.Where(x => x.CountryCode == CountryCode)
                          select new DropDownCityViewModel()
                          {
                              CountryCode = c.CountryCode,
                              City = c.City
                          }).Distinct().ToList();
            return result;
        }

        public List<DropDownAgentViewModel> GetAgentsFromCountryCity(string CountryCode = "", string City = "")
        {
            var result = (from c in dbContext.AgentInformation.Where(x => (x.CountryCode == CountryCode) && ((x.City.Trim()).ToLower() == City))
                          select new DropDownAgentViewModel()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name
                          }).ToList();
            return result;
        }

        public List<DropDownAgentViewModel> GetEmptyAgentsList()
        {
            var result = (from c in dbContext.AgentInformation.Where(x => x.Id == 0)
                          select new DropDownAgentViewModel()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name
                          }).ToList();
            return result;
        }
    }


}