using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SRegistrationVerificationCode
    {

        DB.FAXEREntities dbContext = null;
        public SRegistrationVerificationCode()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.RegistrationVerificationCode Add(RegistrationCodeVerificationViewModel vm)
        {

            RegistrationVerificationCode model = new RegistrationVerificationCode()
            {
                UserId = vm.UserId,
                RegistrationOf = vm.RegistrationOf,
                VerificationCode = vm.VerificationCode,
                Country = vm.Country,
                PhoneNo = vm.PhoneNo
            };
            dbContext.RegistrationVerificationCode.Add(model);
            dbContext.SaveChanges();

            return model;
        }


        public bool IsValidVerificationCode(RegistrationCodeVerificationViewModel vm)
        {


            var data = dbContext.RegistrationVerificationCode.Where(x => x.UserId == vm.UserId && x.VerificationCode == vm.VerificationCode && x.RegistrationOf == vm.RegistrationOf && x.IsExpired == false).ToList().LastOrDefault();

            if (data != null)
            {

                data.IsExpired = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                switch (vm.RegistrationOf)
                {
                    case RegistrationOf.Sender:
                        var faxerdata = dbContext.FaxerLogin.Where(x => x.FaxerId == vm.UserId).FirstOrDefault();
                        faxerdata.IsActive = true;
                        dbContext.Entry(faxerdata).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        break;
                    case RegistrationOf.KiiPayPersonal:
                        var VirtualAccountdata = dbContext.KiiPayPersonalUserLogin.Where(x => x.KiiPayPersonalUserInformationId == vm.UserId).FirstOrDefault();
                        VirtualAccountdata.IsActive = true;
                        dbContext.Entry(VirtualAccountdata).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        break;
                    case RegistrationOf.KiiPayBusiness:
                        var Businessdata = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformationId == vm.UserId).FirstOrDefault();
                        Businessdata.IsActive = true;
                        dbContext.Entry(Businessdata).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        break;
                    case RegistrationOf.Staff:

                        var staffdata = dbContext.StaffLogin.Where(x => x.StaffId == vm.UserId).FirstOrDefault();
                        staffdata.IsActive = true;
                        dbContext.Entry(staffdata).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        break;
                    case RegistrationOf.Agent:

                        var agentdata = dbContext.AgentLogin.Where(x => x.AgentId == vm.UserId).FirstOrDefault();
                        agentdata.IsActive = true;
                        dbContext.Entry(agentdata).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        break;
                    default:
                        break;
                }

                return true;
            }
            return false;

        }

        public string GetVerificationCode(string phoneNo)
        {

            var data = dbContext.RegistrationVerificationCode.Where(x => x.PhoneNo.Trim() == phoneNo.Trim()).ToList().LastOrDefault();
            return data == null ? "" : data.VerificationCode;
        }

        public bool UpdateVerificationCode(int userId, string oldVerificationCode, string newVerificationCode)
        {
            var verificationCodeData = dbContext.RegistrationVerificationCode.Where(x => x.UserId == userId && x.VerificationCode.Trim() == oldVerificationCode.Trim()).FirstOrDefault();
            if (verificationCodeData != null)
            {
                verificationCodeData.VerificationCode = newVerificationCode.Trim();
                verificationCodeData.IsExpired = false;
                dbContext.Entry(verificationCodeData).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public FaxerInformation GetFaxerInformation(string email)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Email == email).FirstOrDefault();
            return data;
        }
    

    }
}