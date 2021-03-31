using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using Twilio.Rest.Preview.Wireless;

namespace FAXER.PORTAL.Services
{
    public class SSenderProfileService
    {
        DB.FAXEREntities dbContext = null;
        public SSenderProfileService()
        {
            dbContext = new DB.FAXEREntities();
        }



        public ServiceResult<FaxerInformation> Update(FaxerInformation model)
        {
            dbContext.Entry<FaxerInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<FaxerInformation>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public SenderProfileEditViewModel GetSenderProfileEdit()
        {
            var SenderId = Common.FaxerSession.LoggedUser == null ? 0 : Common.FaxerSession.LoggedUser.Id;

            var result = (from c in dbContext.FaxerInformation.Where(x => x.Id == SenderId).ToList()
                          select new SenderProfileEditViewModel()
                          {
                              Id = c.Id,
                              City = c.City,
                              Country = c.Country,
                              Address = c.Address1,
                              AddressLine2 = c.Address2,
                              PostCode = c.PostalCode,
                              MobileNumber = c.PhoneNumber,
                              EmailAddress = c.Email,
                              IdCardType = c.IdCardType == null ? 0 : Convert.ToInt32(c.IdCardType),
                              IdCardTypeName = c.IdCardType == null ? "" : Common.Common.GetIDCardTypeName(Convert.ToInt32(c.IdCardType)),
                              IdCardNumber = c.IdCardNumber,
                              IdIssuingCountry = c.IssuingCountry,
                              IdExpiringDate = c.IdCardExpiringDate,
                              AccountNo = c.AccountNo,
                              DateOfBirth = c.DateOfBirth,
                              MobileCode = Common.Common.GetCountryPhoneCode(c.Country)

                          }).FirstOrDefault();

            if (result.IdExpiringDate != default(DateTime))
            {

                result.Day = result.IdExpiringDate.Day;
                result.Month = (Month)result.IdExpiringDate.Month;
                result.Year = result.IdExpiringDate.Year;

            }

            return result;

        }

        public DB.FaxerInformation UpdateSenderInformation(SenderProfileEditViewModel model)
        {

            var data = dbContext.FaxerInformation.Where(x => x.Id == model.Id).FirstOrDefault();
            data.Address1 = model.Address;
            data.Address2 = model.AddressLine2;
            data.City = model.City;
            data.PostalCode = model.PostCode;
            data.PhoneNumber = model.MobileNumber;
            data.Email = model.EmailAddress;
            //data.Country = model.Country;
            data.DateOfBirth = model.DateOfBirth;


            dbContext.Entry<FaxerInformation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

            UpdateFaxerLogin(model);

            //HttpContext.Current.Session.Remove("SentMobilePinCode");
            return data;

        }

        private void UpdateFaxerLogin(SenderProfileEditViewModel model)
        {
            var data = dbContext.FaxerLogin.Where(x => x.FaxerId == model.Id).FirstOrDefault();
            data.UserName = model.EmailAddress;
            data.MobileNo = model.MobileNumber;
            dbContext.Entry<FaxerLogin>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public string SetMobilePinCode(string pinCode)
        {
            Common.FaxerSession.SentMobilePinCode = pinCode;
            return pinCode;
        }


        public string GetMobilePinCode()
        {
            // SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();
            var pinCode = "";
            if (Common.FaxerSession.SentMobilePinCode != null)
            {
                pinCode = Common.FaxerSession.SentMobilePinCode;
            }
            return pinCode;
        }

        public List<IdentificationDetailModel> GetIdentitificationDetailsOfSender(int senderId)
        {
            CommonServices _CommonServices = new CommonServices();
            var senderDocumnetationDetails = _CommonServices.GetSenderDocumentation(senderId);
            var result = (from c in senderDocumnetationDetails
                          join d in dbContext.IdentityCardType on c.IdentificationTypeId equals d.Id into joined
                          from d in joined.DefaultIfEmpty()
                          select new IdentificationDetailModel()
                          {
                              IdentificationTypeId = c.IdentificationTypeId,
                              IdentityNumber = c.IdentityNumber == null ? "xxx-" + Maskidentity(c.DocumentName)
                              : "xxx-" + Maskidentity(c.IdentityNumber),
                              IdentificationType = d == null ? "" : d.CardType,
                              SenderBusinessDocumentationId = c.Id,
                              Status = c.Status,
                              StatusName = Common.Common.GetEnumDescription(c.Status),
                              DocumentUrl = c.DocumentPhotoUrl,
                              DocumentUrlTwo = c.DocumentPhotoUrlTwo,
                              Day = c.ExpiryDate == null ? 0 : c.ExpiryDate.Value.Day,
                              Year = c.ExpiryDate == null ? 0 : c.ExpiryDate.Value.Year,
                              Month = c.ExpiryDate == null ? 0 : (Month)Enum.Parse(typeof(Month), c.ExpiryDate.Value.Month.ToString()),
                              IssuingCountry = c.IssuingCountry,
                              DocumentName = c.DocumentName,

                          }).ToList();
            return result;
        }
        private string Maskidentity(string no)
        {

            if (!string.IsNullOrEmpty(no))
            {

                if (no.Length < 2)
                {

                    return no.Substring(no.Length - 1, 1);

                }
                else
                {


                    return no.Substring(no.Length - 2, 2);
                }
            }
            return "";
        }

        private string GetIdentitiyNumber(string identityNumber)
        {
            string data = "***" + identityNumber.Substring(identityNumber.Length - 2, 2);
            return data;
        }


        internal void DeleteIdentityVerification(int documentationId)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == documentationId).FirstOrDefault();
            dbContext.SenderBusinessDocumentation.Remove(data);
            dbContext.SaveChanges();
        }
    }
}