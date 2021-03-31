using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentStaffRegistrationServices
    {
        DB.FAXEREntities dbContext = null;
        public AgentStaffRegistrationServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        public DB.BecomeAnAgent GetBecomeAnAgentInfo(string agentregistrationNo)
        {

            var data = dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == agentregistrationNo).FirstOrDefault();
            return data;
        }
        public AgentInformation CreateAgent(AgentInformtionViewModel vm)
        {
            var AgentEmail = Common.AgentSession.StaffContaactDetailsViewModel.EmailAddress;
            string accountNo = "";
            if (Common.AgentSession.IsAuxAgent)
            {
                accountNo = "AA" + Common.Common.GenerateRandomDigit(5);
            }
            else
            {
                accountNo = Common.Common.GenerateRandomDigit(10);
            }

            var model = new AgentInformation()
            {
                Address1 = vm.AddressLine1,
                Address2 = vm.AddressLine2,
                ContactPerson = vm.ContactPersonName,
                CountryCode = vm.CountryCode,
                Name = vm.AgentName,
                Password = "",
                PhoneNumber = vm.MobileNumber.FormatPhoneNo(),
                PostalCode = vm.ZipCode,
                State = vm.State,
                AccountNo = accountNo,
                City = vm.City,
                LicenseNumber = vm.BusinessRegistrationNumber,
                RegistrationNumber = vm.BusinessRegistrationNumber,
                //BusinessType = vm.BusinessType,
                TypeOfBusiness = vm.BusinessType,
                Email = AgentEmail,
                IsAUXAgent = Common.AgentSession.IsAuxAgent,
                AgentStatus = AgentStatus.Inactive,
                CreatedDate = DateTime.Now

            };
            var data = dbContext.AgentInformation.Add(model);
            dbContext.SaveChanges();

            return model;
        }

        public DB.AgentInformation GetAgentInformation(int AgentId)
        {

            var data = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            return data;

        }
        public AgentStaffInformation CreateAgentStaff(DB.AgentStaffInformation agentStaffInformation)
        {


            dbContext.AgentStaffInformation.Add(agentStaffInformation);
            dbContext.SaveChanges();

            return agentStaffInformation;


        }

        public AgentInformation CreateAgentInformation(DB.AgentInformation model)
        {



            dbContext.AgentInformation.Add(model);
            dbContext.SaveChanges();
            City city = new City()
            {
                CountryCode = model.CountryCode,
                Module = Module.Agent,
                Name = model.City
            };
            SCity.Save(city);
            return model;


        }


        public bool checkUniqueEmail(string email, int AgentStaffId = 0)
        {
            if (AgentStaffId > 0)
            {
                var data = dbContext.AgentStaffInformation.Where(x => x.EmailAddress == email && x.Id == AgentStaffId).FirstOrDefault();
                if (data != null)
                {
                    return false;
                }
                else return true;
            }
            else
            {

                var data = dbContext.AgentStaffInformation.Where(x => x.EmailAddress == email).FirstOrDefault();
                if (data != null)
                {
                    return false;
                }
                else return true;
            }
        }
        public AgentStaffInformation UpdateAgentStaffInformation(DB.AgentStaffInformation model)
        {

            dbContext.Entry<AgentStaffInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();

            return model;

        }

        public AgentStaffLogin CreateAgentStaffLogin(DB.AgentStaffLogin agentStaffLogin)
        {

            dbContext.AgentStaffLogin.Add(agentStaffLogin);
            dbContext.SaveChanges();
            return agentStaffLogin;

        }

        public AgentStaffLogin UpdateAgentStaffLogin(DB.AgentStaffLogin agentStaffLogin)
        {

            agentStaffLogin.UpdataedBy = Common.AgentSession.AgentStaffLogin.AgentStaffId;
            agentStaffLogin.UpdataedDate = DateTime.Now;
            dbContext.Entry<AgentStaffLogin>(agentStaffLogin).State = EntityState.Modified;
            dbContext.SaveChanges();
            return agentStaffLogin;

        }

        public AgentLogin CreateAgentLogin(AgentLogin model)
        {

            dbContext.AgentLogin.Add(model);
            dbContext.SaveChanges();
            return model;
        }


        public AgentStaffLogin GetAgentStaffLogin(int agentStaffLoginId)
        {

            var agentStaffLogin = dbContext.AgentStaffLogin.Where(x => x.Id == agentStaffLoginId).FirstOrDefault();
            return agentStaffLogin;

        }
        public AgentStaffLogin GetAgentStaffLoginbYagentStaffId(int agentStaffId)
        {

            var agentStaffLogin = dbContext.AgentStaffLogin.Where(x => x.AgentStaffId == agentStaffId).FirstOrDefault();
            return agentStaffLogin;

        }


        public string CheckStaffName(int id)
        {

            //day and time validation
            var data1 = dbContext.AgentStaffLogin.Where(x => x.Id == id).FirstOrDefault();
            if (data1 == null)
            {
                return "noData";
            }

            if (data1 != null)
            {
                DayOfWeek today = DateTime.Today.DayOfWeek;
                int today1 = (int)today;
                var startDay = (int)data1.StartDay;
                var endDay = (int)data1.EndDay;
                if (startDay < endDay)
                {
                    if (!(today1 >= startDay && today1 <= endDay))
                    {

                        return "dayMismatch";
                    }
                }
                else if (endDay < startDay)
                {
                    if (!((today1 >= startDay && today1 <= 6) || (today1 >= 0 && today1 <= endDay)))
                    {
                        return "dayMismatch";
                    }
                }

                //time validation
                TimeSpan diff = new TimeSpan();
                var ts = -new TimeSpan(0, int.Parse(Common.AgentSession.AgentTimeZone), 0);
                TimeSpan serverTime = getServerTimeByTimeZone();
                if (serverTime < ts)
                {

                    diff = (ts - serverTime);

                }
                else
                {
                    diff = ts - serverTime;
                }

                TimeSpan totalTime = System.DateTime.Now.TimeOfDay.Add(diff);
                if (!((totalTime >= TimeSpan.Parse(data1.StartTime)) && (totalTime <= TimeSpan.Parse(data1.EndTime))))
                {
                    return "logInTime";
                }
            }


            return "success";
        }

        private TimeSpan getServerTimeByTimeZone()
        {

            return TimeZoneInfo.Local.BaseUtcOffset;
        }

        internal bool IsAgentCodeValid(string agentCode)
        {
            bool isvalid = false;
            var agent = dbContext.AgentLogin.Where(x => x.LoginCode == agentCode).FirstOrDefault();
            if (agent != null)
            {
                isvalid = true;
            }
            return isvalid;
        }

        internal void AddAgentDocument(StaffComplianceDocViewModel vm, int agentStafId)
        {
            var identityCardId = dbContext.IdentityCardType.Where(x => x.CardType.ToLower() == vm.IdCaardType.ToLower()).Select(x => x.Id).FirstOrDefault();
            var agentStaffInfo = dbContext.AgentStaffInformation.Where(x => x.Id == agentStafId).FirstOrDefault();
            AgentNewDocument model = new AgentNewDocument()
            {
                AgentStaffId = agentStafId,
                DocumentPhotoUrl = vm.IdentificationDoc,
                DocumentTitleName = vm.IdCaardType,
                IdentificationNumber = vm.IdCardNumber,
                IssuingCountry = vm.IssuingCountry,
                IdentificationTypeId = identityCardId ,
                DocumentType = DocumentType.Staff,
                Status = DocumentApprovalStatus.InProgress,
                UploadDateTime = DateTime.Now,
                ExpiryDate = new DateTime(vm.ExpiryYear, (int)vm.ExpiryMonth, vm.ExpiryDay),
                AgentStaffAccountNo = agentStaffInfo.AgentMFSCode,
                AgentStaffName = agentStaffInfo.FirstName + " " + (string.IsNullOrEmpty(agentStaffInfo.MiddleName) == true ? "" : agentStaffInfo.MiddleName + " ") + agentStaffInfo.LastName,
                DocumentExpires = DocumentExpires.Yes,
            };
            dbContext.AgentNewDocument.Add(model);
            dbContext.SaveChanges();
        }
    }
}