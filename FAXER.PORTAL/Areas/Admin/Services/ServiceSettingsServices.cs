using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ServiceSettingsServices
    {
        DB.FAXEREntities db = null;
        public ServiceSettingsServices()
        {
            db = new DB.FAXEREntities();
        }
        public List<DropDownViewModel> GetServiceType()
        {
            var result = new List<DropDownViewModel>();
            result.Add(new DropDownViewModel()
            {
                Id = 2,
                Name = "Bank Deposit"
            }); result.Add(new DropDownViewModel()
            {
                Id = 3,
                Name = "Cash Pickup"
            }); result.Add(new DropDownViewModel()
            {
                Id = 4,
                Name = "Other Wallet"
            }); result.Add(new DropDownViewModel()
            {
                Id = 1,
                Name = "KiiPay Wallet"
            });
            result.Add(new DropDownViewModel()
            {
                Id = 5,
                Name = "Bill Payment"
            });
            return result;

        }
        public bool AddServiceSetting(ServiceSettingViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            TransferServiceMaster transferServiceMaster = new TransferServiceMaster()
            {
                ReceivingCountry = vm.Master.ReceivingCountry,
                SendingCountry = vm.Master.SendingCountry,
                ReceivingCurrency = vm.Master.ReceivingCurrency,
                CreatedById = staffId,
                CreatedDateTime = DateTime.Now,
                SendingCurrency = vm.Master.SendingCurrrency
            };

            var transferServices = db.TransferServiceMaster.Add(transferServiceMaster);
            db.SaveChanges();

            List<TransferServiceDetails> details = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                    select new TransferServiceDetails()
                                                    {
                                                        TransferMasterId = transferServices.Id,
                                                        ServiceType = c.ServiceType

                                                    }).ToList();

            TransferServiceDetails(details);
            vm.Master.Id = transferServices.Id;
            AddBankAcceptingCurrency(vm);

            return true;
        }

        private void AddBankAcceptingCurrency(ServiceSettingViewModel vm)
        {
            BankAccepitngCurrencyServices _bankAcceptingCurreny = new BankAccepitngCurrencyServices();
            if (vm.BankDetails != null)
            {
                foreach (var item in vm.BankDetails.Where(x => x.IsChecked == true))
                {
                    BankAcceptingCurrency model = new BankAcceptingCurrency()
                    {
                        BankId = item.Id ?? 0,
                        CreateDate = DateTime.Now,
                        Currency = vm.Master.ReceivingCurrency,
                        ServiceSettingId = vm.Master.Id
                    };
                    _bankAcceptingCurreny.Add(model);
                }
            }

        }

        internal List<BankViewModel> GetBanks(string receivingCountry, string receivngCurrecny)
        {
            if (!string.IsNullOrEmpty(receivingCountry) || !string.IsNullOrEmpty(receivngCurrecny))
            {
                BankServices _bankServices = new BankServices();
                string currencyOfCountryCode = Common.Common.GetCountryCurrency(receivingCountry);
                if (receivngCurrecny == currencyOfCountryCode)
                {
                    return new List<BankViewModel>();
                }
                var banks = _bankServices.GetBankList(receivingCountry);
                return banks;
            }
            return new List<BankViewModel>();
        }

        public bool AddServiceSettingByCurrency(ServiceSettingViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            TransferServiceByCurrencyMaster transferServiceMaster = new TransferServiceByCurrencyMaster()
            {
                ReceivingCurrency = vm.Master.ReceivingCurrency,
                SendingCurrency = vm.Master.SendingCurrrency,
                CreatedById = staffId,
                CreatedDateTime = DateTime.Now,
                SendingCountry = "All",
                ReceivingCountry = "All"
            };

            db.TransferServiceByCurrencyMaster.Add(transferServiceMaster);
            db.SaveChanges();
            vm.Master.CreatedById = staffId;
            vm.Master.Id = transferServiceMaster.Id;
            List<TransferServiceByCurrencyDetails> details = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                              select new TransferServiceByCurrencyDetails()
                                                              {
                                                                  TransferServiceByCurrencyMasterId = vm.Master.Id,
                                                                  ServiceType = c.ServiceType

                                                              }).ToList();
            if (details.Count() > 0)
            {
                TransferServiceByCurrencyDetails(details);
                AddTransferServicesSettting(vm);
            }
            return true;
        }

        private void AddTransferServicesSettting(ServiceSettingViewModel vm)
        {
            var sendingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.Master.SendingCurrrency, vm.Master.SendingCountry);
            var ReceivingCountry = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.Master.ReceivingCurrency, vm.Master.ReceivingCountry);

            foreach (var sendingCountry in sendingCountries)
            {
                foreach (var receivingCountry in ReceivingCountry)
                {
                    TransferServiceMaster master = new TransferServiceMaster()
                    {
                        CreatedById = vm.Master.CreatedById,
                        CreatedDateTime = DateTime.Now,
                        TransferServicesByCurrencyId = vm.Master.Id,
                        ReceivingCountry = receivingCountry,
                        SendingCountry = sendingCountry,
                        ReceivingCurrency = vm.Master.ReceivingCurrency,
                        SendingCurrency = vm.Master.SendingCurrrency
                    };

                    db.TransferServiceMaster.Add(master);
                    db.SaveChanges();

                    List<TransferServiceDetails> transferDetails = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                                    select new TransferServiceDetails()
                                                                    {
                                                                        TransferMasterId = master.Id,
                                                                        ServiceType = c.ServiceType
                                                                    }).ToList();
                    TransferServiceDetails(transferDetails);
                }
            }
        }

        public List<ServiceSettingViewModel> GetCurrentSetting(string SendingCountry = "", string ReceivingCountry = "")
        {
            List<ServiceSettingViewModel> data = new List<ServiceSettingViewModel>();

            IQueryable<TransferServiceMaster> transferServiceMaster = db.TransferServiceMaster;
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                transferServiceMaster = transferServiceMaster.Where(x => x.SendingCountry == SendingCountry);
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                transferServiceMaster = transferServiceMaster.Where(x => x.ReceivingCountry == ReceivingCountry);
            }
            List<TransferServiceMasterViewModel> serviceMasterData = (from c in transferServiceMaster
                                                                      join sendingCountry in db.Country on c.SendingCountry equals sendingCountry.CountryCode into SC
                                                                      from sendingCountry in SC.DefaultIfEmpty()
                                                                      join receivingCountry in db.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                                                                      select new TransferServiceMasterViewModel()
                                                                      {
                                                                          Id = c.Id,
                                                                          ReceivingCountry = c.ReceivingCountry,
                                                                          SendingCountry = c.SendingCountry,
                                                                          SendingCountryName = sendingCountry == null ? "All" : sendingCountry.CountryName,
                                                                          ReceivingCountryName = receivingCountry.CountryName,
                                                                          ReceivingCurrency = c.ReceivingCurrency,
                                                                          SendingCurrrency = c.SendingCurrency
                                                                      }).ToList();



            foreach (var item in serviceMasterData)
            {

                List<TransferServiceDetailsViewModel> detailList = GetTransferServiceDetails(item.Id);

                data.Add(new ServiceSettingViewModel()
                {
                    Details = detailList,
                    Master = item
                });
            }
            return data;
        }
        public List<ServiceSettingViewModel> GetCurrentSettingByCurrency()
        {
            List<ServiceSettingViewModel> data = new List<ServiceSettingViewModel>();

            List<TransferServiceMasterViewModel> serviceMasterData = (from c in db.TransferServiceByCurrencyMaster
                                                                      select new TransferServiceMasterViewModel()
                                                                      {
                                                                          Id = c.Id,
                                                                          ReceivingCurrency = c.ReceivingCurrency,
                                                                          SendingCurrrency = c.SendingCurrency
                                                                      }).ToList();



            foreach (var item in serviceMasterData)
            {

                List<TransferServiceDetailsViewModel> detailList = GetTransferServiceByCurrencyDetails(item.Id);

                data.Add(new ServiceSettingViewModel()
                {
                    Details = detailList,
                    Master = item
                });
            }
            return data;
        }
        public List<TransferServiceDetailsViewModel> GetTransferServiceByCurrencyDetails(int transferServiceId)
        {
            List<TransferServiceDetailsViewModel> detailList = (from c in db.TransferServiceByCurrencyDetails
                                                                select new TransferServiceDetailsViewModel()
                                                                {
                                                                    Id = c.Id,
                                                                    ServiceType = c.ServiceType,
                                                                    TransferServiceMasterId = c.TransferServiceByCurrencyMasterId

                                                                }).ToList();


            return detailList;
        }
        public List<TransferServiceDetailsViewModel> GetTransferServiceDetails(int transferServiceId)
        {
            List<TransferServiceDetailsViewModel> detailList = (from c in db.TransferServiceDetails
                                                                select new TransferServiceDetailsViewModel()
                                                                {
                                                                    Id = c.Id,
                                                                    ServiceType = c.ServiceType,
                                                                    TransferServiceMasterId = c.TransferMasterId

                                                                }).ToList();


            return detailList;
        }
        public List<BankViewModel> GetBankDetails(int transferServiceId, string countryCode)
        {
            List<BankViewModel> banks = (from bank in db.Bank.Where(x => x.CountryCode == countryCode)
                                         join c in db.BankAcceptingCurrency.Where(x => x.ServiceSettingId == transferServiceId) on bank.Id equals c.BankId into bk
                                         from c in bk.DefaultIfEmpty()
                                         select new BankViewModel()
                                         {
                                             Id = bank.Id,
                                             Address = bank.Address,
                                             Code = bank.Code,
                                             CountryCode = bank.CountryCode,
                                             Name = bank.Name,
                                             IsChecked = c == null ? false : true
                                         }).ToList();


            return banks;
        }

        public bool TransferServiceDetails(List<TransferServiceDetails> details)
        {
            db.TransferServiceDetails.AddRange(details);
            db.SaveChanges();
            return true;
        }
        public bool TransferServiceByCurrencyDetails(List<TransferServiceByCurrencyDetails> details)
        {
            db.TransferServiceByCurrencyDetails.AddRange(details);
            db.SaveChanges();
            return true;
        }

        public bool Remove(int id)
        {
            var master = db.TransferServiceMaster.Where(x => x.Id == id).FirstOrDefault();
            db.TransferServiceMaster.Remove(master);
            db.SaveChanges();
            return true;
        }
        public bool RemoveByCurrency(int id)
        {
            var master = db.TransferServiceByCurrencyMaster.Where(x => x.Id == id).FirstOrDefault();
            var masteremove = db.TransferServiceMaster.Where(x => x.TransferServicesByCurrencyId == master.Id).ToList();
            foreach (var item in masteremove)
            {
                var Removedetails = db.TransferServiceDetails.Where(x => x.TransferMasterId == item.Id).ToList();
                db.TransferServiceDetails.RemoveRange(Removedetails);
                db.SaveChanges();
            }
            db.TransferServiceMaster.RemoveRange(masteremove);
            db.TransferServiceByCurrencyMaster.Remove(master);
            db.SaveChanges();
            return true;
        }


        internal void UpdateServiceSetting(ServiceSettingViewModel vm)
        {
            var master = db.TransferServiceMaster.Where(x => x.Id == vm.Master.Id).FirstOrDefault();
            master.Id = vm.Master.Id;
            master.SendingCurrency = vm.Master.SendingCurrrency;
            master.SendingCountry = vm.Master.SendingCountry;
            master.ReceivingCountry = vm.Master.ReceivingCountry;
            master.ReceivingCurrency = vm.Master.ReceivingCurrency;
            master.CreatedById = vm.Master.CreatedById;
            master.CreatedDateTime = master.CreatedDateTime;

            db.Entry<TransferServiceMaster>(master).State = EntityState.Modified;
            db.SaveChanges();

            var details = db.TransferServiceDetails.Where(x => x.TransferMasterId == master.Id).ToList();
            db.TransferServiceDetails.RemoveRange(details);
            db.SaveChanges();

            List<TransferServiceDetails> transferServiceDetails = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                                   select new TransferServiceDetails()
                                                                   {
                                                                       TransferMasterId = master.Id,
                                                                       ServiceType = c.ServiceType

                                                                   }).ToList();
            TransferServiceDetails(transferServiceDetails);
            UpdateBankAcceptingCurrency(vm);

        }
        private void UpdateBankAcceptingCurrency(ServiceSettingViewModel vm)
        {
            BankAccepitngCurrencyServices _bankAcceptingCurreny = new BankAccepitngCurrencyServices();
            var bankAcceptingCurrency = _bankAcceptingCurreny.BankAcceptingCurrencies().Where(x => x.ServiceSettingId == vm.Master.Id);
            if (bankAcceptingCurrency != null)
            {
                _bankAcceptingCurreny.RemoveRange(bankAcceptingCurrency);
            }
            AddBankAcceptingCurrency(vm);
        }

        internal void UpdateServiceSettingByCurrency(ServiceSettingViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            var serviceSetting = db.TransferServiceMaster.Where(x => x.Id == vm.Master.Id).FirstOrDefault();
            var master = db.TransferServiceByCurrencyMaster.Where(x => x.Id == vm.Master.Id).FirstOrDefault();
            //var master = db.TransferServiceByCurrencyMaster.Where(x => x.Id == serviceSetting.TransferServicesByCurrencyId).FirstOrDefault();
            if (master != null)
            {
                master.SendingCurrency = vm.Master.SendingCurrrency;
                master.ReceivingCurrency = vm.Master.ReceivingCurrency;
                master.SendingCountry = "All";
                master.ReceivingCountry = "All";
                master.CreatedById = staffId;
                master.CreatedDateTime = master.CreatedDateTime;

                db.Entry(master).State = EntityState.Modified;
                db.SaveChanges();

                var details = db.TransferServiceByCurrencyDetails.Where(x => x.TransferServiceByCurrencyMasterId == master.Id).ToList();
                db.TransferServiceByCurrencyDetails.RemoveRange(details);
                db.SaveChanges();

                List<TransferServiceByCurrencyDetails> transferServiceDetails = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                                                 select new TransferServiceByCurrencyDetails()
                                                                                 {
                                                                                     TransferServiceByCurrencyMasterId = master.Id,
                                                                                     ServiceType = c.ServiceType
                                                                                 }).ToList();
                if (transferServiceDetails.Count() > 0)
                {
                    TransferServiceByCurrencyDetails(transferServiceDetails);
                }
                var masteremove = db.TransferServiceMaster.Where(x => x.TransferServicesByCurrencyId == master.Id).ToList();
                foreach (var item in masteremove)
                {
                    var Removedetails = db.TransferServiceDetails.Where(x => x.TransferMasterId == item.Id).ToList();
                    db.TransferServiceDetails.RemoveRange(Removedetails);
                    db.SaveChanges();
                }
                db.TransferServiceMaster.RemoveRange(masteremove);
                db.SaveChanges();
                AddTransferServicesSettting(vm);

            }
            else
            {

                Remove(vm.Master.Id);
                AddServiceSettingByCurrency(vm);
            }

        }

        internal List<TransferServiceDetailsViewModel> GetTransferServiceDetails(string sendingCountry, string receivingCountry)
        {
            List<TransferServiceDetailsViewModel> detailList = (from c in db.TransferServiceDetails.ToList()
                                                                join d in db.TransferServiceMaster.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry).ToList() on c.TransferMasterId equals d.Id
                                                                select new TransferServiceDetailsViewModel()
                                                                {
                                                                    Id = c.Id,
                                                                    ServiceType = c.ServiceType,
                                                                    TransferServiceMasterId = c.TransferMasterId

                                                                }).ToList();


            return detailList;
        }
        internal List<TransferServiceDetailsViewModel> GetTransferServiceDetailsByCurrency(string sendingCountry, string receivingCountry, string receivingCurrency)
        {
            var transfermaster = (from c in db.TransferServiceMaster.Where(x => x.SendingCountry == sendingCountry &&
                                                                      x.ReceivingCountry == receivingCountry).ToList()
                                  select new TransferServiceMaster()
                                  {
                                      Id = c.Id,
                                      SendingCountry = c.SendingCountry,
                                      ReceivingCountry = c.ReceivingCountry,
                                      ReceivingCurrency = c.ReceivingCurrency == null ? Common.Common.GetCountryCurrency(c.ReceivingCountry) : c.ReceivingCurrency,
                                  });


            List<TransferServiceDetailsViewModel> detailList = (from c in db.TransferServiceDetails.ToList()
                                                                join d in transfermaster.Where(x => x.ReceivingCurrency == receivingCurrency) on c.TransferMasterId equals d.Id
                                                                select new TransferServiceDetailsViewModel()
                                                                {
                                                                    Id = c.Id,
                                                                    ServiceType = c.ServiceType,
                                                                    TransferServiceMasterId = c.TransferMasterId
                                                                }).ToList();


            return detailList;
        }


        public ServiceResult<IQueryable<TransferServiceDetails>> GetTransferServiceDetailList()
        {

            return new ServiceResult<IQueryable<TransferServiceDetails>>()
            {
                Data = db.TransferServiceDetails,
                Status = ResultStatus.OK
            };

        }
        public ServiceResult<IQueryable<TransferServiceByCurrencyDetails>> GetTransferServiceByCurrencyDetailList()
        {

            return new ServiceResult<IQueryable<TransferServiceByCurrencyDetails>>()
            {
                Data = db.TransferServiceByCurrencyDetails,
                Status = ResultStatus.OK
            };

        }


    }
}
