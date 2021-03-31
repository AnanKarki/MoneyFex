using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewRegisteredPartnersServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<PartnerInformation> List()
        {
            var data = dbContext.PartnerInformation.ToList();
            return data;
        }


        public List<PartnerInformation> ListOfActivePartner()
        {
            var data = dbContext.PartnerInformation.Where(x=>x.IsActive==true).ToList();
            return data;
        }

        public List<PartnerInformation> ListOfInActivePartner()
        {
            var data = dbContext.PartnerInformation.Where(x => x.IsActive == false).ToList();
            return data;
        }
        public List<ViewRegisteredPartnersViewModel> getList(string CountryCode = "", string City = "")
        {

            var data = dbContext.PartnerInformation.Where(x => x.IsDeleted == false);
            if (!string.IsNullOrEmpty(CountryCode))
            {
                data = data.Where(x => x.Country == CountryCode);
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City.ToLower() == City.ToLower());

            }

            var result = (from c in data.ToList()
                          select new ViewRegisteredPartnersViewModel()
                          {
                              Id = c.Id,
                              NameOfPartner = c.Name,
                              Address = c.Address1 + ", " + c.City + ", " + c.State + ", " + CommonService.getCountryNameFromCode(c.Country),
                              Telephone = c.Phone,
                              Email = c.Email,
                              Website = c.Website,
                              PartnerType = c.PartnerType,
                              PartnerLogo = c.LogoUrl,
                              Status = c.IsActive == true ? "Active" : "Inactive",
                              FirstLetterOfPartner = c.Name == null ? "" : c.Name.Substring(0, 1).ToLower(),
                          }).ToList();

            return result;
        }

        public RegisterAPartnerViewModel getInfo(int id)
        {
            if (id != 0)
            {
                var c = dbContext.PartnerInformation.Find(id);
                if (c != null)
                {
                    RegisterAPartnerViewModel info = new RegisterAPartnerViewModel()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        LicenseNo = c.LicenseNo,
                        ContactPersonName = c.ContactPersonName,
                        PartnerType = c.PartnerType,
                        Address1 = c.Address1,
                        Address2 = c.Address2,
                        State = c.State,
                        PostalCode = c.PostalCode,
                        City = c.City,
                        Country = CommonService.getCountryNameFromCode(c.Country),
                        CountryPhoneCode = CommonService.getPhoneCodeFromCountry(c.Country),
                        PhoneNumber = c.Phone,
                        EmailAddress = c.Email,
                        Website = c.Website,
                        PartnerLogoUrl = c.LogoUrl
                    };
                    return info;
                }

            }
            return null;
        }

        public bool updatePartnerInfo(RegisterAPartnerViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.PartnerInformation.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.LicenseNo = model.LicenseNo;
                    data.ContactPersonName = model.ContactPersonName;
                    data.PartnerType = model.PartnerType;
                    data.Address1 = model.Address1;
                    data.Address2 = model.Address2;
                    data.State = model.State;
                    data.PostalCode = model.PostalCode;
                    data.City = model.City;
                    data.Phone = model.PhoneNumber;
                    data.Website = model.Website;
                    data.LogoUrl = model.PartnerLogoUrl;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        public bool checkExistingPartnerEmail(string email)
        {
            var data = dbContext.PartnerInformation.Where(x => x.Email.ToLower() == email.ToLower());
            if (data.Count() == 0)
            {
                return true;
            }
            return false;
        }

        public bool savePartner(RegisterAPartnerViewModel model)
        {
            if (model != null)
            {
                PartnerInformation data = new PartnerInformation()
                {
                    Name = model.Name,
                    LicenseNo = model.LicenseNo,
                    ContactPersonName = model.ContactPersonName,
                    PartnerType = model.PartnerType,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    City = model.City,
                    Country = model.Country,
                    Phone = model.PhoneNumber,
                    Email = model.EmailAddress,
                    Website = model.Website,
                    LogoUrl = model.PartnerLogoUrl,
                    IsActive = true,
                    IsDeleted = false
                };
                dbContext.PartnerInformation.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }


        public bool activateDeactivatePartner(int id)
        {
            if (id != 0)
            {
                var data = dbContext.PartnerInformation.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    if (data.IsActive == true)
                    {
                        data.IsActive = false;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return true;
                    }
                    else if (data.IsActive == false)
                    {
                        data.IsActive = true;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return true;
                    }
                }

            }
            return false;
        }

        public bool deletePartner(int id)
        {
            if (id != 0)
            {
                var data = dbContext.PartnerInformation.Where(x => x.Id == id).FirstOrDefault();
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