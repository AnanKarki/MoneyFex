using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BlackListedReceiverServcies
    {
        DB.FAXEREntities dbContext = null;

        public BlackListedReceiverServcies()
        {
            dbContext = new DB.FAXEREntities();
        }
        internal void Update(ReceiverDetailsInfoViewModel vm)
        {
            var model = dbContext.Recipients.Where(x => x.Id == vm.Id).FirstOrDefault();
            model.BankId = vm.BankId ?? 0;
            model.AccountNo = vm.ReceiverAccountNo;
            model.Country = vm.ReceiverCountry;
            model.MobileNo = vm.ReceiverPhoneNo;
            model.MobileWalletProvider = vm.MobileWalletProvider ?? 0;
            model.ReceiverName = vm.ReceiverName;
            model.Service = vm.Service;
            model.BranchCode = vm.BankCode;
            model.IsBanned = true;
            dbContext.Entry<Recipients>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal void Add(ReceiverDetailsInfoViewModel vm)
        {
            Recipients model = new Recipients()
            {
                BankId = vm.BankId ?? 0,
                AccountNo = vm.ReceiverAccountNo,
                Country = vm.ReceiverCountry,
                MobileNo = vm.ReceiverPhoneNo,
                MobileWalletProvider = vm.MobileWalletProvider ?? 0,
                ReceiverName = vm.ReceiverName,
                Service = vm.Service,
                BranchCode = vm.BankCode,
                IsBanned = true
            };
            dbContext.Recipients.Add(model);
            dbContext.SaveChanges();

            ReceiverServices _receiverServices = new ReceiverServices();
            _receiverServices.AddBlackListedReceiver(model);
        }

        internal void Delete(int id)
        {
            var model = dbContext.Recipients.Where(x => x.Id == id).FirstOrDefault();
            model.IsBanned = false;
            dbContext.Entry(model).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        #region old Design
        //public List<BlackListedReceiverViewModel> GetBanRecevicerList()
        //{
        //    var data = dbContext.BlacklistedReceiver.Where(x => x.IsBlocked == true && x.IsDeleted == false).ToList();
        //    List<BlackListedReceiverViewModel> vm = (from c in data
        //                                             select new BlackListedReceiverViewModel
        //                                             {
        //                                                 Id = c.Id,
        //                                                 ReceiverAccountNo = c.ReceiverAccountNo,
        //                                                 ReceiverCountry = Common.Common.GetCountryName(c.ReceiverCountry),
        //                                                 ReceiverName = c.ReceiverName,
        //                                                 TransferMethod = c.TransferMethod,
        //                                                 TransferMethodName = c.TransferMethod.ToString(),
        //                                                 ReceiverTelephone = c.ReceiverTelephone,
        //                                                 BankNameOrMobileWalletProvider = c.BankNameOrMobileWalletProvider,
        //                                                 BankCode = c.BankCode,
        //                                                 IsDeleted = c.IsDeleted

        //                                             }).ToList();
        //    return vm;
        //}

        //internal void Add(BlackListedReceiverViewModel vm)
        //{
        //    int staffId = Common.StaffSession.LoggedStaff.StaffId;
        //    BlacklistedReceiver model = new BlacklistedReceiver()
        //    {
        //        ReceiverTelephone = vm.ReceiverTelephone,
        //        TransferMethod = vm.TransferMethod,
        //        ReceiverName = vm.ReceiverName,
        //        BankCode = vm.BankCode,
        //        BankNameOrMobileWalletProvider = vm.BankNameOrMobileWalletProvider,
        //        IsBlocked = true,
        //        ReceiverCountry = vm.ReceiverCountry,
        //        ReceiverAccountNo = vm.ReceiverAccountNo,
        //        CareatedDate = DateTime.Now,
        //        CreatedByUserId = staffId,
        //        IsDeleted = false

        //    };
        //    dbContext.BlacklistedReceiver.Add(model);
        //    dbContext.SaveChanges();
        //}

        //internal void Update(BlackListedReceiverViewModel vm)
        //{
        //    var data = dbContext.BlacklistedReceiver.Where(x => x.Id == vm.Id).FirstOrDefault();
        //    data.ReceiverAccountNo = vm.ReceiverAccountNo;
        //    data.ReceiverCountry = vm.ReceiverCountry;
        //    data.ReceiverName = vm.ReceiverName;
        //    data.ReceiverTelephone = vm.ReceiverTelephone;
        //    data.TransferMethod = vm.TransferMethod;
        //    data.BankNameOrMobileWalletProvider = vm.BankNameOrMobileWalletProvider;
        //    data.BankCode = vm.BankCode;

        //    dbContext.Entry<BlacklistedReceiver>(data).State = EntityState.Modified;
        //    dbContext.SaveChanges();

        //}

        //internal void Delete(int id)
        //{
        //    var data = dbContext.BlacklistedReceiver.Where(x => x.Id == id).FirstOrDefault();
        //    data.IsDeleted = true;
        //    dbContext.Entry<BlacklistedReceiver>(data).State = EntityState.Modified;
        //    dbContext.SaveChanges();

        //}
        #endregion

    }
}