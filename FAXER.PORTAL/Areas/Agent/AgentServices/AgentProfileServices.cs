using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentProfileServices
    {
        FAXEREntities dbContext = null;
        CommonServices commonServices = null;
        public AgentProfileServices()
        {
            dbContext = new FAXEREntities();
            commonServices = new CommonServices();
        }

        public AgentProfileViewModel getAgentStaffProfileDetails()
        {
            int agentId = Common.AgentSession.LoggedUser.Id;
            var result = (from c in dbContext.AgentInformation.Where(x => x.Id == agentId).ToList()
                          join agentStaff in dbContext.AgentStaffInformation on c.Id equals agentStaff.AgentId
                          join country in dbContext.Country on c.CountryCode equals country.CountryCode
                          select new AgentProfileViewModel()
                          {
                              Id = c.Id,
                              AgentStaffId = agentStaff.Id,
                              FullName = agentStaff.FirstName + " " + agentStaff.MiddleName + " " + agentStaff.LastName,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              City = c.City,
                              State = c.State,
                              PostalCode = c.PostalCode,
                              Country = country.CountryName,
                              CountryCode = c.CountryCode,
                              CountryPhoneCode = country.CountryPhoneCode,
                              PhoneNumber = c.PhoneNumber,
                              Email = c.Email,
                              PhotoIdUrl = agentStaff.IDDocPhoto,
                              Address = c.Address1 + "," + c.City + "," + country.CountryName,
                              AccountNo = c.AccountNo,
                              AgentName = c.Name,
                              RegistrationNumber = c.RegistrationNumber
                          }).FirstOrDefault();
            result.Address1Hidden = hideCharacters(result.Address1);
            result.CityHidden = hideCharacters(result.City);
            result.PostalCodeHidden = hideCharacters(result.PostalCode);
            result.PhoneNubmerHidden = hideCharacters(result.PhoneNumber);
            result.EmailHidden = hideEmailCharacters(result.Email);
            return result;
        }

        public string hideCharacters(string textString)
        {
            if (!string.IsNullOrEmpty(textString))
            {
                textString = textString.Trim();
                int length = textString.Length;
                string xx = "";
                for (int i = 0; i < length - 2; i++)
                {
                    xx = xx + "*";
                }
                string newString = textString[0] + xx + textString[length - 1];
                return newString;

            }
            return "";
        }

        public string hideEmailCharacters(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                string[] stringArray = email.Split('@');

                stringArray[0] = stringArray[0].Trim();
                int length1 = stringArray[0].Length;
                string xx = "";
                for (int i = 0; i < length1 - 2; i++)
                {
                    xx = xx + "*";
                }
                string newString1 = stringArray[0][0] + xx + stringArray[0][length1 - 1];

                stringArray[1] = stringArray[1].Trim();
                int length2 = stringArray[1].Length;
                string xx1 = "";
                for (int i = 0; i < length2 - 2; i++)
                {
                    xx1 = xx1 + "*";
                }
                string newString2 = stringArray[1][0] + xx1 + stringArray[1][length2 - 1];

                return newString1 + "@" + newString2;

            }
            return "";
        }

        public string getAgentStaffPhoneNumberWithCode()
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.Id == Common.AgentSession.AgentStaffLogin.AgentStaffId).FirstOrDefault();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data.Country) && !string.IsNullOrEmpty(data.PhoneNumber))
                {
                    string countryPhoneCode = commonServices.getPhoneCodeFromCountry(data.Country).Trim();
                    string phoneNumber = data.PhoneNumber.Trim();
                    return countryPhoneCode + phoneNumber;
                }
            }
            return "";
        }

        public bool updateAgentStaffAddressDetails(AgentProfileViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.AgentStaffInformation.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.Address1 = model.Address1;
                    data.Address2 = model.Address2;
                    data.City = model.City;
                    data.State = model.State;
                    data.PostalCode = model.PostalCode;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        internal void UpdateAgentProfile(AgentProfileViewModel vm)
        {
            var agentInfo = dbContext.AgentInformation.Where(x => x.Id == vm.Id).FirstOrDefault();
            agentInfo.Email = vm.Email;
            agentInfo.PhoneNumber = vm.PhoneNumber;
            agentInfo.Address1 = vm.Address1;
            agentInfo.Address2 = vm.Address2;
            agentInfo.City = vm.City;
            agentInfo.State = vm.State;
            agentInfo.PostalCode = vm.PostalCode;
            dbContext.Entry(agentInfo).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            var agentStaffInformation = dbContext.AgentStaffInformation.Where(x => x.Id == vm.AgentStaffId).FirstOrDefault();
            agentStaffInformation.Address1 = vm.Address1;
            agentStaffInformation.Address2 = vm.Address2;
            agentStaffInformation.City = vm.City;
            agentStaffInformation.State = vm.State;
            agentStaffInformation.PostalCode = vm.PostalCode;
            agentStaffInformation.EmailAddress = vm.Email;
            agentStaffInformation.PhoneNumber = vm.PhoneNumber;
            dbContext.Entry(agentStaffInformation).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
           
        }

        public bool updatePhoneNumber(int agentStaffId, string phoneNumber)
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            if (data != null)
            {
                data.PhoneNumber = phoneNumber;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool updateEmail(int agentStaffId, string email)
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            var data1 = dbContext.AgentStaffLogin.Where(x => x.AgentStaffId == agentStaffId).FirstOrDefault();

            if (data != null && data1 != null)
            {
                data.EmailAddress = email;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                data1.Username = email;
                dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


                return true;
            }
            return false;
        }

        public bool updatePhotoUrl(int agentStaffId, string photoUrl)
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            if (data != null)
            {
                data.IDDocPhoto = photoUrl;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
        public string GetMobilePin()
        {
            string code = "";
            if (Common.AgentSession.VerificationCode == null || Common.AgentSession.VerificationCode == "")
            {

                code = Common.Common.GenerateRandomDigit(8);
                Common.AgentSession.VerificationCode = code;

                SmsApi smsApiServices = new SmsApi();
                var message = smsApiServices.GetAddressUpdateMessage(code);
                string phoneNo = getAgentStaffPhoneNumberWithCode();
                smsApiServices.SendSMS(phoneNo, message);
            }
            else
            {
                code = Common.AgentSession.VerificationCode;
            }
            string mobilePinCode = code;


            return mobilePinCode;


        }
    }
}