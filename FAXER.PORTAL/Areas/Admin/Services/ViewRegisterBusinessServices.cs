using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewRegisterBusinessServices
    {
        FAXEREntities dbContext = null;
        public ViewRegisterBusinessServices()
        {
            dbContext = new FAXEREntities();
        }
        CommonServices CommonService = new CommonServices();

        public List<ViewRegisterBusinessViewModel> getBusinessList(string CountryCode = "", string City = "")
        {
            //var data = dbContext.BusinessLogin.Where(x => x.IsDeleted == false).ToList();
            var data = new List<DB.KiiPayBusinessLogin>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformation.BusinessOperationCountryCode == CountryCode && x.IsDeleted == false).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City.ToLower() && x.IsDeleted == false).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformation.BusinessOperationCity.ToLower() == City.ToLower() && x.KiiPayBusinessInformation.BusinessOperationCountryCode == CountryCode && x.IsDeleted == false).ToList();
            }


            var result = (from c in data
                          select new ViewRegisterBusinessViewModel()
                          {
                              Id = c.KiiPayBusinessInformation.Id,
                              BusinessName = c.KiiPayBusinessInformation.BusinessName,
                              BusinessRegNumber = c.KiiPayBusinessInformation.RegistrationNumBer,
                              Email = c.KiiPayBusinessInformation.Email,
                              ContactName = c.KiiPayBusinessInformation.ContactPerson,
                              Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                              Telepone = CommonService.getPhoneCodeFromCountry(c.KiiPayBusinessInformation.BusinessOperationCountryCode) + c.KiiPayBusinessInformation.PhoneNumber,
                              Fax = CommonService.getPhoneCodeFromCountry(c.KiiPayBusinessInformation.BusinessOperationCountryCode) + c.KiiPayBusinessInformation.FaxNumber,
                              Website = c.KiiPayBusinessInformation.Website,
                              BusinessMobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                              City = c.KiiPayBusinessInformation.BusinessOperationCity,
                              Country = CommonService.getCountryNameFromCode(c.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              Address2 = c.KiiPayBusinessInformation.BusinessOperationAddress2,
                              State = c.KiiPayBusinessInformation.BusinessOperationState,
                              PostalCode = c.KiiPayBusinessInformation.BusinessOperationPostalCode,
                              IsActive = c.IsActive ? "Active" : "Inactive",
                              BusinessType = Common.Common.GetEnumDescription((BusinessType)dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == c.KiiPayBusinessInformation.RegistrationNumBer).Select(x => x.BusinessType).FirstOrDefault())
                          }).ToList();



            return result;
        }

        public ViewRegisterBusinessViewModel GetBusinessInfo(int id)
        {
            var data = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.Id == id)
                        join d in dbContext.KiiPayBusinessLogin on c.Id equals d.KiiPayBusinessInformationId into joinedT
                        join e in dbContext.Country on c.BusinessOperationCountryCode equals e.CountryCode
                        from d in joinedT.DefaultIfEmpty()
                        select new ViewRegisterBusinessViewModel()
                        {
                            Id = c.Id,
                            BusinessName = c.BusinessName,
                            BusinessRegNumber = c.BusinessLicenseNumber,
                            Email = c.Email,
                            ContactName = c.ContactPerson,
                            Address = c.BusinessOperationAddress1,
                            Telepone = c.PhoneNumber,
                            Fax = c.FaxNumber,
                            Website = c.Website,
                            BusinessMobileNo = c.BusinessMobileNo,
                            City = c.BusinessOperationCity,
                            Country = e.CountryName,
                            Address2 = c.BusinessOperationAddress2,
                            State = c.BusinessOperationState,
                            PostalCode = c.BusinessOperationPostalCode,
                            IsActive = d.IsActive ? "Active" : "Inactive",
                            LoginCount = d.LoginFailCount,
                            BusinessTypeEnum = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == c.RegistrationNumBer).Select(x => x.BusinessType).FirstOrDefault()
                        }).FirstOrDefault();



            return data;
        }

        public ViewRegisterBusinessViewModel GetBusinessInfoFromRegNum(string RegNumber)
        {
            var data = (from c in dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformation.RegistrationNumBer == RegNumber)
                        select new ViewRegisterBusinessViewModel()
                        {
                            Id = c.KiiPayBusinessInformation.Id,
                            BusinessName = c.KiiPayBusinessInformation.BusinessName,
                            BusinessRegNumber = c.KiiPayBusinessInformation.RegistrationNumBer,
                            Email = c.KiiPayBusinessInformation.Email,
                            ContactName = c.KiiPayBusinessInformation.ContactPerson,
                            Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                            Telepone = c.KiiPayBusinessInformation.PhoneNumber,
                            Fax = c.KiiPayBusinessInformation.FaxNumber,
                            Website = c.KiiPayBusinessInformation.Website,
                            BusinessMobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                            City = c.KiiPayBusinessInformation.BusinessOperationCity,
                            Country = c.KiiPayBusinessInformation.BusinessOperationCountryCode,
                            Address2 = c.KiiPayBusinessInformation.BusinessOperationAddress2,
                            State = c.KiiPayBusinessInformation.BusinessOperationState,
                            PostalCode = c.KiiPayBusinessInformation.BusinessOperationPostalCode,
                            IsActive = c.IsActive ? "Active" : "Inactive",
                        }).FirstOrDefault();
            return data;
        }

        internal bool UpdateMerchant(ViewRegisterBusinessViewModel model)
        {
            var check1 = dbContext.KiiPayBusinessInformation.Where(x => x.Id == model.Id).FirstOrDefault();

            if (check1 != null)
            {

                var BecomeMerchantDetails = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == check1.RegistrationNumBer).FirstOrDefault();
                    //update in database
                    check1.BusinessName = model.BusinessName;
                check1.RegistrationNumBer = model.BusinessRegNumber;
                check1.Email = model.Email;
                check1.ContactPerson = model.ContactName;
                check1.BusinessOperationAddress1 = model.Address;
                check1.BusinessOperationState = model.State;
                check1.BusinessOperationPostalCode = model.PostalCode;
                check1.BusinessOperationCountryCode = model.Country;
                check1.PhoneNumber = model.Telepone;
                check1.FaxNumber = model.Fax;
                check1.Website = model.Website;
                check1.BusinessOperationCity = model.City;
                check1.BusinessOperationAddress2 = model.Address2;


                dbContext.Entry(check1).State = EntityState.Modified;
                dbContext.SaveChanges();


                BecomeMerchantDetails.BusinessType = model.BusinessTypeEnum;
                dbContext.Entry(BecomeMerchantDetails).State = EntityState.Modified;
                dbContext.SaveChanges();
                SCity.Save(check1.BusinessOperationCity, check1.BusinessOperationCountryCode, DB.Module.BusinessMerchant);
                return true;
            }
            return false;
        }

        public bool CheckMerchantEmail(string email)
        {
            email = email.Trim();
            email = email.ToLower();
            var data = dbContext.KiiPayBusinessInformation.Where(x => (x.Email.ToLower()).Trim() == email).FirstOrDefault();
            var data1 = dbContext.BecomeAMerchant.Where(x => (x.BusinessEmailAddress.ToLower()).Trim() == email).FirstOrDefault();
            if ((data != null) && (data1 != null))
            {
                return true;
            }
            return false;
        }
        public ViewRegisterBusinessViewModel GetBecomeMerchant(string regNumber)
        {

            var model = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == regNumber).FirstOrDefault();
            var vm = new ViewRegisterBusinessViewModel()
            {
                BusinessRegNumber = model.RegistrationNumber,
                ContactFirstName = model.FirstName,
                ContactLastName = model.LastName,
                BusinessName = model.CompanyBusinessName,
                BusinessLicenseNumber = model.BusinessLicenseRegistrationNumber,
                Address = model.Address1,
                Address2 = model.Address2,
                Street = model.Street,
                City = model.City,
                State = model.StateProvince,
                PostalCode = model.PostZipCode,
                Country = model.CountryCode,
                Email = model.BusinessEmailAddress,
                Telepone = model.ContactPhone,
                Fax = model.FaxNo,
                Website = model.Website
            };
            return vm;

        }
        internal bool AddMerchant(ViewRegisterBusinessViewModel model)
        {

            var check = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == model.BusinessRegNumber).FirstOrDefault();


            if (check == null)
            {
                string activationCode = Guid.NewGuid().ToString();

                var data = new BecomeAMerchant()
                {
                    RegistrationNumber = model.BusinessRegNumber,
                    FirstName = model.ContactFirstName,
                    LastName = model.ContactLastName,
                    CompanyBusinessName = model.BusinessName,
                    BusinessLicenseRegistrationNumber = model.BusinessLicenseNumber,
                    Address1 = model.Address,
                    Address2 = model.Address2,
                    Street = model.Street,
                    City = model.City,
                    StateProvince = model.State,
                    PostZipCode = model.PostalCode,
                    CountryCode = model.Country,
                    BusinessEmailAddress = model.Email,
                    ContactPhone = model.Telepone,
                    FaxNo = model.Fax,
                    Website = model.Website,
                    ActivationCode = activationCode,
                    BusinessType = model.BusinessTypeEnum
                };
                //insert into database

                var SavedInfo = dbContext.BecomeAMerchant.Add(data);
                dbContext.SaveChanges();
                SCity.Save(data.City, data.CountryCode, DB.Module.BusinessMerchant);

                string msg = "";
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";

                var link = string.Format("{0}/Businesses/BecomeAMerchant/ValidateLink?activationCode={1}", baseUrl, activationCode);
                try
                {

                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessRegistrationEmail?BusinessName=" + data.CompanyBusinessName + "&Link=" + link);

                    //mail.SendMail(SavedInfo.BusinessEmailAddress, "Business Merchant Registration Link.", "Click the following link to get the registration Code " + link);
                    mail.SendMail(SavedInfo.BusinessEmailAddress, "Business Registration Information - Link", body);
                    //mail.SendMail("anankarki97@gmail.com", "Business Registration Information - Link", body);
                    msg = "Registraion Completed";
                }
                catch (Exception)
                {

                    msg = "Problem finding mail";
                }


                ////inserting into businesslogin table
                //var guId = Guid.NewGuid().ToString();
                //var loginDetails = new BusinessLogin()
                //{
                //    Username = model.Email,
                //    LoginCode = GetNewLoginCode(6),
                //    IsActive = false,
                //    ActivationCode = guId,
                //    BusinessInformationId = SavedInfo.Id
                //};
                //dbContext.BusinessLogin.Add(loginDetails);
                //dbContext.SaveChanges();


                return true;

            }
            else
            {
                var check1 = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == model.BusinessRegNumber).FirstOrDefault();
                if (check1 != null)
                {
                    //update in database
                    check1.CompanyBusinessName = model.BusinessName;
                    check1.BusinessEmailAddress = model.Email;
                    check1.FirstName = model.ContactFirstName;
                    check1.LastName = model.ContactLastName;
                    check1.BusinessEmailAddress = model.Email;
                    check1.Address1 = model.Address;
                    check1.Address2 = model.Address2;
                    check1.Street = model.Street;
                    check1.StateProvince = model.State;
                    check.BusinessLicenseRegistrationNumber = model.BusinessLicenseNumber;
                    check1.PostZipCode = model.PostalCode;
                    check1.CountryCode = model.Country;
                    check1.ContactPhone = model.Telepone;
                    check1.FaxNo = model.Fax;
                    check1.Website = model.Website;
                    check1.City = model.City;


                    dbContext.Entry(check1).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    SCity.Save(check1.City, check1.CountryCode, DB.Module.BusinessMerchant);

                    //sending mail to the user
                    string activationCode = check1.ActivationCode;
                    string msg = "";
                    string body = "";
                    MailCommon mail = new MailCommon();
                    var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    var link = string.Format("{0}/Businesses/BecomeAMerchant/ValidateLink?activationCode={1}", baseUrl, activationCode);
                    try
                    {
                        
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessRegistrationEmail?BusinessName=" + check1.CompanyBusinessName + "&Link=" + link);

                        mail.SendMail(check1.BusinessEmailAddress, "Business registration Information - Link", body);
                        //mail.SendMail("anankarki97@gmail.com", "Business registration Information - Link", body);
                        msg = "Registraion Completed";
                    }
                    catch (Exception)
                    {

                        msg = "Problem finding mail";
                    }



                    //var ActivationCode = dbContext.BusinessLogin.Where(x => x.BusinessInformationId == check.Id).FirstOrDefault().ActivationCode;
                    //var Username = check1.Email;
                    //MailCommon mail = new MailCommon();
                    //var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    //var link = string.Format("{0}/Businesses/BusinessSignUp/FirstLogin?ActivationCode={1}", baseUrl, ActivationCode);
                    //try
                    //{

                    //    mail.SendMail(Username, "Click the followng to Activate Your Account", link);
                    //}
                    //catch (Exception)
                    //{

                    //    throw;
                    //}




                    return true;

                }



            }

            return false;

        }

        public bool DeleteMerchant(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
                data.IsDeleted = true;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateAccountStatus(int id)
        {
            var data = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
            if (data != null)
            {
                if (data.IsActive == true)
                {
                    data.IsActive = false;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                else if (data.IsActive == false)
                {
                    data.IsActive = true;
                    data.LoginFailCount = 0;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;

            }
            return false;
        }

        public string GetNewLoginCode(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(5).ToString());
            //return s;
            while (dbContext.KiiPayBusinessLogin.Where(x => x.LoginCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(5).ToString());
            }
            return s;
        }


        public bool AddNewBusinessNote(BusinessNote model)
        {

            dbContext.BusinessNote.Add(model);
            dbContext.SaveChanges();
            return true;
        }


        public List<BusinessNoteViewModel> GetBusinessNotes(int KiiPayBusinessInformationId)
        {

            var result = (from c in dbContext.BusinessNote.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).OrderByDescending(x => x.CreatedDateTime).ToList()
                          select new BusinessNoteViewModel()
                          {
                              Date = c.CreatedDateTime.ToString("dd/MM/yyyy"),
                              Time = c.CreatedDateTime.ToString("HH:mm"),
                              Note = c.Note,
                              StaffName = c.CreatedByStaffName

                          }).ToList();

            return result;
        }
    }
}