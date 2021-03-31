using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class PayoutProviderServices
    {
        DB.FAXEREntities dbContext = null;
        public PayoutProviderServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<PayoutProvider> Master()
        {
            var data = dbContext.PayoutProvider.ToList();
            return data;
        }
        public List<PayoutProviderDetails> Details()
        {
            var data = dbContext.PayoutProviderDetails.ToList();
            return data;
        }

        //public List<PayoutProviderViewModel> PayoutProviderList(string sendingCountry, string receivingCountry, int tranferMethod)
        //{
        //    var data = List();
        //    if (!string.IsNullOrEmpty(sendingCountry))
        //    {
        //        data = data.Where(x => x.SendingCountry == sendingCountry).ToList();

        //    }
        //    if (!string.IsNullOrEmpty(receivingCountry))
        //    {

        //        data = data.Where(x => x.ReceivingCountry == receivingCountry).ToList();
        //    }
        //    if (tranferMethod != 0)
        //    {
        //        data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)tranferMethod).ToList();
        //    }
        //    var result = (from c in data
        //                  select new PayoutProviderViewModel()
        //                  {
        //                      Id = c.Id,
        //                      TransferMethod = c.TransferMethod,
        //                      Code = c.Code,
        //                      Name = c.Name,
        //                      ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
        //                      SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
        //                      TransferMethodName = c.TransferMethod.ToString(),
        //                      BranchName = c.BranchName

        //                  }).ToList();

        //    return result;
        //}


        public List<PayoutProviderViewModel> PayoutProviderList(string sendingCountry, string receivingCountry, int tranferMethod)
        {
            var data = Master();
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                data = data.Where(x => x.SendingCountry == sendingCountry).ToList();

            }
            if (!string.IsNullOrEmpty(receivingCountry))
            {

                data = data.Where(x => x.ReceivingCountry == receivingCountry).ToList();
            }
            if (tranferMethod != 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)tranferMethod).ToList();
            }

            List<PayoutProviderViewModel> PayoutProvider = new List<PayoutProviderViewModel>();

            List<PayoutProviderMasterViewModel> MasterData = (from c in data
                                                              select new PayoutProviderMasterViewModel()
                                                              {
                                                                  Id = c.Id,
                                                                  ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                                                  SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                                                  TransferMethod = c.TransferMethod,
                                                                  TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                                                                  ReceivingCountryFlag = c.ReceivingCountry.ToLower(),
                                                                  SendingCountryFlag = c.SendingCountry.ToLower(),
                                                                  Name = c.Name,
                                                                  Code = c.Code

                                                              }).ToList();



            foreach (var item in MasterData)
            {

                List<PayoutProviderDetailsViewModel> detailList = GetPayoutProvideDetails(item.Id);

                PayoutProvider.Add(new PayoutProviderViewModel()
                {
                    Details = detailList,
                    Master = item
                });
            }
            return PayoutProvider;
        }

        public PayoutProviderMasterViewModel GetPayoutProvideeMasterDetails(int PayoutProviderId)
        {
            var data = Master().Where(x => x.Id == PayoutProviderId);
            var master = (from c in data
                          select new PayoutProviderMasterViewModel()
                          {
                              Id = c.Id,
                              Code = c.Code,
                              Name = c.Name,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountry = c.SendingCountry,
                              TransferMethod = c.TransferMethod
                          }).FirstOrDefault();


            return master;
        }

        public List<PayoutProviderDetailsViewModel> GetPayoutProvideDetails(int? PayoutProviderId)
        {
            var data = Details().Where(x => x.PayoutProviderId == PayoutProviderId);
            List<PayoutProviderDetailsViewModel> detailList = (from c in data
                                                               select new PayoutProviderDetailsViewModel()
                                                               {
                                                                   Id = c.Id,
                                                                   BranchName = c.BranchName,
                                                                   Code = c.Code,
                                                                   Name = c.Name,
                                                                   PayoutProviderId = c.PayoutProviderId
                                                               }).ToList();


            return detailList;
        }

        public void Delete(int id)
        {
            var data = Master().Where(x => x.Id == id).FirstOrDefault();
            dbContext.PayoutProvider.Remove(data);
            dbContext.SaveChanges();
        }
        public void DeleteDetails(int DetailId)
        {
            var data = Details().Where(x => x.Id == DetailId).FirstOrDefault();
            dbContext.PayoutProviderDetails.Remove(data);
            dbContext.SaveChanges();
        }


        public PayoutProviderViewModel PayoutProvider(int id)
        {
            var data = Master().Where(x => x.Id == id);



            PayoutProviderMasterViewModel MasterData = (from c in data
                                                        select new PayoutProviderMasterViewModel()
                                                        {
                                                            Id = c.Id,
                                                            ReceivingCountry = c.ReceivingCountry,
                                                            SendingCountry = c.SendingCountry,
                                                            TransferMethod = c.TransferMethod,
                                                            TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod)
                                                        }).FirstOrDefault();



            List<PayoutProviderDetailsViewModel> detailList = GetPayoutProvideDetails(MasterData.Id);

            PayoutProviderViewModel PayoutProvider = new PayoutProviderViewModel()
            {
                Details = detailList,
                Master = MasterData

            };



            return PayoutProvider;
        }

        public void AddPayoutProviders(PayoutProviderViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

            PayoutProvider model = new PayoutProvider()
            {
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                ReceivingCountry = vm.Master.ReceivingCountry,
                SendingCountry = vm.Master.SendingCountry,
                TransferMethod = vm.Master.TransferMethod,
                Code = vm.Master.Code,
                Name = vm.Master.Name
            };
            dbContext.PayoutProvider.Add(model);
            dbContext.SaveChanges();

            if (vm.Master.TransferMethod == TransactionTransferMethod.BankDeposit)
            {
                Bank bank = new Bank()
                {
                    Code = model.Code,
                    Name = model.Name,
                    CountryCode = vm.Master.ReceivingCountry,
                    PayoutProviderId = model.Id,
                    CreatedBy = StaffId,
                    CreatedDate = DateTime.Now

                };
                dbContext.Bank.Add(bank);
                dbContext.SaveChanges();
                foreach (var item in vm.Details)
                {
                    BankBranch bankBranch = new BankBranch()
                    {
                        BankId = bank.Id,
                        BranchCode = item.Code,
                        BranchName = item.Name

                    };
                    dbContext.BankBranch.Add(bankBranch);

                }
            }
            if (vm.Master.TransferMethod == TransactionTransferMethod.OtherWallet)
            {
                MobileWalletOperator mobileWalletOperator = new MobileWalletOperator()
                {
                    Code = model.Code,
                    Name = model.Name,
                    Country = vm.Master.ReceivingCountry,
                    PayoutProviderId = model.Id,
                    CreatedBy = StaffId,
                    CreatedDate = DateTime.Now
                };
                dbContext.MobileWalletOperator.Add(mobileWalletOperator);

                foreach (var item in vm.Details)
                {
                    // Yesma Branch ko kura chaina ahile nagarda pani huncha 
                }
            }
            dbContext.SaveChanges();

            List<PayoutProviderDetails> details = (from c in vm.Details
                                                   select new PayoutProviderDetails()
                                                   {
                                                       PayoutProviderId = model.Id,
                                                       BranchName = c.BranchName,
                                                       Code = c.Code,
                                                       Name = c.Name
                                                   }).ToList();
            AddDetails(details);

        }

        public void UpdatePayoutProviders(PayoutProviderViewModel vm)
        {
            var data = Master().Where(x => x.Id == vm.Master.Id).FirstOrDefault();

            data.ReceivingCountry = vm.Master.ReceivingCountry;
            data.SendingCountry = vm.Master.SendingCountry;
            data.TransferMethod = vm.Master.TransferMethod;


            if (vm.Master.TransferMethod == TransactionTransferMethod.BankDeposit)
            {

                var bank = dbContext.Bank.Where(x => x.PayoutProviderId == data.Id).FirstOrDefault();

                bank.Code = vm.Master.Code;
                bank.Name = vm.Master.Name;
                bank.CountryCode = vm.Master.ReceivingCountry;
                bank.PayoutProviderId = data.Id;
                bank.CreatedBy = bank.CreatedBy;
                bank.CreatedDate = bank.CreatedDate;

                dbContext.Entry(bank).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                var BankBranch = dbContext.BankBranch.Where(x => x.BankId == bank.Id).ToList();
                dbContext.BankBranch.RemoveRange(BankBranch);
                dbContext.SaveChanges();

                foreach (var item in vm.Details)
                {
                    BankBranch bankBranch = new BankBranch()
                    {
                        BankId = bank.Id,
                        BranchCode = item.Code,
                        BranchName = item.Name

                    };
                    dbContext.BankBranch.Add(bankBranch);

                }

            }
            if (vm.Master.TransferMethod == TransactionTransferMethod.OtherWallet)
            {

                var mobileWalletOperator = dbContext.MobileWalletOperator.Where(x => x.PayoutProviderId == data.Id).FirstOrDefault();
                mobileWalletOperator = new MobileWalletOperator()
                {
                    Id = mobileWalletOperator.Id,
                    Code = vm.Master.Code,
                    Name = vm.Master.Name,
                    Country = vm.Master.ReceivingCountry,
                    PayoutProviderId = data.Id,
                    CreatedBy = mobileWalletOperator.CreatedBy,
                    CreatedDate = mobileWalletOperator.CreatedDate
                };

                dbContext.Entry(mobileWalletOperator).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }

            dbContext.Entry<PayoutProvider>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


            var details = dbContext.PayoutProviderDetails.Where(x => x.PayoutProviderId == data.Id).ToList();
            dbContext.PayoutProviderDetails.RemoveRange(details);
            dbContext.SaveChanges();

            List<PayoutProviderDetails> PayoutProviderDetails = (from c in vm.Details
                                                                 select new PayoutProviderDetails()
                                                                 {
                                                                     PayoutProviderId = data.Id,
                                                                     BranchName = c.BranchName,
                                                                     Code = c.Code,
                                                                     Name = c.Name
                                                                 }).ToList();
            AddDetails(PayoutProviderDetails);

        }


        public void AddDetails(List<PayoutProviderDetails> details)
        {
            dbContext.PayoutProviderDetails.AddRange(details);
            dbContext.SaveChanges();
        }

    }
}
