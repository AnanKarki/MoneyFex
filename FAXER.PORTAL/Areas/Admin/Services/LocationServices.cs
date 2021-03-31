using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class LocationServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices ComonnService = new CommonServices();

        public AddNewLocationViewModel GetLocation(int id)
        {
            var result = (from c in dbContext.AgentLocations.Where(x => x.Id == id).ToList()
                          select new AddNewLocationViewModel()
                          {
                              Id = c.Id,
                              Address=c.Address,
                              AgentId=c.AgentId,
                              AgentType=c.AgentType,
                              City=c.City,
                              ContactNo=c.ContactNo,
                              Country=c.CountryCode
                              

                          }).FirstOrDefault();
            return result;
        }

        public List<LocationViewModel> getView()
        {
            var result = (from c in dbContext.AgentLocations.Where(x => x.IsDeleted == false).ToList()
                          select new LocationViewModel()
                          {
                              Id = c.Id,
                              Country = ComonnService.getCountryNameFromCode(c.CountryCode),
                              City = c.City,
                              AgentType = c.AgentType,
                              AgentName = c.AgentType == AgentType.Agent ? ComonnService.getAgentName(c.AgentId) : ComonnService.getBusinessName(c.AgentId),
                              Address = c.Address,
                              ContactNo = c.ContactNo,
                              AgentId=c.AgentId,
                          }).ToList();
            return result;
        }

        public List<DropDownAgentViewModel> getAgentList(string country = "", string city = "", int agentType=0)
        {
            if (agentType == 1)
            {
                var result = (from c in dbContext.AgentInformation.Where(x => x.CountryCode == country && x.City.ToLower() == city.ToLower()).ToList()
                              select new DropDownAgentViewModel()
                              {
                                  AgentId = c.Id,
                                  AgentName = c.Name
                              }).ToList();
                return result;
            }
            else if (agentType == 2)
            {
                var result = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.BusinessOperationCountryCode == country && x.BusinessOperationCity.ToLower() == city.ToLower()).ToList()
                              select new DropDownAgentViewModel()
                              {
                                  AgentId = c.Id,
                                  AgentName = c.BusinessName
                              }).ToList();
                return result;
            }
            var result1 = new List<DropDownAgentViewModel>();
            return result1;
            
        }
        public bool saveEditLocation(AddNewLocationViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.AgentLocations.Find(model.Id);
                if (data != null)
                {
                    data.ContactNo = model.ContactNo;
                    data.City = model.City;
                    data.AgentId = model.AgentId;
                    data.Address = model.Address;
                    data.AgentType = model.AgentType;
                    data.ModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool saveLocation (AddNewLocationViewModel model)
        {
            if (model != null)
            {
                AgentLocations data = new AgentLocations
                {
                    CountryCode = model.Country,
                    City = model.City,
                    AgentType = model.AgentType,
                    AgentId = model.AgentId,
                    Address = model.Address,
                    ContactNo = model.ContactNo
                };
                dbContext.AgentLocations.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public string getAddress(int agentType=0, int agentid=0)
        {
            if (agentType == 1)
            {
                var data = dbContext.AgentInformation.Find(agentid);
                if (data == null)
                {
                    return "";
                }
                return data.Address1;
            }
            else if (agentType == 2)
            {
                var data = dbContext.KiiPayBusinessInformation.Find(agentid);
                if (data == null)
                {
                    return "";
                }
                return data.BusinessOperationAddress1;
            }
            return null;
        }

        public string getContactNo (int agentType = 0, int agentId = 0)
        {
            if (agentType == 1)
            {
                var data = dbContext.AgentInformation.Find(agentId);
                if (data == null)
                {
                    return "";
                }
                return data.PhoneNumber;
            }
            else if (agentType == 2)
            {
                var data = dbContext.KiiPayBusinessInformation.Find(agentId);
                if (data == null)
                {
                    return "";
                }
                return data.PhoneNumber;
            }
            return null;
        }

        public bool deleteLocation(int id)
        {
            if (id != 0)
            {
                var data = dbContext.AgentLocations.Where(x => x.Id == id).FirstOrDefault();
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



    }
}