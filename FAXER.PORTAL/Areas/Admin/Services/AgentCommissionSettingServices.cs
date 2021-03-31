using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentCommissionSettingServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<AgentCommissionSettingViewModel> getAgentCommissionList(string Country = "", string City = "", int Agent = 0)
        {
            var data = dbContext.AgentCommission.ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City).ToList();
            }
            if (Agent > 0)
            {
                data = data.Where(x => x.AgentId == Agent).ToList();
            }

            var result = (from c in data
                          select new AgentCommissionSettingViewModel()
                          {
                              Id = c.Id,
                              SendingCommission = c.SendingRate,
                              ReceivingCommission = c.ReceivingRate,
                              Country = CommonService.getCountryNameFromCode(c.Country),
                              Code = c.Country,
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              TransferSevice = c.TransferSevice,
                              CommissionType = c.CommissionType,
                              AccountNo = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.AccountNo).FirstOrDefault()
                          }).ToList();
            return result;
        }

        public AgentCommissionSettingViewModel getEditInfo(int AgentId, int TransferSevice, int CommissionType)
        {
            if (AgentId != 0 && CommissionType != 0 && TransferSevice != 0)
            {
                var data = dbContext.AgentCommission.Where(x => x.AgentId == AgentId && x.CommissionType == (CommissionType)CommissionType && x.TransferSevice == (TransferService)TransferSevice).FirstOrDefault();
                if (data != null)
                {
                    AgentCommissionSettingViewModel result = new AgentCommissionSettingViewModel()
                    {
                        Id = data.Id,
                        Country = data.Country,
                        SendingCommission = data.SendingRate,
                        ReceivingCommission = data.ReceivingRate
                    };
                    return result;
                }
            }
            return null;
        }
        public void getAgentInfo(int TransferService, int CommissionType, ref AgentCommissionSettingViewModel vm)
        {
            if (CommissionType != 0 && TransferService != 0)
            {
                var data = dbContext.AgentCommission.Where(x => x.CommissionType == (CommissionType)CommissionType && x.TransferSevice == (TransferService)TransferService).FirstOrDefault();
                if (data != null)
                {
                    AgentCommissionSettingViewModel result = new AgentCommissionSettingViewModel()
                    {
                        Id = data.Id,
                        Country = data.Country,
                        SendingCommission = data.SendingRate,
                        ReceivingCommission = data.ReceivingRate,
                        AgentId = data.AgentId,
                        AccountNo = dbContext.AgentInformation.Where(x => x.Id == data.AgentId).Select(x => x.AccountNo).FirstOrDefault()
                    };
                    vm.SendingCommission = data.SendingRate;
                    vm.ReceivingCommission = data.ReceivingRate;


                }

            }

        }
        public bool DeleteAgentCommission(int id)
        {
            if (id != 0)
            {
                var data = dbContext.AgentCommission.Find(id);
                if (data != null)
                {
                    data.SendingRate = 0;
                    data.ReceivingRate = 0;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool SaveAgentCommission(AgentCommissionSettingViewModel vm)
        {
            var data = dbContext.AgentCommission.Where(x => x.AgentId == vm.AgentId && x.CommissionType == vm.CommissionType && x.TransferSevice == vm.TransferSevice).FirstOrDefault();
            if (data == null)
            {
                AgentCommission model = new AgentCommission()
                {
                    Country = vm.Country,
                    SendingRate = vm.SendingCommission,
                    ReceivingRate = vm.ReceivingCommission,
                    AgentId = vm.AgentId,
                    CommissionType = vm.CommissionType,
                    CommissionDueDate = vm.CommissionDueDate,
                    TransferSevice = vm.TransferSevice,
                    City = vm.City

                };
                dbContext.AgentCommission.Add(model);
                dbContext.SaveChanges();
                return true;
            }
            else if (data != null)
            {
                data.SendingRate = vm.SendingCommission;
                data.ReceivingRate = vm.ReceivingCommission;
                data.CommissionType = vm.CommissionType;
                data.CommissionDueDate = vm.CommissionDueDate;
                data.TransferSevice = vm.TransferSevice;

                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public AgentCommissionSettingViewModel GetAgentCommissionDataById(int id)
        {
            var AgentCommission = dbContext.AgentCommission.Where(x => x.Id == id).FirstOrDefault();
            var vm = new AgentCommissionSettingViewModel()
            {
                AgentId = AgentCommission.AgentId,
                Country = AgentCommission.Country,
                City = AgentCommission.City,
                AccountNo = dbContext.AgentInformation.Where(x => x.Id == AgentCommission.AgentId).Select(x => x.AccountNo).FirstOrDefault(),
                CommissionType = AgentCommission.CommissionType,
                TransferSevice = AgentCommission.TransferSevice,
                CommissionDueDate = AgentCommission.CommissionDueDate,
                ReceivingCommission = AgentCommission.ReceivingRate,
                SendingCommission = AgentCommission.ReceivingRate

            };
            return vm;
        }
        public bool SaveAgentCommissionHistory(AgentCommissionSettingViewModel vm)
        {
            AgentCommissionHisotry model = new AgentCommissionHisotry()
            {
                Country = vm.Country,
                SendingRate = vm.SendingCommission,
                ReceivingRate = vm.ReceivingCommission,
                AgentId = vm.AgentId,
                CommissionType = vm.CommissionType,
                CommissionDueDate = vm.CommissionDueDate,
                TransferSevice = vm.TransferSevice,
                CreatedDate = DateTime.Now,

            };
            dbContext.AgentCommissionHisotry.Add(model);
            dbContext.SaveChanges();
            return true;

        }
        public List<AgentCommissionHisotryViewModel> getAgentCommissionHistoryOnTransferFeeList()
        {
            var result = (from c in dbContext.AgentCommissionHisotry.Where(x => x.CommissionType == CommissionType.CommissionOnFee).ToList()
                          select new AgentCommissionHisotryViewModel()
                          {
                              Id = c.Id,
                              SendingRate = c.SendingRate,
                              ReceivingRate = c.ReceivingRate,
                              Country = CommonService.getCountryNameFromCode(c.Country),
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              TransferSevice = c.TransferSevice,
                              CreatedDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;
        }

        public List<AgentCommissionHisotryViewModel> getAgentCommissionHistoryOnAmount()
        {
            var result = (from c in dbContext.AgentCommissionHisotry.Where(x => x.CommissionType == CommissionType.CommissionOnAmount).ToList()
                          select new AgentCommissionHisotryViewModel()
                          {
                              Id = c.Id,
                              SendingRate = c.SendingRate,
                              ReceivingRate = c.ReceivingRate,
                              Country = CommonService.getCountryNameFromCode(c.Country),
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              TransferSevice = c.TransferSevice,
                              CreatedDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;
        }
        public List<AgentCommissionHisotryViewModel> getAgentCommissionHistoryOnFlatFee()
        {
            var result = (from c in dbContext.AgentCommissionHisotry.Where(x => x.CommissionType == CommissionType.FlatFee).ToList()
                          select new AgentCommissionHisotryViewModel()
                          {
                              Id = c.Id,
                              SendingRate = c.SendingRate,
                              ReceivingRate = c.ReceivingRate,
                              Country = CommonService.getCountryNameFromCode(c.Country),
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              TransferSevice = c.TransferSevice,
                              CreatedDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;
        }
        public List<AgentCommissionHisotryViewModel> getAgentCommissionHistory(string Country, string City, int TransferService,
            int Agent, int CommissionType, string YearMonth)
        {
            var data = dbContext.AgentCommissionHisotry.ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            //if (!string.IsNullOrEmpty(City))
            //{
                
            //    data = data.Where(x => x.City == City).ToList();
            //}
            if (TransferService > 0)
            {
                data = data.Where(x => x.TransferSevice ==(TransferService)TransferService).ToList();
            }
            if (Agent > 0)
            {
                data = data.Where(x => x.AgentId == Agent).ToList();
            }
            if (CommissionType > 0)
            {
                data = data.Where(x => x.CommissionType == (CommissionType)CommissionType).ToList();
            }
            if (!string.IsNullOrEmpty(YearMonth))
            {
                var Date = YearMonth.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                data = data.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();

            }
            var result = (from c in data
                          select new AgentCommissionHisotryViewModel()
                          {
                              Id = c.Id,
                              SendingRate = c.SendingRate,
                              ReceivingRate = c.ReceivingRate,
                              Country = CommonService.getCountryNameFromCode(c.Country),
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              TransferSevice = c.TransferSevice,
                              CreatedDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;
        }

    }


}