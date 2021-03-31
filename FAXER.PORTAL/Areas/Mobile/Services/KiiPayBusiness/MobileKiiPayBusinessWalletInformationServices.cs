using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness
{
    public class MobileKiiPayBusinessWalletInformationServices
    {

        DB.FAXEREntities dbContext = null;
        public MobileKiiPayBusinessWalletInformationServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessWalletInformation Add(DB.KiiPayBusinessWalletInformation model)
        {

            dbContext.KiiPayBusinessWalletInformation.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public ServiceResult<IQueryable<KiiPayBusinessWalletInformation>> List()
        {
            return new ServiceResult<IQueryable<KiiPayBusinessWalletInformation>>
            {
                Data = dbContext.KiiPayBusinessWalletInformation,
                Status = ResultStatus.OK
            };
        }
        public Suppliers SaveSuppliers(Suppliers suppliers)
        {
            dbContext.Suppliers.Add(suppliers);
            dbContext.SaveChanges();
            return suppliers;
        }
    }
}