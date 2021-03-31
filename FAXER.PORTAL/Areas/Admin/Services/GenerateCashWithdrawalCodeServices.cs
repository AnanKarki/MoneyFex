using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class GenerateCashWithdrawalCodeServices
    {

        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        public GenerateCashWithdrawalCodeServices()
        {
            _commonServices = new CommonServices();
            dbContext = new DB.FAXEREntities();
        }

        public GenerateCashWithdrawalCodeVM Add(GenerateCashWithdrawalCodeVM vm)
        {


            vm.WithdrawalCode = GenerateWithdrawalCode();
            var model = new DB.AgentCashWithdrawalCode()
            {

                AgentId = vm.AgentId,
                AgentCode = vm.AgentCode,
                City = vm.City,
                CountryCode = vm.CountryCode,
                StaffCode = vm.StaffCode,
                StaffId = vm.StaffId,
                Status = DB.AgentWithdrawalCodeStatus.NoUse,
                WithdrawalCode = vm.WithdrawalCode,
                CodeGeneratorId = Common.StaffSession.LoggedStaff.StaffId,
                GeneratedDate=DateTime.Now
                

            };

            dbContext.AgentCashWithdrawalCode.Add(model);
            dbContext.SaveChanges();
            // Sms


            Common.SmsApi smsApiServices = new Common.SmsApi();
            string AgentName = dbContext.AgentInformation.Where(x => x.AccountNo == model.AgentCode).Select(x => x.Name).FirstOrDefault();
            string message = smsApiServices.GetCashWithdrawalCodeMSG(AgentName, model.WithdrawalCode);

            string PhoneNo = Common.Common.GetCountryPhoneCode(model.CountryCode) + " " + dbContext.StaffInformation.Where(x => x.StaffMFSCode == model.StaffCode).Select(x => x.PhoneNumber).FirstOrDefault();
            smsApiServices.SendSMS(PhoneNo, message);
            // End 
            return vm;
        }

        public List<GenerateCashWithdrawalCodeVM> GetWithdrawalCodeInformation()
        {

            var result = (from c in dbContext.AgentCashWithdrawalCode.ToList()
                          select new GenerateCashWithdrawalCodeVM()
                          {
                              AgentCode = c.Agent.AccountNo,
                              AgentName = c.Agent.Name,
                              City = c.City,
                              CountryCode = c.CountryCode,
                              CountryName = Common.Common.GetCountryName(c.CountryCode),
                              StaffCode = c.Staff.StaffMFSCode,
                              StaffName = c.Staff.FirstName + " " + c.Staff.MiddleName + " " + c.Staff.LastName,
                              WithdrawalCode = c.WithdrawalCode,
                              StatusName = Enum.GetName(typeof(DB.AgentWithdrawalCodeStatus), c.Status),
                              Status = c.Status,
                              Date = c.GeneratedDate,
                              GeneratedDate = c.GeneratedDate.ToString("MMM-dd-yyyy"),
                              GeneratedStaffName = _commonServices.getStaffName(c.CodeGeneratorId)
                          }).ToList();

            return result;

        }

        public string GenerateWithdrawalCode()
        {

            var code = Common.Common.GenerateRandomDigit(10);
            while (dbContext.AgentCashWithdrawalCode.Where(x => x.WithdrawalCode == code).Count() > 0)
            {

                code = Common.Common.GenerateRandomDigit(10);
            }

            return code;

        }

        public List<DropDownStaffViewModel> GetStaffDropdownList(string countryCode, string city)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => (x.Country == countryCode))
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownAgentViewModel> GetAgentDropdownList(string countryCode, string city)
        {
            var result = (from c in dbContext.AgentInformation.Where(x => (x.CountryCode == countryCode) && ((x.City.Trim()).ToLower() == city.ToLower().Trim()))
                          select new DropDownAgentViewModel()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name
                          }).ToList();
            return result;
        }

        public DB.AgentInformation GetAgentInformation(int AgentId)
        {

            var data = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            return data;

        }

        public DB.StaffInformation GetStaffInformation(int StaffId)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == StaffId).FirstOrDefault();
            return data;

        }
    }
}