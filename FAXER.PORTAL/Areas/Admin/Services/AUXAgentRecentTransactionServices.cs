using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AUXAgentRecentTransactionServices
    {

        FAXEREntities db = null;
        CommonServices _commonServices = null;
        private IQueryable<AgentInformation> agentInformation;
        private IQueryable<AgentLogin> agentLogin;
        private List<RegisteredAUXAgentViewModel> RegisteredAuxAgents;

        public AUXAgentRecentTransactionServices()
        {
            db = new FAXEREntities();
            _commonServices = new CommonServices();
            RegisteredAuxAgents = new List<RegisteredAUXAgentViewModel>();
        }
        public List<AgentInformation> GetAuxAgentInformation()
        {
            var list = db.AgentInformation.Where(x => x.IsAUXAgent == true).ToList();
            return list;
        }

        public List<AgentLogin> GetAgentLogins()
        {
            var list = db.AgentLogin.ToList();
            return list;
        }
        public List<AUXAgentRecentTransactionViewModel> getAuxAgentRecentTransactionList(int AgentID, string sendingCountry,
               string ReceivingCountry, string Date, string Sender, string Status, string Receiver, string Identifier)
        {
            IQueryable<AgentStaffInformation> payingStaff = db.AgentStaffInformation.Where(x => x.Agent.IsAUXAgent == true);
            IQueryable<FaxingNonCardTransaction> cashPickUp = db.FaxingNonCardTransaction;
            IQueryable<TopUpSomeoneElseCardTransaction> kiipay = db.TopUpSomeoneElseCardTransaction;
            IQueryable<MobileMoneyTransfer> otherWallet = db.MobileMoneyTransfer;
            IQueryable<BankAccountDeposit> bank = db.BankAccountDeposit;
            if (AgentID > 0)
            {
                payingStaff = payingStaff.Where(x => x.AgentId == AgentID);
            }

            if (!string.IsNullOrEmpty(sendingCountry))
            {
                cashPickUp = cashPickUp.Where(x => x.SendingCountry == sendingCountry);
                kiipay = kiipay.Where(x => x.SendingCountry == sendingCountry);
                otherWallet = otherWallet.Where(x => x.SendingCountry == sendingCountry);
                bank = bank.Where(x => x.SendingCountry == sendingCountry);
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                cashPickUp = cashPickUp.Where(x => x.ReceivingCountry == ReceivingCountry);
                kiipay = kiipay.Where(x => x.ReceivingCountry == ReceivingCountry);
                otherWallet = otherWallet.Where(x => x.ReceivingCountry == ReceivingCountry);
                bank = bank.Where(x => x.ReceivingCountry == ReceivingCountry);
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);

                cashPickUp = cashPickUp.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(FromDate) &&
                                                     DbFunctions.TruncateTime(x.TransactionDate) <= DbFunctions.TruncateTime(ToDate));

                kiipay = kiipay.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(FromDate) &&
                                                     DbFunctions.TruncateTime(x.TransactionDate) <= DbFunctions.TruncateTime(ToDate));

                otherWallet = otherWallet.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(FromDate) &&
                                                      DbFunctions.TruncateTime(x.TransactionDate) <= DbFunctions.TruncateTime(ToDate));

                bank = bank.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(FromDate) &&
                                                     DbFunctions.TruncateTime(x.TransactionDate) <= DbFunctions.TruncateTime(ToDate));

            }


            if (!string.IsNullOrEmpty(Identifier))
            {
                Identifier = Identifier.Trim();
                cashPickUp = cashPickUp.Where(x => x.ReceiptNumber.ToLower().Contains(Identifier.ToLower()));
                kiipay = kiipay.Where(x => x.ReceiptNumber.ToLower().Contains(Identifier.ToLower()));
                otherWallet = otherWallet.Where(x => x.ReceiptNo.ToLower().Contains(Identifier.ToLower())); ;
                bank = bank.Where(x => x.ReceiptNo.ToLower().Contains(Identifier.ToLower()));
            }

            var CashPickupTransfer = (from c in cashPickUp.ToList()
                                      join d in payingStaff on c.PayingStaffId equals d.Id
                                      select new AUXAgentRecentTransactionViewModel()
                                      {
                                          Id = c.Id,
                                          SendingCountry = _commonServices.getCountryNameFromCode(c.SendingCountry),
                                          SendingCountrycode = c.SendingCountry,
                                          Date = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                          TransactionDate = c.TransactionDate,
                                          AgentAccountNo = _commonServices.GetAgentStaffMFSCode((int)c.PayingStaffId),
                                          AgentName = c.AgentStaffName,
                                          SendingCurrency = _commonServices.getCurrencyCodeFromCountry(c.SendingCountry),
                                          ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                          ReceivingCountryCode = c.ReceivingCountry,
                                          Amount = c.TotalAmount,
                                          Identifier = c.MFCN,
                                          Receiver = c.NonCardReciever.FullName,
                                          Sender = _commonServices.GetSenderName(c.SenderId),
                                          StatusName = Common.Common.GetEnumDescription(c.FaxingStatus),
                                          Status = c.FaxingStatus,
                                          TransactionType = TransactionServiceType.CashPickUp,
                                          Type = Agent.Models.Type.Transfer,
                                          AgentId = (int)c.PayingStaffId,
                                          SenderId = c.SenderId,
                                          RecipentId = c.RecipientId,
                                          IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,
                                      }).ToList();


            var KiiPayWalletTransfer = (from c in kiipay.ToList()
                                        join d in payingStaff on c.PayingStaffId equals d.Id
                                        select new AUXAgentRecentTransactionViewModel()
                                        {
                                            Id = c.Id,
                                            SendingCountry = _commonServices.getCountryNameFromCode(c.SendingCountry),
                                            Date = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                            TransactionDate = c.TransactionDate,
                                            AgentAccountNo = _commonServices.GetAgentStaffMFSCode((int)c.PayingStaffId),
                                            AgentName = c.PayingStaffName,
                                            SendingCurrency = _commonServices.getCurrencyCodeFromCountry(c.SendingCountry),
                                            ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                            ReceivingCountryCode = c.ReceivingCountry,
                                            Amount = c.TotalAmount,
                                            Identifier = c.ReceiptNumber,
                                            Receiver = c.KiiPayPersonalWalletInformation.FirstName + ' ' + c.KiiPayPersonalWalletInformation.MiddleName + ' ' + c.KiiPayPersonalWalletInformation.LastName,
                                            Sender = _commonServices.GetSenderName(c.FaxerId),
                                            StatusName = "No Staus Enum in TopUpSomeoneElseCardTransaction",
                                            SendingCountrycode = c.SendingCountry,
                                            TransactionType = TransactionServiceType.KiiPayWallet,
                                            Type = Agent.Models.Type.Transfer,
                                            AgentId = (int)c.PayingStaffId,
                                            SenderId = c.FaxerId,
                                            RecipentId = c.RecipientId,
                                            IsAwaitForApproval = false,

                                        }).ToList();

            var OtherMobilesWalletsTransfer = (from c in otherWallet.ToList()
                                               join d in payingStaff on c.PayingStaffId equals d.Id
                                               select new AUXAgentRecentTransactionViewModel()
                                               {
                                                   Id = c.Id,
                                                   SendingCountry = _commonServices.getCountryNameFromCode(c.SendingCountry),
                                                   SendingCountrycode = c.SendingCountry,
                                                   Date = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                                   TransactionDate = c.TransactionDate,
                                                   AgentAccountNo = _commonServices.GetAgentStaffMFSCode((int)c.PayingStaffId),
                                                   AgentName = c.PayingStaffName,
                                                   SendingCurrency = _commonServices.getCurrencyCodeFromCountry(c.SendingCountry),
                                                   ReceivingCountryCode = c.ReceivingCountry,
                                                   ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                                   Amount = c.TotalAmount,
                                                   Identifier = c.ReceiptNo,
                                                   Receiver = c.ReceiverName,
                                                   Sender = _commonServices.GetSenderName(c.SenderId),
                                                   StatusName = Common.Common.GetEnumDescription(c.Status),
                                                   TransactionType = TransactionServiceType.MobileWallet,
                                                   Type = Agent.Models.Type.Transfer,
                                                   SenderId = c.SenderId,
                                                   AgentId = (int)c.PayingStaffId,
                                                   RecipentId = c.RecipientId,
                                                   IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,

                                               }).ToList();
            var BankAccountDepositTransfer = (from c in bank.ToList()
                                              join d in payingStaff on c.PayingStaffId equals d.Id
                                              select new AUXAgentRecentTransactionViewModel()
                                              {
                                                  Id = c.Id,
                                                  SendingCountry = _commonServices.getCountryNameFromCode(c.SendingCountry),
                                                  SendingCountrycode = c.SendingCountry,
                                                  Date = c.TransactionDate.ToString("MMM-dd-yyyy"),
                                                  TransactionDate = c.TransactionDate,
                                                  AgentAccountNo = _commonServices.GetAgentStaffMFSCode((int)c.PayingStaffId),
                                                  AgentName = c.PayingStaffName,
                                                  SendingCurrency = _commonServices.getCurrencyCodeFromCountry(c.SendingCountry),
                                                  ReceivingCountry = _commonServices.getCountryNameFromCode(c.ReceivingCountry),
                                                  ReceivingCountryCode = c.ReceivingCountry,
                                                  Amount = c.TotalAmount,
                                                  Identifier = c.ReceiptNo,
                                                  Receiver = c.ReceiverName,
                                                  Sender = _commonServices.GetSenderName(c.SenderId),
                                                  StatusName = Common.Common.GetEnumDescription(c.Status),
                                                  TransactionType = TransactionServiceType.BankDeposit,
                                                  Type = Agent.Models.Type.Transfer,
                                                  SenderId = c.SenderId,
                                                  AgentId = (int)c.PayingStaffId,
                                                  RecipentId = c.RecipientId,
                                                  IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,
                                              }).ToList();

            var finalList = CashPickupTransfer.Concat(KiiPayWalletTransfer)
                .Concat(OtherMobilesWalletsTransfer).Concat(BankAccountDepositTransfer).ToList();

            if (!string.IsNullOrEmpty(Sender))
            {
                Sender = Sender.Trim();
                finalList = finalList.Where(x => x.Sender.ToLower().Contains(Sender.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(Receiver))
            {
                Receiver = Receiver.Trim();
                finalList = finalList.Where(x => x.Receiver.ToLower().Contains(Receiver.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Status))
            {
                Status = Status.Trim();
                finalList = finalList.Where(x => x.StatusName.ToLower() == Status.ToLower()).ToList();
            }
            return finalList.OrderByDescending(x => x.TransactionDate).ToList();
        }
        public List<AUXAgentRecentTransactionViewModel> getAuxAgentRecentTransactionList(SenderTransactionSearchParamVm searchParamVm)
        {
            DailyTransactionStatementServices dailyTransactionStatementServices = new DailyTransactionStatementServices();
            IQueryable<AgentStaffInformation> payingStaff = db.AgentStaffInformation.Where(x => x.Agent.IsAUXAgent == true);
            searchParamVm = dailyTransactionStatementServices.GetTrimmedSearchparam(searchParamVm);
            var transactionStatements = dailyTransactionStatementServices.GetTransactionHistoryOfSender(searchParamVm);
            var result = (from c in transactionStatements
                          select new AUXAgentRecentTransactionViewModel()
                          {
                              Id = c.Id,
                              SendingCountry = c.SendingCountryName,
                              SendingCountrycode = c.SendingCountry,
                              Date = c.FormatedDate,
                              TransactionDate = c.DateAndTime,
                              AgentAccountNo = c.AgentAccountNo,
                              AgentName = c.StaffName,
                              SendingCurrency = c.SendingCurrency,
                              ReceivingCountry = c.ReceivingCountryName,
                              ReceivingCountryCode = c.ReceivingCountry,
                              Amount = c.TotalAmount,
                              Identifier = c.TransactionIdentifier,
                              Receiver = c.ReceiverName,
                              Sender = c.SenderName,
                              StatusName = c.StatusName,
                              TransactionType = (TransactionServiceType)c.TransactionServiceType,
                              Type = Agent.Models.Type.Transfer,
                              SenderId = c.SenderId,
                              AgentId = (int)c.PayingStaffId,
                              RecipentId = c.RecipientId,
                              IsAwaitForApproval = c.IsAwaitForApproval,
                              TotalCount = c.TotalCount
                          }).ToList();
            return result;

        }

        internal RegisteredAUXAgentViewModel GetAuxAgentContactDetails(int agentId)
        {
            var staffInfo = db.AgentStaffInformation.Where(x => x.AgentId == agentId).FirstOrDefault();
            RegisteredAUXAgentViewModel vm = new RegisteredAUXAgentViewModel()
            {
                AgentName = staffInfo.FirstName + " " + staffInfo.MiddleName + " " + staffInfo.LastName,
                Address = staffInfo.Address1 + staffInfo.Address2,
                DOB = staffInfo.DateOfBirth.ToString("dd/MM/yyyy"),
                Gender = staffInfo.Gender.ToString(),
                Country = Common.Common.GetCountryName(staffInfo.Country),
                City = staffInfo.City,
                Email = staffInfo.EmailAddress,
                Telephone = staffInfo.PhoneNumber,
                Date = staffInfo.CreatedDate.HasValue == true ? staffInfo.CreatedDate.ToString() : ""
            };
            return vm;
        }

        public RegisteredAUXAgentViewModel GetRegisteredAUXAgentViewModel(int id)
        {
            agentInformation = db.AgentInformation.Where(x => x.IsAUXAgent == true);
            agentLogin = db.AgentLogin;
            GetAuxAgents();
            return RegisteredAuxAgents.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<RegisteredAUXAgentViewModel> GetRegisteredAuxAgents(string sendingCountry = "", string city = "", string date = "",
            string agentName = "", string accountNo = "", string loginCode = "",
            string telephone = "", string email = "")
        {
            agentInformation = db.AgentInformation.Where(x => x.IsAUXAgent == true);
            agentLogin = db.AgentLogin;

            SearchAuxAgentByParam(new SearchParamVM()
            {
                AccountNo = accountNo,
                AgentName = agentName,
                City = city,
                Date = date,
                Email = email,
                LoginCode = loginCode,
                SendingCountry = sendingCountry,
                Telephone = telephone,
            });
            GetAuxAgents();
            return RegisteredAuxAgents;
        }

        private void GetAuxAgents()
        {
            RegisteredAuxAgents = (from c in agentInformation.Where(x => x.IsAUXAgent == true).ToList()
                                   join d in agentLogin on c.Id equals d.AgentId
                                   select new RegisteredAUXAgentViewModel()
                                   {
                                       Id = c.Id,
                                       AccountNo = c.AccountNo,
                                       Address = c.Address1,
                                       AgentName = c.Name,
                                       BusinessType = c.TypeOfBusiness == null ? "" : Common.Common.GetEnumDescription(c.TypeOfBusiness),
                                       TypeOfBusiness = c.TypeOfBusiness.HasValue == true ? c.TypeOfBusiness.Value : BusinessType.Non,
                                       Country = _commonServices.getCountryNameFromCode(c.CountryCode),
                                       CountryCode = c.CountryCode,
                                       City = c.City,
                                       Email = c.Email,
                                       LoginCode = d.LoginCode,
                                       Telephone = c.PhoneNumber,
                                       StatusName = Enum.GetName(typeof(AgentStatus), c.AgentStatus),
                                       Date = c.CreatedDate.ToString(),
                                       CreationDate = c.CreatedDate,
                                       AgentStatus = c.AgentStatus
                                   }).OrderByDescending(x => x.CreationDate).ToList();
        }
        private void SearchAuxAgentByParam(SearchParamVM searchParam)
        {
            if (!string.IsNullOrEmpty(searchParam.SendingCountry))
            {
                agentInformation = agentInformation.Where(x => x.CountryCode == searchParam.SendingCountry);
            }
            if (!string.IsNullOrEmpty(searchParam.City))
            {
                agentInformation = agentInformation.Where(x => x.City == searchParam.City);
            }
            if (!string.IsNullOrEmpty(searchParam.AgentName))
            {
                searchParam.AgentName = searchParam.AgentName.Trim();
                agentInformation = agentInformation.Where(x => x.Name.ToLower().Contains(searchParam.AgentName.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.AccountNo))
            {
                searchParam.AccountNo = searchParam.AccountNo.Trim();
                agentInformation = agentInformation.Where(x => x.AccountNo.ToLower().Contains(searchParam.AccountNo.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.LoginCode))
            {
                searchParam.LoginCode = searchParam.LoginCode.Trim();
                agentLogin = agentLogin.Where(x => x.LoginCode.ToLower().Contains(searchParam.LoginCode.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Telephone))
            {
                searchParam.Telephone = searchParam.Telephone.Trim();
                agentInformation = agentInformation.Where(x => x.PhoneNumber.ToLower().Contains(searchParam.Telephone.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Email))
            {
                searchParam.Email = searchParam.Email.Trim();
                agentInformation = agentInformation.Where(x => x.Email.ToLower().Contains(searchParam.Email.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Date))
            {
                string[] DateString = searchParam.Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                agentInformation = agentInformation.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);
            }


        }
    }
}