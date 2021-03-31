using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class CashWithdrawalServices
    {
        FAXEREntities db = null;
        public CashWithdrawalServices()
        {
            db = new FAXEREntities();
        }


        public List<WithdrawalListVm> getCashWithdrawalList()
        {
            var agentCashWithdrawal = (from c in db.CashWithdrawalByAgent.ToList()
                                       select new WithdrawalListVm()
                                       {
                                           Id = c.Id,
                                           WithdrawalType = CashWithdrawalType.WithdrawalByAgent,
                                           Amount = c.WithdrawalAmount,
                                           StaffName = c.AgentStaffName,
                                           StaffCode = c.AgentStaffName,
                                           DateAndTime = c.TransactionDateTime,
                                           TransactionMonth = c.TransactionDateTime.Month,
                                           Status = c.Status,
                                           ReceiptUrl = "",
                                           ReceiptNo = c.ReceiptNo,
                                           AgentId = c.AgentId
                                       }).ToList();

            var staffCashWithdrawal = (from c in db.CashWithdrawalByStaff.ToList()
                                       select new WithdrawalListVm()
                                       {
                                           Id = c.Id,
                                           WithdrawalType = CashWithdrawalType.MoneyFexStaffWithdrawal,
                                           Amount = c.WithdrawalAmount,
                                           StaffName = c.Staff.FirstName + " " + c.Staff.MiddleName + " " + c.Staff.LastName,
                                           StaffCode = c.Staff.StaffMFSCode,
                                           DateAndTime = c.TransactionDateTime,
                                           TransactionMonth = c.TransactionDateTime.Month,
                                           Status = c.Status,
                                           ReceiptUrl = "",
                                           ReceiptNo = c.ReceiptNo,
                                           AgentId = c.AgentId
                                       }).ToList();
            var finalList = agentCashWithdrawal.Concat(staffCashWithdrawal).OrderBy(x => x.DateAndTime).ToList();
            return finalList;
        }
        public bool isStaffExist(string staffMFSCode)
        {
            if (!string.IsNullOrEmpty(staffMFSCode))
            {
                var data = db.StaffInformation.Where(x => x.StaffMFSCode.Trim() == staffMFSCode.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidWithdrawalCode(string WithdrawalCode, string StaffCode)
        {

            var IsValid = db.AgentCashWithdrawalCode.Where(x => x.WithdrawalCode == WithdrawalCode &&
                                              x.StaffCode == StaffCode
                                              && x.Status == DB.AgentWithdrawalCodeStatus.NoUse).FirstOrDefault() == null ? false : true;

            return IsValid;
        }
        public StaffWithdrawalViewModel getStaffData(string staffMFSCode)
        {
            if (!string.IsNullOrEmpty(staffMFSCode))
            {
                var data = db.StaffInformation.Where(x => x.StaffMFSCode.Trim() == staffMFSCode.Trim()).FirstOrDefault();
                if (data != null)
                {
                    StaffWithdrawalViewModel model = new StaffWithdrawalViewModel()
                    {
                        StaffId = data.Id,
                        StaffCode = data.StaffMFSCode,
                        FirstName = data.FirstName,
                        MiddleName = data.MiddleName,
                        LastName = data.LastName,
                        Address = data.Address1,
                        StaffMoneyFexCode = data.StaffMFSCode,
                        StaffImageUrl = "",
                        AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName

                    };
                    return model;
                }
            }
            return null;
        }

        public CashWithdrawalByStaff saveCashWithdrawalByStaff(StaffWithdrawalViewModel model)
        {

            CashWithdrawalByStaff data = new CashWithdrawalByStaff()
            {
                StaffId = model.StaffId,
                AgentId = Common.AgentSession.AgentInformation.Id,
                StaffIdCardType = model.IDType,
                StaffIdCardNumber = model.IDNumber,
                StaffIDCardExpiryDate = model.IDExpiryDate,
                StaffIDCardIssuingCountry = model.IssuingCountry,
                AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                WithdrawalAmount = model.WithdrawalAmount,
                TransactionDateTime = DateTime.Now,
                Status = WithdrawalStatus.Pending,
                CashWithdrawalCode = Common.AgentSession.CashWithdrawalCode,
                AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                ReceiptNo = GetCashWithdrawalReceiptNo()
            };
            db.CashWithdrawalByStaff.Add(data);
            db.SaveChanges();
            return data;

        }

        public CashWithdrawalByAgent saveCashWithdrawalByAgent(AgentWithdrawViewModel model)
        {

            CashWithdrawalByAgent data = new CashWithdrawalByAgent()
            {
                AgentId = Common.AgentSession.AgentInformation.Id,
                AgentStaffName = model.AgentStaffName,
                WithdrawalAmount = model.WithdrawAmount,
                TransactionDateTime = DateTime.Now,
                Status = WithdrawalStatus.Pending,
                ReceiptNo = GetCashWithdrawalReceiptNo()
            };
            db.CashWithdrawalByAgent.Add(data);
            db.SaveChanges();
            return data;

        }

        public bool UpdateCashWithdrawalCode(string WithdrawalCode)
        {

            var data = db.AgentCashWithdrawalCode.Where(x => x.WithdrawalCode == WithdrawalCode).FirstOrDefault();
            data.Status = AgentWithdrawalCodeStatus.Use;
            db.Entry(data).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return true;

        }

        public CashWithdrawalReceiptVM GetCashWithdrawalReceiptDetails(int TransactionId, bool IsWithdrawalByAgent = false)
        {

            var data = new CashWithdrawalReceiptVM();
            if (IsWithdrawalByAgent == false)
            {
                data = (from c in db.CashWithdrawalByStaff.Where(x => x.Id == TransactionId).ToList()
                        join d in db.StaffInformation on c.StaffId equals d.Id
                        select new CashWithdrawalReceiptVM()
                        {
                            ReceiptNo = c.ReceiptNo,
                            Date = c.TransactionDateTime.ToString("dd/MM/yyyy"),
                            Time = c.TransactionDateTime.ToString("HH:mm"),
                            AgentName = c.Agent.Name,
                            AgentAccountNO = c.Agent.AccountNo,
                            StaffName = c.Staff.FirstName + " " + c.Staff.MiddleName + " " + c.Staff.LastName,
                            StaffCode = c.Staff.StaffMFSCode,
                            WithdrawalAmount = c.WithdrawalAmount,
                            WithdrawalCode = c.CashWithdrawalCode,
                            Currency = Common.Common.GetCurrencySymbol(c.Agent.CountryCode),
                            WithDrawalType = "Withdrawal By Staff",
                            AdminCodeGenerator = GetAdminCodeGenerator(c.CashWithdrawalCode),
                            IsWithdrawalByAgent = false


                        }).FirstOrDefault();
            }
            else
            {

                data = (from c in db.CashWithdrawalByAgent.Where(x => x.Id == TransactionId).ToList()
                        select new CashWithdrawalReceiptVM()
                        {
                            ReceiptNo = c.ReceiptNo,
                            Date = c.TransactionDateTime.ToString("dd/MM/yyyy"),
                            Time = c.TransactionDateTime.ToString("HH:mm"),
                            AgentName = c.Agent.Name,
                            AgentAccountNO = c.Agent.AccountNo,
                            WithdrawalAmount = c.WithdrawalAmount,
                            Currency = Common.Common.GetCurrencySymbol(c.Agent.CountryCode),
                            WithDrawalType = "Withdrawal By Agent",
                            IsWithdrawalByAgent = true
                        }).FirstOrDefault();
            }
            return data;

        }





        private string GetAdminCodeGenerator(string WithdrawalCode)
        {
            var WithdrawalCodeDetails = db.AgentCashWithdrawalCode.Where(x => x.WithdrawalCode == WithdrawalCode).FirstOrDefault();

            if (WithdrawalCodeDetails.CodeGeneratorId > 0)
            {

                var result = db.StaffLogin.Where(x => x.StaffId == WithdrawalCodeDetails.CodeGeneratorId).FirstOrDefault();
                return "Admin-" + result.LoginCode.Substring(result.LoginCode.Length - 4, 4);

            }

            return "";
        }
        internal string GetCashWithdrawalReceiptNo()
        {
            string AgentCode = Common.AgentSession.AgentInformation.AccountNo;
            //this code should be unique and random with 8 digit length
            var val = "CW-" + Common.Common.GenerateRandomDigit(6);
            while ((db.CashWithdrawalByAgent.Where(x => x.ReceiptNo == val).Count() > 0) && db.CashWithdrawalByStaff.Where(x => x.ReceiptNo == val).Count() > 0)
            {
                val = GetCashWithdrawalReceiptNo();
            }
            return val;
        }
    }
}