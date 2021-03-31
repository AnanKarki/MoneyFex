using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFindLocation
    {

        DB.FAXEREntities dbContext = null;
        public SFindLocation()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<Models.ServiceTypeDetailVm> GetServiceTypeDetails(int? ServiceType, string Country = "", string City = "" , string PostalCode="" )
        {

            var data = dbContext.AgentLocations.ToList();
            if (ServiceType != null && !string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City)) {

                data = dbContext.AgentLocations.Where(x => x.AgentType == (AgentType)ServiceType && x.CountryCode == Country && x.City.ToLower() == City.ToLower()).ToList();

            }
            else if (!string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City))
            {

                data = dbContext.AgentLocations.Where(x => x.CountryCode == Country && x.City.ToLower() == City.ToLower()).ToList();

            }
            else if (!string.IsNullOrEmpty(Country) )
            {

                data = dbContext.AgentLocations.Where(x => x.CountryCode == Country).ToList();

            }

            if (!string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(PostalCode) && ServiceType != null) {

                if (ServiceType == (int)AgentType.Agent)
                {
                    data = (from c in dbContext.AgentLocations.Where(x => x.AgentType == (AgentType)ServiceType)
                            join d in dbContext.AgentInformation on c.AgentId equals d.Id
                            select c).ToList();
                }
                else {


                    data = (from c in dbContext.AgentLocations.Where(x => x.AgentType == (AgentType)ServiceType)
                            join d in dbContext.KiiPayBusinessInformation on c.AgentId equals d.Id
                            select c).ToList();

                }
            }

          

            var result = (from c in data
                          select new Models.ServiceTypeDetailVm()
                          {
                              BusinessName = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).Name : GetBusinessInfo(c.AgentId).BusinessName,
                              Address = c.Address,
                              City = c.City,
                              Country = Common.Common.GetCountryName(c.CountryCode),
                              BusinessType = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentBusinessType(GetAgentInfo(c.AgentId).RegistrationNumber) : GetMerchantBusinessType(GetBusinessInfo(c.AgentId).RegistrationNumBer),
                              PostalCode = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).PostalCode : GetBusinessInfo(c.AgentId).BusinessOperationPostalCode,
                              State = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).State : GetBusinessInfo(c.AgentId).BusinessOperationState,
                              Telephone = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).PhoneNumber : GetBusinessInfo(c.AgentId).PhoneNumber,
                              Website = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).Website : GetBusinessInfo(c.AgentId).Website,
                              PhoneCode = Common.Common.GetCountryPhoneCode(c.CountryCode),
                              EmailAddress = c.AgentType == Areas.Admin.ViewModels.AgentType.Agent ? GetAgentInfo(c.AgentId).Email : GetBusinessInfo(c.AgentId).Email,
                          }).ToList();



            return result;

        }

        public DB.AgentInformation GetAgentInfo(int ID)
        {


            var data = dbContext.AgentInformation.Where(x => x.Id == ID).FirstOrDefault();
            return data;

        }

        public DB.KiiPayBusinessInformation GetBusinessInfo(int Id)
        {


            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == Id).FirstOrDefault();
            return data;
        }

        public string GetMerchantBusinessType(string registrationNo)
        {


            var businessType = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == registrationNo).Select(x => x.BusinessType).FirstOrDefault();
            
            string value = Common.Common.GetEnumDescription((BusinessType)businessType);
            return value;

        }

        public string GetAgentBusinessType(string registrationNo)
        {


            var businessType = dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == registrationNo).Select(x => x.BusinessType).FirstOrDefault();

           // var businessType = dbContext.AgentInformation.Where(x => x.RegistrationNumber == registrationNo).Select(x => x.BusinessType).FirstOrDefault();

            string value = Common.Common.GetEnumDescription((BusinessType)businessType);
            return value;
        }

    }
}